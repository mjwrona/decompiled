// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Threading.TaskServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Threading
{
  public static class TaskServiceExtensions
  {
    public static Task RunAsync<TBootstrapped>(
      this ITaskService taskService,
      string actionName,
      Func<IVssRequestContext, TBootstrapped> bootstrapFunc,
      Func<TBootstrapped, Task> executeFunc,
      CancellationToken cancellationToken)
    {
      return taskService.RunAsync(actionName, (Func<IVssRequestContext, Task>) (async rc => await executeFunc(bootstrapFunc(rc))), cancellationToken);
    }

    public static Task RunAsync<TRequest, TBootstrapped>(
      this ITaskService taskService,
      string actionName,
      Func<IVssRequestContext, TBootstrapped> bootstrapFunc,
      TRequest request,
      Func<TBootstrapped, TRequest, Task> executeFunc,
      CancellationToken cancellationToken)
    {
      return taskService.RunAsync(actionName, (Func<IVssRequestContext, Task>) (async rc => await executeFunc(bootstrapFunc(rc), request)), cancellationToken);
    }
  }
}
