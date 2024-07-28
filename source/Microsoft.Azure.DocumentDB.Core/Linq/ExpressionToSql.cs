// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ExpressionToSql
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial;
using Microsoft.Azure.Documents.Sql;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class ExpressionToSql
  {
    private static string SqlRoot = "root";
    private static string DefaultParameterName = "v";
    private static bool usePropertyRef = false;
    private static SqlIdentifier RootIdentifier = SqlIdentifier.Create(ExpressionToSql.SqlRoot);

    public static SqlQuery TranslateQuery(Expression inputExpression)
    {
      TranslationContext context = new TranslationContext();
      ExpressionToSql.Translate(inputExpression, context);
      return context.currentQuery.FlattenAsPossible().GetSqlQuery();
    }

    public static Collection Translate(Expression inputExpression, TranslationContext context)
    {
      if (inputExpression == null)
        throw new ArgumentNullException(nameof (inputExpression));
      Collection collection;
      switch (inputExpression.NodeType)
      {
        case ExpressionType.Call:
          MethodCallExpression inputExpression1 = (MethodCallExpression) inputExpression;
          int num = context.PeekMethod() != null ? 0 : (inputExpression1.Method.Name.Equals("Any") ? 1 : 0);
          collection = ExpressionToSql.VisitMethodCall(inputExpression1, context);
          if (num != 0)
          {
            collection = ExpressionToSql.ConvertToScalarAnyCollection(context);
            break;
          }
          break;
        case ExpressionType.Constant:
          collection = ExpressionToSql.TranslateInput((ConstantExpression) inputExpression, context);
          break;
        case ExpressionType.MemberAccess:
          collection = ExpressionToSql.VisitMemberAccessCollectionExpression(inputExpression, context, ExpressionToSql.GetBindingParameterName(context));
          break;
        case ExpressionType.Parameter:
          collection = ExpressionToSql.ConvertToCollection(ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context));
          break;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
      }
      return collection;
    }

    private static Collection TranslateInput(
      ConstantExpression inputExpression,
      TranslationContext context)
    {
      if (!CustomTypeExtensions.IsAssignableFrom(typeof (IDocumentQuery), inputExpression.Type))
        throw new DocumentQueryException(ClientResources.InputIsNotIDocumentQuery);
      if (!(inputExpression.Value is IDocumentQuery))
        throw new DocumentQueryException(ClientResources.InputIsNotIDocumentQuery);
      context.currentQuery = new QueryUnderConstruction(context.GetGenerateNewParameterFunc());
      Type elementType = TypeSystem.GetElementType(inputExpression.Type);
      context.SetInputParameter(elementType, "root");
      return new Collection(ExpressionToSql.SqlRoot);
    }

    private static string GetBindingParameterName(TranslationContext context)
    {
      MethodCallExpression methodCallExpression = context.PeekMethod();
      string bindingParameterName = (string) null;
      if (methodCallExpression.Arguments.Count > 1 && methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression && lambdaExpression.Parameters.Count > 0)
        bindingParameterName = lambdaExpression.Parameters[0].Name;
      if (bindingParameterName == null)
        bindingParameterName = ExpressionToSql.DefaultParameterName;
      return bindingParameterName;
    }

    internal static SqlScalarExpression VisitNonSubqueryScalarExpression(
      Expression inputExpression,
      TranslationContext context)
    {
      if (inputExpression == null)
        return (SqlScalarExpression) null;
      switch (inputExpression.NodeType)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
        case ExpressionType.And:
        case ExpressionType.AndAlso:
        case ExpressionType.ArrayIndex:
        case ExpressionType.Coalesce:
        case ExpressionType.Divide:
        case ExpressionType.Equal:
        case ExpressionType.ExclusiveOr:
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LeftShift:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
        case ExpressionType.Modulo:
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
        case ExpressionType.NotEqual:
        case ExpressionType.Or:
        case ExpressionType.OrElse:
        case ExpressionType.RightShift:
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return ExpressionToSql.VisitBinary((BinaryExpression) inputExpression, context);
        case ExpressionType.ArrayLength:
        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
        case ExpressionType.Not:
        case ExpressionType.Quote:
        case ExpressionType.TypeAs:
          return ExpressionToSql.VisitUnary((UnaryExpression) inputExpression, context);
        case ExpressionType.Call:
          return ExpressionToSql.VisitMethodCallScalar((MethodCallExpression) inputExpression, context);
        case ExpressionType.Conditional:
          return ExpressionToSql.VisitConditional((ConditionalExpression) inputExpression, context);
        case ExpressionType.Constant:
          return ExpressionToSql.VisitConstant((ConstantExpression) inputExpression);
        case ExpressionType.Invoke:
          return ExpressionToSql.VisitInvocation((InvocationExpression) inputExpression, context);
        case ExpressionType.ListInit:
          return ExpressionToSql.VisitListInit((ListInitExpression) inputExpression, context);
        case ExpressionType.MemberAccess:
          return ExpressionToSql.VisitMemberAccess((MemberExpression) inputExpression, context);
        case ExpressionType.MemberInit:
          return ExpressionToSql.VisitMemberInit((MemberInitExpression) inputExpression, context);
        case ExpressionType.New:
          return ExpressionToSql.VisitNew((NewExpression) inputExpression, context);
        case ExpressionType.NewArrayInit:
        case ExpressionType.NewArrayBounds:
          return ExpressionToSql.VisitNewArray((NewArrayExpression) inputExpression, context);
        case ExpressionType.Parameter:
          return ExpressionToSql.VisitParameter((ParameterExpression) inputExpression, context);
        case ExpressionType.TypeIs:
          return ExpressionToSql.VisitTypeIs((TypeBinaryExpression) inputExpression, context);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
      }
    }

    private static SqlScalarExpression VisitMethodCallScalar(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      if (!methodCallExpression.Method.Equals((object) CustomTypeExtensions.GetMethod(typeof (UserDefinedFunctionProvider), "Invoke")))
        return BuiltinFunctionVisitor.VisitBuiltinFunctionCall(methodCallExpression, context);
      string str = ((ConstantExpression) methodCallExpression.Arguments[0]).Value as string;
      SqlIdentifier name = !string.IsNullOrEmpty(str) ? SqlIdentifier.Create(str) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.UdfNameIsNullOrEmpty));
      List<SqlScalarExpression> scalarExpressionList = new List<SqlScalarExpression>();
      if (methodCallExpression.Arguments.Count == 2)
      {
        if (methodCallExpression.Arguments[1] is NewArrayExpression)
        {
          foreach (Expression expression in ((NewArrayExpression) methodCallExpression.Arguments[1]).Expressions)
            scalarExpressionList.Add(ExpressionToSql.VisitScalarExpression(expression, context));
        }
        else if (methodCallExpression.Arguments[1].NodeType == ExpressionType.Constant && (object) methodCallExpression.Arguments[1].Type == (object) typeof (object[]))
        {
          foreach (object obj in (object[]) ((ConstantExpression) methodCallExpression.Arguments[1]).Value)
            scalarExpressionList.Add(ExpressionToSql.VisitConstant(Expression.Constant(obj)));
        }
        else
          scalarExpressionList.Add(ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[1], context));
      }
      return (SqlScalarExpression) SqlFunctionCallScalarExpression.Create(name, true, scalarExpressionList.ToArray());
    }

    private static SqlObjectProperty VisitBinding(MemberBinding binding, TranslationContext context)
    {
      switch (binding.BindingType)
      {
        case MemberBindingType.Assignment:
          return ExpressionToSql.VisitMemberAssignment((MemberAssignment) binding, context);
        case MemberBindingType.MemberBinding:
          return ExpressionToSql.VisitMemberMemberBinding((MemberMemberBinding) binding, context);
        default:
          return ExpressionToSql.VisitMemberListBinding((MemberListBinding) binding, context);
      }
    }

    private static SqlUnaryScalarOperatorKind GetUnaryOperatorKind(ExpressionType type)
    {
      switch (type)
      {
        case ExpressionType.Negate:
          return SqlUnaryScalarOperatorKind.Minus;
        case ExpressionType.UnaryPlus:
          return SqlUnaryScalarOperatorKind.Plus;
        case ExpressionType.Not:
          return SqlUnaryScalarOperatorKind.Not;
        case ExpressionType.OnesComplement:
          return SqlUnaryScalarOperatorKind.BitwiseNot;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.UnaryOperatorNotSupported, (object) type));
      }
    }

    private static SqlScalarExpression VisitUnary(
      UnaryExpression inputExpression,
      TranslationContext context)
    {
      SqlScalarExpression expression = ExpressionToSql.VisitScalarExpression(inputExpression.Operand, context);
      if (expression is SqlInScalarExpression && inputExpression.NodeType == ExpressionType.Not)
      {
        SqlInScalarExpression scalarExpression = (SqlInScalarExpression) expression;
        return (SqlScalarExpression) SqlInScalarExpression.Create(scalarExpression.Expression, true, (IReadOnlyList<SqlScalarExpression>) scalarExpression.Items);
      }
      return inputExpression.NodeType == ExpressionType.Quote || inputExpression.NodeType == ExpressionType.Convert ? expression : (SqlScalarExpression) SqlUnaryScalarExpression.Create(ExpressionToSql.GetUnaryOperatorKind(inputExpression.NodeType), expression);
    }

    private static SqlBinaryScalarOperatorKind GetBinaryOperatorKind(
      ExpressionType expressionType,
      Type resultType)
    {
      switch (expressionType)
      {
        case ExpressionType.Add:
          return (object) resultType == (object) typeof (string) ? SqlBinaryScalarOperatorKind.StringConcat : SqlBinaryScalarOperatorKind.Add;
        case ExpressionType.And:
          return SqlBinaryScalarOperatorKind.BitwiseAnd;
        case ExpressionType.AndAlso:
          return SqlBinaryScalarOperatorKind.And;
        case ExpressionType.Coalesce:
          return SqlBinaryScalarOperatorKind.Coalesce;
        case ExpressionType.Divide:
          return SqlBinaryScalarOperatorKind.Divide;
        case ExpressionType.Equal:
          return SqlBinaryScalarOperatorKind.Equal;
        case ExpressionType.ExclusiveOr:
          return SqlBinaryScalarOperatorKind.BitwiseXor;
        case ExpressionType.GreaterThan:
          return SqlBinaryScalarOperatorKind.GreaterThan;
        case ExpressionType.GreaterThanOrEqual:
          return SqlBinaryScalarOperatorKind.GreaterThanOrEqual;
        case ExpressionType.LessThan:
          return SqlBinaryScalarOperatorKind.LessThan;
        case ExpressionType.LessThanOrEqual:
          return SqlBinaryScalarOperatorKind.LessThanOrEqual;
        case ExpressionType.Modulo:
          return SqlBinaryScalarOperatorKind.Modulo;
        case ExpressionType.Multiply:
          return SqlBinaryScalarOperatorKind.Multiply;
        case ExpressionType.NotEqual:
          return SqlBinaryScalarOperatorKind.NotEqual;
        case ExpressionType.Or:
          return SqlBinaryScalarOperatorKind.BitwiseOr;
        case ExpressionType.OrElse:
          return SqlBinaryScalarOperatorKind.Or;
        case ExpressionType.Subtract:
          return SqlBinaryScalarOperatorKind.Subtract;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.BinaryOperatorNotSupported, (object) expressionType));
      }
    }

    private static SqlScalarExpression VisitBinary(
      BinaryExpression inputExpression,
      TranslationContext context)
    {
      MethodCallExpression left = (MethodCallExpression) null;
      ConstantExpression right = (ConstantExpression) null;
      bool reverseNodeType = false;
      if (inputExpression.Left.NodeType == ExpressionType.Call && inputExpression.Right.NodeType == ExpressionType.Constant)
      {
        left = (MethodCallExpression) inputExpression.Left;
        right = (ConstantExpression) inputExpression.Right;
      }
      else if (inputExpression.Right.NodeType == ExpressionType.Call && inputExpression.Left.NodeType == ExpressionType.Constant)
      {
        left = (MethodCallExpression) inputExpression.Right;
        right = (ConstantExpression) inputExpression.Left;
        reverseNodeType = true;
      }
      if (left != null && right != null && ExpressionToSql.TryMatchStringCompareTo(left, right, inputExpression.NodeType))
        return ExpressionToSql.VisitStringCompareTo(left, right, inputExpression.NodeType, reverseNodeType, context);
      SqlScalarExpression scalarExpression1 = ExpressionToSql.VisitScalarExpression(inputExpression.Left, context);
      SqlScalarExpression scalarExpression2 = ExpressionToSql.VisitScalarExpression(inputExpression.Right, context);
      if (inputExpression.NodeType == ExpressionType.ArrayIndex)
        return (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create(scalarExpression1, scalarExpression2);
      int binaryOperatorKind = (int) ExpressionToSql.GetBinaryOperatorKind(inputExpression.NodeType, inputExpression.Type);
      if (scalarExpression1.Kind == SqlObjectKind.MemberIndexerScalarExpression && scalarExpression2.Kind == SqlObjectKind.LiteralScalarExpression)
        scalarExpression2 = ExpressionToSql.ApplyCustomConverters(inputExpression.Left, scalarExpression2 as SqlLiteralScalarExpression);
      else if (scalarExpression2.Kind == SqlObjectKind.MemberIndexerScalarExpression && scalarExpression1.Kind == SqlObjectKind.LiteralScalarExpression)
        scalarExpression1 = ExpressionToSql.ApplyCustomConverters(inputExpression.Right, scalarExpression1 as SqlLiteralScalarExpression);
      SqlScalarExpression leftExpression = scalarExpression1;
      SqlScalarExpression rightExpression = scalarExpression2;
      return (SqlScalarExpression) SqlBinaryScalarExpression.Create((SqlBinaryScalarOperatorKind) binaryOperatorKind, leftExpression, rightExpression);
    }

    private static SqlScalarExpression ApplyCustomConverters(
      Expression left,
      SqlLiteralScalarExpression right)
    {
      MemberExpression memberExpression = !(left is UnaryExpression) ? left as MemberExpression : ((UnaryExpression) left).Operand as MemberExpression;
      if (memberExpression != null)
      {
        Type type1 = memberExpression.Type;
        if (type1.IsNullable())
          type1 = type1.NullableUnderlyingType();
        CustomAttributeData customAttributeData1 = memberExpression.Member.CustomAttributes.Where<CustomAttributeData>((Func<CustomAttributeData, bool>) (ca => (object) ca.AttributeType == (object) typeof (JsonConverterAttribute))).FirstOrDefault<CustomAttributeData>();
        CustomAttributeData customAttributeData2 = type1.GetsCustomAttributes().Where<CustomAttributeData>((Func<CustomAttributeData, bool>) (ca => (object) ca.AttributeType == (object) typeof (JsonConverterAttribute))).FirstOrDefault<CustomAttributeData>();
        if (customAttributeData1 == null)
          customAttributeData1 = customAttributeData2;
        CustomAttributeData customAttributeData3 = customAttributeData1;
        if (customAttributeData3 != null)
        {
          Type type2 = (Type) customAttributeData3.ConstructorArguments[0].Value;
          object obj = (object) null;
          if (type1.IsEnum())
          {
            Number64 number64 = ((SqlNumberLiteral) right.Literal).Value;
            obj = !number64.IsDouble ? Enum.ToObject(type1, (object) Number64.ToLong(number64)) : Enum.ToObject(type1, (object) Number64.ToDouble(number64));
          }
          else if ((object) type1 == (object) typeof (DateTime))
            obj = ((SqlObjectLiteral) right.Literal).Value;
          if (obj != null)
          {
            string str;
            if ((object) CustomTypeExtensions.GetConstructor(type2, Type.EmptyTypes) != null)
              str = JsonConvert.SerializeObject(obj, (JsonConverter) Activator.CreateInstance(type2));
            else
              str = JsonConvert.SerializeObject(obj);
            return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlObjectLiteral.Create((object) str, true));
          }
        }
      }
      return (SqlScalarExpression) right;
    }

    private static bool TryMatchStringCompareTo(
      MethodCallExpression left,
      ConstantExpression right,
      ExpressionType compareOperator)
    {
      if (!left.Method.Equals((object) CustomTypeExtensions.GetMethod(typeof (string), "CompareTo", new Type[1]
      {
        typeof (string)
      })) || left.Arguments.Count != 1)
        return false;
      switch (compareOperator)
      {
        case ExpressionType.Equal:
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
          if ((object) right.Type != (object) typeof (int) || (int) right.Value != 0)
          {
            int? nullable = (object) right.Type == (object) typeof (int?) ? (int?) right.Value : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.StringCompareToInvalidConstant));
            if (nullable.HasValue)
            {
              nullable = (int?) right.Value;
              if (nullable.Value == 0)
                goto label_8;
            }
          }
