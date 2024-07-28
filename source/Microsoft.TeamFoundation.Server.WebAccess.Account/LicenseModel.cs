// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.LicenseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
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
