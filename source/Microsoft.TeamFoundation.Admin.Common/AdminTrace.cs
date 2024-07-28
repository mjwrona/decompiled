// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AdminTrace
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.TeamFoundation.Common;
using System;

namespace Microsoft.TeamFoundation.Admin
{
  public static class AdminTrace
  {
    public static void RegisterPassword(string password)
    {
    }

    public static void Error(string message) => AdminTraceLogger.Default.Error(message);

    public static void Error(string message, params object[] args) => AdminTraceLogger.Default.Error(message, args);

    public static void Error(Exception exception) => AdminTraceLogger.Default.Error(exception);

    public static void Warning(string message) => AdminTraceLogger.Default.Warning(message);

    public static void Warning(string message, params object[] args) => AdminTraceLogger.Default.Warning(message, args);

    public static void Warning(Exception exception) => AdminTraceLogger.Default.Warning(exception);

    public static void Info(string message) => AdminTraceLogger.Default.Info(message);

    public static void Info(string message, params object[] args) => AdminTraceLogger.Default.Info(message, args);

    public static void Verbose(string message) => AdminTraceLogger.Default.Verbose(message);

    public static void Verbose(string message, params object[] args) => AdminTraceLogger.Default.Verbose(message, args);

    public static void Heading(string message) => AdminTraceLogger.Default.Heading(message);

    public static void Heading2(string message) => AdminTraceLogger.Default.Heading2(message);

    public static void Enter(string message) => AdminTraceLogger.Default.Enter(message);

    public static void Exit(string message) => AdminTraceLogger.Default.Exit(message);
  }
}
