// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.TableRequestMessageFactory
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal static class TableRequestMessageFactory
  {
    internal static StorageRequestMessage BuildStorageRequestMessageForTableOperation(
      Uri uri,
      TableOperation operation,
      ICanonicalizer canonicalizer,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      HttpMethod httpMethod = RESTCommandGeneratorUtils.ExtractHttpMethod(operation);
      StorageRequestMessage message = new StorageRequestMessage(httpMethod, uri, canonicalizer, cred, cred.AccountName);
      message.Headers.AcceptCharset.ParseAdd("UTF-8");
      message.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
      TablePayloadFormat payloadFormat = options.PayloadFormat.Value;
      Logger.LogInformational(ctx, "Setting payload format for the request to '{0}'.", (object) payloadFormat);
      TableRequestMessageFactory.SetAcceptHeaderValueForStorageRequestMessage(message, payloadFormat);
      message.Headers.Add("DataServiceVersion", "3.0;");
      if (operation.OperationType == TableOperationType.InsertOrMerge || operation.OperationType == TableOperationType.Merge)
        message.Headers.Add("X-HTTP-Method", "MERGE");
      if ((operation.OperationType == TableOperationType.Delete || operation.OperationType == TableOperationType.Replace || operation.OperationType == TableOperationType.Merge) && operation.ETag != null)
        message.Headers.Add("If-Match", operation.ETag);
      if (operation.OperationType == TableOperationType.Insert)
        message.Headers.Add("Prefer", operation.EchoContent ? "return-content" : "return-no-content");
      if (operation.OperationType == TableOperationType.Insert || operation.OperationType == TableOperationType.Merge || operation.OperationType == TableOperationType.InsertOrMerge || operation.OperationType == TableOperationType.InsertOrReplace || operation.OperationType == TableOperationType.Replace)
      {
        MultiBufferMemoryStream bufferMemoryStream = new MultiBufferMemoryStream();
        using (JsonTextWriter jsonWriter = new JsonTextWriter((TextWriter) new StreamWriter((Stream) new NonCloseableStream((Stream) bufferMemoryStream))))
          TableRequestMessageFactory.WriteEntityContent(operation, ctx, options, jsonWriter);
        bufferMemoryStream.Seek(0L, SeekOrigin.Begin);
        message.Content = (HttpContent) new StreamContent((Stream) bufferMemoryStream);
      }
      if (httpMethod != HttpMethod.Head && httpMethod != HttpMethod.Get && message.Content != null)
        message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      return message;
    }

    internal static StorageRequestMessage BuildStorageRequestMessageForTableBatchOperation(
      Uri uri,
      TableBatchOperation batch,
      ICanonicalizer canonicalizer,
      string tableName,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      StorageRequestMessage message = new StorageRequestMessage(HttpMethod.Post, NavigationHelper.AppendPathToSingleUri(uri, "$batch"), canonicalizer, cred, cred.AccountName);
      message.Headers.AcceptCharset.ParseAdd("UTF-8");
      message.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
      TablePayloadFormat payloadFormat = options.PayloadFormat.Value;
      Logger.LogInformational(ctx, "Setting payload format for the request to '{0}'.", (object) payloadFormat);
      TableRequestMessageFactory.SetAcceptHeaderValueForStorageRequestMessage(message, payloadFormat);
      message.Headers.Add("DataServiceVersion", "3.0;");
      MultiBufferMemoryStream bufferMemoryStream = new MultiBufferMemoryStream();
      string str1 = Guid.NewGuid().ToString();
      using (StreamWriter streamWriter = new StreamWriter((Stream) new NonCloseableStream((Stream) bufferMemoryStream)))
      {
        string str2 = Guid.NewGuid().ToString();
        string str3 = "--batch_" + str1;
        string str4 = "--changeset_" + str2;
        string str5 = "Accept: ";
        switch (payloadFormat)
        {
          case TablePayloadFormat.JsonFullMetadata:
            str5 += "application/json;odata=fullmetadata";
            break;
          case TablePayloadFormat.Json:
            str5 += "application/json;odata=minimalmetadata";
            break;
          case TablePayloadFormat.JsonNoMetadata:
            str5 += "application/json;odata=nometadata";
            break;
        }
        streamWriter.WriteLine(str3);
        bool flag = batch.Count == 1 && batch[0].OperationType == TableOperationType.Retrieve;
        if (!flag)
        {
          streamWriter.WriteLine("Content-Type: multipart/mixed; boundary=changeset_" + str2);
          streamWriter.WriteLine();
        }
        foreach (TableOperation operation in batch)
        {
          HttpMethod httpMethod = RESTCommandGeneratorUtils.ExtractHttpMethod(operation);
          if (operation.OperationType == TableOperationType.Merge || operation.OperationType == TableOperationType.InsertOrMerge)
            httpMethod = new HttpMethod("MERGE");
          if (!flag)
            streamWriter.WriteLine(str4);
          streamWriter.WriteLine("Content-Type: application/http");
          streamWriter.WriteLine("Content-Transfer-Encoding: binary");
          streamWriter.WriteLine();
          string str6 = Uri.EscapeUriString(RESTCommandGeneratorUtils.GenerateRequestURI(operation, uri, tableName).ToString()).Replace("%25", "%");
          streamWriter.WriteLine(httpMethod.ToString() + " " + str6 + " HTTP/1.1");
          streamWriter.WriteLine(str5);
          streamWriter.WriteLine("Content-Type: application/json");
          if (operation.OperationType == TableOperationType.Insert)
            streamWriter.WriteLine("Prefer: " + (operation.EchoContent ? "return-content" : "return-no-content"));
          streamWriter.WriteLine("DataServiceVersion: 3.0;");
          if (operation.OperationType == TableOperationType.Delete || operation.OperationType == TableOperationType.Replace || operation.OperationType == TableOperationType.Merge)
            streamWriter.WriteLine("If-Match: " + operation.ETag);
          streamWriter.WriteLine();
          if (operation.OperationType != TableOperationType.Delete && operation.OperationType != TableOperationType.Retrieve)
          {
            using (JsonTextWriter jsonWriter = new JsonTextWriter((TextWriter) streamWriter))
            {
              jsonWriter.CloseOutput = false;
              TableRequestMessageFactory.WriteEntityContent(operation, ctx, options, jsonWriter);
            }
            streamWriter.WriteLine();
          }
        }
        if (!flag)
          streamWriter.WriteLine(str4 + "--");
        streamWriter.WriteLine(str3 + "--");
      }
      bufferMemoryStream.Seek(0L, SeekOrigin.Begin);
      message.Content = (HttpContent) new StreamContent((Stream) bufferMemoryStream);
      message.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/mixed");
      message.Content.Headers.ContentType.Parameters.Add(NameValueHeaderValue.Parse("boundary=batch_" + str1));
      return message;
    }

    internal static StorageRequestMessage BuildStorageRequestMessageForTableQuery(
      Uri uri,
      UriQueryBuilder builder,
      ICanonicalizer canonicalizer,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      StorageRequestMessage message = new StorageRequestMessage(HttpMethod.Get, builder.AddToUri(uri), canonicalizer, cred, cred.AccountName);
      message.Headers.AcceptCharset.ParseAdd("UTF-8");
      message.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
      TablePayloadFormat payloadFormat = options.PayloadFormat.Value;
      Logger.LogInformational(ctx, "Setting payload format for the request to '{0}'.", (object) payloadFormat);
      TableRequestMessageFactory.SetAcceptHeaderValueForStorageRequestMessage(message, payloadFormat);
      message.Headers.Add("DataServiceVersion", "3.0;");
      Logger.LogInformational(ctx, "Setting payload format for the request to '{0}'.", (object) payloadFormat);
      return message;
    }

    private static void SetAcceptHeaderValueForStorageRequestMessage(
      StorageRequestMessage message,
      TablePayloadFormat payloadFormat)
    {
      switch (payloadFormat)
      {
        case TablePayloadFormat.JsonFullMetadata:
          message.Headers.Accept.ParseAdd("application/json;odata=fullmetadata");
          break;
        case TablePayloadFormat.Json:
          message.Headers.Accept.ParseAdd("application/json;odata=minimalmetadata");
          break;
        case TablePayloadFormat.JsonNoMetadata:
          message.Headers.Accept.ParseAdd("application/json;odata=nometadata");
          break;
      }
    }

    private static void WriteEntityContent(
      TableOperation operation,
      OperationContext ctx,
      TableRequestOptions options,
      JsonTextWriter jsonWriter)
    {
      ITableEntity entity = operation.Entity;
      Dictionary<string, object> o = new Dictionary<string, object>();
      OperationContext operationContext = ctx;
      int operationType = (int) operation.OperationType;
      TableRequestOptions options1 = options;
      foreach (KeyValuePair<string, object> propertiesWithKey in TableRequestMessageFactory.GetPropertiesWithKeys(entity, operationContext, (TableOperationType) operationType, options1))
      {
        if (propertiesWithKey.Value != null)
        {
          if (propertiesWithKey.Value.GetType() == typeof (DateTime))
          {
            Dictionary<string, object> dictionary = o;
            string key = propertiesWithKey.Key;
            DateTime universalTime = (DateTime) propertiesWithKey.Value;
            universalTime = universalTime.ToUniversalTime();
            string str = universalTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
            dictionary[key] = (object) str;
            o[propertiesWithKey.Key + "@odata.type"] = (object) "Edm.DateTime";
          }
          else if (propertiesWithKey.Value.GetType() == typeof (byte[]))
          {
            o[propertiesWithKey.Key] = (object) Convert.ToBase64String((byte[]) propertiesWithKey.Value);
            o[propertiesWithKey.Key + "@odata.type"] = (object) "Edm.Binary";
          }
          else if (propertiesWithKey.Value.GetType() == typeof (long))
          {
            o[propertiesWithKey.Key] = (object) propertiesWithKey.Value.ToString();
            o[propertiesWithKey.Key + "@odata.type"] = (object) "Edm.Int64";
          }
          else if (propertiesWithKey.Value.GetType() == typeof (Guid))
          {
            o[propertiesWithKey.Key] = (object) propertiesWithKey.Value.ToString();
            o[propertiesWithKey.Key + "@odata.type"] = (object) "Edm.Guid";
          }
          else
            o[propertiesWithKey.Key] = propertiesWithKey.Value;
        }
      }
      JObject.FromObject((object) o, EntityTranslator.JsonSerializer).WriteTo((JsonWriter) jsonWriter);
    }

    private static IEnumerable<KeyValuePair<string, object>> GetPropertiesWithKeys(
      ITableEntity entity,
      OperationContext operationContext,
      TableOperationType operationType,
      TableRequestOptions options,
      bool ignoreEncryption = false)
    {
      if (operationType == TableOperationType.Insert)
      {
        if (entity.PartitionKey != null)
          yield return new KeyValuePair<string, object>("PartitionKey", (object) entity.PartitionKey);
        if (entity.RowKey != null)
          yield return new KeyValuePair<string, object>("RowKey", (object) entity.RowKey);
      }
      foreach (KeyValuePair<string, object> propertiesFrom in TableRequestMessageFactory.GetPropertiesFromDictionary(entity.WriteEntity(operationContext), options, entity.PartitionKey, entity.RowKey, ignoreEncryption))
        yield return propertiesFrom;
    }

    internal static IEnumerable<KeyValuePair<string, object>> GetPropertiesFromDictionary(
      IDictionary<string, EntityProperty> properties,
      TableRequestOptions options,
      string partitionKey,
      string rowKey,
      bool ignoreEncryption)
    {
      return properties.Select<KeyValuePair<string, EntityProperty>, KeyValuePair<string, object>>((Func<KeyValuePair<string, EntityProperty>, KeyValuePair<string, object>>) (kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value.PropertyAsObject)));
    }

    internal static StorageRequestMessage BuildStorageRequestMessageForTableServiceProperties(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      HttpMethod httpMethod,
      ServiceProperties properties,
      ICanonicalizer canonicalizer,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", nameof (properties));
      builder.Add("restype", "service");
      if (timeout.HasValue && timeout.Value > 0)
        builder.Add(nameof (timeout), timeout.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      StorageRequestMessage storageRequestMessage = new StorageRequestMessage(httpMethod, builder.AddToUri(uri), canonicalizer, cred, cred.AccountName);
      if (httpMethod.Equals(HttpMethod.Put))
      {
        MultiBufferMemoryStream bufferMemoryStream = new MultiBufferMemoryStream(1024);
        try
        {
          properties.WriteServiceProperties((Stream) bufferMemoryStream);
        }
        catch (InvalidOperationException ex)
        {
          bufferMemoryStream.Dispose();
          throw new ArgumentException(ex.Message, nameof (properties));
        }
        bufferMemoryStream.Seek(0L, SeekOrigin.Begin);
        storageRequestMessage.Content = (HttpContent) new StreamContent((Stream) bufferMemoryStream);
      }
      return storageRequestMessage;
    }

    internal static StorageRequestMessage BuildStorageRequestMessageForGetServiceStats(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      ICanonicalizer canonicalizer,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "stats");
      builder.Add("restype", "service");
      if (timeout.HasValue && timeout.Value > 0)
        builder.Add(nameof (timeout), timeout.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return new StorageRequestMessage(HttpMethod.Get, builder.AddToUri(uri), canonicalizer, cred, cred.AccountName);
    }

    internal static StorageRequestMessage BuildStorageRequestMessageForTablePermissions(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      HttpMethod httpMethod,
      TablePermissions permissions,
      ICanonicalizer canonicalizer,
      StorageCredentials cred,
      OperationContext ctx,
      TableRequestOptions options)
    {
      if (builder == null)
        builder = new UriQueryBuilder();
      builder.Add("comp", "acl");
      if (timeout.HasValue && timeout.Value > 0)
        builder.Add(nameof (timeout), timeout.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      StorageRequestMessage storageRequestMessage = new StorageRequestMessage(httpMethod, builder.AddToUri(uri), canonicalizer, cred, cred.AccountName);
      if (httpMethod.Equals(HttpMethod.Put))
      {
        MultiBufferMemoryStream bufferMemoryStream = new MultiBufferMemoryStream(1024);
        TableRequestMessageFactory.WriteSharedAccessIdentifiers(permissions.SharedAccessPolicies, (Stream) bufferMemoryStream);
        bufferMemoryStream.Seek(0L, SeekOrigin.Begin);
        storageRequestMessage.Content = (HttpContent) new StreamContent((Stream) bufferMemoryStream);
      }
      return storageRequestMessage;
    }

    private static void WriteSharedAccessIdentifiers(
      SharedAccessTablePolicies sharedAccessPolicies,
      Stream outputStream)
    {
      TableRequestMessageFactory.WriteSharedAccessIdentifiers<SharedAccessTablePolicy>((IDictionary<string, SharedAccessTablePolicy>) sharedAccessPolicies, outputStream, (Action<SharedAccessTablePolicy, XmlWriter>) ((policy, writer) =>
      {
        writer.WriteElementString("Start", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessStartTime));
        writer.WriteElementString("Expiry", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessExpiryTime));
        writer.WriteElementString("Permission", SharedAccessTablePolicy.PermissionsToString(policy.Permissions));
      }));
    }

    private static void WriteSharedAccessIdentifiers<T>(
      IDictionary<string, T> sharedAccessPolicies,
      Stream outputStream,
      Action<T, XmlWriter> writePolicyXml)
    {
      CommonUtility.AssertNotNull(nameof (sharedAccessPolicies), (object) sharedAccessPolicies);
      CommonUtility.AssertNotNull(nameof (outputStream), (object) outputStream);
      if (sharedAccessPolicies.Count > 5)
        throw new ArgumentOutOfRangeException(nameof (sharedAccessPolicies), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Too many '{0}' shared access policy identifiers provided. Server does not support setting more than '{1}' on a single container, queue, table, or share.", new object[2]
        {
          (object) sharedAccessPolicies.Count,
          (object) 5
        }));
      Stream output = outputStream;
      using (XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8
      }))
      {
        xmlWriter.WriteStartElement("SignedIdentifiers");
        foreach (string key in (IEnumerable<string>) sharedAccessPolicies.Keys)
        {
          xmlWriter.WriteStartElement("SignedIdentifier");
          xmlWriter.WriteElementString("Id", key);
          xmlWriter.WriteStartElement("AccessPolicy");
          T sharedAccessPolicy = sharedAccessPolicies[key];
          writePolicyXml(sharedAccessPolicy, xmlWriter);
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndElement();
        }
        xmlWriter.WriteEndDocument();
      }
    }
  }
}
