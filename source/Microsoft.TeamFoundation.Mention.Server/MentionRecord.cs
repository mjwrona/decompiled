// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionRecord
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using System;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class MentionRecord
  {
    public string SourceId { get; set; }

    public string SourceType { get; set; }

    public int SourceDataspaceId { get; set; }

    public string RawText { get; set; }

    public string ArtifactId { get; set; }

    public string ArtifactType { get; set; }

    public int? CommentId { get; set; }

    public string MentionAction { get; set; }

    public string TargetId { get; set; }

    public Guid? StorageKey { get; set; }

    public int TargetDataspaceId { get; set; }

    public string NormalizedSourceId { get; set; }

    public MentionUpdateState UpdateState { get; set; }
  }
}
