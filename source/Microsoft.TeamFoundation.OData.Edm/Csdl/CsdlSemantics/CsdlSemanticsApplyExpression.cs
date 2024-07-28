// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsApplyExpression
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
  internal class CsdlSemanticsApplyExpression : 
    CsdlSemanticsExpression,
    IEdmApplyExpression,
    IEdmExpression,
    IEdmElement,
    IEdmCheckable
  {
    private readonly CsdlApplyExpression expression;
    private readonly CsdlSemanticsSchema schema;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsApplyExpression, IEdmFunction> appliedFunctionCache = new Cache<CsdlSemanticsApplyExpression, IEdmFunction>();
    private static readonly Func<CsdlSemanticsApplyExpression, IEdmFunction> ComputeAppliedFunctionFunc = (Func<CsdlSemanticsApplyExpression, IEdmFunction>) (me => me.ComputeAppliedFunction());
    private readonly Cache<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>> argumentsCache = new Cache<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>>();
    private static readonly Func<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>> ComputeArgumentsFunc = (Func<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>>) (me => me.ComputeArguments());

    public CsdlSemanticsApplyExpression(
      CsdlApplyExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
      this.schema = schema;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.FunctionApplication;

    public IEdmFunction AppliedFunction => this.appliedFunctionCache.GetValue(this, CsdlSemanticsApplyExpression.ComputeAppliedFunctionFunc, (Func<CsdlSemanticsApplyExpression, IEdmFunction>) null);

    public IEnumerable<IEdmExpression> Arguments => this.argumentsCache.GetValue(this, CsdlSemanticsApplyExpression.ComputeArgumentsFunc, (Func<CsdlSemanticsApplyExpression, IEnumerable<IEdmExpression>>) null);

    public IEnumerable<EdmError> Errors => this.AppliedFunction is IUnresolvedElement ? this.AppliedFunction.Errors() : Enumerable.Empty<EdmError>();

    private IEdmFunction ComputeAppliedFunction()
    {
      if (this.expression.Function == null)
        return (IEdmFunction) null;
      IEnumerable<IEdmFunction> source1 = this.schema.FindOperations(this.expression.Function).OfType<IEdmFunction>();
      if (source1.Count<IEdmFunction>() == 0)
        return (IEdmFunction) new UnresolvedFunction(this.expression.Function, Strings.Bad_UnresolvedOperation((object) this.expression.Function), this.Location);
      IEnumerable<IEdmFunction> source2 = source1.Where<IEdmFunction>(new Func<IEdmFunction, bool>(this.IsMatchingFunction));
      int num = source2.Count<IEdmFunction>();
      if (num > 1)
      {
        IEnumerable<IEdmFunction> source3 = source2.Where<IEdmFunction>(new Func<IEdmFunction, bool>(this.IsExactMatch));
        return source3.Count<IEdmFunction>() != 1 ? (IEdmFunction) new UnresolvedFunction(this.expression.Function, Strings.Bad_AmbiguousOperation((object) this.expression.Function), this.Location) : source3.Single<IEdmFunction>();
      }
      return num == 0 ? (IEdmFunction) new UnresolvedFunction(this.expression.Function, Strings.Bad_OperationParametersDontMatch((object) this.expression.Function), this.Location) : source2.Single<IEdmFunction>();
    }

    private IEnumerable<IEdmExpression> ComputeArguments()
    {
      bool flag = this.expression.Function == null;
      List<IEdmExpression> arguments = new List<IEdmExpression>();
      foreach (CsdlExpressionBase expression in this.expression.Arguments)
      {
        if (flag)
          flag = false;
        else
          arguments.Add(CsdlSemanticsModel.WrapExpression(expression, this.bindingContext, this.schema));
      }
      return (IEnumerable<IEdmExpression>) arguments;
    }

    private bool IsMatchingFunction(IEdmOperation operation)
    {
      if (operation.Parameters.Count<IEdmOperationParameter>() != this.Arguments.Count<IEdmExpression>())
        return false;
      IEnumerator<IEdmExpression> enumerator = this.Arguments.GetEnumerator();
      foreach (IEdmOperationParameter parameter in operation.Parameters)
      {
        enumerator.MoveNext();
        if (!enumerator.Current.TryCast(parameter.Type, (IEdmType) this.bindingContext, false, out IEnumerable<EdmError> _))
          return false;
      }
      return true;
    }

    private bool IsExactMatch(IEdmOperation operation)
    {
      IEnumerator<IEdmExpression> enumerator = this.Arguments.GetEnumerator();
      foreach (IEdmOperationParameter parameter in operation.Parameters)
      {
        enumerator.MoveNext();
        if (!enumerator.Current.TryCast(parameter.Type, (IEdmType) this.bindingContext, true, out IEnumerable<EdmError> _))
          return false;
      }
      return true;
    }
  }
}
