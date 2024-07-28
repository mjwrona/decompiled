// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettingRow
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardCardSettingRow
  {
    public BoardCardSettingRow()
    {
    }

    public BoardCardSettingRow(string field, string property, string value)
    {
      this.Field = field;
      this.Property = property;
      this.Value = value;
    }

    public string Type { get; set; }

    public string Field { get; set; }

    public string Property { get; set; }

    public string Value { get; set; }
  }
}
