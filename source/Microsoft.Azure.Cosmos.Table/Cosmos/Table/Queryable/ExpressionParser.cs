// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ExpressionParser
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ExpressionParser : DataServiceALinqExpressionVisitor
  {
    private int? takeCount;

    internal TableRequestOptions RequestOptions { get; set; }

    internal OperationContext OperationContext { get; set; }

    internal ConstantExpression Resolver { get; set; }

    internal ProjectionQueryOptionExpression Projection { get; set; }

    public int? TakeCount
    {
      get => this.takeCount;
      set
      {
        if (value.HasValue && value.Value <= 0)
          throw new ArgumentException("Take count must be positive and greater than 0.");
        this.takeCount = value;
      }
    }

    public string FilterString { get; set; }

    public IList<string> SelectColumns { get; set; }

    internal ExpressionParser() => this.SelectColumns = (IList<string>) new List<string>();

    internal void Translate(Expression e) => this.Visit(e);

    internal override Expression VisitMethodCall(MethodCallExpression m) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The method '{0}' is not supported.", (object) m.Method.Name));

    internal override Expression VisitUnary(UnaryExpression u) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The unary operator '{0}' is not supported.", (object) u.NodeType.ToString()));

    internal override Expression VisitBinary(BinaryExpression b) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The binary operator '{0}' is not supported.", (object) b.NodeType.ToString()));

    internal override Expression VisitConstant(ConstantExpression c) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The constant for '{0}' is not supported.", c.Value));

    internal override Expression VisitTypeIs(TypeBinaryExpression b) => throw new NotSupportedException("An operation between an expression and a type is not supported.");

    internal override Expression VisitConditional(ConditionalExpression c) => throw new NotSupportedException("The conditional expression is not supported.");

    internal override Expression VisitParameter(ParameterExpression p) => throw new NotSupportedException("The parameter expression is not supported.");

    internal override Expression VisitMemberAccess(MemberExpression m) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member access of '{0}' is not supported.", (object) m.Member.Name));

    internal override Expression VisitLambda(LambdaExpression lambda) => throw new NotSupportedException("Lambda Expressions not supported.");

    internal override NewExpression VisitNew(NewExpression nex) => throw new NotSupportedException("New Expressions not supported.");

    internal override Expression VisitMemberInit(MemberInitExpression init) => throw new NotSupportedException("Member Init Expressions not supported.");

    internal override Expression VisitListInit(ListInitExpression init) => throw new NotSupportedException("List Init Expressions not supported.");

    internal override Expression VisitNewArray(NewArrayExpression na) => throw new NotSupportedException("New Array Expressions not supported.");

    internal override Expression VisitInvocation(InvocationExpression iv) => throw new NotSupportedException("Invocation Expressions not supported.");

    internal override Expression VisitNavigationPropertySingletonExpression(
      NavigationPropertySingletonExpression npse)
    {
      throw new NotSupportedException("Navigation not supported.");
    }

    internal override Expression VisitResourceSetExpression(ResourceSetExpression rse)
    {
      this.VisitQueryOptions((ResourceExpression) rse);
      return (Expression) rse;
    }

    internal void VisitQueryOptions(ResourceExpression re)
    {
      if (!re.HasQueryOptions)
        return;
      if (re is ResourceSetExpression resourceSetExpression)
      {
        IEnumerator enumerator = (IEnumerator) resourceSetExpression.SequenceQueryOptions.GetEnumerator();
        while (enumerator.MoveNext())
        {
          Expression current = (Expression) enumerator.Current;
          switch (current.NodeType)
          {
            case (ExpressionType) 10003:
              this.VisitQueryOptionExpression((TakeQueryOptionExpression) current);
              continue;
            case (ExpressionType) 10006:
              this.VisitQueryOptionExpression((FilterQueryOptionExpression) current);
              continue;
            case (ExpressionType) 10009:
              this.VisitQueryOptionExpression((RequestOptionsQueryOptionExpression) current);
              continue;
            case (ExpressionType) 10010:
              this.VisitQueryOptionExpression((OperationContextQueryOptionExpression) current);
              continue;
            case (ExpressionType) 10011:
              this.VisitQueryOptionExpression((EntityResolverQueryOptionExpression) current);
              continue;
            default:
              continue;
          }
        }
      }
      if (re.Projection != null && re.Projection.Paths.Count > 0)
      {
        this.Projection = re.Projection;
        this.SelectColumns = (IList<string>) re.Projection.Paths;
      }
      if (re.CustomQueryOptions.Count <= 0)
        return;
      this.VisitCustomQueryOptions(re.CustomQueryOptions);
    }

    internal virtual void VisitQueryOptionExpression(RequestOptionsQueryOptionExpression roqoe) => this.RequestOptions = (TableRequestOptions) roqoe.RequestOptions.Value;

    internal virtual void VisitQueryOptionExpression(OperationContextQueryOptionExpression ocqoe) => this.OperationContext = (OperationContext) ocqoe.OperationContext.Value;

    internal virtual void VisitQueryOptionExpression(EntityResolverQueryOptionExpression erqoe) => this.Resolver = erqoe.Resolver;

    internal virtual void VisitQueryOptionExpression(TakeQueryOptionExpression tqoe) => this.TakeCount = new int?((int) tqoe.TakeAmount.Value);

    internal virtual void VisitQueryOptionExpression(FilterQueryOptionExpression fqoe) => this.FilterString = ExpressionParser.ExpressionToString(fqoe.Predicate);

    internal void VisitCustomQueryOptions(
      Dictionary<ConstantExpression, ConstantExpression> options)
    {
      throw new NotSupportedException();
    }

    private static string ExpressionToString(Expression expression) => ExpressionWriter.ExpressionToString(expression);
  }
}
