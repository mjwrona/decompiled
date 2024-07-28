// Decompiled with JetBrains decompiler
// Type: Nest.IReindexSource
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IReindexSource
  {
    [DataMember(Name = "index")]
    Indices Index { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "remote")]
    IRemoteSource Remote { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "slice")]
    ISlicedScroll Slice { get; set; }

    [DataMember(Name = "sort")]
    [Obsolete("Deprecated in 7.6.0. Instead consider using query filtering to find the desired subset of data.")]
    IList<ISort> Sort { get; set; }

    [DataMember(Name = "_source")]
    Fields Source { get; set; }
  }
}
