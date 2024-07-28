// Decompiled with JetBrains decompiler
// Type: Nest.ToStringExpressionVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nest
{
  internal class ToStringExpressionVisitor : ExpressionVisitor
  {
    private readonly Stack<string> _stack = new Stack<string>();

    public bool Cachable { get; private set; } = true;

    public string Resolve(Expression expression, bool toLastToken = false)
    {
      this.Visit(expression);
      return toLastToken ? this._stack.Last<string>() : this._stack.Aggregate<string, StringBuilder>(new StringBuilder(), (Func<StringBuilder, string, StringBuilder>) ((sb, name) => (sb.Length > 0 ? sb.Append(".") : sb).Append(name))).ToString();
    }

    public string Resolve(MemberInfo info) => !(info == (MemberInfo) null) ? info.Name : (string) null;

    protected override Expression VisitMember(MemberExpression expression)
    {
      if (this._stack == null)
        return base.VisitMember(expression);
      this._stack.Push(this.Resolve(expression.Member));
      return base.VisitMember(expression);
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCall)
    {
      if (methodCall.Method.Name == "Suffix" && methodCall.Arguments.Any<Expression>())
      {
        this.VisitConstantOrVariable(methodCall, this._stack);
        this.Visit(new ReadOnlyCollection<Expression>((IList<Expression>) new List<Expression>()
        {
          methodCall.Arguments.First<Expression>()
        }));
        return (Expression) methodCall;
      }
      if (methodCall.Method.Name == "get_Item" && methodCall.Arguments.Any<Expression>())
      {
        Type type = methodCall.Object.Type;
        if (!typeof (IDictionary).IsAssignableFrom(type) && !typeof (IDictionary<,>).IsAssignableFrom(type) && (!type.IsGenericType || !(type.GetGenericTypeDefinition() == typeof (IDictionary<,>))))
          return base.VisitMethodCall(methodCall);
        this.VisitConstantOrVariable(methodCall, this._stack);
        this.Visit(methodCall.Object);
        return (Expression) methodCall;
      }
      if (!ToStringExpressionVisitor.IsLinqOperator(methodCall.Method))
        return base.VisitMethodCall(methodCall);
      for (int index = 1; index < methodCall.Arguments.Count; ++index)
        this.Visit(methodCall.Arguments[index]);
      this.Visit(methodCall.Arguments[0]);
      return (Expression) methodCall;
    }

    private void VisitConstantOrVariable(MethodCallExpression methodCall, Stack<string> stack)
    {
      Expression expression = methodCall.Arguments.Last<Expression>();
      switch (expression)
      {
        case ConstantExpression constantExpression:
          stack.Push(constantExpression.Value.ToString());
          break;
        case MemberExpression memberExpression:
          this.Cachable = false;
          stack.Push(memberExpression.Member.Name);
          break;
        default:
          this.Cachable = false;
          stack.Push(expression.ToString());
          break;
      }
    }

    private static bool IsLinqOperator(MethodInfo methodInfo) => (!(methodInfo.DeclaringType != typeof (Queryable)) || !(methodInfo.DeclaringType != typeof (Enumerable))) && methodInfo.GetCustomAttribute<ExtensionAttribute>() != null;
  }
}
