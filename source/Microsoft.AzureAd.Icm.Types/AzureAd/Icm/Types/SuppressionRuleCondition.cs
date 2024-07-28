// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SuppressionRuleCondition
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class SuppressionRuleCondition : FixedRuleCondition
  {
    [DataMember]
    public int? Severity { get; set; }

    [DataMember]
    public bool MatchRaisingLocation { get; set; }

    public static IDictionary<string, InvalidReasons> IsValid(SuppressionRuleCondition condition)
    {
      IDictionary<string, InvalidReasons> dictionary = (IDictionary<string, InvalidReasons>) new Dictionary<string, InvalidReasons>();
      if (condition == null)
      {
        dictionary.Add(nameof (SuppressionRuleCondition), InvalidReasons.CannotBeNull);
        return dictionary;
      }
      IDictionary<string, InvalidReasons> invalidFields = FixedRuleCondition.IsValid((RuleCondition) condition);
      if (condition.Severity.HasValue)
      {
        FieldValidator.ValidateSeverity(condition.Severity, invalidFields);
        if (ParameterValidator.IsLessThan<int>(condition.Severity.Value, 1))
          invalidFields["Severity"] = InvalidReasons.IsOutOfRange;
      }
      if (condition.CorrelationId == null && condition.RoutingId == null && condition.Environment == null && condition.DataCenter == null && condition.DeviceGroup == null && condition.DeviceName == null && condition.ServiceInstanceId == null && !condition.Severity.HasValue)
        invalidFields.Add(nameof (SuppressionRuleCondition), InvalidReasons.ProvideAtLeastOne);
      return invalidFields;
    }
  }
}
