// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildParameter
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

namespace Microsoft.TeamFoundation.Build.Common
{
  [ContentProperty("Json")]
  [Editor("Microsoft.TeamFoundation.Build.Controls.WpfBuildParameterEditor, Microsoft.TeamFoundation.Build.Controls", "System.Activities.Presentation.PropertyEditing.PropertyValueEditor, System.Activities.Presentation")]
  public class BuildParameter
  {
    private JObject m_jsonObject;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public BuildParameter()
    {
    }

    public BuildParameter(object value)
    {
      ArgumentUtility.CheckForNull<object>(value, nameof (value));
      this.Json = BuildParameter.SerializeObjectToJson(value);
    }

    internal BuildParameter(JObject jObject) => this.m_jsonObject = jObject;

    public BuildParameter(string json)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(json, nameof (json));
      this.Json = json;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Json
    {
      get => BuildParameter.SerializeObjectToJson((object) this.m_jsonObject);
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        try
        {
          this.m_jsonObject = JsonConvert.DeserializeObject<JObject>(value);
        }
        catch (JsonReaderException ex)
        {
          throw new BuildParameterSerializationException(ex.Message, (Exception) ex);
        }
      }
    }

    internal JObject Value => this.m_jsonObject;

    public BuildParameter GetBuildParameter(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      JToken jtoken = this.m_jsonObject.SelectToken(path);
      return jtoken != null && jtoken is JObject ? new BuildParameter(jtoken.ToString()) : (BuildParameter) null;
    }

    public T GetValue<T>(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      bool returnedDefault = false;
      object objectValueByPath;
      try
      {
        objectValueByPath = (object) this.GetObjectValueByPath<T>(path, true, out returnedDefault);
      }
      catch (JsonException ex)
      {
        throw new BuildParameterNotFoundException(ex.Message, (Exception) ex);
      }
      return (T) objectValueByPath;
    }

    public T GetValue<T>(string path, T value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      bool returnedDefault = false;
      object objectValueByPath = (object) this.GetObjectValueByPath<T>(path, false, out returnedDefault);
      return objectValueByPath == null | returnedDefault ? value : (T) objectValueByPath;
    }

    private T GetObjectValueByPath<T>(string path, bool errorWhenNoMatch, out bool returnedDefault) => this.GetObjectValueByObject<T>((object) this.m_jsonObject.SelectToken(path, errorWhenNoMatch), out returnedDefault);

    private T GetObjectValueByObject<T>(object value, out bool returnedDefault)
    {
      returnedDefault = false;
      switch (value)
      {
        case null:
          returnedDefault = true;
          return default (T);
        case JValue _:
          JValue jvalue = value as JValue;
          if (typeof (T).IsEnum)
          {
            if (jvalue.Type == JTokenType.String)
            {
              try
              {
                return (T) Enum.Parse(typeof (T), jvalue.Value<string>(), true);
              }
              catch (ArgumentException ex)
              {
                returnedDefault = true;
                return default (T);
              }
            }
          }
          if (typeof (T).IsEnum && jvalue.Type == JTokenType.Integer)
            return (T) (ValueType) jvalue.Value<int>();
          if (typeof (T) == typeof (TimeSpan) && jvalue.Type == JTokenType.String)
            return (T) (ValueType) TimeSpan.Parse(jvalue.Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
          if (typeof (T) == typeof (Uri) && jvalue.Type == JTokenType.String)
            return (T) new Uri(jvalue.Value<string>());
          return typeof (T) == typeof (Guid) && jvalue.Type == JTokenType.String ? (T) (ValueType) new Guid(jvalue.Value<string>()) : jvalue.Value<T>();
        case JObject _:
          JObject jObject = value as JObject;
          string str = jObject.ToString();
          return typeof (T) == typeof (BuildParameter) ? (T) new BuildParameter(jObject) : JsonConvert.DeserializeObject<T>(str);
        case JArray _:
          JArray jarray = value as JArray;
          string objectValueByObject = jarray.ToString();
          if (typeof (T) == typeof (string))
            return (T) objectValueByObject;
          if (!(typeof (T) == typeof (BuildParameter[])))
            return JsonConvert.DeserializeObject<T>(objectValueByObject);
          List<BuildParameter> buildParameterList = new List<BuildParameter>();
          foreach (JToken jtoken in jarray)
          {
            if (jtoken is JObject)
              buildParameterList.Add(new BuildParameter(jtoken as JObject));
          }
          return (T) buildParameterList.ToArray();
        default:
          return (T) value;
      }
    }

    public T GetValue<T>()
    {
      bool returnedDefault = false;
      return this.GetObjectValueByObject<T>((object) this.m_jsonObject.Root, out returnedDefault);
    }

    public void SetValue<T>(string propertyName, T newValue)
    {
      JToken jtoken = JToken.FromObject((object) newValue);
      this.m_jsonObject[propertyName] = jtoken;
    }

    public void Merge(BuildParameter newBuildParameter)
    {
      if (newBuildParameter == null)
        return;
      foreach (KeyValuePair<string, JToken> keyValuePair in newBuildParameter.m_jsonObject)
      {
        JToken jtoken = this.m_jsonObject.SelectToken(keyValuePair.Key);
        if (jtoken != null && jtoken.Type == keyValuePair.Value.Type)
          this.m_jsonObject[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public override string ToString() => this.Json;

    public IDictionary<string, BuildParameterType> GetPropertyTypes()
    {
      Dictionary<string, BuildParameterType> propertyTypes = new Dictionary<string, BuildParameterType>();
      foreach (KeyValuePair<string, JToken> keyValuePair in this.m_jsonObject)
      {
        if (keyValuePair.Value != null)
          propertyTypes.Add(keyValuePair.Key, this.ConvertType(keyValuePair.Value.Type));
      }
      return (IDictionary<string, BuildParameterType>) propertyTypes;
    }

    public BuildParameterType GetArrayType(string propertyName)
    {
      JToken source = this.m_jsonObject.SelectToken(propertyName);
      if (source != null && source.Type == JTokenType.Array)
      {
        JToken jtoken = source.FirstOrDefault<JToken>();
        if (jtoken != null)
          return this.ConvertType(jtoken.Type);
      }
      return BuildParameterType.None;
    }

    private BuildParameterType ConvertType(JTokenType type)
    {
      switch (type)
      {
        case JTokenType.None:
          return BuildParameterType.None;
        case JTokenType.Object:
          return BuildParameterType.Object;
        case JTokenType.Array:
          return BuildParameterType.Array;
        case JTokenType.Integer:
          return BuildParameterType.Integer;
        case JTokenType.Float:
          return BuildParameterType.Float;
        case JTokenType.String:
          return BuildParameterType.String;
        case JTokenType.Boolean:
          return BuildParameterType.Boolean;
        case JTokenType.Null:
          return BuildParameterType.Null;
        case JTokenType.Date:
          return BuildParameterType.Date;
        case JTokenType.Guid:
          return BuildParameterType.Guid;
        case JTokenType.Uri:
          return BuildParameterType.Uri;
        case JTokenType.TimeSpan:
          return BuildParameterType.TimeSpan;
        default:
          return BuildParameterType.None;
      }
    }

    private static string SerializeObjectToJson(object value)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Include
      };
      return JsonConvert.SerializeObject(value, Formatting.None, settings);
    }
  }
}
