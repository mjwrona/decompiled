// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.StringLiteralToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Diagnostics;

namespace Microsoft.OData.UriParser
{
  [DebuggerDisplay("StringLiteralToken ({text})")]
  internal sealed class StringLiteralToken : QueryToken
  {
    private readonly string text;

    internal StringLiteralToken(string text) => this.text = text;

    public override QueryTokenKind Kind => QueryTokenKind.StringLiteral;

    internal string Text => this.text;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => throw new NotImplementedException();
  }
}
