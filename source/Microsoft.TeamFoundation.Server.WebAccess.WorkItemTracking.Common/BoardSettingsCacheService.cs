// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardSettingsCacheService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BoardSettingsCacheService : VssMemoryCacheService<string, BoardSettingsDTO>
  {
    private const int c_maxCacheSize = 8388608;
    private const int c_boardSize = 250;
    private const int c_rowSize = 250;
    private const int c_columnSize = 500;
    private const int c_cardSettingSize = 600;
    private const int c_cardRuleSize = 500;
    private const int c_cardAttributeSize = 500;

    private static MemoryCacheConfiguration<string, BoardSettingsDTO> GetMemoryCacheConfiguration() => new MemoryCacheConfiguration<string, BoardSettingsDTO>().WithMaxSize(8388608L, (ISizeProvider<string, BoardSettingsDTO>) new BoardSettingsCacheService.SizeProvider());

    public BoardSettingsCacheService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, BoardSettingsCacheService.GetMemoryCacheConfiguration())
    {
    }

    private class SizeProvider : ISizeProvider<string, BoardSettingsDTO>
    {
      public long GetSize(string key, BoardSettingsDTO value) => (long) (250 + 250 * value.Rows.Count<BoardRowTable>() + 500 * value.Columns.Count<BoardColumnRow>() + (value.BoardCardSettings != null ? 600 * value.BoardCardSettings.Count<BoardCardSettingRow>() : 0) + (value.BoardCardRules.BoardCardRules != null ? 500 * value.BoardCardRules.BoardCardRules.Count<BoardCardRuleRow>() : 0) + (value.BoardCardRules.BoardCardRuleAttributes != null ? 500 * value.BoardCardRules.BoardCardRuleAttributes.Count<CardRuleAttributeRow>() : 0));
    }
  }
}
