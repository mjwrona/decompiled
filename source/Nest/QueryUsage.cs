// Decompiled with JetBrains decompiler
// Type: Nest.QueryUsage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class QueryUsage
  {
    [DataMember(Name = "total")]
    public int Total { get; internal set; }

    [DataMember(Name = "paging")]
    public int Paging { get; internal set; }

    [DataMember(Name = "failed")]
    public int Failed { get; internal set; }

    [DataMember(Name = "count")]
    public int? Count { get; internal set; }
  }
}
