// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssTaskDispatcherExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VssTaskDispatcherExtensions
  {
    public static void Run(
      this IVssTaskDispatcher taskDispatcher,
      IVssRequestContext requestContext,
      string taskName,
      Func<IVssRequestContext, Task> function,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      taskDispatcher.RunAsync(requestContext, taskName, function, cancellationToken);
    }
  }
}
