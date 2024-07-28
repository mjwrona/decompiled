// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationDecision
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationDecision
  {
    public AuthorizationGrant AuthorizationGrant { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public AuthorizationError AuthorizationError { get; set; }

    public Authorization Authorization { get; set; }

    public bool IsAuthorized => this.AuthorizationGrant != null;

    public bool HasError => this.AuthorizationError != 0;
  }
}
