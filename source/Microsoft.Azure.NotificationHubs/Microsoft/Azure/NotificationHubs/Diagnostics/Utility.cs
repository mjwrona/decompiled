// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.Utility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class Utility
  {
    private ExceptionUtility exceptionUtility;

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.Utility instead")]
    internal Utility(ExceptionUtility exceptionUtility) => this.exceptionUtility = exceptionUtility;

    private ExceptionUtility ExceptionUtility
    {
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] get => this.exceptionUtility;
    }

    internal static void CloseInvalidOutSafeHandle(SafeHandle handle) => handle?.SetHandleAsInvalid();

    internal static void CloseInvalidOutCriticalHandle(CriticalHandle handle) => handle?.SetHandleAsInvalid();

    internal byte[] AllocateByteArray(int size) => Fx.AllocateByteArray(size);

    internal AsyncCallback ThunkCallback(AsyncCallback callback) => new Utility.AsyncThunk(callback, this).ThunkFrame;

    internal TimerCallback ThunkCallback(TimerCallback callback) => new Utility.TimerThunk(callback, this).ThunkFrame;

    internal WaitOrTimerCallback ThunkCallback(WaitOrTimerCallback callback) => new Utility.WaitOrTimerThunk(callback, this).ThunkFrame;

    internal IOCompletionCallback ThunkCallback(IOCompletionCallback callback) => new Utility.IOCompletionThunk(callback, this).ThunkFrame;

    internal WaitCallback ThunkCallback(WaitCallback callback) => new Utility.WaitThunk(callback, this).ThunkFrame;

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    internal virtual bool CallHandler(Exception exception) => false;

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private void TraceExceptionNoThrow(Exception exception, TraceEventType eventType)
    {
      try
      {
        this.ExceptionUtility.TraceHandledException(exception, eventType);
      }
      catch
      {
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private bool HandleAtThreadBase(Exception exception)
    {
      if (ExceptionUtility.IsInfrastructureException(exception))
      {
        this.TraceExceptionNoThrow(exception, TraceEventType.Warning);
        return false;
      }
      this.TraceExceptionNoThrow(exception, TraceEventType.Critical);
      try
      {
        return this.CallHandler(exception);
      }
      catch (Exception ex)
      {
        this.TraceExceptionNoThrow(ex, TraceEventType.Error);
      }
      return false;
    }

    private class Thunk<T> where T : class
    {
      protected T callback;
      protected Utility utility;

      public Thunk(T callback, Utility utility)
      {
        this.callback = callback;
        this.utility = utility;
      }
    }

    private sealed class AsyncThunk : Utility.Thunk<AsyncCallback>
    {
      public AsyncThunk(AsyncCallback callback, Utility utility)
        : base(callback, utility)
      {
      }

      public AsyncCallback ThunkFrame => new AsyncCallback(this.UnhandledExceptionFrame);

      private void UnhandledExceptionFrame(IAsyncResult result)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(result);
        }
        catch (Exception ex)
        {
          if (this.utility.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }

    private sealed class TimerThunk : Utility.Thunk<TimerCallback>
    {
      public TimerThunk(TimerCallback callback, Utility utility)
        : base(callback, utility)
      {
      }

      public TimerCallback ThunkFrame => new TimerCallback(this.UnhandledExceptionFrame);

      private void UnhandledExceptionFrame(object state)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(state);
        }
        catch (Exception ex)
        {
          if (this.utility.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }

    private sealed class WaitOrTimerThunk : Utility.Thunk<WaitOrTimerCallback>
    {
      public WaitOrTimerThunk(WaitOrTimerCallback callback, Utility utility)
        : base(callback, utility)
      {
      }

      public WaitOrTimerCallback ThunkFrame => new WaitOrTimerCallback(this.UnhandledExceptionFrame);

      private void UnhandledExceptionFrame(object state, bool timedOut)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(state, timedOut);
        }
        catch (Exception ex)
        {
          if (this.utility.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }

    private sealed class IOCompletionThunk : Utility.Thunk<IOCompletionCallback>
    {
      public IOCompletionThunk(IOCompletionCallback callback, Utility utility)
        : base(callback, utility)
      {
      }

      public IOCompletionCallback ThunkFrame => new IOCompletionCallback(this.UnhandledExceptionFrame);

      private unsafe void UnhandledExceptionFrame(
        uint error,
        uint bytesRead,
        NativeOverlapped* nativeOverlapped)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(error, bytesRead, nativeOverlapped);
        }
        catch (Exception ex)
        {
          if (this.utility.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }

    private sealed class WaitThunk : Utility.Thunk<WaitCallback>
    {
      public WaitThunk(WaitCallback callback, Utility utility)
        : base(callback, utility)
      {
      }

      public WaitCallback ThunkFrame => new WaitCallback(this.UnhandledExceptionFrame);

      private void UnhandledExceptionFrame(object state)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(state);
        }
        catch (Exception ex)
        {
          if (this.utility.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }
  }
}
