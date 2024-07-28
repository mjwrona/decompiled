// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.NameValueCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Compliance
{
  public static class NameValueCollectionExtensions
  {
    public static string ToQuery(this NameValueCollection query, string prefix = null) => query != null && query.Count != 0 ? (prefix ?? string.Empty) + string.Join("&", ((IEnumerable<string>) query.AllKeys).Select<string, string>((Func<string, string>) (key => HttpUtility.UrlEncode(key) + (object) '=' + Uri.EscapeDataString(query[key])))) : string.Empty;
  }
}
