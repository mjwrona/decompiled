// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostInstanceMapping
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class HostInstanceMapping
  {
    public HostInstanceMapping()
    {
    }

    internal HostInstanceMapping(
      Guid hostId,
      ServiceDefinition instanceMapping,
      ServiceInstance serviceInstance)
    {
      LocationMapping locationMapping = instanceMapping.GetLocationMapping(AccessMappingConstants.ServerAccessMappingMoniker);
      this.HostId = hostId;
      this.ServiceInstance = serviceInstance;
      this.Uri = locationMapping != null ? new Uri(locationMapping.Location) : (Uri) null;
      this.Status = instanceMapping.Status;
    }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public ServiceInstance ServiceInstance { get; set; }

    [DataMember]
    public Uri Uri { get; set; }

    [DataMember]
    public ServiceStatus Status { get; set; }
  }
}
