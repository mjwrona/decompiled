// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.InputBinder
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal sealed class InputBinder : DataServiceALinqExpressionVisitor
  {
    private readonly HashSet<ResourceExpression> referencedInputs = new HashSet<ResourceExpression>((IEqualityComparer<ResourceExpression>) EqualityComparer<ResourceExpression>.Default);
    private readonly ResourceExpression input;
    private readonly ResourceSetExpression inputSet;
    private readonly ParameterExpression inputParameter;

    private InputBinder(ResourceExpression resource, ParameterExpression setReferenceParam)
    {
      this.input = resource;
      this.inputSet = resource as ResourceSetExpression;
      this.inputParameter = setReferenceParam;
    }

    internal static Expression Bind(
      Expression e,
      ResourceExpression currentInput,
      ParameterExpression inputParameter,
      List<ResourceExpression> referencedInputs)
    {
      InputBinder inputBinder = new InputBinder(currentInput, inputParameter);
      Expression expression = inputBinder.Visit(e);
      referencedInputs.AddRange((IEnumerable<ResourceExpression>) inputBinder.referencedInputs);
      return expression;
    }

    internal override Expression VisitMemberAccess(MemberExpression m)
    {
      if (this.inputSet == null || !this.inputSet.HasTransparentScope)
        return base.VisitMemberAccess(m);
      ParameterExpression parameterExpression = (ParameterExpression) null;
      Stack<PropertyInfo> propertyInfoStack = new Stack<PropertyInfo>();
      for (MemberExpression memberExpression = m; memberExpression != null && memberExpression.Member.MemberType == MemberTypes.Property && memberExpression.Expression != null; memberExpression = memberExpression.Expression as MemberExpression)
      {
        propertyInfoStack.Push((PropertyInfo) memberExpression.Member);
        if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
          parameterExpression = (ParameterExpression) memberExpression.Expression;
      }
      if (parameterExpression != this.inputParameter || propertyInfoStack.Count == 0)
        return (Expression) m;
      ResourceExpression resource = this.input;
      resourceSetExpression = this.inputSet;
      bool flag = false;
      while (propertyInfoStack.Count > 0 && resourceSetExpression != null && resourceSetExpression.HasTransparentScope)
      {
        PropertyInfo propertyInfo = propertyInfoStack.Peek();
        if (propertyInfo.Name.Equals(resourceSetExpression.TransparentScope.Accessor, StringComparison.Ordinal))
        {
          resource = (ResourceExpression) resourceSetExpression;
          propertyInfoStack.Pop();
          flag = true;
        }
        else
        {
          Expression expression;
          if (resourceSetExpression.TransparentScope.SourceAccessors.TryGetValue(propertyInfo.Name, out expression))
          {
            flag = true;
            propertyInfoStack.Pop();
            switch (expression)
            {
              case InputReferenceExpression referenceExpression:
                resourceSetExpression = referenceExpression.Target as ResourceSetExpression;
                resource = (ResourceExpression) resourceSetExpression;
                continue;
              case ResourceSetExpression resourceSetExpression:
                if (resourceSetExpression.HasTransparentScope)
                  continue;
                break;
            }
            resource = (ResourceExpression) expression;
          }
          else
            break;
        }
      }
      if (!flag)
        return (Expression) m;
      Expression expression1 = this.CreateReference(resource);
      while (propertyInfoStack.Count > 0)
        expression1 = (Expression) Expression.Property(expression1, propertyInfoStack.Pop());
      return expression1;
    }

    internal override Expression VisitParameter(ParameterExpression p) => (this.inputSet == null || !this.inputSet.HasTransparentScope) && p == this.inputParameter ? this.CreateReference(this.input) : base.VisitParameter(p);

    private Expression CreateReference(ResourceExpression resource)
    {
      this.referencedInputs.Add(resource);
      return (Expression) resource.CreateReference();
    }
  }
}
