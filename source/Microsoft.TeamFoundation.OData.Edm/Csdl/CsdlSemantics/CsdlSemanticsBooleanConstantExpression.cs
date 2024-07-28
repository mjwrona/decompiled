// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsBooleanConstantExpression
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
  internal class CsdlSemanticsBooleanConstantExpression : 
    CsdlSemanticsExpression,
    IEdmBooleanConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmBooleanValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsBooleanConstantExpression, bool> valueCache = new Cache<CsdlSemanticsBooleanConstantExpression, bool>();
    private static readonly Func<CsdlSemanticsBooleanConstantExpression, bool> ComputeValueFunc = (Func<CsdlSemanticsBooleanConstantExpression, bool>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsBooleanConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public bool Value => this.valueCache.GetValue(this, CsdlSemanticsBooleanConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsBooleanConstantExpression, bool>) null);

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.BooleanConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsBooleanConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>>) null);

    private bool ComputeValue()
    {
      bool? result;
      return EdmValueParser.TryParseBool(this.expression.Value, out result) && result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseBool(this.expression.Value, out bool? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidBoolean, Strings.ValueParser_InvalidBoolean((object) this.expression.Value))
      };
    }
  }
}
