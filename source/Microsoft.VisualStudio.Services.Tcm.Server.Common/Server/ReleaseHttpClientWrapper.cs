// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ReleaseHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ReleaseHttpClientWrapper : IReleaseHttpClientWrapper
  {
    private readonly ReleaseHttpClient _client;

    public ReleaseHttpClientWrapper(ReleaseHttpClient client) => this._client = client;

    public Release GetRelease(Guid projectId, int releaseId)
    {
      ReleaseHttpClient client = this._client;
      Guid project = projectId;
      int releaseId1 = releaseId;
      SingleReleaseExpands? nullable = new SingleReleaseExpands?(SingleReleaseExpands.None);
      ApprovalFilters? approvalFilters = new ApprovalFilters?();
      SingleReleaseExpands? expand = nullable;
      int? topGateRecords = new int?();
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetReleaseAsync(project, releaseId1, approvalFilters, expand: expand, topGateRecords: topGateRecords, cancellationToken: cancellationToken).Result;
    }

    public List<Release> GetReleases(
      Guid projectId,
      int definitionId,
      int? definitionEnvironmentId,
      ReleaseStatus statusFilter,
      DateTime maxCreatedTime,
      DateTime minCreatedTime,
      ReleaseQueryOrder queryOrder,
      ReleaseExpands expand,
      int? environmentStatusFilter = null,
      string sourceBranchFilter = null)
    {
      ReleaseHttpClient client = this._client;
      Guid project = projectId;
      int? definitionId1 = new int?(definitionId);
      int? definitionEnvironmentId1 = definitionEnvironmentId;
      ReleaseStatus? statusFilter1 = new ReleaseStatus?(statusFilter);
      DateTime? nullable1 = new DateTime?(maxCreatedTime);
      DateTime? nullable2 = new DateTime?(minCreatedTime);
      ReleaseQueryOrder? nullable3 = new ReleaseQueryOrder?(queryOrder);
      ReleaseExpands? nullable4 = new ReleaseExpands?(expand);
      int? environmentStatusFilter1 = environmentStatusFilter;
      DateTime? minCreatedTime1 = nullable2;
      DateTime? maxCreatedTime1 = nullable1;
      ReleaseQueryOrder? queryOrder1 = nullable3;
      string str = sourceBranchFilter;
      int? top = new int?();
      int? continuationToken = new int?();
      ReleaseExpands? expand1 = nullable4;
      string sourceBranchFilter1 = str;
      bool? isDeleted = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetReleasesAsync(project, definitionId1, definitionEnvironmentId1, (string) null, (string) null, statusFilter1, environmentStatusFilter1, minCreatedTime1, maxCreatedTime1, queryOrder1, top, continuationToken, expand1, (string) null, (string) null, (string) null, sourceBranchFilter1, isDeleted, (IEnumerable<string>) null, (IEnumerable<string>) null, (IEnumerable<int>) null, (string) null, (object) null, cancellationToken).Result;
    }

    public List<ReleaseDefinition> GetReleaseDefinitions(
      Guid projectId,
      ReleaseDefinitionExpands expand)
    {
      return this._client.GetReleaseDefinitionsAsync(projectId, (string) null, new ReleaseDefinitionExpands?(expand), (string) null, (string) null, new int?(), (string) null, new ReleaseDefinitionQueryOrder?(), (string) null, new bool?(), (IEnumerable<string>) null, (IEnumerable<string>) null, (IEnumerable<string>) null, new bool?(), new bool?(), (object) null, new CancellationToken()).Result;
    }
  }
}
