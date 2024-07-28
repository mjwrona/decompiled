// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.IPipelinesLogService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (PipelinesLogService))]
  public interface IPipelinesLogService : IVssFrameworkService
  {
    TaskLog GetLog(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId);

    Action<Stream> GetLogContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId,
      long startLine = 0,
      long endLine = 9223372036854775807);

    IEnumerable<TaskLog> GetLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);

    Action<Stream> GetZippedLogContents(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);
  }
}
