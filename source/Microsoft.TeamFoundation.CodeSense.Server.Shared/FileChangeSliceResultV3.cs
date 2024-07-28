// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.FileChangeSliceResultV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class FileChangeSliceResultV3
  {
    public FileChangeSliceResultV3()
      : this(string.Empty, -1, (IEnumerable<CodeElementDetailsResultV3>) new List<CodeElementDetailsResultV3>(), (IEnumerable<string>) new List<string>(), (IEnumerable<BranchLinkDataV3>) new List<BranchLinkDataV3>(), new CodeElementIdentityCollectionV3(), new SourceControlDataV3())
    {
    }

    public FileChangeSliceResultV3(
      string aggregatePath,
      int changesId,
      IEnumerable<CodeElementDetailsResultV3> details,
      IEnumerable<string> contributingServerPaths,
      IEnumerable<BranchLinkDataV3> branchLinks,
      CodeElementIdentityCollectionV3 codeElementIdentities,
      SourceControlDataV3 sourceControlData)
    {
      this.AggregatePath = aggregatePath;
      this.ChangesId = changesId;
      this.Details = details;
      this.ContributingServerPaths = contributingServerPaths;
      this.BranchLinks = branchLinks;
      this.CodeElementIdentities = codeElementIdentities;
      this.SourceControlData = sourceControlData;
    }

    public FileChangeSliceResultV3(FileChangeSliceResult slice)
    {
      this.AggregatePath = slice.AggregatePath;
      this.ChangesId = slice.ChangesId;
      this.ContributingServerPaths = slice.ContributingServerPaths;
      this.BranchLinks = slice.BranchLinks != null ? slice.BranchLinks.Select<BranchLinkData, BranchLinkDataV3>((Func<BranchLinkData, BranchLinkDataV3>) (link => new BranchLinkDataV3(link))) : Enumerable.Empty<BranchLinkDataV3>();
      this.CodeElementIdentities = new CodeElementIdentityCollectionV3();
      this.SourceControlData = new SourceControlDataV3();
      this.Details = this.GetCodeElementDetails(slice);
    }

    [JsonProperty]
    public string AggregatePath { get; set; }

    [JsonProperty]
    public int ChangesId { get; set; }

    [JsonProperty]
    public string Schema => "Microsoft.CodeSense.FileChangeSliceResult,3.0";

    [JsonProperty]
    public IEnumerable<CodeElementDetailsResultV3> Details { get; set; }

    [JsonProperty]
    public CodeElementIdentityCollectionV3 CodeElementIdentities { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<string> ContributingServerPaths { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<BranchLinkDataV3> BranchLinks { get; set; }

    [JsonProperty]
    public SourceControlDataV3 SourceControlData { get; set; }

    private IEnumerable<CodeElementDetailsResultV3> GetCodeElementDetails(
      FileChangeSliceResult slice)
    {
      List<CodeElementDetailsResultV3> codeElementDetails = new List<CodeElementDetailsResultV3>();
      if (slice.CodeElements != null)
      {
        foreach (CodeElementDetailsResult codeElement in slice.CodeElements)
        {
          int id = this.CodeElementIdentities.Add(new CodeElementIdentityV3(codeElement.Id, codeElement.Kind));
          codeElementDetails.Add(new CodeElementDetailsResultV3(id, codeElement.ElementDetails));
          if (codeElement.ElementDetails != null)
          {
            foreach (CollectorResult elementDetail in codeElement.ElementDetails)
            {
              CodeElementChangeResult data = elementDetail.GetData<CodeElementChangeResult>();
              if (data != null)
              {
                elementDetail.Data = (object) new CodeElementChangeResultV3((IDictionary<ElementChangeKind, HashSet<string>>) new Dictionary<ElementChangeKind, HashSet<string>>()
                {
                  {
                    data.ChangeKind,
                    new HashSet<string>() { data.ChangesId }
                  }
                });
                this.SourceControlData.FetchData(data);
              }
            }
          }
        }
      }
      return (IEnumerable<CodeElementDetailsResultV3>) codeElementDetails;
    }
  }
}
