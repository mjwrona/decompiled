// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentViolationController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ContentValidation.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "contentViolation", ResourceName = "reports")]
  [FeatureEnabled("VisualStudio.Services.ReportViolationToAvertService")]
  public class ContentViolationController : TfsApiController
  {
    public override string TraceArea => "contentViolation";

    public override string ActivityLogArea => "Framework";

    [HttpPost]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public async Task<ContentViolationReportResult> ReportAbuse(
      [FromBody] ContentViolationReport violationReport)
    {
      ContentViolationController violationController = this;
      IVssRequestContext vssRequestContext = violationController.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IContentViolationService service = vssRequestContext.GetService<IContentViolationService>();
      if (!service.IsEnabled(violationController.TfsRequestContext))
        throw new VssResourceNotFoundException(HostingResources.ContentViolationServiceNotEnabled());
      return await service.ReportAsync(vssRequestContext, violationReport);
    }
  }
}
