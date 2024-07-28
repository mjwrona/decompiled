// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (TaskResources), typeof (TaskResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TaskResources.s_resMgr;

    private static string Get(string resourceName) => TaskResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TaskResources.Get(resourceName) : TaskResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TaskResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TaskResources.GetInt(resourceName) : (int) TaskResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TaskResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TaskResources.GetBool(resourceName) : (bool) TaskResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TaskResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TaskResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string AccessDenied(object arg0, object arg1) => TaskResources.Format(nameof (AccessDenied), arg0, arg1);

    public static string AccessDenied(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AccessDenied), culture, arg0, arg1);

    public static string AccessDeniedForPool(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AccessDeniedForPool), arg0, arg1, arg2);

    public static string AccessDeniedForPool(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AccessDeniedForPool), culture, arg0, arg1, arg2);
    }

    public static string AdministerPermissions() => TaskResources.Get(nameof (AdministerPermissions));

    public static string AdministerPermissions(CultureInfo culture) => TaskResources.Get(nameof (AdministerPermissions), culture);

    public static string AgentRequestNotFound(object arg0) => TaskResources.Format(nameof (AgentRequestNotFound), arg0);

    public static string AgentRequestNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentRequestNotFound), culture, arg0);

    public static string AgentRequestExpired(object arg0) => TaskResources.Format(nameof (AgentRequestExpired), arg0);

    public static string AgentRequestExpired(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentRequestExpired), culture, arg0);

    public static string AgentNotFound(object arg0, object arg1) => TaskResources.Format(nameof (AgentNotFound), arg0, arg1);

    public static string AgentNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentNotFound), culture, arg0, arg1);

    public static string AgentNotFoundMatchingDemandPair(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (AgentNotFoundMatchingDemandPair), arg0, arg1, arg2, arg3);
    }

    public static string AgentNotFoundMatchingDemandPair(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundMatchingDemandPair), culture, arg0, arg1, arg2, arg3);
    }

    public static string AgentNotFoundMatchingDemands(object arg0, object arg1) => TaskResources.Format(nameof (AgentNotFoundMatchingDemands), arg0, arg1);

    public static string AgentNotFoundMatchingDemands(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundMatchingDemands), culture, arg0, arg1);
    }

    public static string AgentNotFoundAutoUpdateDisabled(object arg0, object arg1) => TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabled), arg0, arg1);

    public static string AgentNotFoundAutoUpdateDisabled(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabled), culture, arg0, arg1);
    }

    public static string AgentNotFoundAutoUpdateDisabledTask(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabledTask), arg0, arg1, arg2, arg3);
    }

    public static string AgentNotFoundAutoUpdateDisabledTask(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabledTask), culture, arg0, arg1, arg2, arg3);
    }

    public static string AgentNotFoundAutoUpdateDisabledFeature(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabledFeature), arg0, arg1, arg2);
    }

    public static string AgentNotFoundAutoUpdateDisabledFeature(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundAutoUpdateDisabledFeature), culture, arg0, arg1, arg2);
    }

    public static string AgentNotFoundMatchingSingleDemand(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AgentNotFoundMatchingSingleDemand), arg0, arg1, arg2);

    public static string AgentNotFoundMatchingSingleDemand(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentNotFoundMatchingSingleDemand), culture, arg0, arg1, arg2);
    }

    public static string AgentNotFoundPoolEmpty(object arg0) => TaskResources.Format(nameof (AgentNotFoundPoolEmpty), arg0);

    public static string AgentNotFoundPoolEmpty(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentNotFoundPoolEmpty), culture, arg0);

    public static string AgentPoolNotFound(object arg0) => TaskResources.Format(nameof (AgentPoolNotFound), arg0);

    public static string AgentPoolNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentPoolNotFound), culture, arg0);

    public static string AgentPoolNotEnoughPermissions() => TaskResources.Get(nameof (AgentPoolNotEnoughPermissions));

    public static string AgentPoolNotEnoughPermissions(CultureInfo culture) => TaskResources.Get(nameof (AgentPoolNotEnoughPermissions), culture);

    public static string AgentSessionConflict(object arg0, object arg1) => TaskResources.Format(nameof (AgentSessionConflict), arg0, arg1);

    public static string AgentSessionConflict(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentSessionConflict), culture, arg0, arg1);

    public static string AgentSessionExpired(object arg0) => TaskResources.Format(nameof (AgentSessionExpired), arg0);

    public static string AgentSessionExpired(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentSessionExpired), culture, arg0);

    public static string EditAgentPool() => TaskResources.Get(nameof (EditAgentPool));

    public static string EditAgentPool(CultureInfo culture) => TaskResources.Get(nameof (EditAgentPool), culture);

    public static string Listen() => TaskResources.Get(nameof (Listen));

    public static string Listen(CultureInfo culture) => TaskResources.Get(nameof (Listen), culture);

    public static string Manage() => TaskResources.Get(nameof (Manage));

    public static string Manage(CultureInfo culture) => TaskResources.Get(nameof (Manage), culture);

    public static string PoolAdministratorsGroupDescription() => TaskResources.Get(nameof (PoolAdministratorsGroupDescription));

    public static string PoolAdministratorsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolAdministratorsGroupDescription), culture);

    public static string PoolNamespaceAdministratorsGroupDescription() => TaskResources.Get(nameof (PoolNamespaceAdministratorsGroupDescription));

    public static string PoolNamespaceAdministratorsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolNamespaceAdministratorsGroupDescription), culture);

    public static string PoolAdministratorsGroupName() => TaskResources.Get(nameof (PoolAdministratorsGroupName));

    public static string PoolAdministratorsGroupName(CultureInfo culture) => TaskResources.Get(nameof (PoolAdministratorsGroupName), culture);

    public static string PoolServiceAccountsGroupDescription() => TaskResources.Get(nameof (PoolServiceAccountsGroupDescription));

    public static string PoolServiceAccountsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolServiceAccountsGroupDescription), culture);

    public static string PoolServiceAccountsGroupName() => TaskResources.Get(nameof (PoolServiceAccountsGroupName));

    public static string PoolServiceAccountsGroupName(CultureInfo culture) => TaskResources.Get(nameof (PoolServiceAccountsGroupName), culture);

    public static string TaskDefinitionAlreadyUploaded(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TaskDefinitionAlreadyUploaded), arg0, arg1, arg2);

    public static string TaskDefinitionAlreadyUploaded(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionAlreadyUploaded), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionNotFound(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionNotFound), arg0, arg1);

    public static string TaskDefinitionNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TaskDefinitionNotFound), culture, arg0, arg1);

    public static string TaskDefinitionDuplicateFound(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionDuplicateFound), arg0, arg1);

    public static string TaskDefinitionDuplicateFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionDuplicateFound), culture, arg0, arg1);
    }

    public static string View() => TaskResources.Get(nameof (View));

    public static string View(CultureInfo culture) => TaskResources.Get(nameof (View), culture);

    public static string ViewSecrets() => TaskResources.Get(nameof (ViewSecrets));

    public static string ViewSecrets(CultureInfo culture) => TaskResources.Get(nameof (ViewSecrets), culture);

    public static string AgentPoolExists(object arg0) => TaskResources.Format(nameof (AgentPoolExists), arg0);

    public static string AgentPoolExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentPoolExists), culture, arg0);

    public static string InvalidOperationForHostedAgentPool(object arg0) => TaskResources.Format(nameof (InvalidOperationForHostedAgentPool), arg0);

    public static string InvalidOperationForHostedAgentPool(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidOperationForHostedAgentPool), culture, arg0);

    public static string TaskDefinitionCannotBeReplaced(object arg0) => TaskResources.Format(nameof (TaskDefinitionCannotBeReplaced), arg0);

    public static string TaskDefinitionCannotBeReplaced(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskDefinitionCannotBeReplaced), culture, arg0);

    public static string AgentExists(object arg0, object arg1) => TaskResources.Format(nameof (AgentExists), arg0, arg1);

    public static string AgentExists(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentExists), culture, arg0, arg1);

    public static string PackageNotFound(object arg0) => TaskResources.Format(nameof (PackageNotFound), arg0);

    public static string PackageNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PackageNotFound), culture, arg0);

    public static string PackagePropertyUnknown(object arg0) => TaskResources.Format(nameof (PackagePropertyUnknown), arg0);

    public static string PackagePropertyUnknown(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PackagePropertyUnknown), culture, arg0);

    public static string InvalidDemand(object arg0) => TaskResources.Format(nameof (InvalidDemand), arg0);

    public static string InvalidDemand(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidDemand), culture, arg0);

    public static string NoHostedParallelismAvailable() => TaskResources.Get(nameof (NoHostedParallelismAvailable));

    public static string NoHostedParallelismAvailable(CultureInfo culture) => TaskResources.Get(nameof (NoHostedParallelismAvailable), culture);

    public static string PoolIdentityScopeName(object arg0) => TaskResources.Format(nameof (PoolIdentityScopeName), arg0);

    public static string PoolIdentityScopeName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PoolIdentityScopeName), culture, arg0);

    public static string PoolRootScopeName() => TaskResources.Get(nameof (PoolRootScopeName));

    public static string PoolRootScopeName(CultureInfo culture) => TaskResources.Get(nameof (PoolRootScopeName), culture);

    public static string PoolServicePrincipalsGroupDescription() => TaskResources.Get(nameof (PoolServicePrincipalsGroupDescription));

    public static string PoolServicePrincipalsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolServicePrincipalsGroupDescription), culture);

    public static string PoolServicePrincipalsGroupName() => TaskResources.Get(nameof (PoolServicePrincipalsGroupName));

    public static string PoolServicePrincipalsGroupName(CultureInfo culture) => TaskResources.Get(nameof (PoolServicePrincipalsGroupName), culture);

    public static string AgentPoolCannotBeAddedDuringUpgrade() => TaskResources.Get(nameof (AgentPoolCannotBeAddedDuringUpgrade));

    public static string AgentPoolCannotBeAddedDuringUpgrade(CultureInfo culture) => TaskResources.Get(nameof (AgentPoolCannotBeAddedDuringUpgrade), culture);

    public static string InvalidPoolName(object arg0) => TaskResources.Format(nameof (InvalidPoolName), arg0);

    public static string InvalidPoolName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidPoolName), culture, arg0);

    public static string InvalidAgentName(object arg0) => TaskResources.Format(nameof (InvalidAgentName), arg0);

    public static string InvalidAgentName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidAgentName), culture, arg0);

    public static string InvalidCapabilityName() => TaskResources.Get(nameof (InvalidCapabilityName));

    public static string InvalidCapabilityName(CultureInfo culture) => TaskResources.Get(nameof (InvalidCapabilityName), culture);

    public static string InvalidCapabilityNameWithValue(object arg0) => TaskResources.Format(nameof (InvalidCapabilityNameWithValue), arg0);

    public static string InvalidCapabilityNameWithValue(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidCapabilityNameWithValue), culture, arg0);

    public static string InvalidCapabilityValue(object arg0) => TaskResources.Format(nameof (InvalidCapabilityValue), arg0);

    public static string InvalidCapabilityValue(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidCapabilityValue), culture, arg0);

    public static string InvalidSessionOwnerName() => TaskResources.Get(nameof (InvalidSessionOwnerName));

    public static string InvalidSessionOwnerName(CultureInfo culture) => TaskResources.Get(nameof (InvalidSessionOwnerName), culture);

    public static string AgentStillRunningJob(object arg0, object arg1) => TaskResources.Format(nameof (AgentStillRunningJob), arg0, arg1);

    public static string AgentStillRunningJob(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentStillRunningJob), culture, arg0, arg1);

    public static string AgentPoolCannotBeDeletedDuringUpgrade() => TaskResources.Get(nameof (AgentPoolCannotBeDeletedDuringUpgrade));

    public static string AgentPoolCannotBeDeletedDuringUpgrade(CultureInfo culture) => TaskResources.Get(nameof (AgentPoolCannotBeDeletedDuringUpgrade), culture);

    public static string TaskDefinitionMinimumAgentVersionMismatch(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (TaskDefinitionMinimumAgentVersionMismatch), arg0, arg1, arg2);
    }

    public static string TaskDefinitionMinimumAgentVersionMismatch(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionMinimumAgentVersionMismatch), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionHostContextMismatch(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionHostContextMismatch), arg0, arg1);

    public static string TaskDefinitionHostContextMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionHostContextMismatch), culture, arg0, arg1);
    }

    public static string IllegalBuilderState() => TaskResources.Get(nameof (IllegalBuilderState));

    public static string IllegalBuilderState(CultureInfo culture) => TaskResources.Get(nameof (IllegalBuilderState), culture);

    public static string AccessDeniedForHostedPool() => TaskResources.Get(nameof (AccessDeniedForHostedPool));

    public static string AccessDeniedForHostedPool(CultureInfo culture) => TaskResources.Get(nameof (AccessDeniedForHostedPool), culture);

    public static string TaskDefinitionIdNotFound(object arg0) => TaskResources.Format(nameof (TaskDefinitionIdNotFound), arg0);

    public static string TaskDefinitionIdNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskDefinitionIdNotFound), culture, arg0);

    public static string UntrustedHost(object arg0) => TaskResources.Format(nameof (UntrustedHost), arg0);

    public static string UntrustedHost(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UntrustedHost), culture, arg0);

    public static string TaskDefinitionCouldNotBeDeserialized() => TaskResources.Get(nameof (TaskDefinitionCouldNotBeDeserialized));

    public static string TaskDefinitionCouldNotBeDeserialized(CultureInfo culture) => TaskResources.Get(nameof (TaskDefinitionCouldNotBeDeserialized), culture);

    public static string TaskDefinitionIdRouteParameterMismatch(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionIdRouteParameterMismatch), arg0, arg1);

    public static string TaskDefinitionIdRouteParameterMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionIdRouteParameterMismatch), culture, arg0, arg1);
    }

    public static string InvalidAgentVersion() => TaskResources.Get(nameof (InvalidAgentVersion));

    public static string InvalidAgentVersion(CultureInfo culture) => TaskResources.Get(nameof (InvalidAgentVersion), culture);

    public static string VirtualMachineIsGettingProvisioned() => TaskResources.Get(nameof (VirtualMachineIsGettingProvisioned));

    public static string VirtualMachineIsGettingProvisioned(CultureInfo culture) => TaskResources.Get(nameof (VirtualMachineIsGettingProvisioned), culture);

    public static string VirtualMachineInitializingSection() => TaskResources.Get(nameof (VirtualMachineInitializingSection));

    public static string VirtualMachineInitializingSection(CultureInfo culture) => TaskResources.Get(nameof (VirtualMachineInitializingSection), culture);

    public static string ViewEndpoint() => TaskResources.Get(nameof (ViewEndpoint));

    public static string ViewEndpoint(CultureInfo culture) => TaskResources.Get(nameof (ViewEndpoint), culture);

    public static string AdministerEndpoint() => TaskResources.Get(nameof (AdministerEndpoint));

    public static string AdministerEndpoint(CultureInfo culture) => TaskResources.Get(nameof (AdministerEndpoint), culture);

    public static string InvalidUrl(object arg0) => TaskResources.Format(nameof (InvalidUrl), arg0);

    public static string InvalidUrl(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidUrl), culture, arg0);

    public static string InvalidUrlLength(object arg0) => TaskResources.Format(nameof (InvalidUrlLength), arg0);

    public static string InvalidUrlLength(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidUrlLength), culture, arg0);

    public static string QueueExists(object arg0) => TaskResources.Format(nameof (QueueExists), arg0);

    public static string QueueExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (QueueExists), culture, arg0);

    public static string InvalidQueueName(object arg0) => TaskResources.Format(nameof (InvalidQueueName), arg0);

    public static string InvalidQueueName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidQueueName), culture, arg0);

    public static string ValidNameSuggestion(object arg0, object arg1) => TaskResources.Format(nameof (ValidNameSuggestion), arg0, arg1);

    public static string ValidNameSuggestion(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (ValidNameSuggestion), culture, arg0, arg1);

    public static string QueueIdentityScopeName(object arg0) => TaskResources.Format(nameof (QueueIdentityScopeName), arg0);

    public static string QueueIdentityScopeName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (QueueIdentityScopeName), culture, arg0);

    public static string QueueAdministratorsGroupName() => TaskResources.Get(nameof (QueueAdministratorsGroupName));

    public static string QueueAdministratorsGroupName(CultureInfo culture) => TaskResources.Get(nameof (QueueAdministratorsGroupName), culture);

    public static string QueueUsersGroupName() => TaskResources.Get(nameof (QueueUsersGroupName));

    public static string QueueUsersGroupName(CultureInfo culture) => TaskResources.Get(nameof (QueueUsersGroupName), culture);

    public static string QueueAdministratorsGroupDescription() => TaskResources.Get(nameof (QueueAdministratorsGroupDescription));

    public static string QueueAdministratorsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (QueueAdministratorsGroupDescription), culture);

    public static string QueueUsersGroupDescription() => TaskResources.Get(nameof (QueueUsersGroupDescription));

    public static string QueueUsersGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (QueueUsersGroupDescription), culture);

    public static string Use() => TaskResources.Get(nameof (Use));

    public static string Use(CultureInfo culture) => TaskResources.Get(nameof (Use), culture);

    public static string QueueNotFound(object arg0) => TaskResources.Format(nameof (QueueNotFound), arg0);

    public static string QueueNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (QueueNotFound), culture, arg0);

    public static string QueueRootScopeName() => TaskResources.Get(nameof (QueueRootScopeName));

    public static string QueueRootScopeName(CultureInfo culture) => TaskResources.Get(nameof (QueueRootScopeName), culture);

    public static string AccessDeniedForQueue(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AccessDeniedForQueue), arg0, arg1, arg2);

    public static string AccessDeniedForQueue(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AccessDeniedForQueue), culture, arg0, arg1, arg2);
    }

    public static string QueueCreatorsGroupDescription() => TaskResources.Get(nameof (QueueCreatorsGroupDescription));

    public static string QueueCreatorsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (QueueCreatorsGroupDescription), culture);

    public static string QueueCreatorsGroupName() => TaskResources.Get(nameof (QueueCreatorsGroupName));

    public static string QueueCreatorsGroupName(CultureInfo culture) => TaskResources.Get(nameof (QueueCreatorsGroupName), culture);

    public static string Create() => TaskResources.Get(nameof (Create));

    public static string Create(CultureInfo culture) => TaskResources.Get(nameof (Create), culture);

    public static string AccountDoesNotHaveUsageLeft(object arg0) => TaskResources.Format(nameof (AccountDoesNotHaveUsageLeft), arg0);

    public static string AccountDoesNotHaveUsageLeft(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AccountDoesNotHaveUsageLeft), culture, arg0);

    public static string CreateEndpoint() => TaskResources.Get(nameof (CreateEndpoint));

    public static string CreateEndpoint(CultureInfo culture) => TaskResources.Get(nameof (CreateEndpoint), culture);

    public static string ViewAuthorization() => TaskResources.Get(nameof (ViewAuthorization));

    public static string ViewAuthorization(CultureInfo culture) => TaskResources.Get(nameof (ViewAuthorization), culture);

    public static string TaskJsonNotFound(object arg0) => TaskResources.Format(nameof (TaskJsonNotFound), arg0);

    public static string TaskJsonNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskJsonNotFound), culture, arg0);

    public static string HostedJobRequestTimedOut(object arg0) => TaskResources.Format(nameof (HostedJobRequestTimedOut), arg0);

    public static string HostedJobRequestTimedOut(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HostedJobRequestTimedOut), culture, arg0);

    public static string MachineManagementRequestAssignmentFailed(object arg0) => TaskResources.Format(nameof (MachineManagementRequestAssignmentFailed), arg0);

    public static string MachineManagementRequestAssignmentFailed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MachineManagementRequestAssignmentFailed), culture, arg0);

    public static string MetaTaskDefinitionExists(object arg0) => TaskResources.Format(nameof (MetaTaskDefinitionExists), arg0);

    public static string MetaTaskDefinitionExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MetaTaskDefinitionExists), culture, arg0);

    public static string MetaTaskMissingChildTask(object arg0, object arg1) => TaskResources.Format(nameof (MetaTaskMissingChildTask), arg0, arg1);

    public static string MetaTaskMissingChildTask(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (MetaTaskMissingChildTask), culture, arg0, arg1);

    public static string MetaTaskDefinitionNotFound(object arg0) => TaskResources.Format(nameof (MetaTaskDefinitionNotFound), arg0);

    public static string MetaTaskDefinitionNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MetaTaskDefinitionNotFound), culture, arg0);

    public static string PackageExists(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TaskResources.Format(nameof (PackageExists), arg0, arg1, arg2, arg3, arg4);
    }

    public static string PackageExists(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (PackageExists), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string PackageNotFoundWithVersion(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (PackageNotFoundWithVersion), arg0, arg1, arg2);

    public static string PackageNotFoundWithVersion(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (PackageNotFoundWithVersion), culture, arg0, arg1, arg2);
    }

    public static string MissingPackageType() => TaskResources.Get(nameof (MissingPackageType));

    public static string MissingPackageType(CultureInfo culture) => TaskResources.Get(nameof (MissingPackageType), culture);

    public static string AdministratorRole() => TaskResources.Get(nameof (AdministratorRole));

    public static string AdministratorRole(CultureInfo culture) => TaskResources.Get(nameof (AdministratorRole), culture);

    public static string CreatorRole() => TaskResources.Get(nameof (CreatorRole));

    public static string CreatorRole(CultureInfo culture) => TaskResources.Get(nameof (CreatorRole), culture);

    public static string ReaderRole() => TaskResources.Get(nameof (ReaderRole));

    public static string ReaderRole(CultureInfo culture) => TaskResources.Get(nameof (ReaderRole), culture);

    public static string UserRole() => TaskResources.Get(nameof (UserRole));

    public static string UserRole(CultureInfo culture) => TaskResources.Get(nameof (UserRole), culture);

    public static string QueueAdministratorRoleDescription() => TaskResources.Get(nameof (QueueAdministratorRoleDescription));

    public static string QueueAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (QueueAdministratorRoleDescription), culture);

    public static string CreatorRoleDescription() => TaskResources.Get(nameof (CreatorRoleDescription));

    public static string CreatorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (CreatorRoleDescription), culture);

    public static string QueueReaderRoleDescription() => TaskResources.Get(nameof (QueueReaderRoleDescription));

    public static string QueueReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (QueueReaderRoleDescription), culture);

    public static string UserRoleDescription() => TaskResources.Get(nameof (UserRoleDescription));

    public static string UserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (UserRoleDescription), culture);

    public static string TaskDefinitionAlreadyUploadedForExtension(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (TaskDefinitionAlreadyUploadedForExtension), arg0, arg1, arg2, arg3);
    }

    public static string TaskDefinitionAlreadyUploadedForExtension(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionAlreadyUploadedForExtension), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidBuildContributionsTarget(object arg0, object arg1) => TaskResources.Format(nameof (InvalidBuildContributionsTarget), arg0, arg1);

    public static string InvalidBuildContributionsTarget(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidBuildContributionsTarget), culture, arg0, arg1);
    }

    public static string AdministerMetaTaskPermissions() => TaskResources.Get(nameof (AdministerMetaTaskPermissions));

    public static string AdministerMetaTaskPermissions(CultureInfo culture) => TaskResources.Get(nameof (AdministerMetaTaskPermissions), culture);

    public static string DeleteMetaTask() => TaskResources.Get(nameof (DeleteMetaTask));

    public static string DeleteMetaTask(CultureInfo culture) => TaskResources.Get(nameof (DeleteMetaTask), culture);

    public static string EditMetaTask() => TaskResources.Get(nameof (EditMetaTask));

    public static string EditMetaTask(CultureInfo culture) => TaskResources.Get(nameof (EditMetaTask), culture);

    public static string ProjectBuildAdminAccountName() => TaskResources.Get(nameof (ProjectBuildAdminAccountName));

    public static string ProjectBuildAdminAccountName(CultureInfo culture) => TaskResources.Get(nameof (ProjectBuildAdminAccountName), culture);

    public static string ProjectReleaseAdminAccountName() => TaskResources.Get(nameof (ProjectReleaseAdminAccountName));

    public static string ProjectReleaseAdminAccountName(CultureInfo culture) => TaskResources.Get(nameof (ProjectReleaseAdminAccountName), culture);

    public static string ProjectReleaseManagerGroupName() => TaskResources.Get(nameof (ProjectReleaseManagerGroupName));

    public static string ProjectReleaseManagerGroupName(CultureInfo culture) => TaskResources.Get(nameof (ProjectReleaseManagerGroupName), culture);

    public static string ProjectContributorsGroupName() => TaskResources.Get(nameof (ProjectContributorsGroupName));

    public static string ProjectContributorsGroupName(CultureInfo culture) => TaskResources.Get(nameof (ProjectContributorsGroupName), culture);

    public static string TfxCliTaskDefinitionCannotBeReplaced(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (TfxCliTaskDefinitionCannotBeReplaced), arg0, arg1, arg2);
    }

    public static string TfxCliTaskDefinitionCannotBeReplaced(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TfxCliTaskDefinitionCannotBeReplaced), culture, arg0, arg1, arg2);
    }

    public static string ServiceAccountRole() => TaskResources.Get(nameof (ServiceAccountRole));

    public static string ServiceAccountRole(CultureInfo culture) => TaskResources.Get(nameof (ServiceAccountRole), culture);

    public static string ServiceAccountRoleDescription() => TaskResources.Get(nameof (ServiceAccountRoleDescription));

    public static string ServiceAccountRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (ServiceAccountRoleDescription), culture);

    public static string DeploymentMachineGroupNotFound(object arg0) => TaskResources.Format(nameof (DeploymentMachineGroupNotFound), arg0);

    public static string DeploymentMachineGroupNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentMachineGroupNotFound), culture, arg0);

    public static string InvalidDeploymentMachineGroupName(object arg0) => TaskResources.Format(nameof (InvalidDeploymentMachineGroupName), arg0);

    public static string InvalidDeploymentMachineGroupName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidDeploymentMachineGroupName), culture, arg0);

    public static string DeploymentMachineGroupExists(object arg0) => TaskResources.Format(nameof (DeploymentMachineGroupExists), arg0);

    public static string DeploymentMachineGroupExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentMachineGroupExists), culture, arg0);

    public static string DeploymentPoolsCanNotAutoProvisionQueues() => TaskResources.Get(nameof (DeploymentPoolsCanNotAutoProvisionQueues));

    public static string DeploymentPoolsCanNotAutoProvisionQueues(CultureInfo culture) => TaskResources.Get(nameof (DeploymentPoolsCanNotAutoProvisionQueues), culture);

    public static string DeploymentPoolsCanNotBeHosted() => TaskResources.Get(nameof (DeploymentPoolsCanNotBeHosted));

    public static string DeploymentPoolsCanNotBeHosted(CultureInfo culture) => TaskResources.Get(nameof (DeploymentPoolsCanNotBeHosted), culture);

    public static string PoolAdministratorRoleDescription() => TaskResources.Get(nameof (PoolAdministratorRoleDescription));

    public static string PoolAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolAdministratorRoleDescription), culture);

    public static string PoolReaderRoleDescription() => TaskResources.Get(nameof (PoolReaderRoleDescription));

    public static string PoolReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (PoolReaderRoleDescription), culture);

    public static string QueuesShouldUseAutomationPools() => TaskResources.Get(nameof (QueuesShouldUseAutomationPools));

    public static string QueuesShouldUseAutomationPools(CultureInfo culture) => TaskResources.Get(nameof (QueuesShouldUseAutomationPools), culture);

    public static string AccessDeniedForMachineGroup(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AccessDeniedForMachineGroup), arg0, arg1, arg2);

    public static string AccessDeniedForMachineGroup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AccessDeniedForMachineGroup), culture, arg0, arg1, arg2);
    }

    public static string MetaTaskDefinitionRunsOnMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (MetaTaskDefinitionRunsOnMismatch), arg0, arg1, arg2, arg3);
    }

    public static string MetaTaskDefinitionRunsOnMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (MetaTaskDefinitionRunsOnMismatch), culture, arg0, arg1, arg2, arg3);
    }

    public static string GlobalMachineGroupAdministratorRoleDescription() => TaskResources.Get(nameof (GlobalMachineGroupAdministratorRoleDescription));

    public static string GlobalMachineGroupAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (GlobalMachineGroupAdministratorRoleDescription), culture);

    public static string GlobalMachineGroupReaderRoleDescription() => TaskResources.Get(nameof (GlobalMachineGroupReaderRoleDescription));

    public static string GlobalMachineGroupReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (GlobalMachineGroupReaderRoleDescription), culture);

    public static string GlobalMachineGroupUserRoleDescription() => TaskResources.Get(nameof (GlobalMachineGroupUserRoleDescription));

    public static string GlobalMachineGroupUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (GlobalMachineGroupUserRoleDescription), culture);

    public static string InvalidTaskDefinitionTypeForTask(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (InvalidTaskDefinitionTypeForTask), arg0, arg1, arg2);

    public static string InvalidTaskDefinitionTypeForTask(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidTaskDefinitionTypeForTask), culture, arg0, arg1, arg2);
    }

    public static string InvalidTaskDefinitionTypeForTaskGroup(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (InvalidTaskDefinitionTypeForTaskGroup), arg0, arg1, arg2);
    }

    public static string InvalidTaskDefinitionTypeForTaskGroup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidTaskDefinitionTypeForTaskGroup), culture, arg0, arg1, arg2);
    }

    public static string LibraryUserRoleDescription() => TaskResources.Get(nameof (LibraryUserRoleDescription));

    public static string LibraryUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (LibraryUserRoleDescription), culture);

    public static string LibraryAdministratorRoleDescription() => TaskResources.Get(nameof (LibraryAdministratorRoleDescription));

    public static string LibraryAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (LibraryAdministratorRoleDescription), culture);

    public static string LibraryReaderRoleDescription() => TaskResources.Get(nameof (LibraryReaderRoleDescription));

    public static string LibraryReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (LibraryReaderRoleDescription), culture);

    public static string VariableGroupExist(object arg0) => TaskResources.Format(nameof (VariableGroupExist), arg0);

    public static string VariableGroupExist(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VariableGroupExist), culture, arg0);

    public static string VariableGroupNotFound(object arg0) => TaskResources.Format(nameof (VariableGroupNotFound), arg0);

    public static string VariableGroupNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VariableGroupNotFound), culture, arg0);

    public static string SecureFileExists(object arg0) => TaskResources.Format(nameof (SecureFileExists), arg0);

    public static string SecureFileExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (SecureFileExists), culture, arg0);

    public static string SecureFileNotFound(object arg0) => TaskResources.Format(nameof (SecureFileNotFound), arg0);

    public static string SecureFileNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (SecureFileNotFound), culture, arg0);

    public static string SecureFileWithNoContentError() => TaskResources.Get(nameof (SecureFileWithNoContentError));

    public static string SecureFileWithNoContentError(CultureInfo culture) => TaskResources.Get(nameof (SecureFileWithNoContentError), culture);

    public static string SecureFileWithNoContentLengthError() => TaskResources.Get(nameof (SecureFileWithNoContentLengthError));

    public static string SecureFileWithNoContentLengthError(CultureInfo culture) => TaskResources.Get(nameof (SecureFileWithNoContentLengthError), culture);

    public static string ViewLibraryItem() => TaskResources.Get(nameof (ViewLibraryItem));

    public static string ViewLibraryItem(CultureInfo culture) => TaskResources.Get(nameof (ViewLibraryItem), culture);

    public static string AdministerLibraryItem() => TaskResources.Get(nameof (AdministerLibraryItem));

    public static string AdministerLibraryItem(CultureInfo culture) => TaskResources.Get(nameof (AdministerLibraryItem), culture);

    public static string CreateLibraryItem() => TaskResources.Get(nameof (CreateLibraryItem));

    public static string CreateLibraryItem(CultureInfo culture) => TaskResources.Get(nameof (CreateLibraryItem), culture);

    public static string ViewLibraryItemSecrets() => TaskResources.Get(nameof (ViewLibraryItemSecrets));

    public static string ViewLibraryItemSecrets(CultureInfo culture) => TaskResources.Get(nameof (ViewLibraryItemSecrets), culture);

    public static string UseLibraryItem() => TaskResources.Get(nameof (UseLibraryItem));

    public static string UseLibraryItem(CultureInfo culture) => TaskResources.Get(nameof (UseLibraryItem), culture);

    public static string InvalidVariableGroupName(object arg0) => TaskResources.Format(nameof (InvalidVariableGroupName), arg0);

    public static string InvalidVariableGroupName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidVariableGroupName), culture, arg0);

    public static string VariableGroupAccessDeniedForAdminOperation() => TaskResources.Get(nameof (VariableGroupAccessDeniedForAdminOperation));

    public static string VariableGroupAccessDeniedForAdminOperation(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupAccessDeniedForAdminOperation), culture);

    public static string VariableGroupEmptyError() => TaskResources.Get(nameof (VariableGroupEmptyError));

    public static string VariableGroupEmptyError(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupEmptyError), culture);

    public static string InvalidVariableGroup() => TaskResources.Get(nameof (InvalidVariableGroup));

    public static string InvalidVariableGroup(CultureInfo culture) => TaskResources.Get(nameof (InvalidVariableGroup), culture);

    public static string SecureFile() => TaskResources.Get(nameof (SecureFile));

    public static string SecureFile(CultureInfo culture) => TaskResources.Get(nameof (SecureFile), culture);

    public static string InvalidSecureFileName(object arg0) => TaskResources.Format(nameof (InvalidSecureFileName), arg0);

    public static string InvalidSecureFileName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidSecureFileName), culture, arg0);

    public static string SecureFileAccessDeniedForAdminOperation() => TaskResources.Get(nameof (SecureFileAccessDeniedForAdminOperation));

    public static string SecureFileAccessDeniedForAdminOperation(CultureInfo culture) => TaskResources.Get(nameof (SecureFileAccessDeniedForAdminOperation), culture);

    public static string TaskGroupRevisionAlreadyExists(object arg0, object arg1) => TaskResources.Format(nameof (TaskGroupRevisionAlreadyExists), arg0, arg1);

    public static string TaskGroupRevisionAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskGroupRevisionAlreadyExists), culture, arg0, arg1);
    }

    public static string TaskGroupAlreadyUpdated(object arg0) => TaskResources.Format(nameof (TaskGroupAlreadyUpdated), arg0);

    public static string TaskGroupAlreadyUpdated(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskGroupAlreadyUpdated), culture, arg0);

    public static string DuplicateTaskDefinitionInputKey(object arg0) => TaskResources.Format(nameof (DuplicateTaskDefinitionInputKey), arg0);

    public static string DuplicateTaskDefinitionInputKey(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DuplicateTaskDefinitionInputKey), culture, arg0);

    public static string MachineNotFound(object arg0, object arg1) => TaskResources.Format(nameof (MachineNotFound), arg0, arg1);

    public static string MachineNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (MachineNotFound), culture, arg0, arg1);

    public static string DuplicateKeyInVariableGroup(object arg0, object arg1) => TaskResources.Format(nameof (DuplicateKeyInVariableGroup), arg0, arg1);

    public static string DuplicateKeyInVariableGroup(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (DuplicateKeyInVariableGroup), culture, arg0, arg1);

    public static string TaskDefinitionAlreadyExists(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TaskDefinitionAlreadyExists), arg0, arg1, arg2);

    public static string TaskDefinitionAlreadyExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionAlreadyExists), culture, arg0, arg1, arg2);
    }

    public static string VariableGroup() => TaskResources.Get(nameof (VariableGroup));

    public static string VariableGroup(CultureInfo culture) => TaskResources.Get(nameof (VariableGroup), culture);

    public static string ContributionTaskIdsShouldMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TaskResources.Format(nameof (ContributionTaskIdsShouldMatch), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ContributionTaskIdsShouldMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (ContributionTaskIdsShouldMatch), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string FailedToObtainTokenForClient(object arg0) => TaskResources.Format(nameof (FailedToObtainTokenForClient), arg0);

    public static string FailedToObtainTokenForClient(object arg0, CultureInfo culture) => TaskResources.Format(nameof (FailedToObtainTokenForClient), culture, arg0);

    public static string MaintenanceDefinitionNotFound(object arg0, object arg1) => TaskResources.Format(nameof (MaintenanceDefinitionNotFound), arg0, arg1);

    public static string MaintenanceDefinitionNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (MaintenanceDefinitionNotFound), culture, arg0, arg1);
    }

    public static string MaintenanceJobNotFound(object arg0, object arg1) => TaskResources.Format(nameof (MaintenanceJobNotFound), arg0, arg1);

    public static string MaintenanceJobNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (MaintenanceJobNotFound), culture, arg0, arg1);

    public static string AgentPoolMaintenanceNotEnabled(object arg0) => TaskResources.Format(nameof (AgentPoolMaintenanceNotEnabled), arg0);

    public static string AgentPoolMaintenanceNotEnabled(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentPoolMaintenanceNotEnabled), culture, arg0);

    public static string AgentNotFoundNoOnlineEnable(object arg0) => TaskResources.Format(nameof (AgentNotFoundNoOnlineEnable), arg0);

    public static string AgentNotFoundNoOnlineEnable(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentNotFoundNoOnlineEnable), culture, arg0);

    public static string AgentPendingUpdateNotFound(object arg0, object arg1) => TaskResources.Format(nameof (AgentPendingUpdateNotFound), arg0, arg1);

    public static string AgentPendingUpdateNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentPendingUpdateNotFound), culture, arg0, arg1);

    public static string AgentPendingUpdateExists(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AgentPendingUpdateExists), arg0, arg1, arg2);

    public static string AgentPendingUpdateExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentPendingUpdateExists), culture, arg0, arg1, arg2);
    }

    public static string SendAgentUpdateMessage() => TaskResources.Get(nameof (SendAgentUpdateMessage));

    public static string SendAgentUpdateMessage(CultureInfo culture) => TaskResources.Get(nameof (SendAgentUpdateMessage), culture);

    public static string AgentNotResponseUpdateMessage(object arg0) => TaskResources.Format(nameof (AgentNotResponseUpdateMessage), arg0);

    public static string AgentNotResponseUpdateMessage(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentNotResponseUpdateMessage), culture, arg0);

    public static string DeploymentMachineNotFound(object arg0, object arg1) => TaskResources.Format(nameof (DeploymentMachineNotFound), arg0, arg1);

    public static string DeploymentMachineNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (DeploymentMachineNotFound), culture, arg0, arg1);

    public static string GlobalMachineGroupCreatorRoleDescription() => TaskResources.Get(nameof (GlobalMachineGroupCreatorRoleDescription));

    public static string GlobalMachineGroupCreatorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (GlobalMachineGroupCreatorRoleDescription), culture);

    public static string MachineGroupAdministratorRoleDescription() => TaskResources.Get(nameof (MachineGroupAdministratorRoleDescription));

    public static string MachineGroupAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (MachineGroupAdministratorRoleDescription), culture);

    public static string Issues() => TaskResources.Get(nameof (Issues));

    public static string Issues(CultureInfo culture) => TaskResources.Get(nameof (Issues), culture);

    public static string NoErrorAndWarning() => TaskResources.Get(nameof (NoErrorAndWarning));

    public static string NoErrorAndWarning(CultureInfo culture) => TaskResources.Get(nameof (NoErrorAndWarning), culture);

    public static string MachineGroupReaderRoleDescription() => TaskResources.Get(nameof (MachineGroupReaderRoleDescription));

    public static string MachineGroupReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (MachineGroupReaderRoleDescription), culture);

    public static string MachineGroupUserRoleDescription() => TaskResources.Get(nameof (MachineGroupUserRoleDescription));

    public static string MachineGroupUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (MachineGroupUserRoleDescription), culture);

    public static string InvalidScopeId(object arg0) => TaskResources.Format(nameof (InvalidScopeId), arg0);

    public static string InvalidScopeId(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidScopeId), culture, arg0);

    public static string TaskDefinitionDeserializationError(object arg0) => TaskResources.Format(nameof (TaskDefinitionDeserializationError), arg0);

    public static string TaskDefinitionDeserializationError(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskDefinitionDeserializationError), culture, arg0);

    public static string TaskDefinitionDeserializationErrorWithFolder(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionDeserializationErrorWithFolder), arg0, arg1);

    public static string TaskDefinitionDeserializationErrorWithFolder(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskDefinitionDeserializationErrorWithFolder), culture, arg0, arg1);
    }

    public static string UploadTaskDefinitionAccessDenied() => TaskResources.Get(nameof (UploadTaskDefinitionAccessDenied));

    public static string UploadTaskDefinitionAccessDenied(CultureInfo culture) => TaskResources.Get(nameof (UploadTaskDefinitionAccessDenied), culture);

    public static string UnableToAddDeploymentAgent() => TaskResources.Get(nameof (UnableToAddDeploymentAgent));

    public static string UnableToAddDeploymentAgent(CultureInfo culture) => TaskResources.Get(nameof (UnableToAddDeploymentAgent), culture);

    public static string UnableToRemoveDeploymentAgent(object arg0) => TaskResources.Format(nameof (UnableToRemoveDeploymentAgent), arg0);

    public static string UnableToRemoveDeploymentAgent(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UnableToRemoveDeploymentAgent), culture, arg0);

    public static string MetaTaskDefinitionVersionNotFound(object arg0, object arg1) => TaskResources.Format(nameof (MetaTaskDefinitionVersionNotFound), arg0, arg1);

    public static string MetaTaskDefinitionVersionNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (MetaTaskDefinitionVersionNotFound), culture, arg0, arg1);
    }

    public static string TaskGroupPreviewExists(object arg0) => TaskResources.Format(nameof (TaskGroupPreviewExists), arg0);

    public static string TaskGroupPreviewExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskGroupPreviewExists), culture, arg0);

    public static string TaskGroupDisabled(object arg0, object arg1) => TaskResources.Format(nameof (TaskGroupDisabled), arg0, arg1);

    public static string TaskGroupDisabled(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TaskGroupDisabled), culture, arg0, arg1);

    public static string TaskGroupCanOnlyEditLatestVersion(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TaskGroupCanOnlyEditLatestVersion), arg0, arg1, arg2);

    public static string TaskGroupCanOnlyEditLatestVersion(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskGroupCanOnlyEditLatestVersion), culture, arg0, arg1, arg2);
    }

    public static string TaskGroupDraftExists(object arg0) => TaskResources.Format(nameof (TaskGroupDraftExists), arg0);

    public static string TaskGroupDraftExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskGroupDraftExists), culture, arg0);

    public static string DisabledPriorVersionsComment(object arg0) => TaskResources.Format(nameof (DisabledPriorVersionsComment), arg0);

    public static string DisabledPriorVersionsComment(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DisabledPriorVersionsComment), culture, arg0);

    public static string UsingDeprecatedAgent(object arg0) => TaskResources.Format(nameof (UsingDeprecatedAgent), arg0);

    public static string UsingDeprecatedAgent(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UsingDeprecatedAgent), culture, arg0);

    public static string UsingDeprecatedAgents(object arg0) => TaskResources.Format(nameof (UsingDeprecatedAgents), arg0);

    public static string UsingDeprecatedAgents(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UsingDeprecatedAgents), culture, arg0);

    public static string OperationNotAllowedForNonServicePrincipal() => TaskResources.Get(nameof (OperationNotAllowedForNonServicePrincipal));

    public static string OperationNotAllowedForNonServicePrincipal(CultureInfo culture) => TaskResources.Get(nameof (OperationNotAllowedForNonServicePrincipal), culture);

    public static string DraftForDraftTaskGroupIsNotAllowed(object arg0) => TaskResources.Format(nameof (DraftForDraftTaskGroupIsNotAllowed), arg0);

    public static string DraftForDraftTaskGroupIsNotAllowed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DraftForDraftTaskGroupIsNotAllowed), culture, arg0);

    public static string InvalidParentDefinitionIdForDraftTaskGroup() => TaskResources.Get(nameof (InvalidParentDefinitionIdForDraftTaskGroup));

    public static string InvalidParentDefinitionIdForDraftTaskGroup(CultureInfo culture) => TaskResources.Get(nameof (InvalidParentDefinitionIdForDraftTaskGroup), culture);

    public static string ParentTaskGroupNotFound(object arg0) => TaskResources.Format(nameof (ParentTaskGroupNotFound), arg0);

    public static string ParentTaskGroupNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ParentTaskGroupNotFound), culture, arg0);

    public static string DeploymentMachineExists(object arg0, object arg1) => TaskResources.Format(nameof (DeploymentMachineExists), arg0, arg1);

    public static string DeploymentMachineExists(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (DeploymentMachineExists), culture, arg0, arg1);

    public static string DeploymentMachineIdDoesNotMatchWithAgentId(object arg0, object arg1) => TaskResources.Format(nameof (DeploymentMachineIdDoesNotMatchWithAgentId), arg0, arg1);

    public static string DeploymentMachineIdDoesNotMatchWithAgentId(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (DeploymentMachineIdDoesNotMatchWithAgentId), culture, arg0, arg1);
    }

    public static string InvalidTaskGroupInput() => TaskResources.Get(nameof (InvalidTaskGroupInput));

    public static string InvalidTaskGroupInput(CultureInfo culture) => TaskResources.Get(nameof (InvalidTaskGroupInput), culture);

    public static string UnableToPopulateAzureStackData() => TaskResources.Get(nameof (UnableToPopulateAzureStackData));

    public static string UnableToPopulateAzureStackData(CultureInfo culture) => TaskResources.Get(nameof (UnableToPopulateAzureStackData), culture);

    public static string RequestedMoreThanMaxSupport() => TaskResources.Get(nameof (RequestedMoreThanMaxSupport));

    public static string RequestedMoreThanMaxSupport(CultureInfo culture) => TaskResources.Get(nameof (RequestedMoreThanMaxSupport), culture);

    public static string TaskInputValidationInvalid(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TaskInputValidationInvalid), arg0, arg1, arg2);

    public static string TaskInputValidationInvalid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskInputValidationInvalid), culture, arg0, arg1, arg2);
    }

    public static string WrongIdSpecifiedForTaskGroupId(object arg0, object arg1) => TaskResources.Format(nameof (WrongIdSpecifiedForTaskGroupId), arg0, arg1);

    public static string WrongIdSpecifiedForTaskGroupId(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (WrongIdSpecifiedForTaskGroupId), culture, arg0, arg1);
    }

    public static string DeploymentGroupsShouldUseDeploymentPools() => TaskResources.Get(nameof (DeploymentGroupsShouldUseDeploymentPools));

    public static string DeploymentGroupsShouldUseDeploymentPools(CultureInfo culture) => TaskResources.Get(nameof (DeploymentGroupsShouldUseDeploymentPools), culture);

    public static string DeploymentPoolInUseSingleCountMessage(object arg0, object arg1) => TaskResources.Format(nameof (DeploymentPoolInUseSingleCountMessage), arg0, arg1);

    public static string DeploymentPoolInUseSingleCountMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (DeploymentPoolInUseSingleCountMessage), culture, arg0, arg1);
    }

    public static string PasswordText() => TaskResources.Get(nameof (PasswordText));

    public static string PasswordText(CultureInfo culture) => TaskResources.Get(nameof (PasswordText), culture);

    public static string GetDeploymentMachinesApiDeprecated() => TaskResources.Get(nameof (GetDeploymentMachinesApiDeprecated));

    public static string GetDeploymentMachinesApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (GetDeploymentMachinesApiDeprecated), culture);

    public static string GetDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (GetDeploymentMachineApiDeprecated));

    public static string GetDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (GetDeploymentMachineApiDeprecated), culture);

    public static string EndpointIdsAndNamesAreExclusive() => TaskResources.Get(nameof (EndpointIdsAndNamesAreExclusive));

    public static string EndpointIdsAndNamesAreExclusive(CultureInfo culture) => TaskResources.Get(nameof (EndpointIdsAndNamesAreExclusive), culture);

    public static string AddDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (AddDeploymentMachineApiDeprecated));

    public static string AddDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (AddDeploymentMachineApiDeprecated), culture);

    public static string DeleteDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (DeleteDeploymentMachineApiDeprecated));

    public static string DeleteDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (DeleteDeploymentMachineApiDeprecated), culture);

    public static string ReplaceDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (ReplaceDeploymentMachineApiDeprecated));

    public static string ReplaceDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (ReplaceDeploymentMachineApiDeprecated), culture);

    public static string UpdateDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (UpdateDeploymentMachineApiDeprecated));

    public static string UpdateDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (UpdateDeploymentMachineApiDeprecated), culture);

    public static string UpdateDeploymentMachinesApiDeprecated() => TaskResources.Get(nameof (UpdateDeploymentMachinesApiDeprecated));

    public static string UpdateDeploymentMachinesApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (UpdateDeploymentMachinesApiDeprecated), culture);

    public static string CannotModifyAgentsForHostedPool(object arg0) => TaskResources.Format(nameof (CannotModifyAgentsForHostedPool), arg0);

    public static string CannotModifyAgentsForHostedPool(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotModifyAgentsForHostedPool), culture, arg0);

    public static string ResourceLimitsLocation() => TaskResources.Get(nameof (ResourceLimitsLocation));

    public static string ResourceLimitsLocation(CultureInfo culture) => TaskResources.Get(nameof (ResourceLimitsLocation), culture);

    public static string TaskGroupCyclicDependencyErrorFormat(object arg0) => TaskResources.Format(nameof (TaskGroupCyclicDependencyErrorFormat), arg0);

    public static string TaskGroupCyclicDependencyErrorFormat(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskGroupCyclicDependencyErrorFormat), culture, arg0);

    public static string TaskGroupCyclicDependencyPathItemFormat(object arg0, object arg1) => TaskResources.Format(nameof (TaskGroupCyclicDependencyPathItemFormat), arg0, arg1);

    public static string TaskGroupCyclicDependencyPathItemFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskGroupCyclicDependencyPathItemFormat), culture, arg0, arg1);
    }

    public static string DeploymentGroupsExpandMachinesDeprecated() => TaskResources.Get(nameof (DeploymentGroupsExpandMachinesDeprecated));

    public static string DeploymentGroupsExpandMachinesDeprecated(CultureInfo culture) => TaskResources.Get(nameof (DeploymentGroupsExpandMachinesDeprecated), culture);

    public static string DeploymentPoolAdministratorRoleDescription() => TaskResources.Get(nameof (DeploymentPoolAdministratorRoleDescription));

    public static string DeploymentPoolAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (DeploymentPoolAdministratorRoleDescription), culture);

    public static string DeploymentPoolReaderRoleDescription() => TaskResources.Get(nameof (DeploymentPoolReaderRoleDescription));

    public static string DeploymentPoolReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (DeploymentPoolReaderRoleDescription), culture);

    public static string DeploymentPoolUserRoleDescription() => TaskResources.Get(nameof (DeploymentPoolUserRoleDescription));

    public static string DeploymentPoolUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (DeploymentPoolUserRoleDescription), culture);

    public static string DeploymentGroupAdministratorsGroup() => TaskResources.Get(nameof (DeploymentGroupAdministratorsGroup));

    public static string DeploymentGroupAdministratorsGroup(CultureInfo culture) => TaskResources.Get(nameof (DeploymentGroupAdministratorsGroup), culture);

    public static string DeploymentGroupAdministratorsGroupDescription() => TaskResources.Get(nameof (DeploymentGroupAdministratorsGroupDescription));

    public static string DeploymentGroupAdministratorsGroupDescription(CultureInfo culture) => TaskResources.Get(nameof (DeploymentGroupAdministratorsGroupDescription), culture);

    public static string MachineManagementRequestFailed() => TaskResources.Get(nameof (MachineManagementRequestFailed));

    public static string MachineManagementRequestFailed(CultureInfo culture) => TaskResources.Get(nameof (MachineManagementRequestFailed), culture);

    public static string DeploymentGroupExistsWithPool(object arg0, object arg1) => TaskResources.Format(nameof (DeploymentGroupExistsWithPool), arg0, arg1);

    public static string DeploymentGroupExistsWithPool(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (DeploymentGroupExistsWithPool), culture, arg0, arg1);
    }

    public static string GetAgentRequestsForDeploymentMachineApiDeprecated() => TaskResources.Get(nameof (GetAgentRequestsForDeploymentMachineApiDeprecated));

    public static string GetAgentRequestsForDeploymentMachineApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (GetAgentRequestsForDeploymentMachineApiDeprecated), culture);

    public static string GetAgentRequestsForDeploymentMachinesApiDeprecated() => TaskResources.Get(nameof (GetAgentRequestsForDeploymentMachinesApiDeprecated));

    public static string GetAgentRequestsForDeploymentMachinesApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (GetAgentRequestsForDeploymentMachinesApiDeprecated), culture);

    public static string RefreshDeploymentMachinesApiDeprecated() => TaskResources.Get(nameof (RefreshDeploymentMachinesApiDeprecated));

    public static string RefreshDeploymentMachinesApiDeprecated(CultureInfo culture) => TaskResources.Get(nameof (RefreshDeploymentMachinesApiDeprecated), culture);

    public static string CannotDeleteAndAddMetadata() => TaskResources.Get(nameof (CannotDeleteAndAddMetadata));

    public static string CannotDeleteAndAddMetadata(CultureInfo culture) => TaskResources.Get(nameof (CannotDeleteAndAddMetadata), culture);

    public static string UnexpectedMetadataCompressionType() => TaskResources.Get(nameof (UnexpectedMetadataCompressionType));

    public static string UnexpectedMetadataCompressionType(CultureInfo culture) => TaskResources.Get(nameof (UnexpectedMetadataCompressionType), culture);

    public static string InvalidDeploymentTargetTagLength(object arg0) => TaskResources.Format(nameof (InvalidDeploymentTargetTagLength), arg0);

    public static string InvalidDeploymentTargetTagLength(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidDeploymentTargetTagLength), culture, arg0);

    public static string DuplicateTagsNotSupported(object arg0) => TaskResources.Format(nameof (DuplicateTagsNotSupported), arg0);

    public static string DuplicateTagsNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DuplicateTagsNotSupported), culture, arg0);

    public static string DeploymentPoolInUseMultipleCountMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (DeploymentPoolInUseMultipleCountMessage), arg0, arg1, arg2);
    }

    public static string DeploymentPoolInUseMultipleCountMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (DeploymentPoolInUseMultipleCountMessage), culture, arg0, arg1, arg2);
    }

    public static string CannotModifyAgentsForAgentCloudBackedPool(object arg0) => TaskResources.Format(nameof (CannotModifyAgentsForAgentCloudBackedPool), arg0);

    public static string CannotModifyAgentsForAgentCloudBackedPool(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotModifyAgentsForAgentCloudBackedPool), culture, arg0);

    public static string AccessDeniedForAgentCloudPool() => TaskResources.Get(nameof (AccessDeniedForAgentCloudPool));

    public static string AccessDeniedForAgentCloudPool(CultureInfo culture) => TaskResources.Get(nameof (AccessDeniedForAgentCloudPool), culture);

    public static string AgentCloudExists(object arg0) => TaskResources.Format(nameof (AgentCloudExists), arg0);

    public static string AgentCloudExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentCloudExists), culture, arg0);

    public static string AgentCloudNotFound(object arg0) => TaskResources.Format(nameof (AgentCloudNotFound), arg0);

    public static string AgentCloudNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentCloudNotFound), culture, arg0);

    public static string AgentCloudRequestExists(object arg0, object arg1) => TaskResources.Format(nameof (AgentCloudRequestExists), arg0, arg1);

    public static string AgentCloudRequestExists(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentCloudRequestExists), culture, arg0, arg1);

    public static string AgentCloudRequestNotFound(object arg0, object arg1) => TaskResources.Format(nameof (AgentCloudRequestNotFound), arg0, arg1);

    public static string AgentCloudRequestNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentCloudRequestNotFound), culture, arg0, arg1);

    public static string AccessDeniedForAgentCloud(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AccessDeniedForAgentCloud), arg0, arg1, arg2);

    public static string AccessDeniedForAgentCloud(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AccessDeniedForAgentCloud), culture, arg0, arg1, arg2);
    }

    public static string AgentPoolReferencesDifferentAgentCloud(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (AgentPoolReferencesDifferentAgentCloud), arg0, arg1, arg2);
    }

    public static string AgentPoolReferencesDifferentAgentCloud(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentPoolReferencesDifferentAgentCloud), culture, arg0, arg1, arg2);
    }

    public static string PrivateTaskAgentInvalidProvisioningState(object arg0) => TaskResources.Format(nameof (PrivateTaskAgentInvalidProvisioningState), arg0);

    public static string PrivateTaskAgentInvalidProvisioningState(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PrivateTaskAgentInvalidProvisioningState), culture, arg0);

    public static string InvalidAgentCloudType() => TaskResources.Get(nameof (InvalidAgentCloudType));

    public static string InvalidAgentCloudType(CultureInfo culture) => TaskResources.Get(nameof (InvalidAgentCloudType), culture);

    public static string AgentSessionRevoked(object arg0) => TaskResources.Format(nameof (AgentSessionRevoked), arg0);

    public static string AgentSessionRevoked(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentSessionRevoked), culture, arg0);

    public static string InvalidVariableGroupVariableLength(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (InvalidVariableGroupVariableLength), arg0, arg1, arg2);

    public static string InvalidVariableGroupVariableLength(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidVariableGroupVariableLength), culture, arg0, arg1, arg2);
    }

    public static string InvalidOperationOnInternalAgentCloud(object arg0) => TaskResources.Format(nameof (InvalidOperationOnInternalAgentCloud), arg0);

    public static string InvalidOperationOnInternalAgentCloud(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidOperationOnInternalAgentCloud), culture, arg0);

    public static string ImageLabelNotFound(object arg0) => TaskResources.Format(nameof (ImageLabelNotFound), arg0);

    public static string ImageLabelNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ImageLabelNotFound), culture, arg0);

    public static string MetaTaskServiceEndpointSecurityCheckFailedMessage(object arg0) => TaskResources.Format(nameof (MetaTaskServiceEndpointSecurityCheckFailedMessage), arg0);

    public static string MetaTaskServiceEndpointSecurityCheckFailedMessage(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (MetaTaskServiceEndpointSecurityCheckFailedMessage), culture, arg0);
    }

    public static string InvalidAgentCloudSharedSecret() => TaskResources.Get(nameof (InvalidAgentCloudSharedSecret));

    public static string InvalidAgentCloudSharedSecret(CultureInfo culture) => TaskResources.Get(nameof (InvalidAgentCloudSharedSecret), culture);

    public static string AgentFileNotFound(object arg0) => TaskResources.Format(nameof (AgentFileNotFound), arg0);

    public static string AgentFileNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentFileNotFound), culture, arg0);

    public static string AgentMediaTypeNotSupported(object arg0) => TaskResources.Format(nameof (AgentMediaTypeNotSupported), arg0);

    public static string AgentMediaTypeNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentMediaTypeNotSupported), culture, arg0);

    public static string DeploymentPoolDoesNotExist(object arg0) => TaskResources.Format(nameof (DeploymentPoolDoesNotExist), arg0);

    public static string DeploymentPoolDoesNotExist(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentPoolDoesNotExist), culture, arg0);

    public static string MigrateDeploymentGroupsNotSupported() => TaskResources.Get(nameof (MigrateDeploymentGroupsNotSupported));

    public static string MigrateDeploymentGroupsNotSupported(CultureInfo culture) => TaskResources.Get(nameof (MigrateDeploymentGroupsNotSupported), culture);

    public static string CannotEditAgentCloudOnPool() => TaskResources.Get(nameof (CannotEditAgentCloudOnPool));

    public static string CannotEditAgentCloudOnPool(CultureInfo culture) => TaskResources.Get(nameof (CannotEditAgentCloudOnPool), culture);

    public static string InsufficientPoolPermission() => TaskResources.Get(nameof (InsufficientPoolPermission));

    public static string InsufficientPoolPermission(CultureInfo culture) => TaskResources.Get(nameof (InsufficientPoolPermission), culture);

    public static string FailedToMigrateDeploymentGroups(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (FailedToMigrateDeploymentGroups), arg0, arg1, arg2);

    public static string FailedToMigrateDeploymentGroups(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (FailedToMigrateDeploymentGroups), culture, arg0, arg1, arg2);
    }

    public static string AgentAccessTokenExpired() => TaskResources.Get(nameof (AgentAccessTokenExpired));

    public static string AgentAccessTokenExpired(CultureInfo culture) => TaskResources.Get(nameof (AgentAccessTokenExpired), culture);

    public static string AgentCloudResponseStatusCodeInvalid(object arg0, object arg1) => TaskResources.Format(nameof (AgentCloudResponseStatusCodeInvalid), arg0, arg1);

    public static string AgentCloudResponseStatusCodeInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentCloudResponseStatusCodeInvalid), culture, arg0, arg1);
    }

    public static string EnvironmentExists(object arg0) => TaskResources.Format(nameof (EnvironmentExists), arg0);

    public static string EnvironmentExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentExists), culture, arg0);

    public static string EnvironmentNotFound(object arg0) => TaskResources.Format(nameof (EnvironmentNotFound), arg0);

    public static string EnvironmentNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentNotFound), culture, arg0);

    public static string InvalidEnvironmentName(object arg0) => TaskResources.Format(nameof (InvalidEnvironmentName), arg0);

    public static string InvalidEnvironmentName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidEnvironmentName), culture, arg0);

    public static string EnvironmentExistsInProject(object arg0) => TaskResources.Format(nameof (EnvironmentExistsInProject), arg0);

    public static string EnvironmentExistsInProject(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentExistsInProject), culture, arg0);

    public static string EnvironmentReferenceExists(object arg0) => TaskResources.Format(nameof (EnvironmentReferenceExists), arg0);

    public static string EnvironmentReferenceExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentReferenceExists), culture, arg0);

    public static string TryingToShareVariableGroupWithSameProject() => TaskResources.Get(nameof (TryingToShareVariableGroupWithSameProject));

    public static string TryingToShareVariableGroupWithSameProject(CultureInfo culture) => TaskResources.Get(nameof (TryingToShareVariableGroupWithSameProject), culture);

    public static string VariableGroupAccessDeniedForReadOperation() => TaskResources.Get(nameof (VariableGroupAccessDeniedForReadOperation));

    public static string VariableGroupAccessDeniedForReadOperation(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupAccessDeniedForReadOperation), culture);

    public static string ServiceEndpointNotShared(object arg0, object arg1) => TaskResources.Format(nameof (ServiceEndpointNotShared), arg0, arg1);

    public static string ServiceEndpointNotShared(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (ServiceEndpointNotShared), culture, arg0, arg1);

    public static string ServiceEndpointAccessDeniedForReadOperation(object arg0, object arg1) => TaskResources.Format(nameof (ServiceEndpointAccessDeniedForReadOperation), arg0, arg1);

    public static string ServiceEndpointAccessDeniedForReadOperation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (ServiceEndpointAccessDeniedForReadOperation), culture, arg0, arg1);
    }

    public static string FailedToProvisionAgent() => TaskResources.Get(nameof (FailedToProvisionAgent));

    public static string FailedToProvisionAgent(CultureInfo culture) => TaskResources.Get(nameof (FailedToProvisionAgent), culture);

    public static string JobCanceledByPoolProvider() => TaskResources.Get(nameof (JobCanceledByPoolProvider));

    public static string JobCanceledByPoolProvider(CultureInfo culture) => TaskResources.Get(nameof (JobCanceledByPoolProvider), culture);

    public static string EnvironmentAdministratorRoleDescription() => TaskResources.Get(nameof (EnvironmentAdministratorRoleDescription));

    public static string EnvironmentAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentAdministratorRoleDescription), culture);

    public static string EnvironmentUserRoleDescription() => TaskResources.Get(nameof (EnvironmentUserRoleDescription));

    public static string EnvironmentUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentUserRoleDescription), culture);

    public static string GlobalEnvironmentAdministratorRoleDescription() => TaskResources.Get(nameof (GlobalEnvironmentAdministratorRoleDescription));

    public static string GlobalEnvironmentAdministratorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (GlobalEnvironmentAdministratorRoleDescription), culture);

    public static string EnvironmentCreatorRoleDescription() => TaskResources.Get(nameof (EnvironmentCreatorRoleDescription));

    public static string EnvironmentCreatorRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentCreatorRoleDescription), culture);

    public static string EnvironmentReaderRoleDescription() => TaskResources.Get(nameof (EnvironmentReaderRoleDescription));

    public static string EnvironmentReaderRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentReaderRoleDescription), culture);

    public static string EnvironmentReferenceUserRoleDescription() => TaskResources.Get(nameof (EnvironmentReferenceUserRoleDescription));

    public static string EnvironmentReferenceUserRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentReferenceUserRoleDescription), culture);

    public static string EnvironmentPermissionInitializationError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (EnvironmentPermissionInitializationError), arg0, arg1, arg2, arg3);
    }

    public static string EnvironmentPermissionInitializationError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (EnvironmentPermissionInitializationError), culture, arg0, arg1, arg2, arg3);
    }

    public static string AgentRequestFailedDueToExpiration(object arg0) => TaskResources.Format(nameof (AgentRequestFailedDueToExpiration), arg0);

    public static string AgentRequestFailedDueToExpiration(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentRequestFailedDueToExpiration), culture, arg0);

    public static string EnvironmentResourceExists(object arg0) => TaskResources.Format(nameof (EnvironmentResourceExists), arg0);

    public static string EnvironmentResourceExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentResourceExists), culture, arg0);

    public static string EnvironmentResourceNotFound(object arg0, object arg1) => TaskResources.Format(nameof (EnvironmentResourceNotFound), arg0, arg1);

    public static string EnvironmentResourceNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentResourceNotFound), culture, arg0, arg1);

    public static string InvalidResourceName(object arg0) => TaskResources.Format(nameof (InvalidResourceName), arg0);

    public static string InvalidResourceName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidResourceName), culture, arg0);

    public static string EnvironmentReferenceTokenFormatException() => TaskResources.Get(nameof (EnvironmentReferenceTokenFormatException));

    public static string EnvironmentReferenceTokenFormatException(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentReferenceTokenFormatException), culture);

    public static string GlobalEnvironmentReferenceTokenFormatException() => TaskResources.Get(nameof (GlobalEnvironmentReferenceTokenFormatException));

    public static string GlobalEnvironmentReferenceTokenFormatException(CultureInfo culture) => TaskResources.Get(nameof (GlobalEnvironmentReferenceTokenFormatException), culture);

    public static string LibraryCollectionAdminRoleDescription() => TaskResources.Get(nameof (LibraryCollectionAdminRoleDescription));

    public static string LibraryCollectionAdminRoleDescription(CultureInfo culture) => TaskResources.Get(nameof (LibraryCollectionAdminRoleDescription), culture);

    public static string VariableGroupAccessDeniedForCollectionAdminOperation() => TaskResources.Get(nameof (VariableGroupAccessDeniedForCollectionAdminOperation));

    public static string VariableGroupAccessDeniedForCollectionAdminOperation(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupAccessDeniedForCollectionAdminOperation), culture);

    public static string EnvironmentDeploymentExecutionHistoryRecordNotFound(
      object arg0,
      object arg1)
    {
      return TaskResources.Format(nameof (EnvironmentDeploymentExecutionHistoryRecordNotFound), arg0, arg1);
    }

    public static string EnvironmentDeploymentExecutionHistoryRecordNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (EnvironmentDeploymentExecutionHistoryRecordNotFound), culture, arg0, arg1);
    }

    public static string EnvironmentHistoryAccessDeniedException() => TaskResources.Get(nameof (EnvironmentHistoryAccessDeniedException));

    public static string EnvironmentHistoryAccessDeniedException(CultureInfo culture) => TaskResources.Get(nameof (EnvironmentHistoryAccessDeniedException), culture);

    public static string InvalidServiceEndpoint(object arg0) => TaskResources.Format(nameof (InvalidServiceEndpoint), arg0);

    public static string InvalidServiceEndpoint(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidServiceEndpoint), culture, arg0);

    public static string EnvironmentResourcesExceededMaxCount(object arg0, object arg1) => TaskResources.Format(nameof (EnvironmentResourcesExceededMaxCount), arg0, arg1);

    public static string EnvironmentResourcesExceededMaxCount(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (EnvironmentResourcesExceededMaxCount), culture, arg0, arg1);
    }

    public static string ManageHistory() => TaskResources.Get(nameof (ManageHistory));

    public static string ManageHistory(CultureInfo culture) => TaskResources.Get(nameof (ManageHistory), culture);

    public static string InvalidContinuationToken(object arg0) => TaskResources.Format(nameof (InvalidContinuationToken), arg0);

    public static string InvalidContinuationToken(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidContinuationToken), culture, arg0);

    public static string AKSRollingNotSupported() => TaskResources.Get(nameof (AKSRollingNotSupported));

    public static string AKSRollingNotSupported(CultureInfo culture) => TaskResources.Get(nameof (AKSRollingNotSupported), culture);

    public static string InvalidResourcesType() => TaskResources.Get(nameof (InvalidResourcesType));

    public static string InvalidResourcesType(CultureInfo culture) => TaskResources.Get(nameof (InvalidResourcesType), culture);

    public static string VMRunOnceNotSupported() => TaskResources.Get(nameof (VMRunOnceNotSupported));

    public static string VMRunOnceNotSupported(CultureInfo culture) => TaskResources.Get(nameof (VMRunOnceNotSupported), culture);

    public static string EnvironmentPoolAlreadyInUse(object arg0) => TaskResources.Format(nameof (EnvironmentPoolAlreadyInUse), arg0);

    public static string EnvironmentPoolAlreadyInUse(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentPoolAlreadyInUse), culture, arg0);

    public static string DeploymentPoolNotFound(object arg0) => TaskResources.Format(nameof (DeploymentPoolNotFound), arg0);

    public static string DeploymentPoolNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentPoolNotFound), culture, arg0);

    public static string VirtualMachineGroupsShouldUseDeploymentPools() => TaskResources.Get(nameof (VirtualMachineGroupsShouldUseDeploymentPools));

    public static string VirtualMachineGroupsShouldUseDeploymentPools(CultureInfo culture) => TaskResources.Get(nameof (VirtualMachineGroupsShouldUseDeploymentPools), culture);

    public static string AgentRequestBlockedByParallelismLimits(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (AgentRequestBlockedByParallelismLimits), arg0, arg1, arg2, arg3);
    }

    public static string AgentRequestBlockedByParallelismLimits(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentRequestBlockedByParallelismLimits), culture, arg0, arg1, arg2, arg3);
    }

    public static string AgentRequestIsBeingProcessed(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (AgentRequestIsBeingProcessed), arg0, arg1, arg2, arg3);
    }

    public static string AgentRequestIsBeingProcessed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentRequestIsBeingProcessed), culture, arg0, arg1, arg2, arg3);
    }

    public static string AgentRequestPoolProviderSlowWarning(object arg0, object arg1) => TaskResources.Format(nameof (AgentRequestPoolProviderSlowWarning), arg0, arg1);

    public static string AgentRequestPoolProviderSlowWarning(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentRequestPoolProviderSlowWarning), culture, arg0, arg1);
    }

    public static string AgentRequestBlockedByPoolUsage(object arg0) => TaskResources.Format(nameof (AgentRequestBlockedByPoolUsage), arg0);

    public static string AgentRequestBlockedByPoolUsage(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentRequestBlockedByPoolUsage), culture, arg0);

    public static string WaitingOnAgentToStartRequest() => TaskResources.Get(nameof (WaitingOnAgentToStartRequest));

    public static string WaitingOnAgentToStartRequest(CultureInfo culture) => TaskResources.Get(nameof (WaitingOnAgentToStartRequest), culture);

    public static string AgentRequestRunningOrCompleted() => TaskResources.Get(nameof (AgentRequestRunningOrCompleted));

    public static string AgentRequestRunningOrCompleted(CultureInfo culture) => TaskResources.Get(nameof (AgentRequestRunningOrCompleted), culture);

    public static string AgentCloudDidNotReturnStatus() => TaskResources.Get(nameof (AgentCloudDidNotReturnStatus));

    public static string AgentCloudDidNotReturnStatus(CultureInfo culture) => TaskResources.Get(nameof (AgentCloudDidNotReturnStatus), culture);

    public static string AgentCloudDoesNotSupportStatus() => TaskResources.Get(nameof (AgentCloudDoesNotSupportStatus));

    public static string AgentCloudDoesNotSupportStatus(CultureInfo culture) => TaskResources.Get(nameof (AgentCloudDoesNotSupportStatus), culture);

    public static string WaitingForAgentCloudProvisioning(object arg0) => TaskResources.Format(nameof (WaitingForAgentCloudProvisioning), arg0);

    public static string WaitingForAgentCloudProvisioning(object arg0, CultureInfo culture) => TaskResources.Format(nameof (WaitingForAgentCloudProvisioning), culture, arg0);

    public static string AgentCloudConnectionIssue() => TaskResources.Get(nameof (AgentCloudConnectionIssue));

    public static string AgentCloudConnectionIssue(CultureInfo culture) => TaskResources.Get(nameof (AgentCloudConnectionIssue), culture);

    public static string AgentPoolRemoved(object arg0, object arg1) => TaskResources.Format(nameof (AgentPoolRemoved), arg0, arg1);

    public static string AgentPoolRemoved(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (AgentPoolRemoved), culture, arg0, arg1);

    public static string VirtualMachineNotFound(object arg0) => TaskResources.Format(nameof (VirtualMachineNotFound), arg0);

    public static string VirtualMachineNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VirtualMachineNotFound), culture, arg0);

    public static string DemandsNotMet(object arg0, object arg1) => TaskResources.Format(nameof (DemandsNotMet), arg0, arg1);

    public static string DemandsNotMet(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (DemandsNotMet), culture, arg0, arg1);

    public static string DeploymentSkippedOnVM(object arg0) => TaskResources.Format(nameof (DeploymentSkippedOnVM), arg0);

    public static string DeploymentSkippedOnVM(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentSkippedOnVM), culture, arg0);

    public static string DeploymentStrategyNotMet(object arg0) => TaskResources.Format(nameof (DeploymentStrategyNotMet), arg0);

    public static string DeploymentStrategyNotMet(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DeploymentStrategyNotMet), culture, arg0);

    public static string MachineOffline(object arg0) => TaskResources.Format(nameof (MachineOffline), arg0);

    public static string MachineOffline(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MachineOffline), culture, arg0);

    public static string NoMachineFound() => TaskResources.Get(nameof (NoMachineFound));

    public static string NoMachineFound(CultureInfo culture) => TaskResources.Get(nameof (NoMachineFound), culture);

    public static string AccessDeniedForEnvironment(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (AccessDeniedForEnvironment), arg0, arg1, arg2);

    public static string AccessDeniedForEnvironment(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AccessDeniedForEnvironment), culture, arg0, arg1, arg2);
    }

    public static string EnvironmentWithGivenNameNotFound(object arg0) => TaskResources.Format(nameof (EnvironmentWithGivenNameNotFound), arg0);

    public static string EnvironmentWithGivenNameNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentWithGivenNameNotFound), culture, arg0);

    public static string InvalidDeploymentTagWithNullValue() => TaskResources.Get(nameof (InvalidDeploymentTagWithNullValue));

    public static string InvalidDeploymentTagWithNullValue(CultureInfo culture) => TaskResources.Get(nameof (InvalidDeploymentTagWithNullValue), culture);

    public static string GetDeploymentPoolsSummaryInvalidRequest() => TaskResources.Get(nameof (GetDeploymentPoolsSummaryInvalidRequest));

    public static string GetDeploymentPoolsSummaryInvalidRequest(CultureInfo culture) => TaskResources.Get(nameof (GetDeploymentPoolsSummaryInvalidRequest), culture);

    public static string InvalidDeploymentStrategy(object arg0, object arg1) => TaskResources.Format(nameof (InvalidDeploymentStrategy), arg0, arg1);

    public static string InvalidDeploymentStrategy(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (InvalidDeploymentStrategy), culture, arg0, arg1);

    public static string PoolProviderFailedToSendMessageResponseCode(object arg0) => TaskResources.Format(nameof (PoolProviderFailedToSendMessageResponseCode), arg0);

    public static string PoolProviderFailedToSendMessageResponseCode(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (PoolProviderFailedToSendMessageResponseCode), culture, arg0);
    }

    public static string PoolProviderRequestTimeout() => TaskResources.Get(nameof (PoolProviderRequestTimeout));

    public static string PoolProviderRequestTimeout(CultureInfo culture) => TaskResources.Get(nameof (PoolProviderRequestTimeout), culture);

    public static string PoolProviderRequestUnexpectedFailure() => TaskResources.Get(nameof (PoolProviderRequestUnexpectedFailure));

    public static string PoolProviderRequestUnexpectedFailure(CultureInfo culture) => TaskResources.Get(nameof (PoolProviderRequestUnexpectedFailure), culture);

    public static string SharingVariableGroupNotAllowed() => TaskResources.Get(nameof (SharingVariableGroupNotAllowed));

    public static string SharingVariableGroupNotAllowed(CultureInfo culture) => TaskResources.Get(nameof (SharingVariableGroupNotAllowed), culture);

    public static string ProjectAdministratorsGroupName() => TaskResources.Get(nameof (ProjectAdministratorsGroupName));

    public static string ProjectAdministratorsGroupName(CultureInfo culture) => TaskResources.Get(nameof (ProjectAdministratorsGroupName), culture);

    public static string CannotEditIsLegacyOnPool() => TaskResources.Get(nameof (CannotEditIsLegacyOnPool));

    public static string CannotEditIsLegacyOnPool(CultureInfo culture) => TaskResources.Get(nameof (CannotEditIsLegacyOnPool), culture);

    public static string NewEnvrionmentDescription() => TaskResources.Get(nameof (NewEnvrionmentDescription));

    public static string NewEnvrionmentDescription(CultureInfo culture) => TaskResources.Get(nameof (NewEnvrionmentDescription), culture);

    public static string PlanNotFound(object arg0) => TaskResources.Format(nameof (PlanNotFound), arg0);

    public static string PlanNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PlanNotFound), culture, arg0);

    public static string EnvironmentNameFormat(object arg0, object arg1) => TaskResources.Format(nameof (EnvironmentNameFormat), arg0, arg1);

    public static string EnvironmentNameFormat(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentNameFormat), culture, arg0, arg1);

    public static string AgentCloudRequestAlreadyCompleted(object arg0, object arg1) => TaskResources.Format(nameof (AgentCloudRequestAlreadyCompleted), arg0, arg1);

    public static string AgentCloudRequestAlreadyCompleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentCloudRequestAlreadyCompleted), culture, arg0, arg1);
    }

    public static string InvalidEnvironmentResourceFilter() => TaskResources.Get(nameof (InvalidEnvironmentResourceFilter));

    public static string InvalidEnvironmentResourceFilter(CultureInfo culture) => TaskResources.Get(nameof (InvalidEnvironmentResourceFilter), culture);

    public static string VariableGroupAccessDeniedForDeleteOperation() => TaskResources.Get(nameof (VariableGroupAccessDeniedForDeleteOperation));

    public static string VariableGroupAccessDeniedForDeleteOperation(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupAccessDeniedForDeleteOperation), culture);

    public static string OwnerLibraryItem() => TaskResources.Get(nameof (OwnerLibraryItem));

    public static string OwnerLibraryItem(CultureInfo culture) => TaskResources.Get(nameof (OwnerLibraryItem), culture);

    public static string CannotCreateSameVariableGroupMultipleTime() => TaskResources.Get(nameof (CannotCreateSameVariableGroupMultipleTime));

    public static string CannotCreateSameVariableGroupMultipleTime(CultureInfo culture) => TaskResources.Get(nameof (CannotCreateSameVariableGroupMultipleTime), culture);

    public static string InvalidProjectIds(object arg0) => TaskResources.Format(nameof (InvalidProjectIds), arg0);

    public static string InvalidProjectIds(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidProjectIds), culture, arg0);

    public static string AtleastOneProjectReferenceRequired() => TaskResources.Get(nameof (AtleastOneProjectReferenceRequired));

    public static string AtleastOneProjectReferenceRequired(CultureInfo culture) => TaskResources.Get(nameof (AtleastOneProjectReferenceRequired), culture);

    public static string AtleastOneProjectRequiredForDeletion() => TaskResources.Get(nameof (AtleastOneProjectRequiredForDeletion));

    public static string AtleastOneProjectRequiredForDeletion(CultureInfo culture) => TaskResources.Get(nameof (AtleastOneProjectRequiredForDeletion), culture);

    public static string VariableGroupProjectReferenceInformationIdInvalid() => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationIdInvalid));

    public static string VariableGroupProjectReferenceInformationIdInvalid(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationIdInvalid), culture);

    public static string VariableGroupProjectReferenceInformationNameInvalid() => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationNameInvalid));

    public static string VariableGroupProjectReferenceInformationNameInvalid(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationNameInvalid), culture);

    public static string VariableGroupProjectReferenceInformationNotProvided() => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationNotProvided));

    public static string VariableGroupProjectReferenceInformationNotProvided(CultureInfo culture) => TaskResources.Get(nameof (VariableGroupProjectReferenceInformationNotProvided), culture);

    public static string VariableGroupNotPartOfProject(object arg0) => TaskResources.Format(nameof (VariableGroupNotPartOfProject), arg0);

    public static string VariableGroupNotPartOfProject(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VariableGroupNotPartOfProject), culture, arg0);

    public static string RequestWasCancelledByRemoteProvider() => TaskResources.Get(nameof (RequestWasCancelledByRemoteProvider));

    public static string RequestWasCancelledByRemoteProvider(CultureInfo culture) => TaskResources.Get(nameof (RequestWasCancelledByRemoteProvider), culture);

    public static string SkippingTaskGroupRestore() => TaskResources.Get(nameof (SkippingTaskGroupRestore));

    public static string SkippingTaskGroupRestore(CultureInfo culture) => TaskResources.Get(nameof (SkippingTaskGroupRestore), culture);

    public static string InValidTaskGroupUpdateProperties() => TaskResources.Get(nameof (InValidTaskGroupUpdateProperties));

    public static string InValidTaskGroupUpdateProperties(CultureInfo culture) => TaskResources.Get(nameof (InValidTaskGroupUpdateProperties), culture);

    public static string EnvironmentPoolExists(object arg0, object arg1) => TaskResources.Format(nameof (EnvironmentPoolExists), arg0, arg1);

    public static string EnvironmentPoolExists(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentPoolExists), culture, arg0, arg1);

    public static string EnvironmentPoolNotFound(object arg0) => TaskResources.Format(nameof (EnvironmentPoolNotFound), arg0);

    public static string EnvironmentPoolNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentPoolNotFound), culture, arg0);

    public static string EnvironmentResourceCreationFailed(object arg0) => TaskResources.Format(nameof (EnvironmentResourceCreationFailed), arg0);

    public static string EnvironmentResourceCreationFailed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EnvironmentResourceCreationFailed), culture, arg0);

    public static string KubernetesResourceInvalidProvider(object arg0) => TaskResources.Format(nameof (KubernetesResourceInvalidProvider), arg0);

    public static string KubernetesResourceInvalidProvider(object arg0, CultureInfo culture) => TaskResources.Format(nameof (KubernetesResourceInvalidProvider), culture, arg0);

    public static string KubernetesResourceMissingInputs() => TaskResources.Get(nameof (KubernetesResourceMissingInputs));

    public static string KubernetesResourceMissingInputs(CultureInfo culture) => TaskResources.Get(nameof (KubernetesResourceMissingInputs), culture);

    public static string KubernetesResourceUpdateFailed(object arg0) => TaskResources.Format(nameof (KubernetesResourceUpdateFailed), arg0);

    public static string KubernetesResourceUpdateFailed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (KubernetesResourceUpdateFailed), culture, arg0);

    public static string VirtualMachineResourceAgentMismatch() => TaskResources.Get(nameof (VirtualMachineResourceAgentMismatch));

    public static string VirtualMachineResourceAgentMismatch(CultureInfo culture) => TaskResources.Get(nameof (VirtualMachineResourceAgentMismatch), culture);

    public static string NoResourceFoundWithSpecifiedFilter(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TaskResources.Format(nameof (NoResourceFoundWithSpecifiedFilter), arg0, arg1, arg2, arg3, arg4);
    }

    public static string NoResourceFoundWithSpecifiedFilter(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (NoResourceFoundWithSpecifiedFilter), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string KubernetesResourcesDeletionFailed(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (KubernetesResourcesDeletionFailed), arg0, arg1, arg2);

    public static string KubernetesResourcesDeletionFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (KubernetesResourcesDeletionFailed), culture, arg0, arg1, arg2);
    }

    public static string EnvironmentResourceDeploymentHistoryRecordNotFound(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (EnvironmentResourceDeploymentHistoryRecordNotFound), arg0, arg1, arg2);
    }

    public static string EnvironmentResourceDeploymentHistoryRecordNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (EnvironmentResourceDeploymentHistoryRecordNotFound), culture, arg0, arg1, arg2);
    }

    public static string RequestMaxSizeExceeded(object arg0) => TaskResources.Format(nameof (RequestMaxSizeExceeded), arg0);

    public static string RequestMaxSizeExceeded(object arg0, CultureInfo culture) => TaskResources.Format(nameof (RequestMaxSizeExceeded), culture, arg0);

    public static string ElasticPoolAlreadyExistsExceptionMessage(object arg0) => TaskResources.Format(nameof (ElasticPoolAlreadyExistsExceptionMessage), arg0);

    public static string ElasticPoolAlreadyExistsExceptionMessage(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ElasticPoolAlreadyExistsExceptionMessage), culture, arg0);

    public static string ElasticPoolDoesNotExistExceptionMessage(object arg0) => TaskResources.Format(nameof (ElasticPoolDoesNotExistExceptionMessage), arg0);

    public static string ElasticPoolDoesNotExistExceptionMessage(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ElasticPoolDoesNotExistExceptionMessage), culture, arg0);

    public static string ElasticPoolInvalidAgentPoolExceptionMessage() => TaskResources.Get(nameof (ElasticPoolInvalidAgentPoolExceptionMessage));

    public static string ElasticPoolInvalidAgentPoolExceptionMessage(CultureInfo culture) => TaskResources.Get(nameof (ElasticPoolInvalidAgentPoolExceptionMessage), culture);

    public static string AgentRequestPoolNoneMatched(object arg0) => TaskResources.Format(nameof (AgentRequestPoolNoneMatched), arg0);

    public static string AgentRequestPoolNoneMatched(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AgentRequestPoolNoneMatched), culture, arg0);

    public static string AgentRequestPoolNoUpdateStatus(object arg0, object arg1) => TaskResources.Format(nameof (AgentRequestPoolNoUpdateStatus), arg0, arg1);

    public static string AgentRequestPoolNoUpdateStatus(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AgentRequestPoolNoUpdateStatus), culture, arg0, arg1);
    }

    public static string UseEndpoint() => TaskResources.Get(nameof (UseEndpoint));

    public static string UseEndpoint(CultureInfo culture) => TaskResources.Get(nameof (UseEndpoint), culture);

    public static string RequestedBy(object arg0) => TaskResources.Format(nameof (RequestedBy), arg0);

    public static string RequestedBy(object arg0, CultureInfo culture) => TaskResources.Format(nameof (RequestedBy), culture, arg0);

    public static string DuplicatePoolNameAndTypeExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return TaskResources.Format(nameof (DuplicatePoolNameAndTypeExceptionMessage), arg0, arg1, arg2);
    }

    public static string DuplicatePoolNameAndTypeExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (DuplicatePoolNameAndTypeExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string VirtualMachineResourceLinkedPoolNull() => TaskResources.Get(nameof (VirtualMachineResourceLinkedPoolNull));

    public static string VirtualMachineResourceLinkedPoolNull(CultureInfo culture) => TaskResources.Get(nameof (VirtualMachineResourceLinkedPoolNull), culture);

    public static string InvalidEnvironmentVirtualMachineResourceName(object arg0) => TaskResources.Format(nameof (InvalidEnvironmentVirtualMachineResourceName), arg0);

    public static string InvalidEnvironmentVirtualMachineResourceName(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidEnvironmentVirtualMachineResourceName), culture, arg0);
    }

    public static string InvalidAgentCloudUpdateOfSharedSecret() => TaskResources.Get(nameof (InvalidAgentCloudUpdateOfSharedSecret));

    public static string InvalidAgentCloudUpdateOfSharedSecret(CultureInfo culture) => TaskResources.Get(nameof (InvalidAgentCloudUpdateOfSharedSecret), culture);

    public static string InternalCloudAgentDefinitionExistsExceptionMessage(object arg0) => TaskResources.Format(nameof (InternalCloudAgentDefinitionExistsExceptionMessage), arg0);

    public static string InternalCloudAgentDefinitionExistsExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InternalCloudAgentDefinitionExistsExceptionMessage), culture, arg0);
    }

    public static string InternalCloudAgentDefinitionNotFoundExceptionMessage(object arg0) => TaskResources.Format(nameof (InternalCloudAgentDefinitionNotFoundExceptionMessage), arg0);

    public static string InternalCloudAgentDefinitionNotFoundExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InternalCloudAgentDefinitionNotFoundExceptionMessage), culture, arg0);
    }

    public static string ClassicPipelinesDisabled() => TaskResources.Get(nameof (ClassicPipelinesDisabled));

    public static string ClassicPipelinesDisabled(CultureInfo culture) => TaskResources.Get(nameof (ClassicPipelinesDisabled), culture);

    public static string ExclusiveLockRequestAlreadyQueued(object arg0) => TaskResources.Format(nameof (ExclusiveLockRequestAlreadyQueued), arg0);

    public static string ExclusiveLockRequestAlreadyQueued(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ExclusiveLockRequestAlreadyQueued), culture, arg0);

    public static string PersistedStageGroupAlreadyMapped(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (PersistedStageGroupAlreadyMapped), arg0, arg1, arg2, arg3);
    }

    public static string PersistedStageGroupAlreadyMapped(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (PersistedStageGroupAlreadyMapped), culture, arg0, arg1, arg2, arg3);
    }
  }
}
