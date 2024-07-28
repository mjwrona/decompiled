// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyWork.WidgetSettingsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyWork, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8442996D-DF5E-4B6F-9622-CCF23EF07ED1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.MyWork.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.MyWork
{
  public class WidgetSettingsModel
  {
    public string WidgetName { get; set; }

    public Dictionary<string, string> Settings { get; set; }
  }
}
