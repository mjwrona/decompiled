// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationFeatureAvailabilityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.FeatureAvailability;
using Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.MessageBus;
using Microsoft.VisualStudio.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationFeatureAvailabilityService : 
    ITeamFoundationFeatureAvailabilityService,
    IVssFrameworkService
  {
    private readonly IFeatureAvailabilitySecurityManager m_securityManager;
    private Guid m_serviceHostId;
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal const string s_Area = "FeatureAvailabilityService";
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal const string s_Layer = "BusinessLogic";
    private const int s_MaxFeatureNameLength = 100;
    private const int s_MaxFeatureDescriptionLength = 2000;
    private const string c_featureAvailabilityDefinitionsPath = "/FeatureAvailability/Definitions";
    private const string c_featureAvailabilityEntriesPath = "/FeatureAvailability/Entries";
    private const string c_definitionsToken = "Definitions";
    private const string c_availabilityStateSlashedToken = "/AvailabilityState";
    private const string c_usersSlashedToken = "/Users";
    private const string c_customerStagesSlashedToken = "/CustomerStages";
    private const string c_nameSlashedToken = "/Name";
    private const string c_descriptionSlashedToken = "/Description";
    private const string c_ownerSlashedToken = "/Owner";

    public TeamFoundationFeatureAvailabilityService()
      : this((IFeatureAvailabilitySecurityManager) new FeatureAvailabilitySecurityManager())
    {
    }

    public TeamFoundationFeatureAvailabilityService(
      IFeatureAvailabilitySecurityManager securityManager)
    {
      this.m_securityManager = securityManager;
    }

    public void ServiceStart(IVssRequestContext requestContext) => this.m_serviceHostId = requestContext.ServiceHost.InstanceId;

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    internal void RegisterFeature(
      IVssRequestContext requestContext,
      string featureName,
      string featureDescription)
    {
      this.RegisterFeatures(requestContext, (IEnumerable<FeatureDefinition>) new FeatureDefinition[1]
      {
        new FeatureDefinition()
        {
          Name = featureName,
          Description = featureDescription
        }
      });
    }

    internal void RegisterFeatures(
      IVssRequestContext requestContext,
      IEnumerable<FeatureDefinition> featureDefinitions,
      bool throwIfExists = true)
    {
      requestContext.TraceEnter(93200, "FeatureAvailabilityService", "BusinessLogic", nameof (RegisterFeatures));
      try
      {
        requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.EditFeatureFlags, false);
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        foreach (FeatureDefinition featureDefinition in featureDefinitions)
        {
          TeamFoundationFeatureAvailabilityService.ValidateFeatureName(featureDefinition.Name);
          TeamFoundationFeatureAvailabilityService.ValidateFeatureDescription(featureDefinition.Description);
          if (TeamFoundationFeatureAvailabilityService.FeatureExists(requestContext, featureDefinition.Name))
          {
            if (throwIfExists)
              throw new DuplicateFeatureException(featureDefinition.Name);
          }
          else
          {
            registryEntryList.Add(new RegistryEntry(TeamFoundationFeatureAvailabilityService.GenerateFeatureDefinitionNameRegistryPath(featureDefinition.Name), featureDefinition.Name));
            if (!string.IsNullOrEmpty(featureDefinition.Description))
              registryEntryList.Add(new RegistryEntry(TeamFoundationFeatureAvailabilityService.GenerateFeatureDefinitionDescriptionRegistryPath(featureDefinition.Name), featureDefinition.Description));
          }
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IVssRegistryService>().WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      finally
      {
        requestContext.TraceLeave(93201, "FeatureAvailabilityService", "BusinessLogic", nameof (RegisterFeatures));
      }
    }

    internal void RegisterFeature(IVssRequestContext requestContext, string featureName) => this.RegisterFeature(requestContext, featureName, string.Empty);

    internal void UnregisterFeature(IVssRequestContext requestContext, string featureName) => this.UnregisterFeatures(requestContext, (IEnumerable<string>) new string[1]
    {
      featureName
    });

    internal void UnregisterFeatures(
      IVssRequestContext requestContext,
      IEnumerable<string> featureNames,
      bool throwIfNotFound = true)
    {
      requestContext.TraceEnter(93202, "FeatureAvailabilityService", "BusinessLogic", nameof (UnregisterFeatures));
      try
      {
        requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.EditFeatureFlags, false);
        List<string> stringList = new List<string>();
        foreach (string featureName in featureNames)
        {
          TeamFoundationFeatureAvailabilityService.ValidateFeatureName(featureName);
          if (!TeamFoundationFeatureAvailabilityService.FeatureExists(requestContext, featureName))
          {
            if (throwIfNotFound)
              throw new MissingFeatureException(featureName);
          }
          else
          {
            stringList.Add(TeamFoundationFeatureAvailabilityService.GenerateFeatureEntryRootRegistryPath(featureName) + "/...");
            stringList.Add(TeamFoundationFeatureAvailabilityService.GenerateFeatureDefinitionRootRegistryPath(featureName) + "/...");
          }
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IVssRegistryService>().DeleteEntries(vssRequestContext, stringList.ToArray());
      }
      finally
      {
        requestContext.TraceLeave(93203, "FeatureAvailabilityService", "BusinessLogic", nameof (UnregisterFeatures));
      }
    }

    public void SetFeatureState(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state,
      Guid? userId = null,
      bool checkFeatureExists = true)
    {
      requestContext.TraceEnter(93204, "FeatureAvailabilityService", "BusinessLogic", nameof (SetFeatureState));
      try
      {
        requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.EditFeatureFlags, userId.HasValue);
        if (checkFeatureExists && !TeamFoundationFeatureAvailabilityService.FeatureExists(requestContext, featureName))
          throw new MissingFeatureException(featureName);
        TeamFoundationFeatureAvailabilityService.ValidateFeatureName(featureName);
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
        string str = !userId.HasValue ? TeamFoundationFeatureAvailabilityService.GenerateServiceHostAvailabilityStateRegistryPath(featureName) : TeamFoundationFeatureAvailabilityService.GenerateUserAvailabilityStatesRegistryPath(userId.ToString(), featureName);
        if (state == FeatureAvailabilityState.Undefined)
          service.DeleteEntries(vssRequestContext, str);
        else
          service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
          {
            new RegistryEntry(str, ((int) state).ToString())
          });
        vssRequestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(vssRequestContext);
        if (!userId.HasValue && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.IsFeatureEnabled("VisualStudio.Services.FeatureAvailability.LogCollectionFeatureStatesToFCM"))
          TeamFoundationFeatureAvailabilityService.LogFeatureStateChangeRecord(requestContext, featureName, state);
        IVssServiceHost serviceHost = requestContext.To(TeamFoundationHostType.Application).ServiceHost;
        string message;
        if (requestContext.IsSystemContext)
          message = string.Format("System edited the Feature Flag '{1}' to '{2}' on the {3} host on behalf of user {0}", (object) requestContext.UserContext.Identifier, (object) featureName, (object) state, (object) serviceHost.Name);
        else
          message = string.Format("{0} on machine {1} edited the Feature Flag '{2}' to '{3}' on the {4} host", (object) requestContext.AuthenticatedUserName, (object) requestContext.RemoteIPAddress(), (object) featureName, (object) state, (object) serviceHost.Name);
        if (serviceHost.Is(TeamFoundationHostType.Deployment))
          TeamFoundationEventLog.Default.Log(requestContext, message, TeamFoundationEventId.FeatureFlagStateChangeInformation, EventLogEntryType.Information);
        requestContext.Trace(1011031, TraceLevel.Info, "FeatureAvailabilityService", "BusinessLogic", message);
      }
      catch
      {
        if (!userId.HasValue && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.IsFeatureEnabled("VisualStudio.Services.FeatureAvailability.LogCollectionFeatureStatesToFCM"))
          TeamFoundationFeatureAvailabilityService.LogFailedFeatureStateChangeRecord(requestContext, featureName, state);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(93205, "FeatureAvailabilityService", "BusinessLogic", nameof (SetFeatureState));
      }
    }

    internal void SetFeatureStateForStages(
      IVssRequestContext requestContext,
      StageFeatureStates stageFeatureStates,
      string sequenceNumber)
    {
      requestContext.TraceEnter(93204, "FeatureAvailabilityService", "BusinessLogic", nameof (SetFeatureStateForStages));
      try
      {
        requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new InvalidRequestContextHostException(string.Format("Feature flags cannot be set at a stage level using host type {0}", (object) requestContext.ServiceHost.HostType));
        this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.EditFeatureFlags, false);
        IEnumerable<RegistryEntry> registryEntries = (IEnumerable<RegistryEntry>) new RegistryEntry[1]
        {
          new RegistryEntry(FeatureAvailabilityMessageBusConstants.LastProcessedMessageSequenceNumberRegistryPath, sequenceNumber)
        };
        foreach (KeyValuePair<string, List<FeatureFlagSetting>> keyValuePair in stageFeatureStates.FeatureFlagsByStage)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(keyValuePair.Key, "stageName");
          registryEntries = registryEntries.Concat<RegistryEntry>(TeamFoundationFeatureAvailabilityService.GenerateRegistryEntriesForStage(keyValuePair.Key, keyValuePair.Value));
        }
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<IVssRegistryService>().WriteEntries(vssRequestContext, registryEntries);
        string message;
        if (requestContext.IsSystemContext)
          message = "System edited the stage feature flags state to " + stageFeatureStates.ToString() + " on the " + requestContext.ServiceHost.Name + " host on behalf of user " + requestContext.UserContext.Identifier;
        else
          message = requestContext.AuthenticatedUserName + " on machine " + requestContext.RemoteIPAddress() + " edited the stage feature flags state to " + stageFeatureStates.ToString();
        TeamFoundationEventLog.Default.Log(requestContext, message, TeamFoundationEventId.FeatureFlagStateChangeInformation, EventLogEntryType.Information);
        requestContext.Trace(1011031, TraceLevel.Info, "FeatureAvailabilityService", "BusinessLogic", message);
      }
      finally
      {
        requestContext.TraceLeave(93205, "FeatureAvailabilityService", "BusinessLogic", nameof (SetFeatureStateForStages));
      }
    }

    public virtual bool IsFeatureEnabled(IVssRequestContext requestContext, string featureName)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.ViewFeatureFlagByName, false);
      FeatureAvailabilityInformation state;
      if (requestContext.TryGetCachedFeatureAvailability(featureName, out state))
        return state.EffectiveState == FeatureAvailabilityState.On;
      TeamFoundationFeatureAvailabilityService.ValidateFeatureName(featureName);
      return this.GetFeatureInformationInternal(requestContext, new FeatureDefinition()
      {
        Name = featureName,
        Description = string.Empty
      }).EffectiveState == FeatureAvailabilityState.On;
    }

    public IEnumerable<FeatureAvailabilityInformation> GetFeatureInformation(
      IVssRequestContext requestContext)
    {
      return this.GetFeatureInformation(requestContext, new Guid?());
    }

    public IEnumerable<FeatureAvailabilityInformation> GetFeatureInformation(
      IVssRequestContext requestContext,
      Guid? userId)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.ViewAllFeatureFlags, userId.HasValue);
      return TeamFoundationFeatureAvailabilityService.GetFeatureDefinitions(requestContext).Select<FeatureDefinition, FeatureAvailabilityInformation>((Func<FeatureDefinition, FeatureAvailabilityInformation>) (definition => this.GetFeatureInformationInternal(requestContext, definition, userId)));
    }

    public FeatureAvailabilityInformation GetFeatureInformation(
      IVssRequestContext requestContext,
      string featureName,
      bool checkFeatureExists = true)
    {
      return this.GetFeatureInformation(requestContext, featureName, new Guid?(), checkFeatureExists);
    }

    public FeatureAvailabilityInformation GetFeatureInformation(
      IVssRequestContext requestContext,
      string featureName,
      Guid? userId,
      bool checkFeatureExists = true)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      this.m_securityManager.CheckPermissions(requestContext, FeatureAvailabilityPermissions.ViewFeatureFlagByName, userId.HasValue);
      TeamFoundationFeatureAvailabilityService.ValidateFeatureName(featureName);
      if (!TeamFoundationFeatureAvailabilityService.FeatureExists(requestContext, featureName) & checkFeatureExists)
        throw new MissingFeatureException(featureName);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string registryValue = TeamFoundationFeatureAvailabilityService.GetRegistryValue(vssRequestContext, vssRequestContext.GetService<IVssRegistryService>(), TeamFoundationFeatureAvailabilityService.GenerateFeatureDefinitionDescriptionRegistryPath(featureName));
      return this.GetFeatureInformationInternal(requestContext, new FeatureDefinition()
      {
        Name = featureName,
        Description = registryValue ?? string.Empty
      }, userId);
    }

    internal static IEnumerable<RegistryEntry> GenerateRegistryEntriesForStage(
      string stage,
      List<FeatureFlagSetting> featureFlags)
    {
      return featureFlags.Select<FeatureFlagSetting, RegistryEntry>((Func<FeatureFlagSetting, RegistryEntry>) (featureFlag => new RegistryEntry(TeamFoundationFeatureAvailabilityService.GenerateCustomerStageAvailabilityStatesRegistryPath(stage, featureFlag.FeatureFlagName), featureFlag.IsRemoved ? string.Empty : ((int) featureFlag.PreferredState).ToString())));
    }

    internal static string GenerateFeatureDefinitionsRootRegistryPath() => "/FeatureAvailability/Definitions";

    internal static string GenerateFeatureDefinitionNameRegistryPath(string featureName) => "/FeatureAvailability/Definitions" + "/" + featureName + "/Name";

    internal static string GenerateFeatureDefinitionDescriptionRegistryPath(string featureName) => "/FeatureAvailability/Definitions" + "/" + featureName + "/Description";

    internal static string GenerateFeatureDefinitionOwnerRegistryPath(string featureName) => "/FeatureAvailability/Definitions" + "/" + featureName + "/Owner";

    internal static string GenerateFeatureEntryRootRegistryPath(string featureName) => "/FeatureAvailability/Entries" + "/" + featureName;

    private static string GenerateFeatureDefinitionRootRegistryPath(string featureName) => "/FeatureAvailability/Definitions" + "/" + featureName;

    internal static string GenerateServiceHostAvailabilityStateRegistryPath(string featureName) => "/FeatureAvailability/Entries" + "/" + featureName + "/AvailabilityState";

    internal static string GenerateCustomerStageAvailabilityStatesRegistryPath(
      string stage,
      string featureName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}{2}/{3}{4}", (object) "/FeatureAvailability/Entries", (object) featureName, (object) "/CustomerStages", (object) stage, (object) "/AvailabilityState");
    }

    internal static string GenerateUserAvailabilityStatesRegistryPath(
      string userId,
      string featureName)
    {
      int num = userId.IndexOf('\\');
      if (num >= 0)
        userId = userId.Substring(num + 1);
      if (string.IsNullOrEmpty(userId))
        return string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}{2}/{3}{4}", (object) "/FeatureAvailability/Entries", (object) featureName, (object) "/Users", (object) userId, (object) "/AvailabilityState");
    }

    private static void LogFeatureStateChangeRecord(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<IChangeRecordService>().LogCompletedChangeEvent(TeamFoundationFeatureAvailabilityService.GetChangeTitle(featureName, state), TeamFoundationFeatureAvailabilityService.GetChangeDescription(requestContext, featureName, state));
    }

    private static void LogFailedFeatureStateChangeRecord(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state)
    {
      requestContext.GetService<IChangeRecordService>().LogFailedChangeEvent(TeamFoundationFeatureAvailabilityService.GetChangeTitle(featureName, state), TeamFoundationFeatureAvailabilityService.GetChangeDescription(requestContext, featureName, state));
    }

    private static string GetChangeTitle(string featureName, FeatureAvailabilityState state) => string.Format("Feature Flag - {0} : {1}", (object) featureName, (object) state);

    private static string GetChangeDescription(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state)
    {
      string str = requestContext.IsSystemContext ? requestContext.UserContext.Identifier : requestContext.AuthenticatedUserName;
      return string.Format("Set feature flag {0} to {1}, as requested by {2}", (object) featureName, (object) state, (object) str);
    }

    private FeatureAvailabilityInformation GetFeatureInformationInternal(
      IVssRequestContext requestContext,
      FeatureDefinition featureDefinition,
      Guid? userId = null)
    {
      FeatureAvailabilityInformation state;
      if (requestContext.TryGetCachedFeatureAvailability(featureDefinition.Name, out state))
        return state;
      FeatureAvailabilityInformation informationInternal = new FeatureAvailabilityInformation()
      {
        Name = featureDefinition.Name,
        Description = featureDefinition.Description,
        EffectiveState = FeatureAvailabilityState.Undefined
      };
      bool explicitStateSet = false;
      if (userId.HasValue)
      {
        TeamFoundationFeatureAvailabilityService.FillFeatureAvailabilityInformationByUser(requestContext, informationInternal, userId);
        explicitStateSet = true;
      }
      TeamFoundationFeatureAvailabilityService.FillFeatureAvailabilityInformationByServiceHosts(requestContext, new RegistryQuery(TeamFoundationFeatureAvailabilityService.GenerateServiceHostAvailabilityStateRegistryPath(featureDefinition.Name)), informationInternal, explicitStateSet);
      if (informationInternal.EffectiveState != FeatureAvailabilityState.Off)
        TeamFoundationFeatureAvailabilityService.FillFeatureAvailabilityInformationByCustomerStage(requestContext, informationInternal);
      if (informationInternal.EffectiveState == FeatureAvailabilityState.Undefined)
        informationInternal.EffectiveState = FeatureAvailabilityState.Off;
      requestContext.CacheFeatureAvailability(featureDefinition.Name, informationInternal);
      return informationInternal;
    }

    private static void FillFeatureAvailabilityInformationByCustomerStage(
      IVssRequestContext requestContext,
      FeatureAvailabilityInformation information)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (vssRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !vssRequestContext.IsFeatureEnabled("VisualStudio.Services.FeatureAvailability.FirstClassCustomerStages"))
        return;
      string customerStage = requestContext.GetService<ICustomerStageService>().GetCustomerStage(requestContext);
      if (string.IsNullOrWhiteSpace(customerStage))
        return;
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery(TeamFoundationFeatureAvailabilityService.GenerateCustomerStageAvailabilityStatesRegistryPath(customerStage, information.Name));
      IVssRequestContext requestContext1 = vssRequestContext;
      ref RegistryQuery local = ref registryQuery;
      FeatureAvailabilityState state = TeamFoundationFeatureAvailabilityService.FeatureAvailabilityStateFromString(service.Read(requestContext1, in local).GetSingleValue());
      TeamFoundationFeatureAvailabilityService.PreserveDefinedAvailabilityState(information, state);
    }

    private static void FillFeatureAvailabilityInformationByServiceHosts(
      IVssRequestContext requestContext,
      RegistryQuery registryQuery,
      FeatureAvailabilityInformation information,
      bool explicitStateSet = false)
    {
      FeatureAvailabilityState state = TeamFoundationFeatureAvailabilityService.FeatureAvailabilityStateFromString(requestContext.GetService<IVssRegistryService>().Read(requestContext, in registryQuery).FirstOrDefault<RegistryItem>().Value);
      if (!explicitStateSet)
        information.ExplicitState = state;
      TeamFoundationFeatureAvailabilityService.PreserveDefinedAvailabilityState(information, state);
      if (state == FeatureAvailabilityState.Off)
        return;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Parent);
      if (requestContext1 == null)
        return;
      TeamFoundationFeatureAvailabilityService.FillFeatureAvailabilityInformationByServiceHosts(requestContext1, registryQuery, information, true);
    }

    internal static FeatureAvailabilityState FeatureAvailabilityStateFromString(string str)
    {
      FeatureAvailabilityState availabilityState = FeatureAvailabilityState.Undefined;
      switch (str)
      {
        case "1":
          availabilityState = FeatureAvailabilityState.On;
          break;
        case "0":
          availabilityState = FeatureAvailabilityState.Off;
          break;
      }
      return availabilityState;
    }

    private static void FillFeatureAvailabilityInformationByUser(
      IVssRequestContext requestContext,
      FeatureAvailabilityInformation information,
      Guid? userId,
      bool fallThru = true)
    {
      Guid userId1 = requestContext.GetUserId();
      if (userId1 == Guid.Empty && !userId.HasValue)
        return;
      Guid guid = userId ?? userId1;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string registryValue = TeamFoundationFeatureAvailabilityService.GetRegistryValue(requestContext, service, TeamFoundationFeatureAvailabilityService.GenerateUserAvailabilityStatesRegistryPath(guid.ToString(), information.Name), fallThru);
      if (userId1 == Guid.Empty)
        return;
      if (string.IsNullOrEmpty(registryValue) && requestContext.UserContext != (IdentityDescriptor) null)
      {
        string identifier = requestContext.UserContext.Identifier;
        if (!string.IsNullOrWhiteSpace(identifier))
          registryValue = TeamFoundationFeatureAvailabilityService.GetRegistryValue(requestContext, service, TeamFoundationFeatureAvailabilityService.GenerateUserAvailabilityStatesRegistryPath(identifier, information.Name));
      }
      FeatureAvailabilityState result;
      if (string.IsNullOrEmpty(registryValue) || !Enum.TryParse<FeatureAvailabilityState>(registryValue, out result))
        return;
      TeamFoundationFeatureAvailabilityService.PreserveDefinedAvailabilityState(information, result);
      information.ExplicitState = result;
    }

    private static void PreserveDefinedAvailabilityState(
      FeatureAvailabilityInformation information,
      FeatureAvailabilityState state)
    {
      if (state == FeatureAvailabilityState.Undefined || state == information.EffectiveState)
        return;
      information.EffectiveState = state;
    }

    private static IEnumerable<FeatureDefinition> GetFeatureDefinitions(
      IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService registryService = deploymentRequestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection source = registryService.ReadEntries(deploymentRequestContext, (RegistryQuery) ("/FeatureAvailability/Definitions" + "/*"), true);
      return source == null ? Enumerable.Empty<FeatureDefinition>() : source.Where<RegistryEntry>((Func<RegistryEntry, bool>) (entry => !VssStringComparer.RegistryPath.Equals(entry.Name, "Definitions"))).Select<RegistryEntry, FeatureDefinition>((Func<RegistryEntry, FeatureDefinition>) (entry =>
      {
        string registryValue = TeamFoundationFeatureAvailabilityService.GetRegistryValue(deploymentRequestContext, registryService, TeamFoundationFeatureAvailabilityService.GenerateFeatureDefinitionDescriptionRegistryPath(entry.Name));
        return new FeatureDefinition()
        {
          Name = entry.Name,
          Description = registryValue ?? string.Empty
        };
      }));
    }

    public static bool FeatureExists(IVssRequestContext requestContext, string featureName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().ReadEntries(vssRequestContext, (RegistryQuery) ("/FeatureAvailability/Definitions" + "/" + featureName + "/Name"), true).Count > 0;
    }

    internal static void ValidateFeatureName(string featureName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureName, nameof (featureName));
      if (featureName.Length > 100)
        throw new InvalidFeatureNameException();
      foreach (char c in featureName)
      {
        if (!char.IsLetterOrDigit(c) && c != '.')
          throw new InvalidFeatureNameException();
      }
    }

    private static void ValidateFeatureDescription(string description)
    {
      if (!string.IsNullOrEmpty(description) && description.Length > 2000)
        throw new InvalidFeatureDescriptionException();
    }

    private static string GetRegistryValue(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      string path,
      bool fallThru = false)
    {
      string registryValue = (string) null;
      try
      {
        registryValue = registryService.GetValue(requestContext, (RegistryQuery) path, fallThru, (string) null);
      }
      catch (RegistryPathException ex)
      {
      }
      return registryValue;
    }
  }
}
