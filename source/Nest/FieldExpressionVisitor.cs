// Decompiled with JetBrains decompiler
// Type: Nest.FieldExpressionVisitor
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
  internal class FieldExpressionVisitor : ExpressionVisitor
  {
    private readonly IConnectionSettingsValues _settings;
    private readonly Stack<string> _stack = new Stack<string>();

    public FieldExpressionVisitor(IConnectionSettingsValues settings) => this._settings = settings;

    public string Resolve(Expression expression, bool toLastToken = false)
    {
      this.Visit(expression);
      return toLastToken ? this._stack.Last<string>() : this._stack.Aggregate<string, StringBuilder>(new StringBuilder(this._stack.Sum<string>((Func<string, int>) (s => s.Length)) + (this._stack.Count - 1)), (Func<StringBuilder, string, StringBuilder>) ((sb, name) => (sb.Length > 0 ? sb.Append(".") : sb).Append(name))).ToString();
    }

    public string Resolve(MemberInfo info)
    {
      if (info == (MemberInfo) null)
        return (string) null;
      string name = info.Name;
      IPropertyMapping propertyMapping;
      if (this._settings.PropertyMappings.TryGetValue(info, out propertyMapping))
        return propertyMapping.Name;
      ElasticsearchPropertyAttributeBase propertyAttributeBase = ElasticsearchPropertyAttributeBase.From(info);
      return propertyAttributeBase != null && !propertyAttributeBase.Name.IsNullOrEmpty() ? propertyAttributeBase.Name : this._settings.PropertyMappingProvider?.CreatePropertyMapping(info)?.Name ?? this._settings.DefaultFieldNameInferrer(name);
    }

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
        FieldExpressionVisitor.VisitConstantOrVariable(methodCall, this._stack);
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
        FieldExpressionVisitor.VisitConstantOrVariable(methodCall, this._stack);
        this.Visit(methodCall.Object);
        return (Expression) methodCall;
      }
      if (!FieldExpressionVisitor.IsLinqOperator(methodCall.Method))
        return base.VisitMethodCall(methodCall);
      for (int index = 1; index < methodCall.Arguments.Count; ++index)
        this.Visit(methodCall.Arguments[index]);
      this.Visit(methodCall.Arguments[0]);
      return (Expression) methodCall;
    }

    private static void VisitConstantOrVariable(
      MethodCallExpression methodCall,
      Stack<string> stack)
    {
      Expression body = methodCall.Arguments.Last<Expression>();
      string str = body is ConstantExpression constantExpression ? constantExpression.Value.ToString() : Expression.Lambda(body).Compile().DynamicInvoke().ToString();
      stack.Push(str);
    }

    private static bool IsLinqOperator(MethodInfo methodInfo) => (!(methodInfo.DeclaringType != typeof (Queryable)) || !(methodInfo.DeclaringType != typeof (Enumerable))) && methodInfo.GetCustomAttribute<ExtensionAttribute>() != null;
  }
}
