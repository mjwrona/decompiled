// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [KnownType(typeof (AzureKeyVaultVariableValue))]
  public class VariableValue : IEquatable<VariableValue>
  {
    public VariableValue()
    {
    }

    public VariableValue(VariableValue value)
      : this(value.Value, value.IsSecret, value.IsReadOnly)
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

    public static implicit operator VariableValue(string value) => new VariableValue(value, false, true);

    public bool Equals(VariableValue variableValue) => variableValue.IsSecret == this.IsSecret && variableValue.IsReadOnly == this.IsReadOnly && string.Equals(variableValue.Value, this.Value);
  }
}
