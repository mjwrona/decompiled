// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.CodeReindexingStatusEvaluator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class CodeReindexingStatusEvaluator : IReindexingStatusEvaluator
  {
    private TraceMetaData m_traceMetaData;

    public CodeReindexingStatusEvaluator() => this.m_traceMetaData = new TraceMetaData(1083143, "Indexing Pipeline", "IndexingOperation");

    public bool Evaluate(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      bool suspendGitPrimaryIndexing;
      bool gitRepositories = this.EvaluateGitRepositories(requestContext, indexingUnitDataAccess, out suspendGitPrimaryIndexing);
      if (gitRepositories)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "All git repos have caught up.");
      if (!suspendGitPrimaryIndexing)
        return false;
      bool suspendTfvcPrimaryIndexing;
      bool tfvcRepositories = this.EvaluateTfvcRepositories(requestContext, indexingUnitDataAccess, out suspendTfvcPrimaryIndexing);
      if (tfvcRepositories)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "All tfvc repos have caught up.");
      bool suspendCustomPrimaryIndexing;
      bool customRepositories = this.EvaluateCustomRepositories(requestContext, indexingUnitDataAccess, out suspendCustomPrimaryIndexing);
      if (customRepositories)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "All Custom repos have caught up.");
      bool flag1 = suspendGitPrimaryIndexing & suspendTfvcPrimaryIndexing & suspendCustomPrimaryIndexing;
      bool flag2 = gitRepositories & tfvcRepositories & customRepositories;
      if (flag2)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "All repos have caught up. Can finalize Index.");
      if (requestContext.IsCollectionFinalizationPaused((IEntityType) CodeEntityType.GetInstance()))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Suspension of indexing on primary IUs and collection finalization is paused because registry key [{0}] is set to true.", (object) "/Service/ALMSearch/Settings/PauseCodeCollectionFinalizationDuringZLRI")));
        return false;
      }
      if (flag1)
      {
        requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, "/Service/ALMSearch/Settings/SuspendCodeIndexingOnPrimary", true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "Suspending primary indexing for code entity.");
      }
      return flag2;
    }

    private string GetDifferencesBetweenIUs(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryRepositoryIndexingUnits,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowRepositoryIndexingUnits)
    {
      HashSet<Guid> hashSet1 = primaryRepositoryIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId)).ToHashSet<Guid>();
      HashSet<Guid> hashSet2 = shadowRepositoryIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId)).ToHashSet<Guid>();
      string str1 = string.Join<Guid>(",", hashSet1.Except<Guid>((IEnumerable<Guid>) hashSet2));
      string str2 = string.Join<Guid>(",", hashSet2.Except<Guid>((IEnumerable<Guid>) hashSet1));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Primary and Shadow Indexing Units are not equal.")));
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("ReposNotInShadow -> {0}, ReposNotInPrimary -> {1}.", (object) str1, (object) str2)));
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("TotalRepoCountInPrimary = {0}. TotalRepoCountInShadow = {1}", (object) hashSet1.Count, (object) hashSet2.Count)));
      return stringBuilder.ToString();
    }

    private bool EvaluateCustomRepositories(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      out bool suspendCustomPrimaryIndexing)
    {
      if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassReindexStatusServiceCheckForCustom"))
      {
        suspendCustomPrimaryIndexing = true;
        return true;
      }
      suspendCustomPrimaryIndexing = false;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList1 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "CustomRepository", false, (IEntityType) CodeEntityType.GetInstance(), -1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList2 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "CustomRepository", true, (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnitList1 == null)
        indexingUnitList1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnitList2 == null)
        indexingUnitList2 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return this.CompareCustomDepoIndexInformationOfPrimaryAndShadowIUs(requestContext, indexingUnitList1, indexingUnitList2, out suspendCustomPrimaryIndexing);
      if (!indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && !indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no custom repo Indexing units.")));
        suspendCustomPrimaryIndexing = true;
        return true;
      }
      if (!indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no primary custom repo Indexing units.")));
        suspendCustomPrimaryIndexing = true;
        return true;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no shadow custom repo Indexing units.")));
      return false;
    }

    private bool CompareCustomDepoIndexInformationOfPrimaryAndShadowIUs(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryCustomRepositoryIndexingUnits,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowCustomRepositoryIndexingUnits,
      out bool suspendCustomPrimaryIndexing)
    {
      suspendCustomPrimaryIndexing = false;
      bool flag = true;
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxPermittedStalenessInShadowIndexingUnitsInSec", 14400);
      Dictionary<Guid, Dictionary<string, Dictionary<string, DepotIndexInfo>>> dictionary1 = primaryCustomRepositoryIndexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Dictionary<string, Dictionary<string, DepotIndexInfo>>>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Dictionary<string, Dictionary<string, DepotIndexInfo>>>) (x => (x.Properties as CustomRepoCodeIndexingProperties).DepotIndexInfo));
      Dictionary<Guid, Dictionary<string, Dictionary<string, DepotIndexInfo>>> dictionary2 = shadowCustomRepositoryIndexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Dictionary<string, Dictionary<string, DepotIndexInfo>>>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Dictionary<string, Dictionary<string, DepotIndexInfo>>>) (x => (x.Properties as CustomRepoCodeIndexingProperties).DepotIndexInfo));
      foreach (Guid key1 in dictionary1.Keys)
      {
        Dictionary<string, Dictionary<string, DepotIndexInfo>> depoIndexeInfoDictionary = dictionary1[key1];
        Dictionary<string, Dictionary<string, DepotIndexInfo>> dictionary3;
        if (!dictionary2.TryGetValue(key1, out dictionary3))
        {
          if (this.GetMaxLastIndexedChangeIDForRepo(depoIndexeInfoDictionary) > 0L)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Repository {0} not found in custom shadow indexing units. Returning false.", (object) key1)));
            return false;
          }
        }
        else if (dictionary3.Count == 0)
        {
          if (depoIndexeInfoDictionary.Count != 0)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Bulk Indexing for custom repository {0} is not yet complete for shadow indexing unit. Returning false.", (object) key1)));
            return false;
          }
        }
        else
        {
          foreach (string key2 in depoIndexeInfoDictionary.Keys)
          {
            Dictionary<string, DepotIndexInfo> branchToDepotIndexInfo = depoIndexeInfoDictionary[key2];
            Dictionary<string, DepotIndexInfo> dictionary4;
            if (!dictionary3.TryGetValue(key2, out dictionary4))
            {
              long changeIdForDepot = this.GetMaxLastIndexedChangeIDForDepot(branchToDepotIndexInfo);
              if (changeIdForDepot > 0L)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("topfolder {0} is absent for shadow repo {1}. ", (object) key2, (object) key1)) + FormattableString.Invariant(FormattableStringFactory.Create("Primary topfolder MaxLastIndexedChangeId is {0}. Returning false.", (object) changeIdForDepot)));
                return false;
              }
            }
            else
            {
              foreach (string key3 in branchToDepotIndexInfo.Keys)
              {
                DepotIndexInfo depotIndexInfo1 = branchToDepotIndexInfo[key3];
                DepotIndexInfo depotIndexInfo2;
                if (!dictionary4.TryGetValue(key3, out depotIndexInfo2))
                {
                  if (depotIndexInfo1.LastIndexedChangeId > 0L)
                  {
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("branch {0} is absent for top folder {1} and for shadow repo {2}. ", (object) key3, (object) key2, (object) key1)) + FormattableString.Invariant(FormattableStringFactory.Create("Primary branch LastIndexedChangeId is {0}. Returning false.", (object) depotIndexInfo1.LastIndexedChangeId)));
                    return false;
                  }
                }
                else if (depotIndexInfo1.LastIndexedChangeId > 0L || depotIndexInfo2.LastIndexedChangeId > 0L)
                {
                  if (depotIndexInfo2.LastIndexedChangeId >= depotIndexInfo1.LastIndexedChangeId && depotIndexInfo1.LastIndexedChangeId > 0L)
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow is ahead of primary for repo {0} , folder{1} and branch {2}", (object) key1, (object) key2, (object) key3)));
                  else if ((depotIndexInfo1.LastIndexedCommitTime - depotIndexInfo2.LastIndexedCommitTime).TotalSeconds <= (double) configValueOrDefault)
                  {
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow IU is lagging behind Primary IU for repo {0} , folder {1} and branch {2}", (object) key1, (object) key2, (object) key3)) + FormattableString.Invariant(FormattableStringFactory.Create(" is within threshold. Cannot Finalize.")));
                    flag = false;
                  }
                  else
                  {
                    Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow IU is lagging behind Primary IU for repo {0} , folder {1} and branch {2}", (object) key1, (object) key2, (object) key3)) + FormattableString.Invariant(FormattableStringFactory.Create(" is beyond threshold. Cannot Finalize.")));
                    return false;
                  }
                }
              }
            }
          }
        }
      }
      suspendCustomPrimaryIndexing = true;
      return flag;
    }

    internal long GetMaxLastIndexedChangeIDForRepo(
      Dictionary<string, Dictionary<string, DepotIndexInfo>> depoIndexeInfoDictionary)
    {
      long indexedChangeIdForRepo = -1;
      foreach (Dictionary<string, DepotIndexInfo> branchToDepotIndexInfo in depoIndexeInfoDictionary.Values)
      {
        long changeIdForDepot = this.GetMaxLastIndexedChangeIDForDepot(branchToDepotIndexInfo);
        if (changeIdForDepot > indexedChangeIdForRepo)
          indexedChangeIdForRepo = changeIdForDepot;
      }
      return indexedChangeIdForRepo;
    }

    internal long GetMaxLastIndexedChangeIDForDepot(
      Dictionary<string, DepotIndexInfo> branchToDepotIndexInfo)
    {
      long changeIdForDepot = -1;
      foreach (DepotIndexInfo depotIndexInfo in branchToDepotIndexInfo.Values)
      {
        if (depotIndexInfo.LastIndexedChangeId > changeIdForDepot)
          changeIdForDepot = depotIndexInfo.LastIndexedChangeId;
      }
      return changeIdForDepot;
    }

    private bool EvaluateGitRepositories(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      out bool suspendGitPrimaryIndexing)
    {
      if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassReindexStatusServiceCheckForGit"))
      {
        suspendGitPrimaryIndexing = true;
        return true;
      }
      suspendGitPrimaryIndexing = false;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList1 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "Git_Repository", false, (IEntityType) CodeEntityType.GetInstance(), -1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList2 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "Git_Repository", true, (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnitList1 == null)
        indexingUnitList1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnitList2 == null)
        indexingUnitList2 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        if (indexingUnitList1.Count > indexingUnitList2.Count)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, this.GetDifferencesBetweenIUs(indexingUnitList1, indexingUnitList2));
          return false;
        }
        Dictionary<Guid, Dictionary<string, string>> primaryRepoToBranchToCommitsDictionary;
        Dictionary<Guid, Dictionary<string, string>> shadowRepoToBranchToCommitsDictionary;
        if (!this.ValidateBranchIndexInformationOfPrimaryAndShadowIUs(indexingUnitList1, indexingUnitList2, out primaryRepoToBranchToCommitsDictionary, out shadowRepoToBranchToCommitsDictionary))
          return false;
        bool gitRepositories = true;
        suspendGitPrimaryIndexing = true;
        foreach (Guid key in primaryRepoToBranchToCommitsDictionary.Keys)
        {
          Dictionary<string, DateTime> pushTimeDictionary1 = this.GetBranchToCommitPushTimeDictionary(requestContext, primaryRepoToBranchToCommitsDictionary[key], key);
          Dictionary<string, DateTime> pushTimeDictionary2 = this.GetBranchToCommitPushTimeDictionary(requestContext, shadowRepoToBranchToCommitsDictionary[key], key, true);
          bool flag = this.CompareGitPrimaryWithShadowCommitPushTimes(requestContext, key, pushTimeDictionary1, pushTimeDictionary2, out suspendGitPrimaryIndexing);
          if (!suspendGitPrimaryIndexing)
            return false;
          gitRepositories &= flag;
        }
        return gitRepositories;
      }
      if (!indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && !indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no git repo Indexing units.")));
        suspendGitPrimaryIndexing = true;
        return true;
      }
      if (!indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no primary git repo Indexing units.")));
        suspendGitPrimaryIndexing = true;
        return true;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no shadow git repo Indexing units.")));
      return false;
    }

    private bool ValidateBranchIndexInformationOfPrimaryAndShadowIUs(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryGitRepositoryIndexingUnits,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowGitRepositoryIndexingUnits,
      out Dictionary<Guid, Dictionary<string, string>> primaryRepoToBranchToCommitsDictionary,
      out Dictionary<Guid, Dictionary<string, string>> shadowRepoToBranchToCommitsDictionary)
    {
      primaryRepoToBranchToCommitsDictionary = new Dictionary<Guid, Dictionary<string, string>>();
      shadowRepoToBranchToCommitsDictionary = new Dictionary<Guid, Dictionary<string, string>>();
      Dictionary<Guid, Dictionary<string, GitBranchIndexInfo>> dictionary1 = primaryGitRepositoryIndexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Dictionary<string, GitBranchIndexInfo>>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Dictionary<string, GitBranchIndexInfo>>) (x => (x.Properties as GitCodeRepoIndexingProperties).BranchIndexInfo));
      Dictionary<Guid, Dictionary<string, GitBranchIndexInfo>> dictionary2 = shadowGitRepositoryIndexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Dictionary<string, GitBranchIndexInfo>>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Dictionary<string, GitBranchIndexInfo>>) (x => (x.Properties as GitCodeRepoIndexingProperties).BranchIndexInfo));
      foreach (Guid key1 in dictionary1.Keys)
      {
        Dictionary<string, GitBranchIndexInfo> dictionary3;
        if (!dictionary2.TryGetValue(key1, out dictionary3))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Repository {0} not found in shadow indexing units. Returning false.", (object) key1)));
          return false;
        }
        Dictionary<string, GitBranchIndexInfo> dictionary4 = dictionary1[key1];
        if (dictionary3.Count == 0)
        {
          if (dictionary4.Count != 0)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Bulk Indexing for repository {0} is not yet complete for shadow indexing unit. Returning false.", (object) key1)));
            return false;
          }
        }
        else
        {
          Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
          Dictionary<string, string> dictionary6 = new Dictionary<string, string>();
          foreach (string key2 in dictionary4.Keys)
          {
            GitBranchIndexInfo gitBranchIndexInfo1 = dictionary4[key2];
            GitBranchIndexInfo gitBranchIndexInfo2;
            if (!dictionary3.TryGetValue(key2, out gitBranchIndexInfo2))
            {
              if (!gitBranchIndexInfo1.IsDefaultLastIndexedCommitId() && !string.IsNullOrEmpty(gitBranchIndexInfo1.LastIndexedCommitId))
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Branch {0} is absent for shadow repo {1}. ", (object) key2, (object) key1)) + FormattableString.Invariant(FormattableStringFactory.Create("Primary branch's LastIndexedCommitId is {0}. Returning false.", (object) gitBranchIndexInfo1.LastIndexedCommitId)));
                return false;
              }
            }
            else
            {
              bool flag1 = gitBranchIndexInfo1.IsDefaultLastIndexedCommitId() || string.IsNullOrEmpty(gitBranchIndexInfo1.LastIndexedCommitId);
              bool flag2 = gitBranchIndexInfo2.IsDefaultLastIndexedCommitId() || string.IsNullOrEmpty(gitBranchIndexInfo2.LastIndexedCommitId);
              if (!(flag1 & flag2))
              {
                if (!flag1 && !flag2)
                {
                  dictionary5.Add(key2, gitBranchIndexInfo1.LastIndexedCommitId);
                  dictionary6.Add(key2, gitBranchIndexInfo2.LastIndexedCommitId);
                }
                else
                {
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("LastIndexedCommitId for primary repo {0} and branch {1} is {2}.", (object) key1, (object) key2, (object) gitBranchIndexInfo1.LastIndexedCommitId)) + FormattableString.Invariant(FormattableStringFactory.Create(" LastIndexedCommitId for shadow repo {0} and branch {1} is {2}.", (object) key1, (object) key2, (object) gitBranchIndexInfo2.LastIndexedCommitId)) + FormattableString.Invariant(FormattableStringFactory.Create(" Returning false.")));
                  return false;
                }
              }
            }
          }
          primaryRepoToBranchToCommitsDictionary.Add(key1, dictionary5);
          shadowRepoToBranchToCommitsDictionary.Add(key1, dictionary6);
        }
      }
      return true;
    }

    internal Dictionary<string, DateTime> GetBranchToCommitPushTimeDictionary(
      IVssRequestContext requestContext,
      Dictionary<string, string> branchToCommitIds,
      Guid repoId,
      bool isShadow = false)
    {
      GitHttpClient redirectedClientIfNeeded = requestContext.GetRedirectedClientIfNeeded<GitHttpClient>();
      Dictionary<string, DateTime> pushTimeDictionary = new Dictionary<string, DateTime>();
      List<string> list = branchToCommitIds.Values.ToHashSet<string>().ToList<string>();
      List<GitCommitRef> source = this.FetchCommitsFromVC(requestContext, repoId, list, redirectedClientIfNeeded);
      Dictionary<string, DateTime> dictionary = source != null ? source.ToDictionary<GitCommitRef, string, DateTime>((Func<GitCommitRef, string>) (x => x.CommitId), (Func<GitCommitRef, DateTime>) (x => x.Push != null ? x.Push.Date : x.Committer.Date)) : (Dictionary<string, DateTime>) null;
      if (dictionary != null && dictionary.Any<KeyValuePair<string, DateTime>>())
      {
        List<string> stringList = new List<string>();
        foreach (string key in branchToCommitIds.Keys)
        {
          DateTime dateTime;
          if (!dictionary.TryGetValue(branchToCommitIds[key], out dateTime))
            stringList.Add(key);
          else
            pushTimeDictionary[key] = dateTime;
        }
        if (stringList.Any<string>())
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Unable to retrieve commits for branches {0} for repositoy {1}. IsShadow {2}", (object) string.Join(",", (IEnumerable<string>) stringList), (object) repoId, (object) isShadow)));
      }
      return pushTimeDictionary;
    }

    private bool CompareGitPrimaryWithShadowCommitPushTimes(
      IVssRequestContext requestContext,
      Guid repositoryId,
      Dictionary<string, DateTime> primaryBranchToCommitPushTimes,
      Dictionary<string, DateTime> shadowBranchToCommitPushTimes,
      out bool suspendGitPrimaryIndexing)
    {
      suspendGitPrimaryIndexing = false;
      bool flag = true;
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxPermittedStalenessInShadowIndexingUnitsInSec", 14400);
      foreach (string key in primaryBranchToCommitPushTimes.Keys)
      {
        DateTime dateTime;
        if (!shadowBranchToCommitPushTimes.TryGetValue(key, out dateTime))
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Branch {0} was excluded from shadow for repo {1}. Continuing.", (object) key, (object) repositoryId)));
        else if (!(dateTime >= primaryBranchToCommitPushTimes[key]))
        {
          if ((primaryBranchToCommitPushTimes[key] - dateTime).TotalSeconds > (double) configValueOrDefault)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Branch {0} in shadow for repo {1} is lagging behind primary by more than the threshold. Cannot suspend primary indexing.", (object) key, (object) repositoryId)));
            return false;
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("lastShadowPushTime {0} is lagging behind lastPrimaryPushTime {1} for branch {2} in Git repository {3}.", (object) dateTime, (object) primaryBranchToCommitPushTimes[key], (object) key, (object) repositoryId)) + FormattableString.Invariant(FormattableStringFactory.Create(" Cannot Finalize.")));
          flag = false;
        }
      }
      suspendGitPrimaryIndexing = true;
      if (flag)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Git repo {0} has caught up.", (object) repositoryId)));
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Git repo {0} is nearing completion for shadow Indexing Unit.", (object) repositoryId)));
      return flag;
    }

    internal List<GitCommitRef> FetchCommitsFromVC(
      IVssRequestContext requestContext,
      Guid repositoryId,
      List<string> commitIds,
      GitHttpClient gitHttpClient)
    {
      int count1 = commitIds.Count;
      List<GitCommitRef> gitCommitRefList1 = new List<GitCommitRef>(count1);
      if (count1 == 0)
        return gitCommitRefList1;
      int count2 = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/MaxNumOfCommitsToFetchFromVc", true, 25);
      int count3 = 0;
      while (count3 < count1)
      {
        List<string> list = commitIds.Skip<string>(count3).Take<string>(count2).ToList<string>();
        List<GitCommitRef> gitCommitRefList2;
        try
        {
          gitCommitRefList2 = this.FetchCommitDetails(requestContext, repositoryId, list, gitHttpClient);
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.m_traceMetaData, ex.Message);
          if (count2 != 1)
          {
            count2 = 1;
            continue;
          }
          gitCommitRefList2 = (List<GitCommitRef>) null;
        }
        if (gitCommitRefList2 == null || !gitCommitRefList2.Any<GitCommitRef>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(this.m_traceMetaData.TracePoint, TraceLevel.Info, this.m_traceMetaData.TraceArea, this.m_traceMetaData.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Received no commits for {0}. Ignoring these commits from evaluation.", (object) string.Join(",", (IEnumerable<string>) list))));
          count3 += count2;
        }
        else
        {
          gitCommitRefList1.AddRange((IEnumerable<GitCommitRef>) gitCommitRefList2);
          count3 += count2;
        }
      }
      return gitCommitRefList1;
    }

    private List<GitCommitRef> FetchCommitDetails(
      IVssRequestContext requestContext,
      Guid repositoryId,
      List<string> commitIds,
      GitHttpClient gitHttpClient)
    {
      return ExponentialBackoffRetryInvoker.Instance.Invoke<List<GitCommitRef>>((Func<object>) (() =>
      {
        GitHttpClient gitHttpClient1 = gitHttpClient;
        Guid repositoryId1 = repositoryId;
        GitQueryCommitsCriteria searchCriteria = new GitQueryCommitsCriteria();
        searchCriteria.Ids = commitIds;
        searchCriteria.IncludePushData = true;
        int? skip = new int?();
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return (object) gitHttpClient1.GetCommitsAsync(repositoryId1, searchCriteria, skip, top, cancellationToken: cancellationToken).Result;
      }), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkitemFieldCacheService/GetFieldsFromTfsRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CrawlOperationRetryIntervalInSec"), true, this.m_traceMetaData);
    }

    private bool EvaluateTfvcRepositories(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      out bool suspendTfvcPrimaryIndexing)
    {
      suspendTfvcPrimaryIndexing = false;
      bool tfvcRepositories = true;
      if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassReindexStatusServiceCheckForTfvc"))
      {
        suspendTfvcPrimaryIndexing = true;
        return true;
      }
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxPermittedStalenessInShadowIndexingUnitsInSec", 14400);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList1 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "TFVC_Repository", false, (IEntityType) CodeEntityType.GetInstance(), -1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList2 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "TFVC_Repository", true, (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnitList1 == null)
        indexingUnitList1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingUnitList2 == null)
        indexingUnitList2 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (!indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && !indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no indexing units for tfvc. Returning true.")));
        suspendTfvcPrimaryIndexing = true;
        return true;
      }
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        if (indexingUnitList1.Count > indexingUnitList2.Count)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, this.GetDifferencesBetweenIUs(indexingUnitList1, indexingUnitList2));
          return false;
        }
        Dictionary<Guid, int> dictionary1 = indexingUnitList1.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => (x.Properties as TfvcCodeRepoIndexingProperties).LastIndexedChangeSetId));
        Dictionary<Guid, int> dictionary2 = indexingUnitList2.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => (x.Properties as TfvcCodeRepoIndexingProperties).LastIndexedChangeSetId));
        TfvcHttpClient redirectedClientIfNeeded = requestContext.GetRedirectedClientIfNeeded<TfvcHttpClient>();
        foreach (Guid key in dictionary1.Keys)
        {
          int changeSetId;
          if (!dictionary2.TryGetValue(key, out changeSetId))
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Repository {0} not found in shadow indexing units. Returning false.", (object) key)));
            return false;
          }
          if (dictionary1[key] <= changeSetId)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow is ahead of primary for Tfvc repo {0}.", (object) key)));
          }
          else
          {
            TfvcChangeset tfvcChangeset1 = this.FetchChangeSetDetails(requestContext, redirectedClientIfNeeded, key, dictionary1[key]);
            TfvcChangeset tfvcChangeset2 = this.FetchChangeSetDetails(requestContext, redirectedClientIfNeeded, key, changeSetId);
            if (tfvcChangeset1 == null && tfvcChangeset2 == null)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Could not fetch the changeset details of primary and shadow indexing units for Tfvc Repository {0}.", (object) key)) + FormattableString.Invariant(FormattableStringFactory.Create(" Primary Changeset {0}, Shadow Changeset {1}. Ignoring this repo.", (object) dictionary1[key], (object) changeSetId)));
            else if (tfvcChangeset1 != null && tfvcChangeset2 != null)
            {
              if (tfvcChangeset2.CreatedDate >= tfvcChangeset1.CreatedDate)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow is ahead of primary for Tfvc repo {0}.", (object) key)));
              }
              else
              {
                if ((tfvcChangeset1.CreatedDate - tfvcChangeset2.CreatedDate).TotalSeconds > (double) configValueOrDefault)
                {
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Tfvc Shadow indexing unit is lagging behind primary by more than the threshold for repository -> {0}.", (object) key)) + FormattableString.Invariant(FormattableStringFactory.Create(" Primary Changeset {0}, Shadow Changeset {1}. Returning false.", (object) tfvcChangeset1.ChangesetId, (object) tfvcChangeset2.ChangesetId)));
                  return false;
                }
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow Changeset {0} is lagging behind Primary Changeset {1}", (object) tfvcChangeset2.ChangesetId, (object) tfvcChangeset1.ChangesetId)) + FormattableString.Invariant(FormattableStringFactory.Create(" in tfvc repo {0} and is within threshold. Cannot Finalize.", (object) key)));
                tfvcRepositories = false;
              }
            }
            else
            {
              string str = "shadow";
              if (tfvcChangeset1 == null)
                str = "primary";
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Found null changeset for {0} and repository {1}. Returning false for tfvc.", (object) str, (object) key)));
              return false;
            }
          }
        }
        suspendTfvcPrimaryIndexing = true;
        return tfvcRepositories;
      }
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no shadow indexing units for tfvc. Returning false.")));
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no primary indexing units for tfvc. Returning false.")));
      return false;
    }

    private TfvcChangeset FetchChangeSetDetails(
      IVssRequestContext requestContext,
      TfvcHttpClient tfvcHttpClient,
      Guid repositoryId,
      int changeSetId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083147, "Indexing Pipeline", "IndexingOperation", nameof (FetchChangeSetDetails));
      try
      {
        return ExponentialBackoffRetryInvoker.Instance.Invoke<TfvcChangeset>((Func<object>) (() => (object) tfvcHttpClient.GetChangesetAsync(repositoryId, changeSetId).Result), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkitemFieldCacheService/GetFieldsFromTfsRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CrawlOperationRetryIntervalInSec"), true, this.m_traceMetaData);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083148, "Indexing Pipeline", "IndexingOperation", nameof (FetchChangeSetDetails));
      }
    }
  }
}
