// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMembershipTransferService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.UserMapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMembershipTransferService : IVssFrameworkService
  {
    private const string s_Area = "IdentityService";
    private const string s_Layer = "IdentityMembershipTransferService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ProcessTransferQueue(IVssRequestContext requestContext)
    {
      List<KeyValuePair<Guid, Guid>> identitiesToTransfer = (List<KeyValuePair<Guid, Guid>>) null;
      using (IdentityMembershipTransferComponent component = requestContext.CreateComponent<IdentityMembershipTransferComponent>())
        identitiesToTransfer = component.ReadIdentitiesFromTransferQueue();
      if (identitiesToTransfer == null || identitiesToTransfer.Count <= 0)
        return;
      this.TransferMembership(requestContext, (IEnumerable<KeyValuePair<Guid, Guid>>) identitiesToTransfer);
    }

    public void TransferMembership(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      try
      {
        foreach (KeyValuePair<Guid, Guid> identityToTransfer in identitiesToTransfer)
        {
          requestContext.Trace(56105, TraceLevel.Info, "IdentityService", nameof (IdentityMembershipTransferService), "Working on transferring {0} to {1}", (object) identityToTransfer.Key, (object) identityToTransfer.Value);
          IList<Guid> guidList = requestContext.GetService<IUserAccountMappingService>().QueryAccountIds(requestContext.Elevate(), identityToTransfer.Key, UserType.Member, true, true);
          bool flag = false;
          foreach (Guid instanceId in (IEnumerable<Guid>) guidList)
          {
            requestContext.Trace(56108, TraceLevel.Info, "IdentityService", nameof (IdentityMembershipTransferService), "Working on transferring {0} to {1} - account {2}", (object) identityToTransfer.Key, (object) identityToTransfer.Value, (object) instanceId);
            try
            {
              using (IVssRequestContext vssRequestContext1 = service.BeginRequest(requestContext, instanceId, RequestContextType.SystemContext, true, true))
              {
                IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Application);
                vssRequestContext2.GetService<PlatformIdentityService>().IdentityStore.TransferMembership(vssRequestContext2, identityToTransfer);
              }
            }
            catch (HostDoesNotExistException ex)
            {
              requestContext.TraceException(56115, "IdentityService", nameof (IdentityMembershipTransferService), (Exception) ex);
            }
            catch (HostShutdownException ex)
            {
              flag = true;
              requestContext.TraceException(56120, "IdentityService", nameof (IdentityMembershipTransferService), (Exception) ex);
            }
          }
          if (!flag)
          {
            using (IdentityMembershipTransferComponent component = requestContext.CreateComponent<IdentityMembershipTransferComponent>())
              component.RemoveFromTransferQueue((IEnumerable<Guid>) new Guid[1]
              {
                identityToTransfer.Key
              });
            requestContext.Trace(56125, TraceLevel.Info, "IdentityService", nameof (IdentityMembershipTransferService), "Completed transferring {0} to {1}", (object) identityToTransfer.Key, (object) identityToTransfer.Value);
          }
        }
      }
      catch (ServiceNotRegisteredException ex)
      {
        requestContext.TraceException(56130, "IdentityService", nameof (IdentityMembershipTransferService), (Exception) ex);
      }
    }
  }
}
