// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredHostManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuredHostManager : IVssFrameworkService
  {
    internal bool m_filterActiveRequests;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), "/Service/Framework/Settings/FilterActiveRequests");
      this.LoadSettings(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    public void CancelRequest(
      IVssRequestContext requestContext,
      Guid hostId,
      long requestId,
      string reason)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      TeamFoundationHostManagementService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
      IVssRequestContext activeRequest = (IVssRequestContext) null;
      bool flag = false;
      try
      {
        using (IVssRequestContext vssRequestContext = service.BeginRequest(requestContext, hostId, RequestContextType.UserContext, true, true))
        {
          if (vssRequestContext.Elevate().ServiceHost.ServiceHostInternal().TryGetRequest(requestId, out activeRequest))
          {
            IdentityDescriptor authenticatedDescriptor = activeRequest.GetAuthenticatedDescriptor();
            flag = IdentityDescriptorComparer.Instance.Equals(requestContext.UserContext, authenticatedDescriptor) || vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
          }
        }
      }
      catch (HostDoesNotExistException ex)
      {
      }
      if (!flag)
        throw new ArgumentException(FrameworkResources.InvalidRequestId((object) hostId, (object) requestId));
      activeRequest.Cancel(reason);
    }

    public List<TeamFoundationServiceHostActivity> QueryActiveRequests(
      IVssRequestContext requestContext,
      Guid[] hosts,
      bool includeDetails)
    {
      bool flag1 = hosts != null && hosts.Length != 0;
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      Dictionary<Guid, IVssRequestContext[]> activeRequests = service.GetActiveRequests(requestContext, hosts);
      List<TeamFoundationServiceHostActivity> serviceHostActivityList = new List<TeamFoundationServiceHostActivity>();
      foreach (Guid guid in flag1 ? (IEnumerable<Guid>) hosts : (IEnumerable<Guid>) activeRequests.Keys)
      {
        bool flag2 = false;
        try
        {
          using (IVssRequestContext vssRequestContext1 = service.BeginRequest(requestContext, guid, RequestContextType.UserContext, true, true))
          {
            IVssRequestContext[] vssRequestContextArray;
            if (activeRequests.TryGetValue(guid, out vssRequestContextArray))
            {
              if (vssRequestContextArray != null)
              {
                if (vssRequestContextArray.Length != 0)
                {
                  IVssSecurityNamespace securityNamespace = vssRequestContext1.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext1, FrameworkSecurity.FrameworkNamespaceId);
                  if (securityNamespace.HasPermission(vssRequestContext1, FrameworkSecurity.FrameworkNamespaceToken, 1))
                  {
                    TeamFoundationServiceHostActivity serviceHostActivity = new TeamFoundationServiceHostActivity()
                    {
                      Id = vssRequestContext1.ServiceHost.InstanceId,
                      Name = vssRequestContext1.ServiceHost.Name,
                      StartTime = vssRequestContext1.ServiceHost.ServiceHostInternal().StartTime,
                      Status = vssRequestContext1.ServiceHost.Status,
                      StatusReason = vssRequestContext1.ServiceHost.StatusReason
                    };
                    serviceHostActivityList.Add(serviceHostActivity);
                    flag2 = true;
                    bool flag3 = securityNamespace.HasPermission(vssRequestContext1, FrameworkSecurity.FrameworkNamespaceToken, 2);
                    foreach (IVssRequestContext vssRequestContext2 in vssRequestContextArray)
                    {
                      bool includeDetails1 = includeDetails;
                      IdentityDescriptor authenticatedDescriptor = vssRequestContext2.GetAuthenticatedDescriptor();
                      if (!flag3 && !IdentityDescriptorComparer.Instance.Equals(authenticatedDescriptor, requestContext.UserContext))
                      {
                        if (!this.m_filterActiveRequests)
                          includeDetails1 = false;
                        else
                          continue;
                      }
                      serviceHostActivity.ActiveRequests.Add(new TeamFoundationRequestInformation(requestContext, vssRequestContext2, includeDetails1));
                    }
                  }
                }
              }
            }
          }
        }
        catch (HostDoesNotExistException ex)
        {
        }
        finally
        {
          if (flag1 && !flag2)
            serviceHostActivityList.Add((TeamFoundationServiceHostActivity) null);
        }
      }
      return serviceHostActivityList;
    }

    private void LoadSettings(IVssRequestContext requestContext) => this.m_filterActiveRequests = requestContext.GetService<CachedRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/Framework/Settings/FilterActiveRequests", true, true);

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }
  }
}
