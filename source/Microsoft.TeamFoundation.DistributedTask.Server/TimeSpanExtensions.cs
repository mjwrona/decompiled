// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TimeSpanExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class TimeSpanExtensions
  {
    public static string GetPrettyString(this TimeSpan span)
    {
      if (span.Days > 0)
        return string.Format("{0} days", (object) span.Days);
      return span.Hours > 0 ? string.Format("{0} hours", (object) span.Days) : string.Format("{0} minutes", (object) span.Minutes);
    }
  }
}
