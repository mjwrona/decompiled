// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.BaseNotificationEmailData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Resources;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public abstract class BaseNotificationEmailData : INotificationEmailData
  {
    protected const string EmailOpenedEventTrackingHtml = "<img src=\"{0}\" width=\"1px\" height=\"1px\">";
    private const string s_tokenOpen = "%%##";
    private const string s_tokenClose = "##%%";

    protected BaseNotificationEmailData()
    {
      this.EmailId = Guid.NewGuid();
      this.Attributes = new Dictionary<string, string>();
      this.EmailPriority = MailPriority.Normal;
      this.TokenPrefix = "%%##";
      this.TokenPostfix = "##%%";
      this.TokenFormatter = new TokenFormatter((INotificationEmailData) this);
    }

    public Guid EmailId { get; }

    public virtual string EmailVariation => string.Empty;

    public abstract string EmailType();

    public Dictionary<string, string> Attributes { get; protected set; }

    public EmailTemplateHeaderType HeaderType { get; set; }

    public EmailTemplateFooterType FooterType { get; set; }

    public string Body { get; set; }

    public string Subject { get; set; }

    public MailPriority EmailPriority { get; set; }

    public string TokenPrefix { get; protected set; }

    public string TokenPostfix { get; protected set; }

    public bool IsHtml { get; protected set; }

    protected TokenFormatter TokenFormatter { get; }

    public void AddLocalizedContent(ResourceManager resouces, CultureInfo culture = null)
    {
      if (string.IsNullOrWhiteSpace(this.Body))
        throw new EmailNotificationInvalidNotificationDataException(EmailNotificationResources.EmailBodyCannotBeNullOrWhitespace());
      if (resouces == null)
        throw new EmailNotificationArgumentException(EmailNotificationResources.ResourceManagerCannotBeNull());
      if (culture == null)
        culture = CultureInfo.CurrentCulture;
      foreach (string str1 in CommonUtil.GetTokensFromTemplate(this.Body, "%%##", "##%%"))
      {
        if (!this.Attributes.ContainsKey(str1))
        {
          string str2 = resouces.GetString(str1, culture);
          if (!string.IsNullOrWhiteSpace(str2))
            this.Attributes.Add(str1, str2);
        }
      }
    }

    public string Tokenize(string tokenNameRaw) => CommonUtil.GenerateHtmlToken(tokenNameRaw, "%%##", "##%%");

    protected string WrapTrackingUrl(
      IVssRequestContext requestContext,
      string url,
      string interactionId)
    {
      EmailInteraction interaction = new EmailInteraction()
      {
        EmailId = this.EmailId,
        EmailType = this.EmailType(),
        EmailVariation = this.EmailVariation,
        InteractionId = interactionId,
        InteractionType = InteractionType.Click
      };
      Uri redirection = new Uri(url);
      return new EmailInteractionUriBuilder(requestContext, redirection, interaction).BuildEmailInteractionUri().AbsoluteUri;
    }

    protected string GetEmailOpenedEventTrackingHtml(IVssRequestContext requestContext)
    {
      EmailInteraction interaction = new EmailInteraction()
      {
        EmailId = this.EmailId,
        EmailType = this.EmailType(),
        EmailVariation = this.EmailVariation,
        InteractionId = "",
        InteractionType = InteractionType.EmailOpened
      };
      return string.Format("<img src=\"{0}\" width=\"1px\" height=\"1px\">", (object) new EmailInteractionUriBuilder(requestContext, (Uri) null, interaction).BuildEmailInteractionUri().AbsoluteUri);
    }
  }
}
