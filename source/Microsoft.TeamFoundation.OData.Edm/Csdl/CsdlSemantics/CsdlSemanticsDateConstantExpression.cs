// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDateConstantExpression
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
  internal class CsdlSemanticsDateConstantExpression : 
    CsdlSemanticsExpression,
    IEdmDateConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDateValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsDateConstantExpression, Date> valueCache = new Cache<CsdlSemanticsDateConstantExpression, Date>();
    private static readonly Func<CsdlSemanticsDateConstantExpression, Date> ComputeValueFunc = (Func<CsdlSemanticsDateConstantExpression, Date>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsDateConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public Date Value => this.valueCache.GetValue(this, CsdlSemanticsDateConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsDateConstantExpression, Date>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.DateConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsDateConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsDateConstantExpression, IEnumerable<EdmError>>) null);

    private Date ComputeValue()
    {
      Date? result;
      return !EdmValueParser.TryParseDate(this.expression.Value, out result) ? Date.MinValue : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseDate(this.expression.Value, out Date? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidDate, Strings.ValueParser_InvalidDate((object) this.expression.Value))
      };
    }
  }
}
