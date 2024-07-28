// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class RequestContextExtensions
  {
    private const string c_mobileCenterEntryPoint = "MobileCenter";

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

    public static void TraceAlways(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      requestContext.TraceAlways(tracepoint, TraceLevel.Info, "DistributedTask", layer, format, arguments);
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

    public static bool TryGetIsMobileCenter(this IVssRequestContext requestContext)
    {
      bool isMobileCenter = false;
      try
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
        if (organization == null)
          isMobileCenter = false;
        else if (object.Equals((object) organization.TenantId, (object) TaskConstants.MobileCenterIntTenantId) || object.Equals((object) organization.TenantId, (object) TaskConstants.MobileCenterStagingTenantId) || object.Equals((object) organization.TenantId, (object) TaskConstants.MobileCenterProdTenantId))
        {
          isMobileCenter = true;
        }
        else
        {
          string b;
          if (requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[1]
          {
            "Microsoft.VisualStudio.Services.Account.SignupEntryPoint"
          }).Properties.TryGetValue<string>("Microsoft.VisualStudio.Services.Account.SignupEntryPoint", out b))
            isMobileCenter = string.Equals("MobileCenter", b);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException("CustomerIntelligence", ex);
      }
      return isMobileCenter;
    }

    public static bool UsesCustomScopeToken(this IVssRequestContext requestContext)
    {
      foreach (EvaluationPrincipal evaluationPrincipal in requestContext.GetUserEvaluationPrincipals())
      {
        foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) evaluationPrincipal.RoleDescriptors)
        {
          if (roleDescriptor.IsSystemAccessControlType())
            return true;
        }
      }
      return false;
    }
  }
}
