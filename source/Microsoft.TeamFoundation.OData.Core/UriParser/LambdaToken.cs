// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LambdaToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public abstract class LambdaToken : QueryToken
  {
    private readonly QueryToken parent;
    private readonly string parameter;
    private readonly QueryToken expression;

    protected LambdaToken(QueryToken expression, string parameter, QueryToken parent)
    {
      this.expression = expression;
      this.parameter = parameter;
      this.parent = parent;
    }

    public QueryToken Parent => this.parent;

    public QueryToken Expression => this.expression;

    public string Parameter => this.parameter;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
