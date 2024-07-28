// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.LastMentionedInfo
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using System;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class LastMentionedInfo
  {
    public string TargetId { get; set; }

    public string ArtifactType { get; set; }

    public string NormalizedSourceId { get; set; }

    public string SourceType { get; set; }

    public DateTime LastMentionedDate { get; set; }
  }
}
