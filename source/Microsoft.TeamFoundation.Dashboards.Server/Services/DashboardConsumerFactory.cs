// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DashboardConsumerFactory
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public sealed class DashboardConsumerFactory
  {
    public static IDashboardConsumer CreateDashboardConsumer(
      IVssRequestContext context,
      Guid ProjectId,
      Guid? groupId)
    {
      if (groupId.HasValue)
      {
        Guid? nullable = groupId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return (IDashboardConsumer) new TeamDashboardConsumer(ProjectId, groupId.Value);
      }
      return (IDashboardConsumer) new ProjectDashboardConsumer(ProjectId);
    }

    public static IDashboardConsumer CopyDashboardConsumer(
      IVssRequestContext context,
      Guid projectId,
      Guid? groupId)
    {
      return DashboardConsumerFactory.CreateDashboardConsumer(context, projectId, groupId);
    }
  }
}
