// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.VssTokenStorage
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  public abstract class VssTokenStorage
  {
    public VssToken Add(VssTokenKey tokenKey, string tokenValue)
    {
      ArgumentUtility.CheckForNull<VssTokenKey>(tokenKey, nameof (tokenKey));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(tokenValue, nameof (tokenValue));
      return this.AddToken(tokenKey, tokenValue);
    }

    public VssToken Retrieve(VssTokenKey tokenKey)
    {
      ArgumentUtility.CheckForNull<VssTokenKey>(tokenKey, nameof (tokenKey));
      return this.RetrieveToken(tokenKey);
    }

    public bool Remove(VssTokenKey tokenKey)
    {
      ArgumentUtility.CheckForNull<VssTokenKey>(tokenKey, nameof (tokenKey));
      return this.RemoveToken(tokenKey);
    }

    public virtual IEnumerable<string> GetPropertyNames(VssToken token) => (IEnumerable<string>) null;

    public abstract IEnumerable<VssToken> RetrieveAll(string kind);

    public abstract string GetProperty(VssToken token, string name);

    public abstract bool SetProperty(VssToken token, string name, string value);

    public abstract bool SetTokenSecret(VssToken token, string tokenValue);

    public abstract string RetrieveTokenSecret(VssToken token);

    public abstract bool RemoveTokenSecret(VssToken token);

    public abstract bool RemoveAll();

    protected abstract VssToken AddToken(VssTokenKey tokenKey, string tokenValue);

    protected abstract VssToken RetrieveToken(VssTokenKey tokenKey);

    protected abstract bool RemoveToken(VssTokenKey tokenKey);
  }
}
