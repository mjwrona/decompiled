// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.TokenFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public class TokenFormatter
  {
    private const string anchorOpen = "<a href=\"{0}\">";
    private const string anchorClose = "</a>";
    private const string boldTag = "<b>{0}</b>";

    public INotificationEmailData Email { get; private set; }

    public TokenFormatter(INotificationEmailData email) => this.Email = email;

    public void Format(string tokenName, params string[] parameters)
    {
      if (string.IsNullOrWhiteSpace(tokenName) || parameters == null)
        throw new EmailNotificationArgumentException(EmailNotificationResources.InvalidToken());
      if (!this.Email.Attributes.ContainsKey(tokenName))
        return;
      this.Email.Attributes[tokenName] = string.Format(this.Email.Attributes[tokenName], (object[]) parameters);
    }

    public void FormatAnchor(string tokenName, string href)
    {
      if (string.IsNullOrWhiteSpace(tokenName) || href == null)
        throw new EmailNotificationArgumentException(EmailNotificationResources.InvalidToken());
      this.Format(tokenName, string.Format("<a href=\"{0}\">", (object) href), "</a>");
    }

    public void FormatAnchor(string tokenName, string[] hrefs)
    {
      if (string.IsNullOrWhiteSpace(tokenName) || hrefs == null)
        throw new EmailNotificationArgumentException(EmailNotificationResources.InvalidToken());
      List<string> stringList = new List<string>();
      foreach (string href in hrefs)
      {
        stringList.Add(string.Format("<a href=\"{0}\">", (object) href));
        stringList.Add(string.Format("</a>"));
      }
      this.Format(tokenName, stringList.ToArray());
    }

    public string WrapWithAnchor(string displayText, string href)
    {
      ArgumentUtility.CheckForNull<string>(displayText, nameof (displayText));
      ArgumentUtility.CheckForNull<string>(href, nameof (href));
      return string.Format("<a href=\"{0}\">", (object) href) + displayText + "</a>";
    }

    public void FormatBold(string tokenName, string boldToken)
    {
      if (string.IsNullOrWhiteSpace(tokenName) || string.IsNullOrWhiteSpace(boldToken))
        throw new EmailNotificationArgumentException(EmailNotificationResources.InvalidToken());
      this.Format(tokenName, string.Format("<b>{0}</b>", (object) boldToken));
    }
  }
}
