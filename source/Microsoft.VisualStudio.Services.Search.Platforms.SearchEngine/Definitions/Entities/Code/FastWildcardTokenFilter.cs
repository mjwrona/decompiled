// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.FastWildcardTokenFilter
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class FastWildcardTokenFilter : TokenFilterBase
  {
    public FastWildcardTokenFilter()
      : base("fast_wildcard")
    {
    }

    [DataMember(Name = "min_edge_gram")]
    public int? MinEdgeGram { get; set; }

    [DataMember(Name = "inline_gram")]
    public int? InlineGram { get; set; }
  }
}
