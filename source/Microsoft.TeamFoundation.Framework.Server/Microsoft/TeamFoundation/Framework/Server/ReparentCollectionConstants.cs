// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReparentCollectionConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ReparentCollectionConstants
  {
    public const string Area = "ReparentCollection";
    public const string RegistryPath = "/Configuration/ReparentCollection";
    public const string RequestTimeout = "/Configuration/ReparentCollection/RequestTimeout";
    public const string SubStatus = "ReparentCollection.SubStatus";
    public static readonly string JobRetryThreshold = ServicingOrchestrationConstants.JobRetryThreshold("ReparentCollection");
    public static readonly string JobRetryInterval = ServicingOrchestrationConstants.JobRetryInterval("ReparentCollection");

    public static string PatternAll(Guid requestId) => string.Format("{0}/{1}/**", (object) "/Configuration/ReparentCollection", (object) requestId);

    public static string CanRollback(Guid requestId) => string.Format("{0}/{1}/CanRollback", (object) "/Configuration/ReparentCollection", (object) requestId);

    public static string HostAcquired(Guid requestId) => string.Format("{0}/{1}/HostAcquired", (object) "/Configuration/ReparentCollection", (object) requestId);

    public static string RequestStartTime(Guid requestId) => string.Format("{0}/{1}/RequestStartTime", (object) "/Configuration/ReparentCollection", (object) requestId);

    public static string SudoOrganizationId(Guid requestId) => string.Format("{0}/{1}/SudoOrganizationId", (object) "/Configuration/ReparentCollection", (object) requestId);

    public static string OwningRequest(Guid hostId) => string.Format("{0}/{1}/RequestId", (object) "/Configuration/ReparentCollection", (object) hostId);
  }
}
