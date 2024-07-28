// Decompiled with JetBrains decompiler
// Type: Validation.Assumes
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Validation
{
  public static class Assumes
  {
    [DebuggerStepThrough]
    public static void NotNull<T>([ValidatedNotNull] T value) where T : class => Assumes.True((object) value != null);

    [DebuggerStepThrough]
    public static void NotNullOrEmpty([ValidatedNotNull] string value)
    {
      Assumes.NotNull<string>(value);
      Assumes.True(value.Length > 0);
      Assumes.True(value[0] > char.MinValue);
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty<T>([ValidatedNotNull] ICollection<T> values)
    {
      Assumes.NotNull<ICollection<T>>(values);
      Assumes.True(values.Count > 0);
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty<T>([ValidatedNotNull] IEnumerable<T> values)
    {
      Assumes.NotNull<IEnumerable<T>>(values);
      Assumes.True(values.Any<T>());
    }

    [DebuggerStepThrough]
    public static void Null<T>(T value) where T : class => Assumes.True((object) value == null);

    [DebuggerStepThrough]
    public static void Is<T>(object value) => Assumes.True(value is T);

    [DebuggerStepThrough]
    public static void False(bool condition, string message = null)
    {
      if (!condition)
        return;
      Assumes.Fail(message);
    }

    [DebuggerStepThrough]
    public static void False(bool condition, string unformattedMessage, object arg1)
    {
      if (!condition)
        return;
      Assumes.Fail(Assumes.Format(unformattedMessage, arg1));
    }

    [DebuggerStepThrough]
    public static void False(bool condition, string unformattedMessage, params object[] args)
    {
      if (!condition)
        return;
      Assumes.Fail(Assumes.Format(unformattedMessage, args));
    }

    [DebuggerStepThrough]
    public static void True(bool condition, string message = null)
    {
      if (condition)
        return;
      Assumes.Fail(message);
    }

    [DebuggerStepThrough]
    public static void True(bool condition, string unformattedMessage, object arg1)
    {
      if (condition)
        return;
      Assumes.Fail(Assumes.Format(unformattedMessage, arg1));
    }

    [DebuggerStepThrough]
    public static void True(bool condition, string unformattedMessage, params object[] args)
    {
      if (condition)
        return;
      Assumes.Fail(Assumes.Format(unformattedMessage, args));
    }

    [DebuggerStepThrough]
    public static Exception NotReachable()
    {
      Assumes.InternalErrorException internalErrorException = new Assumes.InternalErrorException();
      if (true)
        throw internalErrorException;
      return (Exception) null;
    }

    [DebuggerStepThrough]
    public static void Present<T>(T component)
    {
      if ((object) component != null)
        return;
      Assumes.Fail(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Strings.ServiceMissing, new object[1]
      {
        (object) PrivateErrorHelpers.TrimGenericWrapper(typeof (T), typeof (Lazy<>)).FullName
      }));
    }

    [DebuggerStepThrough]
    public static Exception Fail(string message = null, bool showAssert = true)
    {
      Assumes.InternalErrorException internalErrorException = new Assumes.InternalErrorException(message, showAssert);
      if (true)
        throw internalErrorException;
      return (Exception) null;
    }

    public static Exception Fail(string message, Exception innerException, bool showAssert = true)
    {
      Assumes.InternalErrorException internalErrorException = new Assumes.InternalErrorException(message, innerException, showAssert);
      if (true)
        throw internalErrorException;
      return (Exception) null;
    }

    private static string Format(string format, params object[] arguments) => PrivateErrorHelpers.Format(format, arguments);

    private class InternalErrorException : Exception
    {
      [DebuggerStepThrough]
      public InternalErrorException(string message = null, bool showAssert = true)
        : base(message ?? Strings.InternalExceptionMessage)
      {
        this.ShowAssertDialog(showAssert);
      }

      [DebuggerStepThrough]
      public InternalErrorException(string message, Exception innerException, bool showAssert = true)
        : base(message ?? Strings.InternalExceptionMessage, innerException)
      {
        this.ShowAssertDialog(showAssert);
      }

      [DebuggerStepThrough]
      private void ShowAssertDialog(bool showAssert)
      {
        if (!showAssert)
          return;
        string message = this.Message;
        if (this.InnerException != null)
          message = message + " " + (object) this.InnerException;
        Report.Fail(message);
      }
    }
  }
}
