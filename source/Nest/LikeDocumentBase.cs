// Decompiled with JetBrains decompiler
// Type: Nest.LikeDocumentBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class LikeDocumentBase : ILikeDocument
  {
    public object Document { get; set; }

    public Fields Fields { get; set; }

    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    public Routing Routing { get; set; }
  }
}
