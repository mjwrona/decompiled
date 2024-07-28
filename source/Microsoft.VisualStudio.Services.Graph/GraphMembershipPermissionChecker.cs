// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipPermissionChecker
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphMembershipPermissionChecker
  {
    private static readonly string MembershipTraversalsRootToken = "MembershipTraversals";
    private const string c_area = "Graph";
    private const string c_layer = "GraphMembershipPermissionChecker";

    public static void CheckReadMembershipsPermission(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      GraphMembershipPermissionChecker.CheckReadMembershipsPermission(context, groupDescriptor);
      GraphMembershipPermissionChecker.CheckReadMembershipsPermission(context, memberDescriptor);
    }

    public static void CheckReadMembershipsPermission(
      IVssRequestContext context,
      SubjectDescriptor descriptor)
    {
      context.TraceEnter(15270000, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckReadMembershipsPermission));
      context.TraceDataConditionally(15270001, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "Received input parameters", (Func<object>) (() => (object) new
      {
        descriptor = descriptor
      }), nameof (CheckReadMembershipsPermission));
      try
      {
        if (context.IsSystemContext)
          context.Trace(15270002, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "CheckReadMembershipsPermission succeeded because caller is a system context");
        else if (context.UserContext.ToSubjectDescriptor(context) == descriptor)
          context.Trace(15270003, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "CheckReadMembershipsPermission succeeded because caller is reading themselves");
        else if (GraphMembershipPermissionChecker.HasPermission(context, FrameworkSecurity.IdentitySecurityRootToken, 1, false))
          context.Trace(15270004, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "CheckReadMembershipsPermission succeeded because caller has Read permission on IdentitySecurityRootToken");
        else
          GraphMembershipPermissionChecker.CheckPermission(context, context.ServiceHost.InstanceId.ToString(), 1, true);
      }
      finally
      {
        context.TraceLeave(15270009, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckReadMembershipsPermission));
      }
    }

    public static void CheckWriteMembershipsPermission(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      context.TraceEnter(15270340, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckWriteMembershipsPermission));
      context.TraceDataConditionally(15270341, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "Received input parameters", (Func<object>) (() => (object) new
      {
        groupDescriptor = groupDescriptor,
        memberDescriptor = memberDescriptor
      }), nameof (CheckWriteMembershipsPermission));
      try
      {
        try
        {
          GraphMembershipPermissionChecker.CheckPermission(context, context.ServiceHost.InstanceId.ToString(), 8, false);
        }
        catch (AccessCheckException ex)
        {
          GraphMembershipPermissionChecker.CheckPermission(context, context.ServiceHost.InstanceId.ToString(), 8, true);
          context.TraceAlways(15270353, TraceLevel.Info, "Graph", nameof (GraphMembershipPermissionChecker), string.Format("Check group permission worked with alwaysAllowAdministrators flag set to true but failed with the flag set to false for user: {0}, group: {1}", (object) context.UserContext, (object) groupDescriptor));
        }
        IdentityPermissionHelper.CheckUpdateMembershipPermissionsOnJITManagedOrganizations(context, groupDescriptor);
        GraphMembershipPermissionChecker.CheckReadMembershipsPermission(context, memberDescriptor);
      }
      finally
      {
        context.TraceLeave(15270349, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckWriteMembershipsPermission));
      }
    }

    public static void CheckMembershipTraversalsPermission(
      IVssRequestContext context,
      int depth,
      GraphTraversalDirection direction)
    {
      string membershipTraversalsToken = GraphMembershipPermissionChecker.GenerateMembershipTraversalsToken(depth, direction);
      GraphMembershipPermissionChecker.CheckPermission(context, membershipTraversalsToken, 1, false);
      context.Trace(15270350, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "CheckMembershipTraversalsPermission succeeded because caller has Read permission on token");
    }

    public static string GenerateMembershipTraversalsToken(
      int depth,
      GraphTraversalDirection direction)
    {
      if (direction == GraphTraversalDirection.Unknown)
        throw new GraphBadRequestException(Resources.UnsupportedGraphTraversalDirection());
      string str1 = "";
      string str2;
      if (depth == 1)
      {
        str2 = GraphMembershipPermissionChecker.TraversalDepth.Direct;
      }
      else
      {
        if (depth != -1)
          throw new GraphBadRequestException(Resources.UnsupportedTraversedDescendantsDepth((object) str1));
        str2 = GraphMembershipPermissionChecker.TraversalDepth.Expanded;
      }
      return string.Format("{0}{1}{2}{3}{4}", (object) GraphMembershipPermissionChecker.MembershipTraversalsRootToken, (object) FrameworkSecurity.RegistryPathSeparator, (object) str2, (object) FrameworkSecurity.RegistryPathSeparator, (object) direction);
    }

    private static bool HasPermission(
      IVssRequestContext context,
      string token,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      context.TraceEnter(15270010, "Graph", nameof (GraphMembershipPermissionChecker), nameof (HasPermission));
      context.TraceDataConditionally(15270011, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "Received input parameters", (Func<object>) (() => (object) new
      {
        token = token,
        requiredPermissions = requiredPermissions,
        alwaysAllowAdministrators = alwaysAllowAdministrators
      }), nameof (HasPermission));
      try
      {
        bool hasPermission = GraphMembershipPermissionChecker.EvaluatePermissionsOnIdentitiesNamespace<bool>(context, (Func<IVssRequestContext, IVssSecurityNamespace, bool>) ((securityContext, identitiesNamespace) => identitiesNamespace.HasPermission(securityContext, token, requiredPermissions, alwaysAllowAdministrators)));
        context.TraceDataConditionally(15270012, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "Evaluated permissions on identities namespace", (Func<object>) (() => (object) new
        {
          hasPermission = hasPermission
        }), nameof (HasPermission));
        return hasPermission;
      }
      finally
      {
        context.TraceLeave(15270019, "Graph", nameof (GraphMembershipPermissionChecker), nameof (HasPermission));
      }
    }

    private static void CheckPermission(
      IVssRequestContext context,
      string token,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      context.TraceEnter(15270020, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckPermission));
      context.TraceDataConditionally(15270021, TraceLevel.Verbose, "Graph", nameof (GraphMembershipPermissionChecker), "Received input parameters", (Func<object>) (() => (object) new
      {
        token = token,
        requiredPermissions = requiredPermissions,
        alwaysAllowAdministrators = alwaysAllowAdministrators
      }), nameof (CheckPermission));
      try
      {
        GraphMembershipPermissionChecker.EvaluatePermissionsOnIdentitiesNamespace<bool>(context, (Func<IVssRequestContext, IVssSecurityNamespace, bool>) ((securityContext, identitiesNamespace) =>
        {
          identitiesNamespace.CheckPermission(securityContext, token, requiredPermissions, alwaysAllowAdministrators);
          return true;
        }));
      }
      finally
      {
        context.TraceLeave(15270029, "Graph", nameof (GraphMembershipPermissionChecker), nameof (CheckPermission));
      }
    }

    private static TResult EvaluatePermissionsOnIdentitiesNamespace<TResult>(
      IVssRequestContext context,
      Func<IVssRequestContext, IVssSecurityNamespace, TResult> evaluatePermissions)
    {
      IVssRequestContext vssRequestContext = context.ExecutionEnvironment.IsOnPremisesDeployment ? context : context.To(TeamFoundationHostType.Application);
      IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.IdentitiesNamespaceId);
      return evaluatePermissions(vssRequestContext, securityNamespace);
    }

    private static class TraversalDepth
    {
      public static readonly string Direct = nameof (Direct);
      public static readonly string Expanded = nameof (Expanded);
    }
  }
}
