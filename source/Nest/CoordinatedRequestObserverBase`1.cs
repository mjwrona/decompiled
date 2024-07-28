// Decompiled with JetBrains decompiler
// Type: Nest.CoordinatedRequestObserverBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public abstract class CoordinatedRequestObserverBase<T> : IObserver<T>
  {
    private readonly Action _completed;
    private readonly Action<Exception> _onError;
    private readonly Action<T> _onNext;

    protected CoordinatedRequestObserverBase(
      Action<T> onNext = null,
      Action<Exception> onError = null,
      Action completed = null)
    {
      this._onNext = onNext;
      this._onError = onError;
      this._completed = completed;
    }

    public void OnCompleted()
    {
      Action completed = this._completed;
      if (completed == null)
        return;
      completed();
    }

    public void OnError(Exception error)
    {
      if (error is UnexpectedElasticsearchClientException elasticsearchClientException && elasticsearchClientException.InnerException != null && elasticsearchClientException.InnerException is OperationCanceledException innerException)
      {
        Action<Exception> onError = this._onError;
        if (onError == null)
          return;
        onError((Exception) innerException);
      }
      else
      {
        Action<Exception> onError = this._onError;
        if (onError == null)
          return;
        onError(error);
      }
    }

    public void OnNext(T value)
    {
      Action<T> onNext = this._onNext;
      if (onNext == null)
        return;
      onNext(value);
    }
  }
}
