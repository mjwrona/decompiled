// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ITokenFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal interface ITokenFactory
  {
    [return: NotNull]
    IToken Create(
      Tuple<ITokenSource, ICharStream> source,
      int type,
      string text,
      int channel,
      int start,
      int stop,
      int line,
      int charPositionInLine);

    [return: NotNull]
    IToken Create(int type, string text);
  }
}
