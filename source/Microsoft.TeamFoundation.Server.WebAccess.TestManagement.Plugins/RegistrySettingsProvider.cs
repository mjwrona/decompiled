// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.RegistrySettingsProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class RegistrySettingsProvider : IExtensionDataProvider
  {
    private static readonly IList<RegistrySettingsProvider.Setting> Settings = (IList<RegistrySettingsProvider.Setting>) new ReadOnlyCollection<RegistrySettingsProvider.Setting>((IList<RegistrySettingsProvider.Setting>) new List<RegistrySettingsProvider.Setting>()
    {
      new RegistrySettingsProvider.Setting()
      {
        Key = "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold",
        DefaultValue = (string) null
      },
      new RegistrySettingsProvider.Setting()
      {
        Key = "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus",
        DefaultValue = (string) null
      },
      new RegistrySettingsProvider.Setting()
      {
        Key = "/Service/WebAccess/TestManagement/Settings/ResultsFetchBatchSize",
        DefaultValue = 20000.ToString()
      },
      new RegistrySettingsProvider.Setting()
      {
        Key = "/Service/WebAccess/TestManagement/Settings/TotalResultsToFetch",
        DefaultValue = 350000.ToString()
      }
    });

    public string Name => "TestManagement.Provider.RegistrySettingsProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      return (object) RegistrySettingsProvider.Settings.ToDictionary<RegistrySettingsProvider.Setting, string, string>((Func<RegistrySettingsProvider.Setting, string>) (setting => setting.Key), (Func<RegistrySettingsProvider.Setting, string>) (setting => registryService.GetValue(requestContext, (RegistryQuery) setting.Key, setting.DefaultValue)));
    }

    private class Setting
    {
      public string Key { get; set; }

      public string DefaultValue { get; set; }
    }
  }
}
