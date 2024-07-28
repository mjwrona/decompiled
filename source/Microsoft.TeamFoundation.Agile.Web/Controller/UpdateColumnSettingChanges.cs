// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.UpdateColumnSettingChanges
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public class UpdateColumnSettingChanges
  {
    public bool IsColumnNameChanged;
    public bool IsWIPLimitChanged;
    public bool IsSplitColumnStateChanged;
    public bool IsDescriptionChanged;
    public bool IsColumnStateChanged;
    public bool IsColumnOrderChanged;
    public int TotalColumnCount;
    public int NewColumnCount;
    public int DeletedColumnCount;
    public int DescriptionColumnCount;
    public int SplitColumnCount;
    public int AverageWipLimitInProgressColumn;
  }
}
