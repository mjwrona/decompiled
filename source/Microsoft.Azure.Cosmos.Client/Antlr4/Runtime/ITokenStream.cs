// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ITokenStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime
{
  internal interface ITokenStream : IIntStream
  {
    [return: NotNull]
    IToken LT(int k);

    [return: NotNull]
    IToken Get(int i);

    ITokenSource TokenSource { get; }

    [return: NotNull]
    string GetText(Interval interval);

    [return: NotNull]
    string GetText();

    [return: NotNull]
    string GetText(RuleContext ctx);

    [return: NotNull]
    string GetText(IToken start, IToken stop);
  }
}
