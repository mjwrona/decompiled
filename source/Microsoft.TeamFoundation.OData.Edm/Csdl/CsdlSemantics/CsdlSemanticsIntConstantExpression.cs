// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsIntConstantExpression
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
  internal class CsdlSemanticsIntConstantExpression : 
    CsdlSemanticsExpression,
    IEdmIntegerConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmIntegerValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsIntConstantExpression, long> valueCache = new Cache<CsdlSemanticsIntConstantExpression, long>();
    private static readonly Func<CsdlSemanticsIntConstantExpression, long> ComputeValueFunc = (Func<CsdlSemanticsIntConstantExpression, long>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsIntConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public long Value => this.valueCache.GetValue(this, CsdlSemanticsIntConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsIntConstantExpression, long>) null);

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.IntegerConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsIntConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>>) null);

    private long ComputeValue()
    {
      long? result;
      return !EdmValueParser.TryParseLong(this.expression.Value, out result) ? 0L : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseLong(this.expression.Value, out long? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidInteger, Strings.ValueParser_InvalidInteger((object) this.expression.Value))
      };
    }
  }
}
