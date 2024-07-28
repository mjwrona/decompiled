// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsMaxLengths
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsMaxLengths
  {
    NodePropertyNameLength = 120, // 0x00000078
    FieldNameLength = 128, // 0x00000080
    FileCommentLength = 255, // 0x000000FF
    FileDisplayName = 255, // 0x000000FF
    KeywordLength = 255, // 0x000000FF
    LinkCommentLength = 255, // 0x000000FF
    LinkURIType = 255, // 0x000000FF
    LinkURLDisplayName = 255, // 0x000000FF
    NodeNameLength = 255, // 0x000000FF
    StoredQueryDescription = 255, // 0x000000FF
    StoredQueryName = 255, // 0x000000FF
    ConstantLength = 256, // 0x00000100
    DescriptionLength = 1024, // 0x00000400
    ToolTipLength = 1024, // 0x00000400
    StoredQueryFormatting = 1250, // 0x000004E2
    StoredQueryText = 2000, // 0x000007D0
    FilePath = 2083, // 0x00000823
    LinkURITarget = 2083, // 0x00000823
    LinkURLTarget = 2083, // 0x00000823
    TextLength = 32000, // 0x00007D00
  }
}
