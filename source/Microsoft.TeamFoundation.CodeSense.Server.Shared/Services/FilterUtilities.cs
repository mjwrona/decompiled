// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.FilterUtilities
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public static class FilterUtilities
  {
    public static int ByRetentionV3(
      int retentionPeriod,
      SourceControlDataV3 sourceControlData,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      List<BranchDetailsResultV3> branchList,
      List<BranchLinkDataV3> branchMap,
      IDictionary<string, HashSet<int>> includedChanges)
    {
      if (retentionPeriod != -1)
      {
        HashSet<string> stringSet = sourceControlData.FilterData(retentionPeriod);
        if (stringSet.Any<string>())
        {
          FilterUtilities.FilterIncludedChanges(includedChanges, (IEnumerable<string>) stringSet);
          FilterUtilities.FilterBranchLinks(branchMap, stringSet);
          FilterUtilities.FilterCodeElements(branchList, codeElementIdentities, stringSet);
          return stringSet.Select<string, int>((Func<string, int>) (c => int.Parse(c))).Max();
        }
      }
      return -1;
    }

    public static void ByRetentionV2Summary(
      IVssRequestContext requestContext,
      string details,
      List<BranchSummaryResult> branchList,
      List<BranchLinkData> branchMap)
    {
      if (details == null)
        return;
      FileDetailsResult fileDetailsResult = JsonConvert.DeserializeObject<FileDetailsResult>(details, CodeSenseSerializationSettings.JsonSerializerSettings);
      if (fileDetailsResult == null)
        return;
      HashSet<string> filteredChangesets = FilterUtilities.ByRetentionV2Details(requestContext, fileDetailsResult.BranchList, branchMap);
      if (!filteredChangesets.Any<string>())
        return;
      foreach (BranchSummaryResult branch in branchList)
      {
        foreach (CodeElementSummaryResult codeElement in branch.CodeElements)
        {
          CollectorResult collectorResult = codeElement.ElementSummaries.FirstOrDefault<CollectorResult>();
          if (collectorResult != null)
          {
            AuthorSummaryCollection data = collectorResult.GetData<AuthorSummaryCollection>();
            if (data != null)
            {
              foreach (AuthorSummary authorSummary in (List<AuthorSummary>) data)
                authorSummary.ChangeSummaries.RemoveAll((Predicate<AuthorSummary.ChangeSummary>) (c => filteredChangesets.Contains(c.Id)));
              data.RemoveAll((Predicate<AuthorSummary>) (a => a.ChangeSummaries.Count == 0));
              if (data.Any<AuthorSummary>())
                collectorResult.Data = (object) data;
              else
                codeElement.ElementSummaries = (IEnumerable<CollectorResult>) null;
            }
          }
        }
        branch.CodeElements.RemoveAll((Predicate<CodeElementSummaryResult>) (e => e.ElementSummaries == null));
      }
      branchList.RemoveAll((Predicate<BranchSummaryResult>) (b => b.CodeElements.Count == 0));
    }

    public static HashSet<string> ByRetentionV2Details(
      IVssRequestContext requestContext,
      List<BranchDetailsResult> branchList,
      List<BranchLinkData> branchMap)
    {
      HashSet<string> filteredChangesets = new HashSet<string>();
      int retentionPeriod = requestContext.GetService<IVssRegistryService>().GetRetentionPeriod(requestContext);
      if (retentionPeriod != -1)
      {
        DateTime windowStart = DateTime.UtcNow.AddMonths(-retentionPeriod);
        branchMap?.RemoveAll((Predicate<BranchLinkData>) (l => l.Date < windowStart));
        if (branchList != null)
        {
          foreach (BranchDetailsResult branch in branchList)
            branch.Details.RemoveAll((Predicate<FileChangeAggregateResult>) (c =>
            {
              CodeElementChangeResult changesetDetails = FilterUtilities.GetChangesetDetails(c);
              if (changesetDetails == null || !(changesetDetails.Date < windowStart))
                return false;
              filteredChangesets.Add(changesetDetails.ChangesId);
              return true;
            }));
          branchList.RemoveAll((Predicate<BranchDetailsResult>) (b => b.Details.Count == 0));
        }
      }
      return filteredChangesets;
    }

    private static CodeElementChangeResult GetChangesetDetails(FileChangeAggregateResult changeset)
    {
      CodeElementDetailsResult elementDetailsResult = changeset.CodeElements.FirstOrDefault<CodeElementDetailsResult>();
      if (elementDetailsResult != null && elementDetailsResult.ElementDetails != null)
      {
        CollectorResult collectorResult = elementDetailsResult.ElementDetails.FirstOrDefault<CollectorResult>();
        if (collectorResult != null)
          return collectorResult.GetData<CodeElementChangeResult>();
      }
      return (CodeElementChangeResult) null;
    }

    private static void FilterIncludedChanges(
      IDictionary<string, HashSet<int>> includedChanges,
      IEnumerable<string> changesetsToFilter)
    {
      if (includedChanges == null || changesetsToFilter == null || !changesetsToFilter.Any<string>())
        return;
      IEnumerable<int> other = changesetsToFilter.Select<string, int>((Func<string, int>) (c => int.Parse(c)));
      foreach (string key in includedChanges.Keys.ToArray<string>())
        includedChanges[key]?.ExceptWith(other);
    }

    private static void FilterCodeElements(
      List<BranchDetailsResultV3> branchList,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      HashSet<string> changesetToFilter)
    {
      if (branchList == null || codeElementIdentities == null || changesetToFilter == null || !changesetToFilter.Any<string>())
        return;
      HashSet<int> codeElementIds = new HashSet<int>((IEnumerable<int>) codeElementIdentities.Identities.Keys);
      foreach (BranchDetailsResultV3 branch in branchList)
      {
        HashSet<int> emptyCodeElements = new HashSet<int>();
        foreach (CodeElementDetailsResultV3 detail in branch.Details)
        {
          CollectorResult collectorResult = detail.ElementDetails.FirstOrDefault<CollectorResult>((Func<CollectorResult, bool>) (r => r.Id == "Microsoft.Changes"));
          if (collectorResult != null)
          {
            CodeElementChangeResultV3 data = collectorResult.GetData<CodeElementChangeResultV3>();
            if (data != null)
            {
              data.Filter(changesetToFilter);
              if (data.IsEmpty)
                emptyCodeElements.Add(detail.Id);
              else
                codeElementIds.Remove(detail.Id);
            }
            collectorResult.Data = (object) data;
          }
        }
        if (emptyCodeElements.Any<int>())
          branch.Details.RemoveAll((Predicate<CodeElementDetailsResultV3>) (d => emptyCodeElements.Contains(d.Id)));
      }
      branchList.RemoveAll((Predicate<BranchDetailsResultV3>) (b => b.Details.Count == 0));
      codeElementIdentities.Remove(codeElementIds);
    }

    private static void FilterBranchLinks(
      List<BranchLinkDataV3> branchLinks,
      HashSet<string> changesetsToFilter)
    {
      if (branchLinks == null || changesetsToFilter == null || !changesetsToFilter.Any<string>())
        return;
      branchLinks.RemoveAll((Predicate<BranchLinkDataV3>) (l => changesetsToFilter.Contains(l.TargetChangesId)));
    }
  }
}
