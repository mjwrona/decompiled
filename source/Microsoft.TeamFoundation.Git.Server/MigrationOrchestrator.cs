// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MigrationOrchestrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class MigrationOrchestrator
  {
    public void CleanInProgressRefs(
      IVssRequestContext rc,
      ITfsGitRepository source,
      ITfsGitRepository target)
    {
      ITeamFoundationGitRefService service = rc.GetService<ITeamFoundationGitRefService>();
      List<TfsGitRefUpdateRequest> refUpdates = new List<TfsGitRefUpdateRequest>();
      string[] refNames = new string[1]
      {
        string.Format("refs/repo_migrate/target/{0}/in_progress", (object) target.Key.RepoId)
      };
      foreach (TfsGitRef matchingName in source.Refs.MatchingNames((IEnumerable<string>) refNames, GitRefSearchType.StartsWith))
        refUpdates.Add(new TfsGitRefUpdateRequest(matchingName.Name, matchingName.ObjectId, Sha1Id.Empty));
      service.UpdateRefs(rc, source.Key.RepoId, refUpdates);
    }

    public bool AreMigrationsInProgress(
      IVssRequestContext rc,
      ITfsGitRepository source,
      ITfsGitRepository target)
    {
      string[] refNames = new string[1]
      {
        string.Format("refs/repo_migrate/target/{0}/in_progress", (object) target.Key.RepoId)
      };
      return source.Refs.MatchingNames((IEnumerable<string>) refNames, GitRefSearchType.StartsWith).Any<TfsGitRef>();
    }

    public IEnumerable<RepoMigrationRequest> Orchestrate(
      IVssRequestContext rc,
      ITfsGitRepository source,
      ITfsGitRepository target,
      string branchNamePrefix,
      bool migrateMetadata,
      bool skipValidationsForTestingOnly)
    {
      IVssRegistryService service = rc.GetService<IVssRegistryService>();
      Dictionary<string[], List<string>> dictionary1 = new Dictionary<string[], List<string>>((IEqualityComparer<string[]>) new MigrationOrchestrator.StructuralEqualityComparer<string[]>());
      List<RepoMigrationRequest> migrationRequestList = new List<RepoMigrationRequest>();
      string refSubstringPrefix = branchNamePrefix + "/" + target.Name + "/";
      string[] refNames = new string[1]
      {
        refSubstringPrefix
      };
      int num1 = service.GetValue<int>(rc, (RegistryQuery) "/Service/Git/Settings/MaxMigrationJobConcurrency", 10);
      List<TfsGitRef> source1 = source.Refs.MatchingNames((IEnumerable<string>) refNames, GitRefSearchType.StartsWith);
      if (!migrateMetadata)
      {
        Dictionary<string, BranchMigrationData> dictionary2 = new Dictionary<string, BranchMigrationData>();
        foreach (TfsGitRef tfsGitRef in source1)
        {
          BranchMigrationData branchMigrationData = MigrationBranchParser.Parse(tfsGitRef.Name, refSubstringPrefix);
          if (branchMigrationData != null && target.Refs.MatchingName(branchMigrationData.TargetName)?.Name == null)
          {
            bool flag = true;
            foreach (string parentName in branchMigrationData.ParentNames)
            {
              if (target.Refs.MatchingName(parentName) == null)
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              if (!dictionary1.ContainsKey(branchMigrationData.ParentNames))
                dictionary1.Add(branchMigrationData.ParentNames, new List<string>());
              dictionary1[branchMigrationData.ParentNames].Add(tfsGitRef.Name);
            }
          }
        }
        int num2 = 0;
        foreach (KeyValuePair<string[], List<string>> keyValuePair in dictionary1)
        {
          if (num2 < num1)
          {
            migrationRequestList.Add(new RepoMigrationRequest()
            {
              SourceRepoId = source.Key.RepoId,
              TargetRepoId = target.Key.RepoId,
              ParentRefNames = keyValuePair.Key,
              BranchesToMigrate = keyValuePair.Value.ToArray(),
              MigrateCode = true,
              MigrateMetadata = false,
              MigrateSecurity = true,
              SkipValidationsForTestingOnly = skipValidationsForTestingOnly,
              MigrationBranchPrefix = refSubstringPrefix
            });
            ++num2;
          }
          else
            break;
        }
      }
      else if (source1.Count > 0)
        migrationRequestList.Add(new RepoMigrationRequest()
        {
          SourceRepoId = source.Key.RepoId,
          TargetRepoId = target.Key.RepoId,
          ParentRefNames = Array.Empty<string>(),
          BranchesToMigrate = source1.Select<TfsGitRef, string>((Func<TfsGitRef, string>) (r => r.Name.Replace(refSubstringPrefix, string.Empty))).ToArray<string>(),
          MigrateCode = false,
          MigrateMetadata = true,
          MigrateSecurity = false,
          SkipValidationsForTestingOnly = skipValidationsForTestingOnly,
          MigrationBranchPrefix = refSubstringPrefix
        });
      return (IEnumerable<RepoMigrationRequest>) migrationRequestList;
    }

    private class StructuralEqualityComparer<T> : IEqualityComparer<T>
    {
      public bool Equals(T x, T y) => StructuralComparisons.StructuralEqualityComparer.Equals((object) x, (object) y);

      public int GetHashCode(T obj) => StructuralComparisons.StructuralEqualityComparer.GetHashCode((object) obj);
    }
  }
}
