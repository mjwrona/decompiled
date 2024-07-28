// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.ITerminalNode
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Tree
{
  internal interface ITerminalNode : IParseTree, ISyntaxTree, ITree
  {
    IToken Symbol { get; }

    IRuleNode Parent { get; }
  }
}
