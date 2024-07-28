// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SuppressionRule
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  [KnownType(typeof (SuppressionRuleCondition))]
  [KnownType(typeof (FixedRuleCondition))]
  [KnownType(typeof (RuleCondition))]
  public class SuppressionRule : Rule
  {
    private static readonly string[] ValidFields = new string[13]
    {
      "RoutingId",
      "CorrelationId",
      "Environment",
      "DataCenter",
      "DeviceGroup",
      "DeviceName",
      "ServiceInstanceId",
      "Severity",
      "ModifiedBy",
      "Description",
      nameof (StartDate),
      nameof (EndDate),
      nameof (Mode)
    };

    public SuppressionRule() => this.RuleType = RuleType.Suppression;

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public DateTime EndDate { get; set; }

    [DataMember]
    public SuppressionMode Mode { get; set; }

    [DataMember]
    public DateTime StartDate { get; set; }

    public static IDictionary<string, InvalidReasons> IsValid(SuppressionRule rule)
    {
      IDictionary<string, InvalidReasons> invalidReasons = (IDictionary<string, InvalidReasons>) new Dictionary<string, InvalidReasons>();
      if (!FieldValidator.ValidateNotNull((object) rule, nameof (SuppressionRule), invalidReasons))
        return invalidReasons;
      IDictionary<string, InvalidReasons> invalidFields = SuppressionRuleCondition.IsValid(rule.Condition as SuppressionRuleCondition);
      if (rule.Mode == SuppressionMode.Invalid)
        invalidFields["Mode"] = InvalidReasons.IsUnexpected;
      FieldValidator.ValidateDate("StartDate", rule.StartDate, invalidFields);
      FieldValidator.ValidateDate("EndDate", rule.EndDate, invalidFields);
      if (rule.EndDate <= rule.StartDate)
        invalidFields["EndDate"] = InvalidReasons.CannotBeInPast;
      if (string.IsNullOrWhiteSpace(rule.Description))
        invalidFields["Description"] = InvalidReasons.CannotBeEmpty;
      return invalidFields;
    }

    public static void ValidateFilterSet(IEnumerable<RuleFilter> filterSet)
    {
      ArgumentCheck.ThrowIfNull((object) filterSet, "Filter set.", nameof (ValidateFilterSet), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\SuppressionRule.cs");
      ArgumentCheck.ThrowIfGreaterThan<long>((long) filterSet.Count<RuleFilter>(), 5L, "Max Filters.", nameof (ValidateFilterSet), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\SuppressionRule.cs");
      foreach (RuleFilter filter in filterSet.AsParallel<RuleFilter>())
        SuppressionRule.ValidateFilter(filter);
    }

    private static void ValidateFilter(RuleFilter filter)
    {
      ArgumentCheck.ThrowIfNull((object) filter, "SuppressionRules.RuleFilter", nameof (ValidateFilter), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\SuppressionRule.cs");
      Dictionary<string, object> filterProperties = filter.FilterProperties;
      foreach (string key in filterProperties.Keys)
      {
        if (!((IEnumerable<string>) SuppressionRule.ValidFields).Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          throw new ArgumentException("Unsupported parameter in filter set: " + key);
      }
      RuleFilter.ValidateString(filterProperties, "RoutingId", "filter.RoutingId");
      RuleFilter.ValidateString(filterProperties, "CorrelationId", "filter.CorrelationId");
      RuleFilter.ValidateString(filterProperties, "Environment", "filter.Environment");
      RuleFilter.ValidateString(filterProperties, "DataCenter", "filter.DataCenter");
      RuleFilter.ValidateString(filterProperties, "DeviceGroup", "filter.DeviceGroup");
      RuleFilter.ValidateString(filterProperties, "DeviceName", "filter.DeviceName");
      RuleFilter.ValidateString(filterProperties, "ServiceInstanceId", "filter.ServiceInstanceId");
      RuleFilter.ValidateString(filterProperties, "ModifiedBy", "filter.ModifiedBy");
      RuleFilter.ValidateDate(filterProperties, "StartDate", "filter.StartDate");
      RuleFilter.ValidateDate(filterProperties, "EndDate", "filter.EndDate");
      if (filterProperties.ContainsKey("Mode"))
      {
        string str = filterProperties["Mode"] as string;
        ArgumentCheck.ThrowIfEmptyOrWhiteSpace(str, "filter.SuppressionMode", nameof (ValidateFilter), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\SuppressionRule.cs");
        SuppressionMode result;
        if (str != null && (!Enum.TryParse<SuppressionMode>(str, true, out result) || result == SuppressionMode.Invalid))
          throw new ArgumentException("Invalid suppression mode. Supported values are: HideIncident, DiscardIncident");
      }
      if (filterProperties.ContainsKey("Severity") && filterProperties["Severity"] is string s)
      {
        int result = 0;
        if (!int.TryParse(s, out result))
          throw new ArgumentException("Severity should be a number between 1 and 4.");
        ArgumentCheck.ThrowIfNotInRangeInclusive<int>(result, 1, 4, "filter.Severity", nameof (ValidateFilter), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Rules\\SuppressionRule.cs");
      }
      if (!filterProperties.ContainsKey("MatchRaisingLocation"))
        return;
      object obj = filterProperties["MatchRaisingLocation"];
      if (obj != null && !(obj is bool))
        throw new ArgumentException("Match Raising Location should be a valid boolean.");
    }
  }
}
