// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.WorkItemTrackingHttpClient
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  [ResourceArea("5264459E-E5E0-4BD8-B118-0985E68A4EC5")]
  public class WorkItemTrackingHttpClient : WorkItemTrackingHttpClientBase
  {
    private HttpStatusCode? lastResponseStatusCode;

    public WorkItemTrackingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public HttpStatusCode? LastResponseStatusCode => this.lastResponseStatusCode;

    public async Task<AttachmentReference> CreateAttachmentAsync(
      string fileName,
      string uploadType = null,
      string areaPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AttachmentReference attachmentAsync;
      using (FileStream uploadStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
        attachmentAsync = await this.CreateAttachmentAsync((Stream) uploadStream, fileName, uploadType, areaPath, userState, cancellationToken);
      return attachmentAsync;
    }

    public virtual async Task<List<WitBatchResponse>> GetQueriesBatchAsync(
      IEnumerable<Guid> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClient trackingHttpClient = this;
      if (ids == null)
        throw new ArgumentNullException(nameof (ids));
      HttpMethod httpMethod = HttpMethod.Post;
      ApiResourceVersion apiVersion = new ApiResourceVersion("2.2-preview.2");
      ApiResourceLocation location = new ApiResourceLocation();
      location.Area = "wit";
      location.ResourceName = "batch";
      location.RouteTemplate = "_apis/{area}/${resource}";
      location.MinVersion = apiVersion.ApiVersion;
      location.MaxVersion = apiVersion.ApiVersion;
      location.ReleasedVersion = apiVersion.ApiVersion;
      List<WitBatchRequest> witBatchRequests = new List<WitBatchRequest>();
      if (ids.Any<Guid>())
      {
        Guid queriesLocationId = WitConstants.WorkItemTrackingLocationIds.Queries;
        ApiResourceLocation resourceLocation = await trackingHttpClient.GetResourceLocationAsync(queriesLocationId, userState, cancellationToken).ConfigureAwait(false);
        if (resourceLocation == null)
        {
          // ISSUE: explicit non-virtual call
          throw new VssResourceNotFoundException(queriesLocationId, __nonvirtual (trackingHttpClient.BaseAddress));
        }
        foreach (Guid id in ids)
          witBatchRequests.Add(new WitBatchRequest()
          {
            Method = "GET",
            Uri = "/" + resourceLocation.RouteTemplate.Replace("{area}", resourceLocation.Area).Replace("{resource}", resourceLocation.ResourceName).Replace("{id}", id.ToString())
          });
        queriesLocationId = new Guid();
      }
      HttpContent content = (HttpContent) new ObjectContent<List<WitBatchRequest>>(witBatchRequests, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<WitBatchResponse> queriesBatchAsync;
      using (HttpRequestMessage requestMessage = trackingHttpClient.CreateRequestMessage(httpMethod, location, version: apiVersion, content: content))
        queriesBatchAsync = await trackingHttpClient.SendAsync<List<WitBatchResponse>>(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      httpMethod = (HttpMethod) null;
      apiVersion = (ApiResourceVersion) null;
      location = (ApiResourceLocation) null;
      witBatchRequests = (List<WitBatchRequest>) null;
      return queriesBatchAsync;
    }

    public virtual async Task<List<WitBatchResponse>> ExecuteBatchRequest(
      IEnumerable<WitBatchRequest> requests,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClient trackingHttpClient = this;
      if (requests == null)
        throw new ArgumentNullException(nameof (requests));
      if (!requests.Any<WitBatchRequest>())
        throw new ArgumentOutOfRangeException(nameof (requests));
      HttpMethod post = HttpMethod.Post;
      ApiResourceVersion version = new ApiResourceVersion("7.1-preview");
      ApiResourceLocation location = new ApiResourceLocation();
      location.Area = "wit";
      location.ResourceName = "batch";
      location.RouteTemplate = "_apis/{area}/${resource}";
      location.MinVersion = version.ApiVersion;
      location.MaxVersion = version.ApiVersion;
      location.ReleasedVersion = version.ApiVersion;
      HttpContent content = (HttpContent) new ObjectContent<List<WitBatchRequest>>(requests.ToList<WitBatchRequest>(), (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<WitBatchResponse> witBatchResponseList;
      using (HttpRequestMessage requestMessage = trackingHttpClient.CreateRequestMessage(post, location, version: version, content: content))
        witBatchResponseList = await trackingHttpClient.SendAsync<List<WitBatchResponse>>(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      return witBatchResponseList;
    }

    public virtual WitBatchRequest CreateWorkItemBatchRequest(
      int id,
      JsonPatchDocument document,
      bool bypassRules,
      bool suppressNotifications)
    {
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      return this.CreateWorkItemBatchRequest(document, string.Format("/_apis/wit/workItems/{0}?bypassRules={1}&suppressNotifications={2}", (object) id, (object) bypassRules, (object) suppressNotifications));
    }

    public virtual WitBatchRequest CreateWorkItemBatchRequest(
      Guid projectId,
      string type,
      JsonPatchDocument document,
      bool bypassRules,
      bool suppressNotifications)
    {
      if (Guid.Empty.Equals(projectId))
        throw new ArgumentOutOfRangeException(nameof (projectId));
      return this.CreateWorkItemBatchRequest(projectId.ToString(), type, document, bypassRules, suppressNotifications);
    }

    public virtual WitBatchRequest CreateWorkItemBatchRequest(
      string project,
      string type,
      JsonPatchDocument document,
      bool bypassRules,
      bool suppressNotifications)
    {
      if (string.IsNullOrEmpty(project))
        throw new ArgumentNullException(nameof (project));
      if (string.IsNullOrEmpty(type))
        throw new ArgumentNullException(nameof (type));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      return this.CreateWorkItemBatchRequest(document, string.Format("/{0}/_apis/wit/workItems/${1}?bypassRules={2}&suppressNotifications={3}", (object) project, (object) type, (object) bypassRules, (object) suppressNotifications));
    }

    private WitBatchRequest CreateWorkItemBatchRequest(JsonPatchDocument document, string uri)
    {
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion(5.0, 3);
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      };
      return new WitBatchRequest()
      {
        Method = "PATCH",
        Headers = new Dictionary<string, string>()
        {
          {
            "Content-Type",
            "application/json-patch+json"
          }
        },
        Uri = string.Format("{0}&api-version={1}", (object) uri, (object) apiResourceVersion),
        Body = JsonConvert.SerializeObject((object) document, settings)
      };
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      this.lastResponseStatusCode = new HttpStatusCode?(response.StatusCode);
      return base.HandleResponseAsync(response, cancellationToken);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) new Dictionary<string, Type>()
    {
      {
        "RuleValidationException",
        typeof (RuleValidationException)
      }
    };
  }
}
