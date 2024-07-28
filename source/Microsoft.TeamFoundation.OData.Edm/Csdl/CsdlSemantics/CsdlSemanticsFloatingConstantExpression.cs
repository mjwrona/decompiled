// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsFloatingConstantExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsFloatingConstantExpression : 
    CsdlSemanticsExpression,
    IEdmFloatingConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmFloatingValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsFloatingConstantExpression, double> valueCache = new Cache<CsdlSemanticsFloatingConstantExpression, double>();
    private static readonly Func<CsdlSemanticsFloatingConstantExpression, double> ComputeValueFunc = (Func<CsdlSemanticsFloatingConstantExpression, double>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsFloatingConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public double Value => this.valueCache.GetValue(this, CsdlSemanticsFloatingConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsFloatingConstantExpression, double>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.FloatingConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsFloatingConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsFloatingConstantExpression, IEnumerable<EdmError>>) null);

    private double ComputeValue()
    {
      double? result;
      return !EdmValueParser.TryParseFloat(this.expression.Value, out result) ? 0.0 : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseFloat(this.expression.Value, out double? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidFloatingPoint, Strings.ValueParser_InvalidFloatingPoint((object) this.expression.Value))
      };
    }
  }
}
