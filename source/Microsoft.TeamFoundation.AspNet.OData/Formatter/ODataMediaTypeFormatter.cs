// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataMediaTypeFormatter
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Formatter
{
  public class ODataMediaTypeFormatter : MediaTypeFormatter
  {
    private readonly IEnumerable<ODataPayloadKind> _payloadKinds;

    public ODataMediaTypeFormatter(IEnumerable<ODataPayloadKind> payloadKinds) => this._payloadKinds = payloadKinds != null ? payloadKinds : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (payloadKinds));

    internal ODataMediaTypeFormatter(ODataMediaTypeFormatter formatter, HttpRequestMessage request)
      : base((MediaTypeFormatter) formatter)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      this._payloadKinds = formatter._payloadKinds;
      this.Request = request;
      this.BaseAddressFactory = formatter.BaseAddressFactory;
    }

    public Func<HttpRequestMessage, Uri> BaseAddressFactory { get; set; }

    public HttpRequestMessage Request { get; set; }

    public override MediaTypeFormatter GetPerRequestFormatterInstance(
      Type type,
      HttpRequestMessage request,
      MediaTypeHeaderValue mediaType)
    {
      base.GetPerRequestFormatterInstance(type, request, mediaType);
      return this.Request != null && this.Request == request ? (MediaTypeFormatter) this : (MediaTypeFormatter) new ODataMediaTypeFormatter(this, request);
    }

    public override void SetDefaultContentHeaders(
      Type type,
      HttpContentHeaders headers,
      MediaTypeHeaderValue mediaType)
    {
      MediaTypeHeaderValue newMediaType = (MediaTypeHeaderValue) null;
      if (ODataOutputFormatterHelper.TryGetContentHeader(type, mediaType, out newMediaType))
        headers.ContentType = newMediaType;
      else
        base.SetDefaultContentHeaders(type, headers, mediaType);
      IEnumerable<string> acceptCharsetValues = this.Request.Headers.AcceptCharset.Select<StringWithQualityHeaderValue, string>((Func<StringWithQualityHeaderValue, string>) (cs => cs.Value));
      string charSet = string.Empty;
      if (ODataOutputFormatterHelper.TryGetCharSet(headers.ContentType, acceptCharsetValues, out charSet))
        headers.ContentType.CharSet = charSet;
      headers.TryAddWithoutValidation("OData-Version", ODataUtils.ODataVersionToString(ResultHelpers.GetODataResponseVersion(this.Request)));
      foreach (KeyValuePair<string, string> header in ODataOutputFormatterHelper.PrepareResponseMessage((IWebApiRequestMessage) new WebApiRequestMessage(this.Request), (IWebApiHeaders) new WebApiRequestHeaders(this.Request.Headers), (Func<IServiceProvider, ODataMessageWrapper>) (services => ODataMessageWrapperHelper.Create((Stream) null, headers, services))).Headers)
      {
        if (!headers.Contains(header.Key))
          headers.TryAddWithoutValidation(header.Key, header.Value);
      }
    }

    public override bool CanReadType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (this.Request == null)
        return false;
      ODataDeserializerProvider deserializerProvider = ServiceProviderServiceExtensions.GetRequiredService<ODataDeserializerProvider>(this.Request.GetRequestContainer());
      return ODataInputFormatterHelper.CanReadType(type, this.Request.GetModel(), this.Request.ODataProperties().Path, this._payloadKinds, (Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer>) (objectType => (Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer) deserializerProvider.GetEdmTypeDeserializer(objectType)), (Func<Type, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer>) (objectType => deserializerProvider.GetODataDeserializer(objectType, this.Request)));
    }

    public override bool CanWriteType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (this.Request == null)
        return false;
      ODataSerializerProvider serializerProvider = ServiceProviderServiceExtensions.GetRequiredService<ODataSerializerProvider>(this.Request.GetRequestContainer());
      bool isGenericSingleResult = false;
      if (type.IsGenericType)
      {
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        Type baseType = TypeHelper.GetBaseType(type);
        Type type1 = typeof (SingleResult<>);
        isGenericSingleResult = genericTypeDefinition == type1 || baseType == typeof (SingleResult);
      }
      return ODataOutputFormatterHelper.CanWriteType(type, this._payloadKinds, isGenericSingleResult, (IWebApiRequestMessage) new WebApiRequestMessage(this.Request), (Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer>) (objectType => serializerProvider.GetODataPayloadSerializer(objectType, this.Request)));
    }

    public override Task<object> ReadFromStreamAsync(
      Type type,
      Stream readStream,
      HttpContent content,
      IFormatterLogger formatterLogger)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (readStream == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readStream));
      if (this.Request == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReadFromStreamAsyncMustHaveRequest);
      object defaultValueForType = MediaTypeFormatter.GetDefaultValueForType(type);
      HttpContentHeaders contentHeaders = content == null ? (HttpContentHeaders) null : content.Headers;
      if (contentHeaders != null)
      {
        long? contentLength = contentHeaders.ContentLength;
        long num = 0;
        if (!(contentLength.GetValueOrDefault() == num & contentLength.HasValue))
        {
          try
          {
            Func<ODataDeserializerContext> getODataDeserializerContext = (Func<ODataDeserializerContext>) (() => new ODataDeserializerContext()
            {
              Request = this.Request
            });
            Action<Exception> logErrorAction = (Action<Exception>) (ex =>
            {
              if (formatterLogger == null)
                throw ex;
              formatterLogger.LogError(string.Empty, ex);
            });
            ODataDeserializerProvider deserializerProvider = ServiceProviderServiceExtensions.GetRequiredService<ODataDeserializerProvider>(this.Request.GetRequestContainer());
            return Task.FromResult<object>(ODataInputFormatterHelper.ReadFromStream(type, defaultValueForType, this.Request.GetModel(), this.GetBaseAddressInternal(this.Request), (IWebApiRequestMessage) new WebApiRequestMessage(this.Request), (Func<IODataRequestMessage>) (() => (IODataRequestMessage) ODataMessageWrapperHelper.Create(readStream, contentHeaders, this.Request.GetODataContentIdMapping(), this.Request.GetRequestContainer())), (Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer>) (objectType => (Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer) deserializerProvider.GetEdmTypeDeserializer(objectType)), (Func<Type, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer>) (objectType => deserializerProvider.GetODataDeserializer(objectType, this.Request)), getODataDeserializerContext, (Action<IDisposable>) (disposable => this.Request.RegisterForDispose(disposable)), logErrorAction));
          }
          catch (Exception ex)
          {
            return Microsoft.AspNet.OData.Common.TaskHelpers.FromError<object>(ex);
          }
        }
      }
      return Task.FromResult<object>(defaultValueForType);
    }

    public override Task WriteToStreamAsync(
      Type type,
      object value,
      Stream writeStream,
      HttpContent content,
      TransportContext transportContext,
      CancellationToken cancellationToken)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (writeStream == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeStream));
      if (this.Request == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.WriteToStreamAsyncMustHaveRequest);
      if (cancellationToken.IsCancellationRequested)
        return Microsoft.AspNet.OData.Common.TaskHelpers.Canceled();
      try
      {
        if (this.Request.GetConfiguration() == null)
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustContainConfiguration);
        HttpContentHeaders contentHeaders = content == null ? (HttpContentHeaders) null : content.Headers;
        UrlHelper urlHelper = this.Request.GetUrlHelper() ?? new UrlHelper(this.Request);
        Func<ODataSerializerContext> getODataSerializerContext = (Func<ODataSerializerContext>) (() => new ODataSerializerContext()
        {
          Request = this.Request,
          Url = urlHelper
        });
        ODataSerializerProvider serializerProvider = ServiceProviderServiceExtensions.GetRequiredService<ODataSerializerProvider>(this.Request.GetRequestContainer());
        ODataOutputFormatterHelper.WriteToStream(type, value, this.Request.GetModel(), ResultHelpers.GetODataResponseVersion(this.Request), this.GetBaseAddressInternal(this.Request), contentHeaders == null ? (MediaTypeHeaderValue) null : contentHeaders.ContentType, (IWebApiUrlHelper) new WebApiUrlHelper(urlHelper), (IWebApiRequestMessage) new WebApiRequestMessage(this.Request), (IWebApiHeaders) new WebApiRequestHeaders(this.Request.Headers), (Func<IServiceProvider, ODataMessageWrapper>) (services => ODataMessageWrapperHelper.Create(writeStream, contentHeaders, services)), (Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer>) (edmType => (Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer) serializerProvider.GetEdmTypeSerializer(edmType)), (Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer>) (objectType => serializerProvider.GetODataPayloadSerializer(objectType, this.Request)), getODataSerializerContext);
        return Microsoft.AspNet.OData.Common.TaskHelpers.Completed();
      }
      catch (Exception ex)
      {
        return Microsoft.AspNet.OData.Common.TaskHelpers.FromError(ex);
      }
    }

    private Uri GetBaseAddressInternal(HttpRequestMessage request) => this.BaseAddressFactory != null ? this.BaseAddressFactory(request) : ODataMediaTypeFormatter.GetDefaultBaseAddress(request);

    public static Uri GetDefaultBaseAddress(HttpRequestMessage request)
    {
      string uriString = request != null ? (request.GetUrlHelper() ?? new UrlHelper(request)).CreateODataLink() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      if (uriString == null)
        throw new SerializationException(SRResources.UnableToDetermineBaseUrl);
      return uriString[uriString.Length - 1] == '/' ? new Uri(uriString) : new Uri(uriString + "/");
    }
  }
}
