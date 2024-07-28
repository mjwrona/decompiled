// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.FileChangeSliceResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class FileChangeSliceResult
  {
    public FileChangeSliceResult(IEnumerable<CodeElementDetailsResult> codeElements)
      : this(codeElements, Enumerable.Empty<string>(), Enumerable.Empty<BranchLinkData>())
    {
    }

    public FileChangeSliceResult(
      IEnumerable<CodeElementDetailsResult> codeElements,
      IEnumerable<string> contributingServerPaths,
      IEnumerable<BranchLinkData> branchLinks)
      : this(string.Empty, -1, codeElements, contributingServerPaths, branchLinks)
    {
    }

    public FileChangeSliceResult(
      string aggregatePath,
      int changesId,
      IEnumerable<CodeElementDetailsResult> codeElements,
      IEnumerable<string> contributingServerPaths,
      IEnumerable<BranchLinkData> branchLinks)
    {
      ArgumentUtility.CheckForNull<IEnumerable<CodeElementDetailsResult>>(codeElements, nameof (codeElements));
      this.AggregatePath = aggregatePath;
      this.ChangesId = changesId;
      this.CodeElements = codeElements;
      this.ContributingServerPaths = contributingServerPaths;
      this.BranchLinks = branchLinks;
    }

    [JsonConstructor]
    private FileChangeSliceResult()
    {
    }

    [JsonProperty]
    public string AggregatePath { get; set; }

    [JsonProperty]
    public int ChangesId { get; set; }

    [JsonProperty]
    public string Schema => "Microsoft.CodeSense.FileChangeSliceResult,1.0";

    [JsonProperty]
    public IEnumerable<CodeElementDetailsResult> CodeElements { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<string> ContributingServerPaths { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<BranchLinkData> BranchLinks { get; private set; }
  }
}
