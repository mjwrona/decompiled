// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceProperties
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [JsonConverter(typeof (ResourcePropertiesJsonConverter))]
  public class ResourceProperties
  {
    private IDictionary<string, JToken> m_items;
    private static readonly JsonSerializer s_serializer = JsonUtility.CreateJsonSerializer();

    public ResourceProperties()
    {
    }

    internal ResourceProperties(IDictionary<string, JToken> items) => this.m_items = (IDictionary<string, JToken>) new Dictionary<string, JToken>(items, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private ResourceProperties(ResourceProperties propertiesToClone)
    {
      if (propertiesToClone == null)
        return;
      int? count = propertiesToClone.m_items?.Count;
      int num = 0;
      if (!(count.GetValueOrDefault() > num & count.HasValue))
        return;
      this.m_items = (IDictionary<string, JToken>) new Dictionary<string, JToken>(propertiesToClone.m_items, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public int Count
    {
      get
      {
        IDictionary<string, JToken> items = this.m_items;
        return items == null ? 0 : items.Count;
      }
    }

    internal IDictionary<string, JToken> Items
    {
      get
      {
        if (this.m_items == null)
          this.m_items = (IDictionary<string, JToken>) new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_items;
      }
    }

    public IReadOnlyDictionary<string, JToken> GetItems() => (IReadOnlyDictionary<string, JToken>) new ReadOnlyDictionary<string, JToken>(this.Items);

    public ResourceProperties Clone() => new ResourceProperties(this);

    public bool Delete(string name) => this.Items.Remove(name);

    public bool DeleteAllExcept(ISet<string> names)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) names, nameof (names));
      bool flag = false;
      IDictionary<string, JToken> items = this.m_items;
      if ((items != null ? (items.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (string name in this.m_items.Keys.Where<string>((Func<string, bool>) (x => !names.Contains(x))).ToArray<string>())
          flag |= this.Delete(name);
      }
      return flag;
    }

    public T Get<T>(string name, T defaultValue = null)
    {
      JToken jtoken;
      if (!this.Items.TryGetValue(name, out jtoken) || jtoken == null)
        return defaultValue;
      return typeof (T) == typeof (JToken) ? (T) jtoken : jtoken.ToObject<T>(ResourceProperties.s_serializer);
    }

    public bool TryGetValue<T>(string name, out T value)
    {
      JToken jtoken;
      if (this.Items.TryGetValue(name, out jtoken) && jtoken != null)
      {
        value = !(typeof (T) == typeof (JToken)) ? jtoken.ToObject<T>(ResourceProperties.s_serializer) : (T) jtoken;
        return true;
      }
      value = default (T);
      return false;
    }

    public void Set<T>(string name, T value)
    {
      if ((object) value == null)
        this.Items[name] = (JToken) null;
      else if (typeof (T) == typeof (JToken))
        this.Items[name] = (object) value as JToken;
      else
        this.Items[name] = JToken.FromObject((object) value, ResourceProperties.s_serializer);
    }

    public void UnionWith(ResourceProperties properties, bool overwrite = false)
    {
      if (properties?.m_items == null)
        return;
      foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) properties.m_items)
      {
        if (overwrite || !this.Items.ContainsKey(keyValuePair.Key))
          this.Items[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    internal IDictionary<string, object> ToStringDictionary() => (IDictionary<string, object>) this.Items.ToDictionary<KeyValuePair<string, JToken>, string, object>((Func<KeyValuePair<string, JToken>, string>) (x => x.Key), (Func<KeyValuePair<string, JToken>, object>) (x => ResourceProperties.ToObject(x.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private static object ToObject(JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:
          return (object) ResourceProperties.ToDictionary(token as JObject);
        case JTokenType.Array:
          return (object) (token as JArray).Select<JToken, object>((Func<JToken, object>) (x => ResourceProperties.ToObject(x))).ToList<object>();
        case JTokenType.Integer:
          return (object) Convert.ToString((int) token);
        case JTokenType.Float:
          return (object) Convert.ToString((float) token);
        case JTokenType.String:
          return (object) (string) token;
        case JTokenType.Boolean:
          return (object) Convert.ToString((bool) token);
        case JTokenType.Date:
          return (object) Convert.ToString((DateTime) token);
        case JTokenType.Guid:
          return (object) Convert.ToString((object) (Guid) token);
        case JTokenType.Uri:
          return (object) Convert.ToString((object) (Uri) token);
        case JTokenType.TimeSpan:
          return (object) Convert.ToString((object) (TimeSpan) token);
        default:
          return (object) null;
      }
    }

    private static IDictionary<string, object> ToDictionary(JObject @object)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (JProperty property in @object.Properties())
        dictionary[property.Name] = ResourceProperties.ToObject(property.Value);
      return (IDictionary<string, object>) dictionary;
    }
  }
}
