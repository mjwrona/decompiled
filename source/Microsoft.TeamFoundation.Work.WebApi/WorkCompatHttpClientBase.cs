// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.WorkCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  public abstract class WorkCompatHttpClientBase : VssHttpClientBase
  {
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<BoardFilterSettings> GetBoardFilterSettingsAsync(
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new VssServiceResponseException(HttpStatusCode.BadRequest, AgileResources.NotAvailable, (Exception) new NotSupportedException());
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<BoardFilterSettings> UpdateBoardFilterSettingsAsync(
      BoardFilterSettings filterSettings,
      TeamContext teamContext,
      string board,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new VssServiceResponseException(HttpStatusCode.BadRequest, AgileResources.NotAvailable, (Exception) new NotSupportedException());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TeamMemberCapacity>> GetCapacitiesAsync(
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
      return this.SendAsync<List<TeamMemberCapacity>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TeamMemberCapacity> GetCapacityAsync(
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
      return this.SendAsync<TeamMemberCapacity>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TeamMemberCapacity>> ReplaceCapacitiesAsync(
      IEnumerable<TeamMemberCapacity> capacities,
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
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TeamMemberCapacity>>(capacities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TeamMemberCapacity>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TeamMemberCapacity> UpdateCapacityAsync(
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
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TeamMemberCapacity>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TeamMemberCapacityIdentityRef>> GetCapacitiesWithIdentityRefAsync(
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
      return this.SendAsync<List<TeamMemberCapacityIdentityRef>>(method, locationId, routeValues, new ApiResourceVersion(6.1, 2), userState: userState, cancellationToken: cancellationToken);
    }

    protected WorkCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    protected WorkCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    protected WorkCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    protected WorkCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected WorkCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }
  }
}
