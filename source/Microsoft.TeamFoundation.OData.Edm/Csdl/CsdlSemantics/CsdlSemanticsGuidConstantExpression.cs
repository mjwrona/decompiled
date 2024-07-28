// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsGuidConstantExpression
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
  internal class CsdlSemanticsGuidConstantExpression : 
    CsdlSemanticsExpression,
    IEdmGuidConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmGuidValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsGuidConstantExpression, Guid> valueCache = new Cache<CsdlSemanticsGuidConstantExpression, Guid>();
    private static readonly Func<CsdlSemanticsGuidConstantExpression, Guid> ComputeValueFunc = (Func<CsdlSemanticsGuidConstantExpression, Guid>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsGuidConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public Guid Value => this.valueCache.GetValue(this, CsdlSemanticsGuidConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsGuidConstantExpression, Guid>) null);

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.GuidConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsGuidConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>>) null);

    private Guid ComputeValue()
    {
      Guid? result;
      return !EdmValueParser.TryParseGuid(this.expression.Value, out result) ? Guid.Empty : result.Value;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseGuid(this.expression.Value, out Guid? _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidGuid, Strings.ValueParser_InvalidGuid((object) this.expression.Value))
      };
    }
  }
}
