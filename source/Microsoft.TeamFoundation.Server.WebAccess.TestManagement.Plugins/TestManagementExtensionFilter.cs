// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestManagementExtensionFilter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestManagementExtensionFilter : ContributionFilter<TestManagementExtensionFilterProps>
  {
    public override string Name => "TestManagementExtension";

    public override TestManagementExtensionFilterProps ParseProperties(JObject properties)
    {
      TestManagementExtensionFilterProps properties1 = new TestManagementExtensionFilterProps();
      bool flag;
      if (!properties.TryGetValue<bool>("installed", out flag))
        throw new ArgumentException("Missing required 'installed' property", "installed");
      properties1.Installed = flag;
      return properties1;
    }

    public override bool ApplyConstraint(
      IVssRequestContext requestContext,
      TestManagementExtensionFilterProps properties,
      Contribution contribution)
    {
      bool flag = LicenseCheckHelper.IsAdvancedTestExtensionEnabled(requestContext);
      return properties.Installed == flag;
    }
  }
}
