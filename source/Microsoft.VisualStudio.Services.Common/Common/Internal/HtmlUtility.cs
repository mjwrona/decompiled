// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.HtmlUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HtmlUtility
  {
    public static string CreateAutoSubmitForm(
      string uriString,
      IDictionary<string, string> inputs,
      string title = null)
    {
      ArgumentUtility.CheckForNull<string>(uriString, nameof (uriString));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(inputs, nameof (inputs));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) inputs)
        stringBuilder.Append("<input type=\"hidden\" name=\"" + input.Key + "\" value=\"" + input.Value + "\" />");
      return "\r\n                <html>\r\n                    <head>\r\n                        <title>" + title + "</title>\r\n                    </head>\r\n                    <body>\r\n                        <form method=\"POST\" name=\"hiddenform\" action=\"" + uriString + "\" >\r\n                            " + stringBuilder.ToString() + "\r\n                            <noscript>\r\n                                <p>Script is disabled. Click Submit to continue.</p>\r\n                                <input type=\"submit\" value=\"Submit\" />\r\n                            </noscript>\r\n                        </form>\r\n                        <script language=\"javascript\">\r\n                            document.forms[0].submit();\r\n                        </script>\r\n                    </body>\r\n                </html>";
    }
  }
}
