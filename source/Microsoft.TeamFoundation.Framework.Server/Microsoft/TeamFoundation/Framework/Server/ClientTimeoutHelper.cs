// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientTimeoutHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ClientTimeoutHelper
  {
    private readonly RegistryQuery m_timeoutKey;
    private readonly int m_defaultTimeoutMilliseconds;

    public ClientTimeoutHelper(RegistryQuery timeoutKey, int defaultTimeoutMilliseconds)
    {
      this.m_timeoutKey = timeoutKey;
      this.m_defaultTimeoutMilliseconds = defaultTimeoutMilliseconds;
    }

    public void ExecuteWithTimeout(
      IVssRequestContext requestContext,
      Action<CancellationToken> action)
    {
      int millisecondsDelay = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in this.m_timeoutKey, this.m_defaultTimeoutMilliseconds);
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(millisecondsDelay);
        try
        {
          action(linkedTokenSource.Token);
        }
        catch (System.OperationCanceledException ex)
        {
          if (!requestContext.CancellationToken.IsCancellationRequested)
            throw new TimeoutException(CommonResources.HttpRequestTimeout((object) TimeSpan.FromMilliseconds((double) millisecondsDelay)), (Exception) ex);
          throw;
        }
      }
    }

    public T ExecuteWithTimeout<T>(
      IVssRequestContext requestContext,
      Func<CancellationToken, T> func)
    {
      int millisecondsDelay = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in this.m_timeoutKey, this.m_defaultTimeoutMilliseconds);
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(millisecondsDelay);
        try
        {
          return func(linkedTokenSource.Token);
        }
        catch (System.OperationCanceledException ex)
        {
          if (!requestContext.CancellationToken.IsCancellationRequested)
            throw new TimeoutException(CommonResources.HttpRequestTimeout((object) TimeSpan.FromMilliseconds((double) millisecondsDelay)), (Exception) ex);
          throw;
        }
      }
    }
  }
}
