// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionManifestMethods
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class ExtensionManifestMethods
  {
    private static JsonSerializer s_serializer = new VssJsonMediaTypeFormatter().CreateJsonSerializer();
    private static MustacheTemplateParser s_defaultTemplateParser = new MustacheTemplateParser();

    public static string GetTemplateUriProperty(
      this ExtensionManifest extensionManifest,
      string uriTemplate,
      object replacementValues,
      MustacheTemplateParser customTemplateParser = null,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      if (extensionManifest.ServiceInstanceType.HasValue)
      {
        Guid? serviceInstanceType = extensionManifest.ServiceInstanceType;
        Guid empty = Guid.Empty;
        if ((serviceInstanceType.HasValue ? (serviceInstanceType.HasValue ? (serviceInstanceType.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
        {
          if (!(ExtensionManifestMethods.ObjectToJToken(replacementValues) is JObject jobject1))
            jobject1 = new JObject();
          JObject jobject2 = jobject1;
          serviceInstanceType = extensionManifest.ServiceInstanceType;
          JToken jtoken = (JToken) serviceInstanceType.Value;
          jobject2["$ServiceInstanceType"] = jtoken;
          replacementValues = (object) jobject1;
        }
      }
      string templateUriProperty = ExtensionManifestMethods.ResolveTemplatedProperty(uriTemplate, replacementValues, customTemplateParser, additionalEvaluationData);
      if (templateUriProperty == null || !templateUriProperty.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !templateUriProperty.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
      {
        string baseUri = extensionManifest.BaseUri;
        if (!string.IsNullOrEmpty(baseUri))
        {
          string str = ExtensionManifestMethods.ResolveTemplatedProperty(baseUri, replacementValues, customTemplateParser, additionalEvaluationData);
          if (!string.IsNullOrEmpty(str))
          {
            if (string.IsNullOrEmpty(templateUriProperty))
              templateUriProperty = str;
            else
              templateUriProperty = str.TrimEnd('/') + "/" + templateUriProperty.TrimStart('/');
          }
        }
      }
      return templateUriProperty;
    }

    private static string ResolveTemplatedProperty(
      string propertyValue,
      object replacementValues,
      MustacheTemplateParser customTemplateParser = null,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      if (!string.IsNullOrEmpty(propertyValue))
      {
        MustacheExpression mustacheExpression = (customTemplateParser == null ? ExtensionManifestMethods.s_defaultTemplateParser : customTemplateParser).Parse(propertyValue);
        if (mustacheExpression.IsContextBased)
          replacementValues = (object) ExtensionManifestMethods.ObjectToJToken(replacementValues);
        propertyValue = mustacheExpression.Evaluate(replacementValues, additionalEvaluationData);
      }
      return propertyValue;
    }

    private static JToken ObjectToJToken(object objectToConvert)
    {
      switch (objectToConvert)
      {
        case JToken jtoken:
        case null:
          return jtoken;
        default:
          jtoken = JToken.FromObject(objectToConvert, ExtensionManifestMethods.s_serializer);
          goto case null;
      }
    }
  }
}
