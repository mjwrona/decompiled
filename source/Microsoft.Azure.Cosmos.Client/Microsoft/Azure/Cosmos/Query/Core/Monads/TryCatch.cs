// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Monads.TryCatch
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Monads
{
  internal readonly struct TryCatch
  {
    private readonly TryCatch<TryCatch.Void> voidTryCatch;

    private TryCatch(TryCatch<TryCatch.Void> voidTryCatch) => this.voidTryCatch = voidTryCatch;

    public Exception Exception => this.voidTryCatch.Exception;

    public bool Succeeded => this.voidTryCatch.Succeeded;

    public bool Failed => this.voidTryCatch.Failed;

    public void Match(Action onSuccess, Action<Exception> onError) => this.voidTryCatch.Match((Action<TryCatch.Void>) (dummy => onSuccess()), onError);

    public TryCatch Try(Action onSuccess) => new TryCatch(this.voidTryCatch.Try((Action<TryCatch.Void>) (dummy => onSuccess())));

    public TryCatch<T> Try<T>(Func<T> onSuccess) => this.voidTryCatch.Try<T>((Func<TryCatch.Void, T>) (dummy => onSuccess()));

    public Task<TryCatch<T>> TryAsync<T>(Func<Task<T>> onSuccess) => this.voidTryCatch.TryAsync<T>((Func<TryCatch.Void, Task<T>>) (dummy => onSuccess()));

    public TryCatch Catch(Action<Exception> onError) => new TryCatch(this.voidTryCatch.Catch(onError));

    public async Task<TryCatch> CatchAsync(Func<Exception, Task> onError) => new TryCatch(await this.voidTryCatch.CatchAsync(onError));

    public void ThrowIfFailed() => this.voidTryCatch.ThrowIfFailed();

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is TryCatch other && this.Equals(other);
    }

    public bool Equals(TryCatch other) => this.voidTryCatch.Equals(other.voidTryCatch);

    public override int GetHashCode() => this.voidTryCatch.GetHashCode();

    public static TryCatch FromResult() => new TryCatch(TryCatch<TryCatch.Void>.FromResult(new TryCatch.Void()));

    public static TryCatch FromException(Exception exception) => new TryCatch(TryCatch<TryCatch.Void>.FromException(exception));

    public static Task UnsafeWaitAsync(
      Task<TryCatch> tryCatchTask,
      CancellationToken cancellationToken)
    {
      return tryCatchTask.ContinueWith((Action<Task<TryCatch>>) (antecedent => antecedent.Result.ThrowIfFailed()), cancellationToken);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private readonly struct Void
    {
    }
  }
}
