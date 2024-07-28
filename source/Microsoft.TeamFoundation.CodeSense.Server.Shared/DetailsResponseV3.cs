// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.DetailsResponseV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class DetailsResponseV3
  {
    public DetailsResponseV3(
      DateTime timeStamp,
      string resourceVersion,
      bool indexComplete,
      DateTime? indexCompleteTo,
      int retentionPeriod,
      IEnumerable<BranchLinkDataV3> branchMap,
      IList<BranchDetailsResultV3> branchList,
      CodeElementIdentityCollectionV3 codeElements,
      SourceControlDataV3 sourceControlData)
    {
      this.TimeStamp = timeStamp;
      this.ResourceVersion = resourceVersion;
      this.IndexComplete = indexComplete;
      this.IndexCompleteTo = indexCompleteTo;
      this.RetentionPeriod = retentionPeriod;
      this.BranchMap = branchMap;
      this.BranchList = branchList;
      this.CodeElements = codeElements;
      this.SourceControlData = sourceControlData;
    }

    [JsonProperty]
    public DateTime TimeStamp { get; private set; }

    [JsonProperty]
    public string ResourceVersion { get; private set; }

    [JsonProperty]
    public bool IndexComplete { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? IndexCompleteTo { get; private set; }

    [JsonProperty]
    public int RetentionPeriod { get; private set; }

    [JsonProperty]
    public IEnumerable<BranchLinkDataV3> BranchMap { get; private set; }

    [JsonProperty]
    public IList<BranchDetailsResultV3> BranchList { get; private set; }

    [JsonProperty]
    public CodeElementIdentityCollectionV3 CodeElements { get; private set; }

    [JsonProperty]
    public SourceControlDataV3 SourceControlData { get; private set; }
  }
}
