// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Oidc.JsonWebKeyResponse
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Oidc
{
  [DataContract]
  public class JsonWebKeyResponse
  {
    [DataMember(Name = "N", EmitDefaultValue = false)]
    public string N { get; set; }

    [DataMember(Name = "kty", EmitDefaultValue = false)]
    public string Kty { get; set; }

    [DataMember(Name = "kid", EmitDefaultValue = false)]
    public string Kid { get; set; }

    [DataMember(Name = "alg", EmitDefaultValue = false)]
    public string Alg { get; set; }

    [DataMember(Name = "E", EmitDefaultValue = false)]
    public string E { get; set; }

    [DataMember(Name = "use", EmitDefaultValue = false)]
    public string Use { get; set; }

    [DataMember(Name = "x5c", EmitDefaultValue = false)]
    public IList<string> X5c { get; set; } = (IList<string>) new List<string>();

    [DataMember(Name = "x5t", EmitDefaultValue = false)]
    public string X5t { get; set; }

    public JsonWebKeyResponse()
    {
    }

    public JsonWebKeyResponse(string kid, string kty)
    {
      this.Kid = kid;
      this.Kty = kty;
    }
  }
}
