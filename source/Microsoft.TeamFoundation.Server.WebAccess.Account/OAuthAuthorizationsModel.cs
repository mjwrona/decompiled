// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.OAuthAuthorizationsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class OAuthAuthorizationsModel
  {
    public string ApplicationId { get; set; }

    public string ApplicationName { get; set; }

    public bool HasApplicationImage { get; set; }

    public string ApplicationImage { get; set; }

    public string ApplicationDescription { get; set; }

    public string Provider { get; set; }

    public string ProviderUrl { get; set; }

    public string IssueDateDisplay { get; set; }

    public string ExpirationDateDisplay { get; set; }

    public bool IsExpired { get; set; }

    public string Token { get; set; }
  }
}
