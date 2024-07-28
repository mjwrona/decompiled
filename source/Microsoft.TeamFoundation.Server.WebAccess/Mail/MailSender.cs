// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Mail.MailSender
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc.Async;

namespace Microsoft.TeamFoundation.Server.WebAccess.Mail
{
  public static class MailSender
  {
    private static string s_traceArea = "WebAccess.Common";
    private const int ApiCommonMailSent = 505020;
    public const string paramSendMailWarning = "sendMailWarning";

    public static JsObject BeginSendMail(
      MailMessage message,
      IVssRequestContext tfsRequestContext,
      bool prohibitExternalEmailAddresses,
      AsyncManager asyncManager)
    {
      JsObject jsObject = new JsObject();
      asyncManager.OutstandingOperations.Increment();
      try
      {
        ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
        List<Guid> toRecipientsWithoutEmail;
        System.Net.Mail.MailMessage mailMessage = message.CreateMailMessage(tfsRequestContext, prohibitExternalEmailAddresses, out toRecipientsWithoutEmail);
        TeamFoundationMailService service1 = tfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();
        service1.ValidateMessage(tfsRequestContext, mailMessage);
        service1.QueueMailJob(tfsRequestContext.To(TeamFoundationHostType.Deployment), mailMessage);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = tfsRequestContext.GetUserIdentity();
        tfsRequestContext.Trace(505020, TraceLevel.Info, MailSender.s_traceArea, TfsTraceLayers.Controller, "Mail sent to {0} users from user '{1}' and message size {2}, unresolved TO recipients count {3} : {4}.", (object) mailMessage.To.Count, (object) userIdentity.DisplayName, (object) (mailMessage.Body == null ? 0 : mailMessage.Body.Length), (object) toRecipientsWithoutEmail.Count, (object) toRecipientsWithoutEmail.StringJoin<Guid>(';'));
        if (toRecipientsWithoutEmail.Count > 0)
        {
          TeamFoundationIdentityService service2 = tfsRequestContext.GetService<TeamFoundationIdentityService>();
          Guid[] array = toRecipientsWithoutEmail.Distinct<Guid>().ToArray<Guid>();
          IVssRequestContext requestContext = tfsRequestContext;
          Guid[] teamFoundationIds = array;
          TeamFoundationIdentity[] source = service2.ReadIdentities(requestContext, teamFoundationIds);
          if (source != null)
          {
            if (source.Length != 0)
            {
              asyncManager.Parameters["sendMailWarning"] = (object) string.Format(WACommonResources.SendMailWarning, (object) ((IEnumerable<TeamFoundationIdentity>) source).Select<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (id => id.DisplayName)).StringJoin<string>(';'));
              jsObject["sendMailWarning"] = (object) string.Format(WACommonResources.SendMailWarning, (object) ((IEnumerable<TeamFoundationIdentity>) source).Select<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (id => id.DisplayName)).StringJoin<string>(';'));
            }
          }
        }
      }
      catch (MailValidationException ex)
      {
        throw ex;
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServiceException(WACommonResources.ErrorSendEmail, ex);
      }
      finally
      {
        asyncManager.OutstandingOperations.Decrement();
      }
      return jsObject;
    }

    public static JsObject SendMailCompleted(AsyncManager asyncManager)
    {
      JsObject jsObject = new JsObject();
      if (asyncManager.Parameters.ContainsKey("sendMailWarning"))
        jsObject["sendMailWarning"] = asyncManager.Parameters["sendMailWarning"];
      return jsObject;
    }
  }
}
