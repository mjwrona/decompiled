// Decompiled with JetBrains decompiler
// Type: Validation.Verify
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Diagnostics;

namespace Validation
{
  public static class Verify
  {
    [DebuggerStepThrough]
    public static void Operation(bool condition, string message)
    {
      if (!condition)
        throw new InvalidOperationException(message);
    }

    [DebuggerStepThrough]
    public static void Operation(bool condition, string unformattedMessage, object arg1)
    {
      if (!condition)
        throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1));
    }

    [DebuggerStepThrough]
    public static void Operation(
      bool condition,
      string unformattedMessage,
      object arg1,
      object arg2)
    {
      if (!condition)
        throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1, arg2));
    }

    [DebuggerStepThrough]
    public static void Operation(bool condition, string unformattedMessage, params object[] args)
    {
      if (!condition)
        throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, args));
    }

    [DebuggerStepThrough]
    public static Exception FailOperation(string message, params object[] args) => throw new InvalidOperationException(PrivateErrorHelpers.Format(message, args));

    [DebuggerStepThrough]
    public static void NotDisposed(IDisposableObservable disposedValue, string message = null)
    {
      Requires.NotNull<IDisposableObservable>(disposedValue, nameof (disposedValue));
      if (!disposedValue.IsDisposed)
        return;
      string objectName = disposedValue != null ? disposedValue.GetType().FullName : string.Empty;
      if (message != null)
        throw new ObjectDisposedException(objectName, message);
      throw new ObjectDisposedException(objectName);
    }

    [DebuggerStepThrough]
    public static void NotDisposed(bool condition, object disposedValue, string message = null)
    {
      if (condition)
        return;
      string objectName = disposedValue != null ? disposedValue.GetType().FullName : string.Empty;
      if (message != null)
        throw new ObjectDisposedException(objectName, message);
      throw new ObjectDisposedException(objectName);
    }

    [DebuggerStepThrough]
    public static void NotDisposed(bool condition, string message)
    {
      if (!condition)
        throw new ObjectDisposedException(message);
    }
  }
}
