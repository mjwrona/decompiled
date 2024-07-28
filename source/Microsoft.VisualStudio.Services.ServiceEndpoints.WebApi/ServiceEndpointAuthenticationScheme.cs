// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ServiceEndpointAuthenticationScheme
  {
    [DataMember(EmitDefaultValue = false, Name = "InputDescriptors")]
    private List<InputDescriptor> m_inputDescriptors;
    private List<AuthorizationHeader> m_authorizationHeaders;
    private List<DataSourceBinding> m_datasourcebindings;
    private List<ClientCertificate> m_clientCertificates;
    private IDictionary<string, string> m_properties;

    [DataMember(EmitDefaultValue = false)]
    public string Scheme { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AuthorizationUrl { get; set; }

    [DataMember]
    public bool RequiresOAuth2Configuration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<DataSourceBinding> DataSourceBindings
    {
      get => this.m_datasourcebindings ?? (this.m_datasourcebindings = new List<DataSourceBinding>());
      set => this.m_datasourcebindings = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public List<AuthorizationHeader> AuthorizationHeaders
    {
      get => this.m_authorizationHeaders ?? (this.m_authorizationHeaders = new List<AuthorizationHeader>());
      set => this.m_authorizationHeaders = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public List<ClientCertificate> ClientCertificates
    {
      get => this.m_clientCertificates ?? (this.m_clientCertificates = new List<ClientCertificate>());
      set => this.m_clientCertificates = value;
    }

    public List<InputDescriptor> InputDescriptors
    {
      get => this.m_inputDescriptors ?? (this.m_inputDescriptors = new List<InputDescriptor>());
      set => this.m_inputDescriptors = value;
    }

    [DataMember]
    public IDictionary<string, string> Properties
    {
      get => this.m_properties ?? (this.m_properties = (IDictionary<string, string>) new Dictionary<string, string>());
      set => this.m_properties = value;
    }
  }
}
