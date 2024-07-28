// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal static class ElasticResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ElasticResources), typeof (ElasticResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ElasticResources.s_resMgr;

    private static string Get(string resourceName) => ElasticResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ElasticResources.Get(resourceName) : ElasticResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ElasticResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ElasticResources.GetInt(resourceName) : (int) ElasticResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ElasticResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ElasticResources.GetBool(resourceName) : (bool) ElasticResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ElasticResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ElasticResources.Get(resourceName, culture);
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

    public static string AgentFailedToStart(object arg0) => ElasticResources.Format(nameof (AgentFailedToStart), arg0);

    public static string AgentFailedToStart(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (AgentFailedToStart), culture, arg0);

    public static string AgentWentOffline(object arg0) => ElasticResources.Format(nameof (AgentWentOffline), arg0);

    public static string AgentWentOffline(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (AgentWentOffline), culture, arg0);

    public static string AzurePipelinesExtensionMessage(object arg0) => ElasticResources.Format(nameof (AzurePipelinesExtensionMessage), arg0);

    public static string AzurePipelinesExtensionMessage(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (AzurePipelinesExtensionMessage), culture, arg0);

    public static string CustomScriptExtensionMessage(object arg0) => ElasticResources.Format(nameof (CustomScriptExtensionMessage), arg0);

    public static string CustomScriptExtensionMessage(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (CustomScriptExtensionMessage), culture, arg0);

    public static string DecreasingCapacity(object arg0, object arg1) => ElasticResources.Format(nameof (DecreasingCapacity), arg0, arg1);

    public static string DecreasingCapacity(object arg0, object arg1, CultureInfo culture) => ElasticResources.Format(nameof (DecreasingCapacity), culture, arg0, arg1);

    public static string DecreasingCapacityError(object arg0) => ElasticResources.Format(nameof (DecreasingCapacityError), arg0);

    public static string DecreasingCapacityError(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (DecreasingCapacityError), culture, arg0);

    public static string DeletingUnhealthyMachines(object arg0) => ElasticResources.Format(nameof (DeletingUnhealthyMachines), arg0);

    public static string DeletingUnhealthyMachines(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (DeletingUnhealthyMachines), culture, arg0);

    public static string DisablingAutomaticOSUpgradePolicy() => ElasticResources.Get(nameof (DisablingAutomaticOSUpgradePolicy));

    public static string DisablingAutomaticOSUpgradePolicy(CultureInfo culture) => ElasticResources.Get(nameof (DisablingAutomaticOSUpgradePolicy), culture);

    public static string DisablingOverprovisionSetting() => ElasticResources.Get(nameof (DisablingOverprovisionSetting));

    public static string DisablingOverprovisionSetting(CultureInfo culture) => ElasticResources.Get(nameof (DisablingOverprovisionSetting), culture);

    public static string DisablingSinglePlacementGroupSetting() => ElasticResources.Get(nameof (DisablingSinglePlacementGroupSetting));

    public static string DisablingSinglePlacementGroupSetting(CultureInfo culture) => ElasticResources.Get(nameof (DisablingSinglePlacementGroupSetting), culture);

    public static string DisablingVmssUpgradePolicy() => ElasticResources.Get(nameof (DisablingVmssUpgradePolicy));

    public static string DisablingVmssUpgradePolicy(CultureInfo culture) => ElasticResources.Get(nameof (DisablingVmssUpgradePolicy), culture);

    public static string ElasticPoolInvalidValue(object arg0) => ElasticResources.Format(nameof (ElasticPoolInvalidValue), arg0);

    public static string ElasticPoolInvalidValue(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (ElasticPoolInvalidValue), culture, arg0);

    public static string ElasticPoolDoesNotExist(object arg0) => ElasticResources.Format(nameof (ElasticPoolDoesNotExist), arg0);

    public static string ElasticPoolDoesNotExist(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (ElasticPoolDoesNotExist), culture, arg0);

    public static string GetScalesetFailure(object arg0) => ElasticResources.Format(nameof (GetScalesetFailure), arg0);

    public static string GetScalesetFailure(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (GetScalesetFailure), culture, arg0);

    public static string IncreasingCapacity(object arg0, object arg1) => ElasticResources.Format(nameof (IncreasingCapacity), arg0, arg1);

    public static string IncreasingCapacity(object arg0, object arg1, CultureInfo culture) => ElasticResources.Format(nameof (IncreasingCapacity), culture, arg0, arg1);

    public static string IncreasingCapacityError(object arg0) => ElasticResources.Format(nameof (IncreasingCapacityError), arg0);

    public static string IncreasingCapacityError(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (IncreasingCapacityError), culture, arg0);

    public static string InvalidServiceEndpoint() => ElasticResources.Get(nameof (InvalidServiceEndpoint));

    public static string InvalidServiceEndpoint(CultureInfo culture) => ElasticResources.Get(nameof (InvalidServiceEndpoint), culture);

    public static string ReimagingMachines(object arg0) => ElasticResources.Format(nameof (ReimagingMachines), arg0);

    public static string ReimagingMachines(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (ReimagingMachines), culture, arg0);

    public static string ReimagingMachinesError(object arg0) => ElasticResources.Format(nameof (ReimagingMachinesError), arg0);

    public static string ReimagingMachinesError(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (ReimagingMachinesError), culture, arg0);

    public static string RemovingAutoscaleRule(object arg0) => ElasticResources.Format(nameof (RemovingAutoscaleRule), arg0);

    public static string RemovingAutoscaleRule(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (RemovingAutoscaleRule), culture, arg0);

    public static string SavedNode(object arg0) => ElasticResources.Format(nameof (SavedNode), arg0);

    public static string SavedNode(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (SavedNode), culture, arg0);

    public static string ServiceEndpointDoesNotExist() => ElasticResources.Get(nameof (ServiceEndpointDoesNotExist));

    public static string ServiceEndpointDoesNotExist(CultureInfo culture) => ElasticResources.Get(nameof (ServiceEndpointDoesNotExist), culture);

    public static string UnhealthyAgentCameOnline(object arg0) => ElasticResources.Format(nameof (UnhealthyAgentCameOnline), arg0);

    public static string UnhealthyAgentCameOnline(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (UnhealthyAgentCameOnline), culture, arg0);

    public static string UpdateElasticNodeStateNotPendingDelete() => ElasticResources.Get(nameof (UpdateElasticNodeStateNotPendingDelete));

    public static string UpdateElasticNodeStateNotPendingDelete(CultureInfo culture) => ElasticResources.Get(nameof (UpdateElasticNodeStateNotPendingDelete), culture);

    public static string UpdateScalesetFailure(object arg0) => ElasticResources.Format(nameof (UpdateScalesetFailure), arg0);

    public static string UpdateScalesetFailure(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (UpdateScalesetFailure), culture, arg0);

    public static string UpdatingVirtualMachineScaleSet() => ElasticResources.Get(nameof (UpdatingVirtualMachineScaleSet));

    public static string UpdatingVirtualMachineScaleSet(CultureInfo culture) => ElasticResources.Get(nameof (UpdatingVirtualMachineScaleSet), culture);

    public static string VirtualMachineExtensionMessageError(object arg0) => ElasticResources.Format(nameof (VirtualMachineExtensionMessageError), arg0);

    public static string VirtualMachineExtensionMessageError(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (VirtualMachineExtensionMessageError), culture, arg0);

    public static string VirtualMachineStopped(object arg0) => ElasticResources.Format(nameof (VirtualMachineStopped), arg0);

    public static string VirtualMachineStopped(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (VirtualMachineStopped), culture, arg0);

    public static string RefreshingExpiredMachines(object arg0) => ElasticResources.Format(nameof (RefreshingExpiredMachines), arg0);

    public static string RefreshingExpiredMachines(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (RefreshingExpiredMachines), culture, arg0);

    public static string ServiceEndpointDisabled(object arg0) => ElasticResources.Format(nameof (ServiceEndpointDisabled), arg0);

    public static string ServiceEndpointDisabled(object arg0, CultureInfo culture) => ElasticResources.Format(nameof (ServiceEndpointDisabled), culture, arg0);
  }
}
