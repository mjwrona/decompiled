// Decompiled with JetBrains decompiler
// Type: Nest.ProducerConsumerBackPressure
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public class ProducerConsumerBackPressure
  {
    private readonly int _backPressureFactor;
    private readonly SemaphoreSlim _consumerLimiter;
    private readonly int _slots;

    internal ProducerConsumerBackPressure(int? backPressureFactor, int maxConcurrency)
    {
      this._backPressureFactor = backPressureFactor.GetValueOrDefault(4);
      this._slots = maxConcurrency * this._backPressureFactor;
      this._consumerLimiter = new SemaphoreSlim(this._slots, this._slots);
    }

    public Task WaitAsync(CancellationToken token = default (CancellationToken)) => this._consumerLimiter.WaitAsync(token);

    public void Release()
    {
      int releaseCount = Math.Min(this._slots - this._consumerLimiter.CurrentCount, this._backPressureFactor);
      if (releaseCount <= 0)
        return;
      this._consumerLimiter.Release(releaseCount);
    }
  }
}
