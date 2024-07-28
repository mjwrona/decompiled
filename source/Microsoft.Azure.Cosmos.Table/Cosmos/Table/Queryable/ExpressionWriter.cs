// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ExpressionWriter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ExpressionWriter : DataServiceALinqExpressionVisitor
  {
    internal readonly StringBuilder builder;
    private readonly Stack<Expression> expressionStack;
    private bool cantTranslateExpression;
    private Expression parent;

    protected ExpressionWriter()
    {
      this.builder = new StringBuilder();
      this.expressionStack = new Stack<Expression>();
      this.expressionStack.Push((Expression) null);
    }

    internal static string ExpressionToString(Expression e) => new ExpressionWriter().ConvertExpressionToString(e);

    internal string ConvertExpressionToString(Expression e)
    {
      string str = this.Translate(e);
      if (!this.cantTranslateExpression)
        return str;
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The expression {0} is not supported.", (object) e.ToString()));
    }

    internal override Expression Visit(Expression exp)
    {
      this.parent = this.expressionStack.Peek();
      this.expressionStack.Push(exp);
      Expression expression = base.Visit(exp);
      this.expressionStack.Pop();
      return expression;
    }

    internal override Expression VisitConditional(ConditionalExpression c)
    {
      this.cantTranslateExpression = true;
      return (Expression) c;
    }

    internal override Expression VisitLambda(LambdaExpression lambda)
    {
      this.cantTranslateExpression = true;
      return (Expression) lambda;
    }

    internal override NewExpression VisitNew(NewExpression nex)
    {
      this.cantTranslateExpression = true;
      return nex;
    }

    internal override Expression VisitMemberInit(MemberInitExpression init)
    {
      this.cantTranslateExpression = true;
      return (Expression) init;
    }

    internal override Expression VisitListInit(ListInitExpression init)
    {
      this.cantTranslateExpression = true;
      return (Expression) init;
    }

    internal override Expression VisitNewArray(NewArrayExpression na)
    {
      this.cantTranslateExpression = true;
      return (Expression) na;
    }

    internal override Expression VisitInvocation(InvocationExpression iv)
    {
      this.cantTranslateExpression = true;
      return (Expression) iv;
    }

    internal override Expression VisitInputReferenceExpression(InputReferenceExpression ire)
    {
      if (this.parent == null || this.parent.NodeType != ExpressionType.MemberAccess)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The expression {0} is not supported.", this.parent != null ? (object) this.parent.ToString() : (object) ire.ToString()));
      return (Expression) ire;
    }

    internal override Expression VisitMethodCall(MethodCallExpression m)
    {
      string methodName;
      if (TypeSystem.TryGetQueryOptionMethod(m.Method, out methodName))
      {
        this.builder.Append(methodName);
        this.builder.Append('(');
        if (methodName == "substringof")
        {
          this.Visit(m.Arguments[0]);
          this.builder.Append(',');
          this.Visit(m.Object);
        }
        else
        {
          if (m.Object != null)
            this.Visit(m.Object);
          if (m.Arguments.Count > 0)
          {
            if (m.Object != null)
              this.builder.Append(',');
            for (int index = 0; index < m.Arguments.Count; ++index)
            {
              this.Visit(m.Arguments[index]);
              if (index < m.Arguments.Count - 1)
                this.builder.Append(',');
            }
          }
        }
        this.builder.Append(')');
      }
      else
        this.cantTranslateExpression = true;
      return (Expression) m;
    }

    internal override Expression VisitMemberAccess(MemberExpression m)
    {
      if ((object) (m.Member as FieldInfo) != null)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Referencing public field '{0}' not supported in query option expression.  Use public property instead.", (object) m.Member.Name));
      if (m.Member.DeclaringType == typeof (EntityProperty))
      {
        if (!(m.Expression is MethodCallExpression expression) || expression.Arguments.Count != 1 || !(expression.Method == ReflectionUtil.DictionaryGetItemMethodInfo) || !(expression.Object is MemberExpression memberExpression) || !(memberExpression.Member.DeclaringType == typeof (DynamicTableEntity)) || !(memberExpression.Member.Name == "Properties"))
          throw new NotSupportedException("Referencing {0} on EntityProperty only supported with properties dictionary exposed via DynamicTableEntity.");
        if (!(expression.Arguments[0] is ConstantExpression constantExpression) || !(constantExpression.Value is string))
          throw new NotSupportedException("Accessing property dictionary of DynamicTableEntity requires a string constant for property name.");
        this.builder.Append(this.TranslateMemberName((string) constantExpression.Value));
        return (Expression) constantExpression;
      }
      Expression exp = this.Visit(m.Expression);
      if (m.Member.Name == "Value" && m.Member.DeclaringType.IsGenericType && m.Member.DeclaringType.GetGenericTypeDefinition() == typeof (Nullable<>))
        return (Expression) m;
      if (!ExpressionWriter.IsInputReference(exp) && exp.NodeType != ExpressionType.Convert && exp.NodeType != ExpressionType.ConvertChecked)
        this.builder.Append('/');
      this.builder.Append(this.TranslateMemberName(m.Member.Name));
      return (Expression) m;
    }

    internal override Expression VisitConstant(ConstantExpression c)
    {
      string result = (string) null;
      if (c.Value == null)
      {
        this.builder.Append("null");
        return (Expression) c;
      }
      if (!ClientConvert.TryKeyPrimitiveToString(c.Value, out result))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not convert constant {0} expression to string.", c.Value));
      this.builder.Append(result);
      return (Expression) c;
    }

    internal override Expression VisitUnary(UnaryExpression u)
    {
      switch (u.NodeType)
      {
        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
        case ExpressionType.UnaryPlus:
          return (Expression) u;
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
          this.builder.Append(' ');
          this.builder.Append(this.TranslateOperator(u.NodeType));
          this.VisitOperand(u.Operand);
          goto case ExpressionType.Convert;
        case ExpressionType.Not:
          this.builder.Append("not");
          this.builder.Append(' ');
          this.VisitOperand(u.Operand);
          goto case ExpressionType.Convert;
        default:
          this.cantTranslateExpression = true;
          goto case ExpressionType.Convert;
      }
    }

    internal override Expression VisitBinary(BinaryExpression b)
    {
      this.VisitOperand(b.Left);
      this.builder.Append(' ');
      string str = this.TranslateOperator(b.NodeType);
      if (string.IsNullOrEmpty(str))
        this.cantTranslateExpression = true;
      else
        this.builder.Append(str);
      this.builder.Append(' ');
      this.VisitOperand(b.Right);
      return (Expression) b;
    }

    internal override Expression VisitTypeIs(TypeBinaryExpression b)
    {
      this.builder.Append("isof");
      this.builder.Append('(');
      if (!ExpressionWriter.IsInputReference(b.Expression))
      {
        this.Visit(b.Expression);
        this.builder.Append(',');
        this.builder.Append(' ');
      }
      this.builder.Append('\'');
      this.builder.Append(this.TypeNameForUri(b.TypeOperand));
      this.builder.Append('\'');
      this.builder.Append(')');
      return (Expression) b;
    }

    internal override Expression VisitParameter(ParameterExpression p) => (Expression) p;

    private static bool IsInputReference(Expression exp) => exp is InputReferenceExpression || exp is ParameterExpression;

    private string TypeNameForUri(Type type)
    {
      Type type1 = Nullable.GetUnderlyingType(type);
      if ((object) type1 == null)
        type1 = type;
      type = type1;
      if (!ClientConvert.IsKnownType(type))
        return (string) null;
      return ClientConvert.IsSupportedPrimitiveTypeForUri(type) ? ClientConvert.ToTypeName(type) : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Can't cast to unsupported type '{0}'", (object) type.Name));
    }

    private void VisitOperand(Expression e)
    {
      switch (e)
      {
        case BinaryExpression _:
        case UnaryExpression _:
          this.builder.Append('(');
          this.Visit(e);
          this.builder.Append(')');
          break;
        default:
          this.Visit(e);
          break;
      }
    }

    private string Translate(Expression e)
    {
      this.Visit(e);
      return this.builder.ToString();
    }

    protected virtual string TranslateMemberName(string memberName) => memberName;

    protected virtual object TranslateConstantValue(object value) => value;

    protected virtual string TranslateOperator(ExpressionType type)
    {
      switch (type)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
          return "add";
        case ExpressionType.And:
        case ExpressionType.AndAlso:
          return "and";
        case ExpressionType.Divide:
          return "div";
        case ExpressionType.Equal:
          return "eq";
        case ExpressionType.GreaterThan:
          return "gt";
        case ExpressionType.GreaterThanOrEqual:
          return "ge";
        case ExpressionType.LessThan:
          return "lt";
        case ExpressionType.LessThanOrEqual:
          return "le";
        case ExpressionType.Modulo:
          return "mod";
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
          return "mul";
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
          return "-";
        case ExpressionType.NotEqual:
          return "ne";
        case ExpressionType.Or:
        case ExpressionType.OrElse:
          return "or";
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return "sub";
        default:
          return (string) null;
      }
    }
  }
}
