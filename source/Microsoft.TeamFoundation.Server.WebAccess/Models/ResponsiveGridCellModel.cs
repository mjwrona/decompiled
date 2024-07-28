// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.ResponsiveGridCellModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  public class ResponsiveGridCellModel
  {
    public int SectionNumber { get; set; }

    public int Columns { get; set; }

    public int Rows { get; set; }

    public string Area { get; set; }

    public string ViewName { get; set; }

    public object Model { get; set; }

    public string ControlString { get; set; }

    public bool AdjustHeight { get; set; }
  }
}
