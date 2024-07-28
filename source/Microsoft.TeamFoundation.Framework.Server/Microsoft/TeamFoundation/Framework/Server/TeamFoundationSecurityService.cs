// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSecurityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.Security.Messages;
using Microsoft.VisualStudio.Services.Security.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationSecurityService : 
    VssBaseService,
    ITeamFoundationSecurityService,
    IVssFrameworkService,
    ITeamFoundationSecurityService<IVssSecurityNamespace>
  {
    private Guid m_serviceHostInstanceId;
    private LocalSecurityService m_localSecurityService;
    private TeamFoundationTask m_namespaceReloadTask;
    private bool m_cacheFresh;
    private int m_cacheVersion = -1;
    private ILockName m_lockName;
    private IDictionary<Guid, RemoteSecurityNamespaceDescription> m_pointers;
    private Dictionary<Guid, IRemoteSecurityNamespace> m_namespaces = new Dictionary<Guid, IRemoteSecurityNamespace>();
    private INotificationRegistration m_remoteNamespacesRegistration;
    private const string c_area = "Security";
    private const string c_layer = "TeamFoundationSecurityService";

    internal TeamFoundationSecurityService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostInstanceId = systemRequestContext.ServiceHost.InstanceId;
      this.m_lockName = this.CreateLockName(systemRequestContext, "remotenamespaces");
      this.m_localSecurityService = systemRequestContext.GetService<LocalSecurityService>();
      this.m_localSecurityService.RemotePointersChanged += new IVssRequestContextEventHandler(this.OnRemotePointersChanged);
      this.m_remoteNamespacesRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.RemoteSecurityNamespacesChanged, new SqlNotificationCallback(this.OnRemoteSecurityNamespacesChanged), false, false);
      int lifetimeInMilliseconds = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings.NamespaceMetadataCacheLifetimeInMilliseconds;
      this.ResetNamespaceReloadTask(systemRequestContext, lifetimeInMilliseconds);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_remoteNamespacesRegistration.Unregister(systemRequestContext);
      if (this.m_localSecurityService != null)
        this.m_localSecurityService.RemotePointersChanged -= new IVssRequestContextEventHandler(this.OnRemotePointersChanged);
      this.ResetNamespaceReloadTask(systemRequestContext, 0);
    }

    public IVssSecurityNamespace CreateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      return (IVssSecurityNamespace) this.m_localSecurityService.CreateSecurityNamespace(requestContext, description);
    }

    public IVssSecurityNamespace UpdateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description)
    {
      return (IVssSecurityNamespace) this.m_localSecurityService.UpdateSecurityNamespace(requestContext, description);
    }

    public bool DeleteSecurityNamespace(IVssRequestContext requestContext, Guid namespaceId) => this.m_localSecurityService.DeleteSecurityNamespace(requestContext, namespaceId);

    public virtual IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      requestContext.TraceEnter(56420, "Security", nameof (TeamFoundationSecurityService), nameof (GetSecurityNamespace));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
        this.EnsurePointersLoaded(requestContext);
        RemoteSecurityNamespaceDescription namespaceDescription1 = (RemoteSecurityNamespaceDescription) null;
        IRemoteSecurityNamespace securityNamespace1 = (IRemoteSecurityNamespace) null;
        using (requestContext.AcquireReaderLock(this.m_lockName))
        {
          IRemoteSecurityNamespace securityNamespace2;
          if (this.m_namespaces.TryGetValue(namespaceId, out securityNamespace2))
            return (IVssSecurityNamespace) securityNamespace2;
          if (!this.m_pointers.TryGetValue(namespaceId, out namespaceDescription1))
            namespaceDescription1 = (RemoteSecurityNamespaceDescription) null;
        }
        if (namespaceDescription1 != null)
        {
          if (namespaceDescription1.ServiceOwner == SecurityServiceConstants.ToParentServiceOwner)
            securityNamespace1 = (IRemoteSecurityNamespace) this.CreateToParentSecurityNamespace(requestContext, namespaceId);
          else if (this.ShouldDereferenceRemotePointer(requestContext, namespaceDescription1.ServiceOwner))
          {
            requestContext.Trace(56423, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), "GetSecurityNamespace description fetch {0} from {1}", (object) namespaceId, (object) namespaceDescription1.ServiceOwner);
            IVssRequestContext vssRequestContext = requestContext.Elevate();
            Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription vssDescription = vssRequestContext.GetClient<SecurityHttpClient>(namespaceDescription1.ServiceOwner).QuerySecurityNamespacesAsync(namespaceId, true).SyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>>().FirstOrDefault<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>();
            if (vssDescription != null && vssDescription.IsRemotable)
              securityNamespace1 = (IRemoteSecurityNamespace) this.CreateRemoteSecurityNamespace(vssRequestContext, vssDescription, namespaceDescription1.ServiceOwner);
          }
          if (securityNamespace1 != null && securityNamespace1.IsCacheable)
          {
            using (requestContext.AcquireWriterLock(this.m_lockName))
            {
              RemoteSecurityNamespaceDescription namespaceDescription2;
              if (this.m_pointers.TryGetValue(namespaceId, out namespaceDescription2))
              {
                if (namespaceDescription2.ServiceOwner == securityNamespace1.ServiceOwner)
                {
                  IRemoteSecurityNamespace securityNamespace3;
                  if (this.m_namespaces.TryGetValue(namespaceId, out securityNamespace3))
                    securityNamespace1 = securityNamespace3;
                  else
                    this.m_namespaces.Add(namespaceId, securityNamespace1);
                }
              }
            }
          }
        }
        return (IVssSecurityNamespace) securityNamespace1 ?? (IVssSecurityNamespace) this.m_localSecurityService.GetSecurityNamespace(requestContext, namespaceId) ?? this.m_localSecurityService.GetShimSecurityNamespace(requestContext, namespaceId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56421, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56422, "Security", nameof (TeamFoundationSecurityService), nameof (GetSecurityNamespace));
      }
    }

    public IList<IVssSecurityNamespace> GetSecurityNamespaces(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56430, "Security", nameof (TeamFoundationSecurityService), nameof (GetSecurityNamespaces));
      try
      {
        this.ValidateRequestContext(requestContext);
        this.EnsurePointersLoaded(requestContext);
        IEnumerable<RemoteSecurityNamespaceDescription> toFetch = (IEnumerable<RemoteSecurityNamespaceDescription>) null;
        List<IVssSecurityNamespace> securityNamespaces1 = new List<IVssSecurityNamespace>();
        using (requestContext.AcquireReaderLock(this.m_lockName))
        {
          if (this.m_pointers.Count > 0)
          {
            securityNamespaces1.AddRange((IEnumerable<IVssSecurityNamespace>) this.m_namespaces.Values);
            IEnumerable<RemoteSecurityNamespaceDescription> source = this.m_pointers.Values.Where<RemoteSecurityNamespaceDescription>((Func<RemoteSecurityNamespaceDescription, bool>) (s => !this.m_namespaces.ContainsKey(s.NamespaceId)));
            if (source.Any<RemoteSecurityNamespaceDescription>())
              toFetch = (IEnumerable<RemoteSecurityNamespaceDescription>) source.ToList<RemoteSecurityNamespaceDescription>();
          }
        }
        if (toFetch != null)
        {
          List<IRemoteSecurityNamespace> securityNamespaceList = this.FetchSecurityNamespacesFromRemote(requestContext, toFetch);
          if (securityNamespaceList.Any<IRemoteSecurityNamespace>((Func<IRemoteSecurityNamespace, bool>) (s => s.IsCacheable)))
          {
            using (requestContext.AcquireWriterLock(this.m_lockName))
            {
              for (int index = 0; index < securityNamespaceList.Count; ++index)
              {
                if (securityNamespaceList[index].IsCacheable)
                {
                  Guid namespaceId = securityNamespaceList[index].Description.NamespaceId;
                  RemoteSecurityNamespaceDescription namespaceDescription;
                  if (this.m_pointers.TryGetValue(namespaceId, out namespaceDescription) && namespaceDescription.ServiceOwner == securityNamespaceList[index].ServiceOwner)
                  {
                    IRemoteSecurityNamespace securityNamespace;
                    if (this.m_namespaces.TryGetValue(namespaceId, out securityNamespace))
                      securityNamespaceList[index] = securityNamespace;
                    else
                      this.m_namespaces.Add(namespaceId, securityNamespaceList[index]);
                  }
                }
              }
            }
          }
          securityNamespaces1.AddRange((IEnumerable<IVssSecurityNamespace>) securityNamespaceList);
        }
        IList<LocalSecurityNamespace> securityNamespaces2 = this.m_localSecurityService.GetSecurityNamespaces(requestContext);
        foreach (IRemoteSecurityNamespace securityNamespace in securityNamespaces1)
        {
          for (int index = 0; index < securityNamespaces2.Count; ++index)
          {
            if (securityNamespaces2[index].Description.NamespaceId == securityNamespace.Description.NamespaceId)
            {
              securityNamespaces2.RemoveAt(index);
              break;
            }
          }
        }
        securityNamespaces1.AddRange((IEnumerable<IVssSecurityNamespace>) securityNamespaces2);
        IList<IVssSecurityNamespace> securityNamespaces3 = this.m_localSecurityService.GetShimSecurityNamespaces(requestContext);
        foreach (IVssSecurityNamespace securityNamespace in securityNamespaces1)
        {
          for (int index = 0; index < securityNamespaces3.Count; ++index)
          {
            if (securityNamespaces3[index].Description.NamespaceId == securityNamespace.Description.NamespaceId)
            {
              securityNamespaces3.RemoveAt(index);
              break;
            }
          }
        }
        securityNamespaces1.AddRange((IEnumerable<IVssSecurityNamespace>) securityNamespaces3);
        return (IList<IVssSecurityNamespace>) securityNamespaces1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56431, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56432, "Security", nameof (TeamFoundationSecurityService), nameof (GetSecurityNamespaces));
      }
    }

    private bool ShouldDereferenceRemotePointer(
      IVssRequestContext requestContext,
      Guid serviceOwner)
    {
      bool flag = ServiceInstanceTypes.SPS == serviceOwner;
      if (!flag && (requestContext.ServiceHost.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Unknown)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        flag = vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMappings(vssRequestContext, requestContext.ServiceHost.InstanceId).Any<HostInstanceMapping>((Func<HostInstanceMapping, bool>) (mapping => mapping.ServiceInstance.InstanceType == serviceOwner));
      }
      return flag;
    }

    public void RemoveIdentityACEs(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.m_localSecurityService.RemoveIdentityACEs(requestContext, identities);
    }

    internal RemoteSecurityNamespace CreateRemoteSecurityNamespace(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription vssDescription,
      Guid serviceOwner)
    {
      SecurityNamespaceDescription description = SecurityConverter.Convert(vssDescription);
      description.IsRemotable = false;
      description.IsRemoted = true;
      return new RemoteSecurityNamespace(requestContext, description, this.m_localSecurityService.GetExtensionForExtensionType(description.ExtensionType), serviceOwner);
    }

    private ToParentSecurityNamespace CreateToParentSecurityNamespace(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      IVssRequestContext vssRequestContext = requestContext;
      while (!vssRequestContext.GetService<IdentityService>().IdentityServiceInternal().Domain.IsMaster)
      {
        vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
        if (vssRequestContext != null)
        {
          IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, namespaceId);
          if (securityNamespace != null)
            return new ToParentSecurityNamespace(securityNamespace, vssRequestContext.ServiceHost.InstanceId);
        }
        else
          break;
      }
      return (ToParentSecurityNamespace) null;
    }

    private void OnRemotePointersChanged(object sender, IVssRequestContextEventArgs e)
    {
      IVssRequestContext requestContext = e.RequestContext;
      requestContext.TraceEnter(56400, "Security", nameof (TeamFoundationSecurityService), nameof (OnRemotePointersChanged));
      try
      {
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          ++this.m_cacheVersion;
          this.m_cacheFresh = false;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56401, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56402, "Security", nameof (TeamFoundationSecurityService), nameof (OnRemotePointersChanged));
      }
    }

    private void OnRemoteSecurityNamespacesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56440, "Security", nameof (TeamFoundationSecurityService), nameof (OnRemoteSecurityNamespacesChanged));
      try
      {
        RemoteSecurityNamespacesChangedMessage message = TeamFoundationSerializationUtility.Deserialize<RemoteSecurityNamespacesChangedMessage>(eventData);
        bool flag = false;
        using (requestContext.AcquireReaderLock(this.m_lockName))
        {
          foreach (IRemoteSecurityNamespace securityNamespace in this.m_namespaces.Values)
          {
            if (securityNamespace.ServiceOwner == message.ServiceOwner)
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          return;
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          foreach (Guid key in new List<Guid>(this.m_namespaces.Values.Where<IRemoteSecurityNamespace>((Func<IRemoteSecurityNamespace, bool>) (s => s.ServiceOwner == message.ServiceOwner)).Select<IRemoteSecurityNamespace, Guid>((Func<IRemoteSecurityNamespace, Guid>) (s => s.Description.NamespaceId))))
            this.m_namespaces.Remove(key);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56441, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56442, "Security", nameof (TeamFoundationSecurityService), nameof (OnRemoteSecurityNamespacesChanged));
      }
    }

    private void EnsurePointersLoaded(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56410, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
      try
      {
        if (this.m_cacheFresh)
        {
          requestContext.Trace(56413, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
        }
        else
        {
          int cacheVersion = this.m_cacheVersion;
          Dictionary<Guid, RemoteSecurityNamespaceDescription> dictionary = this.m_localSecurityService.GetRemoteSecurityNamespaces(requestContext).ToDictionary<RemoteSecurityNamespaceDescription, Guid>((Func<RemoteSecurityNamespaceDescription, Guid>) (s => s.NamespaceId));
          if (this.m_cacheFresh)
          {
            requestContext.Trace(56414, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
          }
          else
          {
            using (requestContext.AcquireWriterLock(this.m_lockName))
            {
              if (this.m_cacheFresh)
              {
                requestContext.Trace(56415, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
              }
              else
              {
                this.m_pointers = (IDictionary<Guid, RemoteSecurityNamespaceDescription>) dictionary;
                this.m_namespaces.Clear();
                if (this.m_cacheVersion != cacheVersion)
                  return;
                requestContext.Trace(56416, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
                this.m_cacheFresh = true;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56411, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56412, "Security", nameof (TeamFoundationSecurityService), nameof (EnsurePointersLoaded));
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostInstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.SecurityServiceRequestContextHostMessage((object) this.m_serviceHostInstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    private void NamespaceReloadTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(56760, "Security", nameof (TeamFoundationSecurityService), nameof (NamespaceReloadTask));
      try
      {
        this.EnsurePointersLoaded(requestContext);
        IEnumerable<RemoteSecurityNamespaceDescription> toFetch = (IEnumerable<RemoteSecurityNamespaceDescription>) null;
        using (requestContext.AcquireReaderLock(this.m_lockName))
        {
          if (this.m_pointers.Count > 0)
          {
            IEnumerable<RemoteSecurityNamespaceDescription> source = this.m_pointers.Values.Where<RemoteSecurityNamespaceDescription>((Func<RemoteSecurityNamespaceDescription, bool>) (s => this.m_namespaces.ContainsKey(s.NamespaceId)));
            if (source.Any<RemoteSecurityNamespaceDescription>())
              toFetch = (IEnumerable<RemoteSecurityNamespaceDescription>) source.ToList<RemoteSecurityNamespaceDescription>();
          }
        }
        if (toFetch == null)
          return;
        requestContext.Trace(56763, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), "Reload all resolved remoted namespaces.");
        List<IRemoteSecurityNamespace> source1 = this.FetchSecurityNamespacesFromRemote(requestContext, toFetch);
        if (!source1.Any<IRemoteSecurityNamespace>((Func<IRemoteSecurityNamespace, bool>) (s => s.IsCacheable)))
          return;
        using (requestContext.AcquireWriterLock(this.m_lockName))
        {
          foreach (IRemoteSecurityNamespace securityNamespace in source1)
          {
            if (securityNamespace.IsCacheable)
            {
              Guid namespaceId = securityNamespace.Description.NamespaceId;
              RemoteSecurityNamespaceDescription namespaceDescription;
              if (this.m_pointers.TryGetValue(namespaceId, out namespaceDescription) && namespaceDescription.ServiceOwner == securityNamespace.ServiceOwner)
                this.m_namespaces[namespaceId] = securityNamespace;
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56762, "Security", nameof (TeamFoundationSecurityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56761, "Security", nameof (TeamFoundationSecurityService), nameof (NamespaceReloadTask));
      }
    }

    private List<IRemoteSecurityNamespace> FetchSecurityNamespacesFromRemote(
      IVssRequestContext requestContext,
      IEnumerable<RemoteSecurityNamespaceDescription> toFetch)
    {
      List<IRemoteSecurityNamespace> securityNamespaceList = new List<IRemoteSecurityNamespace>();
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      foreach (IGrouping<Guid, RemoteSecurityNamespaceDescription> source in toFetch.GroupBy<RemoteSecurityNamespaceDescription, Guid>((Func<RemoteSecurityNamespaceDescription, Guid>) (s => s.ServiceOwner)))
      {
        if (source.Key == SecurityServiceConstants.ToParentServiceOwner)
        {
          foreach (RemoteSecurityNamespaceDescription namespaceDescription in (IEnumerable<RemoteSecurityNamespaceDescription>) source)
          {
            IRemoteSecurityNamespace securityNamespace = (IRemoteSecurityNamespace) this.CreateToParentSecurityNamespace(requestContext, namespaceDescription.NamespaceId);
            if (securityNamespace != null)
              securityNamespaceList.Add(securityNamespace);
          }
        }
        else if (this.ShouldDereferenceRemotePointer(requestContext, source.Key))
        {
          if (source.Take<RemoteSecurityNamespaceDescription>(2).Count<RemoteSecurityNamespaceDescription>() > 1)
          {
            HashSet<Guid> guidSet = new HashSet<Guid>(source.Select<RemoteSecurityNamespaceDescription, Guid>((Func<RemoteSecurityNamespaceDescription, Guid>) (s => s.NamespaceId)));
            requestContext.Trace(56433, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), "GetSecurityNamespaces description bulk fetch {0} namespaces from {1}", (object) guidSet.Count, (object) source.Key);
            foreach (Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription vssDescription in vssRequestContext.GetClient<SecurityHttpClient>(source.Key).QuerySecurityNamespacesAsync(Guid.Empty, true).SyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>>())
            {
              if (vssDescription != null && vssDescription.IsRemotable && guidSet.Contains(vssDescription.NamespaceId))
                securityNamespaceList.Add((IRemoteSecurityNamespace) this.CreateRemoteSecurityNamespace(vssRequestContext, vssDescription, source.Key));
            }
          }
          else
          {
            RemoteSecurityNamespaceDescription namespaceDescription = source.FirstOrDefault<RemoteSecurityNamespaceDescription>();
            requestContext.Trace(56434, TraceLevel.Verbose, "Security", nameof (TeamFoundationSecurityService), "GetSecurityNamespaces description fetch {0} from {1}", (object) namespaceDescription.NamespaceId, (object) source.Key);
            Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription vssDescription = vssRequestContext.GetClient<SecurityHttpClient>(source.Key).QuerySecurityNamespacesAsync(namespaceDescription.NamespaceId, true).SyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>>().FirstOrDefault<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>();
            if (vssDescription != null && vssDescription.IsRemotable)
              securityNamespaceList.Add((IRemoteSecurityNamespace) this.CreateRemoteSecurityNamespace(vssRequestContext, vssDescription, source.Key));
          }
        }
      }
      return securityNamespaceList;
    }

    internal void ResetNamespaceReloadTask(
      IVssRequestContext requestContext,
      int newCacheLifetimeValue)
    {
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      if (this.m_namespaceReloadTask != null)
      {
        service.RemoveTask(requestContext, this.m_namespaceReloadTask);
        this.m_namespaceReloadTask = (TeamFoundationTask) null;
      }
      if (newCacheLifetimeValue == 0)
        return;
      this.m_namespaceReloadTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.NamespaceReloadTask), (object) null, newCacheLifetimeValue);
      service.AddTask(requestContext, this.m_namespaceReloadTask);
    }
  }
}
