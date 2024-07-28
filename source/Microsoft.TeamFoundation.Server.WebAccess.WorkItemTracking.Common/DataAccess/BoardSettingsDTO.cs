// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsDTO
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsDTO
  {
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public Guid ExtensionId { get; set; }

    public string BacklogLevelId { get; set; }

    public DateTime ExtensionLastChangedDate { get; set; }

    public IEnumerable<BoardRowTable> Rows { get; set; }

    public IEnumerable<BoardColumnRow> Columns { get; set; }

    public IEnumerable<BoardCardSettingRow> BoardCardSettings { get; set; }

    public BoardCardRulesDTO BoardCardRules { get; set; }

    public BoardOptionRecord Options { get; set; }
  }
}
