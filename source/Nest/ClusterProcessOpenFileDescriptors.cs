// Decompiled with JetBrains decompiler
// Type: Nest.ClusterProcessOpenFileDescriptors
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterProcessOpenFileDescriptors
  {
    [DataMember(Name = "avg")]
    public long Avg { get; internal set; }

    [DataMember(Name = "max")]
    public long Max { get; internal set; }

    [DataMember(Name = "min")]
    public long Min { get; internal set; }
  }
}
