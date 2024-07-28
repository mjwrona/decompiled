// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Monads.TryCatch`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Monads
{
  internal readonly struct TryCatch<TResult>
  {
    private readonly Either<Exception, TResult> either;

    private TryCatch(Either<Exception, TResult> either) => this.either = either;

    public bool Succeeded => this.either.IsRight;

    public bool Failed => !this.Succeeded;

    public TResult Result
    {
      get
      {
        if (this.Succeeded)
          return this.either.FromRight(default (TResult));
        throw new InvalidOperationException("Tried to get the result of a TryCatch that ended in an exception.");
      }
    }

    public Exception Exception
    {
      get
      {
        if (!this.Succeeded)
          return this.either.FromLeft((Exception) null);
        throw new InvalidOperationException("Tried to get the exception of a TryCatch that ended in a result.");
      }
    }

    public Exception InnerMostException
    {
      get
      {
        Exception innerMostException = this.Exception;
        while (innerMostException.InnerException != null)
          innerMostException = innerMostException.InnerException;
        return innerMostException;
      }
    }

    public void Match(Action<TResult> onSuccess, Action<Exception> onError) => this.either.Match(onError, onSuccess);

    public TryCatch<TResult> Try(Action<TResult> onSuccess)
    {
      if (this.Succeeded)
        onSuccess(this.either.FromRight(default (TResult)));
      return this;
    }

    public TryCatch<T> Try<T>(Func<TResult, T> onSuccess) => !this.Succeeded ? TryCatch<T>.FromException(this.either.FromLeft((Exception) null)) : TryCatch<T>.FromResult(onSuccess(this.either.FromRight(default (TResult))));

    public async Task<TryCatch<T>> TryAsync<T>(Func<TResult, Task<T>> onSuccess)
    {
      TryCatch<T> tryCatch;
      if (this.Succeeded)
        tryCatch = TryCatch<T>.FromResult(await onSuccess(this.either.FromRight(default (TResult))));
      else
        tryCatch = TryCatch<T>.FromException(this.either.FromLeft((Exception) null));
      return tryCatch;
    }

    public TryCatch<TResult> Catch(Action<Exception> onError)
    {
      if (!this.Succeeded)
        onError(this.either.FromLeft((Exception) null));
      return this;
    }

    public TryCatch<TResult> Catch(Func<Exception, TryCatch<TResult>> onError) => !this.Succeeded ? onError(this.either.FromLeft((Exception) null)) : this;

    public async Task<TryCatch<TResult>> CatchAsync(Func<Exception, Task> onError)
    {
      if (!this.Succeeded)
        await onError(this.either.FromLeft((Exception) null));
      return this;
    }

    public async Task<TryCatch<TResult>> CatchAsync(Func<Exception, Task<TryCatch<TResult>>> onError) => !this.Succeeded ? await onError(this.either.FromLeft((Exception) null)) : this;

    public void ThrowIfFailed()
    {
      if (this.Succeeded)
        return;
      ExceptionDispatchInfo.Capture(this.Exception).Throw();
    }

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is TryCatch<TResult> other && this.Equals(other);
    }

    public bool Equals(TryCatch<TResult> other) => this.either.Equals(other.either);

    public override int GetHashCode() => this.either.GetHashCode();

    public static TryCatch<TResult> FromResult(TResult result) => new TryCatch<TResult>((Either<Exception, TResult>) result);

    public static TryCatch<TResult> FromException(Exception exception)
    {
      StackTrace stackTrace = new StackTrace(1);
      return new TryCatch<TResult>((Either<Exception, TResult>) (Exception) new ExceptionWithStackTraceException("TryCatch resulted in an exception.", exception, stackTrace));
    }

    public static bool ConvertToTryGet<T>(TryCatch<T> tryCatch, out T result)
    {
      if (tryCatch.Failed)
      {
        result = default (T);
        return false;
      }
      result = tryCatch.Result;
      return true;
    }

    public static T UnsafeGetResult<T>(TryCatch<T> tryCatch)
    {
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static Task<T> UnsafeGetResultAsync<T>(
      Task<TryCatch<T>> tryCatch,
      CancellationToken cancellationToken)
    {
      return tryCatch.ContinueWith<T>((Func<Task<TryCatch<T>>, T>) (antecedent =>
      {
        antecedent.Result.ThrowIfFailed();
        return antecedent.Result.Result;
      }), cancellationToken);
    }
  }
}
