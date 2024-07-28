// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsBinaryConstantExpression
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
  internal class CsdlSemanticsBinaryConstantExpression : 
    CsdlSemanticsExpression,
    IEdmBinaryConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmBinaryValue,
    IEdmPrimitiveValue,
    IEdmValue,
    IEdmCheckable
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsBinaryConstantExpression, byte[]> valueCache = new Cache<CsdlSemanticsBinaryConstantExpression, byte[]>();
    private static readonly Func<CsdlSemanticsBinaryConstantExpression, byte[]> ComputeValueFunc = (Func<CsdlSemanticsBinaryConstantExpression, byte[]>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsBinaryConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public byte[] Value => this.valueCache.GetValue(this, CsdlSemanticsBinaryConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsBinaryConstantExpression, byte[]>) null);

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.BinaryConstant;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsBinaryConstantExpression.ComputeErrorsFunc, (Func<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>>) null);

    private byte[] ComputeValue()
    {
      byte[] result;
      return !EdmValueParser.TryParseBinary(this.expression.Value, out result) ? new byte[0] : result;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmValueParser.TryParseBinary(this.expression.Value, out byte[] _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidBinary, Strings.ValueParser_InvalidBinary((object) this.expression.Value))
      };
    }
  }
}
