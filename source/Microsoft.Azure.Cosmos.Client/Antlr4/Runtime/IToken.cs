// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.IToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime
{
  internal interface IToken
  {
    string Text { get; }

    int Type { get; }

    int Line { get; }

    int Column { get; }

    int Channel { get; }

    int TokenIndex { get; }

    int StartIndex { get; }

    int StopIndex { get; }

    ITokenSource TokenSource { get; }

    ICharStream InputStream { get; }
  }
}
