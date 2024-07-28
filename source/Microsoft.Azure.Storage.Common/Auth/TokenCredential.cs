// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Auth.TokenCredential
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Storage.Auth
{
  public sealed class TokenCredential : IDisposable
  {
    private volatile string token;
    private readonly Timer timer;
    private readonly RenewTokenFuncAsync renewTokenFuncAsync;
    private readonly CancellationTokenSource cancellationTokenSource;
    private TimeSpan renewFrequency;

    public string Token
    {
      get => this.token;
      set => this.token = value;
    }

    public TokenCredential(string initialToken)
      : this(initialToken, (RenewTokenFuncAsync) null, (object) null, new TimeSpan())
    {
    }

    public TokenCredential(
      string initialToken,
      RenewTokenFuncAsync periodicTokenRenewer,
      object state,
      TimeSpan renewFrequency)
    {
      this.token = initialToken;
      if (periodicTokenRenewer == null)
        return;
      this.renewTokenFuncAsync = periodicTokenRenewer;
      this.renewFrequency = renewFrequency;
      this.timer = new Timer(new TimerCallback(this.RenewTokenAsync), state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
      this.timer.Change(this.renewFrequency, Timeout.InfiniteTimeSpan);
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
      this.timer?.Dispose();
      this.cancellationTokenSource?.Cancel();
    }

    private async void RenewTokenAsync(object state)
    {
      try
      {
        NewTokenAndFrequency tokenAndFrequency = await this.renewTokenFuncAsync(state, this.cancellationTokenSource.Token).ConfigureAwait(false);
        this.token = tokenAndFrequency.Token;
        this.renewFrequency = tokenAndFrequency.Frequency ?? this.renewFrequency;
      }
      catch (OperationCanceledException ex)
      {
        if (!ex.CancellationToken.Equals(this.cancellationTokenSource.Token))
          throw ex;
      }
      finally
      {
        if (!this.cancellationTokenSource.IsCancellationRequested)
          this.timer.Change(this.renewFrequency, Timeout.InfiniteTimeSpan);
      }
    }
  }
}
