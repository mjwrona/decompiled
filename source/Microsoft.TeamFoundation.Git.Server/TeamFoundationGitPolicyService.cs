// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitPolicyService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitPolicyService : ITeamFoundationGitPolicyService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<PolicyConfigurationRecord> GetLatestGitPolicyConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? repoId,
      string refName,
      Guid? policyType,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId)
    {
      return this.GetLatestGitPolicyConfigurations(requestContext, projectId, repoId, new string[1]
      {
        refName
      }, policyType, top, firstConfigurationId, out nextConfigurationId);
    }

    public IEnumerable<PolicyConfigurationRecord> GetLatestGitPolicyConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? repoId,
      string[] refNames)
    {
      return this.GetLatestGitPolicyConfigurations(requestContext, projectId, repoId, refNames, new Guid?(), int.MaxValue, 1, out int? _);
    }

    private IEnumerable<PolicyConfigurationRecord> GetLatestGitPolicyConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? repoId,
      string[] refNames,
      Guid? policyType,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId)
    {
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      string[] scopes1 = GitPolicyScopeResolver.RepositoryPathToScopes(repoId, repoId.HasValue && DefaultBranchUtils.IsAnyDefaultBranch(requestContext, repoId.Value, refNames), refNames);
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      string[] scopes2 = scopes1;
      int top1 = top;
      int firstConfigurationId1 = firstConfigurationId;
      ref int? local = ref nextConfigurationId;
      Guid? policyType1 = policyType;
      return service.GetLatestPolicyConfigurationRecordsByScope(requestContext1, projectId1, (IEnumerable<string>) scopes2, top1, firstConfigurationId1, out local, policyType1);
    }

    public List<ProjectPolicySummaryItem> GetProjectBranchPolicySummary(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      HashSet<Guid> branchPolicyTypeIds = requestContext.GetService<ITeamFoundationPolicyService>().GetPolicyTypes(requestContext).Where<ITeamFoundationPolicy>((Func<ITeamFoundationPolicy, bool>) (pt => pt is ITfsGitRefUpdatePolicy)).Select<ITeamFoundationPolicy, Guid>((Func<ITeamFoundationPolicy, Guid>) (pt => pt.Id)).ToHashSet<Guid>();
      List<(string, Guid)> projectPolicySummary;
      using (GitCoreComponent component = requestContext.CreateComponent<GitCoreComponent>())
        projectPolicySummary = component.GetProjectPolicySummary(projectId);
      return projectPolicySummary.Where<(string, Guid)>((Func<(string, Guid), bool>) (tup => branchPolicyTypeIds.Contains(tup.PolicyTypeId))).GroupBy<(string, Guid), string>((Func<(string, Guid), string>) (tup => tup.Scope)).Select<IGrouping<string, (string, Guid)>, ProjectPolicySummaryItem>((Func<IGrouping<string, (string, Guid)>, ProjectPolicySummaryItem>) (g => new ProjectPolicySummaryItem()
      {
        scope = g.Key,
        policyTypeIds = g.Select<(string, Guid), Guid>((Func<(string, Guid), Guid>) (tup => tup.PolicyTypeId)).ToList<Guid>()
      })).ToList<ProjectPolicySummaryItem>();
    }

    public GetExistingBranchHintsResult GetExistingBranchHints(
      IVssRequestContext requestContext,
      Guid projectId,
      string branchPattern,
      bool isDefault,
      int top)
    {
      using (GitCoreComponent component = requestContext.CreateComponent<GitCoreComponent>())
        return component.GetExistingBranchHints(projectId, branchPattern, isDefault, top);
    }

    public void ProjectPoliciesRemove(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope)
    {
      string projectUri = ProjectInfo.GetProjectUri(projectId);
      LocalSecurityNamespace securityNamespace = requestContext.GetService<LocalSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      string securable = GitUtils.CalculateSecurable(projectUri, Guid.Empty, (string) null);
      IVssRequestContext requestContext1 = requestContext;
      string token = securable;
      securityNamespace.CheckPermission(requestContext1, token, 2048, false);
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      List<Guid> branchPolicyTypeIds = service.GetPolicyTypes(requestContext).Where<ITeamFoundationPolicy>((Func<ITeamFoundationPolicy, bool>) (pt => pt is ITfsGitRefUpdatePolicy)).Select<ITeamFoundationPolicy, Guid>((Func<ITeamFoundationPolicy, Guid>) (pt => pt.Id)).ToList<Guid>();
      List<PolicyConfigurationRecord> list = service.GetLatestPolicyConfigurationRecordsByScope(requestContext, projectId, (IEnumerable<string>) new string[1]
      {
        scope
      }, int.MaxValue, 1, out int? _).Where<PolicyConfigurationRecord>((Func<PolicyConfigurationRecord, bool>) (p => branchPolicyTypeIds.Contains(p.TypeId))).ToList<PolicyConfigurationRecord>();
      Exception exception = (Exception) null;
      foreach (PolicyConfigurationRecord configurationRecord in list)
      {
        try
        {
          service.DeletePolicyConfiguration(requestContext, projectId, configurationRecord.ConfigurationId);
        }
        catch (Exception ex)
        {
          TracepointUtils.TraceException(requestContext, 1013484, GitServerUtils.GitPolicyArea, nameof (TeamFoundationGitPolicyService), ex, caller: nameof (ProjectPoliciesRemove));
          exception = exception ?? ex;
        }
      }
      if (exception != null)
        throw exception;
    }
  }
}
