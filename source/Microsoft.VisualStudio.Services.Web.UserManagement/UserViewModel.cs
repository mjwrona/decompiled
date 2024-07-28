// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.UserViewModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class UserViewModel
  {
    public List<UserModel> Users { get; set; }

    public List<LicenseModel> Licenses { get; set; }

    public Dictionary<string, LicenseModel> licenseDictionary { get; set; }

    public Dictionary<string, int> LicenseOverview { get; set; }

    public List<string> LicenseErrors { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["Licenses"] = (object) this.Licenses.Select<LicenseModel, JsObject>((Func<LicenseModel, JsObject>) (x => x.ToJson()));
      json["LicenseErrors"] = (object) this.LicenseErrors;
      json["Dictionary"] = (object) this.licenseDictionary.Select<KeyValuePair<string, LicenseModel>, JsObject>((Func<KeyValuePair<string, LicenseModel>, JsObject>) (x => x.Value.ToJson()));
      return json;
    }
  }
}
