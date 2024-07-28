// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.CorrelationRule
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class CorrelationRule : Rule
  {
    public CorrelationRule() => this.RuleType = RuleType.Correlation;

    [DataMember]
    public bool MatchServiceInstanceId { get; set; }

    [DataMember]
    public bool MatchDataCenter { get; set; }

    [DataMember]
    public bool MatchDeviceGroup { get; set; }

    [DataMember]
    public bool MatchDeviceName { get; set; }

    [DataMember]
    public bool MatchSeverity { get; set; }

    [DataMember]
    public CorrelationMode Mode { get; set; }

    [DataMember]
    public int? CreationDateWindow { get; set; }

    [DataMember]
    public int? CorrelationDateWindow { get; set; }

    [DataMember]
    public bool IncludeOriginatingTenant { get; set; }

    public static void ThrowIfNotValid(Rule rule)
    {
      CorrelationRule correlationRule = rule as CorrelationRule;
      ArgumentCheck.ThrowIfNull((object) rule, nameof (rule), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\CorrelationRule.cs");
      if (correlationRule == null)
        throw new ArgumentException(TypeUtility.Format("Unsupported rule type [{0}] encountered while validating CorrelationRule", (object) rule.GetType().Name), nameof (rule));
      FixedRuleCondition.ThrowIfNotValid(correlationRule.Condition);
    }
  }
}
