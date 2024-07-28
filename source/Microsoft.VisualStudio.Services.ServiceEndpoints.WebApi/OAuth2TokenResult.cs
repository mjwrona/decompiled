// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.OAuth2TokenResult
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class OAuth2TokenResult
  {
    [DataMember(EmitDefaultValue = false)]
    public string AccessToken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RefreshToken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorDescription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Scope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ExpiresIn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string IssuedAt { get; set; }
  }
}
