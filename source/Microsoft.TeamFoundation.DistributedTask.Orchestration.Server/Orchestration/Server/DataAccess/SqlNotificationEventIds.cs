// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.SqlNotificationEventIds
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal static class SqlNotificationEventIds
  {
    public static readonly Guid AgentCloudChanged = new Guid("{09462FE9-C2C8-41CB-AB70-159148B3D6B4}");
    public static readonly Guid AgentQueueCreated = new Guid("{A0B8B413-AB7A-4C0F-A09E-6699B6FAA7CF}");
    public static readonly Guid AgentQueueDeleted = new Guid("{CE9BA1B4-F97B-45D4-ABF4-017B196A9A27}");
    public static readonly Guid PoolChanged = new Guid("{24E30C74-C7B2-46FA-A813-69E780890D4C}");
    public static readonly Guid SessionCreated = new Guid("{EDDD3490-A8ED-42E9-858C-37F6F7F8C6D0}");
    public static readonly Guid SessionDeleted = new Guid("{AB5155B6-E990-49CE-A014-49CFFBC34317}");
    public static readonly Guid TasksChanged = new Guid("{86AEE2EF-BCCD-4927-8C4C-A911DF6A9007}");
    public static readonly Guid PackagesChanged = new Guid("{94E06CC1-FBB9-4E9F-A9BF-44D872FE6D61}");
    public static readonly Guid PlanEventSubscriptionAdded = new Guid("{73A2FA88-75E5-4B5B-9E90-6B6F5C0A6BAD}");
    public static readonly Guid PlanEventSubscriptionDeleted = new Guid("{B5700CE2-1C12-49F3-81EC-AA8DF33C67F3}");
    public static readonly Guid AgentQueuesDeleted = new Guid("{16D3C033-97A4-4116-B439-12251DFE9E8E}");
  }
}
