// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDurationConstantExpression
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
  internal class CsdlSemanticsDurationConstantExpression : 
    CsdlSemanticsExpression,
    IEdmDurationConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDurationValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsDurationConstantExpression, TimeSpan> valueCache = new Cache<CsdlSemanticsDurationConstantExpression, TimeSpan>();
    private static readonly Func<CsdlSemanticsDurationConstantExpression, TimeSpan> ComputeValueFunc = (Func<CsdlSemanticsDurationConstantExpression, TimeSpan>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsDurationConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public TimeSpan Value => this.valueCache.GetValue(this, CsdlSemanticsDurationConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsDurationConstantExpression, TimeSpan>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.DurationConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsDurationConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>>) null);

    private TimeSpan ComputeValue()
    {
      TimeSpan? result;
      return !EdmValueParser.TryParseDuration(this.expression.Value, out result) ? TimeSpan.Zero : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseDuration(this.expression.Value, out TimeSpan? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidDuration, Strings.ValueParser_InvalidDuration((object) this.expression.Value))
      };
    }
  }
}
