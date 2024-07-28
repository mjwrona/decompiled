// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointType
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
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
    private List<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData> m_dependencyData;
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

    public List<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData> DependencyData => this.m_dependencyData ?? (this.m_dependencyData = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData>());

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
