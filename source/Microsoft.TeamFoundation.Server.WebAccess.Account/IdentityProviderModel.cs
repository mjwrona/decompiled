// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.IdentityProviderModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class IdentityProviderModel
  {
    public string IdentityProviderDisplayURL { get; set; }

    public string IdentityProviderURL { get; set; }

    public bool StaySignedIn { get; set; }

    public string StaySignedInCookiePath { get; set; }

    public bool Force { get; set; }

    public bool AutoSelect { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["identityProviderDisplayURL"] = (object) this.IdentityProviderDisplayURL;
      json["identityProviderURL"] = (object) this.IdentityProviderURL;
      json["staySignedIn"] = (object) this.StaySignedIn;
      json["staySignedInCookiePath"] = (object) this.StaySignedInCookiePath;
      json["force"] = (object) this.Force;
      json["autoSelect"] = (object) this.AutoSelect;
      return json;
    }
  }
}
