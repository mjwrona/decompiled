// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class TermVectorFilter : ITermVectorFilter
  {
    public int? MaximumDocumentFrequency { get; set; }

    public int? MaximumNumberOfTerms { get; set; }

    public int? MaximumTermFrequency { get; set; }

    public int? MaximumWordLength { get; set; }

    public int? MinimumDocumentFrequency { get; set; }

    public int? MinimumTermFrequency { get; set; }

    public int? MinimumWordLength { get; set; }
  }
}
