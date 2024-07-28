// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DataProviderSerializationUtil
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal static class DataProviderSerializationUtil
  {
    private static readonly DateTime s_javascriptDateOffset = new DateTime(1970, 1, 1);
    private static readonly Regex s_msJsonDateRegex = new Regex("(^|[^\\\\])\\\"\\\\/Date\\((-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\\\"");
    private static readonly JsonSerializer s_serializerRaw = new VssJsonMediaTypeFormatter(true, true, true).CreateJsonSerializer();
    private static readonly Guid s_internalGuid = Guid.NewGuid();
    private static readonly string s_internalDatePrefix = "/__msjson_date__" + DataProviderSerializationUtil.s_internalGuid.ToString() + "(";
    private const string c_msjsonDatePrefix = "/Date(";
    private const string c_dateSuffix = ")/";

    public static T DeserializeDataProviderResponse<T>(
      IVssRequestContext requestContext,
      string responseContent,
      bool useRegExForDateDeserialization)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (DeserializeDataProviderResponse)))
        return DataProviderSerializationUtil.DeserializeRawResponse(requestContext, responseContent, useRegExForDateDeserialization).ToObject<T>(DataProviderSerializationUtil.s_serializerRaw);
    }

    public static JToken DeserializeRawResponse(
      IVssRequestContext requestContext,
      string responseContent,
      bool useRegEx)
    {
      string a = (string) null;
      using (PerformanceTimer.StartMeasure(requestContext, "ReplaceContent" + (useRegEx ? "WithRegex" : "WithoutRegex")))
        a = !useRegEx ? responseContent.Replace("\\/Date(", "\\/__msjson_date__" + DataProviderSerializationUtil.s_internalGuid.ToString() + "(") : DataProviderSerializationUtil.s_msJsonDateRegex.Replace(responseContent, "$1{\"__msjson_date__\":$2}");
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        DateParseHandling = DateParseHandling.None
      };
      object obj = JsonConvert.DeserializeObject(a, settings);
      JToken result = obj as JToken;
      if (!string.Equals(a, responseContent))
      {
        if (result != null)
          DataProviderSerializationUtil.ReplaceDatesInJToken(result, useRegEx, (Action<DateTime>) (date => result = (JToken) date));
        else if (obj is string str)
          result = DataProviderSerializationUtil.GetValueToReplace((JToken) str);
      }
      return result;
    }

    private static void ReplaceDateArrayValues(JArray array, bool useRegEx)
    {
      for (int i = 0; i < array.Count; i++)
      {
        if (!useRegEx && array[i].Type == JTokenType.String)
          array[i] = DataProviderSerializationUtil.GetValueToReplace(array[i]);
        else
          DataProviderSerializationUtil.ReplaceDatesInJToken(array[i], useRegEx, (Action<DateTime>) (date => array[i] = (JToken) date));
      }
    }

    private static void ReplaceDateProperties(
      JObject obj,
      bool useRegEx,
      Action<DateTime> replaceAction)
    {
      List<JProperty> list = obj.Properties().ToList<JProperty>();
      if (useRegEx && list.Count == 1 && list[0].Name == "__msjson_date__" && list[0].Value.Type == JTokenType.Integer)
      {
        long num = (long) list[0].Value;
        DateTime dateTime = DataProviderSerializationUtil.s_javascriptDateOffset.AddMilliseconds((double) num);
        replaceAction(dateTime);
      }
      else
      {
        foreach (JProperty jproperty in list)
        {
          JProperty prop = jproperty;
          if (!useRegEx && prop.Value.Type == JTokenType.String)
            prop.Value = DataProviderSerializationUtil.GetValueToReplace(prop.Value);
          else
            DataProviderSerializationUtil.ReplaceDatesInJToken(prop.Value, useRegEx, (Action<DateTime>) (date => obj[prop.Name] = (JToken) date));
        }
      }
    }

    private static void ReplaceDatesInJToken(
      JToken token,
      bool useRegEx,
      Action<DateTime> objectReplaceAction)
    {
      if (token.Type == JTokenType.Array)
      {
        DataProviderSerializationUtil.ReplaceDateArrayValues(token as JArray, useRegEx);
      }
      else
      {
        if (token.Type != JTokenType.Object)
          return;
        DataProviderSerializationUtil.ReplaceDateProperties(token as JObject, useRegEx, objectReplaceAction);
      }
    }

    private static JToken GetValueToReplace(JToken value)
    {
      string str1 = (string) value;
      if (string.IsNullOrWhiteSpace(str1) || !str1.StartsWith(DataProviderSerializationUtil.s_internalDatePrefix))
        return value;
      string str2 = str1.Trim();
      if (str2.EndsWith(")/"))
      {
        string[] strArray = str2.Replace(DataProviderSerializationUtil.s_internalDatePrefix, string.Empty).Replace(")/", string.Empty).Split('+', '-');
        long result;
        if (strArray.Length != 0 && long.TryParse(strArray[0], out result))
          return (JToken) DataProviderSerializationUtil.s_javascriptDateOffset.AddMilliseconds((double) result);
      }
      return (JToken) str2.Replace(DataProviderSerializationUtil.s_internalDatePrefix, "/Date(");
    }
  }
}
