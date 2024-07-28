// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableOperationHttpResponseParsers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class TableOperationHttpResponseParsers
  {
    internal static TableResult TableOperationPreProcess(
      TableResult result,
      TableOperation operation,
      HttpResponseMessage resp,
      Exception ex)
    {
      result.HttpStatusCode = (int) resp.StatusCode;
      if (operation.OperationType == TableOperationType.Retrieve)
      {
        if (resp.StatusCode != HttpStatusCode.OK && resp.StatusCode != HttpStatusCode.NotFound)
        {
          CommonUtility.AssertNotNull(nameof (ex), (object) ex);
          throw ex;
        }
      }
      else
      {
        if (ex != null)
          throw ex;
        if (operation.OperationType == TableOperationType.Insert)
        {
          if (operation.EchoContent)
          {
            if (resp.StatusCode != HttpStatusCode.Created)
              throw ex;
          }
          else if (resp.StatusCode != HttpStatusCode.NoContent)
            throw ex;
        }
        else if (resp.StatusCode != HttpStatusCode.NoContent)
          throw ex;
      }
      string str = resp.Headers.ETag != null ? resp.Headers.ETag.ToString() : (string) null;
      if (str != null)
      {
        result.Etag = str;
        if (operation.Entity != null)
          operation.Entity.ETag = result.Etag;
      }
      return result;
    }

    internal static async Task<TableResult> TableOperationPostProcessAsync(
      TableResult result,
      TableOperation operation,
      RESTCommand<TableResult> cmd,
      HttpResponseMessage resp,
      OperationContext ctx,
      TableRequestOptions options,
      string accountName,
      CancellationToken cancellationToken)
    {
      string str = resp.Headers.ETag != null ? resp.Headers.ETag.ToString() : (string) null;
      if (operation.OperationType != TableOperationType.Retrieve && operation.OperationType != TableOperationType.Insert)
      {
        result.Etag = str;
        operation.Entity.ETag = result.Etag;
      }
      else if (operation.OperationType == TableOperationType.Insert && !operation.EchoContent)
      {
        if (str != null)
        {
          result.Etag = str;
          operation.Entity.ETag = result.Etag;
          operation.Entity.Timestamp = TableOperationHttpResponseParsers.ParseETagForTimestamp(result.Etag);
        }
      }
      else
      {
        MediaTypeHeaderValue contentType = resp.Content.Headers.ContentType;
        if (contentType.MediaType.Equals("application/json") && contentType.Parameters.Contains(NameValueHeaderValue.Parse("odata=nometadata")))
        {
          result.Etag = str;
          await TableOperationHttpResponseParsers.ReadEntityUsingJsonParserAsync(result, operation, cmd.ResponseStream, ctx, options, cancellationToken);
        }
        else
          await TableOperationHttpResponseParsers.ReadOdataEntityAsync(result, operation, cmd.ResponseStream, ctx, accountName, options, cancellationToken);
      }
      return result;
    }

    internal static async Task<TableBatchResult> TableBatchOperationPostProcessAsync(
      TableBatchResult result,
      TableBatchOperation batch,
      RESTCommand<TableBatchResult> cmd,
      HttpResponseMessage resp,
      OperationContext ctx,
      TableRequestOptions options,
      string accountName,
      CancellationToken cancellationToken)
    {
      StreamReader streamReader = new StreamReader(cmd.ResponseStream);
      string currentLine = await streamReader.ReadLineAsync().ConfigureAwait(false);
      ConfiguredTaskAwaitable<string> configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
      currentLine = await configuredTaskAwaitable;
      int index = 0;
      bool failError = false;
      while (currentLine != null && !currentLine.StartsWith("--batchresponse"))
      {
        for (; !currentLine.StartsWith("HTTP"); currentLine = await configuredTaskAwaitable)
          configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        int statusCode = int.Parse(currentLine.Substring(9, 3));
        Dictionary<string, string> headers = new Dictionary<string, string>();
        configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        for (currentLine = await configuredTaskAwaitable; !string.IsNullOrWhiteSpace(currentLine); currentLine = await configuredTaskAwaitable)
        {
          int length = currentLine.IndexOf(':');
          headers[currentLine.Substring(0, length)] = currentLine.Substring(length + 2);
          configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        }
        MemoryStream bodyStream = (MemoryStream) null;
        configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        currentLine = await configuredTaskAwaitable;
        if (statusCode != 204)
          bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(currentLine));
        configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        currentLine = await configuredTaskAwaitable;
        configuredTaskAwaitable = streamReader.ReadLineAsync().ConfigureAwait(false);
        currentLine = await configuredTaskAwaitable;
        TableOperation operation = batch[index];
        TableResult result1 = new TableResult()
        {
          Result = (object) operation.Entity
        };
        result.Add(result1);
        string str1 = (string) null;
        if (headers.ContainsKey("Content-Type"))
          str1 = headers["Content-Type"];
        result1.HttpStatusCode = statusCode;
        bool flag;
        if (operation.OperationType == TableOperationType.Insert)
        {
          failError = statusCode == 409;
          flag = !operation.EchoContent ? statusCode != 204 : statusCode != 201;
        }
        else if (operation.OperationType == TableOperationType.Retrieve)
        {
          if (statusCode == 404)
          {
            ++index;
            continue;
          }
          flag = statusCode != 200;
        }
        else
        {
          failError = statusCode == 404;
          flag = statusCode != 204;
        }
        if (failError)
        {
          if (cmd.ParseErrorAsync != null)
            cmd.CurrentResult.ExtendedErrorInformation = cmd.ParseErrorAsync((Stream) bodyStream, resp, str1, CancellationToken.None).Result;
          cmd.CurrentResult.HttpStatusCode = statusCode;
          if (!string.IsNullOrEmpty(cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage))
          {
            string errorMessage = cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage;
            cmd.CurrentResult.HttpStatusMessage = errorMessage.Substring(0, errorMessage.IndexOf("\n", StringComparison.Ordinal));
          }
          else
            cmd.CurrentResult.HttpStatusMessage = statusCode.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          throw new StorageException(cmd.CurrentResult, cmd.CurrentResult.ExtendedErrorInformation != null ? cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage : "An unknown error has occurred, extended error information not available.", (Exception) null)
          {
            IsRetryable = false
          };
        }
        if (flag)
        {
          if (cmd.ParseErrorAsync != null)
            cmd.CurrentResult.ExtendedErrorInformation = cmd.ParseErrorAsync((Stream) bodyStream, resp, str1, CancellationToken.None).Result;
          cmd.CurrentResult.HttpStatusCode = statusCode;
          if (!string.IsNullOrEmpty(cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage))
          {
            string errorMessage = cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage;
            cmd.CurrentResult.HttpStatusMessage = errorMessage.Substring(0, errorMessage.IndexOf("\n", StringComparison.Ordinal));
          }
          else
            cmd.CurrentResult.HttpStatusMessage = statusCode.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          string str2 = Convert.ToString(index, (IFormatProvider) CultureInfo.InvariantCulture);
          if (cmd.CurrentResult.ExtendedErrorInformation != null && !string.IsNullOrEmpty(cmd.CurrentResult.ExtendedErrorInformation.ErrorMessage))
          {
            string errorInformation = TableOperationHttpResponseParsers.ExtractEntityIndexFromExtendedErrorInformation(cmd.CurrentResult);
            if (!string.IsNullOrEmpty(errorInformation))
              str2 = errorInformation;
          }
          throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Element {0} in the batch returned an unexpected response code.", (object) str2), (Exception) null)
          {
            IsRetryable = true
          };
        }
        if (headers.ContainsKey("ETag") && !string.IsNullOrEmpty(headers["ETag"]))
        {
          result1.Etag = headers["ETag"];
          if (operation.Entity != null)
            operation.Entity.ETag = result1.Etag;
        }
        if (operation.OperationType == TableOperationType.Retrieve || operation.OperationType == TableOperationType.Insert && operation.EchoContent)
        {
          if (headers["Content-Type"].Contains("application/json;odata=nometadata"))
            await TableOperationHttpResponseParsers.ReadEntityUsingJsonParserAsync(result1, operation, (Stream) bodyStream, ctx, options, cancellationToken).ConfigureAwait(false);
          else
            await TableOperationHttpResponseParsers.ReadOdataEntityAsync(result1, operation, (Stream) bodyStream, ctx, accountName, options, cancellationToken).ConfigureAwait(false);
        }
        else if (operation.OperationType == TableOperationType.Insert)
          operation.Entity.Timestamp = TableOperationHttpResponseParsers.ParseETagForTimestamp(result1.Etag);
        ++index;
        headers = (Dictionary<string, string>) null;
        bodyStream = (MemoryStream) null;
      }
      return result;
    }

    internal static async Task<ResultSegment<TElement>> TableQueryPostProcessGenericAsync<TElement, TQueryType>(
      Stream responseStream,
      Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, TElement> resolver,
      HttpResponseMessage resp,
      TableRequestOptions options,
      OperationContext ctx,
      CancellationToken cancellationToken)
    {
      ResultSegment<TElement> retSeg = new ResultSegment<TElement>(new List<TElement>());
      retSeg.ContinuationToken = TableOperationHttpResponseParsers.ContinuationFromResponse(resp);
      MediaTypeHeaderValue contentType = resp.Content.Headers.ContentType;
      if (contentType.MediaType.Equals("application/json") && contentType.Parameters.Contains(NameValueHeaderValue.Parse("odata=nometadata")))
      {
        await TableOperationHttpResponseParsers.ReadQueryResponseUsingJsonParserAsync<TElement>(retSeg, responseStream, resp.Headers.ETag?.Tag, resolver, options.PropertyResolver, typeof (TQueryType), (OperationContext) null, options, cancellationToken);
      }
      else
      {
        foreach (KeyValuePair<string, Dictionary<string, object>> keyValuePair in await TableOperationHttpResponseParsers.ReadQueryResponseUsingJsonParserMetadataAsync(responseStream, cancellationToken))
          retSeg.Results.Add(TableOperationHttpResponseParsers.ReadAndResolve<TElement>(keyValuePair.Key, keyValuePair.Value, resolver, options));
      }
      Logger.LogInformational(ctx, "Retrieved '{0}' results with continuation token '{1}'.", (object) retSeg.Results.Count, (object) retSeg.ContinuationToken);
      return retSeg;
    }

    internal static async Task ReadQueryResponseUsingJsonParserAsync<TElement>(
      ResultSegment<TElement> retSeg,
      Stream responseStream,
      string etag,
      Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, TElement> resolver,
      Func<string, string, string, string, EdmType> propertyResolver,
      Type type,
      OperationContext ctx,
      TableRequestOptions options,
      CancellationToken cancellationToken)
    {
      StreamReader reader1 = new StreamReader(responseStream);
      bool disablePropertyResolverCache = false;
      if (TableEntity.DisablePropertyResolverCache)
      {
        disablePropertyResolverCache = TableEntity.DisablePropertyResolverCache;
        Logger.LogVerbose(ctx, "Property resolver cache is disabled.");
      }
      using (JsonReader reader2 = (JsonReader) new JsonTextReader((TextReader) reader1))
      {
        reader2.DateParseHandling = DateParseHandling.None;
        foreach (JToken token in (IEnumerable<JToken>) (await JObject.LoadAsync(reader2, cancellationToken).ConfigureAwait(false))["value"])
        {
          Dictionary<string, object> dictionary1 = TableOperationHttpResponseParsers.ReadSingleItem(token, out string _);
          Dictionary<string, string> entityAttributes = new Dictionary<string, string>();
          foreach (string key1 in dictionary1.Keys)
          {
            if (dictionary1[key1] == null)
              entityAttributes.Add(key1, (string) null);
            else if (dictionary1[key1] is string)
              entityAttributes.Add(key1, (string) dictionary1[key1]);
            else if (dictionary1[key1] is DateTime)
            {
              Dictionary<string, string> dictionary2 = entityAttributes;
              string key2 = key1;
              DateTime universalTime = (DateTime) dictionary1[key1];
              universalTime = universalTime.ToUniversalTime();
              string str = universalTime.ToString("o");
              dictionary2.Add(key2, str);
            }
            else
            {
              if (!(dictionary1[key1] is bool) && !(dictionary1[key1] is double) && !(dictionary1[key1] is int))
                throw new StorageException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid type in JSON object. Detected type is {0}, which is not a valid JSON type.", (object) dictionary1[key1].GetType().ToString()));
              entityAttributes.Add(key1, dictionary1[key1].ToString());
            }
          }
          retSeg.Results.Add(TableOperationHttpResponseParsers.ReadAndResolveWithEdmTypeResolver<TElement>(entityAttributes, resolver, propertyResolver, etag, type, ctx, disablePropertyResolverCache, options));
        }
        if (await reader2.ReadAsync(cancellationToken).ConfigureAwait(false))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The JSON reader has not yet reached the completed state."));
      }
    }

    public static async Task<List<KeyValuePair<string, Dictionary<string, object>>>> ReadQueryResponseUsingJsonParserMetadataAsync(
      Stream responseStream,
      CancellationToken cancellationToken)
    {
      List<KeyValuePair<string, Dictionary<string, object>>> results = new List<KeyValuePair<string, Dictionary<string, object>>>();
      using (JsonReader reader = (JsonReader) new JsonTextReader((TextReader) new StreamReader(responseStream)))
      {
        reader.DateParseHandling = DateParseHandling.None;
        foreach (JToken token in (IEnumerable<JToken>) (await JObject.LoadAsync(reader, cancellationToken).ConfigureAwait(false))["value"])
        {
          string etag;
          Dictionary<string, object> dictionary = TableOperationHttpResponseParsers.ReadSingleItem(token, out etag);
          results.Add(new KeyValuePair<string, Dictionary<string, object>>(etag, dictionary));
        }
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The JSON reader has not yet reached the completed state."));
      }
      return results;
    }

    internal static TableContinuationToken ContinuationFromResponse(HttpResponseMessage response)
    {
      HttpResponseHeaders headers = response.Headers;
      IEnumerable<string> values1 = (IEnumerable<string>) new List<string>();
      string str1 = (string) null;
      if (headers.TryGetValues("x-ms-continuation-NextPartitionKey", out values1))
        str1 = values1.First<string>();
      IEnumerable<string> values2 = (IEnumerable<string>) new List<string>();
      string str2 = (string) null;
      if (headers.TryGetValues("x-ms-continuation-NextRowKey", out values2))
        str2 = values2.First<string>();
      IEnumerable<string> values3 = (IEnumerable<string>) new List<string>();
      string str3 = (string) null;
      if (headers.TryGetValues("x-ms-continuation-NextTableName", out values3))
        str3 = values3.First<string>();
      string str4 = string.IsNullOrEmpty(str1) ? (string) null : str1;
      string str5 = string.IsNullOrEmpty(str2) ? (string) null : str2;
      string str6 = string.IsNullOrEmpty(str3) ? (string) null : str3;
      if (str4 == null && str5 == null && str6 == null)
        return (TableContinuationToken) null;
      return new TableContinuationToken()
      {
        NextPartitionKey = str4,
        NextRowKey = str5,
        NextTableName = str6
      };
    }

    private static DateTimeOffset ParseETagForTimestamp(string etag)
    {
      if (etag.StartsWith("W/", StringComparison.Ordinal))
        etag = etag.Substring(2);
      etag = etag.Substring("\"datetime'".Length, etag.Length - 2 - "\"datetime'".Length);
      return DateTimeOffset.Parse(Uri.UnescapeDataString(etag), (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
    }

    private static async Task ReadEntityUsingJsonParserAsync(
      TableResult result,
      TableOperation operation,
      Stream stream,
      OperationContext ctx,
      TableRequestOptions options,
      CancellationToken cancellationToken)
    {
      using (JsonReader reader = (JsonReader) new JsonTextReader((TextReader) new StreamReader(stream)))
      {
        Dictionary<string, object> dictionary = TableOperationHttpResponseParsers.ReadSingleItem((JToken) await JObject.LoadAsync(reader, cancellationToken).ConfigureAwait(false), out string _);
        Dictionary<string, string> entityAttributes = new Dictionary<string, string>();
        foreach (string key in dictionary.Keys)
        {
          if (dictionary[key] == null)
          {
            entityAttributes.Add(key, (string) null);
          }
          else
          {
            Type type = dictionary[key].GetType();
            if (type == typeof (string))
              entityAttributes.Add(key, (string) dictionary[key]);
            else if (type == typeof (DateTime))
              entityAttributes.Add(key, ((DateTime) dictionary[key]).ToUniversalTime().ToString("o"));
            else if (type == typeof (bool))
              entityAttributes.Add(key, ((bool) dictionary[key]).ToString());
            else if (type == typeof (double))
            {
              entityAttributes.Add(key, ((double) dictionary[key]).ToString());
            }
            else
            {
              if (!(type == typeof (int)))
                throw new StorageException();
              entityAttributes.Add(key, ((int) dictionary[key]).ToString());
            }
          }
        }
        if (operation.OperationType == TableOperationType.Retrieve)
          result.Result = TableOperationHttpResponseParsers.ReadAndResolveWithEdmTypeResolver<object>(entityAttributes, operation.RetrieveResolver, options.PropertyResolver, result.Etag, operation.PropertyResolverType, ctx, TableEntity.DisablePropertyResolverCache, options);
        else
          TableOperationHttpResponseParsers.ReadAndUpdateTableEntityWithEdmTypeResolver(operation.Entity, entityAttributes, EntityReadFlags.Timestamp | EntityReadFlags.Etag, options.PropertyResolver, ctx);
        if (reader.Read())
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The JSON reader has not yet reached the completed state."));
      }
    }

    private static Dictionary<string, object> ReadSingleItem(JToken token, out string etag)
    {
      Dictionary<string, object> source = token.ToObject<Dictionary<string, object>>();
      etag = !source.ContainsKey("odata.etag") ? (string) null : (string) source["odata.etag"];
      foreach (string key in source.Keys.Where<string>((Func<string, bool>) (key => key.StartsWith("odata.", StringComparison.Ordinal))).ToArray<string>())
        source.Remove(key);
      if (source.ContainsKey("Timestamp") && source["Timestamp"].GetType() == typeof (string))
        source["Timestamp"] = (object) DateTime.Parse((string) source["Timestamp"], (IFormatProvider) CultureInfo.InvariantCulture);
      if (source.ContainsKey("Timestamp@odata.type"))
        source.Remove("Timestamp@odata.type");
      foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => kvp.Value != null && kvp.Value.GetType() == typeof (long))).ToArray<KeyValuePair<string, object>>())
        source[keyValuePair.Key] = (object) (int) (long) keyValuePair.Value;
      foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => kvp.Key.EndsWith("@odata.type", StringComparison.Ordinal))).ToArray<KeyValuePair<string, object>>())
      {
        source.Remove(keyValuePair.Key);
        string key = keyValuePair.Key.Split(new char[1]
        {
          '@'
        }, StringSplitOptions.RemoveEmptyEntries)[0];
        switch ((string) keyValuePair.Value)
        {
          case "Edm.DateTime":
            source[key] = (object) DateTime.Parse((string) source[key], (IFormatProvider) null, DateTimeStyles.AdjustToUniversal);
            break;
          case "Edm.Binary":
            source[key] = (object) Convert.FromBase64String((string) source[key]);
            break;
          case "Edm.Guid":
            source[key] = (object) Guid.Parse((string) source[key]);
            break;
          case "Edm.Int64":
            source[key] = (object) long.Parse((string) source[key], (IFormatProvider) CultureInfo.InvariantCulture);
            break;
          default:
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected EDM type from the Table Service: {0}.", keyValuePair.Value));
        }
      }
      return source;
    }

    private static T ReadAndResolveWithEdmTypeResolver<T>(
      Dictionary<string, string> entityAttributes,
      Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, T> resolver,
      Func<string, string, string, string, EdmType> propertyResolver,
      string etag,
      Type type,
      OperationContext ctx,
      bool disablePropertyResolverCache,
      TableRequestOptions options)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      DateTimeOffset dateTimeOffset = new DateTimeOffset();
      Dictionary<string, EntityProperty> properties = new Dictionary<string, EntityProperty>();
      Dictionary<string, EdmType> dictionary = (Dictionary<string, EdmType>) null;
      HashSet<string> encryptedPropertyDetailsSet = (HashSet<string>) null;
      if (type != (Type) null)
        dictionary = disablePropertyResolverCache ? TableOperationHttpResponseParsers.CreatePropertyResolverDictionary(type) : TableEntity.PropertyResolverCache.GetOrAdd(type, new Func<Type, Dictionary<string, EdmType>>(TableOperationHttpResponseParsers.CreatePropertyResolverDictionary));
      foreach (KeyValuePair<string, string> entityAttribute in entityAttributes)
      {
        if (entityAttribute.Key == "PartitionKey")
          str1 = entityAttribute.Value;
        else if (entityAttribute.Key == "RowKey")
          str2 = entityAttribute.Value;
        else if (entityAttribute.Key == "Timestamp")
        {
          dateTimeOffset = DateTimeOffset.Parse(entityAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
          if (etag == null)
            etag = TableOperationHttpResponseParsers.GetETagFromTimestamp(entityAttribute.Value);
        }
        else if (propertyResolver != null)
        {
          Logger.LogVerbose(ctx, "Using the property resolver provided via TableRequestOptions to deserialize the entity.");
          try
          {
            EdmType edmType = propertyResolver(str1, str2, entityAttribute.Key, entityAttribute.Value);
            Logger.LogVerbose(ctx, "Attempting to deserialize '{0}' as type '{1}'", (object) entityAttribute.Key, (object) edmType);
            try
            {
              TableOperationHttpResponseParsers.CreateEntityPropertyFromObject(properties, encryptedPropertyDetailsSet, entityAttribute, edmType);
            }
            catch (FormatException ex)
            {
              throw new StorageException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to parse property '{0}' with value '{1}' as type '{2}'", (object) entityAttribute.Key, (object) entityAttribute.Value, (object) edmType), (Exception) ex)
              {
                IsRetryable = false
              };
            }
          }
          catch (StorageException ex)
          {
            throw;
          }
          catch (Exception ex)
          {
            throw new StorageException("The custom property resolver delegate threw an exception. Check the inner exception for more details.", ex)
            {
              IsRetryable = false
            };
          }
        }
        else if (type != (Type) null)
        {
          Logger.LogVerbose(ctx, "Using the default property resolver to deserialize the entity.");
          if (dictionary != null)
          {
            EdmType edmType;
            dictionary.TryGetValue(entityAttribute.Key, out edmType);
            Logger.LogVerbose(ctx, "Attempting to deserialize '{0}' as type '{1}'", (object) entityAttribute.Key, (object) edmType);
            TableOperationHttpResponseParsers.CreateEntityPropertyFromObject(properties, encryptedPropertyDetailsSet, entityAttribute, edmType);
          }
        }
        else
        {
          Logger.LogVerbose(ctx, "No property resolver available. Deserializing the entity properties as strings.");
          TableOperationHttpResponseParsers.CreateEntityPropertyFromObject(properties, encryptedPropertyDetailsSet, entityAttribute, EdmType.String);
        }
      }
      return resolver(str1, str2, dateTimeOffset, (IDictionary<string, EntityProperty>) properties, etag);
    }

    private static Dictionary<string, EdmType> CreatePropertyResolverDictionary(Type type)
    {
      Dictionary<string, EdmType> resolverDictionary = new Dictionary<string, EdmType>();
      foreach (PropertyInfo property in (IEnumerable<PropertyInfo>) type.GetProperties())
      {
        if (property.PropertyType == typeof (byte[]))
          resolverDictionary.Add(property.Name, EdmType.Binary);
        else if (property.PropertyType == typeof (bool) || property.PropertyType == typeof (bool?))
          resolverDictionary.Add(property.Name, EdmType.Boolean);
        else if (property.PropertyType == typeof (DateTime) || property.PropertyType == typeof (DateTime?) || property.PropertyType == typeof (DateTimeOffset) || property.PropertyType == typeof (DateTimeOffset?))
          resolverDictionary.Add(property.Name, EdmType.DateTime);
        else if (property.PropertyType == typeof (double) || property.PropertyType == typeof (double?))
          resolverDictionary.Add(property.Name, EdmType.Double);
        else if (property.PropertyType == typeof (Guid) || property.PropertyType == typeof (Guid?))
          resolverDictionary.Add(property.Name, EdmType.Guid);
        else if (property.PropertyType == typeof (int) || property.PropertyType == typeof (int?))
          resolverDictionary.Add(property.Name, EdmType.Int32);
        else if (property.PropertyType == typeof (long) || property.PropertyType == typeof (long?))
          resolverDictionary.Add(property.Name, EdmType.Int64);
        else
          resolverDictionary.Add(property.Name, EdmType.String);
      }
      return resolverDictionary;
    }

    private static string GetETagFromTimestamp(string timeStampString)
    {
      timeStampString = Uri.EscapeDataString(timeStampString);
      return "W/\"datetime'" + timeStampString + "'\"";
    }

    private static void CreateEntityPropertyFromObject(
      Dictionary<string, EntityProperty> properties,
      HashSet<string> encryptedPropertyDetailsSet,
      KeyValuePair<string, string> prop,
      EdmType edmType)
    {
      if (encryptedPropertyDetailsSet != null && encryptedPropertyDetailsSet.Contains(prop.Key))
        properties.Add(prop.Key, EntityProperty.CreateEntityPropertyFromObject((object) prop.Value, EdmType.Binary));
      else
        properties.Add(prop.Key, EntityProperty.CreateEntityPropertyFromObject((object) prop.Value, edmType));
    }

    private static async Task ReadOdataEntityAsync(
      TableResult result,
      TableOperation operation,
      Stream responseStream,
      OperationContext ctx,
      string accountName,
      TableRequestOptions options,
      CancellationToken cancellationToken)
    {
      KeyValuePair<string, Dictionary<string, object>> keyValuePair = await TableOperationHttpResponseParsers.ReadSingleItemResponseUsingJsonParserMetadataAsync(responseStream, cancellationToken).ConfigureAwait(false);
      if (operation.OperationType == TableOperationType.Retrieve)
      {
        result.Result = TableOperationHttpResponseParsers.ReadAndResolve<object>(keyValuePair.Key, keyValuePair.Value, operation.RetrieveResolver, options);
        result.Etag = keyValuePair.Key;
      }
      else
        result.Etag = TableOperationHttpResponseParsers.ReadAndUpdateTableEntity(operation.Entity, keyValuePair.Key, keyValuePair.Value, EntityReadFlags.Timestamp | EntityReadFlags.Etag, ctx);
    }

    public static async Task<KeyValuePair<string, Dictionary<string, object>>> ReadSingleItemResponseUsingJsonParserMetadataAsync(
      Stream responseStream,
      CancellationToken cancellationToken)
    {
      KeyValuePair<string, Dictionary<string, object>> keyValuePair;
      using (JsonReader reader = (JsonReader) new JsonTextReader((TextReader) new StreamReader(responseStream)))
      {
        reader.DateParseHandling = DateParseHandling.None;
        string etag;
        Dictionary<string, object> properties = TableOperationHttpResponseParsers.ReadSingleItem((JToken) await JObject.LoadAsync(reader, cancellationToken).ConfigureAwait(false), out etag);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The JSON reader has not yet reached the completed state."));
        keyValuePair = new KeyValuePair<string, Dictionary<string, object>>(etag, properties);
      }
      return keyValuePair;
    }

    private static T ReadAndResolve<T>(
      string etag,
      Dictionary<string, object> props,
      Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, T> resolver,
      TableRequestOptions options)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      DateTimeOffset dateTimeOffset = new DateTimeOffset();
      Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
      foreach (KeyValuePair<string, object> prop in props)
      {
        string key = prop.Key;
        switch (key)
        {
          case "PartitionKey":
            str1 = (string) prop.Value;
            continue;
          case "RowKey":
            str2 = (string) prop.Value;
            continue;
          case "Timestamp":
            dateTimeOffset = new DateTimeOffset((DateTime) prop.Value);
            continue;
          default:
            dictionary.Add(key, EntityProperty.CreateEntityPropertyFromObject(prop.Value));
            continue;
        }
      }
      return resolver(str1, str2, dateTimeOffset, (IDictionary<string, EntityProperty>) dictionary, etag);
    }

    internal static string ReadAndUpdateTableEntity(
      ITableEntity entity,
      string etag,
      Dictionary<string, object> props,
      EntityReadFlags flags,
      OperationContext ctx)
    {
      if ((flags & EntityReadFlags.Etag) > (EntityReadFlags) 0)
        entity.ETag = etag;
      Dictionary<string, EntityProperty> properties = (flags & EntityReadFlags.Properties) > (EntityReadFlags) 0 ? new Dictionary<string, EntityProperty>() : (Dictionary<string, EntityProperty>) null;
      if (flags > (EntityReadFlags) 0)
      {
        foreach (KeyValuePair<string, object> prop in props)
        {
          if (prop.Key == "PartitionKey")
          {
            if ((flags & EntityReadFlags.PartitionKey) != (EntityReadFlags) 0)
              entity.PartitionKey = (string) prop.Value;
          }
          else if (prop.Key == "RowKey")
          {
            if ((flags & EntityReadFlags.RowKey) != (EntityReadFlags) 0)
              entity.RowKey = (string) prop.Value;
          }
          else if (prop.Key == "Timestamp")
          {
            if ((flags & EntityReadFlags.Timestamp) != (EntityReadFlags) 0)
              entity.Timestamp = (DateTimeOffset) (DateTime) prop.Value;
          }
          else if ((flags & EntityReadFlags.Properties) > (EntityReadFlags) 0)
            properties.Add(prop.Key, EntityProperty.CreateEntityPropertyFromObject(prop.Value));
        }
        if ((flags & EntityReadFlags.Properties) > (EntityReadFlags) 0)
          entity.ReadEntity((IDictionary<string, EntityProperty>) properties, ctx);
      }
      return etag;
    }

    internal static void ReadAndUpdateTableEntityWithEdmTypeResolver(
      ITableEntity entity,
      Dictionary<string, string> entityAttributes,
      EntityReadFlags flags,
      Func<string, string, string, string, EdmType> propertyResolver,
      OperationContext ctx)
    {
      Dictionary<string, EntityProperty> properties = (flags & EntityReadFlags.Properties) > (EntityReadFlags) 0 ? new Dictionary<string, EntityProperty>() : (Dictionary<string, EntityProperty>) null;
      Dictionary<string, EdmType> dictionary = (Dictionary<string, EdmType>) null;
      if (entity.GetType() != typeof (DynamicTableEntity))
      {
        if (!TableEntity.DisablePropertyResolverCache)
        {
          dictionary = TableEntity.PropertyResolverCache.GetOrAdd(entity.GetType(), new Func<Type, Dictionary<string, EdmType>>(TableOperationHttpResponseParsers.CreatePropertyResolverDictionary));
        }
        else
        {
          Logger.LogVerbose(ctx, "Property resolver cache is disabled.");
          dictionary = TableOperationHttpResponseParsers.CreatePropertyResolverDictionary(entity.GetType());
        }
      }
      if (flags <= (EntityReadFlags) 0)
        return;
      foreach (KeyValuePair<string, string> entityAttribute in entityAttributes)
      {
        if (entityAttribute.Key == "PartitionKey")
          entity.PartitionKey = entityAttribute.Value;
        else if (entityAttribute.Key == "RowKey")
          entity.RowKey = entityAttribute.Value;
        else if (entityAttribute.Key == "Timestamp")
        {
          if ((flags & EntityReadFlags.Timestamp) != (EntityReadFlags) 0)
            entity.Timestamp = (DateTimeOffset) DateTime.Parse(entityAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else if ((flags & EntityReadFlags.Properties) > (EntityReadFlags) 0)
        {
          if (propertyResolver != null)
          {
            Logger.LogVerbose(ctx, "Using the property resolver provided via TableRequestOptions to deserialize the entity.");
            try
            {
              EdmType edmType = propertyResolver(entity.PartitionKey, entity.RowKey, entityAttribute.Key, entityAttribute.Value);
              Logger.LogVerbose(ctx, "Attempting to deserialize '{0}' as type '{1}'", (object) entityAttribute.Key, (object) edmType.GetType().ToString());
              try
              {
                properties.Add(entityAttribute.Key, EntityProperty.CreateEntityPropertyFromObject((object) entityAttribute.Value, edmType.GetType()));
              }
              catch (FormatException ex)
              {
                throw new StorageException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to parse property '{0}' with value '{1}' as type '{2}'", (object) entityAttribute.Key, (object) entityAttribute.Value, (object) edmType.ToString()), (Exception) ex)
                {
                  IsRetryable = false
                };
              }
            }
            catch (StorageException ex)
            {
              throw;
            }
            catch (Exception ex)
            {
              throw new StorageException("The custom property resolver delegate threw an exception. Check the inner exception for more details.", ex)
              {
                IsRetryable = false
              };
            }
          }
          else if (entity.GetType() != typeof (DynamicTableEntity))
          {
            Logger.LogVerbose(ctx, "Using the default property resolver to deserialize the entity.");
            if (dictionary != null)
            {
              EdmType type;
              dictionary.TryGetValue(entityAttribute.Key, out type);
              Logger.LogVerbose(ctx, "Attempting to deserialize '{0}' as type '{1}'", (object) entityAttribute.Key, (object) type);
              properties.Add(entityAttribute.Key, EntityProperty.CreateEntityPropertyFromObject((object) entityAttribute.Value, type));
            }
          }
          else
          {
            Logger.LogVerbose(ctx, "No property resolver available. Deserializing the entity properties as strings.");
            properties.Add(entityAttribute.Key, EntityProperty.CreateEntityPropertyFromObject((object) entityAttribute.Value, typeof (string)));
          }
        }
      }
      if ((flags & EntityReadFlags.Properties) <= (EntityReadFlags) 0)
        return;
      entity.ReadEntity((IDictionary<string, EntityProperty>) properties, ctx);
    }

    internal static string ExtractEntityIndexFromExtendedErrorInformation(RequestResult result)
    {
      if (result != null && result.ExtendedErrorInformation != null && !string.IsNullOrEmpty(result.ExtendedErrorInformation.ErrorMessage))
      {
        int length = result.ExtendedErrorInformation.ErrorMessage.IndexOf(":");
        if (length > 0 && length < 3)
          return result.ExtendedErrorInformation.ErrorMessage.Substring(0, length);
      }
      return (string) null;
    }

    internal static Task<ServiceProperties> ReadServicePropertiesAsync(Stream inputStream)
    {
      using (XmlReader reader = XmlReader.Create(inputStream))
        return ServiceProperties.FromServiceXmlAsync(XDocument.Load(reader));
    }

    internal static Task<ServiceStats> ReadServiceStatsAsync(Stream inputStream)
    {
      using (XmlReader reader = XmlReader.Create(inputStream))
        return ServiceStats.FromServiceXmlAsync(XDocument.Load(reader));
    }

    internal static Task<TablePermissions> ParseGetAclAsync(
      RESTCommand<TablePermissions> cmd,
      HttpResponseMessage resp,
      OperationContext ctx)
    {
      TablePermissions result = new TablePermissions();
      CommonUtility.AssertNotNull("permissions", (object) result);
      SharedAccessTablePolicies sharedAccessPolicies = result.SharedAccessPolicies;
      foreach (KeyValuePair<string, SharedAccessTablePolicy> accessIdentifier in new TableAccessPolicyResponse(cmd.ResponseStream).AccessIdentifiers)
        sharedAccessPolicies.Add(accessIdentifier.Key, accessIdentifier.Value);
      return Task.FromResult<TablePermissions>(result);
    }

    internal static DateTime ToUTCTime(this string str) => DateTime.Parse(str, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
  }
}
