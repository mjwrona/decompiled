// Decompiled with JetBrains decompiler
// Type: Validation.Report
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Diagnostics;

namespace Validation
{
  public static class Report
  {
    [DebuggerStepThrough]
    public static void IfNotPresent<T>(T part)
    {
      if ((object) part != null)
        return;
      Report.Fail(Strings.ServiceMissing, (object) PrivateErrorHelpers.TrimGenericWrapper(typeof (T), typeof (Lazy<>)).FullName);
    }

    [DebuggerStepThrough]
    public static void If(bool condition, string message = null)
    {
      if (!condition)
        return;
      Report.Fail(message);
    }

    [DebuggerStepThrough]
    public static void IfNot(bool condition, string message = null)
    {
      if (condition)
        return;
      Report.Fail(message);
    }

    [DebuggerStepThrough]
    public static void IfNot(bool condition, string message, object arg1)
    {
      if (condition)
        return;
      Report.Fail(PrivateErrorHelpers.Format(message, arg1));
    }

    [DebuggerStepThrough]
    public static void IfNot(bool condition, string message, object arg1, object arg2)
    {
      if (condition)
        return;
      Report.Fail(PrivateErrorHelpers.Format(message, arg1, arg2));
    }

    [DebuggerStepThrough]
    public static void IfNot(bool condition, string message, params object[] args)
    {
      if (condition)
        return;
      Report.Fail(PrivateErrorHelpers.Format(message, args));
    }

    [DebuggerStepThrough]
    public static void Fail(string message = null)
    {
      if (message != null)
        return;
      message = "A recoverable error has been detected.";
    }

    [DebuggerStepThrough]
    public static void Fail(string message, params object[] args) => Report.Fail(PrivateErrorHelpers.Format(message, args));
  }
}
