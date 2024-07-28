// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.WorkItemTrackingHttpClientCompatBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  public class WorkItemTrackingHttpClientCompatBase : VssHttpClientBase
  {
    public WorkItemTrackingHttpClientCompatBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingHttpClientCompatBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public async Task<AttachmentReference> CreateAttachmentAsync(
      string fileName,
      string uploadType,
      object userState,
      CancellationToken cancellationToken)
    {
      AttachmentReference attachmentAsync;
      using (FileStream uploadStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
        attachmentAsync = await this.CreateAttachmentAsync((Stream) uploadStream, fileName, uploadType, userState, cancellationToken);
      return attachmentAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<FieldDependentRule> GetDependentFieldsAsync(
      string project,
      string type,
      string field,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<FieldDependentRule>(new HttpMethod("GET"), new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb"), (object) new
      {
        project = project,
        type = type,
        field = field
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<FieldDependentRule> GetDependentFieldsAsync(
      Guid project,
      string type,
      string field,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<FieldDependentRule>(new HttpMethod("GET"), new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb"), (object) new
      {
        project = project,
        type = type,
        field = field
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemTypeFieldInstance> GetWorkItemTypeFieldAsync(
      string project,
      string type,
      string field,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type,
        field = field
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeFieldInstance>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid id,
      string fileName,
      object userState,
      CancellationToken cancellationToken)
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await clientCompatBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await clientCompatBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      Guid id,
      string fileName,
      object userState,
      CancellationToken cancellationToken)
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await clientCompatBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await clientCompatBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemTypeFieldInstance> GetWorkItemTypeFieldAsync(
      Guid project,
      string type,
      string field,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type,
        field = field
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeFieldInstance>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemTypeFieldInstance>> GetWorkItemTypeFieldsAsync(
      string project,
      string type,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeFieldInstance>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemTypeFieldInstance>> GetWorkItemTypeFieldsAsync(
      Guid project,
      string type,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeFieldInstance>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<AttachmentReference> CreateAttachmentAsync(
      Stream uploadStream,
      string fileName,
      string uploadType,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(fileName))
        collection.Add(nameof (fileName), fileName);
      if (!string.IsNullOrEmpty(uploadType))
        collection.Add(nameof (uploadType), uploadType);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AttachmentReference>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<WorkItemHistory>> GetHistoryAsync(
      int id,
      int? top,
      int? skip,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f74eba29-47a1-4152-9381-84040aced527");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WorkItemHistory>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<WorkItemHistory> GetHistoryByIdAsync(
      int id,
      int revisionNumber,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<WorkItemHistory>(new HttpMethod("GET"), new Guid("f74eba29-47a1-4152-9381-84040aced527"), (object) new
      {
        id = id,
        revisionNumber = revisionNumber
      }, new ApiResourceVersion("3.0-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      string project,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      Guid project,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      string project,
      bool includeDeleted,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeDeleted), includeDeleted.ToString());
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      Guid project,
      bool includeDeleted,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeDeleted), includeDeleted.ToString());
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      bool includeDeleted,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      int? watermark,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeDeleted), includeDeleted.ToString());
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (includeIdentityRef.HasValue)
        keyValuePairList.Add(nameof (includeIdentityRef), includeIdentityRef.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion("2.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      string project,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      string continuationToken,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      bool? includeDeleted,
      bool? includeTagRef,
      bool? includeLatestOnly,
      ReportingRevisionsExpand? expand,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      Guid project,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      string continuationToken,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      bool? includeDeleted,
      bool? includeTagRef,
      bool? includeLatestOnly,
      ReportingRevisionsExpand? expand,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      IEnumerable<string> fields,
      IEnumerable<string> types,
      string continuationToken,
      DateTime? startDateTime,
      bool? includeIdentityRef,
      bool? includeDeleted,
      bool? includeTagRef,
      bool? includeLatestOnly,
      ReportingRevisionsExpand? expand,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion("3.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      string project,
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      Guid project,
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      int? watermark,
      DateTime? startDateTime,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      string project,
      int? watermark,
      DateTime? startDateTime,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      Guid project,
      int? watermark,
      DateTime? startDateTime,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (watermark.HasValue)
        keyValuePairList.Add(nameof (watermark), watermark.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [Obsolete("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(object userState, CancellationToken cancellationToken) instead.", false)]
    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      object userState,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(object userState, CancellationToken cancellationToken) instead.");
    }

    [Obsolete("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(string project, object userState, CancellationToken cancellationToken) instead.", false)]
    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      string project,
      object userState,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(string project, object userState, CancellationToken cancellationToken) instead.");
    }

    [Obsolete("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(Guid project, object userState, CancellationToken cancellationToken) instead.", false)]
    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      Guid project,
      object userState,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("This method is not supported anymore. Please use Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemReferencesAsync(Guid project, object userState, CancellationToken cancellationToken) instead.");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<WorkItemReference>> GetDeletedWorkItemReferencesAsync(
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), version: new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<WorkItemReference>> GetDeletedWorkItemReferencesAsync(
      string project,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<WorkItemReference>> GetDeletedWorkItemReferencesAsync(
      Guid project,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      string project,
      string type,
      bool? validateOnly,
      bool? bypassRules,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      Guid project,
      string type,
      bool? validateOnly,
      bool? bypassRules,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      string project,
      string type,
      bool? validateOnly,
      bool? bypassRules,
      bool? suppressNotifications,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection.Add(nameof (suppressNotifications), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      Guid project,
      string type,
      bool? validateOnly,
      bool? bypassRules,
      bool? suppressNotifications,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection.Add(nameof (suppressNotifications), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      int id,
      bool? validateOnly,
      bool? bypassRules,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new{ id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      int id,
      bool? validateOnly,
      bool? bypassRules,
      bool? suppressNotifications,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new{ id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection.Add(nameof (suppressNotifications), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      string project,
      int id,
      bool? validateOnly,
      bool? bypassRules,
      bool? suppressNotifications,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection.Add(nameof (suppressNotifications), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      Guid project,
      int id,
      bool? validateOnly,
      bool? bypassRules,
      bool? suppressNotifications,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection.Add(nameof (suppressNotifications), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<ArtifactUriQueryResult> GetWorkItemsForArtifactUrisAsync(
      ArtifactUriQuery artifactUriQuery,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9a9aa7a-8c09-44d3-ad1b-46e855c1e3d3");
      HttpContent httpContent = (HttpContent) new ObjectContent<ArtifactUriQuery>(artifactUriQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("4.1-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactUriQueryResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<RemoteWorkItemLinkUpdateResult>> UpdateRemoteLinksAsync(
      IEnumerable<RemoteWorkItemLinkUpdate> links,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3f0377f8-d4bf-445b-b1e7-f9e5f1ba8fdb");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<RemoteWorkItemLinkUpdate>>(links, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<RemoteWorkItemLinkUpdateResult>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CleanupRemoteLinksFromProjectDelete(
      IEnumerable<Guid> remoteProjectIds,
      Guid remoteHostId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3f0377f8-d4bf-445b-b1e7-f9e5f1ba8fdb");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Guid>>(remoteProjectIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (remoteHostId), remoteHostId.ToString());
      WorkItemTrackingHttpClientCompatBase clientCompatBase2 = clientCompatBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await clientCompatBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      TeamContext teamContext,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      HttpResponseMessage response = await clientCompatBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      return (int) ConvertUtility.ChangeType((object) clientCompatBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      string project,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      HttpResponseMessage response = await clientCompatBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      return (int) ConvertUtility.ChangeType((object) clientCompatBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      Guid project,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      HttpResponseMessage response = await clientCompatBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      return (int) ConvertUtility.ChangeType((object) clientCompatBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientCompatBase clientCompatBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      HttpResponseMessage response = await clientCompatBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      return (int) ConvertUtility.ChangeType((object) clientCompatBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      TeamContext teamContext,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      string project,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      Guid project,
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      Guid id,
      bool? timePrecision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComment> GetCommentAsync(
      int id,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemComment>(new HttpMethod("GET"), new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf"), (object) new
      {
        id = id,
        revision = revision
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComment> GetCommentAsync(
      string project,
      int id,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemComment>(new HttpMethod("GET"), new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf"), (object) new
      {
        project = project,
        id = id,
        revision = revision
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComment> GetCommentAsync(
      Guid project,
      int id,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemComment>(new HttpMethod("GET"), new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf"), (object) new
      {
        project = project,
        id = id,
        revision = revision
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComments> GetCommentsAsync(
      string project,
      int id,
      int? fromRevision = null,
      int? top = null,
      CommentSortOrder? order = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (fromRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = fromRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (fromRevision), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      return this.SendAsync<WorkItemComments>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComments> GetCommentsAsync(
      Guid project,
      int id,
      int? fromRevision = null,
      int? top = null,
      CommentSortOrder? order = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (fromRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = fromRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (fromRevision), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      return this.SendAsync<WorkItemComments>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemComments> GetCommentsAsync(
      int id,
      int? fromRevision = null,
      int? top = null,
      CommentSortOrder? order = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19335ae7-22f7-4308-93d8-261f9384b7cf");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (fromRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = fromRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (fromRevision), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      return this.SendAsync<WorkItemComments>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<QueryHierarchyItem> CreateQueryAsync(
      QueryHierarchyItem postedQuery,
      string project,
      string query,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<QueryHierarchyItem> CreateQueryAsync(
      QueryHierarchyItem postedQuery,
      Guid project,
      string query,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<QueryHierarchyItem> GetQueryAsync(
      string project,
      string query,
      QueryExpand? expand,
      int? depth,
      bool? includeDeleted,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new
      {
        project = project,
        query = query
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, new ApiResourceVersion(6.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<QueryHierarchyItem> GetQueryAsync(
      Guid project,
      string query,
      QueryExpand? expand,
      int? depth,
      bool? includeDeleted,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new
      {
        project = project,
        query = query
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, new ApiResourceVersion(6.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> UpdateFieldAsync(
      UpdateWorkItemField payload,
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateWorkItemField>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> UpdateFieldAsync(
      UpdateWorkItemField payload,
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateWorkItemField>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> UpdateFieldAsync(
      UpdateWorkItemField payload,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateWorkItemField>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> CreateFieldAsync(
      WorkItemField workItemField,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> CreateFieldAsync(
      WorkItemField workItemField,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> CreateFieldAsync(
      WorkItemField workItemField,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteFieldAsync(
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteFieldAsync(
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteFieldAsync(
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> GetFieldAsync(
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> GetFieldAsync(
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemField> GetFieldAsync(
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.1, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemField>> GetFieldsAsync(
      string project,
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemField>> GetFieldsAsync(
      Guid project,
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemField>> GetFieldsAsync(
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField>>(method, locationId, version: new ApiResourceVersion(7.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
