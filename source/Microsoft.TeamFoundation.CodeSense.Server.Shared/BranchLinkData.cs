// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.BranchLinkData
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class BranchLinkData : IBranchMap
  {
    public BranchLinkData(
      string sourcePath,
      int sourceChangesIdFrom,
      int sourceChangesIdTo,
      string targetPath,
      int targetChangesId,
      VersionControlChangeType changeType,
      string authorDisplayName,
      string authorUniqueName,
      string authorEmail,
      string changesComment,
      DateTime date)
      : this(sourcePath, sourceChangesIdFrom.ToString(), sourceChangesIdTo.ToString(), targetPath, targetChangesId.ToString(), changeType, authorDisplayName, authorUniqueName, authorEmail, changesComment, date)
    {
    }

    public BranchLinkData(
      string sourcePath,
      string sourceChangesIdFrom,
      string sourceChangesIdTo,
      string targetPath,
      string targetChangesId,
      VersionControlChangeType changeType,
      string authorDisplayName,
      string authorUniqueName,
      string authorEmail,
      string changesComment,
      DateTime date)
    {
      this.SourcePath = sourcePath;
      this.SourceChangesIdFrom = sourceChangesIdFrom;
      this.SourceChangesIdTo = sourceChangesIdTo;
      this.TargetPath = targetPath;
      this.TargetChangesId = targetChangesId;
      this.ChangeType = changeType;
      this.AuthorDisplayName = authorDisplayName;
      this.AuthorUniqueName = authorUniqueName;
      this.AuthorEmail = authorEmail;
      this.ChangesComment = changesComment;
      this.Date = date;
    }

    [JsonConstructor]
    private BranchLinkData()
    {
    }

    [JsonProperty]
    public string SourcePath { get; set; }

    [JsonProperty]
    public string SourceChangesIdFrom { get; set; }

    [JsonProperty]
    public string SourceChangesIdTo { get; set; }

    [JsonProperty]
    public string TargetPath { get; set; }

    [JsonProperty]
    public string TargetChangesId { get; set; }

    [JsonProperty]
    [JsonConverter(typeof (StringEnumConverter))]
    public VersionControlChangeType ChangeType { get; set; }

    [JsonProperty]
    public string AuthorDisplayName { get; set; }

    [JsonProperty]
    public string AuthorUniqueName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string AuthorEmail { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ChangesComment { get; set; }

    [JsonProperty]
    public DateTime Date { get; set; }
  }
}
