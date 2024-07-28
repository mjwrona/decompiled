// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestHubUserSettingsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestHubUserSettingsHelper : TestManagementUserSettingsHelper
  {
    private const string c_selectedSuiteIdKey = "SelectedSuiteId";
    private const string c_selectedPlanIdKey = "SelectedPlanId";
    private const string c_testHubMoniker = "TestHub";

    internal TestHubUserSettingsHelper(
      IVssRequestContext requestContext,
      string projectName,
      string teamIdentityString)
      : base(requestContext, projectName, teamIdentityString)
    {
    }

    public int SelectedPlanId
    {
      get
      {
        int result = 0;
        string propertyValue = this.GetPropertyValue(nameof (SelectedPlanId));
        return string.IsNullOrEmpty(propertyValue) || !int.TryParse(propertyValue, out result) ? 0 : result;
      }
    }

    public int SelectedSuiteId
    {
      get
      {
        int result = 0;
        string propertyValue = this.GetPropertyValue(nameof (SelectedSuiteId));
        return string.IsNullOrEmpty(propertyValue) || !int.TryParse(propertyValue, out result) ? 0 : result;
      }
    }

    protected override string GetMoniker() => "TestHub";
  }
}
