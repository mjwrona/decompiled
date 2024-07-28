// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.ConfiguredAsyncDisposable
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
  [StructLayout(LayoutKind.Auto)]
  public readonly struct ConfiguredAsyncDisposable
  {
    private readonly IAsyncDisposable _source;
    private readonly bool _continueOnCapturedContext;

    internal ConfiguredAsyncDisposable(IAsyncDisposable source, bool continueOnCapturedContext)
    {
      this._source = source;
      this._continueOnCapturedContext = continueOnCapturedContext;
    }

    public ConfiguredValueTaskAwaitable DisposeAsync() => this._source.DisposeAsync().ConfigureAwait(this._continueOnCapturedContext);
  }
}
