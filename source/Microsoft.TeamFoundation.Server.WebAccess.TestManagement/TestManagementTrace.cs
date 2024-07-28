// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementTrace
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal static class TestManagementTrace
  {
    public const string TraceArea = "WebAccess.TestManagement";

    internal static void TraceVerbose(
      this TestManagerRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.TestRequestContext.RequestContext, 0, TraceLevel.Verbose, "WebAccess.TestManagement", layer, messageFormat, args);
    }

    internal static void TraceInfo(
      this TestManagerRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.TestRequestContext.RequestContext, 0, TraceLevel.Info, "WebAccess.TestManagement", layer, messageFormat, args);
    }

    internal static void TraceWarning(
      this TestManagerRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.TestRequestContext.RequestContext, 0, TraceLevel.Warning, "WebAccess.TestManagement", layer, messageFormat, args);
    }

    internal static void TraceError(
      this TestManagerRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.TestRequestContext.RequestContext, 0, TraceLevel.Error, "WebAccess.TestManagement", layer, messageFormat, args);
    }

    internal static void TraceException(
      this TestManagerRequestContext context,
      string layer,
      Exception ex)
    {
      context.TestRequestContext.RequestContext.TraceException(0, TraceLevel.Error, "WebAccess.TestManagement", layer, ex);
    }

    internal static void TraceEnter(
      this TestManagerRequestContext context,
      string layer,
      string methodName)
    {
      context.TestRequestContext.RequestContext.TraceEnter(0, "WebAccess.TestManagement", layer, methodName);
    }

    internal static void TraceLeave(
      this TestManagerRequestContext context,
      string layer,
      string methodName)
    {
      context.TestRequestContext.RequestContext.TraceLeave(0, "WebAccess.TestManagement", layer, methodName);
    }

    internal static void Trace(
      this TestManagerRequestContext context,
      int tracePoint,
      TraceLevel traceLevel,
      string traceArea,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.TestRequestContext.RequestContext, tracePoint, traceLevel, traceArea, layer, messageFormat, args);
    }
  }
}
