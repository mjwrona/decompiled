// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.SecurityModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class SecurityModel
  {
    public bool isAdmin { get; set; }

    public bool isAadAccount { get; set; }

    public bool isAccountOwner { get; set; }

    public bool inTrail { get; set; }

    public bool displayManageLink { get; set; }

    public string licenseLink { get; set; }

    public string noLicenseMessageLinked { get; set; }

    public string monthlyCycle { get; set; }

    public Dictionary<string, bool> FeatureFlags { get; set; }

    public string tenantName { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("isAdmin", (object) this.isAdmin);
      json.Add("isAadAccount", (object) this.isAadAccount);
      json.Add("isAccountOwner", (object) this.isAccountOwner);
      json.Add("displayLink", (object) this.displayManageLink);
      json.Add("licenseLink", (object) this.licenseLink);
      json.Add("noLicenseLink", (object) this.noLicenseMessageLinked);
      json.Add("monthlyCycle", (object) this.monthlyCycle);
      json.Add("inTrail", (object) this.inTrail);
      json.Add("featureFlags", (object) this.FeatureFlags);
      json.Add("tenantName", (object) this.tenantName);
      return json;
    }
  }
}
