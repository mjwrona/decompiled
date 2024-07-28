// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionCallToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class FunctionCallToken : QueryToken
  {
    private readonly string name;
    private readonly IEnumerable<FunctionParameterToken> arguments;
    private readonly QueryToken source;

    public FunctionCallToken(string name, IEnumerable<QueryToken> argumentValues)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, nameof (name));
      this.name = name;
      this.arguments = argumentValues == null ? (IEnumerable<FunctionParameterToken>) new ReadOnlyEnumerableForUriParser<FunctionParameterToken>((IEnumerable<FunctionParameterToken>) FunctionParameterToken.EmptyParameterList) : (IEnumerable<FunctionParameterToken>) new ReadOnlyEnumerableForUriParser<FunctionParameterToken>(argumentValues.Select<QueryToken, FunctionParameterToken>((Func<QueryToken, FunctionParameterToken>) (v => new FunctionParameterToken((string) null, v))));
      this.source = (QueryToken) null;
    }

    public FunctionCallToken(
      string name,
      IEnumerable<FunctionParameterToken> arguments,
      QueryToken source)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, nameof (name));
      this.name = name;
      this.arguments = (IEnumerable<FunctionParameterToken>) new ReadOnlyEnumerableForUriParser<FunctionParameterToken>((IEnumerable<FunctionParameterToken>) ((object) arguments ?? (object) FunctionParameterToken.EmptyParameterList));
      this.source = source;
    }

    public override QueryTokenKind Kind => QueryTokenKind.FunctionCall;

    public string Name => this.name;

    public IEnumerable<FunctionParameterToken> Arguments => this.arguments;

    public QueryToken Source => this.source;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
