// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Points5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestPoint", ResourceVersion = 2)]
  public class Points5Controller : TestManagementController
  {
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>), null, null)]
    [ClientLocationId("52df686e-bae4-4334-b0ee-b6cf4e6f6b73")]
    public HttpResponseMessage GetPointsList(
      int planId,
      int suiteId,
      string testPointIds = "",
      string testCaseId = "",
      string continuationToken = null,
      bool returnIdentityRef = false,
      bool includePointDetails = true,
      bool isRecursive = false)
    {
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPointsPaging(0, 0, continuationToken, out skipRows, out topToFetch, out watermark, out topRemaining);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> newPoints = this.RevisedPointsHelper.GetNewPoints(this.ProjectInfo, planId, suiteId, testPointIds, testCaseId, skipRows, topToFetch, watermark, returnIdentityRef, includePointDetails, isRecursive);
      continuationToken = (string) null;
      if (!newPoints.IsNullOrEmpty<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>() && newPoints != null && newPoints.Count >= topToFetch && newPoints[topToFetch - 1] != null)
      {
        continuationToken = Utils.GenerateContinuationToken(newPoints[topToFetch - 1].Id, topRemaining);
        newPoints.RemoveAt(topToFetch - 1);
      }
      HttpResponseMessage response = this.GenerateResponse<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>((IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>) newPoints);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }

    [HttpGet]
    [ClientLocationId("52df686e-bae4-4334-b0ee-b6cf4e6f6b73")]
    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> GetPoints(
      int planId,
      int suiteId,
      string pointId,
      bool returnIdentityRef = false,
      bool includePointDetails = true)
    {
      if (!pointId.IsNullOrEmpty<char>() && this.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.ApiDisallowMultipleIds"))
      {
        if (pointId.Split(',').Length > 1)
          throw new ArgumentException(ServerResources.ApiDisallowMultiplePointIdsMessage);
      }
      return this.RevisedPointsHelper.GetPoints(this.ProjectInfo, planId, suiteId, pointId, returnIdentityRef, includePointDetails);
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientLocationId("52df686e-bae4-4334-b0ee-b6cf4e6f6b73")]
    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> UpdateTestPoints(
      int planId,
      int suiteId,
      List<TestPointUpdateParams> testPointUpdateParams,
      bool includePointDetails = true,
      bool returnIdentityRef = false)
    {
      bool flag = true;
      try
      {
        LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      }
      catch (UnauthorizedAccessException ex)
      {
        flag = false;
      }
      ArgumentUtility.CheckForNull<List<TestPointUpdateParams>>(testPointUpdateParams, "TestPointUpdateParams", this.TfsRequestContext.ServiceName);
      if (flag)
        return this.RevisedPointsHelper.PatchNewTestPoints(this.ProjectInfo, planId, suiteId, testPointUpdateParams, includePointDetails, returnIdentityRef);
      if (testPointUpdateParams.Count > 0 && testPointUpdateParams.ElementAt<TestPointUpdateParams>(0) != null && testPointUpdateParams.ElementAt<TestPointUpdateParams>(0).Tester == null)
        return this.RevisedPointsHelper.PatchNewTestPoints(this.ProjectInfo, planId, suiteId, testPointUpdateParams, includePointDetails, returnIdentityRef);
      throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
    }
  }
}
