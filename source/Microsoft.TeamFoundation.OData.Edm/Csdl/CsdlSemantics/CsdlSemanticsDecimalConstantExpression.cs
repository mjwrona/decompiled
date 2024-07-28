// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDecimalConstantExpression
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
  internal class CsdlSemanticsDecimalConstantExpression : 
    CsdlSemanticsExpression,
    IEdmDecimalConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDecimalValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsDecimalConstantExpression, Decimal> valueCache = new Cache<CsdlSemanticsDecimalConstantExpression, Decimal>();
    private static readonly Func<CsdlSemanticsDecimalConstantExpression, Decimal> ComputeValueFunc = (Func<CsdlSemanticsDecimalConstantExpression, Decimal>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsDecimalConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public Decimal Value => this.valueCache.GetValue(this, CsdlSemanticsDecimalConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsDecimalConstantExpression, Decimal>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.DecimalConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsDecimalConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>>) null);

    private Decimal ComputeValue()
    {
      Decimal? result;
      return !EdmValueParser.TryParseDecimal(this.expression.Value, out result) ? 0M : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseDecimal(this.expression.Value, out Decimal? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidDecimal, Strings.ValueParser_InvalidDecimal((object) this.expression.Value))
      };
    }
  }
}
