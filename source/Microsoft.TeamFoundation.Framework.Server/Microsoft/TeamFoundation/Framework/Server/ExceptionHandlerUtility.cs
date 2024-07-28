// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExceptionHandlerUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ExceptionHandlerUtility
  {
    private static IVssExceptionHandler s_instance;

    internal static void Initialize(IVssExceptionHandler exceptionHandler) => ExceptionHandlerUtility.s_instance = exceptionHandler;

    public static void HandleException(Exception exception) => ExceptionHandlerUtility.GetExceptionHandler().HandleException(exception);

    public static void ReportException(
      string watsonReportingName,
      string eventCategory,
      Exception exception,
      string[] additionalInfo)
    {
      ExceptionHandlerUtility.GetExceptionHandler().ReportException(watsonReportingName, eventCategory, exception, additionalInfo);
    }

    private static IVssExceptionHandler GetExceptionHandler()
    {
      if (ExceptionHandlerUtility.s_instance == null)
        ExceptionHandlerUtility.s_instance = (IVssExceptionHandler) new ExceptionHandler();
      return ExceptionHandlerUtility.s_instance;
    }
  }
}
