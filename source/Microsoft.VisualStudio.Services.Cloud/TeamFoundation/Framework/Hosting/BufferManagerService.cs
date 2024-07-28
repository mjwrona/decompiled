// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.BufferManagerService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  internal class BufferManagerService : IVssFrameworkService
  {
    internal static long s_maxLendBufferSize;
    private const long c_defaultMaxLendBufferSize = 50331648;
    private const string c_area = "BufferManager";
    private const string c_layer = "BufferManager";
    private const string c_bufferManagerRegistryRootPath = "/Service/BufferManager";
    private const string c_bufferManagerAllRegistryEntries = "/Service/BufferManager/*";
    internal const string c_bufferManagerMaxLendBufferSizeRegistry = "/Service/BufferManager/MaxLendBufferSize";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), "/Service/BufferManager/*");
      this.ReadServiceSettings(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.CheckSystemRequestContext();
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReadServiceSettings(requestContext);
    }

    internal void ReadServiceSettings(IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckSystemRequestContext();
      BufferManagerService.s_maxLendBufferSize = deploymentContext.GetService<IVssRegistryService>().ReadEntries(deploymentContext, (RegistryQuery) "/Service/BufferManager/*").GetValueFromPath<long>("/Service/BufferManager/MaxLendBufferSize", 50331648L);
    }

    public BufferManagerService.BufferManager GetBufferManager() => BufferManagerService.BufferManager.Instance;

    internal class BufferManager : IBufferManager
    {
      private static readonly Lazy<BufferManagerService.BufferManager> lazy = new Lazy<BufferManagerService.BufferManager>((Func<BufferManagerService.BufferManager>) (() => new BufferManagerService.BufferManager()));
      private ConcurrentDictionary<byte[], ByteArray> m_buffers;
      internal long m_lendBufferSize;
      private VssPerformanceCounter m_lendBufferSizeCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Hosting.PerfCounters.Buffermanager_LendBufferSize");

      public static BufferManagerService.BufferManager Instance => BufferManagerService.BufferManager.lazy.Value;

      private BufferManager()
      {
        this.m_buffers = new ConcurrentDictionary<byte[], ByteArray>();
        this.MaxBufferSize = 32768;
      }

      public int GetDefaultBufferSize() => this.MaxBufferSize;

      public void ReturnBuffer(byte[] buffer)
      {
        ByteArray byteArray;
        if (!this.m_buffers.TryRemove(buffer, out byteArray))
          return;
        Interlocked.Add(ref this.m_lendBufferSize, (long) -byteArray.Bytes.Length);
        this.m_lendBufferSizeCounter.SetValue(this.m_lendBufferSize);
        byteArray?.Dispose();
      }

      public byte[] TakeBuffer(int bufferSize)
      {
        if (this.m_lendBufferSize + (long) bufferSize <= BufferManagerService.s_maxLendBufferSize)
        {
          ByteArray byteArray = new ByteArray(bufferSize);
          Interlocked.Add(ref this.m_lendBufferSize, (long) byteArray.Bytes.Length);
          this.m_lendBufferSizeCounter.SetValue(this.m_lendBufferSize);
          this.m_buffers.TryAdd(byteArray.Bytes, byteArray);
          return byteArray.Bytes;
        }
        TeamFoundationTracingService.TraceRaw(15301, TraceLevel.Info, nameof (BufferManager), nameof (BufferManager), string.Format("Buffer manager starts to lend buffer managed by the GC. We are currently lending: {0}", (object) this.m_lendBufferSize));
        return new byte[bufferSize];
      }

      public int MaxBufferSize { get; private set; }
    }
  }
}
