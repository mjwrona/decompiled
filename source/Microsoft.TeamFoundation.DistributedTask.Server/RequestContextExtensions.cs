// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class RequestContextExtensions
  {
    public static bool IsScopedAgentToken(this IVssRequestContext requestContext)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (EvaluationPrincipal evaluationPrincipal in requestContext.GetUserEvaluationPrincipals())
      {
        if (evaluationPrincipal.PrimaryDescriptor.IsServiceIdentityType())
        {
          flag1 = true;
        }
        else
        {
          foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) evaluationPrincipal.RoleDescriptors)
          {
            if (roleDescriptor.IsSystemAccessControlType())
            {
              flag2 = true;
              break;
            }
          }
        }
      }
      return flag1 & flag2;
    }

    public static string GetScopeListString(this IVssRequestContext requestContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (EvaluationPrincipal evaluationPrincipal in requestContext.GetUserEvaluationPrincipals())
      {
        foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) evaluationPrincipal.RoleDescriptors)
        {
          if (roleDescriptor.IsSystemAccessControlType())
            stringBuilder.AppendLine(roleDescriptor.Identifier);
        }
      }
      return stringBuilder.ToString();
    }

    public static string BuildHyperlink(this IVssRequestContext requestContext)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      return service.LocationForAccessMapping(requestContext, string.Empty, RelativeToSetting.Context, service.GetPublicAccessMapping(requestContext));
    }

    public static PackageVersion GetRecommendedAgentPackageVersion(
      this IVssRequestContext requestContext)
    {
      IPackageMetadataService service = requestContext.GetService<IPackageMetadataService>();
      return service.GetLatestPackageVersion(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV2WindowsPlatformName) ?? service.GetLatestPackageVersion(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV1WindowsPlatformName);
    }

    public static IVssRequestContext ToPoolRequestContext(this IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return requestContext.To(TeamFoundationHostType.Deployment);
      requestContext.CheckProjectCollectionRequestContext();
      return requestContext;
    }

    public static void TraceError(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Error, "DistributedTask", layer, format, arguments);
    }

    public static void TraceError(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Error, "DistributedTask", layer, format, arguments);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(0, "DistributedTask", layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(tracepoint, "DistributedTask", layer, exception);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Info, "DistributedTask", layer, format, arguments);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Info, "DistributedTask", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Warning, "DistributedTask", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Warning, "DistributedTask", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Verbose, "DistributedTask", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Verbose, "DistributedTask", layer, format, arguments);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(0, "DistributedTask", layer, method);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(tracepoint, "DistributedTask", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(0, "DistributedTask", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(tracepoint, "DistributedTask", layer, method);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable TraceSlowCall(
      this IVssRequestContext requestContext,
      TimeSpan timeSpan,
      string layer,
      string format,
      params object[] args)
    {
      return (IDisposable) new TraceWatch(requestContext, 10015150, TraceLevel.Error, timeSpan, "DistributedTask", layer, format, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable TraceScope(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      return (IDisposable) new MethodScope(requestContext, layer, method);
    }
  }
}
