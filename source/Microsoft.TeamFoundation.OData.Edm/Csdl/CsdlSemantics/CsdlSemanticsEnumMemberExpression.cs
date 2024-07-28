// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEnumMemberExpression
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
  internal class CsdlSemanticsEnumMemberExpression : 
    CsdlSemanticsExpression,
    IEdmEnumMemberExpression,
    IEdmExpression,
    IEdmElement,
    IEdmCheckable
  {
    private readonly CsdlEnumMemberExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>> referencedCache = new Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>>();
    private static readonly Func<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>> ComputeReferencedFunc = (Func<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>>) (me => me.ComputeReferenced());
    private readonly Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsEnumMemberExpression(
      CsdlEnumMemberExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.EnumMember;

    public IEnumerable<IEdmEnumMember> EnumMembers => this.referencedCache.GetValue(this, CsdlSemanticsEnumMemberExpression.ComputeReferencedFunc, (Func<CsdlSemanticsEnumMemberExpression, IEnumerable<IEdmEnumMember>>) null);

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsEnumMemberExpression.ComputeErrorsFunc, (Func<CsdlSemanticsEnumMemberExpression, IEnumerable<EdmError>>) null);

    private IEnumerable<IEdmEnumMember> ComputeReferenced()
    {
      IEnumerable<IEdmEnumMember> result;
      return !EdmEnumValueParser.TryParseEnumMember(this.expression.EnumMemberPath, (IEdmModel) this.Schema.Model, this.Location, out result) ? (IEnumerable<IEdmEnumMember>) null : result;
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      if (EdmEnumValueParser.TryParseEnumMember(this.expression.EnumMemberPath, (IEdmModel) this.Schema.Model, this.Location, out IEnumerable<IEdmEnumMember> _))
        return Enumerable.Empty<EdmError>();
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.Location, EdmErrorCode.InvalidEnumMemberPath, Strings.CsdlParser_InvalidEnumMemberPath((object) this.expression.EnumMemberPath))
      };
    }
  }
}
