// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceResponse
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Documents
{
  internal sealed class DocumentServiceResponse : IDisposable
  {
    private readonly JsonSerializerSettings serializerSettings;
    private bool isDisposed;

    internal DocumentServiceResponse(
      Stream body,
      INameValueCollection headers,
      HttpStatusCode statusCode,
      JsonSerializerSettings serializerSettings = null)
    {
      this.ResponseBody = body;
      this.Headers = headers;
      this.StatusCode = statusCode;
      this.serializerSettings = serializerSettings;
      this.SubStatusCode = this.GetSubStatusCodes();
    }

    internal DocumentServiceResponse(
      Stream body,
      INameValueCollection headers,
      HttpStatusCode statusCode,
      IClientSideRequestStatistics clientSideRequestStatistics,
      JsonSerializerSettings serializerSettings = null)
    {
      this.ResponseBody = body;
      this.Headers = headers;
      this.StatusCode = statusCode;
      this.RequestStats = clientSideRequestStatistics;
      this.serializerSettings = serializerSettings;
      this.SubStatusCode = this.GetSubStatusCodes();
    }

    public IClientSideRequestStatistics RequestStats { get; private set; }

    public string ResourceId { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public string StatusDescription { get; set; }

    internal INameValueCollection Headers { get; set; }

    internal static Func<Stream, JsonReader> JsonReaderFactory { get; set; }

    public NameValueCollection ResponseHeaders => this.Headers.ToNameValueCollection();

    public Stream ResponseBody { get; set; }

    public SubStatusCodes SubStatusCode { get; private set; }

    public TResource GetResource<TResource>(ITypeResolver<TResource> typeResolver = null) where TResource : Resource, new()
    {
      if (this.ResponseBody == null || this.ResponseBody.CanSeek && this.ResponseBody.Length == 0L)
        return default (TResource);
      if (typeResolver == null)
        typeResolver = DocumentServiceResponse.GetTypeResolver<TResource>();
      if (!this.ResponseBody.CanSeek)
      {
        MemoryStream destination = new MemoryStream();
        this.ResponseBody.CopyTo((Stream) destination);
        this.ResponseBody.Dispose();
        this.ResponseBody = (Stream) destination;
        this.ResponseBody.Seek(0L, SeekOrigin.Begin);
      }
      TResource resource = JsonSerializable.LoadFrom<TResource>(this.ResponseBody, typeResolver, this.serializerSettings);
      resource.SerializerSettings = this.serializerSettings;
      this.ResponseBody.Seek(0L, SeekOrigin.Begin);
      if (PathsHelper.IsPublicResource(typeof (TResource)))
        resource.AltLink = PathsHelper.GeneratePathForNameBased(typeof (TResource), this.GetOwnerFullName(), resource.Id);
      else if (typeof (TResource).IsGenericType() && typeof (TResource).GetGenericTypeDefinition() == typeof (FeedResource<>))
        resource.AltLink = this.GetOwnerFullName();
      return resource;
    }

    public TResource GetInternalResource<TResource>(Func<TResource> constructor) where TResource : Resource => this.ResponseBody != null && (!this.ResponseBody.CanSeek || this.ResponseBody.Length > 0L) ? JsonSerializable.LoadFromWithConstructor<TResource>(this.ResponseBody, constructor, this.serializerSettings) : default (TResource);

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.ResponseBody != null)
      {
        this.ResponseBody.Dispose();
        this.ResponseBody = (Stream) null;
      }
      this.isDisposed = true;
    }

    public IEnumerable<object> GetQueryResponse(Type resourceType, out int itemCount) => this.GetQueryResponse<object>(resourceType, false, out itemCount);

    public IEnumerable<T> GetQueryResponse<T>(Type resourceType, bool lazy, out int itemCount)
    {
      if (!int.TryParse(this.Headers["x-ms-item-count"], out itemCount))
        itemCount = 0;
      IEnumerable<T> enumerable;
      if (typeof (T) == typeof (object))
      {
        string ownerName = (string) null;
        if (PathsHelper.IsPublicResource(resourceType))
          ownerName = this.GetOwnerFullName();
        enumerable = (IEnumerable<T>) this.GetEnumerable<object>(resourceType, (Func<JsonReader, object>) (jsonReader =>
        {
          JToken jObject = JToken.Load(jsonReader);
          return jObject.Type == JTokenType.Object || jObject.Type == JTokenType.Array ? (object) new QueryResult((JContainer) jObject, ownerName, this.serializerSettings) : (object) jObject;
        }));
      }
      else
      {
        JsonSerializer serializer = this.serializerSettings != null ? JsonSerializer.Create(this.serializerSettings) : JsonSerializer.Create();
        enumerable = this.GetEnumerable<T>(resourceType, (Func<JsonReader, T>) (jsonReader => serializer.Deserialize<T>(jsonReader)));
      }
      if (lazy)
        return enumerable;
      List<T> queryResponse = new List<T>(itemCount);
      queryResponse.AddRange(enumerable);
      return (IEnumerable<T>) queryResponse;
    }

    internal SubStatusCodes GetSubStatusCodes()
    {
      string header = this.Headers["x-ms-substatus"];
      uint num = 0;
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      ref uint local = ref num;
      return uint.TryParse(header, NumberStyles.Integer, (IFormatProvider) invariantCulture, out local) ? (SubStatusCodes) num : SubStatusCodes.Unknown;
    }

    private IEnumerable<T> GetEnumerable<T>(Type resourceType, Func<JsonReader, T> callback)
    {
      if (this.ResponseBody != null)
      {
        using (JsonReader jsonReader = DocumentServiceResponse.Create(this.ResponseBody))
        {
          Helpers.SetupJsonReader(jsonReader, this.serializerSettings);
          string b = resourceType.Name + "s";
          string empty = string.Empty;
          while (jsonReader.Read())
          {
            if (jsonReader.TokenType == JsonToken.PropertyName)
              empty = jsonReader.Value.ToString();
            if (jsonReader.Depth == 1 && jsonReader.TokenType == JsonToken.StartArray && string.Equals(empty, b, StringComparison.Ordinal))
            {
              while (jsonReader.Read() && jsonReader.Depth == 2)
                yield return callback(jsonReader);
              break;
            }
          }
        }
      }
    }

    private static JsonReader Create(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      return DocumentServiceResponse.JsonReaderFactory != null ? DocumentServiceResponse.JsonReaderFactory(stream) : (JsonReader) new JsonTextReader((TextReader) new StreamReader(stream));
    }

    private static ITypeResolver<TResource> GetTypeResolver<TResource>() where TResource : Resource, new()
    {
      ITypeResolver<TResource> typeResolver = (ITypeResolver<TResource>) null;
      if (typeof (TResource) == typeof (Offer))
        typeResolver = (ITypeResolver<TResource>) OfferTypeResolver.ResponseOfferTypeResolver;
      return typeResolver;
    }

    private string GetOwnerFullName() => this.Headers["x-ms-alt-content-path"];
  }
}
