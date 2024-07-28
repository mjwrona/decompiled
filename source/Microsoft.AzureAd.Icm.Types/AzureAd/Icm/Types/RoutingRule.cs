// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RoutingRule
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class RoutingRule : Rule
  {
    public RoutingRule() => this.RuleType = RuleType.Routing;

    [DataMember]
    public string TeamId { get; set; }

    [DataMember]
    public string ContactAlias { get; set; }

    [DataMember]
    public bool ForceSeverity { get; set; }

    [DataMember]
    public int Severity { get; set; }

    [DataMember]
    public string OwningTenantName { get; set; }

    [DataMember]
    public string CategoryId { get; set; }

    [DataMember]
    public bool ForceTeam { get; set; }

    [DataMember]
    public RoutingMatchOperator MatchOperator { get; set; }

    public static void ThrowIfNotValid(Rule rule)
    {
      if (!(rule is RoutingRule routingRule))
        throw new ArgumentException(TypeUtility.Format("Unsupported rule type [{0}] encountered while validating RoutingRule", (object) rule.GetType().Name), nameof (rule));
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(routingRule.TeamId, "rule.TeamName", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\RoutingRule.cs");
      ArgumentCheck.ThrowIfEmptyOrWhiteSpace(routingRule.ContactAlias, "rule.ContactAlias", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\RoutingRule.cs");
      FixedRuleCondition.ThrowIfNotValid(routingRule.Condition);
    }

    public bool IsDefaultRule()
    {
      FixedRuleCondition condition = (FixedRuleCondition) this.Condition;
      return condition.RoutingId == null && condition.CorrelationId == null && condition.Environment == null && condition.DataCenter == null && condition.DeviceGroup == null && condition.DeviceName == null && condition.ServiceInstanceId == null;
    }
  }
}
