// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExtensibilityController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ExtensionFields", ResourceVersion = 1)]
  public class TestExtensibilityController : TestResultsControllerBase
  {
    private ITestExtensibilityHelper _testExtensibilityHelper;

    [HttpPost]
    [ClientLocationId("8CE1923B-F4C7-4E22-B93B-F6284E525EC2")]
    public IList<CustomTestFieldDefinition> AddCustomFields(CustomTestFieldDefinition[] newFields)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.TestExtensibilityHelper.AddNewFields(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), (IList<CustomTestFieldDefinition>) newFields);
    }

    [HttpGet]
    [ClientLocationId("8CE1923B-F4C7-4E22-B93B-F6284E525EC2")]
    public IList<CustomTestFieldDefinition> QueryCustomFields(CustomTestFieldScope scopeFilter)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      return this.TestExtensibilityHelper.QueryFields(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), scopeFilter);
    }

    internal ITestExtensibilityHelper TestExtensibilityHelper
    {
      get
      {
        if (this._testExtensibilityHelper == null)
          this._testExtensibilityHelper = (ITestExtensibilityHelper) new Microsoft.TeamFoundation.TestManagement.Server.TestExtensibilityHelper(this.TestManagementRequestContext);
        return this._testExtensibilityHelper;
      }
      set => this._testExtensibilityHelper = value;
    }
  }
}
