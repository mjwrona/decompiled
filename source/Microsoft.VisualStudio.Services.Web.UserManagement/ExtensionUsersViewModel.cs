// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.ExtensionUsersViewModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class ExtensionUsersViewModel
  {
    public List<UserModel> Users { get; set; }

    public ExtensionAvailabilityViewModel ExtensionAvailabilityViewModel { get; set; }

    public List<string> ExtensionErrors { get; set; }

    public bool InGracePeriod { get; set; }

    public bool GracePeriodExpired { get; set; }

    public string GracePeriodEndDate { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["Extension"] = (object) this.ExtensionAvailabilityViewModel.ToJson();
      json["ExtensionErrors"] = (object) this.ExtensionErrors;
      json["InGracePeriod"] = (object) this.InGracePeriod;
      json["GracePeriodExpired"] = (object) this.GracePeriodExpired;
      json["GracePeriodEndDate"] = (object) this.GracePeriodEndDate;
      return json;
    }
  }
}
