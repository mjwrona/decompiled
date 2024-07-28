// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.AggregateSummaryConverter
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public static class AggregateSummaryConverter
  {
    public static string GetV2Summary(
      string v3Details,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      if (string.IsNullOrEmpty(v3Details))
        return string.Empty;
      FileDetailsResultV3 detailsResult = JsonConvert.DeserializeObject<FileDetailsResultV3>(v3Details, CodeSenseSerializationSettings.JsonSerializerSettings);
      if (detailsResult != null)
      {
        FileSummaryResult v2Summary = AggregateSummaryConverter.GetV2Summary(detailsResult, codeElementIdentities, sourceControlData);
        if (v2Summary != null)
          return JsonConvert.SerializeObject((object) v2Summary, CodeSenseSerializationSettings.JsonSerializerSettings);
      }
      return (string) null;
    }

    public static FileSummaryResult GetV2Summary(
      FileDetailsResultV3 detailsResult,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      if (detailsResult == null || detailsResult.BranchList == null)
        return (FileSummaryResult) null;
      List<BranchSummaryResult> branchList = new List<BranchSummaryResult>();
      foreach (BranchDetailsResultV3 branch in detailsResult.BranchList)
        branchList.Add(AggregateSummaryConverter.GetBranchSummary(branch, codeElementIdentities, sourceControlData));
      return new FileSummaryResult((IList<BranchSummaryResult>) branchList);
    }

    private static BranchSummaryResult GetBranchSummary(
      BranchDetailsResultV3 branch,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      List<CodeElementSummaryResult> codeElements = new List<CodeElementSummaryResult>();
      foreach (CodeElementDetailsResultV3 detail in branch.Details)
      {
        CodeElementIdentityV3 codeElementIdentity = codeElementIdentities[detail.Id];
        foreach (CollectorResult collectorResult in detail.ElementDetails.Where<CollectorResult>((Func<CollectorResult, bool>) (c => c.Id == "Microsoft.Changes")))
        {
          CodeElementChangeResultV3 data = collectorResult.GetData<CodeElementChangeResultV3>();
          if (data != null)
          {
            IEnumerable<CollectorResult> summaryCollection = AggregateSummaryConverter.GetAuthorSummaryCollection(data, sourceControlData);
            codeElements.Add(new CodeElementSummaryResult(codeElementIdentity.Id, codeElementIdentity.ElementKind, summaryCollection));
          }
        }
      }
      return new BranchSummaryResult(branch.ServerPath, codeElements);
    }

    private static IEnumerable<CollectorResult> GetAuthorSummaryCollection(
      CodeElementChangeResultV3 data,
      SourceControlDataV3 sourceControlData)
    {
      Dictionary<string, AuthorSummary> dictionary = new Dictionary<string, AuthorSummary>();
      foreach (KeyValuePair<ElementChangeKind, HashSet<string>> detail in (IEnumerable<KeyValuePair<ElementChangeKind, HashSet<string>>>) data.Details)
      {
        foreach (string changesetId in detail.Value)
        {
          CommitDataV3 changeset = sourceControlData.FindChangeset(changesetId);
          if (changeset != null)
          {
            AuthorSummary.ChangeSummary changeSummary = new AuthorSummary.ChangeSummary(changeset.ChangesId, ConverterUtilities.GetWorkItems(changeset.WorkItems, sourceControlData));
            if (!dictionary.ContainsKey(changeset.AuthorUniqueName))
            {
              UserData user = sourceControlData.FindUser(changeset.AuthorUniqueName);
              AuthorSummary authorSummary = new AuthorSummary(user.UniqueName, user.DisplayName, (IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
              {
                changeSummary
              });
              dictionary.Add(changeset.AuthorUniqueName, authorSummary);
            }
            else
              dictionary[changeset.AuthorUniqueName].AppendChangeSummaries((IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
              {
                changeSummary
              });
          }
        }
      }
      return (IEnumerable<CollectorResult>) new CollectorResult[1]
      {
        new CollectorResult("Microsoft.Changes", (object) dictionary.Values, "1.0")
      };
    }
  }
}