label_8:
          return true;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.StringCompareToInvalidOperator));
      }
    }

    private static SqlScalarExpression VisitStringCompareTo(
      MethodCallExpression left,
      ConstantExpression right,
      ExpressionType compareOperator,
      bool reverseNodeType,
      TranslationContext context)
    {
      if (reverseNodeType)
      {
        switch (compareOperator)
        {
          case ExpressionType.Equal:
            break;
          case ExpressionType.GreaterThan:
            compareOperator = ExpressionType.LessThan;
            break;
          case ExpressionType.GreaterThanOrEqual:
            compareOperator = ExpressionType.LessThanOrEqual;
            break;
          case ExpressionType.LessThan:
            compareOperator = ExpressionType.GreaterThan;
            break;
          case ExpressionType.LessThanOrEqual:
            compareOperator = ExpressionType.GreaterThanOrEqual;
            break;
          default:
            throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.StringCompareToInvalidOperator));
        }
      }
      int binaryOperatorKind = (int) ExpressionToSql.GetBinaryOperatorKind(compareOperator, (Type) null);
      SqlScalarExpression scalarExpression1 = ExpressionToSql.VisitNonSubqueryScalarExpression(left.Object, context);
      SqlScalarExpression scalarExpression2 = ExpressionToSql.VisitNonSubqueryScalarExpression(left.Arguments[0], context);
      SqlScalarExpression leftExpression = scalarExpression1;
      SqlScalarExpression rightExpression = scalarExpression2;
      return (SqlScalarExpression) SqlBinaryScalarExpression.Create((SqlBinaryScalarOperatorKind) binaryOperatorKind, leftExpression, rightExpression);
    }

    private static SqlScalarExpression VisitTypeIs(
      TypeBinaryExpression inputExpression,
      TranslationContext context)
    {
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
    }

    public static SqlScalarExpression VisitConstant(ConstantExpression inputExpression)
    {
      if (inputExpression.Value == null)
        return (SqlScalarExpression) SqlLiteralScalarExpression.SqlNullLiteralScalarExpression;
      if (inputExpression.Type.IsNullable())
        return ExpressionToSql.VisitConstant(Expression.Constant(inputExpression.Value, Nullable.GetUnderlyingType(inputExpression.Type)));
      Type type = inputExpression.Value.GetType();
      if (type.IsValueType())
      {
        if ((object) type == (object) typeof (bool))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlBooleanLiteral.Create((bool) inputExpression.Value));
        if ((object) type == (object) typeof (byte))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (byte) inputExpression.Value));
        if ((object) type == (object) typeof (sbyte))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (sbyte) inputExpression.Value));
        if ((object) type == (object) typeof (char))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlStringLiteral.Create(inputExpression.Value.ToString()));
        if ((object) type == (object) typeof (Decimal))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((Decimal) inputExpression.Value));
        if ((object) type == (object) typeof (double))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((double) inputExpression.Value));
        if ((object) type == (object) typeof (float))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((double) (float) inputExpression.Value));
        if ((object) type == (object) typeof (int))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (int) inputExpression.Value));
        if ((object) type == (object) typeof (uint))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (uint) inputExpression.Value));
        if ((object) type == (object) typeof (long))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) inputExpression.Value));
        if ((object) type == (object) typeof (ulong))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((Decimal) (ulong) inputExpression.Value));
        if ((object) type == (object) typeof (short))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (short) inputExpression.Value));
        if ((object) type == (object) typeof (ushort))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((long) (ushort) inputExpression.Value));
        if ((object) type == (object) typeof (Guid))
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlStringLiteral.Create(inputExpression.Value.ToString()));
      }
      if ((object) type == (object) typeof (string))
        return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlStringLiteral.Create((string) inputExpression.Value));
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Geometry), type))
        return GeometrySqlExpressionFactory.Construct((Expression) inputExpression);
      if (!(inputExpression.Value is IEnumerable))
        return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlObjectLiteral.Create(inputExpression.Value, false));
      List<SqlScalarExpression> scalarExpressionList = new List<SqlScalarExpression>();
      foreach (object obj in (IEnumerable) inputExpression.Value)
        scalarExpressionList.Add(ExpressionToSql.VisitConstant(Expression.Constant(obj)));
      return (SqlScalarExpression) SqlArrayCreateScalarExpression.Create(scalarExpressionList.ToArray());
    }

    private static SqlScalarExpression VisitConditional(
      ConditionalExpression inputExpression,
      TranslationContext context)
    {
      SqlScalarExpression condition = ExpressionToSql.VisitScalarExpression(inputExpression.Test, context);
      SqlScalarExpression scalarExpression1 = ExpressionToSql.VisitScalarExpression(inputExpression.IfTrue, context);
      SqlScalarExpression scalarExpression2 = ExpressionToSql.VisitScalarExpression(inputExpression.IfFalse, context);
      SqlScalarExpression first = scalarExpression1;
      SqlScalarExpression second = scalarExpression2;
      return (SqlScalarExpression) SqlConditionalScalarExpression.Create(condition, first, second);
    }

    private static SqlScalarExpression VisitParameter(
      ParameterExpression inputExpression,
      TranslationContext context)
    {
      Expression inputExpression1 = context.LookupSubstitution(inputExpression);
      return inputExpression1 != null ? ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression1, context) : (SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(inputExpression.Name));
    }

    private static SqlScalarExpression VisitMemberAccess(
      MemberExpression inputExpression,
      TranslationContext context)
    {
      SqlScalarExpression memberExpression = ExpressionToSql.VisitScalarExpression(inputExpression.Expression, context);
      string memberName = inputExpression.Member.GetMemberName();
      if (inputExpression.Expression.Type.IsNullable())
      {
        switch (memberName)
        {
          case "Value":
            return memberExpression;
          case "HasValue":
            return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("IS_DEFINED", memberExpression);
        }
      }
      if (ExpressionToSql.usePropertyRef)
      {
        SqlIdentifier propertyIdentifier = SqlIdentifier.Create(memberName);
        return (SqlScalarExpression) SqlPropertyRefScalarExpression.Create(memberExpression, propertyIdentifier);
      }
      SqlScalarExpression indexExpression = (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlStringLiteral.Create(memberName));
      return (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create(memberExpression, indexExpression);
    }

    private static SqlScalarExpression[] VisitExpressionList(
      ReadOnlyCollection<Expression> inputExpressionList,
      TranslationContext context)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[inputExpressionList.Count];
      for (int index = 0; index < inputExpressionList.Count; ++index)
      {
        SqlScalarExpression scalarExpression = ExpressionToSql.VisitScalarExpression(inputExpressionList[index], context);
        scalarExpressionArray[index] = scalarExpression;
      }
      return scalarExpressionArray;
    }

    private static SqlObjectProperty VisitMemberAssignment(
      MemberAssignment inputExpression,
      TranslationContext context)
    {
      SqlScalarExpression expression = ExpressionToSql.VisitScalarExpression(inputExpression.Expression, context);
      return SqlObjectProperty.Create(SqlPropertyName.Create(inputExpression.Member.GetMemberName()), expression);
    }

    private static SqlObjectProperty VisitMemberMemberBinding(
      MemberMemberBinding inputExpression,
      TranslationContext context)
    {
      throw new DocumentQueryException(ClientResources.MemberBindingNotSupported);
    }

    private static SqlObjectProperty VisitMemberListBinding(
      MemberListBinding inputExpression,
      TranslationContext context)
    {
      throw new DocumentQueryException(ClientResources.MemberBindingNotSupported);
    }

    private static SqlObjectProperty[] VisitBindingList(
      ReadOnlyCollection<MemberBinding> inputExpressionList,
      TranslationContext context)
    {
      SqlObjectProperty[] sqlObjectPropertyArray = new SqlObjectProperty[inputExpressionList.Count];
      for (int index = 0; index < inputExpressionList.Count; ++index)
      {
        SqlObjectProperty sqlObjectProperty = ExpressionToSql.VisitBinding(inputExpressionList[index], context);
        sqlObjectPropertyArray[index] = sqlObjectProperty;
      }
      return sqlObjectPropertyArray;
    }

    private static SqlObjectProperty[] CreateInitializers(
      ReadOnlyCollection<Expression> arguments,
      ReadOnlyCollection<MemberInfo> members,
      TranslationContext context)
    {
      if (arguments.Count != members.Count)
        throw new InvalidOperationException("Expected same number of arguments as members");
      SqlObjectProperty[] initializers = new SqlObjectProperty[arguments.Count];
      for (int index = 0; index < arguments.Count; ++index)
      {
        Expression expression1 = arguments[index];
        MemberInfo member = members[index];
        TranslationContext context1 = context;
        SqlScalarExpression expression2 = ExpressionToSql.VisitScalarExpression(expression1, context1);
        SqlObjectProperty sqlObjectProperty = SqlObjectProperty.Create(SqlPropertyName.Create(member.GetMemberName()), expression2);
        initializers[index] = sqlObjectProperty;
      }
      return initializers;
    }

    private static SqlScalarExpression VisitNew(
      NewExpression inputExpression,
      TranslationContext context)
    {
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Geometry), inputExpression.Type))
        return GeometrySqlExpressionFactory.Construct((Expression) inputExpression);
      if (inputExpression.Arguments.Count <= 0)
        return (SqlScalarExpression) null;
      if (inputExpression.Members == null)
        throw new DocumentQueryException(ClientResources.ConstructorInvocationNotSupported);
      return (SqlScalarExpression) SqlObjectCreateScalarExpression.Create(ExpressionToSql.CreateInitializers(inputExpression.Arguments, inputExpression.Members, context));
    }

    private static SqlScalarExpression VisitMemberInit(
      MemberInitExpression inputExpression,
      TranslationContext context)
    {
      ExpressionToSql.VisitNew(inputExpression.NewExpression, context);
      return (SqlScalarExpression) SqlObjectCreateScalarExpression.Create(ExpressionToSql.VisitBindingList(inputExpression.Bindings, context));
    }

    private static SqlScalarExpression VisitListInit(
      ListInitExpression inputExpression,
      TranslationContext context)
    {
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
    }

    private static SqlScalarExpression VisitNewArray(
      NewArrayExpression inputExpression,
      TranslationContext context)
    {
      SqlScalarExpression[] scalarExpressionArray = ExpressionToSql.VisitExpressionList(inputExpression.Expressions, context);
      if (inputExpression.NodeType == ExpressionType.NewArrayInit)
        return (SqlScalarExpression) SqlArrayCreateScalarExpression.Create(scalarExpressionArray);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
    }

    private static SqlScalarExpression VisitInvocation(
      InvocationExpression inputExpression,
      TranslationContext context)
    {
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) inputExpression.NodeType));
    }

    private static Collection ConvertToCollection(SqlScalarExpression scalar)
    {
      if (ExpressionToSql.usePropertyRef)
        return scalar is SqlPropertyRefScalarExpression propRef1 ? new Collection((SqlCollection) ExpressionToSql.ConvertPropertyRefToPath(propRef1)) : throw new DocumentQueryException(ClientResources.PathExpressionsOnly);
      switch (scalar)
      {
        case SqlMemberIndexerScalarExpression memberIndexerExpression:
          return new Collection((SqlCollection) ExpressionToSql.ConvertMemberIndexerToPath(memberIndexerExpression));
        case SqlPropertyRefScalarExpression propRef2:
          return new Collection((SqlCollection) ExpressionToSql.ConvertPropertyRefToPath(propRef2));
        default:
          throw new DocumentQueryException(ClientResources.PathExpressionsOnly);
      }
    }

    private static Collection ConvertToScalarAnyCollection(TranslationContext context)
    {
      SqlCollection collection = (SqlCollection) SqlSubqueryCollection.Create(context.currentQuery.FlattenAsPossible().GetSqlQuery());
      ParameterExpression newParamter = context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName);
      FromParameterBindings.Binding binding = new FromParameterBindings.Binding(newParamter, collection, false);
      context.currentQuery = new QueryUnderConstruction(context.GetGenerateNewParameterFunc());
      context.currentQuery.AddBinding(binding);
      SqlSelectClause select = SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlBinaryScalarExpression.Create(SqlBinaryScalarOperatorKind.GreaterThan, (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("COUNT", (SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(newParamter.Name))), (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create(0L)))));
      context.currentQuery.AddSelectClause(select);
      return new Collection("Any");
    }

    private static SqlScalarExpression VisitNonSubqueryScalarExpression(
      Expression expression,
      ReadOnlyCollection<ParameterExpression> parameters,
      TranslationContext context)
    {
      foreach (ParameterExpression parameter in parameters)
        context.PushParameter(parameter, context.CurrentSubqueryBinding.ShouldBeOnNewQuery);
      SqlScalarExpression scalarExpression = ExpressionToSql.VisitNonSubqueryScalarExpression(expression, context);
      foreach (ParameterExpression parameter in parameters)
        context.PopParameter();
      return scalarExpression;
    }

    private static SqlScalarExpression VisitNonSubqueryScalarLambda(
      LambdaExpression lambdaExpression,
      TranslationContext context)
    {
      ReadOnlyCollection<ParameterExpression> parameters = lambdaExpression.Parameters;
      if (parameters.Count != 1)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) lambdaExpression.Body, (object) 1, (object) parameters.Count));
      return ExpressionToSql.VisitNonSubqueryScalarExpression(lambdaExpression.Body, parameters, context);
    }

    private static Collection VisitCollectionExpression(
      Expression expression,
      ReadOnlyCollection<ParameterExpression> parameters,
      TranslationContext context)
    {
      foreach (ParameterExpression parameter in parameters)
        context.PushParameter(parameter, context.CurrentSubqueryBinding.ShouldBeOnNewQuery);
      Collection collection = ExpressionToSql.VisitCollectionExpression(expression, context, parameters.Count > 0 ? parameters.First<ParameterExpression>().Name : ExpressionToSql.DefaultParameterName);
      foreach (ParameterExpression parameter in parameters)
        context.PopParameter();
      return collection;
    }

    private static Collection VisitCollectionExpression(
      Expression expression,
      TranslationContext context,
      string parameterName)
    {
      switch (expression.NodeType)
      {
        case ExpressionType.Call:
          return ExpressionToSql.Translate(expression, context);
        case ExpressionType.MemberAccess:
          return ExpressionToSql.VisitMemberAccessCollectionExpression(expression, context, parameterName);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.ExpressionTypeIsNotSupported, (object) expression.NodeType));
      }
    }

    private static Collection VisitCollectionLambda(
      LambdaExpression lambdaExpression,
      TranslationContext context)
    {
      ReadOnlyCollection<ParameterExpression> parameters = lambdaExpression.Parameters;
      if (parameters.Count != 1)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) lambdaExpression.Body, (object) 1, (object) parameters.Count));
      return ExpressionToSql.VisitCollectionExpression(lambdaExpression.Body, lambdaExpression.Parameters, context);
    }

    private static Collection VisitMemberAccessCollectionExpression(
      Expression inputExpression,
      TranslationContext context,
      string parameterName)
    {
      SqlScalarExpression scalar = ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context);
      Type type = inputExpression.Type;
      Collection collection = ExpressionToSql.ConvertToCollection(scalar);
      context.PushCollection(collection);
      ParameterExpression newParamter = context.GenerateNewParamter(type, parameterName);
      context.PushParameter(newParamter, context.CurrentSubqueryBinding.ShouldBeOnNewQuery);
      context.PopParameter();
      context.PopCollection();
      return new Collection(newParamter.Name);
    }

    private static Collection VisitMethodCall(
      MethodCallExpression inputExpression,
      TranslationContext context)
    {
      context.PushMethod(inputExpression);
      Type declaringType = inputExpression.Method.DeclaringType;
      if ((object) declaringType != (object) typeof (Queryable) && (object) declaringType != (object) typeof (Enumerable) || !inputExpression.Method.IsStatic)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.OnlyLINQMethodsAreSupported, (object) inputExpression.Method.Name));
      TypeSystem.GetElementType(inputExpression.Method.ReturnType);
      Expression inputExpression1 = inputExpression.Object == null ? inputExpression.Arguments[0] : throw new DocumentQueryException(ClientResources.ExpectedMethodCallsMethods);
      TypeSystem.GetElementType(inputExpression1.Type);
      Collection collection1 = ExpressionToSql.Translate(inputExpression1, context);
      context.PushCollection(collection1);
      Collection collection2 = new Collection(inputExpression.Method.Name);
      bool shouldBeOnNewQuery = context.currentQuery.ShouldBeOnNewQuery(inputExpression.Method.Name, inputExpression.Arguments.Count);
      context.PushSubqueryBinding(shouldBeOnNewQuery);
      switch (inputExpression.Method.Name)
      {
        case "Any":
          collection2 = new Collection("");
          if (inputExpression.Arguments.Count == 2)
          {
            SqlWhereClause whereClause = ExpressionToSql.VisitWhere(inputExpression.Arguments, context);
            context.currentQuery = context.currentQuery.AddWhereClause(whereClause, context);
            break;
          }
          break;
        case "Average":
          SqlSelectClause select1 = ExpressionToSql.VisitAggregateFunction(inputExpression.Arguments, context, "AVG");
          context.currentQuery = context.currentQuery.AddSelectClause(select1, context);
          break;
        case "Count":
          SqlSelectClause select2 = ExpressionToSql.VisitCount(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddSelectClause(select2, context);
          break;
        case "Distinct":
          SqlSelectClause select3 = ExpressionToSql.VisitDistinct(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddSelectClause(select3, context);
          break;
        case "Max":
          SqlSelectClause select4 = ExpressionToSql.VisitAggregateFunction(inputExpression.Arguments, context, "MAX");
          context.currentQuery = context.currentQuery.AddSelectClause(select4, context);
          break;
        case "Min":
          SqlSelectClause select5 = ExpressionToSql.VisitAggregateFunction(inputExpression.Arguments, context, "MIN");
          context.currentQuery = context.currentQuery.AddSelectClause(select5, context);
          break;
        case "OrderBy":
          SqlOrderbyClause orderBy1 = ExpressionToSql.VisitOrderBy(inputExpression.Arguments, false, context);
          context.currentQuery = context.currentQuery.AddOrderByClause(orderBy1, context);
          break;
        case "OrderByDescending":
          SqlOrderbyClause orderBy2 = ExpressionToSql.VisitOrderBy(inputExpression.Arguments, true, context);
          context.currentQuery = context.currentQuery.AddOrderByClause(orderBy2, context);
          break;
        case "Select":
          SqlSelectClause select6 = ExpressionToSql.VisitSelect(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddSelectClause(select6, context);
          break;
        case "SelectMany":
          context.currentQuery = context.PackageCurrentQueryIfNeccessary();
          collection2 = ExpressionToSql.VisitSelectMany(inputExpression.Arguments, context);
          break;
        case "Skip":
          SqlOffsetSpec offsetSpec = ExpressionToSql.VisitSkip(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddOffsetSpec(offsetSpec, context);
          break;
        case "Sum":
          SqlSelectClause select7 = ExpressionToSql.VisitAggregateFunction(inputExpression.Arguments, context, "SUM");
          context.currentQuery = context.currentQuery.AddSelectClause(select7, context);
          break;
        case "Take":
          if (context.currentQuery.HasOffsetSpec())
          {
            SqlLimitSpec limit = ExpressionToSql.VisitTakeLimit(inputExpression.Arguments, context);
            context.currentQuery = context.currentQuery.AddLimitSpec(limit, context);
            break;
          }
          SqlTopSpec top = ExpressionToSql.VisitTakeTop(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddTopSpec(top);
          break;
        case "ThenBy":
          SqlOrderbyClause thenBy1 = ExpressionToSql.VisitOrderBy(inputExpression.Arguments, false, context);
          context.currentQuery = context.currentQuery.UpdateOrderByClause(thenBy1, context);
          break;
        case "ThenByDescending":
          SqlOrderbyClause thenBy2 = ExpressionToSql.VisitOrderBy(inputExpression.Arguments, true, context);
          context.currentQuery = context.currentQuery.UpdateOrderByClause(thenBy2, context);
          break;
        case "Where":
          SqlWhereClause whereClause1 = ExpressionToSql.VisitWhere(inputExpression.Arguments, context);
          context.currentQuery = context.currentQuery.AddWhereClause(whereClause1, context);
          break;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) inputExpression.Method.Name));
      }
      context.PopSubqueryBinding();
      context.PopCollection();
      context.PopMethod();
      return collection2;
    }

    private static bool IsSubqueryScalarExpression(
      Expression expression,
      out SqlObjectKind? expressionObjKind,
      out bool isMinMaxAvgMethod)
    {
      if (!(expression is MethodCallExpression methodCallExpression))
      {
        expressionObjKind = new SqlObjectKind?();
        isMinMaxAvgMethod = false;
        return false;
      }
      string name = methodCallExpression.Method.Name;
      isMinMaxAvgMethod = false;
      bool flag;
      switch (name)
      {
        case "Any":
          flag = true;
          expressionObjKind = new SqlObjectKind?(SqlObjectKind.ExistsScalarExpression);
          break;
        case "Average":
        case "Max":
        case "Min":
          isMinMaxAvgMethod = true;
          flag = true;
          expressionObjKind = new SqlObjectKind?(SqlObjectKind.SubqueryScalarExpression);
          break;
        case "Count":
          if (methodCallExpression.Arguments.Count > 1)
          {
            flag = true;
            expressionObjKind = new SqlObjectKind?(SqlObjectKind.SubqueryScalarExpression);
            break;
          }
          if (ExpressionToSql.IsSubqueryScalarExpression((Expression) (methodCallExpression.Arguments[0] as MethodCallExpression), out SqlObjectKind? _, out bool _))
          {
            flag = true;
            expressionObjKind = new SqlObjectKind?(SqlObjectKind.SubqueryScalarExpression);
            break;
          }
          flag = false;
          expressionObjKind = new SqlObjectKind?();
          break;
        case "Distinct":
        case "OrderBy":
        case "OrderByDescending":
        case "Select":
        case "SelectMany":
        case "Skip":
        case "Take":
        case "ThenBy":
        case "ThenByDescending":
        case "Where":
          flag = true;
          expressionObjKind = new SqlObjectKind?(SqlObjectKind.ArrayScalarExpression);
          break;
        case "Sum":
          flag = true;
          expressionObjKind = new SqlObjectKind?(SqlObjectKind.SubqueryScalarExpression);
          break;
        default:
          flag = false;
          expressionObjKind = new SqlObjectKind?();
          break;
      }
      return flag;
    }

    private static SqlScalarExpression VisitScalarExpression(
      LambdaExpression lambda,
      TranslationContext context)
    {
      return ExpressionToSql.VisitScalarExpression(lambda.Body, lambda.Parameters, context);
    }

    internal static SqlScalarExpression VisitScalarExpression(
      Expression expression,
      TranslationContext context)
    {
      return ExpressionToSql.VisitScalarExpression(expression, new ReadOnlyCollection<ParameterExpression>((IList<ParameterExpression>) new ParameterExpression[0]), context);
    }

    private static SqlScalarExpression VisitScalarExpression(
      Expression expression,
      ReadOnlyCollection<ParameterExpression> parameters,
      TranslationContext context)
    {
      SqlObjectKind? expressionObjKind;
      bool isMinMaxAvgMethod;
      SqlScalarExpression scalarExpression;
      if (!ExpressionToSql.IsSubqueryScalarExpression(expression, out expressionObjKind, out isMinMaxAvgMethod))
      {
        scalarExpression = ExpressionToSql.VisitNonSubqueryScalarExpression(expression, parameters, context);
      }
      else
      {
        SqlQuery subquery = ExpressionToSql.CreateSubquery(expression, parameters, context);
        ParameterExpression newParamter = context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName);
        TranslationContext context1 = context;
        int subqueryType = isMinMaxAvgMethod ? 3 : (int) expressionObjKind.Value;
        SqlCollection subquerySqlCollection = ExpressionToSql.CreateSubquerySqlCollection(subquery, context1, (SqlObjectKind) subqueryType);
        FromParameterBindings.Binding binding = new FromParameterBindings.Binding(newParamter, subquerySqlCollection, false, context.IsInMainBranchSelect());
        context.CurrentSubqueryBinding.NewBindings.Add(binding);
        scalarExpression = !isMinMaxAvgMethod ? (SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(newParamter.Name)) : (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create((SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(newParamter.Name)), (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create(0L)));
      }
      return scalarExpression;
    }

    private static SqlCollection CreateSubquerySqlCollection(
      SqlQuery query,
      TranslationContext context,
      SqlObjectKind subqueryType)
    {
      switch (subqueryType)
      {
        case SqlObjectKind.ArrayScalarExpression:
          query = SqlQuery.Create(SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlArrayScalarExpression.Create(query))), (SqlFromClause) null, (SqlWhereClause) null, (SqlGroupByClause) null, (SqlOrderbyClause) null, (SqlOffsetLimitClause) null);
          goto case SqlObjectKind.SubqueryScalarExpression;
        case SqlObjectKind.ExistsScalarExpression:
          query = SqlQuery.Create(SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlExistsScalarExpression.Create(query))), (SqlFromClause) null, (SqlWhereClause) null, (SqlGroupByClause) null, (SqlOrderbyClause) null, (SqlOffsetLimitClause) null);
          goto case SqlObjectKind.SubqueryScalarExpression;
        case SqlObjectKind.SubqueryScalarExpression:
          return (SqlCollection) SqlSubqueryCollection.Create(query);
        default:
          throw new DocumentQueryException(string.Format("Unsupported subquery type {0}", (object) subqueryType));
      }
    }

    private static SqlQuery CreateSubquery(
      Expression expression,
      ReadOnlyCollection<ParameterExpression> parameters,
      TranslationContext context)
    {
      bool shouldBeOnNewQuery = context.CurrentSubqueryBinding.ShouldBeOnNewQuery;
      QueryUnderConstruction currentQuery = context.currentQuery;
      QueryUnderConstruction underConstruction = new QueryUnderConstruction(context.GetGenerateNewParameterFunc(), context.currentQuery);
      underConstruction.fromParameters.SetInputParameter(typeof (object), context.currentQuery.GetInputParameterInContext(shouldBeOnNewQuery).Name, context.InScope);
      context.currentQuery = underConstruction;
      if (shouldBeOnNewQuery)
        context.CurrentSubqueryBinding.ShouldBeOnNewQuery = false;
      ExpressionToSql.VisitCollectionExpression(expression, parameters, context);
      QueryUnderConstruction subquery = context.currentQuery.GetSubquery(currentQuery);
      context.CurrentSubqueryBinding.ShouldBeOnNewQuery = shouldBeOnNewQuery;
      context.currentQuery = currentQuery;
      return subquery.FlattenAsPossible().GetSqlQuery();
    }

    private static SqlWhereClause VisitWhere(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      return arguments.Count == 2 ? SqlWhereClause.Create(ExpressionToSql.VisitScalarExpression(Utilities.GetLambda(arguments[1]), context)) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Where", (object) 2, (object) arguments.Count));
    }

    private static SqlSelectClause VisitSelect(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      return arguments.Count == 2 ? SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create(ExpressionToSql.VisitScalarExpression(Utilities.GetLambda(arguments[1]), context))) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Select", (object) 2, (object) arguments.Count));
    }

    private static Collection VisitSelectMany(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      LambdaExpression lambdaExpression = arguments.Count == 2 ? Utilities.GetLambda(arguments[1]) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "SelectMany", (object) 2, (object) arguments.Count));
      bool flag = false;
      for (MethodCallExpression body = lambdaExpression.Body as MethodCallExpression; body != null; body = body.Arguments[0] as MethodCallExpression)
      {
        string name = body.Method.Name;
        flag = ((flag ? 1 : 0) | (name.Equals("Distinct") || name.Equals("Take") || name.Equals("OrderBy") ? 1 : (name.Equals("OrderByDescending") ? 1 : 0))) != 0;
      }
      Collection collection1;
      if (!flag)
      {
        collection1 = ExpressionToSql.VisitCollectionLambda(lambdaExpression, context);
      }
      else
      {
        collection1 = new Collection("");
        SqlCollection collection2 = (SqlCollection) SqlSubqueryCollection.Create(ExpressionToSql.CreateSubquery(lambdaExpression.Body, lambdaExpression.Parameters, context));
        FromParameterBindings.Binding binding = new FromParameterBindings.Binding(context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName), collection2, false);
        context.currentQuery.fromParameters.Add(binding);
      }
      return collection1;
    }

    private static SqlOrderbyClause VisitOrderBy(
      ReadOnlyCollection<Expression> arguments,
      bool isDescending,
      TranslationContext context)
    {
      if (arguments.Count != 2)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "OrderBy", (object) 2, (object) arguments.Count));
      return SqlOrderbyClause.Create(SqlOrderByItem.Create(ExpressionToSql.VisitScalarExpression(Utilities.GetLambda(arguments[1]), context), isDescending));
    }

    private static bool TryGetTopSkipTakeLiteral(
      Expression expression,
      TranslationContext context,
      out SqlNumberLiteral literal)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      literal = (SqlNumberLiteral) null;
      if (ExpressionToSql.VisitScalarExpression(expression, context) is SqlLiteralScalarExpression scalarExpression && scalarExpression.Literal.Kind == SqlObjectKind.NumberLiteral)
      {
        ParameterExpression newParamter = context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName);
        context.PushParameter(newParamter, context.CurrentSubqueryBinding.ShouldBeOnNewQuery);
        context.PopParameter();
        literal = (SqlNumberLiteral) scalarExpression.Literal;
      }
      return literal != null && literal.Value >= (Number64) 0L;
    }

    private static SqlOffsetSpec VisitSkip(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      if (arguments.Count != 2)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Skip", (object) 2, (object) arguments.Count));
      SqlNumberLiteral literal;
      if (ExpressionToSql.TryGetTopSkipTakeLiteral(arguments[1], context, out literal))
        return SqlOffsetSpec.Create(Number64.ToLong(literal.Value));
      throw new ArgumentException(ClientResources.InvalidSkipValue);
    }

    private static SqlLimitSpec VisitTakeLimit(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      if (arguments.Count != 2)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Take", (object) 2, (object) arguments.Count));
      SqlNumberLiteral literal;
      if (ExpressionToSql.TryGetTopSkipTakeLiteral(arguments[1], context, out literal))
        return SqlLimitSpec.Create(Number64.ToLong(literal.Value));
      throw new ArgumentException(ClientResources.InvalidTakeValue);
    }

    private static SqlTopSpec VisitTakeTop(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      if (arguments.Count != 2)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Take", (object) 2, (object) arguments.Count));
      SqlNumberLiteral literal;
      if (ExpressionToSql.TryGetTopSkipTakeLiteral(arguments[1], context, out literal))
        return SqlTopSpec.Create(Number64.ToLong(literal.Value));
      throw new ArgumentException(ClientResources.InvalidTakeValue);
    }

    private static SqlSelectClause VisitAggregateFunction(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context,
      string aggregateFunctionName)
    {
      SqlScalarExpression scalarExpression;
      if (arguments.Count == 1)
      {
        ParameterExpression newParamter = context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName);
        context.PushParameter(newParamter, context.CurrentSubqueryBinding.ShouldBeOnNewQuery);
        scalarExpression = ExpressionToSql.VisitParameter(newParamter, context);
        context.PopParameter();
      }
      else
        scalarExpression = arguments.Count == 2 ? ExpressionToSql.VisitScalarExpression(Utilities.GetLambda(arguments[1]), context) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) aggregateFunctionName, (object) 2, (object) arguments.Count));
      return SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin(aggregateFunctionName, scalarExpression)));
    }

    private static SqlSelectClause VisitDistinct(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      string str = "Distinct";
      if (arguments.Count != 1)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) str, (object) 1, (object) arguments.Count));
      ParameterExpression newParamter = context.GenerateNewParamter(typeof (object), ExpressionToSql.DefaultParameterName);
      return SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create(ExpressionToSql.VisitNonSubqueryScalarLambda(Expression.Lambda((Expression) newParamter, newParamter), context)), hasDistinct: true);
    }

    private static SqlSelectClause VisitCount(
      ReadOnlyCollection<Expression> arguments,
      TranslationContext context)
    {
      SqlScalarExpression scalarExpression = (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create(1L));
      if (arguments.Count == 2)
      {
        SqlWhereClause whereClause = ExpressionToSql.VisitWhere(arguments, context);
        context.currentQuery = context.currentQuery.AddWhereClause(whereClause, context);
      }
      else if (arguments.Count != 1)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidArgumentsCount, (object) "Count", (object) 2, (object) arguments.Count));
      return SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("COUNT", scalarExpression)));
    }

    private static SqlInputPathCollection ConvertPropertyRefToPath(
      SqlPropertyRefScalarExpression propRef)
    {
      List<SqlIdentifier> sqlIdentifierList = new List<SqlIdentifier>();
      while (true)
      {
        sqlIdentifierList.Add(propRef.PropertyIdentifier);
        SqlScalarExpression memberExpression = propRef.MemberExpression;
        if (memberExpression != null)
        {
          if (memberExpression is SqlPropertyRefScalarExpression)
            propRef = memberExpression as SqlPropertyRefScalarExpression;
          else
            break;
        }
        else
          goto label_5;
      }
      throw new DocumentQueryException(ClientResources.NotSupported);
label_5:
      if (sqlIdentifierList.Count == 0)
        throw new DocumentQueryException(ClientResources.NotSupported);
      SqlPathExpression sqlPathExpression = (SqlPathExpression) null;
      for (int index = sqlIdentifierList.Count - 2; index >= 0; --index)
      {
        SqlIdentifier sqlIdentifier = sqlIdentifierList[index];
        sqlPathExpression = (SqlPathExpression) SqlIdentifierPathExpression.Create(sqlPathExpression, sqlIdentifier);
      }
      return SqlInputPathCollection.Create(sqlIdentifierList[sqlIdentifierList.Count - 1], sqlPathExpression);
    }

    private static SqlInputPathCollection ConvertMemberIndexerToPath(
      SqlMemberIndexerScalarExpression memberIndexerExpression)
    {
      List<SqlStringLiteral> sqlStringLiteralList = new List<SqlStringLiteral>();
      SqlScalarExpression memberExpression;
      while (true)
      {
        sqlStringLiteralList.Add((SqlStringLiteral) ((SqlLiteralScalarExpression) memberIndexerExpression.IndexExpression).Literal);
        memberExpression = memberIndexerExpression.MemberExpression;
        switch (memberExpression)
        {
          case null:
            goto label_5;
          case SqlPropertyRefScalarExpression _:
            goto label_2;
          case SqlMemberIndexerScalarExpression _:
            memberIndexerExpression = memberExpression as SqlMemberIndexerScalarExpression;
            continue;
          default:
            goto label_4;
        }
      }
label_2:
      sqlStringLiteralList.Add(SqlStringLiteral.Create(((SqlPropertyRefScalarExpression) memberExpression).PropertyIdentifier.Value));
      goto label_5;
label_4:
      throw new DocumentQueryException(ClientResources.NotSupported);
label_5:
      if (sqlStringLiteralList.Count == 0)
        throw new ArgumentException(nameof (memberIndexerExpression));
      SqlPathExpression sqlPathExpression = (SqlPathExpression) null;
      for (int index = sqlStringLiteralList.Count - 2; index >= 0; --index)
        sqlPathExpression = (SqlPathExpression) SqlStringPathExpression.Create(sqlPathExpression, sqlStringLiteralList[index]);
      return SqlInputPathCollection.Create(SqlIdentifier.Create(sqlStringLiteralList[sqlStringLiteralList.Count - 1].Value), sqlPathExpression);
    }

    public static class LinqMethods
    {
      public const string Any = "Any";
      public const string Average = "Average";
      public const string Count = "Count";
      public const string Max = "Max";
      public const string Min = "Min";
      public const string OrderBy = "OrderBy";
      public const string ThenBy = "ThenBy";
      public const string OrderByDescending = "OrderByDescending";
      public const string ThenByDescending = "ThenByDescending";
      public const string Select = "Select";
      public const string SelectMany = "SelectMany";
      public const string Sum = "Sum";
      public const string Skip = "Skip";
      public const string Take = "Take";
      public const string Distinct = "Distinct";
      public const string Where = "Where";
    }
  }
}
