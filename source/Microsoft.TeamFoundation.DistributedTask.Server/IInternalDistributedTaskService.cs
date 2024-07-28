// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IInternalDistributedTaskService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskService))]
  internal interface IInternalDistributedTaskService : IDistributedTaskService, IVssFrameworkService
  {
    [Obsolete("See public version", false)]
    Task AddTaskDefinitionAsync(
      IVssRequestContext requestContext,
      TaskDefinition taskDefinition,
      bool overwrite);

    [Obsolete("See public version", false)]
    Task UploadTaskDefinitionAsync(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      Stream fileStream,
      Stream iconStream,
      long iconStreamLength,
      Stream helpStream,
      long helpStreamLength);
  }
}
