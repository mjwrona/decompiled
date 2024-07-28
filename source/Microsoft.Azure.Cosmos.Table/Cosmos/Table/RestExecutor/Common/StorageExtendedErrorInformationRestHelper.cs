// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.StorageExtendedErrorInformationRestHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal static class StorageExtendedErrorInformationRestHelper
  {
    public static async Task<StorageExtendedErrorInformation> ReadFromStreamAsync(
      Stream inputStream,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (inputStream), (object) inputStream);
      if (inputStream.CanSeek && inputStream.Length < 1L)
        return (StorageExtendedErrorInformation) null;
      try
      {
        using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(inputStream))
        {
          int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
          cancellationToken.ThrowIfCancellationRequested();
          StorageExtendedErrorInformation errorInformation = await StorageExtendedErrorInformationRestHelper.ReadXmlAsync(reader, cancellationToken).ConfigureAwait(false);
          cancellationToken.ThrowIfCancellationRequested();
          return errorInformation;
        }
      }
      catch (XmlException ex)
      {
        return (StorageExtendedErrorInformation) null;
      }
    }

    private static async Task<StorageExtendedErrorInformation> ReadXmlAsync(
      XmlReader reader,
      CancellationToken cancellationToken)
    {
      StorageExtendedErrorInformation extendedErrorInformation = new StorageExtendedErrorInformation();
      CommonUtility.AssertNotNull(nameof (reader), (object) reader);
      extendedErrorInformation.AdditionalDetails = (IDictionary<string, string>) new Dictionary<string, string>();
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      cancellationToken.ThrowIfCancellationRequested();
      StorageExtendedErrorInformation errorInformation;
      IDictionary<string, string> dictionary;
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "Code", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reader.LocalName, "code", StringComparison.Ordinal) == 0)
          {
            errorInformation = extendedErrorInformation;
            errorInformation.ErrorCode = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            errorInformation = (StorageExtendedErrorInformation) null;
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "Message", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reader.LocalName, "message", StringComparison.Ordinal) == 0)
          {
            errorInformation = extendedErrorInformation;
            errorInformation.ErrorMessage = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            errorInformation = (StorageExtendedErrorInformation) null;
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "exceptiondetails", StringComparison.OrdinalIgnoreCase) == 0)
          {
            await reader.ReadStartElementAsync((string) null, (string) null).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            while (true)
            {
              if (await reader.IsStartElementAsync().ConfigureAwait(false))
              {
                switch (reader.LocalName)
                {
                  case "ExceptionMessage":
                    dictionary = extendedErrorInformation.AdditionalDetails;
                    dictionary.Add("ExceptionMessage", await reader.ReadElementContentAsStringAsync("ExceptionMessage", string.Empty).ConfigureAwait(false));
                    dictionary = (IDictionary<string, string>) null;
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                  case "StackTrace":
                    dictionary = extendedErrorInformation.AdditionalDetails;
                    dictionary.Add("StackTrace", await reader.ReadElementContentAsStringAsync("StackTrace", string.Empty).ConfigureAwait(false));
                    dictionary = (IDictionary<string, string>) null;
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                  default:
                    await reader.SkipAsync().ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                }
              }
              else
                break;
            }
            await reader.ReadEndElementAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else
          {
            dictionary = extendedErrorInformation.AdditionalDetails;
            string key = reader.LocalName;
            dictionary.Add(key, await reader.ReadInnerXmlAsync().ConfigureAwait(false));
            dictionary = (IDictionary<string, string>) null;
            key = (string) null;
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      cancellationToken.ThrowIfCancellationRequested();
      return extendedErrorInformation;
    }

    public static Task<StorageExtendedErrorInformation> ReadExtendedErrorInfoFromStreamAsync(
      Stream inputStream,
      HttpResponseMessage response,
      string contentType,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (inputStream), (object) inputStream);
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      if (inputStream.CanSeek && inputStream.Length <= 0L)
        return Task.FromResult<StorageExtendedErrorInformation>((StorageExtendedErrorInformation) null);
      return response.Content.Headers.ContentType.MediaType.Contains("xml") ? StorageExtendedErrorInformationRestHelper.ReadFromStreamAsync(inputStream, token) : StorageExtendedErrorInformationRestHelper.ReadAndParseJsonExtendedErrorAsync((Stream) new NonCloseableStream(inputStream), token);
    }

    internal static async Task<StorageExtendedErrorInformation> ReadAndParseJsonExtendedErrorAsync(
      Stream responseStream,
      CancellationToken cancellationToken)
    {
      try
      {
        using (JsonReader reader = (JsonReader) new JsonTextReader((TextReader) new StreamReader(responseStream)))
        {
          reader.DateParseHandling = DateParseHandling.None;
          Dictionary<string, object> dictionary1 = (await JObject.LoadAsync(reader, cancellationToken).ConfigureAwait(false)).ToObject<Dictionary<string, object>>();
          StorageExtendedErrorInformation extendedErrorAsync = new StorageExtendedErrorInformation();
          extendedErrorAsync.AdditionalDetails = (IDictionary<string, string>) new Dictionary<string, string>();
          if (dictionary1.ContainsKey("odata.error"))
          {
            Dictionary<string, object> dictionary2 = ((JToken) dictionary1["odata.error"]).ToObject<Dictionary<string, object>>();
            if (dictionary2.ContainsKey("code"))
              extendedErrorAsync.ErrorCode = (string) dictionary2["code"];
            if (dictionary2.ContainsKey("message"))
            {
              Dictionary<string, object> dictionary3 = ((JToken) dictionary2["message"]).ToObject<Dictionary<string, object>>();
              if (dictionary3.ContainsKey("value"))
                extendedErrorAsync.ErrorMessage = (string) dictionary3["value"];
            }
            if (dictionary2.ContainsKey("innererror"))
            {
              Dictionary<string, object> dictionary4 = ((JToken) dictionary2["innererror"]).ToObject<Dictionary<string, object>>();
              if (dictionary4.ContainsKey("message"))
                extendedErrorAsync.AdditionalDetails["ExceptionMessage"] = (string) dictionary4["message"];
              if (dictionary4.ContainsKey("type"))
                extendedErrorAsync.AdditionalDetails["exceptiondetails"] = (string) dictionary4["type"];
              if (dictionary4.ContainsKey("stacktrace"))
                extendedErrorAsync.AdditionalDetails["StackTrace"] = (string) dictionary4["stacktrace"];
            }
          }
          return extendedErrorAsync;
        }
      }
      catch (Exception ex)
      {
        return (StorageExtendedErrorInformation) null;
      }
    }
  }
}
