// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ServiceEndpointAuthenticationScheme
  {
    [DataMember(EmitDefaultValue = false, Name = "InputDescriptors")]
    private List<InputDescriptor> m_inputDescriptors;
    private List<AuthorizationHeader> m_authorizationHeaders;
    private List<ClientCertificate> m_clientCertificates;

    [DataMember(EmitDefaultValue = false)]
    public string Scheme { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

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
  }
}
