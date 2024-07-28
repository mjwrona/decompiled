// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.WorkItemTrackingProcessTemplateHttpClient
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43776F51-3CE9-4177-A1CB-61A3432CC4EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi
{
  [ResourceArea("5264459E-E5E0-4BD8-B118-0985E68A4EC5")]
  public class WorkItemTrackingProcessTemplateHttpClient : 
    WorkItemTrackingProcessTemplateCompatHttpClientBase
  {
    public WorkItemTrackingProcessTemplateHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingProcessTemplateHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingProcessTemplateHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingProcessTemplateHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingProcessTemplateHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AdminBehavior> GetBehaviorAsync(
      Guid processId,
      string behaviorRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("90bf9317-3571-487b-bc8c-a523ba0e05d7");
      object routeValues = (object) new
      {
        processId = processId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (behaviorRefName), behaviorRefName);
      return this.SendAsync<AdminBehavior>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<AdminBehavior>> GetBehaviorsAsync(
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AdminBehavior>>(new HttpMethod("GET"), new Guid("90bf9317-3571-487b-bc8c-a523ba0e05d7"), (object) new
      {
        processId = processId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CheckTemplateExistenceResult> CheckTemplateExistenceAsync(
      Stream uploadStream,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object obj1 = (object) new
      {
        action = "CheckTemplateExistence"
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CheckTemplateExistenceResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Guid> CloneXmlToInheritedAsync(
      Guid SourceProcessId,
      string TargetProcessName,
      string ParentProcessName,
      string processDescription,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object routeValues = (object) new
      {
        action = "CloneXmlToInherited"
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (SourceProcessId), SourceProcessId.ToString());
      keyValuePairList.Add(nameof (TargetProcessName), TargetProcessName);
      keyValuePairList.Add(nameof (ParentProcessName), ParentProcessName);
      keyValuePairList.Add(nameof (processDescription), processDescription);
      return this.SendAsync<Guid>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> ExportProcessTemplateAsync(
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingProcessTemplateHttpClient templateHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object routeValues = (object) new
      {
        action = "Export",
        id = id
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await templateHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await templateHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<ProcessImportResult> ImportProcessTemplateAsync(
      Stream uploadStream,
      bool? ignoreWarnings = null,
      bool? replaceExistingTemplate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object obj1 = (object) new{ action = "Import" };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (ignoreWarnings.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = ignoreWarnings.Value;
        string str = flag.ToString();
        collection.Add(nameof (ignoreWarnings), str);
      }
      if (replaceExistingTemplate.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = replaceExistingTemplate.Value;
        string str = flag.ToString();
        collection.Add(nameof (replaceExistingTemplate), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessImportResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<ProcessPromoteStatus> ImportProcessTemplateStatusAsync(
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProcessPromoteStatus>(new HttpMethod("GET"), new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759"), (object) new
      {
        action = "Status",
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Guid> QueuePromoteProjectToProcessJobAsync(
      string projectName,
      Guid targetProcessId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object routeValues = (object) new
      {
        action = "QueuePromoteProjectToProcessJob"
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (projectName), projectName);
      keyValuePairList.Add(nameof (targetProcessId), targetProcessId.ToString());
      return this.SendAsync<Guid>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
