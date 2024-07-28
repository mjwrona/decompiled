// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.WorkHttpClientBase
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi.Contracts;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.AutomationRules;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [ResourceArea("1D4F49F9-02B9-4E26-B826-2CDB6195F2A9")]
  public abstract class WorkHttpClientBase : WorkCompatHttpClientBase
  {
    public WorkHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateAutomationRuleAsync(
      TeamAutomationRulesSettingsRequestModel ruleRequestModel,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkHttpClientBase workHttpClientBase1 = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2882c15d-0cb3-43b5-8fb7-db62e09a79db");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TeamAutomationRulesSettingsRequestModel>(ruleRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkHttpClientBase workHttpClientBase2 = workHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await workHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<BacklogConfiguration> GetBacklogConfigurationsAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7799f497-3cb5-4f16-ad4f-5cd06012db64");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<BacklogConfiguration>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BacklogLevelWorkItems> GetBacklogLevelWorkItemsAsync(
      TeamContext teamContext,
      string backlogId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c468d96-ab1d-4294-a360-92f07e9ccd98");
      string str3 = str2;
      string str4 = backlogId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        backlogId = str4
      };
      return this.SendAsync<BacklogLevelWorkItems>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BacklogLevelConfiguration> GetBacklogAsync(
      TeamContext teamContext,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a93726f9-7867-4e38-b4f2-0bfafc2f6a94");
      string str3 = str2;
      string str4 = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = str4
      };
      return this.SendAsync<BacklogLevelConfiguration>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BacklogLevelConfiguration>> GetBacklogsAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a93726f9-7867-4e38-b4f2-0bfafc2f6a94");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<List<BacklogLevelConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<BoardBadge> GetBoardBadgeAsync(
      TeamContext teamContext,
      Guid id,
      BoardBadgeColumnOptions? columnOptions = null,
      IEnumerable<string> columns = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0120b002-ab6c-4ca0-98cf-a8d7492f865c");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (columnOptions.HasValue)
        keyValuePairList.Add(nameof (columnOptions), columnOptions.Value.ToString());
      if (columns != null && columns.Any<string>())
        keyValuePairList.Add(nameof (columns), string.Join(",", columns));
      return this.SendAsync<BoardBadge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GetBoardBadgeDataAsync(
      TeamContext teamContext,
      Guid id,
      BoardBadgeColumnOptions? columnOptions = null,
      IEnumerable<string> columns = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0120b002-ab6c-4ca0-98cf-a8d7492f865c");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (columnOptions.HasValue)
        keyValuePairList.Add(nameof (columnOptions), columnOptions.Value.ToString());
      if (columns != null && columns.Any<string>())
        keyValuePairList.Add(nameof (columns), string.Join(",", columns));
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetColumnSuggestedValuesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("eb7ec5a3-1ba3-4fd1-b834-49a5a387e57d"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetColumnSuggestedValuesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("eb7ec5a3-1ba3-4fd1-b834-49a5a387e57d"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetColumnSuggestedValuesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("eb7ec5a3-1ba3-4fd1-b834-49a5a387e57d"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ParentChildWIMap>> GetBoardMappingParentItemsAsync(
      TeamContext teamContext,
      string childBacklogContextCategoryRefName,
      IEnumerable<int> workitemIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("186abea3-5c35-432f-9e28-7a15b4312a0e");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (childBacklogContextCategoryRefName), childBacklogContextCategoryRefName);
      string str4 = (string) null;
      if (workitemIds != null)
        str4 = string.Join<int>(",", workitemIds);
      keyValuePairList.Add(nameof (workitemIds), str4);
      return this.SendAsync<List<ParentChildWIMap>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetRowSuggestedValuesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("bb494cc6-a0f5-4c6c-8dca-ea6912e79eb9"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetRowSuggestedValuesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("bb494cc6-a0f5-4c6c-8dca-ea6912e79eb9"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardSuggestedValue>> GetRowSuggestedValuesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BoardSuggestedValue>>(new HttpMethod("GET"), new Guid("bb494cc6-a0f5-4c6c-8dca-ea6912e79eb9"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Board> GetBoardAsync(
      TeamContext teamContext,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("23ad19fc-3b8e-4877-8462-b3f92bc06b40");
      string str3 = str2;
      string str4 = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = str4
      };
      return this.SendAsync<Board>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardReference>> GetBoardsAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("23ad19fc-3b8e-4877-8462-b3f92bc06b40");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<List<BoardReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Dictionary<string, string>> SetBoardOptionsAsync(
      IDictionary<string, string> options,
      TeamContext teamContext,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("23ad19fc-3b8e-4877-8462-b3f92bc06b40");
      string str3 = str2;
      string str4 = id;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        id = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(options, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Dictionary<string, string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BoardUserSettings> GetBoardUserSettingsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b30d9f58-1891-4b0a-b168-c46408f919b0");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<BoardUserSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BoardUserSettings> UpdateBoardUserSettingsAsync(
      IDictionary<string, string> boardUserSettings,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b30d9f58-1891-4b0a-b168-c46408f919b0");
      string str3 = str2;
      string str4 = board;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(boardUserSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BoardUserSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TeamCapacity> GetCapacitiesWithIdentityRefAndTotalsAsync(
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("74412d15-8c1a-4352-a48d-ef1ed5587d57");
      string str3 = str2;
      Guid guid = iterationId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid
      };
      return this.SendAsync<TeamCapacity>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TeamMemberCapacityIdentityRef> GetCapacityWithIdentityRefAsync(
      TeamContext teamContext,
      Guid iterationId,
      Guid teamMemberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("74412d15-8c1a-4352-a48d-ef1ed5587d57");
      string str3 = str2;
      Guid guid1 = iterationId;
      Guid guid2 = teamMemberId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid1,
        teamMemberId = guid2
      };
      return this.SendAsync<TeamMemberCapacityIdentityRef>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TeamMemberCapacityIdentityRef>> ReplaceCapacitiesWithIdentityRefAsync(
      IEnumerable<TeamMemberCapacityIdentityRef> capacities,
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid1 = new Guid("74412d15-8c1a-4352-a48d-ef1ed5587d57");
      string str3 = str2;
      Guid guid2 = iterationId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TeamMemberCapacityIdentityRef>>(capacities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TeamMemberCapacityIdentityRef>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TeamMemberCapacityIdentityRef> UpdateCapacityWithIdentityRefAsync(
      CapacityPatch patch,
      TeamContext teamContext,
      Guid iterationId,
      Guid teamMemberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid1 = new Guid("74412d15-8c1a-4352-a48d-ef1ed5587d57");
      string str3 = str2;
      Guid guid2 = iterationId;
      Guid guid3 = teamMemberId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid2,
        teamMemberId = guid3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CapacityPatch>(patch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamMemberCapacityIdentityRef>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BoardCardRuleSettings> GetBoardCardRuleSettingsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b044a3d9-02ea-49c7-91a1-b730949cc896");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<BoardCardRuleSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BoardCardRuleSettings> UpdateBoardCardRuleSettingsAsync(
      BoardCardRuleSettings boardCardRuleSettings,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b044a3d9-02ea-49c7-91a1-b730949cc896");
      string str3 = str2;
      string str4 = board;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BoardCardRuleSettings>(boardCardRuleSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BoardCardRuleSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task UpdateTaskboardCardRuleSettingsAsync(
      BoardCardRuleSettings boardCardRuleSettings,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkHttpClientBase workHttpClientBase1 = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3f84a8d1-1aab-423e-a94b-6dcbdcca511f");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BoardCardRuleSettings>(boardCardRuleSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkHttpClientBase workHttpClientBase2 = workHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await workHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<BoardCardSettings> GetBoardCardSettingsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("07c3b467-bc60-4f05-8e34-599ce288fafc");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<BoardCardSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BoardCardSettings> UpdateBoardCardSettingsAsync(
      BoardCardSettings boardCardSettingsToSave,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("07c3b467-bc60-4f05-8e34-599ce288fafc");
      string str3 = str2;
      string str4 = board;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BoardCardSettings>(boardCardSettingsToSave, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BoardCardSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task UpdateTaskboardCardSettingsAsync(
      BoardCardSettings boardCardSettingsToSave,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkHttpClientBase workHttpClientBase1 = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0d63745f-31f3-4cf3-9056-2a064e567637");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BoardCardSettings>(boardCardSettingsToSave, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkHttpClientBase workHttpClientBase2 = workHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await workHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<StreamContent> GetBoardChartImageAsync(
      TeamContext teamContext,
      string board,
      string name,
      int? width = null,
      int? height = null,
      bool? showDetails = null,
      string title = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4ee4d042-64fa-4202-8ca6-dae1ab888985");
      string str3 = str2;
      string str4 = board;
      string str5 = name;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4,
        name = str5
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (width.HasValue)
        keyValuePairList.Add(nameof (width), width.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (height.HasValue)
        keyValuePairList.Add(nameof (height), height.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (showDetails.HasValue)
        keyValuePairList.Add(nameof (showDetails), showDetails.Value.ToString());
      if (title != null)
        keyValuePairList.Add(nameof (title), title);
      return this.SendAsync<StreamContent>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<StreamContent> GetIterationChartImageAsync(
      TeamContext teamContext,
      Guid iterationId,
      string name,
      int? width = null,
      int? height = null,
      bool? showDetails = null,
      string title = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8b94efc1-e022-469d-80aa-8d2ba1c21449");
      string str3 = str2;
      Guid guid = iterationId;
      string str4 = name;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid,
        name = str4
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (width.HasValue)
        keyValuePairList.Add(nameof (width), width.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (height.HasValue)
        keyValuePairList.Add(nameof (height), height.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (showDetails.HasValue)
        keyValuePairList.Add(nameof (showDetails), showDetails.Value.ToString());
      if (title != null)
        keyValuePairList.Add(nameof (title), title);
      return this.SendAsync<StreamContent>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<StreamContent> GetIterationsChartImageAsync(
      TeamContext teamContext,
      string name,
      int? iterationsNumber = null,
      int? width = null,
      int? height = null,
      bool? showDetails = null,
      string title = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("89436dcf-a56b-4f72-a42e-2afef39c88a5");
      string str3 = str2;
      string str4 = name;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        name = str4
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (iterationsNumber.HasValue)
        keyValuePairList.Add(nameof (iterationsNumber), iterationsNumber.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (width.HasValue)
        keyValuePairList.Add(nameof (width), width.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (height.HasValue)
        keyValuePairList.Add(nameof (height), height.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (showDetails.HasValue)
        keyValuePairList.Add(nameof (showDetails), showDetails.Value.ToString());
      if (title != null)
        keyValuePairList.Add(nameof (title), title);
      return this.SendAsync<StreamContent>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BoardChart> GetBoardChartAsync(
      TeamContext teamContext,
      string board,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45fe888c-239e-49fd-958c-df1a1ab21d97");
      string str3 = str2;
      string str4 = board;
      string str5 = name;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4,
        name = str5
      };
      return this.SendAsync<BoardChart>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardChartReference>> GetBoardChartsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45fe888c-239e-49fd-958c-df1a1ab21d97");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<List<BoardChartReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BoardChart> UpdateBoardChartAsync(
      BoardChart chart,
      TeamContext teamContext,
      string board,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("45fe888c-239e-49fd-958c-df1a1ab21d97");
      string str3 = str2;
      string str4 = board;
      string str5 = name;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4,
        name = str5
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BoardChart>(chart, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BoardChart>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<BoardColumn>> GetBoardColumnsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c555d7ff-84e1-47df-9923-a3fe0cd8751b");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<List<BoardColumn>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardColumn>> UpdateBoardColumnsAsync(
      IEnumerable<BoardColumn> boardColumns,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c555d7ff-84e1-47df-9923-a3fe0cd8751b");
      string str3 = str2;
      string str4 = board;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<BoardColumn>>(boardColumns, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<BoardColumn>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DeliveryViewData> GetDeliveryTimelineDataAsync(
      string project,
      string id,
      int? revision = null,
      DateTime? startDate = null,
      DateTime? endDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bdd0834e-101f-49f0-a6ae-509f384a12b4");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDate), startDate.Value);
      if (endDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (endDate), endDate.Value);
      return this.SendAsync<DeliveryViewData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeliveryViewData> GetDeliveryTimelineDataAsync(
      Guid project,
      string id,
      int? revision = null,
      DateTime? startDate = null,
      DateTime? endDate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bdd0834e-101f-49f0-a6ae-509f384a12b4");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (startDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDate), startDate.Value);
      if (endDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (endDate), endDate.Value);
      return this.SendAsync<DeliveryViewData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IterationCapacity> GetTotalIterationCapacitiesAsync(
      string project,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IterationCapacity>(new HttpMethod("GET"), new Guid("1e385ce0-396b-4273-8171-d64562c18d37"), (object) new
      {
        project = project,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IterationCapacity> GetTotalIterationCapacitiesAsync(
      Guid project,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<IterationCapacity>(new HttpMethod("GET"), new Guid("1e385ce0-396b-4273-8171-d64562c18d37"), (object) new
      {
        project = project,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteTeamIterationAsync(
      TeamContext teamContext,
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkHttpClientBase workHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("c9175577-28a1-4b06-9197-8636af9f64ad");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      using (await workHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TeamSettingsIteration> GetTeamIterationAsync(
      TeamContext teamContext,
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c9175577-28a1-4b06-9197-8636af9f64ad");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      return this.SendAsync<TeamSettingsIteration>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TeamSettingsIteration>> GetTeamIterationsAsync(
      TeamContext teamContext,
      string timeframe = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c9175577-28a1-4b06-9197-8636af9f64ad");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timeframe != null)
        keyValuePairList.Add("$timeframe", timeframe);
      return this.SendAsync<List<TeamSettingsIteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TeamSettingsIteration> PostTeamIterationAsync(
      TeamSettingsIteration iteration,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c9175577-28a1-4b06-9197-8636af9f64ad");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TeamSettingsIteration>(iteration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamSettingsIteration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Plan> CreatePlanAsync(
      CreatePlan postedPlan,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePlan>(postedPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Plan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Plan> CreatePlanAsync(
      CreatePlan postedPlan,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePlan>(postedPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Plan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeletePlanAsync(
      string project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeletePlanAsync(
      Guid project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Plan> GetPlanAsync(
      string project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Plan>(new HttpMethod("GET"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Plan> GetPlanAsync(
      Guid project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Plan>(new HttpMethod("GET"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Plan>> GetPlansAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Plan>>(new HttpMethod("GET"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Plan>> GetPlansAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Plan>>(new HttpMethod("GET"), new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Plan> UpdatePlanAsync(
      UpdatePlan updatedPlan,
      string project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdatePlan>(updatedPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Plan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Plan> UpdatePlanAsync(
      UpdatePlan updatedPlan,
      Guid project,
      string id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0b42cb47-cd73-4810-ac90-19c9ba147453");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdatePlan>(updatedPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Plan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<PredefinedQuery>> GetPredefinedQueriesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PredefinedQuery>>(new HttpMethod("GET"), new Guid("9cbba37c-6cc6-4f70-b903-709be86acbf0"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<PredefinedQuery>> GetPredefinedQueriesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PredefinedQuery>>(new HttpMethod("GET"), new Guid("9cbba37c-6cc6-4f70-b903-709be86acbf0"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<PredefinedQuery> GetPredefinedQueryResultsAsync(
      string project,
      string id,
      int? top = null,
      bool? includeCompleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9cbba37c-6cc6-4f70-b903-709be86acbf0");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeCompleted.HasValue)
        keyValuePairList.Add(nameof (includeCompleted), includeCompleted.Value.ToString());
      return this.SendAsync<PredefinedQuery>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<PredefinedQuery> GetPredefinedQueryResultsAsync(
      Guid project,
      string id,
      int? top = null,
      bool? includeCompleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9cbba37c-6cc6-4f70-b903-709be86acbf0");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeCompleted.HasValue)
        keyValuePairList.Add(nameof (includeCompleted), includeCompleted.Value.ToString());
      return this.SendAsync<PredefinedQuery>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ProcessConfiguration> GetProcessConfigurationAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProcessConfiguration>(new HttpMethod("GET"), new Guid("f901ba42-86d2-4b0c-89c1-3f86d06daa84"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ProcessConfiguration> GetProcessConfigurationAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProcessConfiguration>(new HttpMethod("GET"), new Guid("f901ba42-86d2-4b0c-89c1-3f86d06daa84"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardRow>> GetBoardRowsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0863355d-aefd-4d63-8669-984c9b7b0e78");
      string str3 = str2;
      string str4 = board;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      return this.SendAsync<List<BoardRow>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BoardRow>> UpdateBoardRowsAsync(
      IEnumerable<BoardRow> boardRows,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0863355d-aefd-4d63-8669-984c9b7b0e78");
      string str3 = str2;
      string str4 = board;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        board = str4
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<BoardRow>>(boardRows, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<BoardRow>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskboardColumns> GetColumnsAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c6815dbe-8e7e-4ffe-9a79-e83ee712aa92");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<TaskboardColumns>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskboardColumns> UpdateColumnsAsync(
      IEnumerable<UpdateTaskboardColumn> updateColumns,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c6815dbe-8e7e-4ffe-9a79-e83ee712aa92");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<UpdateTaskboardColumn>>(updateColumns, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskboardColumns>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TaskboardWorkItemColumn>> GetWorkItemColumnsAsync(
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1be23c36-8872-4abc-b57d-402cd6c669d9");
      string str3 = str2;
      Guid guid = iterationId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid
      };
      return this.SendAsync<List<TaskboardWorkItemColumn>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateWorkItemColumnAsync(
      UpdateTaskboardWorkItemColumn updateColumn,
      TeamContext teamContext,
      Guid iterationId,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkHttpClientBase workHttpClientBase1 = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid1 = new Guid("1be23c36-8872-4abc-b57d-402cd6c669d9");
      string str3 = str2;
      Guid guid2 = iterationId;
      int num = workItemId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid2,
        workItemId = num
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTaskboardWorkItemColumn>(updateColumn, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkHttpClientBase workHttpClientBase2 = workHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await workHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<TeamSettingsDaysOff> GetTeamDaysOffAsync(
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2d4faa2e-9150-4cbf-a47a-932b1b4a0773");
      string str3 = str2;
      Guid guid = iterationId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid
      };
      return this.SendAsync<TeamSettingsDaysOff>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TeamSettingsDaysOff> UpdateTeamDaysOffAsync(
      TeamSettingsDaysOffPatch daysOffPatch,
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid1 = new Guid("2d4faa2e-9150-4cbf-a47a-932b1b4a0773");
      string str3 = str2;
      Guid guid2 = iterationId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TeamSettingsDaysOffPatch>(daysOffPatch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamSettingsDaysOff>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TeamFieldValues> GetTeamFieldValuesAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("07ced576-58ed-49e6-9c1e-5cb53ab8bf2a");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<TeamFieldValues>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TeamFieldValues> UpdateTeamFieldValuesAsync(
      TeamFieldValuesPatch patch,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07ced576-58ed-49e6-9c1e-5cb53ab8bf2a");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TeamFieldValuesPatch>(patch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamFieldValues>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TeamSetting> GetTeamSettingsAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c3c1012b-bea7-49d7-b45e-1664e566f84c");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<TeamSetting>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TeamSetting> UpdateTeamSettingsAsync(
      TeamSettingsPatch teamSettingsPatch,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c3c1012b-bea7-49d7-b45e-1664e566f84c");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TeamSettingsPatch>(teamSettingsPatch, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamSetting>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IterationWorkItems> GetIterationWorkItemsAsync(
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b3ef1a6-d3ab-44cd-bafd-c7f45db850fa");
      string str3 = str2;
      Guid guid = iterationId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid
      };
      return this.SendAsync<IterationWorkItems>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReorderResult>> ReorderBacklogWorkItemsAsync(
      ReorderOperation operation,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1c22b714-e7e4-41b9-85e0-56ee13ef55ed");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReorderOperation>(operation, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ReorderResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<ReorderResult>> ReorderIterationWorkItemsAsync(
      ReorderOperation operation,
      TeamContext teamContext,
      Guid iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid1 = new Guid("47755db2-d7eb-405a-8c25-675401525fc9");
      string str3 = str2;
      Guid guid2 = iterationId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        iterationId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReorderOperation>(operation, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ReorderResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
