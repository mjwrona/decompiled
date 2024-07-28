// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GvfsUtil
  {
    public const string LastPackTimestampParameterName = "lastPackTimestamp";

    public static long? GetLatestPackTimestamp(HttpRequestBase request, string area)
    {
      string s = request.Params["lastPackTimestamp"];
      if (s == null)
        return new long?();
      long result;
      if (!long.TryParse(s, out result))
        throw new ArgumentException("Invalid Timestamp in the URI.").Expected(area);
      return new long?(result);
    }
  }
}
