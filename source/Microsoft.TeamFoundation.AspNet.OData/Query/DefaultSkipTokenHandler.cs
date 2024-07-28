// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.DefaultSkipTokenHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.AspNet.OData.Query
{
  public class DefaultSkipTokenHandler : SkipTokenHandler
  {
    private const char CommaDelimiter = ',';
    private static char propertyDelimiter = '-';
    internal static DefaultSkipTokenHandler Instance = new DefaultSkipTokenHandler();

    public override Uri GenerateNextPageLink(
      Uri baseUri,
      int pageSize,
      object instance,
      ODataSerializerContext context)
    {
      if (context == null)
        return (Uri) null;
      if (pageSize <= 0)
        return (Uri) null;
      Func<object, string> objToSkipTokenValue = (Func<object, string>) null;
      IList<OrderByNode> orderByNodes = (IList<OrderByNode>) null;
      ExpandedReferenceSelectItem expandedSelectItem = context.CurrentExpandedSelectItem;
      IEdmModel model = context.Model;
      if (context.QueryContext.DefaultQuerySettings.EnableSkipToken)
      {
        if (expandedSelectItem != null)
        {
          if (TypedDelta.IsDeltaOfT(context.ExpandedResource.GetType()))
            return GetNextPageHelper.GetNextPageLink(baseUri, pageSize);
          if (expandedSelectItem.OrderByOption != null)
            orderByNodes = OrderByNode.CreateCollection(expandedSelectItem.OrderByOption);
          Func<object, string> objectToSkipTokenValue = (Func<object, string>) (obj => DefaultSkipTokenHandler.GenerateSkipTokenValue(obj, model, orderByNodes));
          return GetNextPageHelper.GetNextPageLink(baseUri, pageSize, instance, objectToSkipTokenValue);
        }
        if (context.QueryOptions != null && context.QueryOptions.OrderBy != null)
          orderByNodes = context.QueryOptions.OrderBy.OrderByNodes;
        objToSkipTokenValue = (Func<object, string>) (obj => DefaultSkipTokenHandler.GenerateSkipTokenValue(obj, model, orderByNodes));
      }
      return context.InternalRequest.GetNextPageLink(pageSize, instance, objToSkipTokenValue);
    }

    private static string GenerateSkipTokenValue(
      object lastMember,
      IEdmModel model,
      IList<OrderByNode> orderByNodes)
    {
      if (lastMember == null)
        return string.Empty;
      IEnumerable<IEdmProperty> propertiesForSkipToken = DefaultSkipTokenHandler.GetPropertiesForSkipToken(lastMember, model, orderByNodes);
      StringBuilder stringBuilder = new StringBuilder(string.Empty);
      if (propertiesForSkipToken == null)
        return stringBuilder.ToString();
      int num1 = 0;
      int num2 = propertiesForSkipToken.Count<IEdmProperty>() - 1;
      IEdmStructuredObject structuredObject = lastMember as IEdmStructuredObject;
      foreach (IEdmProperty edmProperty in propertiesForSkipToken)
      {
        bool flag = num1 == num2;
        string clrPropertyName = EdmLibHelpers.GetClrPropertyName(edmProperty, model);
        object obj;
        if (structuredObject != null)
          structuredObject.TryGetPropertyValue(clrPropertyName, out obj);
        else
          obj = lastMember.GetType().GetProperty(clrPropertyName).GetValue(lastMember);
        string str = obj != null ? (!edmProperty.Type.IsEnum() ? ODataUriUtils.ConvertToUriLiteral(obj, ODataVersion.V401, model) : ODataUriUtils.ConvertToUriLiteral((object) new ODataEnumValue(obj.ToString(), obj.GetType().FullName), ODataVersion.V401, model)) : ODataUriUtils.ConvertToUriLiteral(obj, ODataVersion.V401);
        stringBuilder.Append(edmProperty.Name).Append(DefaultSkipTokenHandler.propertyDelimiter).Append(str).Append(flag ? string.Empty : ','.ToString());
        ++num1;
      }
      return stringBuilder.ToString();
    }

    public override IQueryable<T> ApplyTo<T>(
      IQueryable<T> query,
      SkipTokenQueryOption skipTokenQueryOption)
    {
      return this.ApplyTo<T>(query, skipTokenQueryOption);
    }

    public override IQueryable ApplyTo(IQueryable query, SkipTokenQueryOption skipTokenQueryOption)
    {
      ODataQuerySettings querySettings = skipTokenQueryOption != null ? skipTokenQueryOption.QuerySettings : throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (skipTokenQueryOption));
      ODataQueryOptions queryOptions = skipTokenQueryOption.QueryOptions;
      IList<OrderByNode> orderByNodes = (IList<OrderByNode>) null;
      if (queryOptions != null)
      {
        OrderByQueryOption stableOrder = queryOptions.GenerateStableOrder();
        if (stableOrder != null)
          orderByNodes = stableOrder.OrderByNodes;
      }
      return DefaultSkipTokenHandler.ApplyToCore(query, querySettings, orderByNodes, skipTokenQueryOption.Context, skipTokenQueryOption.RawValue);
    }

    private static IQueryable ApplyToCore(
      IQueryable query,
      ODataQuerySettings querySettings,
      IList<OrderByNode> orderByNodes,
      ODataQueryContext context,
      string skipTokenRawValue)
    {
      if (context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) "ApplyTo");
      IDictionary<string, OrderByDirection> dictionary1 = orderByNodes == null ? (IDictionary<string, OrderByDirection>) new Dictionary<string, OrderByDirection>() : (IDictionary<string, OrderByDirection>) orderByNodes.OfType<OrderByPropertyNode>().ToDictionary<OrderByPropertyNode, string, OrderByDirection>((Func<OrderByPropertyNode, string>) (node => node.Property.Name), (Func<OrderByPropertyNode, OrderByDirection>) (node => node.Direction));
      IDictionary<string, object> dictionary2 = DefaultSkipTokenHandler.PopulatePropertyValuePairs(skipTokenRawValue, context);
      if (dictionary2.Count == 0)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation("Unable to get property values from the skiptoken value.");
      ExpressionBinderBase expressionBinderBase = (ExpressionBinderBase) new Microsoft.AspNet.OData.Query.Expressions.FilterBinder(context.RequestContainer);
      bool parameterization = querySettings.EnableConstantParameterization;
      ParameterExpression parameterExpression = Expression.Parameter(context.ElementClrType);
      Expression expression = (Expression) null;
      Expression left1 = (Expression) null;
      bool flag = true;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dictionary2)
      {
        string key = keyValuePair.Key;
        MemberExpression left2 = Expression.Property((Expression) parameterExpression, key);
        object obj = keyValuePair.Value;
        if (obj is ODataEnumValue odataEnumValue)
          obj = (object) odataEnumValue.Value;
        Expression right1 = parameterization ? LinqParameterContainer.Parameterize(obj.GetType(), obj) : (Expression) Expression.Constant(obj);
        Expression right2 = !dictionary1.ContainsKey(key) || dictionary1[key] != OrderByDirection.Descending ? expressionBinderBase.CreateBinaryExpression(BinaryOperatorKind.GreaterThan, (Expression) left2, right1, true) : expressionBinderBase.CreateBinaryExpression(BinaryOperatorKind.LessThan, (Expression) left2, right1, true);
        if (flag)
        {
          left1 = expressionBinderBase.CreateBinaryExpression(BinaryOperatorKind.Equal, (Expression) left2, right1, true);
          expression = right2;
          flag = false;
        }
        else
        {
          Expression right3 = (Expression) Expression.AndAlso(left1, right2);
          expression = (Expression) Expression.OrElse(expression, right3);
          left1 = (Expression) Expression.AndAlso(left1, expressionBinderBase.CreateBinaryExpression(BinaryOperatorKind.Equal, (Expression) left2, right1, true));
        }
      }
      Expression where = (Expression) Expression.Lambda(expression, parameterExpression);
      return ExpressionHelpers.Where(query, where, query.ElementType);
    }

    private static IDictionary<string, object> PopulatePropertyValuePairs(
      string value,
      ODataQueryContext context)
    {
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
      IList<string> stringList = DefaultSkipTokenHandler.ParseValue(value, ',');
      IEdmStructuredType elementType = context.ElementType as IEdmStructuredType;
      foreach (string str in (IEnumerable<string>) stringList)
      {
        string[] strArray = str.Split(new char[1]
        {
          DefaultSkipTokenHandler.propertyDelimiter
        }, 2);
        if (strArray.Length <= 1 || string.IsNullOrWhiteSpace(strArray[0]))
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.SkipTokenParseError);
        IEdmTypeReference typeReference = (IEdmTypeReference) null;
        IEdmProperty property = elementType.FindProperty(strArray[0]);
        if (property != null)
          typeReference = property.Type;
        object obj = ODataUriUtils.ConvertFromUriLiteral(strArray[1], ODataVersion.V401, context.Model, typeReference);
        dictionary.Add(strArray[0], obj);
      }
      return dictionary;
    }

    private static IList<string> ParseValue(string value, char delim)
    {
      IList<string> stringList = (IList<string>) new List<string>();
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < value.Length; ++index)
      {
        if (value[index] == '\'' || value[index] == '"')
        {
          stringBuilder.Append(value[index]);
          char ch = value[index];
          ++index;
          while (index < value.Length && (int) value[index] != (int) ch)
            stringBuilder.Append(value[index++]);
          if (index != value.Length)
            stringBuilder.Append(value[index]);
        }
        else if ((int) value[index] == (int) delim)
        {
          stringList.Add(stringBuilder.ToString());
          stringBuilder.Clear();
        }
        else
          stringBuilder.Append(value[index]);
      }
      string str = stringBuilder.ToString();
      if (!string.IsNullOrWhiteSpace(str))
        stringList.Add(str);
      return stringList;
    }

    private static IEnumerable<IEdmProperty> GetPropertiesForSkipToken(
      object lastMember,
      IEdmModel model,
      IList<OrderByNode> orderByNodes)
    {
      if (!(DefaultSkipTokenHandler.GetTypeFromObject(lastMember, model) is IEdmEntityType typeFromObject))
        return (IEnumerable<IEdmProperty>) null;
      IEnumerable<IEdmProperty> propertiesForSkipToken = (IEnumerable<IEdmProperty>) typeFromObject.Key();
      if (orderByNodes == null)
        return propertiesForSkipToken;
      if (orderByNodes.OfType<OrderByOpenPropertyNode>().Any<OrderByOpenPropertyNode>())
        return (IEnumerable<IEdmProperty>) null;
      IList<IEdmProperty> source = (IList<IEdmProperty>) orderByNodes.OfType<OrderByPropertyNode>().Select<OrderByPropertyNode, IEdmProperty>((Func<OrderByPropertyNode, IEdmProperty>) (p => p.Property)).AsList<IEdmProperty>();
      foreach (IEdmProperty edmProperty in propertiesForSkipToken)
      {
        if (!source.Contains(edmProperty))
          source.Add(edmProperty);
      }
      return source.AsEnumerable<IEdmProperty>();
    }

    private static IEdmType GetTypeFromObject(object value, IEdmModel model)
    {
      if (value is SelectExpandWrapper selectExpandWrapper)
        return selectExpandWrapper.GetEdmType().Definition;
      Type type = value.GetType();
      return model.GetEdmType(type);
    }
  }
}
