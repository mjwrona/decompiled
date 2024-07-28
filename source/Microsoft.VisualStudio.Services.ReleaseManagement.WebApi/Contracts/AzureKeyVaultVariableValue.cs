// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AzureKeyVaultVariableValue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class AzureKeyVaultVariableValue : VariableValue
  {
    public AzureKeyVaultVariableValue()
    {
    }

    public AzureKeyVaultVariableValue(AzureKeyVaultVariableValue value)
      : this(value.Value, value.IsSecret, value.Enabled, value.ContentType, value.Expires)
    {
    }

    public AzureKeyVaultVariableValue(
      string value,
      bool isSecret,
      bool enabled,
      string contentType,
      DateTime? expires)
      : base(value, isSecret)
    {
      this.Enabled = enabled;
      this.ContentType = contentType;
      this.Expires = expires;
    }

    [DataMember(EmitDefaultValue = true)]
    public bool Enabled { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string ContentType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? Expires { get; set; }
  }
}
