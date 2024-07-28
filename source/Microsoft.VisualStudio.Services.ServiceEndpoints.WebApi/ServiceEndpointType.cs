// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ServiceEndpointType
  {
    [DataMember(EmitDefaultValue = false, Name = "InputDescriptors")]
    private List<InputDescriptor> m_inputDescriptors;
    [DataMember(EmitDefaultValue = false, Name = "AuthenticationSchemes")]
    private List<ServiceEndpointAuthenticationScheme> m_authenticationSchemes;
    [DataMember(EmitDefaultValue = false, Name = "DataSources")]
    private List<DataSource> m_dataSources;
    [DataMember(EmitDefaultValue = false, Name = "DependencyData")]
    private List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData> m_dependencyData;
    [DataMember(EmitDefaultValue = false, Name = "TrustedHosts")]
    private List<string> m_trustedHosts;

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EndpointUrl EndpointUrl { get; set; }

    public List<DataSource> DataSources => this.m_dataSources ?? (this.m_dataSources = new List<DataSource>());

    public List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData> DependencyData => this.m_dependencyData ?? (this.m_dependencyData = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData>());

    public List<string> TrustedHosts => this.m_trustedHosts ?? (this.m_trustedHosts = new List<string>());

    public List<ServiceEndpointAuthenticationScheme> AuthenticationSchemes => this.m_authenticationSchemes ?? (this.m_authenticationSchemes = new List<ServiceEndpointAuthenticationScheme>());

    [DataMember(EmitDefaultValue = false)]
    public HelpLink HelpLink { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HelpMarkDown { get; set; }

    public List<InputDescriptor> InputDescriptors => this.m_inputDescriptors ?? (this.m_inputDescriptors = new List<InputDescriptor>());

    [DataMember(EmitDefaultValue = false)]
    public Uri IconUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UiContributionId { get; set; }
  }
}
