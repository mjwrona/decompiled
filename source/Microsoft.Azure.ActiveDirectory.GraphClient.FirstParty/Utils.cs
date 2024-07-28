// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Utils
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class Utils
  {
    private static ConcurrentDictionary<string, IList<PropertyInfo>> entityToKeysMap = new ConcurrentDictionary<string, IList<PropertyInfo>>();

    public static string GetUniqueKeyValue(GraphObject graphObject)
    {
      if (!Utils.entityToKeysMap.ContainsKey(graphObject.GetType().FullName))
        Utils.entityToKeysMap[graphObject.GetType().FullName] = Utils.GetEntityKeyPropertyInfos(graphObject.GetType());
      IList<PropertyInfo> entityToKeys = Utils.entityToKeysMap[graphObject.GetType().FullName];
      string str = (string) null;
      foreach (PropertyInfo element in (IEnumerable<PropertyInfo>) entityToKeys)
      {
        KeyAttribute customAttribute = element.GetCustomAttribute<KeyAttribute>(true);
        object obj = element.GetValue((object) graphObject);
        if (obj != null)
        {
          if (customAttribute.IsPrimary)
            return obj.ToString();
          str = obj.ToString();
        }
      }
      return !string.IsNullOrEmpty(str) ? str : throw new ArgumentException(string.Format("The object with type {0} does not have any unique key value set.", (object) graphObject.GetType().FullName));
    }

    public static IDictionary<string, object> GetSerializableGraphObject(GraphObject graphObject)
    {
      Utils.ThrowIfNull((object) graphObject, nameof (graphObject));
      IDictionary<string, object> serializableGraphObject = (IDictionary<string, object>) new Dictionary<string, object>(graphObject.ChangedProperties.Count);
      foreach (string changedProperty in graphObject.ChangedProperties)
      {
        PropertyInfo property = graphObject.GetType().GetProperty(changedProperty);
        JsonPropertyAttribute customAttribute = Utils.GetCustomAttribute<JsonPropertyAttribute>(property, true);
        serializableGraphObject.Add(customAttribute.PropertyName, property.GetValue((object) graphObject, (object[]) null));
      }
      return serializableGraphObject;
    }

    public static void LogResponseHeaders(WebHeaderCollection webHeaders)
    {
      if (webHeaders == null)
        return;
      Logger.Instance.Info("{0}: {1}", (object) "client-request-id", (object) webHeaders["client-request-id"]);
      Logger.Instance.Info("{0}: {1}", (object) "request-id", (object) webHeaders["request-id"]);
      Logger.Instance.Info("{0}: {1}", (object) "ocp-aad-diagnostics-server-name", (object) webHeaders["ocp-aad-diagnostics-server-name"]);
    }

    public static Uri GetListUri(
      GraphObject parent,
      Type objectType,
      GraphConnection graphConnection,
      string nextLink,
      FilterGenerator filter)
    {
      Utils.ThrowIfNullOrEmpty((object) graphConnection, nameof (graphConnection));
      Utils.ThrowIfNullOrEmpty((object) objectType, nameof (objectType));
      if (filter == null)
        filter = new FilterGenerator();
      EntityAttribute customAttribute = Utils.GetCustomAttribute<EntityAttribute>(objectType, true);
      string str = graphConnection.AadGraphEndpoint;
      UriBuilder uriBuilder;
      if (!string.IsNullOrEmpty(nextLink))
      {
        uriBuilder = new UriBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) str,
          (object) nextLink
        }));
      }
      else
      {
        if (parent != null)
          str = string.Format("{0}/{1}/{2}", (object) str, (object) Utils.GetCustomAttribute<EntityAttribute>(parent.GetType(), true).SetName, (object) Utils.GetUniqueKeyValue(parent));
        uriBuilder = new UriBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) str,
          (object) customAttribute.SetName
        }));
      }
      filter["api-version"] = graphConnection.GraphApiVersion;
      Utils.BuildQueryFromFilter(uriBuilder, filter);
      return uriBuilder.Uri;
    }

    public static Uri GetListUri(
      Type objectType,
      GraphConnection graphConnection,
      string nextLink,
      FilterGenerator filter)
    {
      return Utils.GetListUri((GraphObject) null, objectType, graphConnection, nextLink, filter);
    }

    public static Uri GetListUri<T>(
      GraphConnection graphConnection,
      string nextLink,
      FilterGenerator filter)
      where T : DirectoryObject
    {
      return Utils.GetListUri(typeof (T), graphConnection, nextLink, filter);
    }

    public static Uri GetRequestUri<T>(
      GraphConnection graphConnection,
      string objectId,
      params string[] fragments)
      where T : DirectoryObject
    {
      return Utils.GetRequestUri(graphConnection, typeof (T), objectId, (string) null, -1, (IList<QueryParameter>) null, fragments);
    }

    public static Uri GetRequestUri<T>(
      GraphConnection graphConnection,
      string objectId,
      string nextLink,
      int top,
      params string[] fragments)
      where T : DirectoryObject
    {
      return Utils.GetRequestUri(graphConnection, typeof (T), objectId, nextLink, top, (IList<QueryParameter>) null, fragments);
    }

    public static Uri GetRequestUri(
      GraphConnection graphConnection,
      Type typeOfEntityObject,
      string objectId,
      params string[] fragments)
    {
      return Utils.GetRequestUri(graphConnection, typeOfEntityObject, objectId, (string) null, -1, (IList<QueryParameter>) null, fragments);
    }

    public static Uri GetRequestUri(
      GraphConnection graphConnection,
      Type typeOfEntityObject,
      string objectId,
      IList<QueryParameter> queryParameters,
      params string[] fragments)
    {
      return Utils.GetRequestUri(graphConnection, typeOfEntityObject, objectId, (string) null, -1, queryParameters, fragments);
    }

    public static Uri GetRequestUri(
      GraphConnection graphConnection,
      Type parentType,
      string parentObjectId,
      Type containmentType,
      string containmentObjectId,
      params string[] fragments)
    {
      StringBuilder stringBuilder = new StringBuilder(Utils.GetCustomAttribute<EntityAttribute>(containmentType, true).SetName);
      if (!string.IsNullOrEmpty(containmentObjectId))
        stringBuilder.AppendFormat("/{0}", (object) containmentObjectId);
      IList<string> source = fragments == null ? (IList<string>) new List<string>() : (IList<string>) ((IEnumerable<string>) fragments).ToList<string>();
      source.Add(stringBuilder.ToString());
      return Utils.GetRequestUri(graphConnection, parentType, parentObjectId, (string) null, -1, (IList<QueryParameter>) null, source.ToArray<string>());
    }

    public static Uri GetRequestUri(
      GraphConnection graphConnection,
      Type typeOfEntityObject,
      string objectId,
      string nextLink,
      int top,
      IList<QueryParameter> queryParameters,
      params string[] fragments)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string aadGraphEndpoint = graphConnection.AadGraphEndpoint;
      if (!string.IsNullOrEmpty(nextLink))
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) aadGraphEndpoint,
          (object) nextLink
        });
      }
      else
      {
        if (typeOfEntityObject != (Type) null)
        {
          EntityAttribute customAttribute = Utils.GetCustomAttribute<EntityAttribute>(typeOfEntityObject, true);
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
          {
            (object) aadGraphEndpoint,
            (object) customAttribute.SetName
          });
        }
        else
          stringBuilder.Append(aadGraphEndpoint);
        if (!string.IsNullOrEmpty(objectId))
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "/{0}", new object[1]
          {
            (object) objectId
          });
        foreach (string fragment in fragments)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "/{0}", new object[1]
          {
            (object) fragment
          });
      }
      UriBuilder uriBuilder = new UriBuilder(stringBuilder.ToString());
      Utils.AddQueryParameter(uriBuilder, "api-version", graphConnection.GraphApiVersion, true);
      if (queryParameters != null)
      {
        foreach (QueryParameter queryParameter in (IEnumerable<QueryParameter>) queryParameters)
          Utils.AddQueryParameter(uriBuilder, queryParameter.ParameterName, queryParameter.ParameterValue, true);
      }
      if (top > 0)
        Utils.AddQueryParameter(uriBuilder, "$top", top.ToString(), true);
      return uriBuilder.Uri;
    }

    public static void AddQueryParameter(
      UriBuilder uriBuilder,
      string name,
      string value,
      bool overwriteExisting)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      if (string.IsNullOrEmpty(queryString["api-version"]) || overwriteExisting)
        queryString[name] = value;
      uriBuilder.Query = Utils.ToQueryString(queryString);
    }

    public static string ToQueryString(NameValueCollection queryArguments)
    {
      if (queryArguments == null || queryArguments.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string queryArgument in (NameObjectCollectionBase) queryArguments)
      {
        string str = queryArgument == null ? string.Empty : Uri.EscapeDataString(queryArgument) + "=";
        if (stringBuilder.Length > 0)
          stringBuilder.Append("&");
        string[] values = queryArguments.GetValues(queryArgument);
        if (values == null || values.Length == 0)
        {
          stringBuilder.Append(str);
        }
        else
        {
          for (int index = 0; index < values.Length; ++index)
          {
            if (index > 0)
              stringBuilder.Append("&");
            stringBuilder.Append(str);
            string stringToEscape = values[index];
            if (!string.IsNullOrEmpty(stringToEscape))
              stringBuilder.Append(Uri.EscapeDataString(stringToEscape));
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static void BuildQueryFromFilter(UriBuilder uriBuilder, FilterGenerator filter)
    {
      foreach (string name in filter.Names)
        Utils.AddQueryParameter(uriBuilder, name, filter[name], true);
      if (filter.ExpandProperty != LinkProperty.None)
        Utils.AddQueryParameter(uriBuilder, "$expand", Utils.GetLinkName(filter.ExpandProperty), true);
      if (filter.OrderByProperty != GraphProperty.None)
        Utils.AddQueryParameter(uriBuilder, "$orderby", Utils.GetPropertyName(filter.OrderByProperty), true);
      string str;
      if (!string.IsNullOrEmpty(filter.OverrideQueryFilter))
      {
        str = filter.QueryFilter == null ? filter.OverrideQueryFilter : throw new InvalidOperationException("Both QueryFilter and OverrideQueryFilter cannot be used at the same time.");
        if (str.StartsWith("$filter="))
          str = str.Substring("$filter=".Length);
      }
      else
        str = Utils.GetFilterQueryString(filter.QueryFilter);
      if (string.IsNullOrEmpty(str))
        return;
      Utils.AddQueryParameter(uriBuilder, "$filter", str, true);
    }

    public static string GetFilterQueryString(Expression expression)
    {
      switch (expression)
      {
        case null:
          return string.Empty;
        case BinaryExpression binaryExpression:
          ExpressionHelper.ValidateBinaryExpression(binaryExpression);
          switch (binaryExpression.NodeType)
          {
            case ExpressionType.And:
            case ExpressionType.Or:
              ExpressionHelper.ValidateConjunctiveExpression(binaryExpression);
              return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0}) {1} ({2})", new object[3]
              {
                (object) Utils.GetFilterQueryString(binaryExpression.Left),
                (object) ExpressionHelper.GetODataConjuctiveOperator(binaryExpression.NodeType),
                (object) Utils.GetFilterQueryString(binaryExpression.Right)
              });
            case ExpressionType.Equal:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThanOrEqual:
              ExpressionHelper.ValidateLeafExpression(binaryExpression);
              return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
              {
                (object) ExpressionHelper.GetPropertyName(binaryExpression),
                (object) ExpressionHelper.GetODataOperator(binaryExpression.NodeType),
                (object) ExpressionHelper.GetPropertyValue(binaryExpression)
              });
            default:
              throw new ArgumentException("Unsupported binary expression.");
          }
        case MethodCallExpression methodCallExpression:
          if (methodCallExpression.Method == ExpressionHelper.StartsWithMethodInfo)
          {
            if (methodCallExpression.Object == null)
              throw new ArgumentException("Unsupported StartsWith expression.");
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "startswith({0},{1})", new object[2]
            {
              (object) ExpressionHelper.GetPropertyName(methodCallExpression.Object as MemberExpression),
              (object) ExpressionHelper.GetPropertyValue(methodCallExpression.Arguments[0] as ConstantExpression)
            });
          }
          if (methodCallExpression.Method.Name.Equals("Any") && methodCallExpression.Arguments.Count == 1 && methodCallExpression.Method.ReturnType == typeof (bool) && methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant)
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/any(c:c eq {1})", new object[2]
            {
              (object) ExpressionHelper.GetPropertyName(methodCallExpression.Object as MemberExpression),
              (object) ExpressionHelper.GetPropertyValue(methodCallExpression.Arguments[0] as ConstantExpression)
            });
          break;
      }
      throw new ArgumentException("Unsupported expression.");
    }

    public static string GetTenantId(string accessToken)
    {
      Utils.ThrowIfNullOrEmpty((object) accessToken, nameof (accessToken));
      string[] strArray = accessToken.Split(".".ToCharArray());
      string str = strArray.Length == 3 ? strArray[1] : throw new ArgumentException(nameof (accessToken));
      switch (str.Length % 4)
      {
        case 1:
          throw new ArgumentException(nameof (accessToken));
        case 2:
          str += "==";
          break;
        case 3:
          str += "=";
          break;
      }
      string s = str.Replace('-', '+').Replace('_', '/');
      string tenantId = "myorganization";
      try
      {
        Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(Convert.FromBase64String(s)));
        if (dictionary != null)
        {
          if (dictionary.ContainsKey("tid"))
            tenantId = dictionary["tid"] as string;
        }
      }
      catch (JsonReaderException ex)
      {
        Logger.Instance.Warning("Unable to parse the token: {0}", (object) ex);
      }
      catch (FormatException ex)
      {
        Logger.Instance.Warning("Unable to parse the token: {0}", (object) ex);
      }
      catch (InvalidOperationException ex)
      {
        Logger.Instance.Warning("Unable to parse the token: {0}", (object) ex);
      }
      return tenantId;
    }

    public static T GetCustomAttribute<T>(Type sourceType, bool isRequired) where T : Attribute
    {
      Utils.ThrowIfNull((object) sourceType, nameof (sourceType));
      object[] customAttributes = sourceType.GetCustomAttributes(typeof (T), false);
      if (customAttributes == null || customAttributes.Length != 1)
      {
        if (isRequired)
          throw new ArgumentException(nameof (T), "Unable to get details about this property from the proxy.");
        return default (T);
      }
      if (!(customAttributes[0] is T customAttribute) && isRequired)
        throw new ArgumentException(nameof (T), "Unable to get details about this property from the proxy.");
      return customAttribute;
    }

    public static T GetCustomAttribute<T>(PropertyInfo propertyInfo, bool isRequired) where T : Attribute
    {
      Utils.ThrowIfNull((object) propertyInfo, nameof (propertyInfo));
      object[] customAttributes = propertyInfo.GetCustomAttributes(typeof (T), false);
      if (customAttributes == null || customAttributes.Length != 1)
      {
        if (isRequired)
          throw new ArgumentException(nameof (T), "Unable to get details about this entity from the proxy.");
        return default (T);
      }
      if (!(customAttributes[0] is T customAttribute) && isRequired)
        throw new ArgumentException(nameof (T), "Unable to get details about this entity from the proxy.");
      return customAttribute;
    }

    public static T GetCustomAttribute<T>(MethodInfo methodInfo, bool isRequired) where T : Attribute
    {
      Utils.ThrowIfNull((object) methodInfo, nameof (methodInfo));
      object[] customAttributes = methodInfo.GetCustomAttributes(typeof (T), false);
      if (customAttributes == null || customAttributes.Length != 1)
      {
        if (isRequired)
          throw new ArgumentException(nameof (T), "Unable to get information about this method from the proxy.");
        return default (T);
      }
      if (!(customAttributes[0] is T customAttribute) && isRequired)
        throw new ArgumentException(nameof (T), "Unable to get information about this method from the proxy.");
      return customAttribute;
    }

    public static void ValidateGraphObject(GraphObject graphObject, string parameterName)
    {
      Utils.ThrowIfNullOrEmpty((object) graphObject, parameterName);
      Utils.ThrowArgumentExceptionIfNullOrEmpty((object) Utils.GetUniqueKeyValue(graphObject), parameterName);
    }

    public static void ThrowIfNull(object toBeChecked, string parameterName)
    {
      if (toBeChecked == null)
        throw new ArgumentNullException(parameterName);
    }

    public static void ThrowIfNullOrEmpty(object toBeChecked, string parameterName)
    {
      switch (toBeChecked)
      {
        case string _:
          if (!string.IsNullOrEmpty(toBeChecked as string))
            break;
          throw new ArgumentNullException(parameterName);
        case null:
          throw new ArgumentNullException(parameterName);
        case Array array:
          if (array.Length != 0)
            break;
          throw new ArgumentNullException(parameterName);
      }
    }

    public static void ThrowArgumentExceptionIfNullOrEmpty(object toBeChecked, string parameterName)
    {
      if (toBeChecked is string)
      {
        if (string.IsNullOrEmpty(toBeChecked as string))
          throw new ArgumentException(parameterName + " is invalid");
      }
      else if (toBeChecked == null)
        throw new ArgumentException(parameterName + " is invalid");
    }

    public static string GetPropertyName(GraphProperty graphProperty)
    {
      if (graphProperty == GraphProperty.None || (GraphProperty) PropertyNameMap.NameMap.Count <= graphProperty)
        throw new ArgumentException("Invalid graph property");
      return PropertyNameMap.NameMap[(int) graphProperty];
    }

    public static string GetGraphObjectPropertyName(string propertyName)
    {
      if (!string.IsNullOrEmpty(propertyName))
      {
        int index = PropertyNameMap.NameMap.FindIndex((Predicate<string>) (x => x.Equals(propertyName, StringComparison.OrdinalIgnoreCase)));
        if (index != -1)
          return System.Enum.ToObject(typeof (GraphProperty), index).ToString();
      }
      return (string) null;
    }

    public static string GetLinkName(LinkProperty linkProperty)
    {
      if (linkProperty == LinkProperty.None || (LinkProperty) LinkNameMap.NameMap.Count <= linkProperty)
        throw new ArgumentException("Invalid link property");
      return LinkNameMap.NameMap[(int) linkProperty];
    }

    public static LinkAttribute GetLinkAttribute(Type entityType, LinkProperty linkProperty)
    {
      LinkAttribute linkAttribute = Utils.GetLinkAttribute(entityType, linkProperty.ToString());
      Utils.ThrowIfNull((object) linkAttribute, "Unable to lookup link information in the proxy.");
      return linkAttribute;
    }

    public static LinkAttribute GetLinkAttribute(Type entityType, string linkProperty)
    {
      Utils.ThrowIfNullOrEmpty((object) entityType, nameof (entityType));
      PropertyInfo property = entityType.GetProperty(linkProperty);
      Utils.ThrowIfNullOrEmpty((object) property, nameof (linkProperty));
      object[] customAttributes = property.GetCustomAttributes(typeof (LinkAttribute), true);
      return customAttributes.Length != 1 ? (LinkAttribute) null : customAttributes[0] as LinkAttribute;
    }

    public static string BinToHexEncode(IEnumerable<byte> bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in bytes)
        stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }

    public static byte[] HexToBinDecode(string hexString)
    {
      byte[] binDecode = hexString != null ? new byte[hexString.Length >> 1] : throw new ArgumentNullException(nameof (hexString));
      try
      {
        for (int index = 0; index < binDecode.Length; ++index)
          binDecode[index] = Convert.ToByte(hexString.Substring(index << 1, 2), 16);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException("Wrong input format.", nameof (hexString), (Exception) ex);
      }
      return binDecode;
    }

    private static IList<PropertyInfo> GetEntityKeyPropertyInfos(Type type) => (IList<PropertyInfo>) ((IEnumerable<PropertyInfo>) type.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetCustomAttribute<KeyAttribute>(true) != null)).ToList<PropertyInfo>();
  }
}
