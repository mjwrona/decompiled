// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputValue
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
  public class InputValue : ISecuredObject
  {
    private Guid m_namespaceId;
    private int m_requiredPermissions;
    private string m_token;

    [DataMember]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, object> Data { get; set; }

    public void SetSecuredObjectProperties(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_namespaceId = namespaceId;
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
    }

    public Guid NamespaceId => this.m_namespaceId;

    public int RequiredPermissions => this.m_requiredPermissions;

    public string GetToken() => this.m_token;
  }
}
