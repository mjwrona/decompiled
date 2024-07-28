// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SuggestContextQuery : ISuggestContextQuery
  {
    public double? Boost { get; set; }

    public Context Context { get; set; }

    public Union<Distance[], int[]> Neighbours { get; set; }

    public Union<Distance, int> Precision { get; set; }

    public bool? Prefix { get; set; }
  }
}
