// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SharedParameterController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "SharedParameter", ResourceVersion = 1)]
  public class SharedParameterController : TestManagementController
  {
    [HttpDelete]
    [ClientLocationId("8300EECA-0F8C-4EFF-A089-D2DDA409C41F")]
    public void DeleteSharedParameter(int sharedParameterId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
      if (!TfsRestApiHelper.DoesWorkItemExistInExpectedCategory(this.TestManagementRequestContext, sharedParameterId, this.ProjectInfo.Name, "Microsoft.SharedParameterCategory"))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) sharedParameterId, (object) "Microsoft.SharedParameterCategory"));
      if (TfsRestApiHelper.CheckTestCaseWorkItemLink(this.TestManagementRequestContext, sharedParameterId, this.ProjectInfo.Name))
        throw new TestManagementInvalidOperationException(ServerResources.TestCaseReferenceError);
      new PlannedTestsObjectHelper().CheckWorkItemDeletePermission(this.ProjectInfo.Name, this.TestManagementRequestContext);
      IVssRequestContext requestContext = this.TestManagementRequestContext.RequestContext;
      service.DeleteWorkItem(requestContext, new List<int>()
      {
        sharedParameterId
      });
    }
  }
}
