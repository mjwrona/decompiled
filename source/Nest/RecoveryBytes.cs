// Decompiled with JetBrains decompiler
// Type: Nest.RecoveryBytes
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class RecoveryBytes
  {
    [DataMember(Name = "percent")]
    public string Percent { get; internal set; }

    [DataMember(Name = "recovered")]
    public long Recovered { get; internal set; }

    [DataMember(Name = "reused")]
    public long Reused { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }
  }
}
