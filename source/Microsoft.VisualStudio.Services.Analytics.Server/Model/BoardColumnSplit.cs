// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.BoardColumnSplit
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum BoardColumnSplit : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_BOARD_COLUMN_SPLIT_DOING", false)] Doing,
    [LocalizedDisplayName("ENUM_TYPE_BOARD_COLUMN_SPLIT_DONE", false)] Done,
    [LocalizedDisplayName("ENUM_TYPE_BOARD_COLUMN_SPLIT_UNKNOWN", false)] Unknown,
  }
}
