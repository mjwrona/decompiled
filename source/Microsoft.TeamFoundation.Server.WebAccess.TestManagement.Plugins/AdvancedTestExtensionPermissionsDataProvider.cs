// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.AdvancedTestExtensionPermissionsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  internal class AdvancedTestExtensionPermissionsDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.AdvancedTestExtensionPermissions";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      return (object) new
      {
        isAdvancedTestExtensionEnabled = this.IsAdvancedExtensionEnabled(requestContext)
      };
    }

    private bool IsAdvancedExtensionEnabled(IVssRequestContext requestContext) => LicenseCheckHelper.IsAdvancedTestExtensionEnabled(requestContext) || requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().IsFeatureSupported(requestContext, LicenseFeatures.TestManagementId);
  }
}
