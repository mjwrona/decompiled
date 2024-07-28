// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.IExternalArtifactService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  [DefaultServiceImplementation(typeof (ExternalArtifactService))]
  public interface IExternalArtifactService : IVssFrameworkService
  {
    ExternalArtifactCollectionWithStatus GetArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<(Guid repositoryId, string sha)> repositoryAndShas,
      IEnumerable<(Guid repositoryId, string prNumber)> repositoryAndPrNumbers,
      IEnumerable<(Guid repositoryId, string issueNumber)> repositoryAndIssueNumbers);

    IEnumerable<PendingExternalArtifactIdentifier> GetPendingArtifacts(
      IVssRequestContext requestContext);

    void SaveArtifacts(
      IVssRequestContext requestContext,
      string providerKey,
      Guid internalRepositoryId,
      ExternalArtifactCollectionWithStatus artifacts);

    void SaveArtifacts(
      IVssRequestContext requestContext,
      string providerKey,
      IEnumerable<ExternalGitRepo> repos,
      ExternalArtifactCollectionWithStatus artifacts);

    void SaveArtifactWatermarks(
      IVssRequestContext requestContext,
      ExternalArtifactCollectionWithStatus artifacts);
  }
}
