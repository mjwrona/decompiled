// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputDescriptor
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FormInput
{
  [DataContract]
  public class InputDescriptor : ISecuredObject
  {
    private Guid m_namespaceId;
    private int m_requiredPermissions;
    private string m_token;

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Type { get; set; }

    public List<string> SupportedScopes { get; set; }

    [DataMember]
    public IDictionary<string, object> Properties { get; set; }

    [DataMember]
    public InputMode InputMode { get; set; }

    [DataMember]
    public bool IsConfidential { get; set; }

    [DataMember]
    public bool UseInDefaultDescription { get; set; }

    [DataMember]
    public string GroupName { get; set; }

    [DataMember]
    public string ValueHint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public InputValidation Validation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public InputValues Values { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<string> DependencyInputIds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasDynamicValueInformation { get; set; }

    public void SetSecuredObjectProperties(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_namespaceId = namespaceId;
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
      this.Validation?.SetSecuredObjectProperties(namespaceId, requiredPermissions, token);
      this.Values?.SetSecuredObjectProperties(namespaceId, requiredPermissions, token);
    }

    public Guid NamespaceId => this.m_namespaceId;

    public int RequiredPermissions => this.m_requiredPermissions;

    public string GetToken() => this.m_token;
  }
}
