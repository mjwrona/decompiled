// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.StoredProcedureResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos
{
  internal class StoredProcedureResponse<TValue> : IStoredProcedureResponse<TValue>
  {
    private DocumentServiceResponse response;
    private TValue responseBody;
    private JsonSerializerSettings serializerSettings;

    public StoredProcedureResponse()
    {
    }

    internal StoredProcedureResponse(
      DocumentServiceResponse response,
      JsonSerializerSettings serializerSettings = null)
    {
      this.response = response;
      this.serializerSettings = serializerSettings;
      if (typeof (TValue).IsSubclassOf(typeof (JsonSerializable)))
      {
        if (typeof (TValue) == typeof (Document) || typeof (Document).IsAssignableFrom(typeof (TValue)))
        {
          this.responseBody = JsonSerializable.LoadFromWithConstructor<TValue>(response.ResponseBody, (Func<TValue>) (() => (TValue) new Document()), this.serializerSettings);
        }
        else
        {
          if (!(typeof (TValue) == typeof (Attachment)) && !typeof (Attachment).IsAssignableFrom(typeof (TValue)))
            throw new ArgumentException("Cannot serialize object if it is not document or attachment");
          this.responseBody = JsonSerializable.LoadFromWithConstructor<TValue>(response.ResponseBody, (Func<TValue>) (() => (TValue) new Attachment()), this.serializerSettings);
        }
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (StreamReader streamReader = new StreamReader(response.ResponseBody ?? (Stream) memoryStream))
          {
            string end = streamReader.ReadToEnd();
            try
            {
              this.responseBody = (TValue) JsonConvert.DeserializeObject(end, typeof (TValue), this.serializerSettings);
            }
            catch (JsonException ex)
            {
              throw new SerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to deserialize stored procedure response or convert it to type '{0}': {1}", (object) typeof (TValue).FullName, (object) ex.Message), (Exception) ex);
            }
          }
        }
      }
    }

    public string ActivityId => this.response.Headers["x-ms-activity-id"];

    public string SessionToken => this.response.Headers["x-ms-session-token"];

    public string ScriptLog => Helpers.GetScriptLogHeader(this.response.Headers);

    public HttpStatusCode StatusCode => this.response.StatusCode;

    public string MaxResourceQuota => this.response.Headers["x-ms-resource-quota"];

    public string CurrentResourceQuotaUsage => this.response.Headers["x-ms-resource-usage"];

    public double RequestCharge => Helpers.GetHeaderValueDouble(this.response.Headers, "x-ms-request-charge", 0.0);

    public NameValueCollection ResponseHeaders => this.response.ResponseHeaders;

    internal INameValueCollection Headers => this.response.Headers;

    public TValue Response => this.responseBody;

    internal IClientSideRequestStatistics RequestStatistics => this.response.RequestStats;

    public static implicit operator TValue(StoredProcedureResponse<TValue> source) => source.responseBody;
  }
}
