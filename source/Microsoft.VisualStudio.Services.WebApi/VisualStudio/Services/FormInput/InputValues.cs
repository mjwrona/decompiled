// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputValues
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FormInput
{
  [DataContract]
  public class InputValues : ISecuredObject
  {
    private Guid m_namespaceId;
    private int m_requiredPermissions;
    private string m_token;

    [DataMember(EmitDefaultValue = false)]
    public string InputId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<InputValue> PossibleValues { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsLimitedToPossibleValues { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsReadOnly { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public InputValuesError Error { get; set; }

    public void SetSecuredObjectProperties(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_namespaceId = namespaceId;
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
      this.Error?.SetSecuredObjectProperties(namespaceId, requiredPermissions, token);
      if (this.PossibleValues == null || !this.PossibleValues.Any<InputValue>())
        return;
      foreach (InputValue possibleValue in (IEnumerable<InputValue>) this.PossibleValues)
        possibleValue.SetSecuredObjectProperties(namespaceId, requiredPermissions, token);
    }

    public Guid NamespaceId => this.m_namespaceId;

    public int RequiredPermissions => this.m_requiredPermissions;

    public string GetToken() => this.m_token;
  }
}
