// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsPathExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsPathExpression : 
    CsdlSemanticsExpression,
    IEdmPathExpression,
    IEdmExpression,
    IEdmElement
  {
    protected readonly CsdlPathExpression Expression;
    protected readonly IEdmEntityType BindingContext;
    protected readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> PathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();
    protected static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (Func<CsdlSemanticsPathExpression, IEnumerable<string>>) (me => me.ComputePath());

    public CsdlSemanticsPathExpression(
      CsdlPathExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.Expression = expression;
      this.BindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.Expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.Path;

    public IEnumerable<string> PathSegments => this.PathCache.GetValue(this, CsdlSemanticsPathExpression.ComputePathFunc, (Func<CsdlSemanticsPathExpression, IEnumerable<string>>) null);

    public string Path => this.Expression.Path;

    private IEnumerable<string> ComputePath() => (IEnumerable<string>) this.Expression.Path.Split(new char[1]
    {
      '/'
    }, StringSplitOptions.None);
  }
}
