// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.CatchupJobConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class CatchupJobConstants
  {
    public const int DefaultBatchSize = 40;
    public static readonly TimeSpan DefaultCatchupDelay = new TimeSpan(0, 20, 0);
    public static readonly TimeSpan DefaultScheduleDelay = new TimeSpan(0, 20, 0);
    public const int DefaultNumDeliveryThreads = 1;
  }
}
