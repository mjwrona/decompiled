// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.WorkItemReindexingStatusEvaluator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class WorkItemReindexingStatusEvaluator : IReindexingStatusEvaluator
  {
    private readonly TraceMetaData m_traceMetaData;
    private const char WatermarkSeparator = ';';

    public WorkItemReindexingStatusEvaluator() => this.m_traceMetaData = new TraceMetaData(1083143, "Indexing Pipeline", "IndexingOperation");

    public bool Evaluate(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      try
      {
        if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/BypassReindexStatusServiceCheckForWorkItem"))
        {
          requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, "/Service/ALMSearch/Settings/SuspendWorkItemIndexingOnPrimary", true);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "Bypassed workitem checks, suspended indexing on Primary. Can finalize.");
          return true;
        }
        if (requestContext.IsCollectionFinalizationPaused((IEntityType) WorkItemEntityType.GetInstance()))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Suspension of indexing on primary IUs and collection finalization is paused because registry key [{0}] is set to true.", (object) "/Service/ALMSearch/Settings/PauseWorkItemCollectionFinalizationDuringZLRI")));
          return false;
        }
        bool suspendPrimaryIndexing;
        int num = this.EvaluateProjectIndexingUnits(requestContext, indexingUnitDataAccess, out suspendPrimaryIndexing) ? 1 : 0;
        if (num != 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "All project Indexing Units have caught up. Can finalize.");
        if (suspendPrimaryIndexing)
        {
          requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, "/Service/ALMSearch/Settings/SuspendWorkItemIndexingOnPrimary", true);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "WorkItem fields and discussions are nearing completion for shadow indexing units. Suspending Indexing on Primary.");
        }
        return num != 0;
      }
      catch (SearchServiceException ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.m_traceMetaData, (Exception) ex);
        return false;
      }
    }

    private bool EvaluateProjectIndexingUnits(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      out bool suspendPrimaryIndexing)
    {
      suspendPrimaryIndexing = false;
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxPermittedDifferenceInWatermarksForWorkItemContinuationToken", 400);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList1 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", false, (IEntityType) WorkItemEntityType.GetInstance(), -1);
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrEmpty(currentHostConfigValue))
      {
        List<string> deletedProjects = ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>();
        indexingUnitList1 = indexingUnitList1.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => !deletedProjects.Contains(x.TFSEntityId.ToString()))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      }
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList2 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "Project", true, (IEntityType) WorkItemEntityType.GetInstance(), -1);
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
        Dictionary<Guid, string> dictionary1 = indexingUnitList1.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (x => (x.Properties as ProjectWorkItemIndexingProperties).LastIndexedFieldsContinuationToken));
        Dictionary<Guid, string> dictionary2 = indexingUnitList2.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (x => (x.Properties as ProjectWorkItemIndexingProperties).LastIndexedFieldsContinuationToken));
        Dictionary<Guid, string> dictionary3 = indexingUnitList1.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (x => (x.Properties as ProjectWorkItemIndexingProperties).LastIndexedDiscussionContinuationToken));
        Dictionary<Guid, string> dictionary4 = indexingUnitList2.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (x => (x.Properties as ProjectWorkItemIndexingProperties).LastIndexedDiscussionContinuationToken));
        bool suspendWorkItemPrimaryIndexing1;
        int num1 = this.EvaluateContinuationTokens(dictionary1, dictionary2, configValueOrDefault, out suspendWorkItemPrimaryIndexing1) ? 1 : 0;
        bool suspendWorkItemPrimaryIndexing2;
        bool continuationTokens = this.EvaluateContinuationTokens(dictionary3, dictionary4, configValueOrDefault, out suspendWorkItemPrimaryIndexing2, true);
        if (suspendWorkItemPrimaryIndexing1 & suspendWorkItemPrimaryIndexing2)
          suspendPrimaryIndexing = true;
        int num2 = continuationTokens ? 1 : 0;
        return (num1 & num2) != 0;
      }
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no shadow project Indexing Units for WorkItem reindexing")));
        return false;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("There are no primary project Indexing Units for WorkItem reindexing")));
      suspendPrimaryIndexing = true;
      return true;
    }

    private bool EvaluateContinuationTokens(
      Dictionary<Guid, string> primaryProjectToContinuationTokens,
      Dictionary<Guid, string> shadowProjectToContinuationTokens,
      int maxDifferenceInWatermarksForWorkItemContinuationToken,
      out bool suspendWorkItemPrimaryIndexing,
      bool isDiscussionsContinuationToken = false)
    {
      string str = isDiscussionsContinuationToken ? "Discussions" : "Fields";
      suspendWorkItemPrimaryIndexing = false;
      bool continuationTokens = true;
      foreach (Guid key in primaryProjectToContinuationTokens.Keys)
      {
        string continuationToken1;
        if (!shadowProjectToContinuationTokens.TryGetValue(key, out continuationToken1))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Shadow project IndexingUnit for project -> {0} not found. Returning false.", (object) key)));
          return false;
        }
        string continuationToken2 = primaryProjectToContinuationTokens[key];
        if (!string.IsNullOrEmpty(continuationToken2) && !string.IsNullOrEmpty(continuationToken1))
        {
          int continuationToken3 = this.ParseWatermarkFromContinuationToken(continuationToken2, key);
          int continuationToken4 = this.ParseWatermarkFromContinuationToken(continuationToken1, key);
          if (continuationToken4 < continuationToken3)
          {
            if (continuationToken3 - continuationToken4 > maxDifferenceInWatermarksForWorkItemContinuationToken)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("{0} ContinuationToken for Shadow Project Indexing Unit ({1}) is lagging behind Primary Project Indexing Unit", (object) str, (object) continuationToken1)) + FormattableString.Invariant(FormattableStringFactory.Create(" ({0}) by more than the threshold . Project Id is {1}.", (object) continuationToken2, (object) key)));
              return false;
            }
            continuationTokens = false;
          }
        }
        else if (!string.IsNullOrEmpty(continuationToken2) || !string.IsNullOrEmpty(continuationToken1))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("One of either primary or shadow {0} continuation tokens is null or empty for project {1}.", (object) str, (object) key)) + FormattableString.Invariant(FormattableStringFactory.Create(" Primary continuation token {0}, Shadow continuation token {1}.", (object) continuationToken2, (object) continuationToken1)));
          return false;
        }
      }
      if (continuationTokens)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("All project Indexing Units have caught up for {0}.", (object) str)));
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("All project Indexing Units are nearing completion for {0}.", (object) str)));
      suspendWorkItemPrimaryIndexing = true;
      return continuationTokens;
    }

    private int ParseWatermarkFromContinuationToken(string continuationToken, Guid projectId)
    {
      int result;
      if (!int.TryParse(continuationToken.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)[0], out result))
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Could not parse watermark from the continuationToken {0} for project ID - {1}", (object) continuationToken, (object) projectId)));
      return result;
    }

    private string GetDifferencesBetweenIUs(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryProjectIndexingUnits,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowProjectIndexingUnits)
    {
      HashSet<Guid> hashSet1 = primaryProjectIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId)).ToHashSet<Guid>();
      HashSet<Guid> hashSet2 = shadowProjectIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId)).ToHashSet<Guid>();
      string str1 = string.Join<Guid>(",", hashSet1.Except<Guid>((IEnumerable<Guid>) hashSet2));
      string str2 = string.Join<Guid>(",", hashSet2.Except<Guid>((IEnumerable<Guid>) hashSet1));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Primary and Shadow Indexing Units are not equal.")));
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("ProjectsNotInShadow -> {0}, ProjectsNotInPrimary -> {1}.", (object) str1, (object) str2)));
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("TotalProjectsCountInPrimary = {0}. TotalProjectsCountInShadow = {1}", (object) hashSet1.Count, (object) hashSet2.Count)));
      return stringBuilder.ToString();
    }
  }
}
