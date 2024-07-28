// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ShardingInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class ShardingInfo
  {
    [DataMember]
    public Guid MigrationId { get; set; }

    [DataMember]
    public StorageType StorageType { get; set; }

    [DataMember]
    public int VirtualNodes { get; set; }

    public override string ToString() => string.Format("StorageType: {0}, VirtualNodes: {1}", (object) this.StorageType, (object) this.VirtualNodes);
  }
}
