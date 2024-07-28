// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.AsyncIteratorMethodBuilder
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Runtime.InteropServices;
using System.Threading;


#nullable enable
namespace System.Runtime.CompilerServices
{
  [StructLayout(LayoutKind.Auto)]
  public struct AsyncIteratorMethodBuilder
  {
    private AsyncTaskMethodBuilder _methodBuilder;

    #nullable disable
    private object _id;

    public static AsyncIteratorMethodBuilder Create() => new AsyncIteratorMethodBuilder()
    {
      _methodBuilder = AsyncTaskMethodBuilder.Create()
    };


    #nullable enable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MoveNext<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => this._methodBuilder.Start<TStateMachine>(ref stateMachine);

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
      ref TAwaiter awaiter,
      ref TStateMachine stateMachine)
      where TAwaiter : INotifyCompletion
      where TStateMachine : IAsyncStateMachine
    {
      this._methodBuilder.AwaitOnCompleted<TAwaiter, TStateMachine>(ref awaiter, ref stateMachine);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
      ref TAwaiter awaiter,
      ref TStateMachine stateMachine)
      where TAwaiter : ICriticalNotifyCompletion
      where TStateMachine : IAsyncStateMachine
    {
      this._methodBuilder.AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref awaiter, ref stateMachine);
    }

    public void Complete() => this._methodBuilder.SetResult();

    internal object ObjectIdForDebugger => this._id ?? Interlocked.CompareExchange(ref this._id, new object(), (object) null) ?? this._id;
  }
}
