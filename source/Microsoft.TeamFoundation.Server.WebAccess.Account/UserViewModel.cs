// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.UserViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
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
