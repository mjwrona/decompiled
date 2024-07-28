// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions.ReleaseCondition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions
{
  [DataContract]
  public class ReleaseCondition : Condition
  {
    public ReleaseCondition()
    {
    }

    public ReleaseCondition(string name, ConditionType conditionType, string value, bool? result)
    {
      this.Name = name;
      this.ConditionType = conditionType;
      this.Value = value;
      this.Result = result;
    }

    public ReleaseCondition Clone() => new ReleaseCondition(this.Name, this.ConditionType, this.Value, this.Result);
  }
}
