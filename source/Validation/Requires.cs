// Decompiled with JetBrains decompiler
// Type: Validation.Requires
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Validation
{
  public static class Requires
  {
    [DebuggerStepThrough]
    public static T NotNull<T>([ValidatedNotNull] T value, string parameterName) where T : class => (object) value != null ? value : throw new ArgumentNullException(parameterName);

    [DebuggerStepThrough]
    public static IntPtr NotNull(IntPtr value, string parameterName) => !(value == IntPtr.Zero) ? value : throw new ArgumentNullException(parameterName);

    [DebuggerStepThrough]
    public static void NotNull([ValidatedNotNull] Task value, string parameterName)
    {
      if (value == null)
        throw new ArgumentNullException(parameterName);
    }

    [DebuggerStepThrough]
    public static void NotNull<T>([ValidatedNotNull] Task<T> value, string parameterName)
    {
      if (value == null)
        throw new ArgumentNullException(parameterName);
    }

    [DebuggerStepThrough]
    public static T NotNullAllowStructs<T>([ValidatedNotNull] T value, string parameterName) => (object) value != null ? value : throw new ArgumentNullException(parameterName);

    [DebuggerStepThrough]
    public static void NotNullOrEmpty([ValidatedNotNull] string value, string parameterName)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(parameterName);
        case "":
          throw new ArgumentException(Requires.Format(Strings.Argument_EmptyString, (object) parameterName), parameterName);
        default:
          if (value[0] != char.MinValue)
            break;
          goto case "";
      }
    }

    [DebuggerStepThrough]
    public static void NotNullOrWhiteSpace([ValidatedNotNull] string value, string parameterName)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(parameterName);
        case "":
          throw new ArgumentException(Requires.Format(Strings.Argument_EmptyString, (object) parameterName), parameterName);
        default:
          if (value[0] != char.MinValue)
          {
            if (!string.IsNullOrWhiteSpace(value))
              break;
            throw new ArgumentException(Requires.Format(Strings.Argument_Whitespace, (object) parameterName));
          }
          goto case "";
      }
    }

    [DebuggerStepThrough]
    public static void NotNullOrEmpty([ValidatedNotNull] IEnumerable values, string parameterName)
    {
      if (values == null)
        throw new ArgumentNullException(parameterName);
      if (values is ICollection collection)
      {
        if (collection.Count <= 0)
          throw new ArgumentException(Requires.Format(Strings.Argument_EmptyArray, (object) parameterName), parameterName);
      }
      else
      {
        IEnumerator enumerator = values.GetEnumerator();
        using (enumerator as IDisposable)
        {
          if (enumerator.MoveNext())
            return;
        }
        throw new ArgumentException(Requires.Format(Strings.Argument_EmptyArray, (object) parameterName), parameterName);
      }
    }

    [DebuggerStepThrough]
    public static void NotNullEmptyOrNullElements<T>([ValidatedNotNull] IEnumerable<T> values, string parameterName) where T : class
    {
      Requires.NotNull<IEnumerable<T>>(values, parameterName);
      bool flag = false;
      foreach (T obj in values)
      {
        flag = true;
        if ((object) obj == null)
          throw new ArgumentException(Requires.Format(Strings.Argument_NullElement, (object) parameterName), parameterName);
      }
      if (!flag)
        throw new ArgumentException(Requires.Format(Strings.Argument_EmptyArray, (object) parameterName), parameterName);
    }

    [DebuggerStepThrough]
    public static void NullOrNotNullElements<T>(IEnumerable<T> values, string parameterName)
    {
      if (values == null)
        return;
      foreach (T obj in values)
      {
        if ((object) obj == null)
          throw new ArgumentException(Requires.Format(Strings.Argument_NullElement, (object) parameterName), parameterName);
      }
    }

    [DebuggerStepThrough]
    public static void Range(bool condition, string parameterName, string message = null)
    {
      if (condition)
        return;
      Requires.FailRange(parameterName, message);
    }

    [DebuggerStepThrough]
    public static Exception FailRange(string parameterName, string message = null)
    {
      if (string.IsNullOrEmpty(message))
        throw new ArgumentOutOfRangeException(parameterName);
      throw new ArgumentOutOfRangeException(parameterName, message);
    }

    [DebuggerStepThrough]
    public static void Argument(bool condition, string parameterName, string message)
    {
      if (!condition)
        throw new ArgumentException(message, parameterName);
    }

    [DebuggerStepThrough]
    public static void Argument(bool condition, string parameterName, string message, object arg1)
    {
      if (!condition)
        throw new ArgumentException(Requires.Format(message, arg1), parameterName);
    }

    [DebuggerStepThrough]
    public static void Argument(
      bool condition,
      string parameterName,
      string message,
      object arg1,
      object arg2)
    {
      if (!condition)
        throw new ArgumentException(Requires.Format(message, arg1, arg2), parameterName);
    }

    [DebuggerStepThrough]
    public static void Argument(
      bool condition,
      string parameterName,
      string message,
      params object[] args)
    {
      if (!condition)
        throw new ArgumentException(Requires.Format(message, args), parameterName);
    }

    [DebuggerStepThrough]
    public static void That(
      bool condition,
      string parameterName,
      string unformattedMessage,
      params object[] args)
    {
      if (!condition)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, unformattedMessage, args), parameterName);
    }

    [DebuggerStepThrough]
    public static void ValidState(bool condition, string message)
    {
      if (!condition)
        throw new InvalidOperationException(message);
    }

    [DebuggerStepThrough]
    public static Exception Fail(string message) => throw new ArgumentException(message);

    [DebuggerStepThrough]
    public static Exception Fail(string unformattedMessage, params object[] args) => throw Requires.Fail(Requires.Format(unformattedMessage, args));

    [DebuggerStepThrough]
    public static Exception Fail(
      Exception innerException,
      string unformattedMessage,
      params object[] args)
    {
      throw new ArgumentException(Requires.Format(unformattedMessage, args), innerException);
    }

    private static string Format(string format, params object[] arguments) => PrivateErrorHelpers.Format(format, arguments);
  }
}
