// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.StakeholderLicensingHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Organization;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class StakeholderLicensingHelper
  {
    public static bool HasStakeholderLicenseCheckBypassRequestItem(IVssRequestContext requestContext)
    {
      bool flag;
      return requestContext.Items.TryGetValue<bool>("RequestContextItemKeyForStakeholderLicenseCheckBypass", out flag) && flag;
    }

    public static bool EnableStakeholderLicenseCheck(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled(LicensingFeatureFlags.DisableFrameworkLicenseCheckFeatureName) && !StakeholderLicensingHelper.HasStakeholderLicenseCheckBypassRequestItem(requestContext);

    public static bool IsBuildAndReleaseEnabledForStakeholders(IVssRequestContext requestContext)
    {
      if (requestContext.IsPublicResourceLicense())
        return true;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || requestContext.IsFeatureEnabled(LicensingFeatureFlags.DisableBuildAndReleaseForStakeholders))
        return false;
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(requestContext.ServiceHost.Description);
      if (serviceHostTags != ServiceHostTags.EmptyServiceHostTags && serviceHostTags.HasTag(WellKnownServiceHostTags.NoOrgMetadata))
        return false;
      Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[1]
      {
        "SystemProperty.AllowStakeholdersToUseBuildAndRelease"
      });
      return collection != null && collection.Properties.GetValue<bool>("SystemProperty.AllowStakeholdersToUseBuildAndRelease", false);
    }

    public static void SetAllowStakeholdersToUseBuildAndReleaseSetting(
      IVssRequestContext requestContext,
      bool enabled)
    {
      requestContext.CheckProjectCollectionRequestContext();
      ICollectionService service = requestContext.GetService<ICollectionService>();
      PropertyBag propertyBag = new PropertyBag();
      propertyBag["SystemProperty.AllowStakeholdersToUseBuildAndRelease"] = (object) enabled.ToString();
      IVssRequestContext context = requestContext;
      PropertyBag properties = propertyBag;
      service.UpdateProperties(context, properties);
    }
  }
}
