// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.PersonalAccessTokenModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class PersonalAccessTokenModel
  {
    public string AuthorizationId { get; set; }

    public string Description { get; set; }

    public string ExpiresUtc { get; set; }

    public ApplicableAccountMode AccountMode { get; set; }

    public List<string> SelectedAccounts { get; set; }

    public AuthorizedScopeMode ScopeMode { get; set; }

    public List<string> SelectedScopes { get; set; }

    public IDictionary<string, string> ValidExpirationValues { get; set; }

    public string SelectedExpiration { get; set; }

    public string Tenant { get; set; }

    public IList<KeyValuePair<string, string>> AllScopes { get; set; }

    public IDictionary<Guid, string> AllAccounts { get; set; }

    public bool IsValid { get; set; }

    public bool DisplayAllAccountsOption { get; set; }
  }
}
