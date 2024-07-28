// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskHubLicenseDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskHubLicenseDetails
  {
    public static readonly int DefaultFreeLicenseCount;
    [DataMember(Name = "FreeLicenseCount")]
    public int FreePrivateLicenseCount;
    [DataMember]
    public int FreeHostedLicenseCount;
    [DataMember]
    public int EnterpriseUsersCount;
    [DataMember(Name = "PurchasedLicenseCount")]
    public int PurchasedPrivateLicenseCount;
    [DataMember]
    public int PurchasedHostedLicenseCount;
    [DataMember]
    public bool HostedLicensesArePremium;
    [DataMember]
    public int TotalLicenseCount;
    [DataMember]
    public bool HasLicenseCountEverUpdated;
    [DataMember]
    public int MsdnUsersCount;
    [DataMember]
    public int HostedAgentMinutesFreeCount;
    [DataMember]
    public int HostedAgentMinutesUsedCount;
    [DataMember]
    public bool FailedToReachAllProviders;
    [DataMember]
    public int TotalPrivateLicenseCount;
    [DataMember]
    public int TotalHostedLicenseCount;
  }
}
