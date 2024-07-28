// Decompiled with JetBrains decompiler
// Type: Nest.InlineGet`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class InlineGet<TDocument> : IInlineGet<TDocument> where TDocument : class
  {
    public FieldValues Fields { get; internal set; }

    public bool Found { get; internal set; }

    public TDocument Source { get; internal set; }
  }
}
