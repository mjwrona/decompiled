// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UserIdentityViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class UserIdentityViewModel : UserIdentityViewModelBase
  {
    public UserIdentityViewModel()
    {
    }

    public UserIdentityViewModel(TeamFoundationIdentity identity, bool loadAadTenantData = false)
      : base(identity, loadAadTenantData)
    {
      this.MailAddress = identity.GetAttribute("Mail", string.Empty);
    }

    public override string SubHeader => !this.IsAcsAccount ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) this.Domain, (object) this.AccountName) : this.MailAddress;

    public string MailAddress { get; private set; }

    public override string IdentityType => "user";

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["MailAddress"] = (object) this.MailAddress;
      return json;
    }
  }
}
