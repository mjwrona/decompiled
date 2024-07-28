// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.FileDetailsResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class FileDetailsResult
  {
    public FileDetailsResult()
      : this(new List<BranchDetailsResult>())
    {
    }

    public FileDetailsResult(List<BranchDetailsResult> branchList)
    {
      this.ResourceVersion = "2.0";
      this.BranchList = branchList;
    }

    [JsonProperty]
    public string ResourceVersion { get; private set; }

    [JsonProperty]
    public List<BranchDetailsResult> BranchList { get; private set; }
  }
}
