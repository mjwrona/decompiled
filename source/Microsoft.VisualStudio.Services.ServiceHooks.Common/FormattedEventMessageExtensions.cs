// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.FormattedEventMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Web;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class FormattedEventMessageExtensions
  {
    public static string GetHtml(this FormattedEventMessage message) => !string.IsNullOrEmpty(message.Html) ? message.Html : HttpUtility.HtmlEncode(message.Text);

    public static string GetMarkdown(this FormattedEventMessage message) => !string.IsNullOrEmpty(message.Markdown) ? message.Markdown : message.Text;
  }
}
