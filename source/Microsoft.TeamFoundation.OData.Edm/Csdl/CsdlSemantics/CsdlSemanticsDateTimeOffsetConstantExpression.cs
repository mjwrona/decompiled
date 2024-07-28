// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDateTimeOffsetConstantExpression
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
  internal class CsdlSemanticsDateTimeOffsetConstantExpression : 
    CsdlSemanticsExpression,
    IEdmDateTimeOffsetConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDateTimeOffsetValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> valueCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset>();
    private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> ComputeValueFunc = (Func<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsDateTimeOffsetConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public DateTimeOffset Value => this.valueCache.GetValue(this, CsdlSemanticsDateTimeOffsetConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.DateTimeOffsetConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsDateTimeOffsetConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>>) null);

    private DateTimeOffset ComputeValue()
    {
      DateTimeOffset? result;
      return !EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out result) ? DateTimeOffset.MinValue : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out DateTimeOffset? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidDateTimeOffset, Strings.ValueParser_InvalidDateTimeOffset((object) this.expression.Value))
      };
    }
  }
}
