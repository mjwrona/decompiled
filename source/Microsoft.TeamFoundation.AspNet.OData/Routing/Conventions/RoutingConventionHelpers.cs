// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.RoutingConventionHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  internal static class RoutingConventionHelpers
  {
    public static string SelectAction(
      this IEdmOperation operation,
      IWebApiActionMap actionMap,
      bool isCollection)
    {
      if (operation == null)
        return (string) null;
      IEdmOperationParameter operationParameter = operation.Parameters.FirstOrDefault<IEdmOperationParameter>();
      if (!operation.IsBound || operationParameter == null)
        return (string) null;
      IEdmEntityType edmEntityType = (IEdmEntityType) null;
      if (!isCollection)
        edmEntityType = operationParameter.Type.Definition as IEdmEntityType;
      else if (operationParameter.Type.Definition is IEdmCollectionType definition)
        edmEntityType = definition.ElementType.Definition as IEdmEntityType;
      if (edmEntityType == null)
        return (string) null;
      string str = isCollection ? operation.Name + "OnCollectionOf" + edmEntityType.Name : operation.Name + "On" + edmEntityType.Name;
      return actionMap.FindMatchingAction(str, operation.Name);
    }

    public static bool TryMatch(
      IDictionary<string, string> templateParameters,
      IDictionary<string, object> parameters,
      IDictionary<string, object> matches)
    {
      if (templateParameters.Count != parameters.Count)
        return false;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (KeyValuePair<string, string> templateParameter in (IEnumerable<KeyValuePair<string, string>>) templateParameters)
      {
        string key1 = templateParameter.Key;
        object obj;
        if (!parameters.TryGetValue(key1, out obj))
          return false;
        string key2 = templateParameter.Value;
        dictionary.Add(key2, obj);
      }
      foreach (KeyValuePair<string, object> keyValuePair in dictionary)
        matches[keyValuePair.Key] = keyValuePair.Value;
      return true;
    }

    public static bool TryMatch(
      this KeySegment keySegment,
      IDictionary<string, string> mapping,
      IDictionary<string, object> values)
    {
      if (keySegment.Keys.Count<KeyValuePair<string, object>>() != mapping.Count)
        return false;
      IEdmEntityType edmType = keySegment.EdmType as IEdmEntityType;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (KeyValuePair<string, object> key1 in keySegment.Keys)
      {
        KeyValuePair<string, object> key = key1;
        string name;
        if (!mapping.TryGetValue(key.Key, out name))
          return false;
        IEdmTypeReference edmTypeReference = edmType == null ? (IEdmTypeReference) EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(key.Value.GetType()) : (edmType.Key().FirstOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (k => k.Name == key.Key)) ?? edmType.Properties().OfType<IEdmStructuralProperty>().FirstOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Name == key.Key))).Type;
        RoutingConventionHelpers.AddKeyValues(name, key.Value, edmTypeReference, (IDictionary<string, object>) dictionary, (IDictionary<string, object>) dictionary);
      }
      foreach (KeyValuePair<string, object> keyValuePair in dictionary)
        values[keyValuePair.Key] = keyValuePair.Value;
      return true;
    }

    public static void AddKeyValueToRouteData(
      this IWebApiControllerContext controllerContext,
      KeySegment segment,
      string keyName = "key")
    {
      IDictionary<string, object> conventionsStore = controllerContext.Request.Context.RoutingConventionsStore;
      IEdmEntityType edmType = segment.EdmType as IEdmEntityType;
      int num = segment.Keys.Count<KeyValuePair<string, object>>();
      foreach (KeyValuePair<string, object> key in segment.Keys)
      {
        KeyValuePair<string, object> keyValuePair = key;
        bool flag = false;
        IEdmStructuralProperty structuralProperty = edmType.Key().FirstOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (k => k.Name == keyValuePair.Key));
        if (structuralProperty == null)
        {
          structuralProperty = edmType.Properties().OfType<IEdmStructuralProperty>().FirstOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Name == keyValuePair.Key));
          flag = true;
        }
        if (flag || num > 1)
        {
          RoutingConventionHelpers.AddKeyValues(keyName + keyValuePair.Key, keyValuePair.Value, structuralProperty.Type, controllerContext.RouteData, conventionsStore);
        }
        else
        {
          RoutingConventionHelpers.AddKeyValues(keyName, keyValuePair.Value, structuralProperty.Type, controllerContext.RouteData, conventionsStore);
          if (num == 1)
            RoutingConventionHelpers.AddKeyValues(keyName + keyValuePair.Key, keyValuePair.Value, structuralProperty.Type, controllerContext.RouteData, conventionsStore);
        }
      }
    }

    private static void AddKeyValues(
      string name,
      object value,
      IEdmTypeReference edmTypeReference,
      IDictionary<string, object> routeValues,
      IDictionary<string, object> odataValues)
    {
      object obj1 = (object) null;
      object obj2 = (object) null;
      if (value is ConstantNode constantNode)
      {
        if (constantNode.Value is ODataEnumValue paramValue)
        {
          obj2 = (object) new ODataParameterValue((object) paramValue, edmTypeReference);
          obj1 = (object) paramValue.Value;
        }
      }
      else
      {
        obj2 = (object) new ODataParameterValue(value, edmTypeReference);
        obj1 = value;
      }
      routeValues[name] = obj1;
      string key = "DF908045-6922-46A0-82F2-2F6E7F43D1B1_" + name;
      odataValues[key] = obj2;
    }

    public static void AddFunctionParameterToRouteData(
      this IWebApiControllerContext controllerContext,
      OperationSegment functionSegment)
    {
      IDictionary<string, object> conventionsStore = controllerContext.Request.Context.RoutingConventionsStore;
      if (!(functionSegment.Operations.First<IEdmOperation>() is IEdmFunction function))
        return;
      foreach (OperationSegmentParameter parameter in functionSegment.Parameters)
      {
        string name = parameter.Name;
        object parameterValue = functionSegment.GetParameterValue(name);
        RoutingConventionHelpers.AddFunctionParameters(function, name, parameterValue, controllerContext.RouteData, conventionsStore, (IDictionary<string, string>) null);
      }
      ODataOptionalParameter optionalParameter1 = new ODataOptionalParameter();
      foreach (IEdmOptionalParameter optionalParameter2 in function.Parameters.OfType<IEdmOptionalParameter>())
      {
        IEdmOptionalParameter optionalParameter = optionalParameter2;
        if (!functionSegment.Parameters.Any<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (c => c.Name == optionalParameter.Name)))
          optionalParameter1.Add(optionalParameter);
      }
      if (!optionalParameter1.OptionalParameters.Any<IEdmOptionalParameter>())
        return;
      controllerContext.RouteData.Add(ODataRouteConstants.OptionalParameters, (object) optionalParameter1);
    }

    public static void AddFunctionParameters(
      IEdmFunction function,
      string paramName,
      object paramValue,
      IDictionary<string, object> routeData,
      IDictionary<string, object> values,
      IDictionary<string, string> paramMapping)
    {
      IEdmOperationParameter parameter = function.FindParameter(paramName);
      ODataParameterValue odataParameterValue = new ODataParameterValue(paramValue, parameter.Type);
      string key1 = paramName;
      if (paramMapping != null)
        key1 = paramMapping[paramName];
      string key2 = "DF908045-6922-46A0-82F2-2F6E7F43D1B1_" + key1;
      values[key2] = (object) odataParameterValue;
      if (!routeData.ContainsKey(key1))
        routeData.Add(key1, paramValue);
      if (paramValue is ODataNullValue)
        routeData[key1] = (object) null;
      if (!(paramValue is ODataEnumValue odataEnumValue))
        return;
      routeData[key1] = (object) odataEnumValue.Value;
    }

    public static IDictionary<string, string> BuildParameterMappings(
      IEnumerable<OperationSegmentParameter> parameters,
      string segment)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (OperationSegmentParameter parameter in parameters)
      {
        string name = parameter.Name;
        string parameterName = (string) null;
        if (parameter.Value is ConstantNode constantNode)
        {
          if (constantNode.Value is UriTemplateExpression templateExpression)
            parameterName = templateExpression.LiteralText.Trim();
        }
        else
          parameterName = parameter.Value as string;
        string str = parameterName != null && RoutingConventionHelpers.IsRouteParameter(parameterName) ? parameterName.Substring(1, parameterName.Length - 2) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ParameterAliasMustBeInCurlyBraces, parameter.Value, (object) segment));
        dictionary[name] = !string.IsNullOrEmpty(str) ? str : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.EmptyParameterAlias, parameter.Value, (object) segment));
      }
      return (IDictionary<string, string>) dictionary;
    }

    public static bool IsRouteParameter(string parameterName) => parameterName.StartsWith("{", StringComparison.Ordinal) && parameterName.EndsWith("}", StringComparison.Ordinal);
  }
}
