// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions.Condition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions
{
  [KnownType(typeof (ReleaseCondition))]
  [DataContract]
  [XmlInclude(typeof (ReleaseCondition))]
  public class Condition : ReleaseManagementSecuredObject
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public ConditionType ConditionType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Value { get; set; }

    [DataMember]
    public bool? Result { get; set; }

    public Condition()
    {
    }

    public Condition(string name, ConditionType conditionType, string value)
    {
      this.Name = name;
      this.ConditionType = conditionType;
      this.Value = value;
    }

    public Condition Clone() => new Condition(this.Name, this.ConditionType, this.Value);

    public override bool Equals(object obj)
    {
      if (obj is Condition condition && string.Equals(this.Name, condition.Name, StringComparison.OrdinalIgnoreCase) && this.ConditionType == condition.ConditionType && string.Equals(this.Value, condition.Value, StringComparison.OrdinalIgnoreCase))
      {
        bool? result1 = this.Result;
        bool? result2 = condition.Result;
        if (result1.GetValueOrDefault() == result2.GetValueOrDefault() & result1.HasValue == result2.HasValue)
          return true;
      }
      return false;
    }

    public override int GetHashCode() => ((int) ((ConditionType) ((this.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name) : 0) * 397) ^ this.ConditionType) * 397 ^ (this.Value != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value) : 0)) * 397 ^ (this.Result.HasValue ? this.Result.GetHashCode() : 0);
  }
}
