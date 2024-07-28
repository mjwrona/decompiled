// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IRuleManager
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.AzureAd.Icm.Types
{
  [ServiceContract]
  public interface IRuleManager
  {
    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    PagedResult<RoutingRule> EnumRoutingRules(
      Guid tenantId,
      IEnumerable<RuleFilter> filterSet,
      PageToken token);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    IEnumerable<CorrelationRule> EnumCorrelationRules(Guid tenantId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void UpdateRoutingRules(
      Guid tenantId,
      string description,
      IEnumerable<RoutingRule> addUpdate,
      IEnumerable<RuleCondition> remove);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void SetRoutingRules(Guid tenantId, string description, IEnumerable<RoutingRule> rules);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void UpdateCorrelationRules(
      Guid tenantId,
      string description,
      IEnumerable<CorrelationRule> addUpdate,
      IEnumerable<RuleCondition> remove);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void SetCorrelationRules(Guid tenantId, string description, IEnumerable<CorrelationRule> rules);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    PagedResult<SuppressionRule> EnumSuppressionRules(
      Guid tenantId,
      IEnumerable<RuleFilter> filterSet,
      PageToken token,
      bool skipDescription = true);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void EnableSuppression(Guid tenantId, IEnumerable<SuppressionRule> rules, string description);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    void DisableSuppression(
      Guid tenantId,
      IEnumerable<SuppressionRuleCondition> rules,
      string description);
  }
}
