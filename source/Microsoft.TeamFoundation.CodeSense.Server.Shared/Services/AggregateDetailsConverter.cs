// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.AggregateDetailsConverter
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
  public static class AggregateDetailsConverter
  {
    public static string GetV2Details(
      string v3Details,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      if (string.IsNullOrEmpty(v3Details))
        return string.Empty;
      FileDetailsResultV3 detailsResult = JsonConvert.DeserializeObject<FileDetailsResultV3>(v3Details, CodeSenseSerializationSettings.JsonSerializerSettings);
      if (detailsResult != null)
      {
        FileDetailsResult v2Details = AggregateDetailsConverter.GetV2Details(detailsResult, codeElementIdentities, sourceControlData);
        if (v2Details != null)
          return JsonConvert.SerializeObject((object) v2Details, CodeSenseSerializationSettings.JsonSerializerSettings);
      }
      return (string) null;
    }

    public static FileDetailsResult GetV2Details(
      FileDetailsResultV3 detailsResult,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      if (detailsResult == null || detailsResult.BranchList == null)
        return (FileDetailsResult) null;
      List<BranchDetailsResult> branchList = new List<BranchDetailsResult>();
      foreach (BranchDetailsResultV3 branch in detailsResult.BranchList)
        branchList.Add(AggregateDetailsConverter.GetBranchDetails(branch, codeElementIdentities, sourceControlData));
      return new FileDetailsResult(branchList);
    }

    private static BranchDetailsResult GetBranchDetails(
      BranchDetailsResultV3 branch,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      Dictionary<string, List<CodeElementDetailsResult>> dictionary = new Dictionary<string, List<CodeElementDetailsResult>>();
      foreach (CodeElementDetailsResultV3 detail1 in branch.Details)
      {
        CodeElementIdentityV3 codeElementIdentity = codeElementIdentities[detail1.Id];
        foreach (CollectorResult collectorResult in detail1.ElementDetails.Where<CollectorResult>((Func<CollectorResult, bool>) (c => c.Id == "Microsoft.Changes")))
        {
          CodeElementChangeResultV3 data = collectorResult.GetData<CodeElementChangeResultV3>();
          if (data != null)
          {
            foreach (KeyValuePair<ElementChangeKind, HashSet<string>> detail2 in (IEnumerable<KeyValuePair<ElementChangeKind, HashSet<string>>>) data.Details)
            {
              foreach (string str in detail2.Value)
              {
                IEnumerable<CollectorResult> oldChangeResult = AggregateDetailsConverter.GetOldChangeResult(str, detail2.Key, sourceControlData);
                if (oldChangeResult != null)
                {
                  CodeElementDetailsResult elementDetailsResult = new CodeElementDetailsResult(codeElementIdentity.Id, codeElementIdentity.ElementKind, oldChangeResult);
                  if (!dictionary.ContainsKey(str))
                    dictionary.Add(str, new List<CodeElementDetailsResult>()
                    {
                      elementDetailsResult
                    });
                  else
                    dictionary[str].Add(elementDetailsResult);
                }
              }
            }
          }
        }
      }
      return new BranchDetailsResult(branch.ServerPath, dictionary.Values.Select<List<CodeElementDetailsResult>, FileChangeAggregateResult>((Func<List<CodeElementDetailsResult>, FileChangeAggregateResult>) (d => new FileChangeAggregateResult((IEnumerable<CodeElementDetailsResult>) d))).ToList<FileChangeAggregateResult>());
    }

    private static IEnumerable<CollectorResult> GetOldChangeResult(
      string changesetId,
      ElementChangeKind changeKind,
      SourceControlDataV3 sourceControlData)
    {
      CommitDataV3 changeset = sourceControlData.FindChangeset(changesetId);
      if (changeset == null)
        return (IEnumerable<CollectorResult>) Array.Empty<CollectorResult>();
      return (IEnumerable<CollectorResult>) new CollectorResult[1]
      {
        new CollectorResult("Microsoft.Changes", (object) new CodeElementChangeResult(changeKind, changesetId, changeset.ChangesComment, changeset.Date, ConverterUtilities.GetWorkItems(changeset.WorkItems, sourceControlData), sourceControlData.FindUser(changeset.AuthorUniqueName)), "1.0")
      };
    }
  }
}
