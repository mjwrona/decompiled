// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsApiControllerExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TfsApiControllerExtensions
  {
    private const string c_gzipHeader = "gzip";
    private const string c_area = "WebApi";
    private const string c_layer = "TfsApiControllerExtensions";

    public static HttpResponseMessage GenerateResponse<T>(
      this TfsApiController tfsApiController,
      IEnumerable<T> collection,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return tfsApiController.GenerateResponse<T>(collection, (ISecuredObject) null, cancellationToken);
    }

    public static HttpResponseMessage GenerateResponse<T>(
      this TfsApiController tfsApiController,
      IEnumerable<T> collection,
      ISecuredObject secureObject,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return tfsApiController.GenerateResponse<T>(collection, secureObject, false, cancellationToken);
    }

    public static HttpResponseMessage GenerateResponse<T>(
      this TfsApiController tfsApiController,
      IEnumerable<T> collection,
      ISecuredObject secureObject,
      bool useAsync,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpResponseMessage response = tfsApiController.Request.CreateResponse(HttpStatusCode.OK);
      bool sendGzip = tfsApiController.Request.Headers.AcceptEncoding.Any<StringWithQualityHeaderValue>((Func<StringWithQualityHeaderValue, bool>) (x => "gzip".Equals(x.Value, StringComparison.OrdinalIgnoreCase)));
      Action<Exception> handleException = (Action<Exception>) (ex =>
      {
        IVssRequestContext tfsRequestContext = tfsApiController.TfsRequestContext;
        if (tfsRequestContext == null)
          return;
        if (tfsRequestContext.Status == null)
          tfsRequestContext.Status = ex;
        tfsRequestContext.TraceException(103300, "WebApi", nameof (TfsApiControllerExtensions), ex);
      });
      Action<Stream, HttpContent, TransportContext> action1 = (Action<Stream, HttpContent, TransportContext>) (async (stream, httpContent, transportContext) =>
      {
        try
        {
          using (SmartPushStreamContentStream smartStream = new SmartPushStreamContentStream(sendGzip ? (Stream) new GZipStream(stream, CompressionLevel.Fastest) : stream))
            await TfsApiControllerExtensions.SerializeToJsonAsync<T>(collection, (Stream) smartStream, httpContent.Headers, tfsApiController.Request, cancellationToken);
        }
        catch (Exception ex)
        {
          handleException(ex);
          throw;
        }
      });
      Action<Stream, HttpContent, TransportContext> action2 = (Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(sendGzip ? (Stream) new GZipStream(stream, CompressionLevel.Fastest) : stream))
            TfsApiControllerExtensions.SerializeToJson<T>(collection, (Stream) streamContentStream, httpContent.Headers, tfsApiController.Request);
        }
        catch (Exception ex)
        {
          handleException(ex);
          throw;
        }
      });
      HttpResponseMessage httpResponseMessage = response;
      Action<Stream, HttpContent, TransportContext> onStreamAvailable = useAsync ? action1 : action2;
      MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
      mediaType.CharSet = Encoding.UTF8.WebName;
      ISecuredObject securedObject = secureObject;
      PushStreamContent pushStreamContent = TfsApiControllerExtensions.CreatePushStreamContent(onStreamAvailable, mediaType, securedObject);
      httpResponseMessage.Content = (HttpContent) pushStreamContent;
      if (sendGzip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      return response;
    }

    internal static void SerializeToJson<T>(
      IEnumerable<T> collection,
      Stream stream,
      HttpContentHeaders httpContentHeaders,
      HttpRequestMessage requestMessage)
    {
      ServerVssJsonMediaTypeFormatter mediaTypeFormatter = new ServerVssJsonMediaTypeFormatter(requestMessage);
      ServerJsonSerializationHelper.EnsureValidRootType(typeof (T));
      bool flag = !mediaTypeFormatter.BypassSafeArrayWrapping;
      mediaTypeFormatter.BypassSafeArrayWrapping = true;
      Encoding effectiveEncoding = mediaTypeFormatter.SelectCharacterEncoding(httpContentHeaders);
      JsonSerializer jsonSerializer = mediaTypeFormatter.CreateJsonSerializer();
      using (JsonWriter jsonWriter = mediaTypeFormatter.CreateJsonWriter(typeof (T), stream, effectiveEncoding))
      {
        jsonWriter.CloseOutput = false;
        if (flag)
        {
          jsonWriter.WriteStartObject();
          jsonWriter.WritePropertyName("value");
        }
        jsonWriter.WriteStartArray();
        int num = 0;
        foreach (T obj in collection)
        {
          jsonSerializer.Serialize(jsonWriter, (object) obj);
          ++num;
        }
        jsonWriter.WriteEndArray();
        if (flag)
        {
          jsonWriter.WritePropertyName("count");
          jsonWriter.WriteValue(num);
          jsonWriter.WriteEndObject();
        }
        jsonWriter.Flush();
      }
    }

    internal static async Task SerializeToJsonAsync<T>(
      IEnumerable<T> collection,
      Stream stream,
      HttpContentHeaders httpContentHeaders,
      HttpRequestMessage requestMessage,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServerVssJsonMediaTypeFormatter mediaTypeFormatter = new ServerVssJsonMediaTypeFormatter(requestMessage);
      ServerJsonSerializationHelper.EnsureValidRootType(typeof (T));
      bool wrapArray = !mediaTypeFormatter.BypassSafeArrayWrapping;
      mediaTypeFormatter.BypassSafeArrayWrapping = true;
      Encoding effectiveEncoding = mediaTypeFormatter.SelectCharacterEncoding(httpContentHeaders);
      JsonSerializer serializer = mediaTypeFormatter.CreateJsonSerializer();
      using (JsonWriter jsonWriter = mediaTypeFormatter.CreateJsonWriter(typeof (T), stream, effectiveEncoding))
      {
        jsonWriter.CloseOutput = false;
        if (wrapArray)
        {
          await jsonWriter.WriteStartObjectAsync(cancellationToken);
          await jsonWriter.WritePropertyNameAsync("value", cancellationToken);
        }
        await jsonWriter.WriteStartArrayAsync(cancellationToken);
        int count = 0;
        foreach (T o in collection)
        {
          await JObject.FromObject((object) o, serializer).WriteToAsync(jsonWriter, cancellationToken);
          ++count;
        }
        await jsonWriter.WriteEndArrayAsync(cancellationToken);
        if (wrapArray)
        {
          await jsonWriter.WritePropertyNameAsync("count", cancellationToken);
          await jsonWriter.WriteValueAsync(count, cancellationToken);
          await jsonWriter.WriteEndObjectAsync(cancellationToken);
        }
        await jsonWriter.FlushAsync(cancellationToken);
      }
      serializer = (JsonSerializer) null;
    }

    private static PushStreamContent CreatePushStreamContent(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      MediaTypeHeaderValue mediaType,
      ISecuredObject securedObject)
    {
      return securedObject == null ? new PushStreamContent(onStreamAvailable, mediaType) : (PushStreamContent) new VssServerPushStreamContent(onStreamAvailable, mediaType, (object) securedObject);
    }
  }
}
