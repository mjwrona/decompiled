// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConcurrentOperation`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ConcurrentOperation<T>
  {
    private IVssRequestContext m_mainThreadRequestContext;
    private ConcurrentQueue<T> m_queue;
    private Action<IVssRequestContext, ConcurrentQueue<T>> m_action;
    private string m_keyName;
    private int m_numThreads;
    private int m_minItemsForConcurrency;
    private Exception m_errorOnParallelThread;
    private int m_threadsRunning;
    internal const int DefaultMaxThreads = 4;

    public ConcurrentOperation(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, ConcurrentQueue<T>> action,
      IReadOnlyCollection<T> input,
      string keyName)
      : this(requestContext, action, input, keyName, false)
    {
    }

    private ConcurrentOperation(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, ConcurrentQueue<T>> action,
      IReadOnlyCollection<T> input,
      string keyName,
      bool disableCoreSafetyLimit)
    {
      this.m_mainThreadRequestContext = requestContext;
      this.m_queue = new ConcurrentQueue<T>((IEnumerable<T>) input);
      this.m_action = action;
      this.m_keyName = keyName;
      this.m_numThreads = ConcurrentOperation<T>.ReadParallelizationSetting(requestContext, input, keyName, disableCoreSafetyLimit);
      this.m_minItemsForConcurrency = ConcurrentOperation<T>.ReadMinItemsForConcurrency(requestContext, keyName);
    }

    internal static ConcurrentOperation<T> CreateForTest(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, ConcurrentQueue<T>> action,
      IReadOnlyCollection<T> input,
      string keyName)
    {
      return new ConcurrentOperation<T>(requestContext, action, input, keyName, true);
    }

    public void Execute()
    {
      if (this.m_queue.Count == 0)
        return;
      if (this.m_queue.Count < this.m_minItemsForConcurrency)
      {
        this.m_action(this.m_mainThreadRequestContext, this.m_queue);
      }
      else
      {
        try
        {
          SemaphoreSlim mainThreadProceed = new SemaphoreSlim(0);
          Guid hostId = this.m_mainThreadRequestContext.ServiceHost.InstanceId;
          IVssRequestContext deploymentContext = (IVssRequestContext) null;
          deploymentContext = this.m_mainThreadRequestContext.ToDeploymentHostContext();
          ITeamFoundationHostManagementService hms = deploymentContext.GetService<ITeamFoundationHostManagementService>();
          IVssCustomerIntelligenceService service = this.m_mainThreadRequestContext.GetService<IVssCustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("NumThreads", (double) this.m_numThreads);
          IVssRequestContext threadRequestContext = this.m_mainThreadRequestContext;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(threadRequestContext, nameof (ConcurrentOperation<T>), nameof (Execute), properties);
          RequestContextType requestContextType = ConcurrentOperation<T>.DetermineRequestContextType(this.m_mainThreadRequestContext);
          Interlocked.Add(ref this.m_threadsRunning, this.m_numThreads);
          for (int index = 0; index < this.m_numThreads; ++index)
            Task.Run((Action) (() => this.ExecuteParallel(deploymentContext, hms, hostId, requestContextType, this.m_queue, this.m_action, mainThreadProceed)));
          mainThreadProceed.Wait();
          if (this.m_errorOnParallelThread != null)
            throw this.m_errorOnParallelThread;
          if (this.m_queue.Any<T>())
            throw new InvalidOperationException("ConcurrentOperation internal error: " + this.m_keyName + " finished without throwing, but had unprocessed items remaining.");
        }
        catch (Exception ex)
        {
          this.m_mainThreadRequestContext.Cancel("Failure during request, cancelling parallel threads");
          throw;
        }
      }
    }

    private static RequestContextType DetermineRequestContextType(IVssRequestContext requestContext)
    {
      if (requestContext.IsServicingContext)
        return RequestContextType.ServicingContext;
      return requestContext.IsSystemContext ? RequestContextType.SystemContext : RequestContextType.UserContext;
    }

    private void ExecuteParallel(
      IVssRequestContext deploymentContext,
      ITeamFoundationHostManagementService hms,
      Guid hostId,
      RequestContextType requestContextType,
      ConcurrentQueue<T> input,
      Action<IVssRequestContext, ConcurrentQueue<T>> action,
      SemaphoreSlim mainThreadProceed)
    {
      try
      {
        using (IVssRequestContext vssRequestContext = hms.BeginRequest(deploymentContext, hostId, requestContextType))
          action(vssRequestContext, this.m_queue);
      }
      catch (Exception ex)
      {
        Interlocked.CompareExchange<Exception>(ref this.m_errorOnParallelThread, ex, (Exception) null);
      }
      finally
      {
        if (Interlocked.Decrement(ref this.m_threadsRunning) == 0)
          mainThreadProceed.Release();
      }
    }

    private static int ReadParallelizationSetting(
      IVssRequestContext requestContext,
      IReadOnlyCollection<T> input,
      string keyName,
      bool disableCoreSafetyLimit)
    {
      int val1_1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ConcurrentOperation<T>.ParallelizationSettingRegKey(keyName), true, 4);
      int val1_2;
      if (disableCoreSafetyLimit)
      {
        val1_2 = int.MaxValue;
      }
      else
      {
        val1_2 = Math.Min(val1_1, Environment.ProcessorCount / 4);
        if (val1_2 < 1)
          val1_2 = 1;
        if (val1_1 > val1_2)
          val1_1 = val1_2;
      }
      int count = input.Count;
      return val1_1 <= 0 ? Math.Min(val1_2, count) : Math.Min(val1_1, count);
    }

    private static int ReadMinItemsForConcurrency(IVssRequestContext requestContext, string keyName)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ConcurrentOperation<T>.MinItemsForConcurrencyRegKey(keyName), true, 1);
      if (num < 2)
        num = 2;
      return num;
    }

    internal static string ParallelizationSettingRegKey(string keyName) => "/Service/ConcurrentOperation/" + keyName + "/MaxThreads";

    internal static string MinItemsForConcurrencyRegKey(string keyName) => "/Service/ConcurrentOperation/" + keyName + "/MaxThreads";
  }
}
