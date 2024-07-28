// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestSettingsContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestSettingsContractConverter
  {
    public static TestSettings Convert(LegacyTestSettings testSettings)
    {
      if (testSettings == null)
        return (TestSettings) null;
      TestSettings testSettings1 = new TestSettings();
      testSettings1.Id = testSettings.Id;
      testSettings1.Name = testSettings.Name;
      testSettings1.Description = testSettings.Description;
      testSettings1.CreatedBy = testSettings.CreatedBy;
      testSettings1.CreatedByName = testSettings.CreatedByName;
      testSettings1.CreatedDate = testSettings.CreatedDate;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole> source = TestSettingMachineRoleConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole>) testSettings.MachineRoles);
      testSettings1.MachineRoles = source != null ? source.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>() : (Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole[]) null;
      testSettings1.IsPublic = testSettings.IsPublic;
      testSettings1.IsAutomated = testSettings.IsAutomated;
      testSettings1.Settings = testSettings.Settings;
      testSettings1.AreaPath = testSettings.AreaPath;
      testSettings1.AreaId = testSettings.AreaId;
      testSettings1.LastUpdatedBy = testSettings.LastUpdatedBy;
      testSettings1.LastUpdatedByName = testSettings.LastUpdatedByName;
      testSettings1.LastUpdated = testSettings.LastUpdated;
      testSettings1.Revision = testSettings.Revision;
      testSettings1.TeamProjectUri = testSettings.TeamProjectUri;
      return testSettings1;
    }

    public static LegacyTestSettings Convert(TestSettings testSettings)
    {
      if (testSettings == null)
        return (LegacyTestSettings) null;
      LegacyTestSettings legacyTestSettings = new LegacyTestSettings();
      legacyTestSettings.Id = testSettings.Id;
      legacyTestSettings.Name = testSettings.Name;
      legacyTestSettings.Description = testSettings.Description;
      legacyTestSettings.CreatedBy = testSettings.CreatedBy;
      legacyTestSettings.CreatedByName = testSettings.CreatedByName;
      legacyTestSettings.CreatedDate = testSettings.CreatedDate;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole> source = TestSettingMachineRoleConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>) testSettings.MachineRoles);
      legacyTestSettings.MachineRoles = source != null ? source.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole[]) null;
      legacyTestSettings.IsPublic = testSettings.IsPublic;
      legacyTestSettings.IsAutomated = testSettings.IsAutomated;
      legacyTestSettings.Settings = testSettings.Settings;
      legacyTestSettings.AreaPath = testSettings.AreaPath;
      legacyTestSettings.AreaId = testSettings.AreaId;
      legacyTestSettings.LastUpdatedBy = testSettings.LastUpdatedBy;
      legacyTestSettings.LastUpdatedByName = testSettings.LastUpdatedByName;
      legacyTestSettings.LastUpdated = testSettings.LastUpdated;
      legacyTestSettings.Revision = testSettings.Revision;
      legacyTestSettings.TeamProjectUri = testSettings.TeamProjectUri;
      return legacyTestSettings;
    }
  }
}
