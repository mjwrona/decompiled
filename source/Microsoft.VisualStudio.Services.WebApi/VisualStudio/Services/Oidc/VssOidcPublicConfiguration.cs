// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Oidc.VssOidcPublicConfiguration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Oidc
{
  [DataContract]
  public class VssOidcPublicConfiguration
  {
    [DataMember(Name = "issuer", EmitDefaultValue = false)]
    public string Issuer { get; set; }

    [DataMember(Name = "jwks_uri", EmitDefaultValue = false)]
    public string JwksUri { get; set; }

    [DataMember(Name = "subject_types_supported", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> SubjectTypesSupported { get; set; }

    [DataMember(Name = "response_types_supported", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> ResponseTypesSupported { get; set; }

    [DataMember(Name = "claims_supported", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> ClaimsSupported { get; set; }

    [DataMember(Name = "id_token_signing_alg_values_supported", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> IdTokenSigningAlgValuesSupported { get; set; }

    [DataMember(Name = "scopes_supported", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> ScopesSupported { get; set; }

    public VssOidcPublicConfiguration()
    {
    }

    public VssOidcPublicConfiguration(
      string issuer,
      string jwksUri,
      IReadOnlyCollection<string> subjectTypesSupported,
      IReadOnlyCollection<string> responseTypesSupported,
      IReadOnlyCollection<string> claimsSupported,
      IReadOnlyCollection<string> idTokenSigningAlgValuesSupported,
      IReadOnlyCollection<string> scopesSupported)
    {
      this.Issuer = issuer;
      this.JwksUri = jwksUri;
      this.SubjectTypesSupported = subjectTypesSupported;
      this.ResponseTypesSupported = responseTypesSupported;
      this.ClaimsSupported = claimsSupported;
      this.IdTokenSigningAlgValuesSupported = idTokenSigningAlgValuesSupported;
      this.ScopesSupported = scopesSupported;
    }
  }
}
