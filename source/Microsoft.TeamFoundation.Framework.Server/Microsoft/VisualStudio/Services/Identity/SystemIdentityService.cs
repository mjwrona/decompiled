// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SystemIdentityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class SystemIdentityService : ISystemIdentityService, IVssFrameworkService
  {
    private Guid m_domainId;
    private IdentityMapper m_mapper;
    private IDictionary<string, IIdentityProvider> m_syncAgents;
    private IVssSecuritySubjectService m_securitySubjectService;
    private const string c_area = "IdentityService";
    private const string c_layer = "SystemIdentityService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10003001, "IdentityService", nameof (SystemIdentityService), "ServiceStart");
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.m_securitySubjectService = systemRequestContext.GetService<IVssSecuritySubjectService>();
        this.m_domainId = systemRequestContext.ServiceHost.InstanceId;
        this.m_mapper = new IdentityMapper(this.m_domainId);
        this.m_syncAgents = (IDictionary<string, IIdentityProvider>) new Dictionary<string, IIdentityProvider>();
        this.m_syncAgents.Add("System:ServicePrincipal", (IIdentityProvider) new ServicePrincipalProvider());
        this.m_syncAgents.Add("Microsoft.VisualStudio.Services.Claims.AadServicePrincipal", (IIdentityProvider) new AADServicePrincipalProvider());
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10003002, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10003003, "IdentityService", nameof (SystemIdentityService), "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10003004, "IdentityService", nameof (SystemIdentityService), "ServiceEnd");
      try
      {
        this.m_securitySubjectService = (IVssSecuritySubjectService) null;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10003005, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10003006, "IdentityService", nameof (SystemIdentityService), "ServiceEnd");
      }
    }

    public Guid DomainId => this.m_domainId;

    public IdentityMapper IdentityMapper => this.m_mapper;

    public IDictionary<string, IIdentityProvider> SyncAgents => this.m_syncAgents;

    public bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      bool flag = requestContext.IsTracing(10003055, TraceLevel.Verbose, "IdentityService", nameof (SystemIdentityService));
      if (flag)
        requestContext.TraceEnter(10003055, "IdentityService", nameof (SystemIdentityService), nameof (IsMember));
      try
      {
        return groupDescriptor.IsSystemServicePrincipalType() && string.Equals(groupDescriptor.Identifier, "*", StringComparison.Ordinal) && ServicePrincipals.IsServicePrincipal(requestContext, memberDescriptor);
      }
      catch (Exception ex)
      {
        if (flag)
          requestContext.TraceException(10003056, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        if (flag)
          requestContext.TraceLeave(10003057, "IdentityService", nameof (SystemIdentityService), nameof (IsMember));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(10003061, "IdentityService", nameof (SystemIdentityService), nameof (ListGroups));
      try
      {
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10003062, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10003063, "IdentityService", nameof (SystemIdentityService), nameof (ListGroups));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10003070, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      try
      {
        this.CheckIdentityReadPermissions(requestContext, 1);
        if (identityIds == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        for (int index = 0; index < identityIds.Count; ++index)
        {
          Guid identityId = identityIds[index];
          if (identityId == AnonymousAccessConstants.AnonymousSubjectId)
          {
            identityArray[index] = SystemIdentityService.CreateAnonymousIdentity();
          }
          else
          {
            requestContext.Trace(10003081, TraceLevel.Verbose, "IdentityService", nameof (SystemIdentityService), string.Format("Looking up identityId {0} in security subject service.", (object) identityId));
            SecuritySubjectEntry securitySubjectEntry = this.m_securitySubjectService.GetSecuritySubjectEntry(requestContext1, identityId);
            if (securitySubjectEntry != null)
            {
              requestContext.Trace(10003082, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Found entry for identityId {0} in the security subject service.", (object) identityId));
              identityArray[index] = ServicePrincipals.CreateServicePrincipalIdentity(securitySubjectEntry.ToDescriptor(), securitySubjectEntry.Description);
            }
            else
            {
              requestContext.Trace(10003083, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Security subject store does not contain an entry for identityId {0}.", (object) identityId));
              identityArray[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            }
          }
        }
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10003071, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10003072, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(311075, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      try
      {
        this.CheckIdentityReadPermissions(requestContext, 1);
        if (subjectDescriptors == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[subjectDescriptors.Count];
        for (int index = 0; index < subjectDescriptors.Count; ++index)
        {
          SubjectDescriptor subjectDescriptor = subjectDescriptors[index];
          if (subjectDescriptor == new SubjectDescriptor())
          {
            requestContext.Trace(312332, TraceLevel.Verbose, "IdentityService", nameof (SystemIdentityService), "Skipping null subject descriptor");
            identityArray[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          }
          else if (!subjectDescriptor.IsSystemType())
          {
            requestContext.Trace(203111, TraceLevel.Verbose, "IdentityService", nameof (SystemIdentityService), string.Format("Skipping {0} as it not of system type", (object) subjectDescriptor));
            identityArray[index] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          }
          else
          {
            IdentityDescriptor identityDescriptor = subjectDescriptor.ToIdentityDescriptor(requestContext);
            identityArray[index] = this.ReadIdentityByDescriptor(requestContext, identityDescriptor, queryMembership, propertyNameFilters, includeRestrictedVisibility, out IdentityDescriptor _);
          }
        }
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(134060, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(375300, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10003073, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      try
      {
        this.CheckIdentityReadPermissions(requestContext, 1);
        if (descriptors == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
        for (int index = 0; index < descriptors.Count; ++index)
        {
          IdentityDescriptor updatedDescriptorToRead;
          identityArray[index] = this.ReadIdentityByDescriptor(requestContext, descriptors[index], queryMembership, propertyNameFilters, includeRestrictedVisibility, out updatedDescriptorToRead);
          if (updatedDescriptorToRead != (IdentityDescriptor) null)
            descriptors[index] = updatedDescriptorToRead;
        }
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10003074, "IdentityService", nameof (SystemIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10003075, "IdentityService", nameof (SystemIdentityService), nameof (ReadIdentities));
      }
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      out IdentityDescriptor updatedDescriptorToRead)
    {
      updatedDescriptorToRead = (IdentityDescriptor) null;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext.Trace(10003084, TraceLevel.Verbose, "IdentityService", nameof (SystemIdentityService), string.Format("Looking up descriptor {0} in the security subject service.", (object) descriptor));
      Guid spGuid;
      if (ServicePrincipals.TryParse(descriptor, out spGuid, out Guid _))
      {
        SecuritySubjectEntry securitySubjectEntry = this.m_securitySubjectService.GetSecuritySubjectEntry(requestContext1, spGuid);
        if (securitySubjectEntry != null)
        {
          requestContext.Trace(10003085, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Found entry for descriptor {0} in the security subject service, with id {1}.", (object) descriptor, (object) spGuid));
          IdentityDescriptor descriptor1 = securitySubjectEntry.ToDescriptor();
          if (IdentityDescriptorComparer.Instance.Equals(descriptor, descriptor1))
            return ServicePrincipals.CreateServicePrincipalIdentity(descriptor1, securitySubjectEntry.Description);
          if (descriptor.IsClaimsIdentityType() && descriptor1.IsSystemServicePrincipalType())
          {
            requestContext.TraceAlways(10003088, TraceLevel.Warning, "IdentityService", nameof (SystemIdentityService), string.Format("Passed in descriptor {0} and entry descriptor {1} do not match on System type, something is not migrated yet.", (object) descriptor, (object) descriptor1));
            return ServicePrincipals.CreateServicePrincipalIdentity(descriptor1, securitySubjectEntry.Description);
          }
          requestContext.Trace(10003086, TraceLevel.Error, "IdentityService", nameof (SystemIdentityService), string.Format("Passed in descriptor {0} and entry descriptor {1} do not match, this is very unexpected.", (object) descriptor, (object) descriptor1));
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        }
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && spGuid == InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.TFSOnPremises))
        {
          requestContext.Trace(10003087, TraceLevel.Warning, "IdentityService", nameof (SystemIdentityService), string.Format("Security subject store does not yet contain an entry for onPrem Service Principal: {0}. Returning an identity for it anyway.", (object) spGuid));
          return ServicePrincipals.CreateServicePrincipalIdentity(descriptor, (string) null);
        }
        requestContext.Trace(10003087, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Security subject store does not contain an entry for identityId {0}.", (object) spGuid));
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      if (IdentityDescriptorComparer.Instance.Equals(descriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal))
      {
        requestContext.Trace(473768, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Creating anonumous identity for Descriptor {0}.", (object) descriptor));
        return SystemIdentityService.CreateAnonymousIdentity();
      }
      requestContext.Trace(10003085, TraceLevel.Info, "IdentityService", nameof (SystemIdentityService), string.Format("Descriptor {0} did not parse as a service principal, skipping.", (object) descriptor));
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    internal IList<Microsoft.VisualStudio.Services.Identity.Identity> GetServicePrincipalIdentities(
      IVssRequestContext requestContext)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> principalIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (SecuritySubjectEntry securitySubjectEntry in this.m_securitySubjectService.GetSecuritySubjectEntries(requestContext))
      {
        if (securitySubjectEntry.SubjectType == SecuritySubjectType.ServicePrincipal)
          principalIdentities.Add(ServicePrincipals.CreateServicePrincipalIdentity(securitySubjectEntry.ToDescriptor(), securitySubjectEntry.Description));
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) principalIdentities;
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateAnonymousIdentity()
    {
      Microsoft.VisualStudio.Services.Identity.Identity anonymousIdentity = new Microsoft.VisualStudio.Services.Identity.Identity();
      anonymousIdentity.CustomDisplayName = FrameworkResources.AnonymousPrincipalName();
      anonymousIdentity.Descriptor = UserWellKnownIdentityDescriptors.AnonymousPrincipal;
      anonymousIdentity.ProviderDisplayName = FrameworkResources.AnonymousPrincipalName();
      anonymousIdentity.Id = AnonymousAccessConstants.AnonymousSubjectId;
      anonymousIdentity.IsActive = true;
      anonymousIdentity.IsContainer = false;
      anonymousIdentity.MasterId = AnonymousAccessConstants.AnonymousSubjectId;
      anonymousIdentity.SetProperty("SchemaClassName", (object) "User");
      anonymousIdentity.SetProperty("Description", (object) FrameworkResources.AnonymousPrincipalDescription());
      anonymousIdentity.SetProperty("Domain", (object) "TEAM FOUNDATION");
      anonymousIdentity.SetProperty("Account", (object) "Anonymous");
      anonymousIdentity.SetProperty("DN", (object) string.Empty);
      anonymousIdentity.SetProperty("Mail", (object) string.Empty);
      return anonymousIdentity;
    }

    private void CheckIdentityReadPermissions(IVssRequestContext requestContext, int permission)
    {
      if (!(requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GraphSecurityConstants.NamespaceId);
      if (securityNamespace == null)
        return;
      permission |= 1;
      securityNamespace.CheckPermission(requestContext, GraphSecurityConstants.SubjectsToken, permission);
    }
  }
}
