// Decompiled with JetBrains decompiler
// Type: System.Threading.Tasks.TaskAsyncEnumerableExtensions
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;


#nullable enable
namespace System.Threading.Tasks
{
  public static class TaskAsyncEnumerableExtensions
  {
    public static ConfiguredAsyncDisposable ConfigureAwait(
      this IAsyncDisposable source,
      bool continueOnCapturedContext)
    {
      return new ConfiguredAsyncDisposable(source, continueOnCapturedContext);
    }

    public static ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait<T>(
      this IAsyncEnumerable<T> source,
      bool continueOnCapturedContext)
    {
      return new ConfiguredCancelableAsyncEnumerable<T>(source, continueOnCapturedContext, new CancellationToken());
    }

    public static ConfiguredCancelableAsyncEnumerable<T> WithCancellation<T>(
      this IAsyncEnumerable<T> source,
      CancellationToken cancellationToken)
    {
      return new ConfiguredCancelableAsyncEnumerable<T>(source, true, cancellationToken);
    }
  }
}
