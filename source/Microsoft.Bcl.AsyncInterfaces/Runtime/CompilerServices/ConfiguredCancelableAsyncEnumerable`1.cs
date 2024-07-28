// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable`1
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;


#nullable enable
namespace System.Runtime.CompilerServices
{
  [StructLayout(LayoutKind.Auto)]
  public readonly struct ConfiguredCancelableAsyncEnumerable<T>
  {

    #nullable disable
    private readonly IAsyncEnumerable<T> _enumerable;
    private readonly CancellationToken _cancellationToken;
    private readonly bool _continueOnCapturedContext;

    internal ConfiguredCancelableAsyncEnumerable(
      IAsyncEnumerable<T> enumerable,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken)
    {
      this._enumerable = enumerable;
      this._continueOnCapturedContext = continueOnCapturedContext;
      this._cancellationToken = cancellationToken;
    }


    #nullable enable
    public ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait(bool continueOnCapturedContext) => new ConfiguredCancelableAsyncEnumerable<T>(this._enumerable, continueOnCapturedContext, this._cancellationToken);

    public ConfiguredCancelableAsyncEnumerable<T> WithCancellation(
      CancellationToken cancellationToken)
    {
      return new ConfiguredCancelableAsyncEnumerable<T>(this._enumerable, this._continueOnCapturedContext, cancellationToken);
    }

    public ConfiguredCancelableAsyncEnumerable<
    #nullable disable
    T>.Enumerator GetAsyncEnumerator() => new ConfiguredCancelableAsyncEnumerable<T>.Enumerator(this._enumerable.GetAsyncEnumerator(this._cancellationToken), this._continueOnCapturedContext);


    #nullable enable
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Enumerator
    {

      #nullable disable
      private readonly IAsyncEnumerator<T> _enumerator;
      private readonly bool _continueOnCapturedContext;

      internal Enumerator(IAsyncEnumerator<T> enumerator, bool continueOnCapturedContext)
      {
        this._enumerator = enumerator;
        this._continueOnCapturedContext = continueOnCapturedContext;
      }


      #nullable enable
      public ConfiguredValueTaskAwaitable<bool> MoveNextAsync() => this._enumerator.MoveNextAsync().ConfigureAwait(this._continueOnCapturedContext);

      public T Current => this._enumerator.Current;

      public ConfiguredValueTaskAwaitable DisposeAsync() => this._enumerator.DisposeAsync().ConfigureAwait(this._continueOnCapturedContext);
    }
  }
}
