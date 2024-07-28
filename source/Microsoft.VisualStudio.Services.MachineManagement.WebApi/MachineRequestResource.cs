// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestResource
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineRequestResource
  {
    internal MachineRequestResource()
    {
    }

    public MachineRequestResource(
      int resourceId,
      string poolType,
      string resourceVersion,
      string resourcePlatform,
      MachineRequestResourceState state,
      long containerId,
      string downloadUrl,
      DateTime? createdOn)
    {
      this.ResourceId = (long) resourceId;
      this.PoolType = poolType;
      this.ResourceVersion = resourceVersion;
      this.ResourcePlatform = resourcePlatform;
      this.State = state;
      this.ContainerId = containerId;
      this.DownloadUrl = downloadUrl;
      this.CreatedOn = createdOn;
    }

    [DataMember(IsRequired = true)]
    public long ResourceId { get; internal set; }

    [DataMember(IsRequired = true)]
    public string PoolType { get; internal set; }

    [DataMember(IsRequired = true)]
    public string ResourceVersion { get; internal set; }

    [DataMember(IsRequired = false)]
    public string ResourcePlatform { get; internal set; }

    [DataMember(IsRequired = true)]
    public MachineRequestResourceState State { get; internal set; }

    [DataMember(IsRequired = true)]
    public long ContainerId { get; internal set; }

    [DataMember(IsRequired = false)]
    public string RequestType { get; internal set; }

    [DataMember(IsRequired = false)]
    public string DownloadUrl { get; internal set; }

    [DataMember(IsRequired = false)]
    public DateTime? CreatedOn { get; internal set; }
  }
}
