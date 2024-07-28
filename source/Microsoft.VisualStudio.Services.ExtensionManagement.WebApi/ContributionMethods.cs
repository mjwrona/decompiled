// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionMethods
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class ContributionMethods
  {
    private static JsonSerializer s_serializer = new VssJsonMediaTypeFormatter().CreateJsonSerializer();

    public static T GetProperty<T>(
      this Contribution contribution,
      string propertyName,
      T defaultValue = null,
      bool ignoreInvalidTypeError = true)
    {
      if (contribution.Properties != null)
      {
        JToken jtoken = contribution.Properties.SelectToken(propertyName);
        if (jtoken != null)
        {
          try
          {
            return jtoken.ToObject<T>();
          }
          catch (Exception ex)
          {
            if (ignoreInvalidTypeError)
              return defaultValue;
            throw;
          }
        }
      }
      return defaultValue;
    }

    public static object GetPropertyRawValue(this Contribution contribution, string propertyName)
    {
      if (contribution.Properties == null)
        return (object) null;
      JToken token = contribution.Properties.SelectToken(propertyName);
      return token != null ? ContributionMethods.JTokenToObject(token) : (object) null;
    }

    public static void ReplaceProperty(
      this Contribution contribution,
      string propertyName,
      object o)
    {
      JToken jtoken = JToken.FromObject(o, ContributionMethods.s_serializer);
      contribution.Properties.SelectToken(propertyName).Replace(jtoken);
    }

    private static IDictionary<string, object> JObjectToDictionary(JObject jObject)
    {
      if (jObject == null)
        return (IDictionary<string, object>) null;
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
      foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
        dictionary[keyValuePair.Key] = ContributionMethods.JTokenToObject(keyValuePair.Value);
      return dictionary;
    }

    private static object JTokenToObject(JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:
          return (object) ContributionMethods.JObjectToDictionary(token as JObject);
        case JTokenType.Array:
          return (object) token.AsJEnumerable().Select<JToken, object>((Func<JToken, object>) (t => ContributionMethods.JTokenToObject(t))).ToArray<object>();
        case JTokenType.Integer:
        case JTokenType.Float:
          return (object) token.Value<double>();
        case JTokenType.String:
          return (object) token.Value<string>();
        case JTokenType.Boolean:
          return (object) token.Value<bool>();
        default:
          return (object) token;
      }
    }

    public static ReferenceLinks GetReferenceLinks(Contribution contribution, string propertyName)
    {
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (contribution.Properties != null && contribution.Properties.GetValue(propertyName) is JObject jobject1)
      {
        referenceLinks = new ReferenceLinks();
        foreach (KeyValuePair<string, JToken> keyValuePair in jobject1)
        {
          string key = keyValuePair.Key;
          if (keyValuePair.Value is JObject jobject)
          {
            JToken jtoken = jobject.GetValue("uri");
            if (jtoken != null && jtoken.Type == JTokenType.String)
            {
              string href = jtoken.Value<string>();
              referenceLinks.AddLink(key, href);
            }
          }
        }
      }
      return referenceLinks;
    }

    public static bool IsOfType(this Contribution contribution, string contributionTypeId) => string.Equals(contributionTypeId, contribution.Type, StringComparison.OrdinalIgnoreCase);

    public static bool IsTargeting(this Contribution contribution, string contributionId) => contribution.Targets != null && contribution.Targets.Contains(contributionId);
  }
}
