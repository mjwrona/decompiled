// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  internal class HostMigrationLogger
  {
    public static void CreateServicingJobLogInfo(
      IVssRequestContext requestContext,
      SourceHostMigration migrationRequest)
    {
      HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationRequest.MigrationId, migrationRequest.HostProperties.Id, new string[1]
      {
        "SourceAccountMigration"
      }, ServicingJobStatus.Running);
    }

    public static void CreateServicingJobLogInfo(
      IVssRequestContext requestContext,
      TargetHostMigration migrationRequest)
    {
      HostMigrationLogger.CreateServicingJobLogInfo(requestContext, migrationRequest.MigrationId, migrationRequest.HostProperties.Id, new string[1]
      {
        "TargetAccountMigration"
      }, ServicingJobStatus.Running);
    }

    public static void FinishServicingJob(
      IVssRequestContext requestContext,
      SourceHostMigration migrationRequest,
      ServicingJobStatus status,
      ServicingJobResult result)
    {
      HostMigrationLogger.FinishServicingJob(requestContext, migrationRequest.MigrationId, migrationRequest.HostId, new string[1]
      {
        "SourceAccountMigration"
      }, status, result);
    }

    public static void FinishServicingJob(
      IVssRequestContext requestContext,
      TargetHostMigration migrationRequest,
      ServicingJobStatus status,
      ServicingJobResult result)
    {
      HostMigrationLogger.FinishServicingJob(requestContext, migrationRequest.MigrationId, migrationRequest.HostId, new string[1]
      {
        "TargetAccountMigration"
      }, status, result);
    }

    private static void CreateServicingJobLogInfo(
      IVssRequestContext requestContext,
      Guid migrationId,
      Guid hostId,
      string[] operations,
      ServicingJobStatus status)
    {
      requestContext.GetService<TeamFoundationTracingService>().TraceServicingJobDetail(new ServicingJobDetail()
      {
        HostId = hostId,
        JobId = migrationId,
        JobStatus = status,
        OperationClass = "MigrateAccount",
        Operations = operations,
        QueueTime = DateTime.UtcNow
      });
    }

    private static void FinishServicingJob(
      IVssRequestContext requestContext,
      Guid migrationId,
      Guid hostId,
      string[] operations,
      ServicingJobStatus status,
      ServicingJobResult result)
    {
      requestContext.GetService<TeamFoundationTracingService>().TraceServicingJobDetail(new ServicingJobDetail()
      {
        HostId = hostId,
        JobId = migrationId,
        JobStatus = status,
        OperationClass = "MigrateAccount",
        Operations = operations,
        EndTime = DateTime.UtcNow,
        Result = result
      });
    }

    public static void LogInfo(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      string operation,
      string message,
      ServicingStepLogEntryKind entryKind = ServicingStepLogEntryKind.Informational)
    {
      HostMigrationLogger.LogInfo(requestContext, migrationEntry.MigrationId, migrationEntry.CreatedDate, operation, message, entryKind);
    }

    public static void LogInfo(
      IVssRequestContext requestContext,
      Guid migrationId,
      DateTime createdDate,
      string operation,
      string message,
      ServicingStepLogEntryKind entryKind = ServicingStepLogEntryKind.Informational)
    {
      requestContext.CheckDeploymentRequestContext();
      TeamFoundationTracingService service = requestContext.GetService<TeamFoundationTracingService>();
      ServicingStepLogEntry servicingStepLogEntry1 = new ServicingStepLogEntry();
      servicingStepLogEntry1.DetailTime = DateTime.UtcNow;
      servicingStepLogEntry1.ServicingOperation = operation;
      servicingStepLogEntry1.Message = message;
      servicingStepLogEntry1.EntryKind = entryKind;
      ServicingStepLogEntry servicingStepLogEntry2 = servicingStepLogEntry1;
      Guid jobId = migrationId;
      DateTime queueTime = createdDate;
      ServicingStepLogEntry stepDetail = servicingStepLogEntry2;
      service.TraceServicingStepDetail(jobId, queueTime, (ServicingStepDetail) stepDetail);
    }
  }
}
