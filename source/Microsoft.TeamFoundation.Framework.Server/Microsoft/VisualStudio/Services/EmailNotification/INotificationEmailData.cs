// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.INotificationEmailData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public interface INotificationEmailData
  {
    string EmailType();

    EmailTemplateHeaderType HeaderType { get; set; }

    EmailTemplateFooterType FooterType { get; set; }

    bool IsHtml { get; }

    string Body { get; set; }

    string Subject { get; set; }

    MailPriority EmailPriority { get; set; }

    string TokenPrefix { get; }

    string TokenPostfix { get; }

    Dictionary<string, string> Attributes { get; }
  }
}
