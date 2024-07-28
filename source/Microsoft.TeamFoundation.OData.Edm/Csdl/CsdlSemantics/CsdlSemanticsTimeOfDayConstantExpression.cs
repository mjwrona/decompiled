// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTimeOfDayConstantExpression
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
  internal class CsdlSemanticsTimeOfDayConstantExpression : 
    CsdlSemanticsExpression,
    IEdmTimeOfDayConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmTimeOfDayValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsTimeOfDayConstantExpression, TimeOfDay> valueCache = new Cache<CsdlSemanticsTimeOfDayConstantExpression, TimeOfDay>();
    private static readonly Func<CsdlSemanticsTimeOfDayConstantExpression, TimeOfDay> ComputeValueFunc = (Func<CsdlSemanticsTimeOfDayConstantExpression, TimeOfDay>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsTimeOfDayConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsTimeOfDayConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsTimeOfDayConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsTimeOfDayConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsTimeOfDayConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public TimeOfDay Value => this.valueCache.GetValue(this, CsdlSemanticsTimeOfDayConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsTimeOfDayConstantExpression, TimeOfDay>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.TimeOfDayConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsTimeOfDayConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsTimeOfDayConstantExpression, IEnumerable<EdmError>>) null);

    private TimeOfDay ComputeValue()
    {
      TimeOfDay? result;
      return !EdmValueParser.TryParseTimeOfDay(this.expression.Value, out result) ? TimeOfDay.MinValue : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseTimeOfDay(this.expression.Value, out TimeOfDay? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidTimeOfDay, Strings.ValueParser_InvalidTimeOfDay((object) this.expression.Value))
      };
    }
  }
}
