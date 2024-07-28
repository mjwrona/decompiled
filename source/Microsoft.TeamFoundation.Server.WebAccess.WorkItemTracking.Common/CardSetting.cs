// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CardSetting
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class CardSetting : List<FieldSetting>
  {
    public string ItemName { get; set; }

    public CardSetting(string name) => this.ItemName = name;

    public CardSetting(string name, List<BoardCardSettingRow> fieldSettingRows)
      : this(name)
    {
      foreach (IGrouping<string, BoardCardSettingRow> grouping in fieldSettingRows.GroupBy<BoardCardSettingRow, string>((Func<BoardCardSettingRow, string>) (fieldSettingGroup => fieldSettingGroup.Field)))
      {
        IGrouping<string, BoardCardSettingRow> fieldGroup = grouping;
        FieldSetting fieldSetting = new FieldSetting(fieldGroup.Key, fieldGroup.Select<BoardCardSettingRow, KeyValuePair<string, string>>((Func<BoardCardSettingRow, KeyValuePair<string, string>>) (item => new KeyValuePair<string, string>(item.Property, item.Value))).ToList<KeyValuePair<string, string>>());
        if (this.FindIndex((Predicate<FieldSetting>) (field => VssStringComparer.PropertyName.Equals(field.FieldIdentifier, fieldGroup.Key))) < 0)
          this.Add(fieldSetting);
      }
    }
  }
}
