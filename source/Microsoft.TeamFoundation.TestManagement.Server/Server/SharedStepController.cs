// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SharedStepController
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "SharedStep", ResourceVersion = 1)]
  public class SharedStepController : TestManagementController
  {
    [HttpDelete]
    [ClientLocationId("FABB3CC9-E3F8-40B7-8B62-24CC4B73FCCF")]
    public void DeleteSharedStep(int sharedStepId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
      if (!TfsRestApiHelper.DoesWorkItemExistInExpectedCategory(this.TestManagementRequestContext, sharedStepId, this.ProjectInfo.Name, "Microsoft.SharedStepCategory"))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) sharedStepId, (object) "Microsoft.SharedStepCategory"));
      if (TfsRestApiHelper.CheckTestCaseWorkItemLink(this.TestManagementRequestContext, sharedStepId, this.ProjectInfo.Name))
        throw new TestManagementInvalidOperationException(ServerResources.TestCaseReferenceError);
      new PlannedTestsObjectHelper().CheckWorkItemDeletePermission(this.ProjectInfo.Name, this.TestManagementRequestContext);
      IVssRequestContext requestContext = this.TestManagementRequestContext.RequestContext;
      service.DeleteWorkItem(requestContext, new List<int>()
      {
        sharedStepId
      });
    }
  }
}
