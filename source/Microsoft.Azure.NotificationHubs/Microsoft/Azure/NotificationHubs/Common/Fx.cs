// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Fx
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using Microsoft.Azure.NotificationHubs.Properties;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class Fx
  {
    private const string DefaultEventSource = "Microsoft.Notifications";
    private static ExceptionTrace exceptionTrace;
    private static DiagnosticTrace diagnosticTrace;

    private static DiagnosticTrace InitializeTracing() => new DiagnosticTrace("Microsoft.Notifications", DiagnosticTrace.DefaultEtwProviderId);

    public static ExceptionTrace Exception
    {
      get
      {
        if (Fx.exceptionTrace == null)
          Fx.exceptionTrace = new ExceptionTrace("Microsoft.Notifications");
        return Fx.exceptionTrace;
      }
    }

    public static DiagnosticTrace Trace
    {
      get
      {
        if (Fx.diagnosticTrace == null)
          Fx.diagnosticTrace = Fx.InitializeTracing();
        return Fx.diagnosticTrace;
      }
    }

    public static byte[] AllocateByteArray(int size)
    {
      try
      {
        return new byte[size];
      }
      catch (OutOfMemoryException ex)
      {
        throw Fx.Exception.AsError((System.Exception) new InsufficientMemoryException(Microsoft.Azure.NotificationHubs.SR.GetString(Resources.BufferAllocationFailed, (object) size), (System.Exception) ex));
      }
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition, string description)
    {
      int num = condition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    public static void Assert(string description)
    {
    }

    public static void AssertAndThrow(bool condition, string description)
    {
      if (condition)
        return;
      Fx.AssertAndThrow(description);
    }

    public static void AssertIsNotNull(object objectMayBeNull, string description)
    {
      if (objectMayBeNull != null)
        return;
      Fx.AssertAndThrow(description);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static System.Exception AssertAndThrow(string description) => throw Fx.Exception.AsError((System.Exception) new AssertionFailedException(description));

    public static void AssertAndThrowFatal(bool condition, string description)
    {
      if (condition)
        return;
      Fx.AssertAndThrowFatal(description);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static System.Exception AssertAndThrowFatal(string description) => throw Fx.Exception.AsError((System.Exception) new FatalException(description));

    public static void AssertAndFailFastService(bool condition, string description)
    {
      if (condition)
        return;
      Fx.AssertAndFailFastService(description);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static System.Exception AssertAndFailFastService(string description)
    {
      string failFastMessage = SRCore.FailFastMessage((object) description);
      try
      {
        try
        {
          MessagingClientEtwProvider.Provider.EventWriteFailFastOccurred(description);
          Fx.Exception.TraceFailFast(failFastMessage);
          Fx.FailFastInProgress = true;
          Thread.Sleep(TimeSpan.FromSeconds(15.0));
        }
        finally
        {
          Thread thread = new Thread((ThreadStart) (() =>
          {
            throw new FatalException(failFastMessage);
          }));
          thread.Start();
          thread.Join();
        }
      }
      catch
      {
        throw;
      }
      return (System.Exception) null;
    }

    internal static bool FailFastInProgress { get; private set; }

    public static bool IsFatal(System.Exception exception)
    {
      while (true)
      {
        switch (exception)
        {
          case FatalException _:
          case OutOfMemoryException _ when !(exception is InsufficientMemoryException):
          case ThreadAbortException _:
          case AccessViolationException _:
          case SEHException _:
            goto label_1;
          case TypeInitializationException _:
          case TargetInvocationException _:
            exception = exception.InnerException;
            continue;
          case AggregateException _:
            goto label_3;
          case NullReferenceException _:
            goto label_11;
          default:
            goto label_13;
        }
      }
label_1:
      return true;
label_3:
      using (IEnumerator<System.Exception> enumerator = ((AggregateException) exception).InnerExceptions.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (Fx.IsFatal(enumerator.Current))
            return true;
        }
        goto label_13;
      }
label_11:
      MessagingClientEtwProvider.Provider.EventWriteNullReferenceErrorOccurred(exception.ToString());
label_13:
      return false;
    }

    public static TransactionScope CreateTransactionScope(Transaction transaction)
    {
      try
      {
        return transaction == (Transaction) null ? (TransactionScope) null : new TransactionScope(transaction);
      }
      catch (TransactionAbortedException ex)
      {
        CommittableTransaction committableTransaction = new CommittableTransaction();
        try
        {
          return new TransactionScope(committableTransaction.Clone());
        }
        finally
        {
          committableTransaction.Rollback();
        }
      }
    }

    public static void CompleteTransactionScope(ref TransactionScope scope)
    {
      TransactionScope transactionScope = scope;
      if (transactionScope == null)
        return;
      scope = (TransactionScope) null;
      try
      {
        transactionScope.Complete();
      }
      finally
      {
        transactionScope.Dispose();
      }
    }

    [SecurityCritical]
    public static IOCompletionCallback ThunkCallback(IOCompletionCallback callback) => new Fx.IOCompletionThunk(callback).ThunkFrame;

    public static TransactionCompletedEventHandler ThunkTransactionEventHandler(
      TransactionCompletedEventHandler handler)
    {
      return new Fx.TransactionEventHandlerThunk(handler).ThunkFrame;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private static void TraceExceptionNoThrow(System.Exception exception)
    {
      try
      {
        Fx.Exception.TraceUnhandled(exception);
      }
      catch
      {
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private static bool HandleAtThreadBase(System.Exception exception)
    {
      if (exception == null)
        return false;
      Fx.TraceExceptionNoThrow(exception);
      return false;
    }

    [SecurityCritical]
    private sealed class IOCompletionThunk
    {
      private IOCompletionCallback callback;

      public IOCompletionThunk(IOCompletionCallback callback) => this.callback = callback;

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
        catch (System.Exception ex)
        {
          if (Fx.HandleAtThreadBase(ex))
            return;
          throw;
        }
      }
    }

    private sealed class TransactionEventHandlerThunk
    {
      private readonly TransactionCompletedEventHandler callback;

      public TransactionEventHandlerThunk(TransactionCompletedEventHandler callback) => this.callback = callback;

      public TransactionCompletedEventHandler ThunkFrame => new TransactionCompletedEventHandler(this.UnhandledExceptionFrame);

      private void UnhandledExceptionFrame(object sender, TransactionEventArgs args)
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          this.callback(sender, args);
        }
        catch (System.Exception ex)
        {
          throw Fx.AssertAndFailFastService(ex.ToString());
        }
      }
    }

    public static class Tag
    {
      public enum CacheAttrition
      {
        None,
        ElementOnTimer,
        ElementOnGC,
        ElementOnCallback,
        FullPurgeOnTimer,
        FullPurgeOnEachAccess,
        PartialPurgeOnTimer,
        PartialPurgeOnEachAccess,
      }

      public enum Location
      {
        InProcess,
        OutOfProcess,
        LocalSystem,
        LocalOrRemoteSystem,
        RemoteSystem,
      }

      public enum SynchronizationKind
      {
        LockStatement,
        MonitorWait,
        MonitorExplicit,
        InterlockedNoSpin,
        InterlockedWithSpin,
        FromFieldType,
      }

      [Flags]
      public enum BlocksUsing
      {
        MonitorEnter = 0,
        MonitorWait = 1,
        ManualResetEvent = 2,
        AutoResetEvent = ManualResetEvent | MonitorWait, // 0x00000003
        AsyncResult = 4,
        IAsyncResult = AsyncResult | MonitorWait, // 0x00000005
        PInvoke = AsyncResult | ManualResetEvent, // 0x00000006
        InputQueue = PInvoke | MonitorWait, // 0x00000007
        ThreadNeutralSemaphore = 8,
        PrivatePrimitive = ThreadNeutralSemaphore | MonitorWait, // 0x00000009
        OtherInternalPrimitive = ThreadNeutralSemaphore | ManualResetEvent, // 0x0000000A
        OtherFrameworkPrimitive = OtherInternalPrimitive | MonitorWait, // 0x0000000B
        OtherInterop = ThreadNeutralSemaphore | AsyncResult, // 0x0000000C
        Other = OtherInterop | MonitorWait, // 0x0000000D
        NonBlocking = OtherInterop | ManualResetEvent, // 0x0000000E
      }

      public static class Strings
      {
        internal const string ExternallyManaged = "externally managed";
        internal const string AppDomain = "AppDomain";
        internal const string DeclaringInstance = "instance of declaring class";
        internal const string Unbounded = "unbounded";
        internal const string Infinite = "infinite";
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class ExternalResourceAttribute : Attribute
      {
        private readonly Fx.Tag.Location location;
        private readonly string description;

        public ExternalResourceAttribute(Fx.Tag.Location location, string description)
        {
          this.location = location;
          this.description = description;
        }

        public Fx.Tag.Location Location => this.location;

        public string Description => this.description;
      }

      [AttributeUsage(AttributeTargets.Field)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class CacheAttribute : Attribute
      {
        private readonly Type elementType;
        private readonly Fx.Tag.CacheAttrition cacheAttrition;

        public CacheAttribute(Type elementType, Fx.Tag.CacheAttrition cacheAttrition)
        {
          this.Scope = "instance of declaring class";
          this.SizeLimit = "unbounded";
          this.Timeout = "infinite";
          this.elementType = !(elementType == (Type) null) ? elementType : throw Fx.Exception.ArgumentNull(nameof (elementType));
          this.cacheAttrition = cacheAttrition;
        }

        public Type ElementType => this.elementType;

        public Fx.Tag.CacheAttrition CacheAttrition => this.cacheAttrition;

        public string Scope { get; set; }

        public string SizeLimit { get; set; }

        public string Timeout { get; set; }
      }

      [AttributeUsage(AttributeTargets.Field)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class QueueAttribute : Attribute
      {
        private readonly Type elementType;

        public QueueAttribute(Type elementType)
        {
          this.Scope = "instance of declaring class";
          this.SizeLimit = "unbounded";
          this.elementType = !(elementType == (Type) null) ? elementType : throw Fx.Exception.ArgumentNull(nameof (elementType));
        }

        public Type ElementType => this.elementType;

        public string Scope { get; set; }

        public string SizeLimit { get; set; }

        public bool StaleElementsRemovedImmediately { get; set; }

        public bool EnqueueThrowsIfFull { get; set; }
      }

      [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class SynchronizationObjectAttribute : Attribute
      {
        public SynchronizationObjectAttribute()
        {
          this.Blocking = true;
          this.Scope = "instance of declaring class";
          this.Kind = Fx.Tag.SynchronizationKind.FromFieldType;
        }

        public bool Blocking { get; set; }

        public string Scope { get; set; }

        public Fx.Tag.SynchronizationKind Kind { get; set; }
      }

      [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class SynchronizationPrimitiveAttribute : Attribute
      {
        private readonly Fx.Tag.BlocksUsing blocksUsing;

        public SynchronizationPrimitiveAttribute(Fx.Tag.BlocksUsing blocksUsing) => this.blocksUsing = blocksUsing;

        public Fx.Tag.BlocksUsing BlocksUsing => this.blocksUsing;

        public bool SupportsAsync { get; set; }

        public bool Spins { get; set; }

        public string ReleaseMethod { get; set; }
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class BlockingAttribute : Attribute
      {
        public string CancelMethod { get; set; }

        public Type CancelDeclaringType { get; set; }

        public string Conditional { get; set; }
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class GuaranteeNonBlockingAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class NonThrowingAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public class ThrowsAttribute : Attribute
      {
        private readonly Type exceptionType;
        private readonly string diagnosis;

        public ThrowsAttribute(Type exceptionType, string diagnosis)
        {
          if (exceptionType == (Type) null)
            throw Fx.Exception.ArgumentNull(nameof (exceptionType));
          if (string.IsNullOrEmpty(diagnosis))
            throw Fx.Exception.ArgumentNullOrEmpty(nameof (diagnosis));
          this.exceptionType = exceptionType;
          this.diagnosis = diagnosis;
        }

        public Type ExceptionType => this.exceptionType;

        public string Diagnosis => this.diagnosis;
      }

      [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class InheritThrowsAttribute : Attribute
      {
        public Type FromDeclaringType { get; set; }

        public string From { get; set; }
      }

      [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
      [Conditional("CODE_ANALYSIS")]
      public sealed class SecurityNoteAttribute : Attribute
      {
        public string Critical { get; set; }

        public string Safe { get; set; }

        public string Miscellaneous { get; set; }
      }
    }
  }
}
