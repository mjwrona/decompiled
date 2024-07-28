// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenParameters
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.OAuth
{
  [JsonDictionary]
  public class VssOAuthTokenParameters : Dictionary<string, string>, IVssOAuthTokenParameterProvider
  {
    public VssOAuthTokenParameters()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public string RedirectUri
    {
      get => this.GetValueOrDefault("redirect_uri");
      set => this.RemoveOrSetValue("redirect_uri", value);
    }

    public string Resource
    {
      get => this.GetValueOrDefault("resource");
      set => this.RemoveOrSetValue("resource", value);
    }

    public string Scope
    {
      get => this.GetValueOrDefault("scope");
      set => this.RemoveOrSetValue("scope", value);
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    void IVssOAuthTokenParameterProvider.SetParameters(IDictionary<string, string> parameters)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) this)
        parameters[keyValuePair.Key] = keyValuePair.Value;
    }

    private new string GetValueOrDefault(string key)
    {
      string valueOrDefault;
      if (!this.TryGetValue(key, out valueOrDefault))
        valueOrDefault = (string) null;
      return valueOrDefault;
    }

    private void RemoveOrSetValue(string key, string value)
    {
      if (value == null)
        this.Remove(key);
      else
        this[key] = value;
    }
  }
}
