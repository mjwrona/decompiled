// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.BranchDetailsResultV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class BranchDetailsResultV3 : IBranchResult
  {
    public BranchDetailsResultV3()
      : this(string.Empty, new List<CodeElementDetailsResultV3>())
    {
    }

    public BranchDetailsResultV3(string serverPath, List<CodeElementDetailsResultV3> details)
    {
      this.ServerPath = serverPath;
      this.Details = details;
    }

    [JsonProperty]
    public string ServerPath { get; set; }

    [JsonProperty]
    public List<CodeElementDetailsResultV3> Details { get; set; }
  }
}
