// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IReleaseHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface IReleaseHttpClientWrapper
  {
    Release GetRelease(Guid projectId, int releaseId);

    List<Release> GetReleases(
      Guid projectId,
      int definitionId,
      int? definitionEnvironmentId,
      ReleaseStatus statusFilter,
      DateTime maxCreatedTime,
      DateTime minCreatedTime,
      ReleaseQueryOrder queryOrder,
      ReleaseExpands expand,
      int? environmentStatusFilter = null,
      string sourceBranchFilter = null);

    List<ReleaseDefinition> GetReleaseDefinitions(Guid projectId, ReleaseDefinitionExpands expand);
  }
}
