// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.JsonExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public static class JsonExtensions
  {
    private const string c_resourceUrlSelectToken = "resource.url";
    private const string c_resourceWorkItemFieldsSelectToken = "resource.fields";
    private const string c_workItemFieldRefNameSelectToken = "field.refName";
    private const string c_workItemFieldValueSelectToken = "value";

    public static T Flatten<T>(this JObject jObject, T seed, Func<T, string, JToken, T> aggregate) => jObject.Descendants().Where<JToken>((Func<JToken, bool>) (j => j.Children().Count<JToken>() == 0)).Aggregate<JToken, T>(seed, (Func<T, JToken, T>) ((acc, jToken) => aggregate(acc, jToken.GetJTokenPath(), jToken)));

    public static string GetJTokenPath(this JToken jToken)
    {
      if (jToken.Parent == null)
        return string.Empty;
      List<JToken> list = jToken.Ancestors().Reverse<JToken>().ToList<JToken>();
      list.Add(jToken);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < list.Count; ++index)
      {
        JToken jtoken1 = list[index];
        JToken jtoken2 = index + 1 < list.Count ? list[index + 1] : (JToken) null;
        if (jtoken2 != null)
        {
          switch (jtoken1.Type)
          {
            case JTokenType.Array:
            case JTokenType.Constructor:
              int num = ((IList<JToken>) jtoken1).IndexOf(jtoken2);
              stringBuilder.Append("[").Append(num).Append("]");
              continue;
            case JTokenType.Property:
              JProperty jproperty = (JProperty) jtoken1;
              if (stringBuilder.Length > 0)
                stringBuilder.Append(".");
              stringBuilder.Append(jproperty.Name);
              continue;
            default:
              continue;
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static JObject ToJObject(this Event raisedEvent) => JObject.FromObject((object) raisedEvent, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));

    public static string ResourceUrl(this JObject raisedEvent, bool throwErrorIfNotExist = false) => raisedEvent != null ? (string) raisedEvent.SelectToken("resource.url", throwErrorIfNotExist) : throw new ArgumentNullException(nameof (raisedEvent));

    public static string WorkItemFieldValue(
      this JObject raisedEvent,
      string fieldRefName,
      bool throwErrorIfNotExist = false)
    {
      if (raisedEvent == null)
        throw new ArgumentNullException(nameof (raisedEvent));
      if (string.IsNullOrWhiteSpace(fieldRefName))
        throw new ArgumentNullException(nameof (fieldRefName));
      JObject jobject = (JObject) null;
      JArray jarray = (JArray) raisedEvent.SelectToken("resource.fields", throwErrorIfNotExist);
      if (jarray != null)
        jobject = jarray.Children<JObject>().FirstOrDefault<JObject>((Func<JObject, bool>) (o => (string) o.SelectToken("field.refName", throwErrorIfNotExist) == fieldRefName));
      if (jobject != null)
        return (string) jobject.SelectToken("value", throwErrorIfNotExist);
      if (throwErrorIfNotExist)
        throw new JsonException(string.Format(CommonConsumerResources.Error_WorkItemFieldDoNotExist, (object) fieldRefName));
      return (string) null;
    }

    public static string GetStringRepresentation(
      this JObject jObject,
      bool useDefaultFormatting = true,
      Formatting formatting = Formatting.None)
    {
      formatting = useDefaultFormatting ? CommonConsumerSettings.JsonSerializerSettings.Formatting : formatting;
      JsonConverter[] array = CommonConsumerSettings.JsonSerializerSettings.Converters != null ? CommonConsumerSettings.JsonSerializerSettings.Converters.ToArray<JsonConverter>() : (JsonConverter[]) null;
      return jObject.ToString(formatting, array);
    }
  }
}
