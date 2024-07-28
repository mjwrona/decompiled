// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.PartitionContainer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Partitioning
{
  [DataContract]
  public class PartitionContainer
  {
    public PartitionContainer() => this.Tags = new List<string>();

    [DataMember]
    public Guid ContainerId { get; set; }

    [DataMember]
    public Guid ContainerType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Address { get; set; }

    [DataMember]
    public string InternalAddress { get; set; }

    [DataMember]
    public int MaxPartitions { get; set; }

    [DataMember]
    public PartitionContainerStatus Status { get; set; }

    [DataMember]
    public List<string> Tags { get; set; }
  }
}
