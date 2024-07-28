// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitPolicyService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitPolicyService))]
  public interface ITeamFoundationGitPolicyService : IVssFrameworkService
  {
    IEnumerable<PolicyConfigurationRecord> GetLatestGitPolicyConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? repoId,
      string refName,
      Guid? policyType,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId);

    IEnumerable<PolicyConfigurationRecord> GetLatestGitPolicyConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? repoId,
      string[] refNames);

    List<ProjectPolicySummaryItem> GetProjectBranchPolicySummary(
      IVssRequestContext requestContext,
      Guid projectId);

    GetExistingBranchHintsResult GetExistingBranchHints(
      IVssRequestContext requestContext,
      Guid projectId,
      string branchPattern,
      bool isDefault,
      int top);

    void ProjectPoliciesRemove(IVssRequestContext requestContext, Guid projectId, string scope);
  }
}
