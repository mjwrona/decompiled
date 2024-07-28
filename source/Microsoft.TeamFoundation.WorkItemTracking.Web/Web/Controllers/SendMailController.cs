// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.SendMailController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;
using System.Web.Mvc.Async;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "sendMail")]
  public class SendMailController : WorkItemTrackingApiController
  {
    private AsyncManager AsyncManager = new AsyncManager();

    [HttpPost]
    [ClientLocationId("12438500-2F84-4FA7-9F1A-C31871B4959D")]
    public void SendMailAsync([FromBody] SendMailBody body)
    {
      if (body == null)
        throw new VssPropertyValidationException("SendMailBody", ResourceStrings.NullOrEmptyParameter((object) "SendMailBody"));
      if (WorkItemTrackingFeatureFlags.IsDisableSendMailDataProviderEnabled(this.TfsRequestContext))
        MailSender.BeginSendMail(WITDataSource.ModifySendMailMessage(MailMessageFactory.CreateMailMessage(body.message), this.TfsRequestContext, this.TraceArea, this.ProjectInfo, this.Team, body.ids, body.wiql, body.fields, body.persistenceId, body.tempQueryId, body.sortFields, body.projectId), this.TfsRequestContext, this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment, this.AsyncManager);
      else
        WITDataSource.SendMail(MailMessageFactory.CreateMailMessage(body.message), this.TfsRequestContext, this.AsyncManager, this.TraceArea, this.ProjectInfo, this.Team, this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment, body.ids, body.wiql, body.fields, body.persistenceId, body.tempQueryId, body.sortFields, body.projectId);
    }
  }
}
