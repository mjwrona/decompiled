// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.ExtensionAvailabilityViewModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class ExtensionAvailabilityViewModel
  {
    public ExtensionViewModel ExtensionViewModel { get; set; }

    public int IncludedQuantity { get; set; }

    public int InUse { get; set; }

    public int Total { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["ExtensionId"] = (object) this.ExtensionViewModel.ExtensionId;
      json["DisplayName"] = (object) this.ExtensionViewModel.DisplayName;
      json["Total"] = (object) this.Total;
      json["InUse"] = (object) this.InUse;
      json["IncludedQuantity"] = (object) this.IncludedQuantity;
      return json;
    }
  }
}
