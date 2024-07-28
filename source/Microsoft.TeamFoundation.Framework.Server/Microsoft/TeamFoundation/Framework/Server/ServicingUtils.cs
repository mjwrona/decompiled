// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ServicingUtils
  {
    public const string GoLiveOverrideVariable = "TFSAllowUpgradeFromNonGoLive";
    private static ReleaseManifest s_releaseManifest;
    private const string c_minimumSupportedRelease = "Dev14.M83-Part6";

    internal static ServiceLevel GetServiceLevelFromReleaseManifest() => ServicingUtils.ReleaseManifest.CurrentServiceLevel;

    public static bool IsUpgradeAllowedForCollectionDatabase(
      ServiceLevel databaseServiceLevel,
      out string message)
    {
      return ServicingUtils.IsUpgradeAllowedForCollectionDatabase(ServicingUtils.ReleaseManifest, databaseServiceLevel, out message);
    }

    public static bool IsUpgradeAllowedForCollectionDatabase(
      ReleaseManifest manifest,
      ServiceLevel databaseServiceLevel,
      out string message)
    {
      return ServicingUtils.IsUpgradeAllowedForDatabase(manifest, databaseServiceLevel, out message);
    }

    public static bool IsUpgradeAllowedForDatabase(
      ServiceLevel databaseServiceLevel,
      out string message)
    {
      return ServicingUtils.IsUpgradeAllowedForDatabase(ServicingUtils.ReleaseManifest, databaseServiceLevel, out message);
    }

    public static bool IsUpgradeAllowedForDatabase(
      ReleaseManifest manifest,
      ServiceLevel databaseServiceLevel,
      out string message)
    {
      if (databaseServiceLevel >= manifest.CurrentServiceLevel)
      {
        message = FrameworkResources.CannotUpgradeFromHigherServiceLevel();
        return false;
      }
      ServiceLevel serviceLevel = new ServiceLevel("Dev14.M83-Part6");
      if (databaseServiceLevel < serviceLevel)
      {
        message = FrameworkResources.DatabaseIsBelowMinimumServiceLevel();
        return false;
      }
      ReleaseInfo release = manifest.FindRelease(databaseServiceLevel);
      if (release != null && !release.IsGoLive && !ServicingUtils.AllowUpgradeFromNonGoLive)
      {
        message = FrameworkResources.CannotUpgradeFromNonGoLiveDatabase();
        return false;
      }
      message = string.Empty;
      return true;
    }

    internal static bool IsHostUpgrade(IVssRequestContext context)
    {
      IVssRequestContext rootContext = context.RootContext;
      object b;
      return rootContext != null && rootContext.IsServicingContext && rootContext.Items != null && rootContext.Items.TryGetValue(RequestContextItemsKeys.ServicingOperationClass, out b) && b is string && string.Equals("UpgradeHost", (string) b);
    }

    private static bool AllowUpgradeFromNonGoLive => Environment.GetEnvironmentVariable("TFSAllowUpgradeFromNonGoLive") != null;

    private static ReleaseManifest ReleaseManifest
    {
      get
      {
        if (ServicingUtils.s_releaseManifest == null)
        {
          if (string.IsNullOrEmpty(VssExtensionManagementService.DefaultPluginPath))
          {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Azure DevOps Server 2022\\Application Tier\\Web Services\\bin\\Plugins");
            if (Directory.Exists(path))
              VssExtensionManagementService.DefaultPluginPath = path;
          }
          string releaseManifestFile;
          using (IDisposableReadOnlyList<IAdminUtilityExtension> extensionsRaw = VssExtensionManagementService.GetExtensionsRaw<IAdminUtilityExtension>(VssExtensionManagementService.DefaultPluginPath))
            releaseManifestFile = extensionsRaw != null && extensionsRaw.Count >= 1 ? extensionsRaw[0].GetReleaseManifestPath() : throw new TeamFoundationExtensionNotFoundException("IAdminUtilityExtension", VssExtensionManagementService.DefaultPluginPath);
          ServicingUtils.s_releaseManifest = ReleaseManifest.LoadFrom(releaseManifestFile);
        }
        return ServicingUtils.s_releaseManifest;
      }
    }

    internal static void ResetReleaseManifest() => ServicingUtils.s_releaseManifest = (ReleaseManifest) null;
  }
}
