// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ExpressionHelper
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class ExpressionHelper
  {
    public static readonly MethodInfo CompareMethodInfo = typeof (string).GetMethod("Compare", new Type[2]
    {
      typeof (string),
      typeof (string)
    });
    public static readonly MethodInfo StartsWithMethodInfo = typeof (string).GetMethod("StartsWith", new Type[1]
    {
      typeof (string)
    });

    public static void ValidateBinaryExpression(BinaryExpression binaryExpression) => Utils.ThrowIfNull((object) binaryExpression, nameof (binaryExpression));

    public static void ValidateLeafExpression(BinaryExpression binaryExpression)
    {
      Utils.ThrowIfNull((object) binaryExpression, nameof (binaryExpression));
      ExpressionHelper.ValidateLeafNode(binaryExpression.Left);
      ExpressionHelper.ValidateLeafNode(binaryExpression.Right);
    }

    public static void ValidateConjunctiveExpression(BinaryExpression binaryExpression)
    {
      Utils.ThrowIfNull((object) binaryExpression, nameof (binaryExpression));
      ExpressionHelper.ValidateConjuctiveNode(binaryExpression.Left);
      ExpressionHelper.ValidateConjuctiveNode(binaryExpression.Right);
    }

    public static void ValidateLeafNode(Expression expression)
    {
      Utils.ThrowIfNull((object) expression, nameof (expression));
      if (expression.NodeType != ExpressionType.MemberAccess && expression.NodeType != ExpressionType.Constant && expression.NodeType != ExpressionType.Call)
        throw new ArgumentException("The expression has an invalid leaf node.");
    }

    public static void ValidateConjuctiveNode(Expression expression)
    {
      Utils.ThrowIfNull((object) expression, nameof (expression));
      if (expression is ConstantExpression)
        throw new ArgumentException("The expression has an invalid conjuctive node.");
    }

    public static BinaryExpression CreateEqualsExpression(
      Type entityType,
      GraphProperty propertyName,
      object propertyValue)
    {
      return ExpressionHelper.CreateConditionalExpression(entityType, propertyName, propertyValue, ExpressionType.Equal);
    }

    public static BinaryExpression CreateLessThanEqualsExpression(
      Type entityType,
      GraphProperty propertyName,
      object propertyValue)
    {
      return ExpressionHelper.CreateConditionalExpression(entityType, propertyName, propertyValue, ExpressionType.LessThanOrEqual);
    }

    public static BinaryExpression CreateGreaterThanEqualsExpression(
      Type entityType,
      GraphProperty propertyName,
      object propertyValue)
    {
      return ExpressionHelper.CreateConditionalExpression(entityType, propertyName, propertyValue, ExpressionType.GreaterThanOrEqual);
    }

    public static BinaryExpression CreateConditionalExpression(
      Type entityType,
      GraphProperty propertyName,
      object propertyValue,
      ExpressionType expressionType)
    {
      Utils.ThrowIfNull((object) entityType, nameof (entityType));
      Utils.ThrowIfNullOrEmpty((object) propertyName, nameof (propertyName));
      Utils.ThrowIfNullOrEmpty(propertyValue, nameof (propertyValue));
      if (expressionType != ExpressionType.Equal && expressionType != ExpressionType.GreaterThanOrEqual && expressionType != ExpressionType.LessThanOrEqual)
        throw new ArgumentException("Unsupported expression type.", nameof (expressionType));
      PropertyInfo propertyInfo;
      MemberExpression memberExpression = ExpressionHelper.GetMemberExpression(entityType, propertyName, out propertyInfo);
      Type propertyType = propertyInfo.PropertyType;
      string fullName1 = propertyType.FullName;
      bool flag = false;
      if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
      {
        fullName1 = propertyType.GetGenericArguments()[0].FullName;
        flag = true;
      }
      string fullName2 = propertyValue.GetType().FullName;
      if (!string.Equals(fullName1, fullName2))
        throw new ArgumentException("Property types do not match.");
      if ((expressionType == ExpressionType.GreaterThanOrEqual || expressionType == ExpressionType.LessThanOrEqual) && !string.Equals(typeof (string).FullName, fullName1) && !string.Equals(typeof (int).FullName, fullName1))
        throw new ArgumentException("Comparison is supported only on string and int values.");
      Expression left = (Expression) null;
      if (fullName1.Equals(typeof (string).FullName))
        left = (Expression) Expression.Call(ExpressionHelper.CompareMethodInfo, (Expression) memberExpression, (Expression) Expression.Constant(propertyValue));
      switch (expressionType)
      {
        case ExpressionType.Equal:
          return flag ? Expression.Equal((Expression) memberExpression, (Expression) Expression.Constant(propertyValue, propertyType)) : Expression.Equal((Expression) memberExpression, (Expression) Expression.Constant(propertyValue));
        case ExpressionType.GreaterThanOrEqual:
          return Expression.GreaterThanOrEqual(left, (Expression) Expression.Constant((object) 0));
        case ExpressionType.LessThanOrEqual:
          return Expression.LessThanOrEqual(left, (Expression) Expression.Constant((object) 0));
        default:
          throw new ArgumentException("Unsupported expression type.");
      }
    }

    public static MemberExpression GetMemberExpression(
      Type entityType,
      GraphProperty propertyName,
      out PropertyInfo propertyInfo)
    {
      Utils.ThrowIfNull((object) entityType, nameof (entityType));
      MemberInfo[] member = entityType.GetMember(propertyName.ToString());
      propertyInfo = member != null && member.Length == 1 ? member[0] as PropertyInfo : throw new ArgumentException("Unable to resolve the property in the specified type.");
      object[] customAttributes = propertyInfo.GetCustomAttributes(typeof (JsonPropertyAttribute), true);
      if (customAttributes == null || customAttributes.Length != 1)
        throw new ArgumentException("Invalid property used in the filter. JsonProperty attribute was not found.");
      return Expression.MakeMemberAccess((Expression) Expression.Constant(Activator.CreateInstance(entityType)), member[0]);
    }

    public static MethodCallExpression CreateStartsWithExpression(
      Type entityType,
      GraphProperty propertyName,
      string propertyValue)
    {
      Utils.ThrowIfNull((object) entityType, nameof (entityType));
      Utils.ThrowIfNullOrEmpty((object) propertyName, nameof (propertyName));
      Utils.ThrowIfNullOrEmpty((object) propertyValue, nameof (propertyValue));
      PropertyInfo propertyInfo;
      MemberExpression memberExpression = ExpressionHelper.GetMemberExpression(entityType, propertyName, out propertyInfo);
      if (!string.Equals(propertyInfo.PropertyType.FullName, propertyValue.GetType().FullName))
        throw new ArgumentException("Property types do not match.");
      return Expression.Call((Expression) memberExpression, "StartsWith", new Type[0], (Expression) Expression.Constant((object) propertyValue));
    }

    public static MethodCallExpression CreateAnyExpression(
      Type entityType,
      GraphProperty propertyName,
      object propertyValue)
    {
      Utils.ThrowIfNull((object) entityType, nameof (entityType));
      Utils.ThrowIfNullOrEmpty((object) propertyName, nameof (propertyName));
      Utils.ThrowIfNullOrEmpty(propertyValue, nameof (propertyValue));
      PropertyInfo propertyInfo;
      MemberExpression memberExpression = ExpressionHelper.GetMemberExpression(entityType, propertyName, out propertyInfo);
      Type propertyType = propertyInfo.PropertyType;
      Type[] genericArguments = propertyType.GetGenericArguments();
      if (!propertyType.IsGenericType || genericArguments == null || genericArguments.Length != 1 || !propertyType.Name.StartsWith(typeof (ChangeTrackingCollection<>).Name))
        throw new ArgumentException("Any expression is not supported on this type.");
      if (!string.Equals(propertyType.GetGenericArguments()[0].FullName, propertyValue.GetType().FullName))
        throw new ArgumentException("Property types do not match.");
      return Expression.Call((Expression) memberExpression, "Any", new Type[0], (Expression) Expression.Constant(propertyValue));
    }

    public static BinaryExpression JoinExpressions(
      Expression left,
      Expression right,
      ExpressionType expressionType)
    {
      Utils.ThrowIfNullOrEmpty((object) left, nameof (left));
      Utils.ThrowIfNullOrEmpty((object) right, nameof (right));
      switch (expressionType)
      {
        case ExpressionType.And:
          return Expression.And(left, right);
        case ExpressionType.Or:
          return Expression.Or(left, right);
        default:
          throw new ArgumentException("Invalid expressionType");
      }
    }

    public static string GetFormattedValue(object filterValue)
    {
      switch (filterValue)
      {
        case bool _:
          return filterValue.ToString().ToLower();
        case Guid _:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Guid'{0}'", new object[1]
          {
            filterValue
          });
        case byte[] numArray:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X'{0}'", new object[1]
          {
            (object) Utils.BinToHexEncode((IEnumerable<byte>) numArray)
          });
        case DateTime dateTime:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DateTime'{0:s}Z'", new object[1]
          {
            (object) dateTime.ToUniversalTime()
          });
        default:
          return "'" + filterValue + "'";
      }
    }

    public static string GetODataOperator(ExpressionType expressionType)
    {
      switch (expressionType)
      {
        case ExpressionType.Equal:
          return "eq";
        case ExpressionType.GreaterThanOrEqual:
          return "ge";
        case ExpressionType.LessThanOrEqual:
          return "le";
        default:
          throw new ArgumentException("Unsupported expression type");
      }
    }

    public static string GetPropertyName(BinaryExpression binaryExpression)
    {
      Utils.ThrowIfNull((object) binaryExpression, nameof (binaryExpression));
      if (!(binaryExpression.Left is MemberExpression memberExpression1))
        memberExpression1 = binaryExpression.Right as MemberExpression;
      if (memberExpression1 != null)
        return ExpressionHelper.GetPropertyName(memberExpression1);
      if (!(binaryExpression.Left is MethodCallExpression methodCallExpression))
        methodCallExpression = binaryExpression.Right as MethodCallExpression;
      if (methodCallExpression != null)
      {
        if (methodCallExpression.Method != ExpressionHelper.CompareMethodInfo)
          throw new ArgumentException("Unexpected method call in the binary expression.");
        if (!(methodCallExpression.Arguments[0] is MemberExpression memberExpression2))
          memberExpression2 = methodCallExpression.Arguments[1] as MemberExpression;
        if (memberExpression2 != null)
          return ExpressionHelper.GetPropertyName(memberExpression2);
      }
      throw new ArgumentException("Unable to extract the propertyName from the expression.");
    }

    public static string GetPropertyName(MemberExpression memberExpression)
    {
      Utils.ThrowIfNull((object) memberExpression, nameof (memberExpression));
      Utils.ThrowArgumentExceptionIfNullOrEmpty((object) memberExpression.Member, "Invalid MemberInfo");
      object[] customAttributes = memberExpression.Member.GetCustomAttributes(typeof (JsonPropertyAttribute), true);
      if (customAttributes == null || customAttributes.Length != 1)
        throw new ArgumentException("JsonProperty attribute was not found.");
      return customAttributes[0] is JsonPropertyAttribute propertyAttribute ? propertyAttribute.PropertyName : throw new ArgumentException("JsonProperty attribute was not found.");
    }

    public static string GetPropertyValue(BinaryExpression binaryExpression)
    {
      Utils.ThrowIfNull((object) binaryExpression, nameof (binaryExpression));
      if (!(binaryExpression.Left is MethodCallExpression methodCallExpression))
        methodCallExpression = binaryExpression.Right as MethodCallExpression;
      if (methodCallExpression != null)
      {
        if (methodCallExpression.Method != ExpressionHelper.CompareMethodInfo)
          throw new ArgumentException("Unexpected method call in the binary expression.");
        if (!(methodCallExpression.Arguments[1] is ConstantExpression constantExpression))
          constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;
        if (constantExpression != null)
          return ExpressionHelper.GetPropertyValue(constantExpression);
      }
      if (!(binaryExpression.Right is ConstantExpression constantExpression1))
        constantExpression1 = binaryExpression.Left as ConstantExpression;
      return constantExpression1 != null ? ExpressionHelper.GetPropertyValue(constantExpression1) : throw new ArgumentException("Unable to extract the constant property value from expression.");
    }

    public static string GetPropertyValue(ConstantExpression constantExpression)
    {
      Utils.ThrowIfNull((object) constantExpression, nameof (constantExpression));
      return ExpressionHelper.GetFormattedValue(constantExpression.Value);
    }

    public static string GetODataConjuctiveOperator(ExpressionType expressionType)
    {
      switch (expressionType)
      {
        case ExpressionType.And:
          return "and";
        case ExpressionType.Or:
          return "or";
        default:
          throw new ArgumentException("Unsupported expression type");
      }
    }
  }
}
