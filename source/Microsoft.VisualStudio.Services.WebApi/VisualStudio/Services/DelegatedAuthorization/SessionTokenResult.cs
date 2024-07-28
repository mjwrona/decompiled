// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class SessionTokenResult
  {
    public SessionToken SessionToken { get; set; }

    public SessionTokenError SessionTokenError { get; set; }

    public bool HasError => this.SessionTokenError != 0;

    public static implicit operator PatTokenResult(SessionTokenResult s)
    {
      PatToken patToken = (PatToken) null;
      if (s.SessionToken != null)
        patToken = new PatToken()
        {
          DisplayName = s.SessionToken.DisplayName,
          ValidTo = s.SessionToken.ValidTo,
          Scope = s.SessionToken.Scope,
          TargetAccounts = s.SessionToken.TargetAccounts,
          ValidFrom = s.SessionToken.ValidFrom,
          AuthorizationId = s.SessionToken.AuthorizationId,
          Token = s.SessionToken.Token
        };
      return new PatTokenResult()
      {
        PatToken = patToken,
        PatTokenError = s.SessionTokenError
      };
    }
  }
}
