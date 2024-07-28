// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceEmailHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.EmailNotification;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [InheritedExport]
  public interface ICommerceEmailHandler
  {
    void SendEmail(
      IVssRequestContext requestContext,
      INotificationEmailData formatter,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void SendEmail(
      IVssRequestContext requestContext,
      INotificationEmailData formatter,
      MailAddress toAddress);

    void SendEmail(
      IVssRequestContext requestContext,
      INotificationEmailData formatter,
      IList<string> emailAddressList);

    string GetIdentityMailAddress(Microsoft.VisualStudio.Services.Identity.Identity identity);
  }
}
