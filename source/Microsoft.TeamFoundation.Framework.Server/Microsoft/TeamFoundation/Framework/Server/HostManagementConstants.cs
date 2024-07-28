// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostManagementConstants
  {
    public const int InvalidStorageAccount = -1;
    public const int VirtualServiceHostStorageAccount = -2;
    public const int VirtualServiceHostDatabaseId = -2;
    internal static readonly Guid HostUpdatedMessagePublisherJobId = new Guid("E83CC2D8-46C5-4618-8AEA-6EACD379E32F");
    public static readonly string HostStatusSyncFeatureFlag = "Microsoft.AzureDevOps.HostManagement.HostStatusSync";
    public const int ServiceHostProcessLeaseCreateMinutes = 5;
    public static readonly TimeSpan ServiceHostProcessLeaseRenewalAT = TimeSpan.FromMinutes(30.0);
    public static readonly TimeSpan ServiceHostProcessLeaseRenewalJA = TimeSpan.FromMinutes(6.0);
    public static readonly TimeSpan ServiceHostProcessLeaseRenewalGeneric = TimeSpan.FromMinutes(6.0);
    public static readonly TimeSpan ServiceHostProcessLeaseRenewalOther = TimeSpan.FromMinutes(15.0);
  }
}
