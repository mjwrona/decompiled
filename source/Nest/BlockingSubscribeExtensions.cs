// Decompiled with JetBrains decompiler
// Type: Nest.BlockingSubscribeExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Threading;

namespace Nest
{
  public static class BlockingSubscribeExtensions
  {
    public static BulkAllObserver Wait<T>(
      this BulkAllObservable<T> observable,
      TimeSpan maximumRunTime,
      Action<BulkAllResponse> onNext)
      where T : class
    {
      return BlockingSubscribeExtensions.WaitOnObservable<BulkAllObservable<T>, BulkAllResponse, BulkAllObserver>(observable, maximumRunTime, (Func<Action<Exception>, Action, BulkAllObserver>) ((e, c) => new BulkAllObserver(onNext, e, c)));
    }

    public static ScrollAllObserver<T> Wait<T>(
      this IObservable<IScrollAllResponse<T>> observable,
      TimeSpan maximumRunTime,
      Action<IScrollAllResponse<T>> onNext)
      where T : class
    {
      return BlockingSubscribeExtensions.WaitOnObservable<IObservable<IScrollAllResponse<T>>, IScrollAllResponse<T>, ScrollAllObserver<T>>(observable, maximumRunTime, (Func<Action<Exception>, Action, ScrollAllObserver<T>>) ((e, c) => new ScrollAllObserver<T>(onNext, e, c)));
    }

    public static ReindexObserver Wait(
      this IObservable<BulkAllResponse> observable,
      TimeSpan maximumRunTime,
      Action<BulkAllResponse> onNext)
    {
      return BlockingSubscribeExtensions.WaitOnObservable<IObservable<BulkAllResponse>, BulkAllResponse, ReindexObserver>(observable, maximumRunTime, (Func<Action<Exception>, Action, ReindexObserver>) ((e, c) => new ReindexObserver(onNext, e, c)));
    }

    private static TObserver WaitOnObservable<TObservable, TObserve, TObserver>(
      TObservable observable,
      TimeSpan maximumRunTime,
      Func<Action<Exception>, Action, TObserver> factory)
      where TObservable : IObservable<TObserve>
      where TObserver : IObserver<TObserve>
    {
      observable.ThrowIfNull<TObservable>(nameof (observable));
      maximumRunTime.ThrowIfNull<TimeSpan>(nameof (maximumRunTime));
      Exception ex = (Exception) null;
      ManualResetEvent handle = new ManualResetEvent(false);
      TObserver observer = factory((Action<Exception>) (e =>
      {
        ex = e;
        handle.Set();
      }), (Action) (() => handle.Set()));
      observable.Subscribe((IObserver<TObserve>) observer);
      handle.WaitOne(maximumRunTime);
      if (ex != null)
        throw ex;
      return observer;
    }
  }
}
