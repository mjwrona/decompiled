// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.VssToken
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  public abstract class VssToken : VssTokenKey
  {
    protected VssToken(
      string kind,
      string resource,
      string userName,
      string type,
      string tokenValue)
      : base(kind, resource, userName, type)
    {
      if (tokenValue == null)
        tokenValue = string.Empty;
      this.TokenValue = tokenValue;
    }

    public string TokenValue { get; protected set; }

    public bool RefreshTokenValue()
    {
      string str = this.RetrieveValue();
      if (str == null)
      {
        this.TokenValue = string.Empty;
        return false;
      }
      this.TokenValue = str;
      return true;
    }

    public bool SetTokenValue(string token)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(token, nameof (token));
      int num = this.SetValue(token) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.TokenValue = token;
      return num != 0;
    }

    public bool RemoveTokenValue()
    {
      int num = this.RemoveValue() ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.TokenValue = string.Empty;
      return num != 0;
    }

    public virtual IEnumerable<string> GetPropertyNames() => (IEnumerable<string>) null;

    public abstract string GetProperty(string name);

    public abstract bool SetProperty(string name, string value);

    protected abstract string RetrieveValue();

    protected abstract bool SetValue(string token);

    protected abstract bool RemoveValue();
  }
}
