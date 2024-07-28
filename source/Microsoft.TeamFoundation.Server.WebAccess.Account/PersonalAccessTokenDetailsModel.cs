// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.PersonalAccessTokenDetailsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class PersonalAccessTokenDetailsModel : SessionToken
  {
    public PersonalAccessTokenDetailsModel(SessionToken source, string displayDateFormat = "M/d/yyyy")
    {
      this.AuthorizationId = source.AuthorizationId;
      this.DisplayName = HttpUtility.HtmlDecode(source.DisplayName);
      this.Scope = source.Scope;
      this.UserId = source.UserId;
      this.ValidFrom = source.ValidFrom;
      this.ValidTo = source.ValidTo;
      this.DisplayDate = this.ValidTo.ToString(displayDateFormat);
      this.IsExpired = DateTime.UtcNow > source.ValidTo;
      this.Token = source.Token;
      this.AccessId = source.AccessId;
      this.TargetAccounts = source.TargetAccounts;
      this.IsValid = source.IsValid;
      if (!source.IsValid)
        this.Status = AccountResources.TokenStatusRevoked;
      else if (this.IsExpired)
        this.Status = AccountServerResources.TokenExpiredText;
      else
        this.Status = AccountServerResources.TokenStatusActive;
    }

    public string DisplayDate { get; set; }

    public bool IsExpired { get; set; }

    public string Status { get; set; }
  }
}
