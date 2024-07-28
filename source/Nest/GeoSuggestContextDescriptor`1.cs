// Decompiled with JetBrains decompiler
// Type: Nest.GeoSuggestContextDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class GeoSuggestContextDescriptor<T> : 
    SuggestContextDescriptorBase<GeoSuggestContextDescriptor<T>, IGeoSuggestContext, T>,
    IGeoSuggestContext,
    ISuggestContext
    where T : class
  {
    protected override string Type => "geo";

    [Obsolete("No longer valid. Will be removed in next major release")]
    bool? IGeoSuggestContext.Neighbors { get; set; }

    IEnumerable<string> IGeoSuggestContext.Precision { get; set; }

    public GeoSuggestContextDescriptor<T> Precision(params string[] precisions) => this.Assign<string[]>(precisions, (Action<IGeoSuggestContext, string[]>) ((a, v) => a.Precision = (IEnumerable<string>) v));

    [Obsolete("No longer valid. Will be removed in next major release")]
    public GeoSuggestContextDescriptor<T> Neighbors(bool? neighbors = true) => this.Assign<bool?>(neighbors, (Action<IGeoSuggestContext, bool?>) ((a, v) => a.Neighbors = v));
  }
}
