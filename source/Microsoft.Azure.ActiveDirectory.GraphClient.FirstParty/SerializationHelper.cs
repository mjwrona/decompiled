// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.SerializationHelper
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class SerializationHelper
  {
    private static Type[] availableTypes = (Type[]) null;
    private static ConcurrentDictionary<string, Type> typeImplementations = new ConcurrentDictionary<string, Type>();
    private static ConcurrentDictionary<string, Type> entitySetTypeMap = new ConcurrentDictionary<string, Type>();
    private static JsonSerializer jsonSerializer = new JsonSerializer();

    public static string SerializeToJson(object serializableObject)
    {
      StringWriter stringWriter = new StringWriter();
      JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) stringWriter);
      jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
      SerializationHelper.jsonSerializer.Serialize(jsonWriter, serializableObject);
      return stringWriter.ToString();
    }

    public static PagedResults<T> DeserializeJsonResponse<T>(string json, Uri requestUri) where T : GraphObject
    {
      if (string.IsNullOrEmpty(json))
        return (PagedResults<T>) null;
      SerializationHelper.PopulateAvailableTypes();
      PagedResults<T> pagedResults = new PagedResults<T>();
      pagedResults.RequestUri = requestUri;
      if (!(JsonConvert.DeserializeObject(json) is JObject dictionaryToken1))
        throw new ArgumentException("Invalid json input.");
      bool isGraphObject = false;
      bool isCollection = false;
      Type targetObjectType = (Type) null;
      if (dictionaryToken1["odata.metadata"] != null)
      {
        pagedResults.ODataMetadataType = dictionaryToken1["odata.metadata"].ToString();
        SerializationHelper.ParseMetadataType(pagedResults.ODataMetadataType, out targetObjectType, out isGraphObject, out isCollection);
      }
      JToken jtoken;
      if (dictionaryToken1.TryGetValue("odata.nextLink", out jtoken))
        pagedResults.PageToken = jtoken.ToString();
      if (dictionaryToken1.TryGetValue("value", out jtoken))
      {
        if (isCollection)
        {
          foreach (JToken dictionaryToken2 in (IEnumerable<JToken>) jtoken)
          {
            if (isGraphObject)
              pagedResults.Results.Add(SerializationHelper.DeserializeGraphObject<T>(dictionaryToken2, targetObjectType));
            else
              pagedResults.MixedResults.Add(dictionaryToken2.ToString());
          }
        }
        else
          pagedResults.MixedResults.Add(jtoken.ToString());
      }
      else if (isGraphObject)
        pagedResults.Results.Add(SerializationHelper.DeserializeGraphObject<T>((JToken) dictionaryToken1, targetObjectType));
      else
        pagedResults.MixedResults.Add(dictionaryToken1.ToString());
      return pagedResults;
    }

    public static T DeserializeGraphObject<T>(JToken dictionaryToken, Type resultObjectType) where T : GraphObject
    {
      string str = dictionaryToken.ToString();
      Type type = resultObjectType;
      if ((object) type == null)
        type = typeof (T);
      JsonConverter[] jsonConverterArray = new JsonConverter[1]
      {
        (JsonConverter) new AadJsonConverter()
      };
      GraphObject graphObject = JsonConvert.DeserializeObject(str, type, jsonConverterArray) as GraphObject;
      graphObject.TokenDictionary = dictionaryToken;
      graphObject.PropertiesMaterializedFromDeserialization = (IList<string>) new List<string>((IEnumerable<string>) graphObject.ChangedProperties);
      graphObject.ChangedProperties.Clear();
      return graphObject is T obj ? obj : throw new GraphException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected type {0} obtained in response. Only objects of type {1} are expected.", new object[2]
      {
        (object) graphObject.GetType(),
        (object) typeof (T)
      }));
    }

    public static List<BatchResponseItem> DeserializeBatchResponse(
      string contentTypeHeader,
      string responseString,
      IList<BatchRequestItem> batchRequestItems)
    {
      List<BatchResponseItem> batchResponseItemList = new List<BatchResponseItem>();
      Utils.ThrowIfNullOrEmpty((object) contentTypeHeader, nameof (contentTypeHeader));
      string[] strArray1 = contentTypeHeader.Split(";".ToCharArray());
      string empty = string.Empty;
      foreach (string str in strArray1)
      {
        if (str.Trim().StartsWith("boundary="))
        {
          str.Trim().Substring("boundary=".Length);
          break;
        }
      }
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
      {
        bool flag = false;
        int index = 0;
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
        {
          WebHeaderCollection c = (WebHeaderCollection) null;
          while (!streamReader.EndOfStream)
          {
            string str = streamReader.ReadLine();
            if (!string.IsNullOrWhiteSpace(str))
            {
              if (str.StartsWith("HTTP/1.1 "))
                flag = !str.Contains("200 OK");
              else if (str.StartsWith("{\"odata.metadata"))
              {
                PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(str, batchRequestItems[index].RequestUri);
                ++index;
                BatchResponseItem batchResponseItem = new BatchResponseItem();
                batchResponseItem.Failed = flag;
                batchResponseItem.ResultSet = pagedResults;
                batchResponseItemList.Add(batchResponseItem);
                if (c != null)
                {
                  batchResponseItem.BatchHeaders.Add((NameValueCollection) c);
                  c = (WebHeaderCollection) null;
                }
              }
              else
              {
                if (str.StartsWith("{\"odata.error"))
                {
                  BatchResponseItem batchResponseItem = new BatchResponseItem()
                  {
                    Failed = flag,
                    Exception = ErrorResolver.ParseErrorMessageString(HttpStatusCode.BadRequest, str)
                  };
                  batchResponseItem.Exception.ResponseUri = batchRequestItems[index].RequestUri.ToString();
                  batchResponseItemList.Add(batchResponseItem);
                  if (c != null)
                  {
                    batchResponseItem.BatchHeaders.Add((NameValueCollection) c);
                    c = (WebHeaderCollection) null;
                  }
                }
                string[] strArray2 = str.Split(":".ToCharArray());
                if (strArray2 != null && strArray2.Length == 2)
                {
                  if (c == null)
                    c = new WebHeaderCollection();
                  c[strArray2[0]] = strArray2[1];
                }
              }
            }
          }
        }
      }
      return batchResponseItemList;
    }

    public static void ParseMetadataType(
      string metadataKey,
      out Type targetObjectType,
      out bool isGraphObject,
      out bool isCollection)
    {
      isGraphObject = false;
      isCollection = false;
      targetObjectType = (Type) null;
      if (string.IsNullOrEmpty(metadataKey))
        return;
      string[] strArray = metadataKey.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 4)
        return;
      string entitySet = strArray[3];
      int num = entitySet.IndexOf('#');
      if (num > 0)
        entitySet = entitySet.Substring(num + 1);
      SerializationHelper.TryGetTypeForEntitySet(entitySet, out targetObjectType);
      isGraphObject = targetObjectType != (Type) null;
      if (isGraphObject)
        isCollection = !metadataKey.EndsWith("@Element");
      else
        isCollection = entitySet.StartsWith("Collection");
    }

    public static bool TryGetTypeForEntitySet(string entitySet, out Type implementationType) => SerializationHelper.entitySetTypeMap.TryGetValue(entitySet, out implementationType);

    public static bool TryGetImplementationForAadType(string aadType, out Type implementationType) => SerializationHelper.typeImplementations.TryGetValue(aadType, out implementationType);

    private static void PopulateAvailableTypes()
    {
      if (SerializationHelper.availableTypes != null)
        return;
      SerializationHelper.availableTypes = Assembly.GetExecutingAssembly().GetTypes();
      foreach (Type availableType in SerializationHelper.availableTypes)
      {
        EntityAttribute customAttribute = Utils.GetCustomAttribute<EntityAttribute>(availableType, false);
        if (customAttribute != null)
        {
          foreach (string odataType in customAttribute.ODataTypes)
            SerializationHelper.typeImplementations[odataType] = availableType;
          if (!string.IsNullOrEmpty(customAttribute.SetName))
            SerializationHelper.entitySetTypeMap[customAttribute.SetName] = availableType;
        }
      }
    }
  }
}
