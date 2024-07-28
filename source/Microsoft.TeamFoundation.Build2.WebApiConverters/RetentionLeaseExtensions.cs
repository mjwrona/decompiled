// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.RetentionLeaseExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class RetentionLeaseExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.RetentionLease ToServerRetentionLease(
      this Microsoft.TeamFoundation.Build.WebApi.RetentionLease lease)
    {
      return new Microsoft.TeamFoundation.Build2.Server.RetentionLease(lease.LeaseId, lease.OwnerId, lease.RunId, lease.DefinitionId, lease.CreatedOn, lease.ValidUntil, lease.ProtectPipeline);
    }

    public static Microsoft.TeamFoundation.Build2.Server.RetentionLease ToServerRetentionLease(
      this NewRetentionLease lease)
    {
      return new Microsoft.TeamFoundation.Build2.Server.RetentionLease(-1, lease.OwnerId, lease.RunId, lease.DefinitionId, new DateTime(), DateTime.UtcNow + TimeSpan.FromDays((double) Math.Min(lease.DaysValid, (int) (DateTime.MaxValue - DateTime.UtcNow).TotalDays)), lease.ProtectPipeline);
    }

    public static Microsoft.TeamFoundation.Build2.Server.MinimalRetentionLease ToServerMinimalRetentionLease(
      this Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease lease)
    {
      return new Microsoft.TeamFoundation.Build2.Server.MinimalRetentionLease(lease.OwnerId, new int?(lease.RunId), new int?(lease.DefinitionId));
    }

    public static Microsoft.TeamFoundation.Build.WebApi.RetentionLease ToWebApiRetentionLease(
      this Microsoft.TeamFoundation.Build2.Server.RetentionLease lease,
      ISecuredObject securedObject)
    {
      return new Microsoft.TeamFoundation.Build.WebApi.RetentionLease(securedObject)
      {
        LeaseId = lease.Id,
        OwnerId = lease.OwnerId,
        RunId = lease.RunId,
        DefinitionId = lease.DefinitionId,
        CreatedOn = lease.CreatedOn,
        ValidUntil = lease.ValidUntil,
        ProtectPipeline = lease.HighPriority
      };
    }
  }
}
