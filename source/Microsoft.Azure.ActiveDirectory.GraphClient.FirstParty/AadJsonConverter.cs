// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AadJsonConverter
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class AadJsonConverter : JsonConverter
  {
    private static ConcurrentDictionary<string, Dictionary<string, PropertyInfo>> aadToPropertyInfoMap = new ConcurrentDictionary<string, Dictionary<string, PropertyInfo>>();

    public override bool CanConvert(Type objectType) => typeof (GraphObject).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.StartArray)
      {
        List<JToken> list = JToken.ReadFrom(reader).ToList<JToken>();
        ChangeTrackingCollection<GraphObject> trackingCollection = new ChangeTrackingCollection<GraphObject>();
        AadJsonConverter aadJsonConverter = new AadJsonConverter();
        foreach (object obj in list)
        {
          GraphObject graphObject = JsonConvert.DeserializeObject(obj.ToString(), typeof (GraphObject), (JsonConverter) aadJsonConverter) as GraphObject;
          trackingCollection.Add(graphObject);
        }
        return (object) trackingCollection;
      }
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      List<JProperty> list1 = jobject.Properties().ToList<JProperty>();
      JProperty jproperty1 = list1.FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name == "odata.type"));
      Type implementationType;
      object instance;
      if (jproperty1 != null && SerializationHelper.TryGetImplementationForAadType(jproperty1.Value.ToString(), out implementationType) && typeof (GraphObject).IsAssignableFrom(implementationType))
      {
        instance = (object) (Activator.CreateInstance(implementationType) as GraphObject);
      }
      else
      {
        implementationType = objectType;
        instance = Activator.CreateInstance(implementationType);
      }
      if (JsonConvert.DeserializeObject(jobject.ToString(), implementationType) is GraphObject graphObject1 && jproperty1 != null)
      {
        Dictionary<string, PropertyInfo> propertyInfosForAadType = this.GetPropertyInfosForAadType(jproperty1.Value.ToString(), implementationType);
        foreach (JProperty jproperty2 in list1)
        {
          if (!propertyInfosForAadType.TryGetValue(jproperty2.Name, out PropertyInfo _))
            graphObject1.NonSerializedProperties[jproperty2.Name] = (object) jproperty2.Value.ToString();
        }
      }
      return (object) graphObject1;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      switch (value)
      {
        case null:
          break;
        case ChangeTrackingCollection<GraphObject> trackingCollection:
label_12:
          if (trackingCollection.Count > 0)
            throw new ArgumentException("Updating links is not supported from entity.");
          writer.WriteNull();
          break;
        case IEnumerable _:
          trackingCollection = new ChangeTrackingCollection<GraphObject>();
          IEnumerator enumerator = (value as IEnumerable).GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              if (!(enumerator.Current is GraphObject current))
                throw new ArgumentException("Each value in the ChangeTrackingCollection should be of type GraphObject.");
              trackingCollection.Add(current);
            }
            goto label_12;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        default:
          throw new ArgumentException("parameter 'value' is not of type IEnumerable.");
      }
    }

    private Dictionary<string, PropertyInfo> GetPropertyInfosForAadType(
      string graphMetadataType,
      Type graphObjectType)
    {
      if (string.IsNullOrEmpty(graphMetadataType))
        throw new ArgumentNullException(nameof (graphMetadataType));
      Dictionary<string, PropertyInfo> propertyInfosForAadType;
      if (!AadJsonConverter.aadToPropertyInfoMap.TryGetValue(graphMetadataType, out propertyInfosForAadType))
      {
        PropertyInfo[] properties = graphObjectType.GetProperties();
        propertyInfosForAadType = new Dictionary<string, PropertyInfo>();
        AadJsonConverter.aadToPropertyInfoMap[graphMetadataType] = propertyInfosForAadType;
        foreach (PropertyInfo propertyInfo in properties)
        {
          JsonPropertyAttribute customAttribute = Utils.GetCustomAttribute<JsonPropertyAttribute>(propertyInfo, false);
          if (customAttribute != null)
            propertyInfosForAadType[customAttribute.PropertyName] = propertyInfo;
        }
      }
      return propertyInfosForAadType;
    }
  }
}
