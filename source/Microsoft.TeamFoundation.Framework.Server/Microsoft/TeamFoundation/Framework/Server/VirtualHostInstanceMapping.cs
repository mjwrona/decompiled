// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualHostInstanceMapping
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VirtualHostInstanceMapping
  {
    public Guid HostId { get; set; }

    public Guid ServiceInstanceId { get; set; }

    public Guid ServiceInstanceType { get; set; }

    public ServiceDefinition ToServiceDefinition() => new ServiceDefinition("VirtualLocation", this.ServiceInstanceId, TFCommonResources.LocationService(), (string) null, RelativeToSetting.FullyQualified, TFCommonResources.LocationService(), "Framework")
    {
      ParentServiceType = "VsService",
      ParentIdentifier = this.ServiceInstanceType
    };

    internal static VirtualHostInstanceMapping FromServiceDefinition(
      Guid hostId,
      ServiceDefinition instanceMapping)
    {
      return new VirtualHostInstanceMapping()
      {
        HostId = hostId,
        ServiceInstanceId = instanceMapping.Identifier,
        ServiceInstanceType = instanceMapping.ParentIdentifier
      };
    }
  }
}
