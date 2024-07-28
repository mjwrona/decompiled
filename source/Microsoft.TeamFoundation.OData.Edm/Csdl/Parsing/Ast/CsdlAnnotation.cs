// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlAnnotation : CsdlElement
  {
    private readonly CsdlExpressionBase expression;
    private readonly string qualifier;
    private readonly string term;

    public CsdlAnnotation(
      string term,
      string qualifier,
      CsdlExpressionBase expression,
      CsdlLocation location)
      : base(location)
    {
      this.expression = expression;
      this.qualifier = qualifier;
      this.term = term;
    }

    public CsdlExpressionBase Expression => this.expression;

    public string Qualifier => this.qualifier;

    public string Term => this.term;
  }
}
