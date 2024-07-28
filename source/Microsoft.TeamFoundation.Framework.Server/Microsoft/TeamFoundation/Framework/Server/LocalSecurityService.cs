// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalSecurityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Messages;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LocalSecurityService : 
    VssBaseService,
    ITeamFoundationSecurityService,
    IVssFrameworkService,
    ITeamFoundationSecurityService<LocalSecurityNamespace>
  {
    private Guid m_serviceHostInstanceId;
    private long m_completedRefreshId;
    private long m_outstandingRefreshId;
    private int m_sequenceId;
    private long m_templateSequenceId;
    private ILockName m_namespacesLockName;
    private Dictionary<Guid, LocalSecurityNamespace> m_localNamespaces = new Dictionary<Guid, LocalSecurityNamespace>();
    private Dictionary<Guid, RemoteSecurityNamespaceDescription> m_remoteNamespaces = new Dictionary<Guid, RemoteSecurityNamespaceDescription>();
    private Dictionary<Guid, IVssSecurityNamespace> m_shimNamespaces = new Dictionary<Guid, IVssSecurityNamespace>();
    private IDisposableReadOnlyList<ISecurityNamespaceExtension> m_extensions;
    private IDictionary<string, ISecurityNamespaceExtension> m_extensionsMap;
    private IDisposableReadOnlyList<ISecurityDataspaceMapper> m_dataspaceMappers;
    private IDictionary<Guid, ISecurityDataspaceMapper> m_dataspaceMappersMap;
    private ILockName m_messageBusLockName;
    private bool m_messageBusTaskQueued;
    private List<SecurityMessage2> m_messageBusQueue = new List<SecurityMessage2>();
    private SecurityNamespaceTemplateService m_snts;
    private INotificationRegistration m_namespaceRegistration;
    private readonly Guid RepositorySecurityNamespaceGuid = new Guid("A39371CF-0841-4c16-BBD3-276E341BC052");
    private readonly Guid RepositorySecurity2NamespaceGuid = new Guid("3C15A8B7-AF1A-45C2-AA97-2CB97078332E");
    private const string c_area = "Security";
    private const string c_layer = "LocalSecurityService";
    private const int c_imsBatchSizeLimit = 100000;

    internal LocalSecurityService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostInstanceId = systemRequestContext.ServiceHost.InstanceId;
      this.m_namespacesLockName = this.CreateLockName(systemRequestContext, "namespaces");
      this.m_messageBusLockName = this.CreateLockName(systemRequestContext, "messagebus");
      this.m_sequenceId = -1;
      this.m_completedRefreshId = 0L;
      this.m_outstandingRefreshId = 1L;
      this.m_templateSequenceId = -1L;
      this.m_namespaceRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityNamespaceChanged, new SqlNotificationCallback(this.OnSecurityNamespaceChanged), true, false);
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_extensions = systemRequestContext.GetExtensions<ISecurityNamespaceExtension>();
        this.m_extensionsMap = (IDictionary<string, ISecurityNamespaceExtension>) this.m_extensions.ToDictionary<ISecurityNamespaceExtension, string>((Func<ISecurityNamespaceExtension, string>) (s => s.GetType().FullName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_dataspaceMappers = systemRequestContext.GetExtensions<ISecurityDataspaceMapper>();
        this.m_dataspaceMappersMap = (IDictionary<Guid, ISecurityDataspaceMapper>) this.m_dataspaceMappers.ToDictionary<ISecurityDataspaceMapper, Guid>((Func<ISecurityDataspaceMapper, Guid>) (s => s.NamespaceId));
        this.m_snts = systemRequestContext.GetService<SecurityNamespaceTemplateService>();
      }
      else
      {
        IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
        LocalSecurityService service = context.GetService<LocalSecurityService>();
        this.m_extensionsMap = service.m_extensionsMap;
        this.m_dataspaceMappersMap = service.m_dataspaceMappersMap;
        this.m_snts = context.GetService<SecurityNamespaceTemplateService>();
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_namespaceRegistration.Unregister(systemRequestContext);
      if (this.m_extensions != null)
      {
        this.m_extensions.Dispose();
        this.m_extensions = (IDisposableReadOnlyList<ISecurityNamespaceExtension>) null;
      }
      if (this.m_dataspaceMappers == null)
        return;
      this.m_dataspaceMappers.Dispose();
      this.m_dataspaceMappers = (IDisposableReadOnlyList<ISecurityDataspaceMapper>) null;
    }

    public LocalSecurityNamespace CreateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      requestContext.TraceEnter(56010, "Security", nameof (LocalSecurityService), nameof (CreateSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
        description.Validate(requestContext);
        int num = this.EnsureNamespacesLoaded(requestContext);
        try
        {
          using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>("Default"))
            component.CreateSecurityNamespace(SecurityComponent.LocalNamespaceDescription.FromPublicType(description));
        }
        catch
        {
          using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
            Interlocked.Increment(ref this.m_outstandingRefreshId);
          throw;
        }
        LocalSecurityNamespace securityNamespace1 = new LocalSecurityNamespace(requestContext.Elevate(), description, this.GetExtensionForExtensionType(description.ExtensionType), this.TryGetDataspaceMapperForNamespace(requestContext, description.NamespaceId));
        using (requestContext.AcquireWriterLock(this.m_namespacesLockName))
        {
          LocalSecurityNamespace securityNamespace2;
          if (this.m_localNamespaces.TryGetValue(securityNamespace1.Description.NamespaceId, out securityNamespace2) && securityNamespace2.Description.IsProjected)
          {
            requestContext.TraceAlways(56012, TraceLevel.Info, "Security", nameof (LocalSecurityService), string.Format("Security namespace '{0}' already exists as a template. Template win.", (object) securityNamespace1.Description.NamespaceId));
            return securityNamespace2;
          }
          if (num == this.m_sequenceId)
          {
            this.m_localNamespaces[securityNamespace1.Description.NamespaceId] = securityNamespace1;
            if ((securityNamespace1.Description.NamespaceId.Equals(this.RepositorySecurityNamespaceGuid) || securityNamespace1.Description.NamespaceId.Equals(this.RepositorySecurity2NamespaceGuid)) && this.m_localNamespaces.ContainsKey(this.RepositorySecurity2NamespaceGuid))
              this.ReplaceRepositorySecurityNamespaceWithShim(this.m_localNamespaces, this.m_shimNamespaces);
            ++this.m_sequenceId;
          }
          else
            ++this.m_outstandingRefreshId;
        }
        if (description.IsRemotable)
          this.EnqueueCacheInvalidation(requestContext);
        return securityNamespace1;
      }
      catch (Exception ex)
      {
        TraceLevel level = TraceLevel.Error;
        if (ex is SecurityNamespaceAlreadyExistsException)
          level = TraceLevel.Verbose;
        requestContext.TraceException(56011, level, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56014, "Security", nameof (LocalSecurityService), nameof (CreateSecurityNamespace));
      }
    }

    IVssSecurityNamespace ITeamFoundationSecurityService.CreateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      return (IVssSecurityNamespace) this.CreateSecurityNamespace(requestContext, description);
    }

    public void CreateRemoteSecurityNamespace(
      IVssRequestContext requestContext,
      RemoteSecurityNamespaceDescription description)
    {
      this.CreateRemoteSecurityNamespace(requestContext, description, "Default");
    }

    internal void CreateRemoteSecurityNamespace(
      IVssRequestContext requestContext,
      RemoteSecurityNamespaceDescription description,
      string dataspace)
    {
      requestContext.TraceEnter(56110, "Security", nameof (LocalSecurityService), nameof (CreateRemoteSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<RemoteSecurityNamespaceDescription>(description, nameof (description));
        ArgumentUtility.CheckForEmptyGuid(description.NamespaceId, "description.NamespaceId");
        int num = this.EnsureNamespacesLoaded(requestContext);
        try
        {
          using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>(dataspace))
            component.CreateRemoteSecurityNamespace(SecurityComponent.RemoteNamespaceDescription.FromPublicType(description));
        }
        catch
        {
          using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
            Interlocked.Increment(ref this.m_outstandingRefreshId);
          throw;
        }
        using (requestContext.AcquireWriterLock(this.m_namespacesLockName))
        {
          RemoteSecurityNamespaceDescription namespaceDescription;
          if (this.m_remoteNamespaces.TryGetValue(description.NamespaceId, out namespaceDescription) && namespaceDescription.IsProjected)
          {
            requestContext.TraceAlways(56016, TraceLevel.Info, "Security", nameof (LocalSecurityService), string.Format("Remote security namespace '{0}' already exists as a template. Template win.", (object) description.NamespaceId));
            return;
          }
          if (num == this.m_sequenceId)
          {
            this.m_remoteNamespaces[description.NamespaceId] = description;
            ++this.m_sequenceId;
          }
          else
            ++this.m_outstandingRefreshId;
        }
        this.RaiseRemotePointersChanged(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56111, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56112, "Security", nameof (LocalSecurityService), nameof (CreateRemoteSecurityNamespace));
      }
    }

    public LocalSecurityNamespace UpdateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      requestContext.TraceEnter(56015, "Security", nameof (LocalSecurityService), nameof (UpdateSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
        description.Validate(requestContext);
        int num1 = this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          LocalSecurityNamespace securityNamespace;
          if (!this.m_localNamespaces.TryGetValue(description.NamespaceId, out securityNamespace))
            throw new InvalidSecurityNamespaceException(description.NamespaceId);
          if (securityNamespace.Description.IsProjected)
          {
            requestContext.Trace(56017, TraceLevel.Error, "Security", nameof (LocalSecurityService), string.Format("Security namespace '{0}' already exists as a template. Update skipped.", (object) securityNamespace.Description.NamespaceId));
            return securityNamespace;
          }
          int num2 = securityNamespace.Description.IsRemotable ? 1 : 0;
        }
        try
        {
          using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>("Default"))
            component.UpdateSecurityNamespace(SecurityComponent.LocalNamespaceDescription.FromPublicType(description));
        }
        catch
        {
          using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
            Interlocked.Increment(ref this.m_outstandingRefreshId);
          throw;
        }
        LocalSecurityNamespace securityNamespace1 = new LocalSecurityNamespace(requestContext.Elevate(), description, this.GetExtensionForExtensionType(description.ExtensionType), this.TryGetDataspaceMapperForNamespace(requestContext, description.NamespaceId));
        using (requestContext.AcquireWriterLock(this.m_namespacesLockName))
        {
          LocalSecurityNamespace securityNamespace2;
          if (!this.m_localNamespaces.TryGetValue(description.NamespaceId, out securityNamespace2))
            throw new InvalidSecurityNamespaceException(description.NamespaceId);
          if (securityNamespace2.Description.IsProjected)
          {
            requestContext.Trace(56017, TraceLevel.Error, "Security", nameof (LocalSecurityService), string.Format("Security namespace '{0}' already exists as a template. Update skipped.", (object) securityNamespace2.Description.NamespaceId));
            return securityNamespace2;
          }
          if (num1 == this.m_sequenceId)
          {
            this.m_localNamespaces[securityNamespace1.Description.NamespaceId] = securityNamespace1;
            ++this.m_sequenceId;
          }
          else
            ++this.m_outstandingRefreshId;
        }
        return securityNamespace1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56018, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56019, "Security", nameof (LocalSecurityService), nameof (UpdateSecurityNamespace));
      }
    }

    IVssSecurityNamespace ITeamFoundationSecurityService.UpdateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      return (IVssSecurityNamespace) this.UpdateSecurityNamespace(requestContext, description);
    }

    public bool DeleteSecurityNamespace(IVssRequestContext requestContext, Guid namespaceId)
    {
      requestContext.TraceEnter(56020, "Security", nameof (LocalSecurityService), nameof (DeleteSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        int num1 = this.EnsureNamespacesLoaded(requestContext);
        LocalSecurityNamespace securityNamespace;
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          if (this.m_localNamespaces.TryGetValue(namespaceId, out securityNamespace))
          {
            int num2 = securityNamespace.Description.IsRemotable ? 1 : 0;
          }
          else if (!this.m_remoteNamespaces.ContainsKey(namespaceId))
            return false;
        }
        securityNamespace?.RemoveAllAccessControlLists(requestContext);
        try
        {
          using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>("Default"))
            component.DeleteSecurityNamespace(namespaceId);
        }
        catch
        {
          using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
            Interlocked.Increment(ref this.m_outstandingRefreshId);
          throw;
        }
        bool flag1;
        bool flag2;
        using (requestContext.AcquireWriterLock(this.m_namespacesLockName))
        {
          if (num1 == this.m_sequenceId)
          {
            flag1 = this.m_localNamespaces.Remove(namespaceId);
            flag2 = this.m_remoteNamespaces.Remove(namespaceId);
            ++this.m_sequenceId;
          }
          else
          {
            flag1 = this.m_localNamespaces.ContainsKey(namespaceId);
            flag2 = this.m_remoteNamespaces.ContainsKey(namespaceId);
            ++this.m_outstandingRefreshId;
          }
        }
        if (flag2)
          this.RaiseRemotePointersChanged(requestContext);
        return flag1 | flag2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56011, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56014, "Security", nameof (LocalSecurityService), "CreateSecurityNamespace");
      }
    }

    public LocalSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      requestContext.TraceEnter(56020, "Security", nameof (LocalSecurityService), nameof (GetSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        LocalSecurityNamespace securityNamespace;
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          this.m_localNamespaces.TryGetValue(namespaceId, out securityNamespace);
        return securityNamespace;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56023, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56024, "Security", nameof (LocalSecurityService), nameof (GetSecurityNamespace));
      }
    }

    IVssSecurityNamespace ITeamFoundationSecurityService.GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      return (IVssSecurityNamespace) this.GetSecurityNamespace(requestContext, namespaceId) ?? this.GetShimSecurityNamespace(requestContext, namespaceId);
    }

    public IList<LocalSecurityNamespace> GetSecurityNamespaces(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56025, "Security", nameof (LocalSecurityService), nameof (GetSecurityNamespaces));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          return (IList<LocalSecurityNamespace>) new List<LocalSecurityNamespace>((IEnumerable<LocalSecurityNamespace>) this.m_localNamespaces.Values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56028, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56029, "Security", nameof (LocalSecurityService), nameof (GetSecurityNamespaces));
      }
    }

    IList<IVssSecurityNamespace> ITeamFoundationSecurityService.GetSecurityNamespaces(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56025, "Security", nameof (LocalSecurityService), "GetSecurityNamespaces");
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          List<IVssSecurityNamespace> securityNamespaces = new List<IVssSecurityNamespace>((IEnumerable<IVssSecurityNamespace>) this.m_localNamespaces.Values);
          foreach (IVssSecurityNamespace securityNamespace in this.m_shimNamespaces.Values)
          {
            if (!this.m_localNamespaces.ContainsKey(securityNamespace.Description.NamespaceId))
              securityNamespaces.Add(securityNamespace);
          }
          return (IList<IVssSecurityNamespace>) securityNamespaces;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56028, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56029, "Security", nameof (LocalSecurityService), "GetSecurityNamespaces");
      }
    }

    public IVssSecurityNamespace GetShimSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      requestContext.TraceEnter(56070, "Security", nameof (LocalSecurityService), nameof (GetShimSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        IVssSecurityNamespace securityNamespace;
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          this.m_shimNamespaces.TryGetValue(namespaceId, out securityNamespace);
        return securityNamespace;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56073, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56074, "Security", nameof (LocalSecurityService), nameof (GetShimSecurityNamespace));
      }
    }

    public IList<IVssSecurityNamespace> GetShimSecurityNamespaces(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56075, "Security", nameof (LocalSecurityService), nameof (GetShimSecurityNamespaces));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          return (IList<IVssSecurityNamespace>) new List<IVssSecurityNamespace>((IEnumerable<IVssSecurityNamespace>) this.m_shimNamespaces.Values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56078, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56079, "Security", nameof (LocalSecurityService), nameof (GetShimSecurityNamespaces));
      }
    }

    public RemoteSecurityNamespaceDescription GetRemoteSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      requestContext.TraceEnter(56120, "Security", nameof (LocalSecurityService), nameof (GetRemoteSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          RemoteSecurityNamespaceDescription securityNamespace;
          if (this.m_remoteNamespaces.TryGetValue(namespaceId, out securityNamespace))
            return securityNamespace;
        }
        return (RemoteSecurityNamespaceDescription) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56121, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56122, "Security", nameof (LocalSecurityService), nameof (GetRemoteSecurityNamespace));
      }
    }

    public IList<RemoteSecurityNamespaceDescription> GetRemoteSecurityNamespaces(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56130, "Security", nameof (LocalSecurityService), nameof (GetRemoteSecurityNamespaces));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          return (IList<RemoteSecurityNamespaceDescription>) new List<RemoteSecurityNamespaceDescription>((IEnumerable<RemoteSecurityNamespaceDescription>) this.m_remoteNamespaces.Values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56131, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56132, "Security", nameof (LocalSecurityService), nameof (GetRemoteSecurityNamespaces));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity EnsureIdentityIsKnown(
      IVssRequestContext requestContext,
      IdentityDescriptor identity)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identity, nameof (identity));
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity;
      try
      {
        readIdentity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identity
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null || !service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, readIdentity.Descriptor))
        {
          service.AddMemberToGroup(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identity);
          readIdentity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identity
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
        }
        service.MapToWellKnownIdentifier(readIdentity);
      }
      catch (Exception ex)
      {
        if (ex is IdentityNotFoundException)
        {
          readIdentity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identity
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          service.MapToWellKnownIdentifier(readIdentity);
          if (readIdentity == null)
            throw new IdentityNotFoundException(identity);
        }
        else
          throw;
      }
      return readIdentity;
    }

    public void RemoveIdentityACEs(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      requestContext.TraceEnter(56035, "Security", nameof (LocalSecurityService), nameof (RemoveIdentityACEs));
      try
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identities, nameof (identities));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
          ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
        this.ValidateRequestContext(requestContext);
        this.EnsureNamespacesLoaded(requestContext);
        foreach (LocalSecurityNamespace securityNamespace in (IEnumerable<LocalSecurityNamespace>) this.GetSecurityNamespaces(requestContext))
          securityNamespace.RemoveIdentityACEs(requestContext, identities);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56038, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56039, "Security", nameof (LocalSecurityService), nameof (RemoveIdentityACEs));
      }
    }

    public event IVssRequestContextEventHandler RemotePointersChanged;

    private static void ValidateSecurityActions(SecurityNamespaceDescription description)
    {
      if (description.Actions == null)
        return;
      int num = 0;
      foreach (ActionDefinition action in description.Actions)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(action.Name, "action.Name");
        if (action.Bit == 0)
          throw new InvalidSecurityNamespaceDescriptionException(FrameworkResources.SecurityNamespaceHasActionWith0Bit((object) description.Name, (object) description.NamespaceId, (object) action.Name));
        if ((num & action.Bit) != 0)
          throw new InvalidSecurityNamespaceDescriptionException(FrameworkResources.SecurityNamespaceHasDuplicatedAction((object) description.Name, (object) action.Bit));
        num |= action.Bit;
      }
    }

    private static void ValidateDataspaceCategory(
      IVssRequestContext requestContext,
      string dataspaceCategory)
    {
      requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, dataspaceCategory, Guid.Empty, true);
    }

    private void EnqueueCacheInvalidation(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      SecurityMessage2 message = new SecurityMessage2()
      {
        ServiceOwner = requestContext.ServiceInstanceType(),
        InstanceId = requestContext.ServiceHost.InstanceId,
        NamespaceId = Guid.Empty,
        AclStoreId = Guid.Empty,
        NewSequenceId = Array.Empty<long>()
      };
      this.EnqueueCacheInvalidation(requestContext, message);
    }

    internal void EnqueueCacheInvalidation(
      IVssRequestContext requestContext,
      SecurityMessage2 message)
    {
      if (requestContext.ServiceHost.IsCreating())
      {
        requestContext.Trace(816222217, TraceLevel.Info, "Security", nameof (LocalSecurityService), "Skipping service bus message on namespace {0} since host {1} of type {2} is being created.", (object) message.NamespaceId, (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.HostType);
      }
      else
      {
        bool flag;
        if (requestContext.RootContext.TryGetItem<bool>("SkipSendingSecurityServiceBusMessage", out flag) & flag)
        {
          requestContext.Trace(816222218, TraceLevel.Info, "Security", nameof (LocalSecurityService), "Skipping service bus message on namespace {0} host {1} since SkipSendingSecurityServiceBusMessage is set to true.", (object) message.NamespaceId, (object) requestContext.ServiceHost.InstanceId);
        }
        else
        {
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
          TimeSpan timeSpan = TimeSpan.FromSeconds((double) context.GetService<SecuritySettingsService>().Settings.MessageBusTaskDelayInSeconds);
          TeamFoundationTaskService service = context.GetService<TeamFoundationTaskService>();
          using (requestContext.Lock(this.m_messageBusLockName))
          {
            this.m_messageBusQueue.Add(message);
            if (this.m_messageBusTaskQueued)
              return;
            service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishMessageBusTask), (object) null, DateTime.UtcNow + timeSpan, 0));
            this.m_messageBusTaskQueued = true;
          }
        }
      }
    }

    private void PublishMessageBusTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(56607, "Security", nameof (LocalSecurityService), nameof (PublishMessageBusTask));
      try
      {
        List<SecurityMessage2> securityMessage2List = (List<SecurityMessage2>) null;
        using (requestContext.Lock(this.m_messageBusLockName))
        {
          securityMessage2List = this.m_messageBusQueue;
          this.m_messageBusQueue = new List<SecurityMessage2>();
          this.m_messageBusTaskQueued = false;
        }
        SecurityMessageHelpers.CollapseMessages((IList<SecurityMessage2>) securityMessage2List);
        bool flag = requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsTracing(56610, TraceLevel.Info, "Security", nameof (LocalSecurityService));
        StringBuilder stringBuilder = (StringBuilder) null;
        if (flag)
        {
          stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("Published security invalidation messages to service bus");
          foreach (SecurityMessage2 securityMessage2 in securityMessage2List)
            stringBuilder.AppendLine(string.Format("( {0}, {1}, {2}, {3}, {4} )", (object) securityMessage2.ServiceOwner, (object) securityMessage2.InstanceId, (object) securityMessage2.NamespaceId, (object) securityMessage2.AclStoreId, (object) new TokenStoreSequenceId(securityMessage2.NewSequenceId)));
        }
        List<object> objectList = new List<object>((IEnumerable<object>) securityMessage2List);
        for (int index = 0; index < objectList.Count; ++index)
        {
          SecurityMessage2 securityMessage2 = (SecurityMessage2) objectList[index];
          if (Guid.Empty == securityMessage2.NamespaceId)
            objectList[index] = (object) new SecurityMessage()
            {
              ServiceOwner = securityMessage2.ServiceOwner,
              InstanceId = securityMessage2.InstanceId,
              NamespaceId = Guid.Empty,
              NewSequenceId = 0
            };
          else if (WellKnownAclStores.User == securityMessage2.AclStoreId && 1 == securityMessage2.NewSequenceId.Length && securityMessage2.NewSequenceId[0] <= (long) int.MaxValue)
            objectList[index] = (object) new SecurityMessage()
            {
              ServiceOwner = securityMessage2.ServiceOwner,
              InstanceId = securityMessage2.InstanceId,
              NamespaceId = securityMessage2.NamespaceId,
              NewSequenceId = checked ((int) securityMessage2.NewSequenceId[0])
            };
        }
        if (objectList.Count <= 0)
          return;
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Security", objectList.ToArray(), false, false);
        if (!flag)
          return;
        requestContext.TraceAlways(56610, TraceLevel.Info, "Security", nameof (LocalSecurityService), stringBuilder.ToString());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56608, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56609, "Security", nameof (LocalSecurityService), nameof (PublishMessageBusTask));
      }
    }

    private void OnSecurityNamespaceChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56045, "Security", nameof (LocalSecurityService), nameof (OnSecurityNamespaceChanged));
      try
      {
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
          Interlocked.Increment(ref this.m_outstandingRefreshId);
        this.RaiseRemotePointersChanged(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56048, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56049, "Security", nameof (LocalSecurityService), nameof (OnSecurityNamespaceChanged));
      }
    }

    private void RaiseRemotePointersChanged(IVssRequestContext requestContext)
    {
      IVssRequestContextEventHandler remotePointersChanged = this.RemotePointersChanged;
      if (remotePointersChanged == null)
        return;
      remotePointersChanged((object) this, new IVssRequestContextEventArgs(requestContext.Elevate()));
    }

    private int EnsureNamespacesLoaded(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56050, "Security", nameof (LocalSecurityService), nameof (EnsureNamespacesLoaded));
      try
      {
        long sequenceId = this.m_snts.SequenceId;
        long num = Interlocked.Read(ref this.m_outstandingRefreshId);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          if (num <= this.m_completedRefreshId)
          {
            if (sequenceId <= this.m_templateSequenceId)
              return this.m_sequenceId;
          }
        }
        Dictionary<Guid, LocalSecurityNamespace> localNamespaces = new Dictionary<Guid, LocalSecurityNamespace>();
        Dictionary<Guid, IVssSecurityNamespace> shimNamespaces = new Dictionary<Guid, IVssSecurityNamespace>();
        Dictionary<Guid, RemoteSecurityNamespaceDescription> dictionary = new Dictionary<Guid, RemoteSecurityNamespaceDescription>();
        if (!requestContext.IsVirtualServiceHost())
        {
          List<SecurityComponent.NamespaceDescription> namespaceDescriptionList;
          using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>("Default"))
            namespaceDescriptionList = new List<SecurityComponent.NamespaceDescription>(component.QuerySecurityNamespaces());
          foreach (SecurityComponent.NamespaceDescription namespaceDescription in namespaceDescriptionList)
          {
            if (namespaceDescription is SecurityComponent.LocalNamespaceDescription)
            {
              SecurityNamespaceDescription publicType = ((SecurityComponent.LocalNamespaceDescription) namespaceDescription).ToPublicType();
              localNamespaces[namespaceDescription.NamespaceId] = new LocalSecurityNamespace(requestContext.Elevate(), publicType, this.GetExtensionForExtensionType(publicType.ExtensionType), this.TryGetDataspaceMapperForNamespace(requestContext, publicType.NamespaceId));
            }
            else if (namespaceDescription is SecurityComponent.RemoteNamespaceDescription)
              dictionary[namespaceDescription.NamespaceId] = ((SecurityComponent.RemoteNamespaceDescription) namespaceDescription).ToPublicType();
          }
        }
        foreach (NamespaceDescription namespaceDescription1 in this.m_snts.GetNamespaceTemplatesByHostType(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.HostType, out sequenceId).Values)
        {
          if (namespaceDescription1 is SecurityNamespaceDescription description)
          {
            localNamespaces[description.NamespaceId] = new LocalSecurityNamespace(requestContext.Elevate(), description, this.GetExtensionForExtensionType(description.ExtensionType), this.TryGetDataspaceMapperForNamespace(requestContext, description.NamespaceId));
          }
          else
          {
            RemoteSecurityNamespaceDescription namespaceDescription2 = (RemoteSecurityNamespaceDescription) namespaceDescription1;
            dictionary[namespaceDescription2.NamespaceId] = namespaceDescription2;
          }
        }
        if (localNamespaces.ContainsKey(this.RepositorySecurity2NamespaceGuid))
          this.ReplaceRepositorySecurityNamespaceWithShim(localNamespaces, shimNamespaces);
        using (requestContext.AcquireReaderLock(this.m_namespacesLockName))
        {
          if (num <= this.m_completedRefreshId)
          {
            if (sequenceId <= this.m_templateSequenceId)
              return this.m_sequenceId;
          }
        }
        using (requestContext.AcquireWriterLock(this.m_namespacesLockName))
        {
          if (num <= this.m_completedRefreshId && sequenceId <= this.m_templateSequenceId)
            return this.m_sequenceId;
          this.m_localNamespaces = localNamespaces;
          this.m_shimNamespaces = shimNamespaces;
          this.m_remoteNamespaces = dictionary;
          this.m_completedRefreshId = num;
          this.m_templateSequenceId = sequenceId;
          ++this.m_sequenceId;
        }
        this.RaiseRemotePointersChanged(requestContext);
        return this.m_sequenceId;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56053, "Security", nameof (LocalSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56054, "Security", nameof (LocalSecurityService), nameof (EnsureNamespacesLoaded));
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostInstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.SecurityServiceRequestContextHostMessage((object) this.m_serviceHostInstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    internal ISecurityNamespaceExtension GetExtensionForExtensionType(string extensionType)
    {
      ISecurityNamespaceExtension instance;
      if (extensionType == null || !this.m_extensionsMap.TryGetValue(extensionType, out instance))
        instance = (ISecurityNamespaceExtension) DefaultSecurityNamespaceExtension.Instance;
      return instance;
    }

    private ISecurityDataspaceMapper TryGetDataspaceMapperForNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      ISecurityDataspaceMapper mapperForNamespace;
      if (!this.m_dataspaceMappersMap.TryGetValue(namespaceId, out mapperForNamespace) || (mapperForNamespace.SupportedHostTypes & requestContext.ServiceHost.HostType) == TeamFoundationHostType.Unknown)
        mapperForNamespace = (ISecurityDataspaceMapper) null;
      return mapperForNamespace;
    }

    private void ReplaceRepositorySecurityNamespaceWithShim(
      Dictionary<Guid, LocalSecurityNamespace> localNamespaces,
      Dictionary<Guid, IVssSecurityNamespace> shimNamespaces)
    {
      Type type = Type.GetType("Microsoft.TeamFoundation.VersionControl.Server.RepositoryShimSecurityNamespace, Microsoft.TeamFoundation.VersionControl.Server");
      if (!(type != (Type) null))
        return;
      LocalSecurityNamespace localNamespace = localNamespaces[this.RepositorySecurity2NamespaceGuid];
      IVssSecurityNamespace instance = (IVssSecurityNamespace) Activator.CreateInstance(type, (object) localNamespace);
      localNamespaces.Remove(this.RepositorySecurityNamespaceGuid);
      shimNamespaces.Add(this.RepositorySecurityNamespaceGuid, instance);
    }

    internal void RemoveDataspacedACEs(IVssRequestContext requestContext, Guid dataspaceIdentifier)
    {
      IList<LocalSecurityNamespace> securityNamespaces = this.GetSecurityNamespaces(requestContext);
      IEnumerable<Tuple<Guid, string>> namespaces = securityNamespaces.Select<LocalSecurityNamespace, Tuple<Guid, string>>((Func<LocalSecurityNamespace, Tuple<Guid, string>>) (sn => new Tuple<Guid, string>(sn.NamespaceId, sn.Description.DataspaceCategory)));
      IList<Guid> list;
      using (SecurityComponent component = requestContext.CreateComponent<SecurityComponent>())
        list = (IList<Guid>) component.RemoveDataspacedACEs(namespaces, dataspaceIdentifier).ToList<Guid>();
      foreach (Guid guid in securityNamespaces.Where<LocalSecurityNamespace>((Func<LocalSecurityNamespace, bool>) (x => x.Description.IsRemotable)).Join<LocalSecurityNamespace, Guid, Guid, LocalSecurityNamespace>((IEnumerable<Guid>) list, (Func<LocalSecurityNamespace, Guid>) (o => o.NamespaceId), (Func<Guid, Guid>) (i => i), (Func<LocalSecurityNamespace, Guid, LocalSecurityNamespace>) ((o, i) => o)).Select<LocalSecurityNamespace, Guid>((Func<LocalSecurityNamespace, Guid>) (x => x.NamespaceId)).ToList<Guid>())
        this.EnqueueCacheInvalidation(requestContext, new SecurityMessage2()
        {
          ServiceOwner = requestContext.ServiceInstanceType(),
          InstanceId = requestContext.ServiceHost.InstanceId,
          NamespaceId = guid,
          AclStoreId = WellKnownAclStores.User,
          NewSequenceId = new long[1]{ -2L }
        });
    }
  }
}
