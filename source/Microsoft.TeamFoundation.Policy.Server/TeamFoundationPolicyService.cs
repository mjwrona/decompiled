// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.TeamFoundationPolicyService
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.Policy.Server.Utils;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class TeamFoundationPolicyService : ITeamFoundationPolicyService, IVssFrameworkService
  {
    private JsonSerializer m_contextSerializer;
    private JsonSerializerSettings m_settingsSerializerSettings;
    private PolicyTemplateCache m_allPolicyTemplatesCache;
    private static ConcurrentDictionary<Type, string[]> s_featureFlagCache = new ConcurrentDictionary<Type, string[]>();
    private const int c_minTop = 1;
    private const int c_minSkip = 0;
    private const int c_defaultTop = 100;
    private const int c_defaultSkip = 0;
    private const int c_maxTop = 1000;
    private const string c_layer = "PolicyService";

    internal TeamFoundationPolicyService()
    {
      CamelCasePropertyNamesContractResolver contractResolver = new CamelCasePropertyNamesContractResolver();
      this.m_contextSerializer = new JsonSerializer()
      {
        ContractResolver = (IContractResolver) contractResolver
      };
      this.m_settingsSerializerSettings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) contractResolver
      };
    }

    [ExcludeFromCodeCoverage]
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(1390001, 1390002, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.m_allPolicyTemplatesCache = new PolicyTemplateCache(this.GetInstanceOfAllPolicyTypes(requestContext));
      }
    }

    protected virtual IReadOnlyList<ITeamFoundationPolicy> GetInstanceOfAllPolicyTypes(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyList<ITeamFoundationPolicy>) requestContext.GetExtensions<ITeamFoundationPolicy>();
    }

    [ExcludeFromCodeCoverage]
    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(1390004, 1390005, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        if (this.m_allPolicyTemplatesCache == null)
          return;
        ((IDisposable) this.m_allPolicyTemplatesCache)?.Dispose();
        this.m_allPolicyTemplatesCache = (PolicyTemplateCache) null;
      }
    }

    [ExcludeFromCodeCoverage]
    protected virtual IPolicyComponent CreatePolicyComponent(IVssRequestContext requestContext)
    {
      IPolicyConfigurationVersionedCacheService service = requestContext.GetService<IPolicyConfigurationVersionedCacheService>();
      return (IPolicyComponent) new PolicyComponentFacadeVersioned(requestContext, service);
    }

    protected virtual void CheckTeamProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requestedPermissions)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string projectUri = ProjectInfo.GetProjectUri(projectId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri);
      securityNamespace.CheckPermission(requestContext, token, requestedPermissions);
    }

    public virtual void CheckManageEnterprisePolicyPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).CheckPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, TeamProjectCollectionPermissions.ManageEnterprisePolicies, false);

    protected virtual Guid ReadRequestIdentityId(IVssRequestContext requestContext) => requestContext.GetUserId(true);

    private void CheckViewPolicyPermission(IVssRequestContext requestContext, Guid projectId) => this.CheckTeamProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);

    private void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      ITeamFoundationPolicy policyTypeData,
      Guid projectId,
      object settings)
    {
      policyTypeData.CheckEditPoliciesPermission(requestContext, projectId, settings);
    }

    internal bool TryGetPolicyType(
      IVssRequestContext requestContext,
      Guid typeId,
      out ITeamFoundationPolicy policyType)
    {
      return this.m_allPolicyTemplatesCache.TryGetValue(typeId, out policyType);
    }

    internal static object DeserializeAndCheckSettings(
      IVssRequestContext requestContext,
      ITeamFoundationPolicy policyTypeData,
      Guid projectId,
      int? configurationId,
      string settings)
    {
      object settings1;
      try
      {
        settings1 = policyTypeData.DeserializeSettings(settings);
      }
      catch (Exception ex)
      {
        throw new PolicySettingsFormatException(ex.Message);
      }
      string errorMessage;
      bool flag;
      try
      {
        flag = policyTypeData.CheckSettingsValidity(requestContext, projectId, settings1, out errorMessage);
      }
      catch (Exception ex)
      {
        throw new PolicyImplementationException(policyTypeData.DisplayName, configurationId, ex);
      }
      if (!flag)
      {
        if (string.IsNullOrWhiteSpace(errorMessage))
          throw new PolicyImplementationException(policyTypeData.DisplayName, configurationId, PolicyResources.Format("PolicyImplementationBadParseSettings"));
        throw new PolicySettingsFormatException(errorMessage);
      }
      requestContext.Trace(1390057, TraceLevel.Verbose, "Policy", "PolicyService", "Parsed policy settings for project ID {0} given policy ID {1}.", (object) projectId, (object) policyTypeData.Id);
      return settings1;
    }

    public string[] DetermineScopes(
      IVssRequestContext requestContext,
      PolicyConfigurationRecord configRecord)
    {
      ITeamFoundationPolicy policyType;
      if (!this.TryGetPolicyType(requestContext, configRecord.TypeId, out policyType))
        throw new PolicyTypeNotFoundException(configRecord.TypeId);
      try
      {
        object settings = policyType.DeserializeSettings(configRecord.Settings);
        return policyType.GetScopes(settings);
      }
      catch (Exception ex)
      {
        throw new PolicySettingsFormatException(ex.Message);
      }
    }

    private static void EvaluateTopSkip(int? top, int? skip, out int topValue, out int skipValue)
    {
      ref int local1 = ref topValue;
      int? nullable;
      int num1;
      if (top.HasValue)
      {
        nullable = top;
        int num2 = 1;
        if (!(nullable.GetValueOrDefault() < num2 & nullable.HasValue))
        {
          num1 = top.Value;
          goto label_4;
        }
      }
      num1 = 100;
label_4:
      local1 = num1;
      ref int local2 = ref skipValue;
      int num3;
      if (skip.HasValue)
      {
        nullable = skip;
        int num4 = 0;
        if (!(nullable.GetValueOrDefault() < num4 & nullable.HasValue))
        {
          num3 = skip.Value;
          goto label_8;
        }
      }
      num3 = 0;
label_8:
      local2 = num3;
      topValue = Math.Min(1000, topValue);
    }

    private static bool IsPolicyFeatureFlagEnabled(
      IVssRequestContext requestContext,
      ITeamFoundationPolicy policy)
    {
      Type type = policy.GetType();
      string[] source;
      if (!TeamFoundationPolicyService.s_featureFlagCache.TryGetValue(type, out source))
      {
        TeamFoundationPolicyService.s_featureFlagCache[type] = type.GetCustomAttributes(typeof (FeatureEnabledAttribute), true).Cast<FeatureEnabledAttribute>().Select<FeatureEnabledAttribute, string>((Func<FeatureEnabledAttribute, string>) (attribute => attribute.FeatureFlag)).ToArray<string>();
        source = TeamFoundationPolicyService.s_featureFlagCache[type];
      }
      return source.Length == 0 || ((IEnumerable<string>) source).All<string>((Func<string, bool>) (featureFlag => requestContext.IsFeatureEnabled(featureFlag)));
    }

    private ITeamFoundationPolicyTarget GetTargetForArtifact(
      IVssRequestContext requestContext,
      ArtifactId artifactId,
      bool throwIfNotFound)
    {
      foreach (ITeamFoundationPolicyArtifactExtension extension in (IEnumerable<ITeamFoundationPolicyArtifactExtension>) requestContext.GetExtensions<ITeamFoundationPolicyArtifactExtension>(ExtensionLifetime.Service))
      {
        try
        {
          ITeamFoundationPolicyTarget artifactTarget = extension.TryGetArtifactTarget(requestContext, artifactId);
          if (artifactTarget != null)
            return artifactTarget;
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (VssException ex)
        {
          if (throwIfNotFound)
          {
            throw;
          }
          else
          {
            requestContext.TraceException(1390113, TraceLevel.Error, "Policy", "PolicyService", (Exception) ex);
            return (ITeamFoundationPolicyTarget) null;
          }
        }
        catch (Exception ex)
        {
          PolicyImplementationException implementationException = new PolicyImplementationException(extension.GetType().FullName, ex);
          requestContext.TraceException(1390112, "Policy", "PolicyService", (Exception) implementationException);
        }
      }
      if (throwIfNotFound)
        throw new PolicyImplementationException(string.Format("PolicyArtifactCouldNotBeDecoded", (object) LinkingUtilities.EncodeUri(artifactId)));
      return (ITeamFoundationPolicyTarget) null;
    }

    private void AuditLogConfigurationUpdated(
      IVssRequestContext requestContext,
      string action,
      ITeamFoundationPolicy policyTypeData,
      Guid projectId,
      PolicyConfigurationRecord oldConfiguration,
      object oldSettings,
      PolicyConfigurationRecord newConfiguration,
      object newSettings)
    {
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId);
      JArray changeList = new JArray();
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
      {
        ["PolicyTypeId"] = (object) policyTypeData.Id,
        ["PolicyTypeDisplayName"] = (object) policyTypeData.DisplayName,
        ["ProjectName"] = (object) projectName,
        ["ConfigId"] = (object) (newConfiguration != null ? new int?(newConfiguration.ConfigurationId) : oldConfiguration?.ConfigurationId),
        ["RevisionId"] = (object) (newConfiguration != null ? new int?(newConfiguration.ConfigurationRevisionId) : oldConfiguration?.ConfigurationRevisionId),
        ["ConfigProperties"] = (object) changeList
      };
      this.AddConfigChangeDetail(changeList, "IsBlocking", (JToken) (new JValue((object) oldConfiguration?.IsBlocking) ?? JValue.CreateNull()), (JToken) (new JValue((object) newConfiguration?.IsBlocking) ?? JValue.CreateNull()));
      this.AddConfigChangeDetail(changeList, "IsEnabled", (JToken) (new JValue((object) oldConfiguration?.IsEnabled) ?? JValue.CreateNull()), (JToken) (new JValue((object) newConfiguration?.IsEnabled) ?? JValue.CreateNull()));
      if (policyTypeData is IPolicySettingsAuditDetailsProvider auditDetailsProvider)
      {
        dictionary1["PolicyTypeDisplayName"] = (object) auditDetailsProvider.AuditDisplayName;
        object settings = newSettings ?? oldSettings;
        if (settings != null)
        {
          Dictionary<string, JToken> dictionary2 = auditDetailsProvider.SummarizePolicyScope(requestContext, settings);
          // ISSUE: explicit non-virtual call
          if (dictionary2 != null && __nonvirtual (dictionary2.Count) > 0)
          {
            foreach (KeyValuePair<string, JToken> keyValuePair in dictionary2)
              dictionary1.Add(keyValuePair.Key, (object) keyValuePair.Value);
          }
        }
        Dictionary<string, JToken> dictionary3 = (Dictionary<string, JToken>) null;
        Dictionary<string, JToken> dictionary4 = (Dictionary<string, JToken>) null;
        HashSet<string> stringSet = new HashSet<string>();
        if (oldSettings != null)
        {
          dictionary3 = auditDetailsProvider.SummarizePolicySettings(requestContext, oldSettings);
          stringSet.UnionWith((IEnumerable<string>) dictionary3.Keys);
        }
        if (newSettings != null)
        {
          dictionary4 = auditDetailsProvider.SummarizePolicySettings(requestContext, newSettings);
          stringSet.UnionWith((IEnumerable<string>) dictionary4.Keys);
        }
        foreach (string key in stringSet)
        {
          JToken oldValue;
          if (dictionary3 == null || !dictionary3.TryGetValue(key, out oldValue))
            oldValue = (JToken) JValue.CreateNull();
          JToken jtoken;
          if (dictionary4 == null || !dictionary4.TryGetValue(key, out jtoken))
            jtoken = (JToken) JValue.CreateNull();
          this.AddConfigChangeDetail(changeList, "Settings." + key, oldValue, jtoken);
        }
      }
      IVssRequestContext requestContext1 = requestContext;
      string actionId = action;
      Dictionary<string, object> data = dictionary1;
      Guid guid = projectId;
      Guid targetHostId = new Guid();
      Guid projectId1 = guid;
      requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId1);
    }

    private bool AddConfigChangeDetail(
      JArray changeList,
      string property,
      JToken oldValue,
      JToken value)
    {
      if (JToken.DeepEquals(oldValue, value))
        return false;
      JObject jobject = new JObject()
      {
        [nameof (property)] = (JToken) property,
        [nameof (oldValue)] = oldValue,
        [nameof (value)] = value
      };
      changeList.Add((JToken) jobject);
      return true;
    }

    public PolicyConfigurationRecord GetPolicyConfigurationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId,
      int? revisionId = null)
    {
      using (requestContext.TraceBlock(1390028, 1390029, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        TeamFoundationPolicyService.CheckForConfigurationIdOutOfRange(policyConfigurationId, revisionId);
        this.CheckViewPolicyPermission(requestContext, projectId);
        PolicyConfigurationRecord configurationRecord = (PolicyConfigurationRecord) null;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          configurationRecord = policyComponent.GetPolicyConfiguration(projectId, policyConfigurationId, revisionId);
        return configurationRecord != null ? configurationRecord : throw new PolicyConfigurationNotFoundException(policyConfigurationId, revisionId);
      }
    }

    public PolicyConfigurationRecord GetLatestPolicyConfigurationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId)
    {
      using (requestContext.TraceBlock(1390028, 1390029, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        TeamFoundationPolicyService.CheckForConfigurationIdOutOfRange(policyConfigurationId);
        this.CheckViewPolicyPermission(requestContext, projectId);
        PolicyConfigurationRecord configurationRecord = (PolicyConfigurationRecord) null;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          configurationRecord = policyComponent.GetLatestPolicyConfiguration(projectId, policyConfigurationId);
        return configurationRecord != null ? configurationRecord : throw new PolicyConfigurationNotFoundException(policyConfigurationId);
      }
    }

    public IEnumerable<PolicyConfigurationRecord> GetPolicyConfigurationRecordRevisions(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId,
      int? top = null,
      int? skip = null)
    {
      using (requestContext.TraceBlock(1390058, 1390059, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        TeamFoundationPolicyService.CheckForConfigurationIdOutOfRange(policyConfigurationId);
        this.CheckViewPolicyPermission(requestContext, projectId);
        int topValue;
        int skipValue;
        TeamFoundationPolicyService.EvaluateTopSkip(top, skip, out topValue, out skipValue);
        return this.GetPolicyConfigurationRecordRevisionsInternal(requestContext, projectId, policyConfigurationId, topValue, skipValue);
      }
    }

    private static void CheckForConfigurationIdOutOfRange(
      int policyConfigurationId,
      int? revisionId = null)
    {
      if (policyConfigurationId < 1 || revisionId.HasValue && revisionId.Value < 1)
        throw new PolicyConfigurationNotFoundException(policyConfigurationId, revisionId);
    }

    private IEnumerable<PolicyConfigurationRecord> GetPolicyConfigurationRecordRevisionsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId,
      int top,
      int skip)
    {
      using (IPolicyComponent component = this.CreatePolicyComponent(requestContext))
      {
        using (VirtualResultCollection<PolicyConfigurationRecord> rc = component.GetPolicyConfigurationRevisions(projectId, policyConfigurationId, top, skip))
        {
          foreach (PolicyConfigurationRecord currentAs in rc.GetCurrentAsEnumerable())
            yield return currentAs;
        }
      }
    }

    public IEnumerable<PolicyConfigurationRecord> GetLatestPolicyConfigurationRecords(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return this.GetLatestPolicyConfigurationRecords(requestContext, projectId, int.MaxValue, 1, out int? _, new Guid?());
    }

    public IEnumerable<PolicyConfigurationRecord> GetLatestPolicyConfigurationRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId,
      Guid? policyType = null)
    {
      using (requestContext.TraceBlock(1390007, 1390008, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        this.CheckViewPolicyPermission(requestContext, projectId);
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
        {
          nextConfigurationId = new int?();
          if (top == int.MaxValue && firstConfigurationId == 1)
            return (IEnumerable<PolicyConfigurationRecord>) policyComponent.GetLatestPolicyConfigurations(projectId, top, firstConfigurationId, policyType);
          IList<PolicyConfigurationRecord> policyConfigurations = policyComponent.GetLatestPolicyConfigurations(projectId, top, firstConfigurationId, policyType);
          IEnumerable<PolicyConfigurationRecord> configurationRecords = policyConfigurations.Take<PolicyConfigurationRecord>(top);
          PolicyConfigurationRecord configurationRecord = policyConfigurations.Skip<PolicyConfigurationRecord>(top).FirstOrDefault<PolicyConfigurationRecord>();
          if (configurationRecord != null)
            nextConfigurationId = new int?(configurationRecord.ConfigurationId);
          return configurationRecords;
        }
      }
    }

    public IEnumerable<PolicyConfigurationRecord> GetLatestPolicyConfigurationRecordsByScope(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> scopes,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId,
      Guid? policyType = null)
    {
      using (requestContext.TraceBlock(1390011, 1390012, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(scopes, nameof (scopes));
        this.CheckViewPolicyPermission(requestContext, projectId);
        bool useVersion2 = requestContext.IsFeatureEnabled("Policy.EnablePolicyByScopeSprocV2");
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
        {
          nextConfigurationId = new int?();
          IList<PolicyConfigurationRecord> configurationsByScope = policyComponent.GetLatestPolicyConfigurationsByScope(projectId, scopes, (Func<PolicyConfigurationRecord, IEnumerable<string>>) (r => (IEnumerable<string>) this.DetermineScopes(requestContext, r)), top, firstConfigurationId, policyType, useVersion2: useVersion2);
          if (top == int.MaxValue && firstConfigurationId == 1)
            return (IEnumerable<PolicyConfigurationRecord>) configurationsByScope;
          IEnumerable<PolicyConfigurationRecord> configurationRecordsByScope = configurationsByScope.Take<PolicyConfigurationRecord>(top);
          PolicyConfigurationRecord configurationRecord = configurationsByScope.Skip<PolicyConfigurationRecord>(top).FirstOrDefault<PolicyConfigurationRecord>();
          if (configurationRecord != null)
            nextConfigurationId = new int?(configurationRecord.ConfigurationId);
          return configurationRecordsByScope;
        }
      }
    }

    public IDictionary<string, int> GetPolicyConfigurationsCountByScope(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> scopes)
    {
      using (requestContext.TraceBlock(1390021, 1390022, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(scopes, nameof (scopes));
        this.CheckViewPolicyPermission(requestContext, projectId);
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          return policyComponent.GetPolicyConfigurationsCountByScope(projectId, scopes);
      }
    }

    public ITeamFoundationPolicy GetPolicyType(IVssRequestContext requestContext, Guid typeId)
    {
      using (requestContext.TraceBlock(1390031, 1390032, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(typeId, nameof (typeId));
        ITeamFoundationPolicy policyType;
        if (!this.TryGetPolicyType(requestContext, typeId, out policyType))
          throw new PolicyTypeNotFoundException(typeId);
        if (requestContext.IsFeatureEnabled("Policy.HiddenPolicies") && policyType.IsHidden)
          throw new PolicyTypeNotFoundException(typeId);
        return policyType;
      }
    }

    public IEnumerable<ITeamFoundationPolicy> GetPolicyTypes(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(1390034, 1390035, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        IEnumerable<ITeamFoundationPolicy> source = this.m_allPolicyTemplatesCache.Values.Where<ITeamFoundationPolicy>((Func<ITeamFoundationPolicy, bool>) (policy => TeamFoundationPolicyService.IsPolicyFeatureFlagEnabled(requestContext, policy)));
        if (requestContext.IsFeatureEnabled("Policy.HiddenPolicies"))
          source = source.Where<ITeamFoundationPolicy>((Func<ITeamFoundationPolicy, bool>) (policy => !policy.IsHidden));
        return source;
      }
    }

    public PolicyConfigurationRecord CreatePolicyConfiguration(
      IVssRequestContext requestContext,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings)
    {
      using (requestContext.TraceBlock(1390019, 1390020, "Policy", "PolicyService", nameof (CreatePolicyConfiguration)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(typeId, nameof (typeId));
        ArgumentUtility.CheckForNull<string>(settings, nameof (settings));
        ITeamFoundationPolicy policyType;
        if (!this.TryGetPolicyType(requestContext, typeId, out policyType) || !TeamFoundationPolicyService.IsPolicyFeatureFlagEnabled(requestContext, policyType) || requestContext.IsFeatureEnabled("Policy.HiddenPolicies") && policyType.IsHidden)
          throw new PolicyTypeNotFoundException(typeId);
        if (isEnterpriseManaged)
        {
          if (!requestContext.IsFeatureEnabled("Policy.EnterpriseManagedPolicies"))
            throw new InvalidOperationException(PolicyResources.Get("EnterpriseDisabled"));
          this.CheckManageEnterprisePolicyPermission(requestContext);
        }
        object obj = TeamFoundationPolicyService.DeserializeAndCheckSettings(requestContext, policyType, projectId, new int?(), settings);
        string settings1 = JsonConvert.SerializeObject(obj, this.m_settingsSerializerSettings);
        this.CheckEditPoliciesPermission(requestContext, policyType, projectId, obj);
        policyType.CheckSupportedMatchKind(requestContext, obj);
        Guid creatorId = this.ReadRequestIdentityId(requestContext);
        PolicyConfigurationRecord configurationRecord = new PolicyConfigurationRecord(typeId, projectId, isEnabled, isBlocking, isEnterpriseManaged, settings1, creatorId);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        try
        {
          service.PublishDecisionPoint(requestContext, (object) new PolicyConfigurationCreatedNotification(projectId, configurationRecord));
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          throw new PolicyChangeRejectedByPolicyException(ex);
        }
        PolicyConfigurationRecord policyConfiguration;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          policyConfiguration = policyComponent.CreatePolicyConfiguration(configurationRecord, (Func<PolicyConfigurationRecord, IEnumerable<string>>) (r => (IEnumerable<string>) this.DetermineScopes(requestContext, r)));
        try
        {
          this.AuditLogConfigurationUpdated(requestContext, "Policy.PolicyConfigCreated", policyType, projectId, (PolicyConfigurationRecord) null, (object) null, policyConfiguration, obj);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1390045, "Policy", "PolicyService", ex);
        }
        service.PublishNotification(requestContext, (object) new PolicyConfigurationCreatedNotification(projectId, policyConfiguration));
        TeamFoundationPolicyService.AddConfigurationCTData(requestContext, nameof (CreatePolicyConfiguration), isBlocking, policyType, obj, policyConfiguration);
        return policyConfiguration;
      }
    }

    public PolicyConfigurationRecord UpdatePolicyConfiguration(
      IVssRequestContext requestContext,
      int configurationId,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings)
    {
      using (requestContext.TraceBlock(1390043, 1390044, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        TeamFoundationPolicyService.CheckForConfigurationIdOutOfRange(configurationId);
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<string>(settings, nameof (settings));
        ITeamFoundationPolicy policyType;
        if (!this.TryGetPolicyType(requestContext, typeId, out policyType))
          throw new PolicyTypeNotFoundException(typeId);
        object obj1 = TeamFoundationPolicyService.DeserializeAndCheckSettings(requestContext, policyType, projectId, new int?(configurationId), settings);
        string settings1 = JsonConvert.SerializeObject(obj1, this.m_settingsSerializerSettings);
        this.CheckEditPoliciesPermission(requestContext, policyType, projectId, obj1);
        if (isEnterpriseManaged)
        {
          if (!requestContext.IsFeatureEnabled("Policy.EnterpriseManagedPolicies"))
            throw new InvalidOperationException(PolicyResources.Get("EnterpriseDisabled"));
          this.CheckManageEnterprisePolicyPermission(requestContext);
        }
        PolicyConfigurationRecord configurationRecord1 = this.GetLatestPolicyConfigurationRecord(requestContext, projectId, configurationId);
        if (configurationRecord1.TypeId != typeId)
          throw new PolicyTypeCannotBeChangedException();
        object obj2 = policyType.DeserializeSettings(configurationRecord1.Settings);
        this.CheckEditPoliciesPermission(requestContext, policyType, projectId, obj2);
        if (configurationRecord1.IsEnterpriseManaged)
          this.CheckManageEnterprisePolicyPermission(requestContext);
        requestContext.GetService<IdentityService>();
        Guid creatorId = this.ReadRequestIdentityId(requestContext);
        PolicyConfigurationRecord configurationRecord2 = new PolicyConfigurationRecord(configurationId, typeId, projectId, isEnabled, isBlocking, isEnterpriseManaged, settings1, creatorId);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        try
        {
          service.PublishDecisionPoint(requestContext, (object) new PolicyConfigurationUpdatedNotification(projectId, configurationRecord2));
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          throw new PolicyChangeRejectedByPolicyException(ex);
        }
        PolicyConfigurationRecord configurationRecord3;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
        {
          string[] scopes = policyType.GetScopes(obj1);
          configurationRecord3 = policyComponent.UpdatePolicyConfiguration(configurationRecord2, (Func<PolicyConfigurationRecord, IEnumerable<string>>) (r => (IEnumerable<string>) this.DetermineScopes(requestContext, r)), (IEnumerable<string>) scopes, configurationRecord1.ConfigurationRevisionId + 1);
        }
        try
        {
          this.AuditLogConfigurationUpdated(requestContext, "Policy.PolicyConfigModified", policyType, projectId, configurationRecord1, obj2, configurationRecord3, obj1);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1390045, "Policy", "PolicyService", ex);
        }
        service.PublishNotification(requestContext, (object) new PolicyConfigurationUpdatedNotification(projectId, configurationRecord3));
        TeamFoundationPolicyService.AddConfigurationCTData(requestContext, nameof (UpdatePolicyConfiguration), isBlocking, policyType, obj1, configurationRecord3);
        return configurationRecord3;
      }
    }

    public void DeletePolicyConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId)
    {
      using (requestContext.TraceBlock(1390022, 1390023, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        TeamFoundationPolicyService.CheckForConfigurationIdOutOfRange(policyConfigurationId);
        PolicyConfigurationRecord configurationRecord1 = this.GetLatestPolicyConfigurationRecord(requestContext, projectId, policyConfigurationId);
        if (configurationRecord1.IsEnterpriseManaged)
          this.CheckManageEnterprisePolicyPermission(requestContext);
        object obj = (object) null;
        ITeamFoundationPolicy policyType;
        if (this.TryGetPolicyType(requestContext, configurationRecord1.TypeId, out policyType))
        {
          obj = policyType.DeserializeSettings(configurationRecord1.Settings);
          this.CheckEditPoliciesPermission(requestContext, policyType, projectId, obj);
        }
        requestContext.GetService<IdentityService>();
        Guid modifiedById = this.ReadRequestIdentityId(requestContext);
        string[] scopes;
        try
        {
          scopes = this.DetermineScopes(requestContext, configurationRecord1);
        }
        catch (PolicyTypeNotFoundException ex)
        {
          scopes = new string[1]{ "*" };
        }
        PolicyConfigurationRecord configurationRecord2;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          configurationRecord2 = policyComponent.DeletePolicyConfiguration(projectId, policyConfigurationId, modifiedById, (Func<PolicyConfigurationRecord, IEnumerable<string>>) (r => (IEnumerable<string>) this.DetermineScopes(requestContext, r)), (IEnumerable<string>) scopes, configurationRecord1.ConfigurationRevisionId + 1);
        try
        {
          this.AuditLogConfigurationUpdated(requestContext, "Policy.PolicyConfigRemoved", policyType, projectId, configurationRecord2, obj, (PolicyConfigurationRecord) null, (object) null);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1390045, "Policy", "PolicyService", ex);
        }
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        PolicyConfigurationDeletedNotification deletedNotification = new PolicyConfigurationDeletedNotification(projectId, configurationRecord2);
        IVssRequestContext requestContext1 = requestContext;
        PolicyConfigurationDeletedNotification notificationEvent = deletedNotification;
        service.PublishNotification(requestContext1, (object) notificationEvent);
        TeamFoundationPolicyService.AddConfigurationCTData(requestContext, nameof (DeletePolicyConfiguration), false, policyType, (object) null, configurationRecord2);
      }
    }

    private static void AddConfigurationCTData(
      IVssRequestContext requestContext,
      string action,
      bool isBlocking,
      ITeamFoundationPolicy policyTypeData,
      object settingsObj,
      PolicyConfigurationRecord configuration)
    {
      if (configuration == null)
        return;
      ClientTraceData eventData = new ClientTraceData();
      eventData.Add("Action", (object) action);
      eventData.Add("TeamProjectId", (object) configuration.ProjectId.ToString());
      eventData.Add("ConfigurationId", (object) configuration.ConfigurationId);
      eventData.Add("PolicyTypeId", (object) policyTypeData?.Id.ToString());
      eventData.Add("PolicyTypeName", (object) policyTypeData?.DisplayName);
      eventData.Add("IsBlocking", (object) isBlocking);
      eventData.Add("IsEnabled", (object) configuration.IsEnabled);
      policyTypeData?.AppendClientTraceData(settingsObj, ref eventData);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Policy.Server", "PolicyConfiguration", eventData);
    }

    internal static IEnumerable<PolicyEvaluationRecord> FilterOutAllButLatestRecords(
      IEnumerable<PolicyEvaluationRecord> incoming)
    {
      Dictionary<int, PolicyEvaluationRecord> dictionary = new Dictionary<int, PolicyEvaluationRecord>();
      foreach (PolicyEvaluationRecord evaluationRecord1 in incoming)
      {
        PolicyEvaluationRecord evaluationRecord2;
        if (!dictionary.TryGetValue(evaluationRecord1.Configuration.Id, out evaluationRecord2) || evaluationRecord2.Configuration.Revision < evaluationRecord1.Configuration.Revision)
          dictionary[evaluationRecord1.Configuration.Id] = evaluationRecord1;
      }
      return (IEnumerable<PolicyEvaluationRecord>) dictionary.Values;
    }

    public IReadOnlyList<T> GetApplicablePolicies<T>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      out List<PolicyFailures> failedToInitialize,
      bool isBlockingOnly = false,
      bool includeHidden = false)
      where T : class, ITeamFoundationPolicy
    {
      using (requestContext.TraceBlock(1390064, 1390065, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        failedToInitialize = new List<PolicyFailures>();
        List<T> applicablePolicies = new List<T>();
        ActivePolicyEvaluationSet<T> policyEvaluationSet = ActivePolicyEvaluationSet<T>.LoadAllPoliciesForProject(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (r => this.DetermineScopes(requestContext, r)), target.TeamProjectId, target.Scopes, includeHidden);
        policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, target.TeamProjectId);
        policyEvaluationSet.CheckBypass(requestContext, target);
        policyEvaluationSet.CheckScopes(requestContext, target);
        foreach (ActivePolicyEvaluation<T> entry in policyEvaluationSet.Entries)
        {
          if (entry.IsApplicable && (!isBlockingOnly || !entry.IsSkippable && !entry.IsBypassable))
          {
            if (entry.IsBroken)
            {
              PolicyFailures policyFailures = new PolicyFailures(entry.CurrentConfiguration, entry.FailedToLoadReason);
              failedToInitialize.Add(policyFailures);
            }
            else if (entry.IsApplicable)
              applicablePolicies.Add(entry.Policy);
          }
        }
        return (IReadOnlyList<T>) applicablePolicies;
      }
    }

    public PolicyEvaluationRecord GetPolicyEvaluationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid policyEvaluationId,
      bool throwIfNotApplicable = true,
      bool throwIfNotFound = true)
    {
      using (requestContext.TraceBlock(1390108, 1390109, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckForEmptyGuid(policyEvaluationId, nameof (policyEvaluationId));
        PolicyEvaluationRecord record = (PolicyEvaluationRecord) null;
        using (IPolicyComponent policyComponent = this.CreatePolicyComponent(requestContext))
          record = policyComponent.GetPolicyEvaluationRecord(projectId, policyEvaluationId);
        if (record == null)
          return this.ThrowNotFoundOrReturnNull(throwIfNotFound, policyEvaluationId);
        record.Configuration.PopulatePartialWebApi(projectId, requestContext, (ISecuredObject) record);
        ArtifactId artifactId = LinkingUtilities.DecodeUri(record.ArtifactId);
        ITeamFoundationPolicyTarget targetForArtifact = this.GetTargetForArtifact(requestContext, artifactId, false);
        if (targetForArtifact != null && !targetForArtifact.HasReadPermissionInTarget(requestContext))
          return this.ThrowNotFoundOrReturnNull(throwIfNotFound, policyEvaluationId);
        if (this.m_allPolicyTemplatesCache.IsDynamicPolicy(record.Configuration.Type.Id) && targetForArtifact.ShouldDynamicEvaluatePolicies(requestContext))
        {
          requestContext.Trace(1390053, TraceLevel.Verbose, "Policy", "PolicyService", "Found the evaluation record with configuration ID {0} to be a dynamic policy of type {1}.", (object) record.Configuration.Id, (object) record.Configuration.Type.DisplayName);
          ActivePolicyEvaluationSet<IDynamicEvaluationPolicy> policyEvaluationSet = ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>.LoadSpecificPolicyForArtifact(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), projectId, artifactId, record.Configuration.Id);
          policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, projectId);
          bool notApplicable;
          policyEvaluationSet.UpdateDynamicPolicyEvaluationRecord(requestContext, targetForArtifact, record, out notApplicable);
          if (notApplicable & throwIfNotApplicable)
            throw new PolicyEvaluationNotFoundException(policyEvaluationId);
        }
        return record;
      }
    }

    private PolicyEvaluationRecord ThrowNotFoundOrReturnNull(
      bool throwIfNotFound,
      Guid policyEvaluationId)
    {
      if (throwIfNotFound)
        throw new PolicyEvaluationNotFoundException(policyEvaluationId);
      return (PolicyEvaluationRecord) null;
    }

    public void RequeuePolicyEvaluationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid policyEvaluationId)
    {
      using (requestContext.TraceBlock(1390110, 1390111, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        PolicyEvaluationRecord evaluationRecord = this.GetPolicyEvaluationRecord(requestContext, projectId, policyEvaluationId, false, true);
        ArtifactId artifactId = LinkingUtilities.DecodeUri(evaluationRecord.ArtifactId);
        ITeamFoundationPolicyTarget policyTarget = this.GetTargetForArtifact(requestContext, artifactId, true);
        this.NotifyPolicy<ITeamFoundationPolicy>(requestContext, policyTarget, artifactId, evaluationRecord.Configuration.Id, (Func<ITeamFoundationPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) => policy.Requeue(requestContext, existingStatus, existingContext, policyTarget)), (ClientTraceData) null);
      }
    }

    public IEnumerable<PolicyEvaluationRecord> GetPolicyEvaluationRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactId artifactId,
      bool includeNotApplicable = false,
      int? top = null,
      int? skip = null,
      IActivePolicyEvaluationCache policyEvaluationCache = null)
    {
      using (requestContext.TraceBlock(1390049, 1390050, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
        int topValue;
        int skipValue;
        TeamFoundationPolicyService.EvaluateTopSkip(top, skip, out topValue, out skipValue);
        ITeamFoundationPolicyTarget targetForArtifact = this.GetTargetForArtifact(requestContext, artifactId, false);
        if (targetForArtifact == null || !targetForArtifact.HasReadPermissionInTarget(requestContext))
          throw new ArtifactNotFoundException(artifactId);
        IEnumerable<PolicyEvaluationRecord> evaluationRecordsInternal = this.GetPolicyEvaluationRecordsInternal(requestContext, projectId, artifactId, targetForArtifact, includeNotApplicable, topValue, skipValue, policyEvaluationCache);
        if (!includeNotApplicable && requestContext.IsFeatureEnabled("Policy.EnableCachedPolicyABTesting"))
        {
          IEnumerable<PolicyEvaluationRecord> recordsInternal2 = this.GetPolicyEvaluationRecordsInternal2(requestContext, projectId, artifactId, targetForArtifact, includeNotApplicable, topValue, skipValue, policyEvaluationCache);
          new PolicyEvaluationABTestingUtil(requestContext).SafeTestResultsAndReportTrace(evaluationRecordsInternal, recordsInternal2, projectId, artifactId, targetForArtifact.GetType());
        }
        return evaluationRecordsInternal;
      }
    }

    private IEnumerable<PolicyEvaluationRecord> GetPolicyEvaluationRecordsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactId artifactId,
      ITeamFoundationPolicyTarget target,
      bool includeNotApplicable,
      int top,
      int skip,
      IActivePolicyEvaluationCache policyEvaluationCache = null)
    {
      TeamFoundationPolicyService foundationPolicyService = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (GetPolicyEvaluationRecordsInternal)))
      {
        IEnumerable<PolicyEvaluationRecord> incoming;
        using (IPolicyComponent policyComponent = foundationPolicyService.CreatePolicyComponent(requestContext))
        {
          using (VirtualResultCollection<PolicyEvaluationRecord> evaluationRecords = policyComponent.GetPolicyEvaluationRecords(projectId, new int?(), new int?(), artifactId, includeNotApplicable, top, skip))
          {
            incoming = evaluationRecords.GetCurrentAsEnumerable();
            incoming = TeamFoundationPolicyService.FilterOutAllButLatestRecords(incoming);
          }
        }
        ActivePolicyEvaluationSet<IDynamicEvaluationPolicy> policyEvaluationSet = (ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>) null;
        int countOfRecords = 0;
        foreach (PolicyEvaluationRecord record in incoming)
        {
          try
          {
            record.Configuration.PopulatePartialWebApi(projectId, requestContext, (ISecuredObject) record);
            requestContext.Trace(1390097, TraceLevel.Verbose, "Policy", "PolicyService", "Status={0}  Artifact ID={1}  Started Date={2}  Completed Date={3}  Config.ID={4}  Config.Type={5}  Config.Revision={6}", (object) record.Status, (object) record.ArtifactId, (object) record.StartedDate, (object) record.CompletedDate, (object) record.Configuration.Id, (object) record.Configuration.Type, (object) record.Configuration.Revision);
            if (foundationPolicyService.m_allPolicyTemplatesCache.IsDynamicPolicy(record.Configuration.Type.Id) && target.ShouldDynamicEvaluatePolicies(requestContext))
            {
              requestContext.Trace(1390056, TraceLevel.Verbose, "Policy", "PolicyService", "Found the evaluation record with configuration ID {0} to be a dynamic policy of type {1}.", (object) record.Configuration.Id, (object) record.Configuration.Type.DisplayName);
              if (policyEvaluationSet == null)
              {
                policyEvaluationSet = ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>.LoadAllPoliciesForProject(requestContext, new Func<IVssRequestContext, IPolicyComponent>(foundationPolicyService.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (configRecord => this.DetermineScopes(requestContext, configRecord)), projectId, target.Scopes);
                policyEvaluationSet.InitializePolicies(requestContext, foundationPolicyService.m_allPolicyTemplatesCache, projectId, policyEvaluationCache);
              }
              bool notApplicable;
              policyEvaluationSet.UpdateDynamicPolicyEvaluationRecord(requestContext, target, record, out notApplicable);
              if (notApplicable)
                continue;
            }
            ++countOfRecords;
          }
          catch (Exception ex)
          {
            requestContext.RequestTracer.TraceException(1390082, TraceLevel.Error, "Policy", "PolicyService", ex, "Error enumerating policy evaluation records. artifactId={0}, message={1}, stackTrace={2}", (object) artifactId, (object) ex.Message, (object) new StackTrace().ToString());
            record.Status = new PolicyEvaluationStatus?(PolicyEvaluationStatus.Broken);
          }
          yield return record;
        }
        requestContext.Trace(1390055, TraceLevel.Verbose, "Policy", "PolicyService", "Retrieved {0} policy record(s) for project ID '{1}' and artifact ID '{2}'.", (object) countOfRecords, (object) projectId, (object) artifactId);
        policyEvaluationSet = (ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>) null;
      }
    }

    private IEnumerable<PolicyEvaluationRecord> GetPolicyEvaluationRecordsInternal2(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactId artifactId,
      ITeamFoundationPolicyTarget target,
      bool includeNotApplicable,
      int top,
      int skip,
      IActivePolicyEvaluationCache policyEvaluationCache = null)
    {
      TeamFoundationPolicyService foundationPolicyService = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (GetPolicyEvaluationRecordsInternal2)))
      {
        IEnumerable<PolicyEvaluationRecord> allRecords;
        using (IPolicyComponent policyComponent = foundationPolicyService.CreatePolicyComponent(requestContext))
        {
          using (VirtualResultCollection<PolicyEvaluationRecord> evaluationRecords = policyComponent.GetPolicyEvaluationRecords(projectId, new int?(), new int?(), artifactId, includeNotApplicable, top, skip))
          {
            allRecords = evaluationRecords.GetCurrentAsEnumerable();
            allRecords = TeamFoundationPolicyService.FilterOutAllButLatestRecords(allRecords);
          }
        }
        ActivePolicyEvaluationSet<IDynamicEvaluationPolicy> policyEvaluationSet = (ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>) null;
        int countOfRecords = 0;
        foreach (PolicyEvaluationRecord record in allRecords)
        {
          try
          {
            record.Configuration.PopulatePartialWebApi(projectId, requestContext, (ISecuredObject) record);
            requestContext.Trace(1390097, TraceLevel.Verbose, "Policy", "PolicyService", "Status={0}  Artifact ID={1}  Started Date={2}  Completed Date={3}  Config.ID={4}  Config.Type={5}  Config.Revision={6}", (object) record.Status, (object) record.ArtifactId, (object) record.StartedDate, (object) record.CompletedDate, (object) record.Configuration.Id, (object) record.Configuration.Type, (object) record.Configuration.Revision);
            if (foundationPolicyService.m_allPolicyTemplatesCache.IsDynamicPolicy(record.Configuration.Type.Id) && target.ShouldDynamicEvaluatePolicies(requestContext))
            {
              requestContext.Trace(1390056, TraceLevel.Verbose, "Policy", "PolicyService", "Found the evaluation record with configuration ID {0} to be a dynamic policy of type {1}.", (object) record.Configuration.Id, (object) record.Configuration.Type.DisplayName);
              if (policyEvaluationSet == null)
              {
                policyEvaluationSet = ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>.LoadAllPoliciesForProject(requestContext, new Func<IVssRequestContext, IPolicyComponent>(foundationPolicyService.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (configRecord => this.DetermineScopes(requestContext, configRecord)), projectId, target.Scopes);
                policyEvaluationSet.InitializePolicies(requestContext, foundationPolicyService.m_allPolicyTemplatesCache, projectId, policyEvaluationCache);
                policyEvaluationSet.CheckScopes(requestContext, target);
                requestContext.TraceAlways(1390146, TraceLevel.Info, "Policy", "PolicyService", "GetPolicyEvaluationRecordsInternal2 - TotalPolicyEvaluationRecords: FromDB={0}, FromActivePolicyEvaluationSet={1}", (object) allRecords.Count<PolicyEvaluationRecord>(), (object) policyEvaluationSet.Entries.Count<ActivePolicyEvaluation<IDynamicEvaluationPolicy>>());
              }
              bool notApplicable;
              policyEvaluationSet.UpdateDynamicPolicyEvaluationRecordWithCaching(requestContext, target, record, out notApplicable);
              if (notApplicable)
                continue;
            }
            ++countOfRecords;
          }
          catch (Exception ex)
          {
            requestContext.RequestTracer.TraceException(1390082, TraceLevel.Error, "Policy", "PolicyService", ex, "Error enumerating policy evaluation records. artifactId={0}, message={1}, stackTrace={2}", (object) artifactId, (object) ex.Message, (object) new StackTrace().ToString());
            record.Status = new PolicyEvaluationStatus?(PolicyEvaluationStatus.Broken);
          }
          yield return record;
        }
        requestContext.Trace(1390055, TraceLevel.Verbose, "Policy", "PolicyService", "Retrieved {0} policy record(s) for project ID '{1}' and artifact ID '{2}'.", (object) countOfRecords, (object) projectId, (object) artifactId);
        allRecords = (IEnumerable<PolicyEvaluationRecord>) null;
        policyEvaluationSet = (ActivePolicyEvaluationSet<IDynamicEvaluationPolicy>) null;
      }
    }

    public void NotifyPoliciesOfNewArtifact<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      Func<TPolicy, PolicyNotificationResult> action)
      where TPolicy : class, ITeamFoundationPolicy
    {
      this.NotifyPolicies<TPolicy>(requestContext, target, artifactId, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, status, context) => action(policy)), (ClientTraceData) null);
    }

    public void NotifyPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult> action,
      ClientTraceData ctData = null)
      where TPolicy : class, ITeamFoundationPolicy
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (NotifyPolicies)))
      {
        using (requestContext.TraceBlock(1390070, 1390071, "Policy", "PolicyService", nameof (NotifyPolicies)))
        {
          ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
          ArgumentUtility.CheckForNull<ITeamFoundationPolicyTarget>(target, nameof (target));
          ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
          ArgumentUtility.CheckForNull<Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>>(action, nameof (action));
          requestContext.Trace(1390061, TraceLevel.Verbose, "Policy", "PolicyService", "Loading and check policies for artifact {0}", (object) artifactId.ToString());
          ActivePolicyEvaluationSet<TPolicy> policyEvaluationSet = ActivePolicyEvaluationSet<TPolicy>.LoadAllPoliciesForArtifact(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (r => this.DetermineScopes(requestContext, r)), target.TeamProjectId, artifactId, target.Scopes);
          policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, target.TeamProjectId);
          policyEvaluationSet.CheckBypass(requestContext, target);
          policyEvaluationSet.CheckScopes(requestContext, target);
          policyEvaluationSet.Notify(requestContext, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) action);
          policyEvaluationSet.SavePolicyEvaluationRecords(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), ctData);
        }
      }
    }

    public void NotifyPolicy<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      int policyConfigurationId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult> action,
      ClientTraceData ctData = null)
      where TPolicy : class, ITeamFoundationPolicy
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (NotifyPolicy)))
      {
        using (requestContext.TraceBlock(1390072, 1390075, "Policy", "PolicyService", "EvaluatePolicyOnExistingArtifact"))
        {
          ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
          ArgumentUtility.CheckForNull<ITeamFoundationPolicyTarget>(target, nameof (target));
          ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
          ArgumentUtility.CheckForNull<Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>>(action, nameof (action));
          ActivePolicyEvaluationSet<TPolicy> policyEvaluationSet = ActivePolicyEvaluationSet<TPolicy>.LoadSpecificPolicyForArtifact(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), target.TeamProjectId, artifactId, policyConfigurationId);
          policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, target.TeamProjectId);
          policyEvaluationSet.CheckBypass(requestContext, target);
          policyEvaluationSet.CheckScopes(requestContext, target);
          policyEvaluationSet.Notify(requestContext, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) action);
          policyEvaluationSet.SavePolicyEvaluationRecords(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), ctData);
        }
      }
    }

    public PolicyEvaluationTransaction<TPolicy> CheckPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      out PolicyEvaluationResult result,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult> action,
      IActivePolicyEvaluationCache policyEvaluationCacheService = null)
      where TPolicy : class, ITeamFoundationPolicy
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (CheckPolicies)))
      {
        using (requestContext.TraceBlock(1390073, 1390074, "Policy", "PolicyService", "CheckPolicyOnExistingArtifact"))
        {
          ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
          ArgumentUtility.CheckForNull<ITeamFoundationPolicyTarget>(target, nameof (target));
          ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
          ArgumentUtility.CheckForNull<Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult>>(action, nameof (action));
          ActivePolicyEvaluationSet<TPolicy> policyEvaluationSet = ActivePolicyEvaluationSet<TPolicy>.LoadAllPoliciesForArtifact(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (r => this.DetermineScopes(requestContext, r)), target.TeamProjectId, artifactId, target.Scopes);
          artifactId.ToString();
          policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, target.TeamProjectId, policyEvaluationCacheService);
          policyEvaluationSet.CheckBypass(requestContext, target);
          policyEvaluationSet.CheckScopes(requestContext, target);
          result = policyEvaluationSet.Check(requestContext, action);
          return new PolicyEvaluationTransaction<TPolicy>(policyEvaluationSet, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent));
        }
      }
    }

    public PolicyEvaluationResult CheckPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      Func<TPolicy, PolicyCheckResult> action)
      where TPolicy : class, ITeamFoundationPolicy
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (CheckPolicies)))
      {
        using (requestContext.TraceBlock(1390076, 1390077, "Policy", "PolicyService", "CheckPolicyOnTarget"))
        {
          ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
          ArgumentUtility.CheckForNull<ITeamFoundationPolicyTarget>(target, nameof (target));
          ArgumentUtility.CheckForNull<Func<TPolicy, PolicyCheckResult>>(action, nameof (action));
          ActivePolicyEvaluationSet<TPolicy> policyEvaluationSet = ActivePolicyEvaluationSet<TPolicy>.LoadAllPoliciesForProject(requestContext, new Func<IVssRequestContext, IPolicyComponent>(this.CreatePolicyComponent), (Func<PolicyConfigurationRecord, string[]>) (r => this.DetermineScopes(requestContext, r)), target.TeamProjectId, target.Scopes);
          policyEvaluationSet.InitializePolicies(requestContext, this.m_allPolicyTemplatesCache, target.TeamProjectId);
          policyEvaluationSet.CheckBypass(requestContext, target);
          policyEvaluationSet.CheckScopes(requestContext, target);
          return policyEvaluationSet.Check(requestContext, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult>) ((policy, status, context) => action(policy)));
        }
      }
    }
  }
}
