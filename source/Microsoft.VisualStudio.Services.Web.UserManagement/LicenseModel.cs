// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.LicenseModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public sealed class LicenseModel
  {
    public string LicenseType { get; set; }

    public int Available { get; set; }

    public int Maximum { get; set; }

    public int InUse { get; set; }

    public int IncludedQuantity { get; set; }

    public string LicenseEnum { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["LicenseType"] = (object) this.LicenseType;
      json["Available"] = (object) this.Available;
      json["LicenseEnum"] = (object) this.LicenseEnum;
      json["Maximum"] = (object) this.Maximum;
      json["InUse"] = (object) this.InUse;
      json["IncludedQuantity"] = (object) this.IncludedQuantity;
      return json;
    }
  }
}
