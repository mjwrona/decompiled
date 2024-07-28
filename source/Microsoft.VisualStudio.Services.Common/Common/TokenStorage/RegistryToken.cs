// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.RegistryToken
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  internal class RegistryToken : VssToken
  {
    private readonly VssTokenStorage _storage;

    public RegistryToken(VssTokenStorage storage, VssTokenKey tokenKey, string tokenValue)
      : this(storage, tokenKey.Kind, tokenKey.Resource, tokenKey.UserName, tokenKey.Type, tokenValue)
    {
    }

    public RegistryToken(
      VssTokenStorage storage,
      string kind,
      string resource,
      string userName,
      string type,
      string token)
      : base(kind, resource, userName, type, token)
    {
      ArgumentUtility.CheckForNull<VssTokenStorage>(storage, nameof (storage));
      this._storage = storage;
    }

    public override string GetProperty(string name)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      return this._storage.GetProperty((VssToken) this, name);
    }

    public override IEnumerable<string> GetPropertyNames() => this._storage.GetPropertyNames((VssToken) this);

    public override bool SetProperty(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      return this._storage.SetProperty((VssToken) this, name, value);
    }

    protected override string RetrieveValue() => this._storage.RetrieveTokenSecret((VssToken) this);

    protected override bool SetValue(string token) => this._storage.SetTokenSecret((VssToken) this, token);

    protected override bool RemoveValue() => this._storage.RemoveTokenSecret((VssToken) this);
  }
}
