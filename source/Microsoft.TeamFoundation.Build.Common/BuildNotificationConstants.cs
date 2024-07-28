// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildNotificationConstants
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BuildNotificationConstants
  {
    public static readonly string ApplicationName = "BuildNotificationApp.exe";
    private const int WM_APP = 32768;
    public const int UM_SHOW_BUILD_STATUS = 32769;
    public static readonly string BuildNotificationRootKey = BuildCommonUtil.BuildRegistryKeyPath + "\\BuildNotification\\";
    public static readonly string Subscriptions = nameof (Subscriptions);
    public static readonly string SubscriptionsKey = BuildNotificationConstants.BuildNotificationRootKey + BuildNotificationConstants.Subscriptions;
    public static readonly string GatedBuilds = nameof (GatedBuilds);
    public static readonly string AutoStartValue = "AutoStart";
    public static readonly string EventsSubscribedValue = "EventsSubscribed";
    public static readonly string PollingIntervalValue = "PollingInterval";
    public static readonly string TriggerSubscribedValue = "TriggerSubscribed";
    public static readonly string BuildDefinitionSubscriptionTypeValue = "SubscriptionType";
    private const string ConfigurationMutex = "_ECD2DF5D_B86E_4C49_B2EA_8C25474F9BE0_";

    public static Mutex GetConfigurationMutex() => BuildNotificationConstants.GetConfigurationMutex(WindowsIdentity.GetCurrent());

    public static Mutex GetConfigurationMutex(WindowsIdentity identity)
    {
      ArgumentUtility.CheckForNull<WindowsIdentity>(identity, nameof (identity));
      return new Mutex(false, "_ECD2DF5D_B86E_4C49_B2EA_8C25474F9BE0_" + identity.User.Value);
    }
  }
}
