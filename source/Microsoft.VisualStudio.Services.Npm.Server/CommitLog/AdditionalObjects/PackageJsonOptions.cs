// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects.PackageJsonOptions
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects
{
  public class PackageJsonOptions
  {
    public bool ContainsBindingGypFileAtRoot { get; set; }

    public bool ContainsServerJsFileAtRoot { get; set; }

    public IDictionary<string, JToken> AdditionalClientProvidedData { get; set; }

    [JsonIgnore]
    public JToken GitHead
    {
      get
      {
        JToken jtoken;
        return this.AdditionalClientProvidedData != null && this.AdditionalClientProvidedData.TryGetValue("gitHead", out jtoken) ? jtoken : (JToken) null;
      }
      set
      {
        this.AdditionalClientProvidedData = this.AdditionalClientProvidedData ?? (IDictionary<string, JToken>) new Dictionary<string, JToken>();
        this.AdditionalClientProvidedData["gitHead"] = value;
      }
    }

    public PackageJsonOptions Clone() => new PackageJsonOptions()
    {
      ContainsBindingGypFileAtRoot = this.ContainsBindingGypFileAtRoot,
      ContainsServerJsFileAtRoot = this.ContainsServerJsFileAtRoot,
      AdditionalClientProvidedData = this.AdditionalClientProvidedData
    };
  }
}
