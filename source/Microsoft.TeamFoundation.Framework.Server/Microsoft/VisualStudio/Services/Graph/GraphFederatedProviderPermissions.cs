// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphFederatedProviderPermissions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphFederatedProviderPermissions
  {
    private const string Area = "Graph";
    private const string Layer = "GraphFederatedProviderPermissions";

    public static bool HasPermission(
      IVssRequestContext context,
      string providerName,
      IdentityDescriptor identityDescriptor)
    {
      bool result = true;
      if (IdentityDescriptorComparer.Instance.Equals(context.UserContext, identityDescriptor))
      {
        context.TraceDataConditionally(90010112, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Allowed when calling for self", (Func<object>) (() => (object) new
        {
          providerName = providerName,
          identityDescriptor = identityDescriptor,
          UserContext = context.UserContext,
          result = result
        }), nameof (HasPermission));
        return result;
      }
      if (context.UserContext == (IdentityDescriptor) null)
      {
        context.TraceDataConditionally(90010115, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Allowed when calling during authentication (user context not yet set)", (Func<object>) (() => (object) new
        {
          providerName = providerName,
          identityDescriptor = identityDescriptor,
          UserContext = context.UserContext,
          result = result
        }), nameof (HasPermission));
        return result;
      }
      if (context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, FrameworkSecurity.FrameworkNamespaceId).HasPermission(context, FrameworkSecurity.FrameworkNamespaceToken, 4))
      {
        context.TraceDataConditionally(90010113, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Allowed for callers with impersonate permission", (Func<object>) (() => (object) new
        {
          providerName = providerName,
          identityDescriptor = identityDescriptor,
          UserContext = context.UserContext,
          result = result
        }), nameof (HasPermission));
        return result;
      }
      result = false;
      context.TraceDataConditionally(90010114, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Otherwise disallowed", (Func<object>) (() => (object) new
      {
        providerName = providerName,
        identityDescriptor = identityDescriptor,
        UserContext = context.UserContext,
        result = result
      }), nameof (HasPermission));
      return result;
    }

    public static void CheckPermission(
      IVssRequestContext context,
      string providerName,
      IdentityDescriptor identityDescriptor)
    {
      if (IdentityDescriptorComparer.Instance.Equals(context.UserContext, identityDescriptor))
        context.TraceDataConditionally(90010122, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Allowed when calling for self", (Func<object>) (() => (object) new
        {
          providerName = providerName,
          identityDescriptor = identityDescriptor,
          UserContext = context.UserContext
        }), nameof (CheckPermission));
      else if (context.UserContext == (IdentityDescriptor) null)
        context.TraceDataConditionally(90010125, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderPermissions), "Allowed when calling during authentication (user context not yet set)", (Func<object>) (() => (object) new
        {
          providerName = providerName,
          identityDescriptor = identityDescriptor,
          UserContext = context.UserContext
        }), nameof (CheckPermission));
      else
        context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(context, FrameworkSecurity.FrameworkNamespaceToken, 4);
    }
  }
}
