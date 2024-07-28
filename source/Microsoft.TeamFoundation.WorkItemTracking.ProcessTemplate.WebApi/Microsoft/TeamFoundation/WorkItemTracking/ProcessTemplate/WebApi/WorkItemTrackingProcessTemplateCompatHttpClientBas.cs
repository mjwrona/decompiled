// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.WorkItemTrackingProcessTemplateCompatHttpClientBase
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi
{
  public abstract class WorkItemTrackingProcessTemplateCompatHttpClientBase : VssHttpClientBase
  {
    public WorkItemTrackingProcessTemplateCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingProcessTemplateCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingProcessTemplateCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingProcessTemplateCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingProcessTemplateCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<ProcessImportResult> ImportProcessTemplateAsync(
      Stream uploadStream,
      bool? ignoreWarnings,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("29e1f38d-9e9c-4358-86a5-cdf9896a5759");
      object obj1 = (object) new{ action = "Import" };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessImportResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }
  }
}
