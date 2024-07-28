// Decompiled with JetBrains decompiler
// Type: Nest.ClusterFileSystem
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterFileSystem
  {
    [DataMember(Name = "available_in_bytes")]
    public long AvailableInBytes { get; internal set; }

    [DataMember(Name = "free_in_bytes")]
    public long FreeInBytes { get; internal set; }

    [DataMember(Name = "total_in_bytes")]
    public long TotalInBytes { get; internal set; }
  }
}
