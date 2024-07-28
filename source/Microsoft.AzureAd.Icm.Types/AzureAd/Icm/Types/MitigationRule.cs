// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.MitigationRule
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class MitigationRule : Rule
  {
    public MitigationRule() => this.RuleType = RuleType.Correlation;

    [DataMember]
    public string Payload { get; set; }

    public static void ThrowIfNotValid(Rule rule)
    {
      MitigationRule mitigationRule = rule as MitigationRule;
      ArgumentCheck.ThrowIfNull((object) rule, nameof (rule), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\MitigationRule.cs");
      if (mitigationRule == null)
        throw new ArgumentException(TypeUtility.Format("Unsupported rule type [{0}] encountered while validating MitigationRule", (object) rule.GetType().Name), nameof (rule));
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(mitigationRule.Payload, "rule.Payload", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\MitigationRule.cs");
      FixedRuleCondition.ThrowIfNotValid(mitigationRule.Condition);
    }
  }
}
