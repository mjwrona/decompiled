// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeMetricsUtil
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public static class CodeMetricsUtil
  {
    public static readonly DateTime UtcEpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static int CurrentTimeBucket => (int) DateTime.UtcNow.Subtract(CodeMetricsUtil.UtcEpochDateTime).TotalHours;

    public static DateTime ConvertTimeBucketToDateTime(int timeBucket) => CodeMetricsUtil.UtcEpochDateTime.AddHours((double) timeBucket);

    public static int ConvertToBucket(DateTime dateTime) => (int) dateTime.ToUniversalTime().Subtract(CodeMetricsUtil.UtcEpochDateTime).TotalHours;
  }
}
