// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.WebService.Client.RuleManagerClient
// Assembly: Microsoft.AzureAd.Icm.WebService.Client, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 85C23930-39A1-49EE-A03A-507264E2FE4B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.WebService.Client.dll

using Microsoft.AzureAd.Icm.Types;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.AzureAd.Icm.WebService.Client
{
  public class RuleManagerClient : ClientBase<IRuleManager>, IRuleManager
  {
    public RuleManagerClient()
    {
    }

    public RuleManagerClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public RuleManagerClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public RuleManagerClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public RuleManagerClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public PagedResult<RoutingRule> EnumRoutingRules(
      Guid tenantId,
      IEnumerable<RuleFilter> filterSet,
      PageToken token)
    {
      return this.Channel.EnumRoutingRules(tenantId, filterSet, token);
    }

    public IEnumerable<CorrelationRule> EnumCorrelationRules(Guid tenantId) => this.Channel.EnumCorrelationRules(tenantId);

    public void UpdateRoutingRules(
      Guid tenantId,
      string description,
      IEnumerable<RoutingRule> addUpdate,
      IEnumerable<RuleCondition> remove)
    {
      this.Channel.UpdateRoutingRules(tenantId, description, addUpdate, remove);
    }

    public void SetRoutingRules(Guid tenantId, string description, IEnumerable<RoutingRule> rules) => this.Channel.SetRoutingRules(tenantId, description, rules);

    public void UpdateCorrelationRules(
      Guid tenantId,
      string description,
      IEnumerable<CorrelationRule> addUpdate,
      IEnumerable<RuleCondition> remove)
    {
      this.Channel.UpdateCorrelationRules(tenantId, description, addUpdate, remove);
    }

    public void SetCorrelationRules(
      Guid tenantId,
      string description,
      IEnumerable<CorrelationRule> rules)
    {
      this.Channel.SetCorrelationRules(tenantId, description, rules);
    }

    public PagedResult<SuppressionRule> EnumSuppressionRules(
      Guid tenantId,
      IEnumerable<RuleFilter> filterSet,
      PageToken token,
      bool skipDescription = true)
    {
      return this.Channel.EnumSuppressionRules(tenantId, filterSet, token, skipDescription);
    }

    public void EnableSuppression(
      Guid tenantId,
      IEnumerable<SuppressionRule> rules,
      string description)
    {
      this.Channel.EnableSuppression(tenantId, rules, description);
    }

    public void DisableSuppression(
      Guid tenantId,
      IEnumerable<SuppressionRuleCondition> rules,
      string description)
    {
      this.Channel.DisableSuppression(tenantId, rules, description);
    }
  }
}
