// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.SecurityModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class SecurityModel
  {
    public bool isAdmin { get; set; }

    public bool isAccountOwner { get; set; }

    public bool inTrail { get; set; }

    public bool displayManageLink { get; set; }

    public string licenseLink { get; set; }

    public string noLicenseMessageLinked { get; set; }

    public string monthlyCycle { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("isAdmin", (object) this.isAdmin);
      json.Add("isAccountOwner", (object) this.isAccountOwner);
      json.Add("displayLink", (object) this.displayManageLink);
      json.Add("licenseLink", (object) this.licenseLink);
      json.Add("noLicenseLink", (object) this.noLicenseMessageLinked);
      json.Add("monthlyCycle", (object) this.monthlyCycle);
      json.Add("inTrail", (object) this.inTrail);
      return json;
    }
  }
}
