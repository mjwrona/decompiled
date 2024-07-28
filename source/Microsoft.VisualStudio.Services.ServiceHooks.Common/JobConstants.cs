// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.JobConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class JobConstants
  {
    public static readonly Guid DashboardSyncJobId = new Guid("B201BF58-09E1-4136-AED1-6F3B17EF9F8F");
    public static readonly Guid DeleteEmptyAccountsJobId = new Guid("97864C58-EB55-4E91-9B24-45696294C617");
    public static readonly Guid NotificationDetailsCleanupJobId = new Guid("016E5665-7F57-4285-A40A-0570DCF0117C");
    public static readonly Guid NotificationsCatchupJobId = new Guid("C47A95F4-128E-43B8-8CC8-EF73C40C84ED");
  }
}
