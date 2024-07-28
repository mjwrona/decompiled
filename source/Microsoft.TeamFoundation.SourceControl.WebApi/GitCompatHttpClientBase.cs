// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitCompatHttpClientBase : VssHttpClientBase
  {
    public GitCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GitCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GitCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GitCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GitCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      string project,
      bool? includeLinks,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      string project,
      bool? includeLinks,
      object userState)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      Guid project,
      bool? includeLinks,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      Guid project,
      bool? includeLinks,
      object userState)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      bool? includeLinks,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      bool? includeLinks,
      object userState)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string project,
      string repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      string project,
      Guid repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid project,
      string repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitPullRequest> CreatePullRequestAsync(
      GitPullRequest gitPullRequestToCreate,
      Guid project,
      Guid repositoryId,
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
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.2-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitPullRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string project,
      string repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string project,
      Guid repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid project,
      string repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid project,
      Guid repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      string repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> GetRepositoryAsync(
      Guid repositoryId,
      bool? includeParent = null,
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
      if (includeParent.HasValue)
        keyValuePairList.Add(nameof (includeParent), includeParent.Value.ToString());
      return this.SendAsync<GitRepository>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepository gitRepositoryToCreate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(this.ConvertGitRepository(gitRepositoryToCreate), (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("4.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepository gitRepositoryToCreate,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(this.ConvertGitRepository(gitRepositoryToCreate), (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("4.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<GitRepository> CreateRepositoryAsync(
      GitRepository gitRepositoryToCreate,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("225f7195-f9c7-4d14-ab28-a83f7ff77e1f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitRepositoryCreateOptions>(this.ConvertGitRepository(gitRepositoryToCreate), (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("4.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GitRepository>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      string project,
      bool? includeLinks,
      bool? includeAllUrls,
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
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      Guid project,
      bool? includeLinks,
      bool? includeAllUrls,
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
      return this.SendAsync<List<GitRepository>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRepository>> GetRepositoriesAsync(
      bool? includeLinks,
      bool? includeAllUrls,
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
      return this.SendAsync<List<GitRepository>>(method, locationId, version: new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    private GitRepositoryCreateOptions ConvertGitRepository(GitRepository repository)
    {
      if (repository == null)
        return (GitRepositoryCreateOptions) null;
      return new GitRepositoryCreateOptions()
      {
        Name = repository.Name,
        ProjectReference = repository.ProjectReference
      };
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      string repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitConflict>> GetPullRequestConflictsAsync(
      Guid repositoryId,
      int pullRequestId,
      int? skip = null,
      int? top = null,
      bool? includeObsolete = null,
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
      if (includeObsolete.HasValue)
        keyValuePairList.Add(nameof (includeObsolete), includeObsolete.Value.ToString());
      return this.SendAsync<List<GitConflict>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
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
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitItem> GetItemAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
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
      return this.SendAsync<GitItem>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb93c0db-47ed-4a31-8c20-47552878fb44");
      object routeValues = (object) new
      {
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(scopePath))
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid repositoryId,
      string path,
      string scopePath,
      VersionControlRecursionType? recursionLevel,
      bool? includeContentMetadata,
      bool? latestProcessedChange,
      bool? download,
      GitVersionDescriptor versionDescriptor,
      bool? includeContent,
      bool? resolveLfs,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      if (resolveLfs.HasValue)
        keyValuePairList.Add(nameof (resolveLfs), resolveLfs.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<AssociatedWorkItem>> GetPullRequestWorkItemsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("0a637fcc-5370-4ce8-b0e8-98091f5f9482"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRef>> GetRefsAsync(
      string project,
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitRef>> GetRefsAsync(
      string project,
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid project,
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid project,
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRef>> GetRefsAsync(
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? latestStatusesOnly = null,
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
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add(nameof (filter), filter);
      bool flag;
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      if (latestStatusesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = latestStatusesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (latestStatusesOnly), str);
      }
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRef>> GetRefsAsync(
      string repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitRef>> GetRefsAsync(
      Guid repositoryId,
      string filter = null,
      bool? includeLinks = null,
      bool? includeStatuses = null,
      bool? includeMyBranches = null,
      bool? latestStatusesOnly = null,
      bool? peelTags = null,
      string filterContains = null,
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
      return this.SendAsync<List<GitRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<GitBlobRef> GetBlobAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
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
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      return this.SendAsync<GitBlobRef>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobContentAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      string project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      string project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      Guid project,
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      Guid project,
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      string repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetBlobZipAsync(
      Guid repositoryId,
      string sha1,
      bool? download = null,
      string fileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GitCompatHttpClientBase compatHttpClientBase = this;
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
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      string project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid project,
      string repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitCommitRef>> GetPullRequestIterationCommitsAsync(
      Guid project,
      Guid repositoryId,
      int pullRequestId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitCommitRef>>(new HttpMethod("GET"), new Guid("e7ea0883-095f-4926-b5fb-f24691c26fb9"), (object) new
      {
        project = project,
        repositoryId = repositoryId,
        pullRequestId = pullRequestId,
        iterationId = iterationId
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitItem>> GetItemsAsync(
      string repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<GitItem>> GetItemsAsync(
      Guid repositoryId,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContentMetadata = null,
      bool? latestProcessedChange = null,
      bool? download = null,
      bool? includeLinks = null,
      GitVersionDescriptor versionDescriptor = null,
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
      return this.SendAsync<List<GitItem>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      string project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid project,
      string repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GitSuggestion>> GetSuggestionsAsync(
      Guid project,
      Guid repositoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitSuggestion>>(new HttpMethod("GET"), new Guid("9393b4fb-4445-4919-972b-9ad16f442d83"), (object) new
      {
        project = project,
        repositoryId = repositoryId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
