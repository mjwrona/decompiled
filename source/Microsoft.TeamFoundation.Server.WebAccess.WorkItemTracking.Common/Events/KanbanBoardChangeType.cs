// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events.KanbanBoardChangeType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum KanbanBoardChangeType
  {
    UpdateColumns = 1,
    UpdateRows = 2,
    UpdateCardRules = 4,
    UpdateCardSettings = 8,
    UpdateCardReorderingOptions = 16, // 0x00000010
    Create = 1024, // 0x00000400
    Delete = 2048, // 0x00000800
    SoftDelete = 4096, // 0x00001000
    Restore = 8192, // 0x00002000
  }
}
