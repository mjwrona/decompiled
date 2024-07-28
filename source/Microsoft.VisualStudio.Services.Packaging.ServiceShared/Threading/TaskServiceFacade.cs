// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Threading.TaskServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Threading
{
  public class TaskServiceFacade : ITaskService
  {
    public TaskServiceFacade(IVssRequestContext requestContext)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CrequestContext\u003EP = requestContext;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public async Task RunAsync(
      string actionName,
      Func<IVssRequestContext, Task> action,
      CancellationToken cancellationToken)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await this.\u003CrequestContext\u003EP.GetService<IVssTaskService>().RunWithDetachedCancellationAsync(this.\u003CrequestContext\u003EP, actionName, action, cancellationToken);
    }
  }
}
