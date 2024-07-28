// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.HostManagementUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  public static class HostManagementUtil
  {
    public static ServiceHostProperties Convert(HostProperties hostProperties) => new ServiceHostProperties()
    {
      HostId = hostProperties.Id,
      ParentHostId = hostProperties.ParentId,
      Name = hostProperties.Name,
      Description = hostProperties.Description,
      HostType = (ServiceHostType) hostProperties.HostType,
      State = hostProperties.Status,
      StatusReason = hostProperties.StatusReason,
      SubStatus = hostProperties.SubStatus
    };

    public static ServiceHostProperties Convert(IVssServiceHost hostProperties) => new ServiceHostProperties()
    {
      HostId = hostProperties.InstanceId,
      ParentHostId = hostProperties.ParentServiceHost.InstanceId,
      Name = hostProperties.Name,
      Description = hostProperties.Description,
      HostType = (ServiceHostType) hostProperties.HostType,
      State = hostProperties.Status,
      StatusReason = hostProperties.StatusReason
    };
  }
}
