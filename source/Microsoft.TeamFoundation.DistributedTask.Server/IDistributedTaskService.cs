// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDistributedTaskService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (IInternalDistributedTaskService))]
  public interface IDistributedTaskService : IVssFrameworkService
  {
    void DeleteTaskDefinition(IVssRequestContext requestContext, Guid taskId);

    Stream GetTaskDefinition(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      out CompressionType compressionType);

    Stream GetTaskDefinitionIcon(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      out CompressionType compressionType);

    IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      IEnumerable<string> visibility = null,
      bool scopeLocal = false,
      bool allVersions = false);

    Task<bool> UploadTaskDefinitionAsync(
      IVssRequestContext requestContext,
      TaskContribution contribution,
      bool isChangedEventCritical = false);

    Task ValidateInstallAsync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionname,
      string version);
  }
}
