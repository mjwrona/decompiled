// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RMSummaryMailController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "sendmail")]
  public class RMSummaryMailController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public IEnumerable<SummaryMailSection> GetSummaryMailSections(int releaseId)
    {
      IList<MailSectionType> summaryMailSections = SummaryMailHelper.GetAllSummaryMailSections();
      return (IEnumerable<SummaryMailSection>) this.TfsRequestContext.GetService<SummaryMailService>().GetReleaseSummaryMailSections(this.TfsRequestContext, this.ProjectInfo, releaseId, summaryMailSections).ToList<SummaryMailSection>();
    }

    [HttpPost]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public void SendSummaryMail(int releaseId, [FromBody] MailMessage mailMessage) => this.TfsRequestContext.GetService<SummaryMailService>().SendReleaseSummaryMail(this.TfsRequestContext, this.ProjectInfo, releaseId, mailMessage);
  }
}
