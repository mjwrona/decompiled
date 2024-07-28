// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiRequestMessage
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiRequestMessage : IWebApiRequestMessage
  {
    private HttpRequestMessage innerRequest;

    public WebApiRequestMessage(HttpRequestMessage request)
    {
      this.innerRequest = request != null ? request : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      this.Headers = (IWebApiHeaders) new WebApiRequestHeaders(request.Headers);
      HttpRequestMessageProperties context = request.ODataProperties();
      if (context != null)
        this.Context = (IWebApiContext) new WebApiContext(context);
      HttpConfiguration configuration = request.GetConfiguration();
      if (configuration == null)
        return;
      this.Options = (IWebApiOptions) new WebApiOptions(configuration);
    }

    public IWebApiContext Context { get; private set; }

    public IWebApiHeaders Headers { get; private set; }

    public bool IsCountRequest() => ODataCountMediaTypeMapping.IsCountRequest(this.innerRequest.ODataProperties().Path);

    public ODataRequestMethod Method
    {
      get
      {
        bool ignoreCase = true;
        ODataRequestMethod result = ODataRequestMethod.Unknown;
        return Enum.TryParse<ODataRequestMethod>(this.innerRequest.Method.ToString(), ignoreCase, out result) ? result : ODataRequestMethod.Unknown;
      }
    }

    public IWebApiOptions Options { get; private set; }

    public IServiceProvider RequestContainer => this.innerRequest.GetRequestContainer();

    public Uri RequestUri => this.innerRequest.RequestUri;

    public ODataDeserializerProvider DeserializerProvider => this.innerRequest.GetDeserializerProvider();

    public Uri GetNextPageLink(
      int pageSize,
      object instance = null,
      Func<object, string> objToSkipTokenValue = null)
    {
      return this.innerRequest.GetNextPageLink(pageSize, instance, objToSkipTokenValue);
    }

    public string CreateETag(IDictionary<string, object> properties)
    {
      HttpConfiguration configuration = this.innerRequest.GetConfiguration();
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustContainConfiguration);
      return configuration.GetETagHandler().CreateETag(properties)?.ToString();
    }

    public ETag GetETag(EntityTagHeaderValue etagHeaderValue) => this.innerRequest.GetETag(etagHeaderValue);

    public ETag GetETag<TEntity>(EntityTagHeaderValue etagHeaderValue) => (ETag) this.innerRequest.GetETag<TEntity>(etagHeaderValue);

    public IDictionary<string, string> ODataContentIdMapping => this.innerRequest.GetODataContentIdMapping();

    public IODataPathHandler PathHandler => this.innerRequest.GetPathHandler();

    public IDictionary<string, string> QueryParameters
    {
      get
      {
        IDictionary<string, string> queryParameters = (IDictionary<string, string>) new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> queryNameValuePair in this.innerRequest.GetQueryNameValuePairs())
        {
          if (!queryParameters.ContainsKey(queryNameValuePair.Key))
            queryParameters.Add(queryNameValuePair.Key, queryNameValuePair.Value);
        }
        return queryParameters;
      }
    }

    public ODataMessageReaderSettings ReaderSettings => this.innerRequest.GetReaderSettings();

    public ODataMessageWriterSettings WriterSettings => this.innerRequest.GetWriterSettings();
  }
}
