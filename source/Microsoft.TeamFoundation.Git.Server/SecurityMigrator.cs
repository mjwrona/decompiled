// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SecurityMigrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class SecurityMigrator
  {
    private readonly IVssRequestContext m_rc;
    private readonly Func<IVssSecurityNamespace, string, IAccessControlList> m_queryAccessControlList;

    public SecurityMigrator(
      IVssRequestContext rc,
      Func<IVssSecurityNamespace, string, IAccessControlList> queryAccessControlListForTest = null)
    {
      this.m_rc = rc;
      this.m_queryAccessControlList = queryAccessControlListForTest ?? (Func<IVssSecurityNamespace, string, IAccessControlList>) ((sn, token) => sn.QueryAccessControlList(this.m_rc, token, (IEnumerable<IdentityDescriptor>) null, false));
    }

    public string Migrate(
      Guid projectId,
      Guid sourceRepo,
      Guid targetRepo,
      string[] branchesToMigrate,
      string migrationBranchPrefix = null)
    {
      IVssSecurityNamespace securityNamespace = this.m_rc.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.m_rc, GitConstants.GitSecurityNamespaceId);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string migrationBranch in branchesToMigrate)
      {
        BranchMigrationData branchMigrationData = MigrationBranchParser.Parse(migrationBranch, migrationBranchPrefix);
        dictionary[branchMigrationData.SourceName] = branchMigrationData.TargetName;
      }
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      string securable1 = GitUtils.CalculateSecurable(projectId, sourceRepo, (string) null);
      string securable2 = GitUtils.CalculateSecurable(projectId, targetRepo, (string) null);
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        IEnumerable<string> relevantTokens = this.GetRelevantTokens(projectId, sourceRepo, keyValuePair.Key);
        foreach (string str1 in relevantTokens)
        {
          IAccessControlList accessControlList = this.m_queryAccessControlList(securityNamespace, str1);
          string securable3 = GitUtils.CalculateSecurable(projectId, sourceRepo, keyValuePair.Key);
          string securable4 = GitUtils.CalculateSecurable(projectId, targetRepo, keyValuePair.Value);
          string str2 = str1.Replace(securable3, securable4).Replace(securable1, securable2);
          IAccessControlList acl = this.m_queryAccessControlList(securityNamespace, str2);
          if (acl.IsEmpty(true))
            acl.InheritPermissions = accessControlList.InheritPermissions;
          bool flag = false;
          foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          {
            IAccessControlEntry newAce = acl.QueryAccessControlEntry(accessControlEntry.Descriptor);
            if (newAce.IsEmpty)
            {
              newAce.Allow = accessControlEntry.Allow;
              newAce.Deny = accessControlEntry.Deny;
              acl.SetAccessControlEntry(newAce, false);
              flag = true;
            }
            else
              ++num2;
          }
          if (flag)
          {
            ++num1;
            securityNamespace.SetAccessControlLists(this.m_rc, (IEnumerable<IAccessControlList>) new IAccessControlList[1]
            {
              acl
            });
          }
          num3 += relevantTokens.Count<string>();
        }
      }
      return string.Format("Migrated {0} ACLs across {1} tokens. Skipped {2} ACEs.", (object) num1, (object) num3, (object) num2);
    }

    private IEnumerable<string> GetRelevantTokens(
      Guid projectId,
      Guid sourceRepo,
      string sourceBranch)
    {
      HashSet<string> relevantTokens = new HashSet<string>();
      relevantTokens.Add(GitUtils.CalculateSecurable(projectId, sourceRepo, (string) null));
      foreach (string branchNamespace in this.GetBranchNamespaces(sourceBranch))
        relevantTokens.Add(GitUtils.CalculateSecurable(projectId, sourceRepo, branchNamespace));
      return (IEnumerable<string>) relevantTokens;
    }

    private IEnumerable<string> GetBranchNamespaces(string branch)
    {
      List<string> branchNamespaces = new List<string>();
      branchNamespaces.Add(branch);
      int length1 = "refs/heads/".Length;
      string str = branch.Substring(length1);
      for (int length2 = str.LastIndexOf('/'); length2 > -1; length2 = str.LastIndexOf('/'))
      {
        str = str.Substring(0, length2);
        branchNamespaces.Add("refs/heads/" + str);
      }
      return (IEnumerable<string>) branchNamespaces;
    }
  }
}
