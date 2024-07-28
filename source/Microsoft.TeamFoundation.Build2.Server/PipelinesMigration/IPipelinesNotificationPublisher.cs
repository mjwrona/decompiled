// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelinesMigration.IPipelinesNotificationPublisher
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build2.Server.PipelinesMigration
{
  [InheritedExport]
  public interface IPipelinesNotificationPublisher
  {
    void PublishRunStateChangedNotification(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildSnapshot);

    void PublishStageStateChangedNotifications(
      IVssRequestContext requestContext,
      IReadOnlyList<TimelineRecord> stageTimelineRecords,
      IReadOnlyBuildData buildSnapshot);

    void PublishJobStateChangedNotifications(
      IVssRequestContext requestContext,
      TimelineRecord jobTimelineRecord,
      IReadOnlyBuildData buildSnapshot,
      Guid scopeIdentifier);
  }
}
