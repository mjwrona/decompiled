// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Contracts.BranchSummaryResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Contracts
{
  public class BranchSummaryResult : IBranchResult
  {
    public BranchSummaryResult(string serverPath, List<CodeElementSummaryResult> codeElements)
    {
      this.ServerPath = serverPath;
      this.CodeElements = codeElements;
    }

    [JsonConstructor]
    private BranchSummaryResult()
    {
    }

    [JsonProperty]
    public string ServerPath { get; set; }

    [JsonProperty]
    public List<CodeElementSummaryResult> CodeElements { get; set; }

    public void AddCodeElements(IEnumerable<CodeElementSummaryResult> elements)
    {
      List<CodeElementSummaryResult> list = this.CodeElements.ToList<CodeElementSummaryResult>();
      list.AddRange(elements);
      this.CodeElements = list;
    }
  }
}
