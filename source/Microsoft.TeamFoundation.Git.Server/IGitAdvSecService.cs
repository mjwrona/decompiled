// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitAdvSecService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (GitAdvSecService))]
  public interface IGitAdvSecService : IVssFrameworkService
  {
    bool CheckS2SCall(IVssRequestContext rc);

    void DeleteEnablementData(
      IVssRequestContext rc,
      bool allProjects,
      bool includeBillableCommitters,
      IEnumerable<Guid> projectIds);

    void DeleteRepositoryEnablementData(
      IVssRequestContext rc,
      Guid projectId,
      Guid repositoryId,
      bool includeBillableCommitters);

    List<GitBillableCommitter> EstimateBillableCommitters(
      IVssRequestContext rc,
      Guid projectId,
      Guid? repositoryId);

    List<GitBillablePusher> EstimateBillablePushers(IVssRequestContext rc, Guid? projectId);

    bool IsEnabledForRepository(
      IVssRequestContext rc,
      string teamProjectUri,
      Guid repositoryId,
      DateTime? billingDate);

    bool IsEnabledForAnyRepository(IVssRequestContext rc, DateTime? billingDate);

    void OnProjectCreate(IVssRequestContext rc, Guid projectId, bool isRecovered);

    void OnProjectDisable(IVssRequestContext rc, Guid projectId);

    void OnRepositoryCreate(IVssRequestContext rc, RepoKey repoKey);

    void OnRepositoryDestroy(IVssRequestContext rc, RepoKey repoKey);

    void OnRepositoryDisable(IVssRequestContext rc, RepoKey repoKey);

    List<GitBillableCommitter> QueryBillableCommitters(
      IVssRequestContext rc,
      string teamProjectUri,
      DateTime? billingDate,
      int? skip,
      int? top);

    List<GitBillableCommitterDetail> QueryBillableCommittersDetailed(
      IVssRequestContext rc,
      string teamProjectUri,
      DateTime? billingDate);

    List<GitAdvSecEnablementStatus> QueryEnablementStatus(
      IVssRequestContext rc,
      IEnumerable<Guid> projectIds,
      bool includeDeleted,
      DateTime? billingDate,
      int? skip,
      int? take);

    bool QueryEnableOnCreateHostRegKey(IVssRequestContext rc);

    bool QueryEnableOnCreateProjectRegKey(IVssRequestContext rc, Guid projectId);

    void RemoveEnableOnCreateHostRegKey(IVssRequestContext rc);

    void RemoveEnableOnCreateProjectRegKey(IVssRequestContext rc, Guid projectId);

    void UpdateEnablementStatus(
      IVssRequestContext rc,
      IEnumerable<GitAdvSecEnablementUpdate> updates);

    void UpdateEnableOnCreateHostRegKey(IVssRequestContext rc, bool value);

    void UpdateEnableOnCreateProjectRegKey(IVssRequestContext rc, Guid projectId, bool value);
  }
}
