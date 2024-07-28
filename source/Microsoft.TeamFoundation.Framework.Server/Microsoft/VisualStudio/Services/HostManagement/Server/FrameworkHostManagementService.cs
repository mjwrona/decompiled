// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.Server.FrameworkHostManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.HostManagement.Server
{
  internal class FrameworkHostManagementService : IHostManagementService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ServiceHostProperties GetServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      try
      {
        return this.GetHttpClient(requestContext1, hostId).GetServiceHostPropertiesAsync(hostId, (object) null).SyncResult<ServiceHostProperties>();
      }
      catch (PartitionNotFoundException ex)
      {
        requestContext.TraceException(412055, "HostManagement", nameof (FrameworkHostManagementService), (Exception) ex);
        return (ServiceHostProperties) null;
      }
    }

    private HostManagementHttpClient GetHttpClient(IVssRequestContext requestContext, Guid hostId) => PartitionedClientHelper.GetSpsClientForHostId<HostManagementHttpClient>(requestContext, hostId);
  }
}
