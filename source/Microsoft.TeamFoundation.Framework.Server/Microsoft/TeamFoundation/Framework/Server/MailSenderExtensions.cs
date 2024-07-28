// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailSenderExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class MailSenderExtensions
  {
    public static void SendSynchronously(
      this VssMailSender sender,
      IVssRequestContext requestContext,
      MailMessage message,
      string area,
      string layer,
      Stopwatch stopwatch = null)
    {
      MailSenderMessageContext messageContext = new MailSenderMessageContext()
      {
        Message = message,
        Stopwatch = stopwatch
      };
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      sender.SendCompleted = MailSenderExtensions.\u003C\u003EO.\u003C0\u003E__SynchronousSendCompletedEventHandler ?? (MailSenderExtensions.\u003C\u003EO.\u003C0\u003E__SynchronousSendCompletedEventHandler = new SendCompletedEventHandler(MailSenderExtensions.SynchronousSendCompletedEventHandler));
      try
      {
        requestContext.RunSynchronously((Func<Task>) (() => sender.SendMailAsync(messageContext, requestContext.CancellationToken)));
        if (messageContext.Error != null)
          throw messageContext.Error;
      }
      finally
      {
        sender.Tracer.TracePendingMessages(requestContext, area, layer);
      }
    }

    private static void SynchronousSendCompletedEventHandler(
      object sender,
      AsyncCompletedEventArgs e)
    {
      MailSenderMessageContext userState = e.UserState as MailSenderMessageContext;
      userState.Cancelled = e.Cancelled;
      userState.Error = e.Error;
    }
  }
}
