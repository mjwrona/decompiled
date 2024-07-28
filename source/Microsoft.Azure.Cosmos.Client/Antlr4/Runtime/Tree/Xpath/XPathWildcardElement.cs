// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Xpath.XPathWildcardElement
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Tree.Xpath
{
  internal class XPathWildcardElement : XPathElement
  {
    public XPathWildcardElement()
      : base("*")
    {
    }

    public override ICollection<IParseTree> Evaluate(IParseTree t)
    {
      if (this.invert)
        return (ICollection<IParseTree>) new List<IParseTree>();
      IList<IParseTree> parseTreeList = (IList<IParseTree>) new List<IParseTree>();
      foreach (ITree child in (IEnumerable<ITree>) Trees.GetChildren((ITree) t))
        parseTreeList.Add((IParseTree) child);
      return (ICollection<IParseTree>) parseTreeList;
    }
  }
}
