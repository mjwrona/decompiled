// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionParameterToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class FunctionParameterToken : QueryToken
  {
    public static FunctionParameterToken[] EmptyParameterList = new FunctionParameterToken[0];
    private readonly string parameterName;
    private readonly QueryToken valueToken;

    public FunctionParameterToken(string parameterName, QueryToken valueToken)
    {
      this.parameterName = parameterName;
      this.valueToken = valueToken;
    }

    public string ParameterName => this.parameterName;

    public QueryToken ValueToken => this.valueToken;

    public override QueryTokenKind Kind => QueryTokenKind.FunctionParameter;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
