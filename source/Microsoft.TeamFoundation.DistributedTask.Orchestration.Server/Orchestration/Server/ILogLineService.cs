// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ILogLineService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (LogLineService))]
  public interface ILogLineService : IVssFrameworkService
  {
    void CreateLogTable(
      IVssRequestContext requestContext,
      string tableName,
      out string storageAccountKey);

    void DeleteLogTable(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey);

    void InsertLogLines(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      IList<TimelineRecordLogLine> loglines);

    TimelineRecordLogLineResult QueryLogLines(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      string continuationToken,
      long? endLine = null,
      int? takeCount = null);
  }
}
