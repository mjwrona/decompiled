// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenResponse
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.OAuth
{
  [DataContract]
  public class VssOAuthTokenResponse
  {
    [DataMember(Name = "access_token", EmitDefaultValue = false)]
    public string AccessToken { get; set; }

    [DataMember(Name = "error", EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(Name = "errordescription", EmitDefaultValue = false)]
    public string ErrorDescription { get; set; }

    [DataMember(Name = "expires_in", EmitDefaultValue = false)]
    public int ExpiresIn { get; set; }

    [DataMember(Name = "refresh_token", EmitDefaultValue = false)]
    public string RefreshToken { get; set; }

    [DataMember(Name = "scope", EmitDefaultValue = false)]
    public string Scope { get; set; }

    [DataMember(Name = "token_type", EmitDefaultValue = false)]
    public string TokenType { get; set; }
  }
}
