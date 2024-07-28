// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ContributedFeatureService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  internal class ContributedFeatureService : IContributedFeatureService, IVssFrameworkService
  {
    protected const string c_area = "FeatureManagement";
    protected const string c_layer = "ContributedFeatureService";
    internal const int ContributedFeatureListenerError = 10026100;
    internal const int ContributedFeatureListenerCallbackError = 10026101;
    internal const int ContributedFeaturePluginError = 10026102;
    internal const int ContributedFeaturePluginParseError = 10026103;
    internal const int ContributedFeaturePluginNotFoundError = 10026104;
    private const string c_featuresRequestKey = "contributed-features";
    private const string c_featureStatesRequestKey = "contributed-feature-states";
    private const string c_featureContributionType = "ms.vss-web.feature";
    private const string c_featureContributionDataKey = "features";
    private const string c_featureContributionNameProperty = "name";
    private const string c_featureContributionHostScopesProperty = "hostScopes";
    private const string c_featureContributionUserScopesProperty = "userScopes";
    private const string c_featureContributionUserConfigurableProperty = "userConfigurable";
    private const string c_featureContributionHostConfigurableProperty = "hostConfigurable";
    private const string c_featureContributionDefaultStateProperty = "defaultState";
    private const string c_featureContributionIncludeAsClaimProperty = "includeAsClaim";
    private const string c_featureContributionServiceInstanceTypeProperty = "serviceInstanceType";
    private const string c_featureContributionDefaultValueRulesProperty = "defaultValueRules";
    private const string c_featureContributionOverrideRulesProperty = "overrideRules";
    private const string c_featureContributionStateChangedListenersProperty = "featureStateChangedListeners";
    private const string c_featureContributionTagsProperty = "tags";
    private const string c_featureContributionOrderProperty = "order";
    private const string c_featureContributionFeaturePropertiesProperty = "featureProperties";
    private const string c_registryFeaturesSettingPrefix = "Features/";
    private static readonly HashSet<string> c_featureContributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-web.feature"
    };
    private IDictionary<string, IContributedFeatureValuePlugin> m_registeredPlugins;
    private IDictionary<string, IContributedFeatureStateChangedListener> m_registeredListeners;
    private readonly IReadOnlyList<string> ForceEvaluateStateFeaturesFromPlugin = (IReadOnlyList<string>) new List<string>(1)
    {
      "ms.vss-admin-web.user-profile-sync-feature"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_registeredPlugins = (IDictionary<string, IContributedFeatureValuePlugin>) systemRequestContext.GetExtensions<IContributedFeatureValuePlugin>(ExtensionLifetime.Service, throwOnError: true).ToDictionary<IContributedFeatureValuePlugin, string>((Func<IContributedFeatureValuePlugin, string>) (plugin => plugin.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_registeredListeners = (IDictionary<string, IContributedFeatureStateChangedListener>) systemRequestContext.GetExtensions<IContributedFeatureStateChangedListener>(ExtensionLifetime.Service, throwOnError: true).ToDictionary<IContributedFeatureStateChangedListener, string>((Func<IContributedFeatureStateChangedListener, string>) (plugin => plugin.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ContributedFeature GetFeature(IVssRequestContext requestContext, string contributionId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      CachedContributedFeature contributedFeature = (CachedContributedFeature) null;
      this.GetFeaturesLookup(requestContext).TryGetValue(contributionId, out contributedFeature);
      return contributedFeature?.Feature;
    }

    public IEnumerable<ContributedFeature> GetFeatures(IVssRequestContext requestContext) => this.GetFeaturesLookup(requestContext).Values.Select<CachedContributedFeature, ContributedFeature>((Func<CachedContributedFeature, ContributedFeature>) (f => f.Feature));

    public IEnumerable<string> GetFeatureClaims(IVssRequestContext requestContext) => this.GetFeaturesLookup(requestContext).Values.Where<CachedContributedFeature>((Func<CachedContributedFeature, bool>) (f => f.Feature.IncludeAsClaim && this.IsFeatureEnabled(requestContext, f.Feature.Id))).Select<CachedContributedFeature, string>((Func<CachedContributedFeature, string>) (f => f.Feature.Id));

    public IEnumerable<ContributedFeature> GetFeaturesForTarget(
      IVssRequestContext requestContext,
      string contributionId)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "ContributedFeatureService.GetFeaturesForTarget"))
      {
        List<ContributedFeature> featuresForTarget = new List<ContributedFeature>();
        IDictionary<string, CachedContributedFeature> featuresLookup = this.GetFeaturesLookup(requestContext);
        IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) new string[1]
        {
          contributionId
        }, ContributedFeatureService.c_featureContributionTypes, ContributionQueryOptions.IncludeSubTree);
        if (contributions != null)
        {
          foreach (Contribution contribution in contributions)
          {
            CachedContributedFeature contributedFeature;
            if (featuresLookup.TryGetValue(contribution.Id, out contributedFeature))
              featuresForTarget.Add(contributedFeature.Feature);
          }
        }
        return (IEnumerable<ContributedFeature>) featuresForTarget;
      }
    }

    public bool IsFeatureEnabled(IVssRequestContext requestContext, string contributionId) => this.IsFeatureEnabled(requestContext, contributionId, (IDictionary<string, string>) null);

    public bool IsFeatureEnabled(
      IVssRequestContext requestContext,
      string contributionId,
      IDictionary<string, string> scopeValues)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      ContributedFeatureState featureState = this.GetFeatureState(requestContext, contributionId, scopeValues);
      return featureState != null && featureState.State == ContributedFeatureEnabledValue.Enabled;
    }

    public ContributedFeatureState GetFeatureState(
      IVssRequestContext requestContext,
      string contributionId)
    {
      return this.GetFeatureState(requestContext, contributionId, (IDictionary<string, string>) null);
    }

    public ContributedFeatureState GetFeatureState(
      IVssRequestContext requestContext,
      string contributionId,
      IDictionary<string, string> scopeValues)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      IDictionary<string, ContributedFeatureState> featureStatesLookup = this.GetCachedFeatureStatesLookup(requestContext);
      string key = contributionId;
      if (scopeValues != null)
        key = string.Format("{0}-{1}", (object) contributionId, (object) scopeValues.GetHashCode());
      ContributedFeatureState effectiveFeatureState;
      if (!featureStatesLookup.TryGetValue(key, out effectiveFeatureState))
      {
        featureStatesLookup[key] = (ContributedFeatureState) null;
        using (PerformanceTimer.StartMeasure(requestContext, "ContributedFeatureService.GetFeatureState", contributionId))
        {
          CachedContributedFeature cachedFeature;
          if (this.GetFeaturesLookup(requestContext).TryGetValue(contributionId, out cachedFeature))
            effectiveFeatureState = this.GetEffectiveFeatureState(requestContext, cachedFeature, scopeValues, 0);
        }
        featureStatesLookup[key] = effectiveFeatureState;
      }
      return effectiveFeatureState;
    }

    public IDictionary<string, ContributedFeatureState> GetFeatureStates(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      IDictionary<string, string> scopeValues)
    {
      IDictionary<string, ContributedFeatureState> featureStates = (IDictionary<string, ContributedFeatureState>) new Dictionary<string, ContributedFeatureState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ArgumentUtility.CheckForNull<IEnumerable<string>>(contributionIds, nameof (contributionIds));
      foreach (string contributionId in contributionIds)
        featureStates[contributionId] = this.GetFeatureState(requestContext, contributionId, scopeValues);
      return featureStates;
    }

    public ContributedFeatureState GetFeatureState(
      IVssRequestContext requestContext,
      string contributionId,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      CachedContributedFeature contributedFeature;
      if (!this.GetFeaturesLookup(requestContext).TryGetValue(contributionId, out contributedFeature))
        return (ContributedFeatureState) null;
      this.GetScopeIndex(requestContext, contributedFeature.Feature, userScope, scopeName);
      return new ContributedFeatureState()
      {
        FeatureId = contributionId,
        State = this.GetFeatureEnabledValue(requestContext, contributionId, userScope, scopeName, scopeValue),
        Scope = new ContributedFeatureSettingScope()
        {
          UserScoped = userScope.IsUserScoped,
          SettingScope = scopeName
        }
      };
    }

    public ContributedFeatureState GetEffectiveFeatureState(
      IVssRequestContext requestContext,
      string contributionId,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue,
      IDictionary<string, string> allScopeValues)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      CachedContributedFeature cachedFeature;
      if (!this.GetFeaturesLookup(requestContext).TryGetValue(contributionId, out cachedFeature))
        return (ContributedFeatureState) null;
      int scopeIndex = this.GetScopeIndex(requestContext, cachedFeature.Feature, userScope, scopeName);
      if (allScopeValues == null)
        allScopeValues = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!string.IsNullOrEmpty(scopeName))
        allScopeValues[scopeName] = scopeValue;
      return this.GetEffectiveFeatureState(requestContext, cachedFeature, allScopeValues, scopeIndex);
    }

    public IDictionary<string, ContributedFeatureState> GetEffectiveFeatureStates(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue,
      IDictionary<string, string> allScopeValues)
    {
      IDictionary<string, ContributedFeatureState> effectiveFeatureStates = (IDictionary<string, ContributedFeatureState>) new Dictionary<string, ContributedFeatureState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ArgumentUtility.CheckForNull<IEnumerable<string>>(contributionIds, nameof (contributionIds));
      foreach (string contributionId in contributionIds)
        effectiveFeatureStates[contributionId] = this.GetEffectiveFeatureState(requestContext, contributionId, userScope, scopeName, scopeValue, allScopeValues);
      return effectiveFeatureStates;
    }

    private ContributedFeatureState GetEffectiveFeatureState(
      IVssRequestContext requestContext,
      CachedContributedFeature cachedFeature,
      IDictionary<string, string> scopeValues,
      int scopesToSkip)
    {
      ContributedFeature feature = cachedFeature.Feature;
      ContributedFeatureState state = new ContributedFeatureState()
      {
        FeatureId = feature.Id,
        State = ContributedFeatureEnabledValue.Undefined
      };
      if (cachedFeature.OverrideRules != null)
      {
        foreach (ParsedFeatureRule overrideRule in cachedFeature.OverrideRules)
        {
          string reason;
          ContributedFeatureEnabledValue enabledValueFromPlugin = overrideRule.GetEnabledValueFromPlugin(requestContext, scopeValues, out reason);
          if (enabledValueFromPlugin != ContributedFeatureEnabledValue.Undefined)
          {
            state.State = enabledValueFromPlugin;
            state.Overridden = true;
            state.Reason = reason;
            break;
          }
        }
      }
      if (state.State == ContributedFeatureEnabledValue.Undefined)
      {
        this.PopulateExplicitFeatureEnabledValue(requestContext, cachedFeature.Feature, state, scopeValues, scopesToSkip);
        if (state.State == ContributedFeatureEnabledValue.Undefined)
        {
          IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          state.State = this.GetFeatureEnabledValue(requestContext1, feature.Id, SettingsUserScope.AllUsers, (string) null, (string) null);
          if (state.State == ContributedFeatureEnabledValue.Undefined)
          {
            if (cachedFeature.DefaultValueRules != null)
            {
              foreach (ParsedFeatureRule defaultValueRule in cachedFeature.DefaultValueRules)
              {
                string reason;
                ContributedFeatureEnabledValue enabledValueFromPlugin = defaultValueRule.GetEnabledValueFromPlugin(requestContext, scopeValues, out reason);
                if (enabledValueFromPlugin != ContributedFeatureEnabledValue.Undefined)
                {
                  state.State = enabledValueFromPlugin;
                  state.Reason = reason;
                  break;
                }
              }
            }
            if (state.State == ContributedFeatureEnabledValue.Undefined)
              state.State = feature.DefaultState ? ContributedFeatureEnabledValue.Enabled : ContributedFeatureEnabledValue.Disabled;
          }
        }
      }
      if (this.ForceEvaluateStateFeaturesFromPlugin.Contains<string>(feature.Id))
      {
        foreach (ParsedFeatureRule defaultValueRule in cachedFeature.DefaultValueRules)
        {
          string reason;
          ContributedFeatureEnabledValue enabledValueFromPlugin = defaultValueRule.GetEnabledValueFromPlugin(requestContext, scopeValues, out reason);
          if (enabledValueFromPlugin != ContributedFeatureEnabledValue.Undefined)
          {
            state.State = enabledValueFromPlugin;
            state.Reason = reason;
            break;
          }
        }
      }
      return state;
    }

    public ContributedFeatureState GetExplicitFeatureState(
      IVssRequestContext requestContext,
      string contributionId,
      IDictionary<string, string> allScopeValues)
    {
      ContributedFeatureState state = (ContributedFeatureState) null;
      CachedContributedFeature contributedFeature;
      if (this.GetFeaturesLookup(requestContext).TryGetValue(contributionId, out contributedFeature))
      {
        state = new ContributedFeatureState()
        {
          FeatureId = contributedFeature.Feature.Id
        };
        this.PopulateExplicitFeatureEnabledValue(requestContext, contributedFeature.Feature, state, allScopeValues);
      }
      return state;
    }

    private void PopulateExplicitFeatureEnabledValue(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      ContributedFeatureState state,
      IDictionary<string, string> allScopeValues,
      int scopesToSkip = 0)
    {
      if (feature.Scopes == null || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      foreach (ContributedFeatureSettingScope featureSettingScope in feature.Scopes.Skip<ContributedFeatureSettingScope>(scopesToSkip))
      {
        string scopeValue = (string) null;
        if (allScopeValues != null && !string.IsNullOrEmpty(featureSettingScope.SettingScope))
          allScopeValues.TryGetValue(featureSettingScope.SettingScope, out scopeValue);
        ContributedFeatureEnabledValue featureEnabledValue = this.GetFeatureEnabledValue(requestContext, feature.Id, featureSettingScope.UserScoped ? SettingsUserScope.User : SettingsUserScope.AllUsers, featureSettingScope.SettingScope, scopeValue);
        if (featureEnabledValue != ContributedFeatureEnabledValue.Undefined)
        {
          state.State = featureEnabledValue;
          state.Scope = featureSettingScope;
          break;
        }
      }
    }

    public void SetFeatureState(
      IVssRequestContext requestContext,
      string contributionId,
      ContributedFeatureEnabledValue state,
      SettingsUserScope userScope)
    {
      this.SetFeatureState(requestContext, contributionId, state, userScope, (string) null, (string) null);
    }

    public void SetFeatureState(
      IVssRequestContext requestContext,
      string featureId,
      ContributedFeatureEnabledValue state,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureId, nameof (featureId));
      CachedContributedFeature contributedFeature;
      if (!this.GetFeaturesLookup(requestContext).TryGetValue(featureId, out contributedFeature))
        throw new ContributedFeatureNotFoundException(ExtMgmtResources.ContributedFeatureNotFoundMessage((object) featureId));
      this.GetScopeIndex(requestContext, contributedFeature.Feature, userScope, scopeName);
      ISettingsService settingsService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<ISettingsService>() : throw new NotSupportedException("Cannot use the ContributedFeatureService to set settings outside of the project-collection level.");
      if (state == ContributedFeatureEnabledValue.Undefined)
        settingsService.RemoveValue(requestContext, userScope, scopeName, scopeValue, "Features/" + featureId, false, true);
      else
        settingsService.SetValue(requestContext, userScope, scopeName, scopeValue, "Features/" + featureId, (object) state, true);
      this.GetCachedFeatureStatesLookup(requestContext).Remove(contributedFeature.Feature.Id);
      IEnumerable<ContributedFeatureListener> changedListeners = contributedFeature.Feature.FeatureStateChangedListeners;
      if (changedListeners == null)
        return;
      foreach (ContributedFeatureListener contributedFeatureListener in changedListeners)
      {
        IContributedFeatureStateChangedListener stateChangedListener;
        if (this.m_registeredListeners.TryGetValue(contributedFeatureListener.Name, out stateChangedListener))
        {
          try
          {
            stateChangedListener.OnFeatureStateChanged(requestContext, featureId, state, userScope, scopeName, scopeValue, contributedFeatureListener.Properties);
          }
          catch (ContributedFeatureListenerCallbackException ex)
          {
            requestContext.TraceException(10026101, "FeatureManagement", nameof (ContributedFeatureService), (Exception) ex);
            throw;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10026100, TraceLevel.Info, "FeatureManagement", nameof (ContributedFeatureService), ex);
          }
        }
      }
    }

    private int GetScopeIndex(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      SettingsUserScope userScope,
      string scopeName)
    {
      if (feature.Scopes != null)
      {
        int scopeIndex = 0;
        foreach (ContributedFeatureSettingScope scope in feature.Scopes)
        {
          if (userScope.IsUserScoped == scope.UserScoped && scopeName == scope.SettingScope)
            return scopeIndex;
          ++scopeIndex;
        }
      }
      throw new ContributedFeatureInvalidScopeException(ExtMgmtResources.ContributedFeatureInvalidScopeMessage((object) feature.Id, (object) scopeName));
    }

    private IDictionary<string, CachedContributedFeature> GetFeaturesLookup(
      IVssRequestContext requestContext)
    {
      if (requestContext.Items.ContainsKey("contributed-features"))
        return requestContext.Items["contributed-features"] as IDictionary<string, CachedContributedFeature>;
      using (PerformanceTimer.StartMeasure(requestContext, "ContributedFeatureService.GetFeatures"))
      {
        IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
        IEnumerable<Contribution> contributions;
        IDictionary<string, CachedContributedFeature> associatedData;
        if (!service.QueryContributionsForType<IDictionary<string, CachedContributedFeature>>(requestContext, "ms.vss-web.feature", "features", out contributions, out associatedData))
        {
          associatedData = (IDictionary<string, CachedContributedFeature>) new Dictionary<string, CachedContributedFeature>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (contributions != null)
          {
            foreach (Contribution contribution in contributions)
            {
              ContributedFeature feature = new ContributedFeature()
              {
                Id = contribution.Id,
                Name = contribution.GetProperty<string>("name", contribution.Id),
                Description = contribution.Description,
                DefaultState = contribution.GetProperty<bool>("defaultState"),
                ServiceInstanceType = contribution.GetProperty<Guid?>("serviceInstanceType"),
                Links = ContributionMethods.GetReferenceLinks(contribution, "links"),
                IncludeAsClaim = contribution.GetProperty<bool>("includeAsClaim"),
                Tags = (IEnumerable<string>) contribution.GetProperty<string[]>("tags"),
                Order = contribution.GetProperty<int>("order"),
                FeatureProperties = contribution.GetProperty<Dictionary<string, object>>("featureProperties")
              };
              List<ContributedFeatureSettingScope> featureSettingScopeList = new List<ContributedFeatureSettingScope>();
              feature.Scopes = (IEnumerable<ContributedFeatureSettingScope>) featureSettingScopeList;
              if (contribution.GetProperty<bool>("userConfigurable"))
              {
                foreach (string str in contribution.GetProperty<string[]>("userScopes") ?? new string[1])
                  featureSettingScopeList.Add(new ContributedFeatureSettingScope()
                  {
                    UserScoped = true,
                    SettingScope = str
                  });
              }
              if (contribution.GetProperty<bool>("hostConfigurable"))
              {
                foreach (string str in contribution.GetProperty<string[]>("hostScopes") ?? new string[1])
                  featureSettingScopeList.Add(new ContributedFeatureSettingScope()
                  {
                    UserScoped = false,
                    SettingScope = str
                  });
              }
              feature.DefaultValueRules = (IEnumerable<ContributedFeatureValueRule>) this.GetValueRules(contribution.GetPropertyRawValue("defaultValueRules") as IEnumerable<object>);
              feature.OverrideRules = (IEnumerable<ContributedFeatureValueRule>) this.GetValueRules(contribution.GetPropertyRawValue("overrideRules") as IEnumerable<object>);
              feature.FeatureStateChangedListeners = (IEnumerable<ContributedFeatureListener>) this.GetListenerSettings(contribution.GetPropertyRawValue("featureStateChangedListeners") as IEnumerable<object>);
              associatedData[contribution.Id] = new CachedContributedFeature(requestContext, feature, this.m_registeredPlugins);
            }
          }
          service.Set(requestContext, "features", contributions, (object) associatedData);
        }
        requestContext.Items["contributed-features"] = (object) associatedData;
        return associatedData;
      }
    }

    private IDictionary<string, ContributedFeatureState> GetCachedFeatureStatesLookup(
      IVssRequestContext requestContext)
    {
      IDictionary<string, ContributedFeatureState> featureStatesLookup;
      if (requestContext.Items.ContainsKey("contributed-feature-states"))
      {
        featureStatesLookup = requestContext.Items["contributed-feature-states"] as IDictionary<string, ContributedFeatureState>;
      }
      else
      {
        featureStatesLookup = (IDictionary<string, ContributedFeatureState>) new Dictionary<string, ContributedFeatureState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["contributed-feature-states"] = (object) featureStatesLookup;
      }
      return featureStatesLookup;
    }

    private List<ContributedFeatureValueRule> GetValueRules(IEnumerable<object> rules)
    {
      List<ContributedFeatureValueRule> valueRules = (List<ContributedFeatureValueRule>) null;
      if (rules != null && rules.Any<object>())
      {
        valueRules = new List<ContributedFeatureValueRule>();
        foreach (IDictionary<string, object> dictionary in rules.OfType<IDictionary<string, object>>())
        {
          object obj;
          if (dictionary != null && dictionary.TryGetValue("name", out obj) && !string.IsNullOrEmpty(obj as string))
          {
            ContributedFeatureValueRule featureValueRule = new ContributedFeatureValueRule();
            featureValueRule.Name = (string) obj;
            dictionary.Remove("name");
            featureValueRule.Properties = dictionary;
            valueRules.Add(featureValueRule);
          }
        }
      }
      return valueRules;
    }

    private List<ContributedFeatureListener> GetListenerSettings(IEnumerable<object> settings)
    {
      List<ContributedFeatureListener> listenerSettings = (List<ContributedFeatureListener>) null;
      if (settings != null && settings.Any<object>())
      {
        listenerSettings = new List<ContributedFeatureListener>();
        foreach (IDictionary<string, object> dictionary in settings.OfType<IDictionary<string, object>>())
        {
          object obj;
          if (dictionary != null && dictionary.TryGetValue("name", out obj) && !string.IsNullOrEmpty(obj as string))
          {
            ContributedFeatureListener contributedFeatureListener = new ContributedFeatureListener();
            contributedFeatureListener.Name = (string) obj;
            dictionary.Remove("name");
            contributedFeatureListener.Properties = dictionary;
            listenerSettings.Add(contributedFeatureListener);
          }
        }
      }
      return listenerSettings;
    }

    private ContributedFeatureEnabledValue GetFeatureEnabledValue(
      IVssRequestContext requestContext,
      string contributionId,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue)
    {
      bool flag = false;
      if (!userScope.IsUserScoped && string.IsNullOrEmpty(scopeName))
        flag = true;
      return requestContext.GetService<ISettingsService>().GetValue<ContributedFeatureEnabledValue>(flag ? requestContext.Elevate() : requestContext, userScope, scopeName, scopeValue, "Features/" + contributionId, ContributedFeatureEnabledValue.Undefined, false);
    }
  }
}
