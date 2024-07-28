// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.SmtpActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class SmtpActionTask : ActionTask
  {
    public SmtpMailRequest SmtpMailRequest { get; private set; }

    public SmtpActionTask(SmtpMailRequest smtpMailMessage)
    {
      ArgumentUtility.CheckForNull<SmtpMailRequest>(smtpMailMessage, nameof (smtpMailMessage));
      this.SmtpMailRequest = smtpMailMessage;
    }

    public override async Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      SmtpActionTask smtpActionTask = this;
      smtpActionTask.UpdateNotificationForRequest(requestContext, smtpActionTask.SmtpMailRequest.ToString());
      Stopwatch sw = new Stopwatch();
      try
      {
        SmtpClient smtpClient;
        using (smtpClient = new SmtpClient(smtpActionTask.SmtpMailRequest.Host, smtpActionTask.SmtpMailRequest.HostPort))
        {
          smtpClient.EnableSsl = smtpActionTask.SmtpMailRequest.EnableSsl;
          smtpClient.Timeout = (int) timeout.TotalMilliseconds;
          smtpClient.Credentials = smtpActionTask.SmtpMailRequest.Credentials;
          sw.Start();
          try
          {
            await smtpClient.SendMailAsync(smtpActionTask.SmtpMailRequest.MailMessage);
          }
          finally
          {
            sw.Stop();
          }
        }
        smtpActionTask.UpdateNotificationForResponse(requestContext, ServiceHooksWebApiResources.Response_OK(), new double?(sw.Elapsed.TotalSeconds));
      }
      catch (Exception ex)
      {
        smtpActionTask.UpdateNotificationForResponse(requestContext, ServiceHooksWebApiResources.Response_Error(), new double?(sw.Elapsed.TotalSeconds), ex.Message, ex.ToString());
        return new ActionTaskResult(ActionTaskResultLevel.EnduringFailure, ex, ex.Message);
      }
      return new ActionTaskResult(ActionTaskResultLevel.Success);
    }
  }
}
