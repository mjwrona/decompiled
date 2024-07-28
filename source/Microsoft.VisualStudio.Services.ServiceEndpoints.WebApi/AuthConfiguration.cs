// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthConfiguration
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class AuthConfiguration : OAuthConfiguration
  {
    [DataMember(EmitDefaultValue = false, Name = "Parameters")]
    private Dictionary<string, Parameter> m_parameters;

    public AuthConfiguration()
    {
    }

    public AuthConfiguration(AuthConfiguration config)
      : base((OAuthConfiguration) config)
    {
      this.Parameters = config.Parameters;
    }

    public IDictionary<string, Parameter> Parameters
    {
      get => (IDictionary<string, Parameter>) this.m_parameters ?? (IDictionary<string, Parameter>) (this.m_parameters = new Dictionary<string, Parameter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      set => this.m_parameters = value == null ? new Dictionary<string, Parameter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, Parameter>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
