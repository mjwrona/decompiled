// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.HostCleanup.HostCleanupExtension
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.HostCleanup
{
  public class HostCleanupExtension : IHostCleanupExtension
  {
    public void CleanupHostResources(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      DatabasePartition partition)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(deploymentContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082600, "Host Cleanup", "HostCleanup", nameof (CleanupHostResources));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1082600, TraceLevel.Info, "Host Cleanup", "HostCleanup", FormattableString.Invariant(FormattableStringFactory.Create("Started Executing HostCleanUp for ServiceHost = {0}, PartitionId = {1}, HostType = {2}", (object) partition.ServiceHostId, (object) partition.PartitionId, (object) partition.HostType)));
      if (partition.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      try
      {
        new CollectionHostDeletionHelper(deploymentContext, partition.ServiceHostId).CleanUpElasticsearchAndReindexingTable();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082609, "Host Cleanup", "HostCleanup", nameof (CleanupHostResources));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
