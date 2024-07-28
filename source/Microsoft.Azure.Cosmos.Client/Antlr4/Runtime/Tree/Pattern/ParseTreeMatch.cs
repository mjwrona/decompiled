// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.ParseTreeMatch
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Tree.Pattern
{
  internal class ParseTreeMatch
  {
    private readonly IParseTree tree;
    private readonly ParseTreePattern pattern;
    private readonly MultiMap<string, IParseTree> labels;
    private readonly IParseTree mismatchedNode;

    public ParseTreeMatch(
      IParseTree tree,
      ParseTreePattern pattern,
      MultiMap<string, IParseTree> labels,
      IParseTree mismatchedNode)
    {
      if (tree == null)
        throw new ArgumentException("tree cannot be null");
      if (pattern == null)
        throw new ArgumentException("pattern cannot be null");
      if (labels == null)
        throw new ArgumentException("labels cannot be null");
      this.tree = tree;
      this.pattern = pattern;
      this.labels = labels;
      this.mismatchedNode = mismatchedNode;
    }

    [return: Nullable]
    public virtual IParseTree Get(string label)
    {
      IList<IParseTree> parseTreeList = this.labels.Get<string, IList<IParseTree>>(label);
      return parseTreeList == null || parseTreeList.Count == 0 ? (IParseTree) null : parseTreeList[parseTreeList.Count - 1];
    }

    [return: NotNull]
    public virtual IList<IParseTree> GetAll(string label) => this.labels.Get<string, IList<IParseTree>>(label) ?? (IList<IParseTree>) Antlr4.Runtime.Sharpen.Collections.EmptyList<IParseTree>();

    [NotNull]
    public virtual MultiMap<string, IParseTree> Labels => this.labels;

    [Nullable]
    public virtual IParseTree MismatchedNode => this.mismatchedNode;

    public virtual bool Succeeded => this.mismatchedNode == null;

    [NotNull]
    public virtual ParseTreePattern Pattern => this.pattern;

    [NotNull]
    public virtual IParseTree Tree => this.tree;

    public override string ToString() => string.Format("Match {0}; found {1} labels", this.Succeeded ? (object) "succeeded" : (object) "failed", (object) this.Labels.Count);
  }
}
