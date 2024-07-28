// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.TagChunk
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime.Tree.Pattern
{
  internal class TagChunk : Chunk
  {
    private readonly string tag;
    private readonly string label;

    public TagChunk(string tag)
      : this((string) null, tag)
    {
    }

    public TagChunk(string label, string tag)
    {
      if (string.IsNullOrEmpty(tag))
        throw new ArgumentException("tag cannot be null or empty");
      this.label = label;
      this.tag = tag;
    }

    [NotNull]
    public string Tag => this.tag;

    [Nullable]
    public string Label => this.label;

    public override string ToString() => this.label != null ? this.label + ":" + this.tag : this.tag;
  }
}
