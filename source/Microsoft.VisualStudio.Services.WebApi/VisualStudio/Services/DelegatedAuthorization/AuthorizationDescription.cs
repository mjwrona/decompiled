// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationDescription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationDescription
  {
    public Registration ClientRegistration { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public InitiationError InitiationError { get; set; }

    public AuthorizationScopeDescription[] ScopeDescriptions { get; set; }

    public bool HasError => this.InitiationError != 0;
  }
}
