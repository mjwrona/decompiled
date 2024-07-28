// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchHttpRequestMessageExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Results;
using Microsoft.AspNet.OData.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Batch
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ODataBatchHttpRequestMessageExtensions
  {
    private const string BatchIdKey = "BatchId";
    private const string ChangeSetIdKey = "ChangesetId";
    private const string ContentIdKey = "ContentId";
    private const string ContentIdMappingKey = "ContentIdMapping";
    private const string BatchMediaTypeMime = "multipart/mixed";
    private const string BatchMediaTypeJson = "application/json";
    private const string Boundary = "boundary";

    public static Guid? GetODataBatchId(this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      return request.Properties.TryGetValue("BatchId", out obj) ? new Guid?((Guid) obj) : new Guid?();
    }

    public static void SetODataBatchId(this HttpRequestMessage request, Guid batchId)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      request.Properties["BatchId"] = (object) batchId;
    }

    public static Guid? GetODataChangeSetId(this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      return request.Properties.TryGetValue("ChangesetId", out obj) ? new Guid?((Guid) obj) : new Guid?();
    }

    public static void SetODataChangeSetId(this HttpRequestMessage request, Guid changeSetId)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      request.Properties["ChangesetId"] = (object) changeSetId;
    }

    public static string GetODataContentId(this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      return request.Properties.TryGetValue("ContentId", out obj) ? (string) obj : (string) null;
    }

    public static void SetODataContentId(this HttpRequestMessage request, string contentId)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      request.Properties["ContentId"] = (object) contentId;
    }

    public static IDictionary<string, string> GetODataContentIdMapping(
      this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      return request.Properties.TryGetValue("ContentIdMapping", out obj) ? obj as IDictionary<string, string> : (IDictionary<string, string>) null;
    }

    public static void SetODataContentIdMapping(
      this HttpRequestMessage request,
      IDictionary<string, string> contentIdMapping)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      request.Properties["ContentIdMapping"] = (object) contentIdMapping;
    }

    internal static Task<HttpResponseMessage> CreateODataBatchResponseAsync(
      this HttpRequestMessage request,
      IEnumerable<ODataBatchResponseItem> responses,
      ODataMessageQuotas messageQuotas)
    {
      ODataVersion odataResponseVersion = ResultHelpers.GetODataResponseVersion(request);
      IServiceProvider requestContainer = request.GetRequestContainer();
      ODataMessageWriterSettings requiredService = ServiceProviderServiceExtensions.GetRequiredService<ODataMessageWriterSettings>(requestContainer);
      requiredService.Version = new ODataVersion?(odataResponseVersion);
      requiredService.MessageQuotas = messageQuotas;
      MediaTypeHeaderValue contentType = (MediaTypeHeaderValue) null;
      if (request.Headers.Accept.Any<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (t => t.MediaType.Equals("multipart/mixed", StringComparison.OrdinalIgnoreCase))))
        contentType = MediaTypeHeaderValue.Parse(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "multipart/mixed;boundary=batchresponse_{0}", new object[1]
        {
          (object) Guid.NewGuid()
        }));
      else if (request.Headers.Accept.Any<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (t => t.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))))
        contentType = MediaTypeHeaderValue.Parse("application/json");
      HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new ODataBatchContent(responses, requestContainer, contentType);
      return Task.FromResult<HttpResponseMessage>(response);
    }

    internal static void ValidateODataBatchRequest(this HttpRequestMessage request)
    {
      if (request.Content == null)
        throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, SRResources.BatchRequestMissingContent));
      MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
      bool flag1 = contentType != null ? string.Equals(contentType.MediaType, "multipart/mixed", StringComparison.OrdinalIgnoreCase) : throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, SRResources.BatchRequestMissingContentType));
      bool flag2 = string.Equals(contentType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase);
      if (!flag1 && !flag2)
        throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, Microsoft.AspNet.OData.Common.Error.Format(SRResources.BatchRequestInvalidMediaType, (object) "multipart/mixed", (object) "application/json")));
      if (!flag1)
        return;
      NameValueHeaderValue valueHeaderValue = contentType.Parameters.FirstOrDefault<NameValueHeaderValue>((Func<NameValueHeaderValue, bool>) (p => string.Equals(p.Name, "boundary", StringComparison.OrdinalIgnoreCase)));
      if (valueHeaderValue == null || string.IsNullOrEmpty(valueHeaderValue.Value))
        throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, SRResources.BatchRequestMissingBoundary));
    }

    internal static Uri GetODataBatchBaseUri(this HttpRequestMessage request, string oDataRouteName)
    {
      if (oDataRouteName == null)
        return new Uri(request.RequestUri, new Uri("/", UriKind.Relative));
      UrlHelper urlHelper = request.GetUrlHelper() ?? new UrlHelper(request);
      string routeName = oDataRouteName;
      HttpRouteValueDictionary routeValues = new HttpRouteValueDictionary();
      routeValues.Add(ODataRouteConstants.ODataPath, (object) string.Empty);
      return new Uri(urlHelper.Link(routeName, (IDictionary<string, object>) routeValues) ?? throw new InvalidOperationException(SRResources.UnableToDetermineBaseUrl));
    }
  }
}
