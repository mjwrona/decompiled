// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.BranchLinkDataV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class BranchLinkDataV3 : IBranchMap
  {
    public BranchLinkDataV3(
      string sourcePath,
      int sourceChangesIdFrom,
      int sourceChangesIdTo,
      string targetPath,
      int targetChangesId,
      VersionControlChangeType changeType)
      : this(sourcePath, sourceChangesIdFrom.ToString(), sourceChangesIdTo.ToString(), targetPath, targetChangesId.ToString(), changeType)
    {
    }

    public BranchLinkDataV3(
      string sourcePath,
      string sourceChangesIdFrom,
      string sourceChangesIdTo,
      string targetPath,
      string targetChangesId,
      VersionControlChangeType changeType)
    {
      this.SourcePath = sourcePath;
      this.SourceChangesIdFrom = sourceChangesIdFrom;
      this.SourceChangesIdTo = sourceChangesIdTo;
      this.TargetPath = targetPath;
      this.TargetChangesId = targetChangesId;
      this.ChangeType = changeType;
    }

    public BranchLinkDataV3(BranchLinkData branchLink)
    {
      this.SourcePath = branchLink.SourcePath;
      this.SourceChangesIdFrom = branchLink.SourceChangesIdFrom;
      this.SourceChangesIdTo = branchLink.SourceChangesIdTo;
      this.TargetPath = branchLink.TargetPath;
      this.TargetChangesId = branchLink.TargetChangesId;
      this.ChangeType = branchLink.ChangeType;
    }

    [JsonConstructor]
    public BranchLinkDataV3()
    {
    }

    [JsonProperty]
    public string SourcePath { get; set; }

    [JsonProperty]
    public string SourceChangesIdFrom { get; private set; }

    [JsonProperty]
    public string SourceChangesIdTo { get; private set; }

    [JsonProperty]
    public string TargetPath { get; set; }

    [JsonProperty]
    public string TargetChangesId { get; private set; }

    [JsonProperty]
    [JsonConverter(typeof (StringEnumConverter))]
    public VersionControlChangeType ChangeType { get; private set; }
  }
}
