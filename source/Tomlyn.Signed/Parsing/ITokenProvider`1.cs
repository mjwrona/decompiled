// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.ITokenProvider`1
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Collections.Generic;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Parsing
{
  internal interface ITokenProvider<out TSourceView> where TSourceView : ISourceView
  {
    bool HasErrors { get; }

    TSourceView Source { get; }

    LexerState State { get; set; }

    bool MoveNext();

    SyntaxTokenValue Token { get; }

    IEnumerable<DiagnosticMessage> Errors { get; }
  }
}
