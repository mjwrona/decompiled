// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.LinkUtilities
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal static class LinkUtilities
  {
    public static string ForwardLink(int linkId)
    {
      int? retainableLcid = LinkUtilities.GetRetainableLCID();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://go.microsoft.com/fwlink/?LinkID={0}{1}", (object) linkId, retainableLcid.HasValue ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&clcid={0}", (object) retainableLcid.Value.ToString("X")) : (object) string.Empty);
    }

    private static int? GetRetainableLCID() => LinkUtilities.GetRetainableCulture()?.LCID;

    private static CultureInfo GetRetainableCulture()
    {
      HttpContext current = HttpContext.Current;
      CultureInfo threadUiCulture = current == null ? (CultureInfo) null : RequestLanguage.GetThreadUICulture(current.Items);
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      return threadUiCulture == null || threadUiCulture == currentUiCulture ? (CultureInfo) null : currentUiCulture;
    }
  }
}
