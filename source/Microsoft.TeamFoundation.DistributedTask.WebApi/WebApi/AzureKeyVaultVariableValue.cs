// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AzureKeyVaultVariableValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class AzureKeyVaultVariableValue : VariableValue, IEquatable<AzureKeyVaultVariableValue>
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

    public bool Equals(AzureKeyVaultVariableValue variableValue) => this.Equals((VariableValue) variableValue) && !(variableValue.ContentType != this.ContentType) && variableValue.Enabled == this.Enabled && variableValue.Expires.Equals((object) this.Expires);
  }
}
