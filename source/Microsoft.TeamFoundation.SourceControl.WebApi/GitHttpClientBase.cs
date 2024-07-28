// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitHttpClientBase
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Policy.WebApi;
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

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ResourceArea("4E080C62-FA21-4FBC-8FEF-2A10A2B38049")]
  public abstract class GitHttpClientBase : GitCompatHttpClientBase
  {
    public GitHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GitHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GitHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GitHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GitHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteEnablementStatusAsync(
      bool allProjects,
      bool? includeBillableCommitters = null,
      IEnumerable<Guid> projectIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$allProjects", allProjects.ToString());
      if (includeBillableCommitters.HasValue)
        keyValuePairList.Add("$includeBillableCommitters", includeBillableCommitters.Value.ToString());
      if (projectIds != null)
        gitHttpClientBase.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (projectIds), (object) projectIds);
      using (await gitHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AdvSecEnablementStatus>> GetEnablementStatusAsync(
      IEnumerable<Guid> projectIds = null,
      DateTime? billingDate = null,
      int? skip = null,
      int? take = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectIds != null)
        this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (projectIds), (object) projectIds);
      if (billingDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, "$billingDate", billingDate.Value);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (take.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = take.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$take", str);
      }
      return this.SendAsync<List<AdvSecEnablementStatus>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> GetEnableOnCreateHostAsync(
      bool enableOnCreateHost,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$enableOnCreateHost", enableOnCreateHost.ToString());
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> GetEnableOnCreateProjectAsync(
      Guid enableOnCreateProjectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$enableOnCreateProjectId", enableOnCreateProjectId.ToString());
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SetEnableOnCreateHostAsync(
      bool enableOnCreateHost,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$enableOnCreateHost", enableOnCreateHost.ToString());
      using (await gitHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SetEnableOnCreateProjectAsync(
      Guid enableOnCreateProjectId,
      bool enableOnStatus,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$enableOnCreateProjectId", enableOnCreateProjectId.ToString());
      keyValuePairList.Add("$enableOnStatus", enableOnStatus.ToString());
      using (await gitHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateEnablementStatusAsync(
      IEnumerable<AdvSecEnablementUpdate> enablementUpdates,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b43dd56f-a1b4-47a5-a857-73fc1b6c700c");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<AdvSecEnablementUpdate>>(enablementUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillablePusher>> GetEstimatedBillablePushersOrgAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillablePusher>>(new HttpMethod("GET"), new Guid("2277ffbe-28d4-40d6-9c26-40baf26d1408"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillablePusher>> GetEstimatedBillablePushersProjectAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillablePusher>>(new HttpMethod("GET"), new Guid("1df7833e-1eed-447b-81a3-390c74923900"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillablePusher>> GetEstimatedBillablePushersProjectAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillablePusher>>(new HttpMethod("GET"), new Guid("1df7833e-1eed-447b-81a3-390c74923900"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetEstimatedBillableCommittersRepoAsync(
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillableCommitter>>(new HttpMethod("GET"), new Guid("5dcec07b-a844-4efb-9fc1-968fd1f149db"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetEstimatedBillableCommittersRepoAsync(
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillableCommitter>>(new HttpMethod("GET"), new Guid("5dcec07b-a844-4efb-9fc1-968fd1f149db"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetEstimatedBillableCommittersRepoAsync(
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillableCommitter>>(new HttpMethod("GET"), new Guid("5dcec07b-a844-4efb-9fc1-968fd1f149db"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetEstimatedBillableCommittersRepoAsync(
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BillableCommitter>>(new HttpMethod("GET"), new Guid("5dcec07b-a844-4efb-9fc1-968fd1f149db"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> GetPermissionAsync(
      string projectName = null,
      string repositoryId = null,
      string permission = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61b21a05-a60f-4910-a733-ba5347c2142d");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectName != null)
        keyValuePairList.Add("$projectName", projectName);
      if (repositoryId != null)
        keyValuePairList.Add("$repositoryId", repositoryId);
      if (permission != null)
        keyValuePairList.Add("$permission", permission);
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitAnnotatedTag> CreateAnnotatedTagAsync(
      GitAnnotatedTag tagObject,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5e8a8081-3851-4626-b677-9891cc04102e");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAnnotatedTag>(tagObject, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitAnnotatedTag>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitAnnotatedTag> CreateAnnotatedTagAsync(
      GitAnnotatedTag tagObject,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5e8a8081-3851-4626-b677-9891cc04102e");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAnnotatedTag>(tagObject, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitAnnotatedTag>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitAnnotatedTag> CreateAnnotatedTagAsync(
      GitAnnotatedTag tagObject,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5e8a8081-3851-4626-b677-9891cc04102e");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAnnotatedTag>(tagObject, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitAnnotatedTag>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitAnnotatedTag> CreateAnnotatedTagAsync(
      GitAnnotatedTag tagObject,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5e8a8081-3851-4626-b677-9891cc04102e");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAnnotatedTag>(tagObject, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitAnnotatedTag>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitAnnotatedTag> GetAnnotatedTagAsync(
      string project,
      string repositoryId,
      string objectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitAnnotatedTag>(new HttpMethod("GET"), new Guid("5e8a8081-3851-4626-b677-9891cc04102e"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        objectId = objectId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitAnnotatedTag> GetAnnotatedTagAsync(
      string project,
      Guid repositoryId,
      string objectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitAnnotatedTag>(new HttpMethod("GET"), new Guid("5e8a8081-3851-4626-b677-9891cc04102e"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        objectId = objectId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitAnnotatedTag> GetAnnotatedTagAsync(
      Guid project,
      string repositoryId,
      string objectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitAnnotatedTag>(new HttpMethod("GET"), new Guid("5e8a8081-3851-4626-b677-9891cc04102e"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        objectId = objectId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitAnnotatedTag> GetAnnotatedTagAsync(
      Guid project,
      Guid repositoryId,
      string objectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitAnnotatedTag>(new HttpMethod("GET"), new Guid("5e8a8081-3851-4626-b677-9891cc04102e"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        objectId = objectId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetBillableCommittersAsync(
      string project,
      DateTime? billingDate = null,
      int? skip = null,
      int? take = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5c5e3ebc-37b0-4547-a957-945912d44922");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (billingDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, "$billingDate", billingDate.Value);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (take.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = take.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$take", str);
      }
      return this.SendAsync<List<BillableCommitter>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitter>> GetBillableCommittersAsync(
      Guid project,
      DateTime? billingDate = null,
      int? skip = null,
      int? take = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5c5e3ebc-37b0-4547-a957-945912d44922");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (billingDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, "$billingDate", billingDate.Value);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (take.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = take.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$take", str);
      }
      return this.SendAsync<List<BillableCommitter>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitterDetail>> GetBillableCommittersDetailAsync(
      string project,
      string includeDetails,
      DateTime? billingDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5c5e3ebc-37b0-4547-a957-945912d44922");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$includeDetails", includeDetails);
      if (billingDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, "$billingDate", billingDate.Value);
      return this.SendAsync<List<BillableCommitterDetail>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<BillableCommitterDetail>> GetBillableCommittersDetailAsync(
      Guid project,
      string includeDetails,
      DateTime? billingDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5c5e3ebc-37b0-4547-a957-945912d44922");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$includeDetails", includeDetails);
      if (billingDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, "$billingDate", billingDate.Value);
      return this.SendAsync<List<BillableCommitterDetail>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobContentAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      string repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      Guid repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      string project,
      string repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      string project,
      Guid repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      Guid project,
      string repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobsZipAsync(
      IEnumerable<string> blobIds,
      Guid project,
      Guid repositoryId,
      string filename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object obj = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(blobIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (filename != null)
        collection.Add(nameof (filename), filename);
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBlobZipAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      bool? resolveLfs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b28e929-2c99-405d-9c5c-6167a06e6816");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      string project,
      string repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      string project,
      Guid repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      Guid project,
      string repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      Guid project,
      Guid repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      string repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitBranchStats> GetBranchAsync(
      Guid repositoryId,
      string name,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<GitBranchStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      string project,
      string repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      string project,
      Guid repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      Guid project,
      string repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      Guid project,
      Guid repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      string repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitBranchStats>> GetBranchesAsync(
      Guid repositoryId,
      GitVersionDescriptor baseVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitBranchStats>> GetBranchStatsBatchAsync(
      GitQueryBranchStatsCriteria searchCriteria,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d5b216de-d8d5-4d32-ae76-51df755b16d3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryBranchStatsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitBranchStats>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      string project,
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      string project,
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      Guid project,
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      Guid project,
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitChanges> GetChangesAsync(
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5bf884f5-3e07-42e9-afb8-1b872267bf16");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<GitCommitChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      string project,
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      string project,
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      Guid project,
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetCherryPickConflictAsync(
      Guid project,
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      string project,
      string repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      string project,
      Guid repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      Guid project,
      string repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      Guid project,
      Guid repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      string repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetCherryPickConflictsAsync(
      Guid repositoryId,
      int cherryPickId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      string project,
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      string project,
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      Guid project,
      string repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateCherryPickConflictAsync(
      GitConflict conflict,
      Guid project,
      Guid repositoryId,
      int cherryPickId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      string repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      Guid repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      string repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateCherryPickConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      Guid repositoryId,
      int cherryPickId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1fe5aab2-d4c0-4b2f-a030-f3831e7aca26");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        cherryPickId = cherryPickId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      string project,
      string repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      string project,
      Guid repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      Guid project,
      string repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      Guid project,
      Guid repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      string repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitCommitRef>> GetCherryPickRelationshipsAsync(
      Guid repositoryNameOrId,
      string commitId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8af142a4-27c2-4168-9e82-46b8629aaa0d");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> CreateCherryPickAsync(
      GitAsyncRefOperationParameters cherryPickToCreate,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(cherryPickToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitCherryPick> CreateCherryPickAsync(
      GitAsyncRefOperationParameters cherryPickToCreate,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(cherryPickToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitCherryPick> CreateCherryPickAsync(
      GitAsyncRefOperationParameters cherryPickToCreate,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(cherryPickToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitCherryPick> CreateCherryPickAsync(
      GitAsyncRefOperationParameters cherryPickToCreate,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(cherryPickToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitCherryPick> GetCherryPickAsync(
      string project,
      int cherryPickId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitCherryPick>(new HttpMethod("GET"), new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6"), (object) new
      {
        project = project,
        cherryPickId = cherryPickId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickAsync(
      string project,
      int cherryPickId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitCherryPick>(new HttpMethod("GET"), new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6"), (object) new
      {
        project = project,
        cherryPickId = cherryPickId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickAsync(
      Guid project,
      int cherryPickId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitCherryPick>(new HttpMethod("GET"), new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6"), (object) new
      {
        project = project,
        cherryPickId = cherryPickId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickAsync(
      Guid project,
      int cherryPickId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitCherryPick>(new HttpMethod("GET"), new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6"), (object) new
      {
        project = project,
        cherryPickId = cherryPickId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickForRefNameAsync(
      string project,
      string repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickForRefNameAsync(
      string project,
      Guid repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickForRefNameAsync(
      Guid project,
      string repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCherryPick> GetCherryPickForRefNameAsync(
      Guid project,
      Guid repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("033bad68-9a14-43d1-90e0-59cb8856fef6");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitCherryPick>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      string project,
      string repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      string project,
      Guid repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      Guid project,
      string repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      Guid project,
      Guid repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      string repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommitDiffs> GetCommitDiffsAsync(
      Guid repositoryId,
      bool? diffCommonCommit = null,
      int? top = null,
      int? skip = null,
      GitBaseVersionDescriptor baseVersionDescriptor = null,
      GitTargetVersionDescriptor targetVersionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("615588d5-c0c7-4b88-88f8-e625306446e8");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (diffCommonCommit.HasValue)
        keyValuePairList.Add(nameof (diffCommonCommit), diffCommonCommit.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (baseVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (baseVersionDescriptor), (object) baseVersionDescriptor);
      if (targetVersionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (targetVersionDescriptor), (object) targetVersionDescriptor);
      return this.SendAsync<GitCommitDiffs>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      string project,
      string commitId,
      string repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      string project,
      string commitId,
      Guid repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      Guid project,
      string commitId,
      string repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      Guid project,
      string commitId,
      Guid repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      string commitId,
      string repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitCommit> GetCommitAsync(
      string commitId,
      Guid repositoryId,
      int? changeCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeCount.HasValue)
        keyValuePairList.Add(nameof (changeCount), changeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GitCommit>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      string project,
      string repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      string project,
      Guid repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      Guid project,
      string repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      Guid project,
      Guid repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      string repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsAsync(
      Guid repositoryId,
      GitQueryCommitsCriteria searchCriteria,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      string project,
      string repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      string project,
      Guid repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      Guid project,
      string repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      Guid project,
      Guid repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      string repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPushCommitsAsync(
      Guid repositoryId,
      int pushId,
      int? top = null,
      int? skip = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c2570c3b-5b3f-41b8-98bf-5407bfde8d58");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pushId), pushId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      string repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      string project,
      string repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      string project,
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      Guid project,
      string repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetCommitsBatchAsync(
      GitQueryCommitsCriteria searchCriteria,
      Guid project,
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      bool? includeStatuses = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6400dfb2-0bcb-462b-b992-5a57f8f1416c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitQueryCommitsCriteria>(searchCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      if (includeStatuses.HasValue)
        collection1.Add(nameof (includeStatuses), includeStatuses.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitDeletedRepository>> GetDeletedRepositoriesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitDeletedRepository>>(new HttpMethod("GET"), new Guid("2b6869c4-cb25-42b5-b7a3-0d3e6be0a11a"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitDeletedRepository>> GetDeletedRepositoriesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitDeletedRepository>>(new HttpMethod("GET"), new Guid("2b6869c4-cb25-42b5-b7a3-0d3e6be0a11a"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FileDiff>> GetFileDiffsAsync(
      FileDiffsCriteria fileDiffsCriteria,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4c5a7e6-e9f3-4730-a92b-84baacff694b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileDiffsCriteria>(fileDiffsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FileDiff>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FileDiff>> GetFileDiffsAsync(
      FileDiffsCriteria fileDiffsCriteria,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4c5a7e6-e9f3-4730-a92b-84baacff694b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileDiffsCriteria>(fileDiffsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FileDiff>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FileDiff>> GetFileDiffsAsync(
      FileDiffsCriteria fileDiffsCriteria,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4c5a7e6-e9f3-4730-a92b-84baacff694b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileDiffsCriteria>(fileDiffsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FileDiff>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FileDiff>> GetFileDiffsAsync(
      FileDiffsCriteria fileDiffsCriteria,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4c5a7e6-e9f3-4730-a92b-84baacff694b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileDiffsCriteria>(fileDiffsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FileDiff>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitFilePathsCollection> GetFilePathsAsync(
      string project,
      string repositoryId,
      string scopePath = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e74b530c-edfa-402b-88e2-8d04671134f7");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<GitFilePathsCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitFilePathsCollection> GetFilePathsAsync(
      string project,
      Guid repositoryId,
      string scopePath = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e74b530c-edfa-402b-88e2-8d04671134f7");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<GitFilePathsCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitFilePathsCollection> GetFilePathsAsync(
      Guid project,
      string repositoryId,
      string scopePath = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e74b530c-edfa-402b-88e2-8d04671134f7");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<GitFilePathsCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitFilePathsCollection> GetFilePathsAsync(
      Guid project,
      Guid repositoryId,
      string scopePath = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e74b530c-edfa-402b-88e2-8d04671134f7");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<GitFilePathsCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      string project,
      string repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      string project,
      Guid repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      Guid project,
      string repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      Guid project,
      Guid repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      string repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepositoryRef>> GetForksAsync(
      Guid repositoryNameOrId,
      Guid collectionId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("158c0340-bf6f-489c-9625-d572a1480d57");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        collectionId = collectionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepositoryRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      string repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      Guid repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      string project,
      string repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      string project,
      Guid repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      Guid project,
      string repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> CreateForkSyncRequestAsync(
      GitForkSyncRequestParameters syncParams,
      Guid project,
      Guid repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitForkSyncRequestParameters>(syncParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      string project,
      string repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      string project,
      Guid repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      Guid project,
      string repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      Guid project,
      Guid repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      string repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitForkSyncRequest> GetForkSyncRequestAsync(
      Guid repositoryNameOrId,
      int forkSyncOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        forkSyncOperationId = forkSyncOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitForkSyncRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      string project,
      string repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      string project,
      Guid repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      Guid project,
      string repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      Guid project,
      Guid repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      string repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitForkSyncRequest>> GetForkSyncRequestsAsync(
      Guid repositoryNameOrId,
      bool? includeAbandoned = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1703f858-b9d1-46af-ab62-483e9e1055b5");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAbandoned.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAbandoned.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAbandoned), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<GitForkSyncRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitImportRequest> CreateImportRequestAsync(
      GitImportRequest importRequest,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> CreateImportRequestAsync(
      GitImportRequest importRequest,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> CreateImportRequestAsync(
      GitImportRequest importRequest,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> CreateImportRequestAsync(
      GitImportRequest importRequest,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> GetImportRequestAsync(
      string project,
      string repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitImportRequest>(new HttpMethod("GET"), new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitImportRequest> GetImportRequestAsync(
      string project,
      Guid repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitImportRequest>(new HttpMethod("GET"), new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitImportRequest> GetImportRequestAsync(
      Guid project,
      string repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitImportRequest>(new HttpMethod("GET"), new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitImportRequest> GetImportRequestAsync(
      Guid project,
      Guid repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitImportRequest>(new HttpMethod("GET"), new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitImportRequest>> QueryImportRequestsAsync(
      string project,
      string repositoryId,
      bool? includeAbandoned = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAbandoned.HasValue)
        keyValuePairList.Add(nameof (includeAbandoned), includeAbandoned.Value.ToString());
      return this.SendAsync<List<GitImportRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitImportRequest>> QueryImportRequestsAsync(
      string project,
      Guid repositoryId,
      bool? includeAbandoned = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAbandoned.HasValue)
        keyValuePairList.Add(nameof (includeAbandoned), includeAbandoned.Value.ToString());
      return this.SendAsync<List<GitImportRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitImportRequest>> QueryImportRequestsAsync(
      Guid project,
      string repositoryId,
      bool? includeAbandoned = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAbandoned.HasValue)
        keyValuePairList.Add(nameof (includeAbandoned), includeAbandoned.Value.ToString());
      return this.SendAsync<List<GitImportRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitImportRequest>> QueryImportRequestsAsync(
      Guid project,
      Guid repositoryId,
      bool? includeAbandoned = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAbandoned.HasValue)
        keyValuePairList.Add(nameof (includeAbandoned), includeAbandoned.Value.ToString());
      return this.SendAsync<List<GitImportRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitImportRequest> UpdateImportRequestAsync(
      GitImportRequest importRequestToUpdate,
      string project,
      string repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> UpdateImportRequestAsync(
      GitImportRequest importRequestToUpdate,
      string project,
      Guid repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> UpdateImportRequestAsync(
      GitImportRequest importRequestToUpdate,
      Guid project,
      string repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitImportRequest> UpdateImportRequestAsync(
      GitImportRequest importRequestToUpdate,
      Guid project,
      Guid repositoryId,
      int importRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01828ddc-3600-4a41-8633-99b3a73a0eb3");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        importRequestId = importRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitImportRequest>(importRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitImportRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitItem> GetItemAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitItem> GetItemAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitItem> GetItemAsync(
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitItem> GetItemAsync(
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      if (resolveLfs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = resolveLfs.Value;
        string str = flag.ToString();
        collection.Add(nameof (resolveLfs), str);
      }
      if (sanitize.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = sanitize.Value;
        string str = flag.ToString();
        collection.Add(nameof (sanitize), str);
      }
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      string project,
      string repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      string project,
      Guid repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      Guid project,
      string repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      Guid project,
      Guid repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      string repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitItem>> GetItemsAsync(
      Guid repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? zipForUnix = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      bool flag;
      if (includeContentMetadata.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContentMetadata.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContentMetadata), str);
      }
      if (latestProcessedChange.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestProcessedChange.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestProcessedChange), str);
      }
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (zipForUnix.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = zipForUnix.Value;
        string str = flag.ToString();
        collection.Add(nameof (zipForUnix), str);
      }
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      string repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      Guid repositoryId,
      string path,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      bool? resolveLfs = null,
      bool? sanitize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContentMetadata.HasValue)
        keyValuePairList.Add(nameof (includeContentMetadata), includeContentMetadata.Value.ToString());
      if (latestProcessedChange.HasValue)
        keyValuePairList.Add(nameof (latestProcessedChange), latestProcessedChange.Value.ToString());
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (versionDescriptor != null)
        gitHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      if (sanitize.HasValue)
        keyValuePairList.Add(nameof (sanitize), sanitize.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<GitItem>>> GetItemsBatchAsync(
      GitItemRequestData requestData,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("630fd2e4-fb88-4f85-ad21-13f3fd1fbca9");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitItemRequestData>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<GitItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      string project,
      string repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      string project,
      Guid repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      Guid project,
      string repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      Guid project,
      Guid repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      string repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetMergeBasesAsync(
      Guid repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7cf2abb6-c964-4f7e-9872-f78c66e72e9c");
      object routeValues = (object) new
      {
        repositoryNameOrId = repositoryNameOrId,
        commitId = commitId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (otherCommitId), otherCommitId);
      Guid guid;
      if (otherCollectionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherCollectionId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherCollectionId), str);
      }
      if (otherRepositoryId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = otherRepositoryId.Value;
        string str = guid.ToString();
        collection.Add(nameof (otherRepositoryId), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitMerge> CreateMergeRequestAsync(
      GitMergeParameters mergeParameters,
      string project,
      string repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitMergeParameters>(mergeParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitMerge>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitMerge> CreateMergeRequestAsync(
      GitMergeParameters mergeParameters,
      string project,
      Guid repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitMergeParameters>(mergeParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitMerge>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitMerge> CreateMergeRequestAsync(
      GitMergeParameters mergeParameters,
      Guid project,
      string repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitMergeParameters>(mergeParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitMerge>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitMerge> CreateMergeRequestAsync(
      GitMergeParameters mergeParameters,
      Guid project,
      Guid repositoryNameOrId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object obj1 = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitMergeParameters>(mergeParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        collection.Add(nameof (includeLinks), includeLinks.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitMerge>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitMerge> GetMergeRequestAsync(
      string project,
      string repositoryNameOrId,
      int mergeOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        mergeOperationId = mergeOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitMerge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitMerge> GetMergeRequestAsync(
      string project,
      Guid repositoryNameOrId,
      int mergeOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        mergeOperationId = mergeOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitMerge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitMerge> GetMergeRequestAsync(
      Guid project,
      string repositoryNameOrId,
      int mergeOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        mergeOperationId = mergeOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitMerge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitMerge> GetMergeRequestAsync(
      Guid project,
      Guid repositoryNameOrId,
      int mergeOperationId,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("985f7ae9-844f-4906-9897-7ef41516c0e2");
      object routeValues = (object) new
      {
        project = project,
        repositoryNameOrId = repositoryNameOrId,
        mergeOperationId = mergeOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<GitMerge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<GitPolicyConfigurationResponse> GetPolicyConfigurationsAsync(
      string project,
      Guid? repositoryId = null,
      string refName = null,
      Guid? policyType = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2c420070-a0a2-49cc-9639-c9f271c5ff07");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId.HasValue)
        keyValuePairList.Add(nameof (repositoryId), repositoryId.Value.ToString());
      if (refName != null)
        keyValuePairList.Add(nameof (refName), refName);
      if (policyType.HasValue)
        keyValuePairList.Add(nameof (policyType), policyType.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitPolicyConfigurationResponse configurationsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitPolicyConfigurationResponse returnObject = new GitPolicyConfigurationResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          GitPolicyConfigurationResponse configurationResponse1 = returnObject;
          IEnumerable<string> headerValue = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          string str = headerValue != null ? headerValue.FirstOrDefault<string>() : (string) null;
          configurationResponse1.ContinuationToken = str;
          GitPolicyConfigurationResponse configurationResponse = returnObject;
          configurationResponse.PolicyConfigurations = (IEnumerable<PolicyConfiguration>) await gitHttpClientBase.ReadContentAsAsync<List<PolicyConfiguration>>(response, cancellationToken).ConfigureAwait(false);
          configurationResponse = (GitPolicyConfigurationResponse) null;
        }
        configurationsAsync = returnObject;
      }
      return configurationsAsync;
    }

    public virtual async Task<GitPolicyConfigurationResponse> GetPolicyConfigurationsAsync(
      Guid project,
      Guid? repositoryId = null,
      string refName = null,
      Guid? policyType = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2c420070-a0a2-49cc-9639-c9f271c5ff07");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId.HasValue)
        keyValuePairList.Add(nameof (repositoryId), repositoryId.Value.ToString());
      if (refName != null)
        keyValuePairList.Add(nameof (refName), refName);
      if (policyType.HasValue)
        keyValuePairList.Add(nameof (policyType), policyType.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitPolicyConfigurationResponse configurationsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitPolicyConfigurationResponse returnObject = new GitPolicyConfigurationResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          GitPolicyConfigurationResponse configurationResponse1 = returnObject;
          IEnumerable<string> headerValue = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          string str = headerValue != null ? headerValue.FirstOrDefault<string>() : (string) null;
          configurationResponse1.ContinuationToken = str;
          GitPolicyConfigurationResponse configurationResponse = returnObject;
          configurationResponse.PolicyConfigurations = (IEnumerable<PolicyConfiguration>) await gitHttpClientBase.ReadContentAsAsync<List<PolicyConfiguration>>(response, cancellationToken).ConfigureAwait(false);
          configurationResponse = (GitPolicyConfigurationResponse) null;
        }
        configurationsAsync = returnObject;
      }
      return configurationsAsync;
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      string project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      string project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      Guid project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> CreateAttachmentAsync(
      Stream uploadStream,
      Guid project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object obj1 = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
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
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteAttachmentAsync(
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      string project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      string project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      Guid project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      Guid project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      string project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      string project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      string project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      string project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      Guid project,
      string fileName,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      Guid project,
      string fileName,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("965d9361-878b-413b-a494-45d5b5fd8ab7");
      object routeValues = (object) new
      {
        project = project,
        fileName = fileName,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task CreateLikeAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateLikeAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateLikeAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateLikeAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateLikeAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateLikeAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("5f2e2851-1389-425b-a00b-fb2adb3ef31b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<GitCommitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitCommitRef>> GetPullRequestCommitsAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("52823034-34a8-4576-922c-8d8b77e9e4c4"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetPullRequestConflictAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("d840fb74-bbef-42d3-b250-564604c054a4"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      string project,
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdatePullRequestConflictAsync(
      GitConflict conflict,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdatePullRequestConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d840fb74-bbef-42d3-b250-564604c054a4");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIterationChanges> GetPullRequestIterationChangesAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4216bdcf-b6b1-4d59-8b82-c34cc183fc8b");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
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
      if (compareTo.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = compareTo.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$compareTo", str);
      }
      return this.SendAsync<GitPullRequestIterationChanges>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestIteration> GetPullRequestIterationAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestIteration>(new HttpMethod("GET"), new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      string repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestIteration>> GetPullRequestIterationsAsync(
      Guid repositoryId,
      int pullRequestId,
      bool? includeCommits = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d43911ee-6958-46b0-a42b-8445b8a0d004");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString());
      return this.SendAsync<List<GitPullRequestIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestIterationStatusAsync(
      GitPullRequestStatus status,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestIterationStatusAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestIterationStatusAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestIterationStatusesAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestIterationStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("75cf11c5-979f-4038-a76e-058a06adf2bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      string project,
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      string project,
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      Guid project,
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WebApiTagDefinition> CreatePullRequestLabelAsync(
      WebApiCreateTagRequestData label,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiCreateTagRequestData>(label, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestLabelsAsync(
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      using (await gitHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      string repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTagDefinition> GetPullRequestLabelAsync(
      Guid repositoryId,
      int pullRequestId,
      string labelIdOrName,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        labelIdOrName = labelIdOrName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<WebApiTagDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      string repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTagDefinition>> GetPullRequestLabelsAsync(
      Guid repositoryId,
      int pullRequestId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f22387e3-984e-4c52-9c6d-fbb8f14c812d");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<WebApiTagDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetPullRequestPropertiesAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> UpdatePullRequestPropertiesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48a52185-5b9e-4736-9dc1-bb1e2feac80b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestQuery> GetPullRequestQueryAsync(
      GitPullRequestQuery queries,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b3a6eebe-9cf0-49ea-b6cb-1a4c5f5007b0");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestQuery>(queries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRefWithVote>> CreatePullRequestReviewersAsync(
      IdentityRef[] reviewers,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRef[]>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<IdentityRefWithVote>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> CreateUnmaterializedPullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestReviewerAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> GetPullRequestReviewerAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IdentityRefWithVote>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRefWithVote>> GetPullRequestReviewersAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRefWithVote>>(new HttpMethod("GET"), new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      string project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      string repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRefWithVote> UpdatePullRequestReviewerAsync(
      IdentityRefWithVote reviewer,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      string reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityRefWithVote>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRefWithVote>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestReviewersAsync(
      IEnumerable<IdentityRefWithVote> patchVotes,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4b6702c7-aa35-4b89-9c96-b9abf6d3e540");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRefWithVote>>(patchVotes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<GitPullRequest> GetPullRequestByIdAsync(
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequest>(new HttpMethod("GET"), new Guid("01a46dea-7d46-4d40-bc84-319e7c260d99"), (object) new
      {
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestByIdAsync(
      string project,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequest>(new HttpMethod("GET"), new Guid("01a46dea-7d46-4d40-bc84-319e7c260d99"), (object) new
      {
        project = project,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestByIdAsync(
      Guid project,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequest>(new HttpMethod("GET"), new Guid("01a46dea-7d46-4d40-bc84-319e7c260d99"), (object) new
      {
        project = project,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsByProjectAsync(
      string project,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d28130-9cd2-40fa-9f08-902e7daa9efb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsByProjectAsync(
      Guid project,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d28130-9cd2-40fa-9f08-902e7daa9efb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string project,
      string repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string project,
      Guid repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid project,
      string repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid project,
      Guid repositoryId,
      bool? supportsIterations = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (supportsIterations.HasValue)
        collection.Add(nameof (supportsIterations), supportsIterations.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      string repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> GetPullRequestAsync(
      Guid repositoryId,
      int pullRequestId,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      bool? includeCommits = null,
      bool? includeWorkItemRefs = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      bool flag;
      if (includeCommits.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeCommits.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCommits), str);
      }
      if (includeWorkItemRefs.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeWorkItemRefs.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItemRefs), str);
      }
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      string project,
      string repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      string project,
      Guid repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      Guid project,
      string repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      Guid project,
      Guid repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      string repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequest>> GetPullRequestsAsync(
      Guid repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<GitPullRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequest> UpdatePullRequestAsync(
      GitPullRequest gitPullRequestToUpdate,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9946fd70-0d40-406e-b686-b4744cbbcc37");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequest>(gitPullRequestToUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SharePullRequestAsync(
      ShareNotificationContext userMessage,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("696f3a82-47c9-487f-9117-b9d00972ca84");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ShareNotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestStatus> CreatePullRequestStatusAsync(
      GitPullRequestStatus status,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestStatus>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeletePullRequestStatusAsync(
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestStatusAsync(
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestStatusAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestStatusAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestStatusAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePullRequestStatusAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestStatus> GetPullRequestStatusAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitPullRequestStatus>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestStatus>> GetPullRequestStatusesAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitPullRequestStatus>>(new HttpMethod("GET"), new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePullRequestStatusesAsync(
      JsonPatchDocument patchDocument,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b5f6bb4f-8d1e-4d79-8d11-4c9172c99c35");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GitHttpClientBase gitHttpClientBase2 = gitHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await gitHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> CreateCommentAsync(
      Comment comment,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteCommentAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Comment> GetCommentAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Comment>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Comment>> GetCommentsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Comment>>(new HttpMethod("GET"), new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      Comment comment,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("965a3ec7-5ed8-455a-bdcb-835a5ea7fe7b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Comment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> CreateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      string repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> GetPullRequestThreadAsync(
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      string repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPullRequestCommentThread>> GetThreadsAsync(
      Guid repositoryId,
      int pullRequestId,
      int? iteration = null,
      int? baseIteration = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (iteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = iteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$iteration", str);
      }
      if (baseIteration.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = baseIteration.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$baseIteration", str);
      }
      return this.SendAsync<List<GitPullRequestCommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      string project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid project,
      string repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPullRequestCommentThread> UpdateThreadAsync(
      GitPullRequestCommentThread commentThread,
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ab6e2e5d-a0b7-4153-b64a-a4efe0d49449");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPullRequestCommentThread>(commentThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequestCommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ResourceRef>> GetPullRequestWorkItemRefsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceRef>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> CreatePushAsync(
      GitPush push,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitPush>(push, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPush>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitPush> GetPushAsync(
      string project,
      string repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> GetPushAsync(
      string project,
      Guid repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> GetPushAsync(
      Guid project,
      string repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> GetPushAsync(
      Guid project,
      Guid repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> GetPushAsync(
      string repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitPush> GetPushAsync(
      Guid repositoryId,
      int pushId,
      int? includeCommits = null,
      bool? includeRefUpdates = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        pushId = pushId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeCommits.HasValue)
        keyValuePairList.Add(nameof (includeCommits), includeCommits.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRefUpdates.HasValue)
        keyValuePairList.Add(nameof (includeRefUpdates), includeRefUpdates.Value.ToString());
      return this.SendAsync<GitPush>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      string project,
      string repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      string project,
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      Guid project,
      string repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      Guid project,
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      string repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitPush>> GetPushesAsync(
      Guid repositoryId,
      int? skip = null,
      int? top = null,
      GitPushSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ea98d07b-3c87-4971-8ede-a613694ffb55");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<GitPush>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteRepositoryFromRecycleBinAsync(
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a663da97-81db-4eb3-8b83-287670f63073"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteRepositoryFromRecycleBinAsync(
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a663da97-81db-4eb3-8b83-287670f63073"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<GitDeletedRepository>> GetRecycleBinRepositoriesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitDeletedRepository>>(new HttpMethod("GET"), new Guid("a663da97-81db-4eb3-8b83-287670f63073"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitDeletedRepository>> GetRecycleBinRepositoriesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitDeletedRepository>>(new HttpMethod("GET"), new Guid("a663da97-81db-4eb3-8b83-287670f63073"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> RestoreRepositoryFromRecycleBinAsync(
      GitRecycleBinRepositoryDetails repositoryDetails,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a663da97-81db-4eb3-8b83-287670f63073");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRecycleBinRepositoryDetails>(repositoryDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> RestoreRepositoryFromRecycleBinAsync(
      GitRecycleBinRepositoryDetails repositoryDetails,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a663da97-81db-4eb3-8b83-287670f63073");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRecycleBinRepositoryDetails>(repositoryDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      string project,
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      string project,
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid project,
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid project,
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null)
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeStatuses.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeStatuses.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeStatuses), str);
      }
      if (includeMyBranches.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeMyBranches.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeMyBranches), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      if (peelTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = peelTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (peelTags), str);
      }
      if (filterContains != null)
        keyValuePairList.Add(nameof (filterContains), filterContains);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      string project,
      string repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      string project,
      Guid repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      Guid project,
      string repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      Guid project,
      Guid repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      string repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRef> UpdateRefAsync(
      GitRefUpdate newRefInfo,
      Guid repositoryId,
      string filter,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefUpdate>(newRefInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (filter), filter);
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRef>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      string repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      Guid repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      string project,
      string repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      string project,
      Guid repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      Guid project,
      string repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<GitRefUpdateResult>> UpdateRefsAsync(
      IEnumerable<GitRefUpdate> refUpdates,
      Guid project,
      Guid repositoryId,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d874a60-a811-4f62-9c9f-963a6ea0a55b");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitRefUpdate>>(refUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        collection.Add(nameof (projectId), projectId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitRefUpdateResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRefFavorite> CreateFavoriteAsync(
      GitRefFavorite favorite,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefFavorite>(favorite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRefFavorite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRefFavorite> CreateFavoriteAsync(
      GitRefFavorite favorite,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRefFavorite>(favorite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRefFavorite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteRefFavoriteAsync(
      string project,
      int favoriteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb"), (object) new
      {
        project = project,
        favoriteId = favoriteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteRefFavoriteAsync(
      Guid project,
      int favoriteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb"), (object) new
      {
        project = project,
        favoriteId = favoriteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<GitRefFavorite> GetRefFavoriteAsync(
      string project,
      int favoriteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRefFavorite>(new HttpMethod("GET"), new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb"), (object) new
      {
        project = project,
        favoriteId = favoriteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRefFavorite> GetRefFavoriteAsync(
      Guid project,
      int favoriteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRefFavorite>(new HttpMethod("GET"), new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb"), (object) new
      {
        project = project,
        favoriteId = favoriteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRefFavorite>> GetRefFavoritesAsync(
      string project,
      string repositoryId = null,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<GitRefFavorite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRefFavorite>> GetRefFavoritesAsync(
      Guid project,
      string repositoryId = null,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("876f70af-5792-485a-a1c7-d0a7b2f42bbb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<GitRefFavorite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRefFavorite>> GetRefFavoritesForProjectAsync(
      string project,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4720896c-594c-4a6d-b94c-12eddd80b34a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<GitRefFavorite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRefFavorite>> GetRefFavoritesForProjectAsync(
      Guid project,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4720896c-594c-4a6d-b94c-12eddd80b34a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<GitRefFavorite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepositoryCreateOptions gitRepositoryToCreate,
      string sourceRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(gitRepositoryToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (sourceRef != null)
        collection.Add(nameof (sourceRef), sourceRef);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepositoryCreateOptions gitRepositoryToCreate,
      string project,
      string sourceRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(gitRepositoryToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (sourceRef != null)
        collection.Add(nameof (sourceRef), sourceRef);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepositoryCreateOptions gitRepositoryToCreate,
      Guid project,
      string sourceRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(gitRepositoryToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (sourceRef != null)
        collection.Add(nameof (sourceRef), sourceRef);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task DeleteRepositoryAsync(
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteRepositoryAsync(
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteRepositoryAsync(
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      string project,
      bool? includeLinks = null,
      bool? includeAllUrls = null,
      bool? includeHidden = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeAllUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllUrls), str);
      }
      if (includeHidden.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeHidden.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeHidden), str);
      }
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      Guid project,
      bool? includeLinks = null,
      bool? includeAllUrls = null,
      bool? includeHidden = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeAllUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllUrls), str);
      }
      if (includeHidden.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeHidden.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeHidden), str);
      }
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      bool? includeLinks = null,
      bool? includeAllUrls = null,
      bool? includeHidden = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (includeAllUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllUrls), str);
      }
      if (includeHidden.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeHidden.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeHidden), str);
      }
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRepository>(new HttpMethod("GET"), new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      string project,
      string repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      string project,
      Guid repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      Guid project,
      string repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      Guid project,
      Guid repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      string repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryWithParentAsync(
      Guid repositoryId,
      bool includeParent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeParent), includeParent.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> UpdateRepositoryAsync(
      GitRepository newRepositoryInfo,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new
      {
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepository>(newRepositoryInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> UpdateRepositoryAsync(
      GitRepository newRepositoryInfo,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepository>(newRepositoryInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> UpdateRepositoryAsync(
      GitRepository newRepositoryInfo,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepository>(newRepositoryInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      string project,
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      string project,
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      Guid project,
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> GetRevertConflictAsync(
      Guid project,
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitConflict>(new HttpMethod("GET"), new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      string project,
      string repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      string project,
      Guid repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      Guid project,
      string repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      Guid project,
      Guid repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      string repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetRevertConflictsAsync(
      Guid repositoryId,
      int revertId,
      string continuationToken = null,
      int? top = null,
      bool? excludeResolved = null,
      bool? onlyResolved = null,
      bool? includeObsolete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (excludeResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeResolved), str);
      }
      if (onlyResolved.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = onlyResolved.Value;
        string str = flag.ToString();
        collection.Add(nameof (onlyResolved), str);
      }
      if (includeObsolete.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeObsolete.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeObsolete), str);
      }
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      string project,
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      string project,
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      Guid project,
      string repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<GitConflict> UpdateRevertConflictAsync(
      GitConflict conflict,
      Guid project,
      Guid repositoryId,
      int revertId,
      int conflictId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId,
        conflictId = conflictId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitConflict>(conflict, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitConflict>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      string repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      string project,
      Guid repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      string repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflictUpdateResult>> UpdateRevertConflictsAsync(
      IEnumerable<GitConflict> conflictUpdates,
      Guid project,
      Guid repositoryId,
      int revertId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("10d7ae6d-1050-446d-852a-bd5d99f834bf");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        revertId = revertId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GitConflict>>(conflictUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitConflictUpdateResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRevert> CreateRevertAsync(
      GitAsyncRefOperationParameters revertToCreate,
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(revertToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRevert>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRevert> CreateRevertAsync(
      GitAsyncRefOperationParameters revertToCreate,
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(revertToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRevert>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRevert> CreateRevertAsync(
      GitAsyncRefOperationParameters revertToCreate,
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(revertToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRevert>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRevert> CreateRevertAsync(
      GitAsyncRefOperationParameters revertToCreate,
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object obj1 = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitAsyncRefOperationParameters>(revertToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRevert>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRevert> GetRevertAsync(
      string project,
      int revertId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRevert>(new HttpMethod("GET"), new Guid("bc866058-5449-4715-9cf1-a510b6ff193c"), (object) new
      {
        project = project,
        revertId = revertId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertAsync(
      string project,
      int revertId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRevert>(new HttpMethod("GET"), new Guid("bc866058-5449-4715-9cf1-a510b6ff193c"), (object) new
      {
        project = project,
        revertId = revertId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertAsync(
      Guid project,
      int revertId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRevert>(new HttpMethod("GET"), new Guid("bc866058-5449-4715-9cf1-a510b6ff193c"), (object) new
      {
        project = project,
        revertId = revertId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertAsync(
      Guid project,
      int revertId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GitRevert>(new HttpMethod("GET"), new Guid("bc866058-5449-4715-9cf1-a510b6ff193c"), (object) new
      {
        project = project,
        revertId = revertId,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertForRefNameAsync(
      string project,
      string repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitRevert>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertForRefNameAsync(
      string project,
      Guid repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitRevert>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertForRefNameAsync(
      Guid project,
      string repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitRevert>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRevert> GetRevertForRefNameAsync(
      Guid project,
      Guid repositoryId,
      string refName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc866058-5449-4715-9cf1-a510b6ff193c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (refName), refName);
      return this.SendAsync<GitRevert>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      string commitId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      string commitId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      string project,
      string commitId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      string project,
      string commitId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      Guid project,
      string commitId,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitStatus> CreateCommitStatusAsync(
      GitStatus gitCommitStatusToCreate,
      Guid project,
      string commitId,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object obj1 = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitStatus>(gitCommitStatusToCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitStatus>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      string project,
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      string project,
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      Guid project,
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      Guid project,
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        project = project,
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      string commitId,
      string repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitStatus>> GetStatusesAsync(
      string commitId,
      Guid repositoryId,
      int? top = null,
      int? skip = null,
      bool? latestOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("428dd4fb-fda5-4722-af02-9313b80305da");
      object routeValues = (object) new
      {
        commitId = commitId,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      if (latestOnly.HasValue)
        keyValuePairList.Add(nameof (latestOnly), latestOnly.Value.ToString());
      return this.SendAsync<List<GitStatus>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string project,
      string repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string project,
      Guid repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid project,
      string repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid project,
      Guid repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid repositoryId,
      bool? preferCompareBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9393b4fb-4445-4919-972b-9ad16f442d83");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (preferCompareBranch.HasValue)
        keyValuePairList.Add(nameof (preferCompareBranch), preferCompareBranch.Value.ToString());
      return this.SendAsync<List<GitSuggestion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<GitTreeDiffResponse> GetTreeDiffsAsync(
      string project,
      string repositoryId,
      string baseId = null,
      string targetId = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e264ef02-4e92-4cfc-a4b1-5e71894d7b31");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseId != null)
        keyValuePairList.Add(nameof (baseId), baseId);
      if (targetId != null)
        keyValuePairList.Add(nameof (targetId), targetId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitTreeDiffResponse treeDiffsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitTreeDiffResponse returnObject = new GitTreeDiffResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          GitTreeDiffResponse treeDiffResponse = returnObject;
          treeDiffResponse.TreeDiff = await gitHttpClientBase.ReadContentAsAsync<GitTreeDiff>(response, cancellationToken).ConfigureAwait(false);
          treeDiffResponse = (GitTreeDiffResponse) null;
        }
        treeDiffsAsync = returnObject;
      }
      return treeDiffsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<GitTreeDiffResponse> GetTreeDiffsAsync(
      string project,
      Guid repositoryId,
      string baseId = null,
      string targetId = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e264ef02-4e92-4cfc-a4b1-5e71894d7b31");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseId != null)
        keyValuePairList.Add(nameof (baseId), baseId);
      if (targetId != null)
        keyValuePairList.Add(nameof (targetId), targetId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitTreeDiffResponse treeDiffsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitTreeDiffResponse returnObject = new GitTreeDiffResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          GitTreeDiffResponse treeDiffResponse = returnObject;
          treeDiffResponse.TreeDiff = await gitHttpClientBase.ReadContentAsAsync<GitTreeDiff>(response, cancellationToken).ConfigureAwait(false);
          treeDiffResponse = (GitTreeDiffResponse) null;
        }
        treeDiffsAsync = returnObject;
      }
      return treeDiffsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<GitTreeDiffResponse> GetTreeDiffsAsync(
      Guid project,
      string repositoryId,
      string baseId = null,
      string targetId = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e264ef02-4e92-4cfc-a4b1-5e71894d7b31");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseId != null)
        keyValuePairList.Add(nameof (baseId), baseId);
      if (targetId != null)
        keyValuePairList.Add(nameof (targetId), targetId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitTreeDiffResponse treeDiffsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitTreeDiffResponse returnObject = new GitTreeDiffResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          GitTreeDiffResponse treeDiffResponse = returnObject;
          treeDiffResponse.TreeDiff = await gitHttpClientBase.ReadContentAsAsync<GitTreeDiff>(response, cancellationToken).ConfigureAwait(false);
          treeDiffResponse = (GitTreeDiffResponse) null;
        }
        treeDiffsAsync = returnObject;
      }
      return treeDiffsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<GitTreeDiffResponse> GetTreeDiffsAsync(
      Guid project,
      Guid repositoryId,
      string baseId = null,
      string targetId = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e264ef02-4e92-4cfc-a4b1-5e71894d7b31");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (baseId != null)
        keyValuePairList.Add(nameof (baseId), baseId);
      if (targetId != null)
        keyValuePairList.Add(nameof (targetId), targetId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      GitTreeDiffResponse treeDiffsAsync;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        GitTreeDiffResponse returnObject = new GitTreeDiffResponse();
        using (HttpResponseMessage response = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = gitHttpClientBase.GetHeaderValue(response, "x-ms-continuationtoken");
          GitTreeDiffResponse treeDiffResponse = returnObject;
          treeDiffResponse.TreeDiff = await gitHttpClientBase.ReadContentAsAsync<GitTreeDiff>(response, cancellationToken).ConfigureAwait(false);
          treeDiffResponse = (GitTreeDiffResponse) null;
        }
        treeDiffsAsync = returnObject;
      }
      return treeDiffsAsync;
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      string project,
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      string project,
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      Guid project,
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitTreeRef> GetTreeAsync(
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitTreeRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      string project,
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      string project,
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      Guid project,
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      string repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTreeZipAsync(
      Guid repositoryId,
      string sha1,
      string projectId = null,
      bool? recursive = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitHttpClientBase gitHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("729f6437-6f92-44ec-8bee-273a7111063c");
      object routeValues = (object) new
      {
        repositoryId = repositoryId,
        sha1 = sha1
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (projectId != null)
        keyValuePairList.Add(nameof (projectId), projectId);
      if (recursive.HasValue)
        keyValuePairList.Add(nameof (recursive), recursive.Value.ToString());
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await gitHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await gitHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
