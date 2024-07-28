// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (AzureKeyVaultVariableValue))]
  [DataContract]
  public class VariableValue : ReleaseManagementSecuredObject
  {
    public VariableValue()
    {
    }

    public VariableValue(VariableValue value)
      : this(value.Value, value.IsSecret)
    {
    }

    public VariableValue(string value, bool isSecret)
      : this(value, isSecret, false)
    {
    }

    public VariableValue(string value, bool isSecret, bool isReadOnly)
    {
      this.Value = value;
      this.IsSecret = isSecret;
      this.IsReadOnly = isReadOnly;
    }

    [DataMember(EmitDefaultValue = true)]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsSecret { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsReadOnly { get; set; }
  }
}
