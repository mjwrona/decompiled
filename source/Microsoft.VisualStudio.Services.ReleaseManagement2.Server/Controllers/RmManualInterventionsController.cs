// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmManualInterventionsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "manualInterventions")]
  public class RmManualInterventionsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [ClientExample("GET__GetManualIntervention.json", "Get manual intervention", null, null)]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention GetManualIntervention(
      int releaseId,
      int manualInterventionId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention webApi = this.TfsRequestContext.GetService<ManualInterventionsService>().GetManualIntervention(this.TfsRequestContext, this.ProjectId, releaseId, manualInterventionId).ToWebApi(this.TfsRequestContext, this.ProjectId);
      ManualInterventionIdentityHandler.PopulateIdentities(webApi, this.TfsRequestContext);
      return webApi;
    }

    [HttpGet]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    [ClientExample("GET__GetManualInterventions.json", "Get manual interventions", null, null)]
    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention> GetManualInterventions(
      int releaseId)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention> list = this.TfsRequestContext.GetService<ManualInterventionsService>().GetManualInterventionsForRelease(this.TfsRequestContext, this.ProjectId, releaseId).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>) (mi => mi.ToWebApi(this.TfsRequestContext, this.ProjectId))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>();
      ManualInterventionIdentityHandler.PopulateIdentities((IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>) list, this.TfsRequestContext);
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>) list;
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageDeployments)]
    [ClientExample("PATCH__ResumeManualIntervention.json", "Resume manual intervention", null, null)]
    [ClientExample("PATCH__RejectManualIntervention.json", "Reject manual intervention", null, null)]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention UpdateManualIntervention(
      int releaseId,
      int manualInterventionId,
      ManualInterventionUpdateMetadata manualInterventionUpdateMetadata)
    {
      ManualInterventionsService service = this.TfsRequestContext.GetService<ManualInterventionsService>();
      if (service.GetManualIntervention(this.TfsRequestContext, this.ProjectId, releaseId, manualInterventionId).Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ManualInterventionStatus.Pending)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CannotUpdateManualIntervention, (object) manualInterventionId));
      if (manualInterventionUpdateMetadata == null || manualInterventionUpdateMetadata.Status != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualInterventionStatus.Rejected && manualInterventionUpdateMetadata.Status != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualInterventionStatus.Approved)
        throw new InvalidRequestException(Resources.OnlyAllowedToAcceptOrRejectManualIntervention);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention webApi = service.UpdateManualIntervention(this.TfsRequestContext, this.ProjectId, releaseId, manualInterventionId, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ManualInterventionStatus) manualInterventionUpdateMetadata.Status, this.TfsRequestContext.GetUserId(true), manualInterventionUpdateMetadata.Comment, string.Empty).ToWebApi(this.TfsRequestContext, this.ProjectId);
      ManualInterventionIdentityHandler.PopulateIdentities(webApi, this.TfsRequestContext);
      return webApi;
    }
  }
}
