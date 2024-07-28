// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Core.Trace.EtwTraceListener
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Azure.Cosmos.Core.Trace
{
  internal sealed class EtwTraceListener : TraceListener
  {
    public const int MaxEtwEventLength = 32500;
    private readonly EtwNativeInterop.ProviderHandle providerHandle = new EtwNativeInterop.ProviderHandle();

    public EtwTraceListener(Guid providerGuid, string name)
      : base(name)
    {
      this.ProviderGuid = providerGuid;
      uint error = EtwNativeInterop.EventRegister(in providerGuid, IntPtr.Zero, IntPtr.Zero, ref this.providerHandle);
      if (error != 0U)
        throw new Win32Exception((int) error);
    }

    public Guid ProviderGuid { get; }

    public override bool IsThreadSafe { get; } = true;

    public override void Close()
    {
      this.Dispose();
      base.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.providerHandle != null && !this.providerHandle.IsInvalid)
        this.providerHandle.Dispose();
      base.Dispose(disposing);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      if (this.IsFiltered(eventCache, source, eventType, id))
        return;
      string message = format;
      if (args != null && args.Length != 0)
      {
        StringBuilder instance = EtwTraceListener.StringBuilderCache.Instance;
        instance.AppendFormat(format, args);
        if (instance.Length > 32500)
          instance.Remove(32500, instance.Length - 32500);
        message = instance.ToString();
      }
      this.TraceInternal(eventType, message);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string message)
    {
      if (this.IsFiltered(eventCache, source, eventType, id))
        return;
      if (message.Length > 32500)
        message = message.Remove(32500, message.Length - 32500);
      this.TraceInternal(eventType, message);
    }

    private void TraceInternal(TraceEventType eventType, string message)
    {
      int num = (int) EtwNativeInterop.EventWriteString(this.providerHandle, (byte) eventType, 0L, message);
    }

    internal uint LastReturnCode { get; private set; }

    private bool IsFiltered(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id)
    {
      return this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, (string) null, (object[]) null, (object) null, (object[]) null);
    }

    public override void Write(string message)
    {
      if (message.Length > 32500)
        message = message.Remove(32500, message.Length - 32500);
      this.TraceInternal(TraceEventType.Information, message);
    }

    public override void WriteLine(string message) => this.Write(message);

    private static class StringBuilderCache
    {
      private const int MaxBuilderSize = 8000;
      [ThreadStatic]
      private static StringBuilder cachedInstance;

      public static StringBuilder Instance
      {
        get
        {
          if (EtwTraceListener.StringBuilderCache.cachedInstance == null)
            EtwTraceListener.StringBuilderCache.cachedInstance = new StringBuilder(8000);
          EtwTraceListener.StringBuilderCache.cachedInstance.Clear();
          return EtwTraceListener.StringBuilderCache.cachedInstance;
        }
      }
    }
  }
}
