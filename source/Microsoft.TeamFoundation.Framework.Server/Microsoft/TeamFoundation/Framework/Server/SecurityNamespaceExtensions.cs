// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SecurityNamespaceExtensions
  {
    private const string c_area = "Security";
    private const string c_layer = "SecurityNamespaceExtensions";

    public static bool IsHierarchical(this IVssSecurityNamespace securityNamespace) => securityNamespace.Description.NamespaceStructure == SecurityNamespaceStructure.Hierarchical;

    public static IVssSecurityNamespace Secured(this IVssSecurityNamespace securityNamespace) => securityNamespace is SecuredSecurityNamespace ? securityNamespace : (IVssSecurityNamespace) new SecuredSecurityNamespace(securityNamespace);

    public static IVssSecurityNamespace Unsecured(this IVssSecurityNamespace securityNamespace) => securityNamespace is SecuredSecurityNamespace securityNamespace1 ? securityNamespace1.UnsecuredNamespace : securityNamespace;

    public static void CheckPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      EvaluationPrincipal failingPrincipal;
      if (securityNamespace.HasPermissionExpect(requestContext, token, requestedPermissions, true, true, out failingPrincipal, alwaysAllowAdministrators))
        return;
      securityNamespace.ThrowAccessDeniedException(requestContext, token, requestedPermissions, failingPrincipal);
    }

    public static void CheckPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
      foreach (string token in tokens)
        securityNamespace.CheckPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators);
    }

    public static bool HasPermissionExpect(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool expectedResult,
      bool alwaysAllowAdministrators = true)
    {
      return securityNamespace.HasPermissionExpect(requestContext, token, requestedPermissions, expectedResult, false, out EvaluationPrincipal _, alwaysAllowAdministrators);
    }

    public static bool HasPermissionExpect(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool expectedResult,
      bool logSecurityEvaluation,
      out EvaluationPrincipal failingPrincipal,
      bool alwaysAllowAdministrators = true)
    {
      bool flag = false;
      try
      {
        if (expectedResult != securityNamespace.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators))
        {
          securityNamespace.PollForRequestLocalInvalidation(requestContext);
          if (logSecurityEvaluation && !requestContext.IsTracing(55555, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions)))
          {
            SecuritySettingsService.SecurityServiceSettings settings = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>()?.Settings;
            int num = settings != null ? settings.PermissionTracingSamplingDivisor : 8;
            logSecurityEvaluation = num != 0 && requestContext.ContextId % (long) num == 0L;
          }
          StringBuilder stringBuilder = (StringBuilder) null;
          if (logSecurityEvaluation)
          {
            if (!requestContext.RootContext.Items.TryGetValue<StringBuilder>(RequestContextItemsKeys.SecurityEvaluationLogKey, out stringBuilder))
            {
              flag = true;
              stringBuilder = new StringBuilder();
              requestContext.RootContext.Items[RequestContextItemsKeys.SecurityEvaluationLogKey] = (object) stringBuilder;
            }
            SecurityServiceHelpers.ClearCachedSecurityEvaluators(requestContext);
          }
          if (expectedResult != securityNamespace.HasPermission(requestContext, token, requestedPermissions, out failingPrincipal, alwaysAllowAdministrators))
          {
            if (stringBuilder != null)
              requestContext.TraceAlwaysInChunks(55555, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), stringBuilder);
            return !expectedResult;
          }
        }
        failingPrincipal = (EvaluationPrincipal) null;
        return expectedResult;
      }
      finally
      {
        if (flag)
          requestContext.RootContext.Items.Remove(RequestContextItemsKeys.SecurityEvaluationLogKey);
      }
    }

    public static bool HasPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      return securityNamespace.HasPermission(requestContext, token, requestedPermissions, out EvaluationPrincipal _, alwaysAllowAdministrators);
    }

    public static bool? GetPermissionState(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext1,
      string token1,
      int requestedPermissions1,
      bool alwaysAllowAdministrators1 = true)
    {
      return securityNamespace.HasPermissionOnActors(requestContext1, requestContext1.IsSystemContext ? (IEnumerable<IRequestActor>) null : (IEnumerable<IRequestActor>) requestContext1.RequestContextInternal().Actors, token1, requestedPermissions1, (Func<IVssRequestContext, EvaluationPrincipal, string, int, bool, bool?>) ((requestContext2, principal, token2, requestedPermissions2, alwaysAllowAdministrators2) => securityNamespace.QueryableSecurityNamespaceInternal().GetPrincipalPermissionState(requestContext2, principal, token2, requestedPermissions2, alwaysAllowAdministrators2)), out EvaluationPrincipal _, alwaysAllowAdministrators1, false);
    }

    public static bool HasPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      out EvaluationPrincipal failingPrincipal,
      bool alwaysAllowAdministrators = true)
    {
      return securityNamespace.HasPermissionOnActors(requestContext, requestContext.IsSystemContext ? (IEnumerable<IRequestActor>) null : (IEnumerable<IRequestActor>) requestContext.RequestContextInternal().Actors, token, requestedPermissions, out failingPrincipal, alwaysAllowAdministrators);
    }

    internal static bool HasPermissionOnActors(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext1,
      IEnumerable<IRequestActor> actors,
      string token1,
      int requestedPermissions1,
      out EvaluationPrincipal failingPrincipal,
      bool alwaysAllowAdministrators1 = true)
    {
      return securityNamespace.HasPermissionOnActors(requestContext1, actors, token1, requestedPermissions1, (Func<IVssRequestContext, EvaluationPrincipal, string, int, bool, bool?>) ((requestContext2, principal, token2, requestedPermissions2, alwaysAllowAdministrators2) => new bool?(securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(requestContext2, principal, token2, requestedPermissions2, alwaysAllowAdministrators2))), out failingPrincipal, alwaysAllowAdministrators1).Value;
    }

    private static bool? HasPermissionOnActors(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<IRequestActor> actors,
      string token,
      int requestedPermissions,
      Func<IVssRequestContext, EvaluationPrincipal, string, int, bool, bool?> checkPermissionLogic,
      out EvaluationPrincipal failingPrincipal,
      bool alwaysAllowAdministrators = true,
      bool throwExceptionForNotSetPermissionValue = true)
    {
      requestContext.TraceEnter(56000, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionOnActors));
      failingPrincipal = (EvaluationPrincipal) null;
      try
      {
        bool flag = requestContext.IsTracingSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions));
        securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(requestContext);
        token = securityNamespace.CheckAndCanonicalizeToken(token);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("Beginning {0} on token '{1}' for permissions {2} in namespace {3} ", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) token, (object) requestedPermissions, (object) securityNamespace.Description.NamespaceId) + string.Format("({0}). Value of alwaysAllowAdministrators: {1}.", (object) securityNamespace.Description.DisplayName, (object) alwaysAllowAdministrators));
        if (requestContext.IsSystemContext)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)) + ": Returning true because the request context is a system context.");
          return new bool?(true);
        }
        if (actors == null || !actors.Any<IRequestActor>((Func<IRequestActor, bool>) (s => s.Principals.Count > 0)))
          return new bool?(false);
        if (requestedPermissions == 0)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)) + ": Returning true because no permission bits were demanded.");
          return new bool?(true);
        }
        bool? nullable1 = new bool?();
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} actors on the request. Actor identifiers: {2}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) actors.Count<IRequestActor>(), (object) string.Join<Guid>(", ", actors.Select<IRequestActor, Guid>((Func<IRequestActor, Guid>) (x => x.Identifier)))));
        foreach (IRequestActor actor in actors)
        {
          if (actor.Principals.Count == 0)
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Actor {1} has zero EvaluationPrincipals; skipping the actor.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) actor.Identifier));
          }
          else
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} EvaluationPrincipals on the actor {2}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) actor.Principals.Count, (object) actor.Identifier));
            foreach (EvaluationPrincipal evaluationPrincipal in actor.Principals.Values)
            {
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)) + ": Invoking PrincipalHasPermission for EvaluationPrincipal: " + SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal) + ".");
              bool? nullable2 = checkPermissionLogic(requestContext, evaluationPrincipal, token, requestedPermissions, alwaysAllowAdministrators);
              if (nullable2.HasValue)
                nullable1 = new bool?(nullable2.Value);
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluation result from PrincipalHasPermission: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) nullable2));
              if (nullable1.HasValue && !nullable1.Value)
              {
                failingPrincipal = evaluationPrincipal;
                if (flag)
                {
                  requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)) + ": Terminating evaluation due to false result from PrincipalHasPermission.");
                  break;
                }
                break;
              }
            }
            if (nullable1.HasValue)
            {
              if (!nullable1.Value)
                break;
            }
          }
        }
        if (!nullable1.HasValue & throwExceptionForNotSetPermissionValue)
          throw new InvalidOperationException();
        if (nullable1.HasValue && nullable1.Value)
          SecurityNamespaceExtensions.TagPermissionCheck(requestContext, securityNamespace, token, requestedPermissions);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Final result: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionOnActors)), (object) nullable1));
        return nullable1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56013, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56001, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionOnActors));
      }
    }

    private static void TagPermissionCheck(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      int requestedPermissions)
    {
      TrackedSecurityCollection securityCollection;
      if (!requestContext.RootContext.Items.TryGetValue<TrackedSecurityCollection>(RequestContextItemsKeys.SecurityTracking, out securityCollection))
        return;
      securityCollection.Add(securityNamespace, token, requestedPermissions);
    }

    public static IEnumerable<bool> HasPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      return securityNamespace.HasPermissionOnActors(requestContext, requestContext.IsSystemContext ? (IEnumerable<IRequestActor>) null : (IEnumerable<IRequestActor>) requestContext.RequestContextInternal().Actors, tokens, requestedPermissions, alwaysAllowAdministrators);
    }

    internal static IEnumerable<bool> HasPermissionOnActors(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<IRequestActor> actors,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool alwaysAllowAdministrators = true)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
      List<bool> boolList = new List<bool>();
      foreach (string token in tokens)
        boolList.Add(securityNamespace.HasPermissionOnActors(requestContext, actors, token, requestedPermissions, out EvaluationPrincipal _, alwaysAllowAdministrators));
      return (IEnumerable<bool>) boolList;
    }

    public static bool HasPermissionForAllChildren(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound = true,
      bool alwaysAllowAdministrators = true)
    {
      requestContext.TraceEnter(56100, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionForAllChildren));
      try
      {
        bool flag = requestContext.IsTracing(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions));
        securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(requestContext);
        token = securityNamespace.CheckAndCanonicalizeToken(token);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("Beginning {0} on token '{1}' for permissions {2} in namespace {3} ", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) token, (object) requestedPermissions, (object) securityNamespace.Description.NamespaceId) + string.Format("({0}). Value of alwaysAllowAdministrators: {1}. ", (object) securityNamespace.Description.DisplayName, (object) alwaysAllowAdministrators) + string.Format("Value of resultIfNoChildrenFound: {0}.", (object) resultIfNoChildrenFound));
        if (requestContext.IsSystemContext)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)) + ": Returning true because the request context is a system context.");
          return true;
        }
        IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
        if (actors == null || !actors.Any<IRequestActor>((Func<IRequestActor, bool>) (s => s.Principals.Count > 0)))
          return false;
        if (requestedPermissions == 0)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)) + ": Returning true because no permission bits were demanded.");
          return true;
        }
        bool? nullable = new bool?();
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} actors on the request.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) actors.Count));
        foreach (IRequestActor requestActor in (IEnumerable<IRequestActor>) actors)
        {
          if (requestActor.Principals.Count == 0)
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Actor {1} has zero EvaluationPrincipals; skipping the actor.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) requestActor.Identifier));
          }
          else
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} EvaluationPrincipals on the actor {2}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) requestActor.Principals.Count, (object) requestActor.Identifier));
            foreach (EvaluationPrincipal evaluationPrincipal in requestActor.Principals.Values)
            {
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)) + ": Invoking PrincipalHasPermissionForAllChildren for EvaluationPrincipal: " + SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal) + ".");
              nullable = new bool?(securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAllChildren(requestContext, evaluationPrincipal, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators));
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluation result from PrincipalHasPermissionForAllChildren: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) nullable));
              if (!nullable.Value)
              {
                if (flag)
                {
                  requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)) + ": Terminating evaluation due to false result from PrincipalHasPermissionForAllChildren.");
                  break;
                }
                break;
              }
            }
            if (nullable.HasValue)
            {
              if (!nullable.Value)
                break;
            }
          }
        }
        if (!nullable.HasValue)
          throw new InvalidOperationException();
        if (nullable.Value)
          SecurityNamespaceExtensions.TagPermissionCheck(requestContext, securityNamespace, token, requestedPermissions);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Final result: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAllChildren)), (object) nullable));
        return nullable.Value;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56108, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56109, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionForAllChildren));
      }
    }

    public static IEnumerable<bool> HasPermissionForAllChildren(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool resultIfNoChildrenFound = true,
      bool alwaysAllowAdministrators = true)
    {
      List<bool> boolList = new List<bool>();
      foreach (string token in tokens)
        boolList.Add(securityNamespace.HasPermissionForAllChildren(requestContext, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators));
      return (IEnumerable<bool>) boolList;
    }

    public static bool HasPermissionForAnyChildren(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound = false,
      bool alwaysAllowAdministrators = true)
    {
      requestContext.TraceEnter(56120, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionForAnyChildren));
      try
      {
        bool flag = requestContext.IsTracing(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions));
        securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(requestContext);
        token = securityNamespace.CheckAndCanonicalizeToken(token);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("Beginning {0} on token '{1}' for permissions {2} in namespace {3} ", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) token, (object) requestedPermissions, (object) securityNamespace.Description.NamespaceId) + string.Format("({0}). Value of alwaysAllowAdministrators: {1}. ", (object) securityNamespace.Description.DisplayName, (object) alwaysAllowAdministrators) + string.Format("Value of resultIfNoChildrenFound: {0}.", (object) resultIfNoChildrenFound));
        if (requestContext.IsSystemContext)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)) + ": Returning true because the request context is a system context.");
          return true;
        }
        IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
        if (actors == null || !actors.Any<IRequestActor>((Func<IRequestActor, bool>) (s => s.Principals.Count > 0)))
          return false;
        if (requestedPermissions == 0)
        {
          if (flag)
            requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)) + ": Returning true because no permission bits were demanded.");
          return true;
        }
        bool? nullable = new bool?();
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} actors on the request.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) actors.Count));
        foreach (IRequestActor requestActor in (IEnumerable<IRequestActor>) actors)
        {
          if (requestActor.Principals.Count == 0)
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Actor {1} has zero EvaluationPrincipals; skipping the actor.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) requestActor.Identifier));
          }
          else
          {
            if (flag)
              requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluating permissions for {1} EvaluationPrincipals on the actor {2}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) requestActor.Principals.Count, (object) requestActor.Identifier));
            foreach (EvaluationPrincipal evaluationPrincipal in requestActor.Principals.Values)
            {
              IQueryableSecurityNamespaceInternal namespaceInternal = securityNamespace.QueryableSecurityNamespaceInternal();
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)) + ": Invoking PrincipalHasPermission for EvaluationPrincipal: " + SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal) + ".");
              nullable = new bool?(namespaceInternal.PrincipalHasPermission(requestContext, evaluationPrincipal, token, requestedPermissions, alwaysAllowAdministrators));
              if (flag)
                requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluation result from PrincipalHasPermission: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) nullable));
              if (!nullable.Value)
              {
                if (flag)
                  requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)) + ": Since decision from PrincipalHasPermission was false, invoking PrincipalHasPermissionForAnyChildren for EvaluationPrincipal: " + SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal) + ".");
                nullable = new bool?(namespaceInternal.PrincipalHasPermissionForAnyChildren(requestContext, evaluationPrincipal, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators));
                if (flag)
                  requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Evaluation result from PrincipalHasPermissionForAnyChildren: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) nullable));
              }
              if (!nullable.Value)
              {
                if (flag)
                {
                  requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)) + ": Terminating evaluation due to false result from PrincipalHasPermissionForAnyChildren.");
                  break;
                }
                break;
              }
            }
            if (nullable.HasValue)
            {
              if (!nullable.Value)
                break;
            }
          }
        }
        if (!nullable.HasValue)
          throw new InvalidOperationException();
        if (nullable.Value)
          SecurityNamespaceExtensions.TagPermissionCheck(requestContext, securityNamespace, token, requestedPermissions);
        if (flag)
          requestContext.TraceSecurityEvaluation(56011, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), string.Format("{0}: Final result: {1}.", (object) SecurityNamespaceExtensions.GetMethodName(nameof (HasPermissionForAnyChildren)), (object) nullable));
        return nullable.Value;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56128, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56129, "Security", nameof (SecurityNamespaceExtensions), nameof (HasPermissionForAnyChildren));
      }
    }

    public static IEnumerable<bool> HasPermissionForAnyChildren(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      int requestedPermissions,
      bool resultIfNoChildrenFound = false,
      bool alwaysAllowAdministrators = true)
    {
      List<bool> boolList = new List<bool>();
      foreach (string token in tokens)
        boolList.Add(securityNamespace.HasPermissionForAnyChildren(requestContext, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators));
      return (IEnumerable<bool>) boolList;
    }

    public static bool RemoveAccessControlEntries(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries)
    {
      return securityNamespace.RemoveAccessControlEntries(requestContext, token, accessControlEntries.Select<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (s => s.Descriptor)));
    }

    public static IAccessControlEntry SetPermissions(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      return securityNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) new AccessControlEntry(descriptor, allow, deny), merge);
    }

    public static IAccessControlEntry SetAccessControlEntry(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IAccessControlEntry accessControlEntry,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      return securityNamespace.SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) new IAccessControlEntry[1]
      {
        accessControlEntry
      }, (merge ? 1 : 0) != 0, (throwOnInvalidIdentity ? 1 : 0) != 0, (rootNewIdentities ? 1 : 0) != 0).SingleOrDefault<IAccessControlEntry>();
    }

    public static IAccessControlEntry RemovePermissions(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      requestContext.TraceEnter(56160, "Security", nameof (SecurityNamespaceExtensions), nameof (RemovePermissions));
      try
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
        securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(requestContext);
        token = securityNamespace.CheckAndCanonicalizeToken(token);
        descriptor = requestContext.GetService<IdentityService>().MapToWellKnownIdentifier(descriptor);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(securityNamespace.Description.NamespaceId, token, descriptor, permissionsToRemove);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.TraceSecurityEvaluation(56165, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), "'RemovePermissions' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return (IAccessControlEntry) null;
        }
        QueriedAccessControlList accessControlList = securityNamespace.QueryAccessControlLists(requestContext, token, (IEnumerable<EvaluationPrincipal>) new EvaluationPrincipal[1]
        {
          new EvaluationPrincipal(descriptor)
        }, false, false).FirstOrDefault<QueriedAccessControlList>();
        if (accessControlList == null)
        {
          requestContext.TraceSecurityEvaluation(56164, TraceLevel.Verbose, "Security", nameof (SecurityNamespaceExtensions), "No matching entry found for token '{0}', returning an empty ACE.", (object) token);
          return (IAccessControlEntry) new AccessControlEntry(descriptor, 0, 0);
        }
        QueriedAccessControlEntry accessControlEntry1 = accessControlList.AccessControlEntries.Single<QueriedAccessControlEntry>();
        int updatedAllow;
        int updatedDeny;
        SecurityUtility.MergePermissions(accessControlEntry1.Allow, accessControlEntry1.Deny, 0, 0, permissionsToRemove, out updatedAllow, out updatedDeny);
        IAccessControlEntry accessControlEntry2;
        if (updatedAllow == 0 && updatedDeny == 0)
        {
          securityNamespace.RemoveAccessControlEntries(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptor
          });
          accessControlEntry2 = (IAccessControlEntry) new AccessControlEntry(descriptor, 0, 0);
        }
        else
        {
          securityNamespace.SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) new IAccessControlEntry[1]
          {
            (IAccessControlEntry) new AccessControlEntry(descriptor, updatedAllow, updatedDeny)
          }, false);
          accessControlEntry2 = (IAccessControlEntry) new AccessControlEntry(descriptor, updatedAllow & ~updatedDeny, updatedDeny);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
        return accessControlEntry2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56168, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56169, "Security", nameof (SecurityNamespaceExtensions), nameof (RemovePermissions));
      }
    }

    public static void EnsurePermissions(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      AccessControlEntry accessControlEntry1 = new AccessControlEntry(descriptor, allow, deny);
      IAccessControlEntry accessControlEntry2 = securityNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, false)?.QueryAccessControlEntry(descriptor);
      if (accessControlEntry2 != null && accessControlEntry1.Allow == accessControlEntry2.Allow && accessControlEntry1.Deny == accessControlEntry2.Deny)
        return;
      securityNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) accessControlEntry1, merge);
    }

    public static IAccessControlList QueryAccessControlList(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo)
    {
      return securityNamespace.QueryAccessControlLists(requestContext, token, descriptors, includeExtendedInfo, false).FirstOrDefault<IAccessControlList>() ?? (IAccessControlList) new AccessControlList(token, true);
    }

    public static IEnumerable<IAccessControlList> QueryAccessControlLists(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      bool includeExtendedInfo,
      bool recurse)
    {
      return securityNamespace.QueryAccessControlLists(requestContext, token, (IEnumerable<IdentityDescriptor>) null, includeExtendedInfo, recurse);
    }

    public static IEnumerable<IAccessControlList> QueryAccessControlLists(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo,
      bool recurse)
    {
      List<IAccessControlList> accessControlListList = new List<IAccessControlList>();
      foreach (QueriedAccessControlList accessControlList in securityNamespace.QueryableSecurityNamespaceInternal().QueryAccessControlLists(requestContext, token, descriptors != null ? descriptors.Select<IdentityDescriptor, EvaluationPrincipal>((Func<IdentityDescriptor, EvaluationPrincipal>) (s => new EvaluationPrincipal(s))) : (IEnumerable<EvaluationPrincipal>) null, includeExtendedInfo, recurse))
      {
        IEnumerable<AccessControlEntry> aces = !includeExtendedInfo ? accessControlList.AccessControlEntries.Select<QueriedAccessControlEntry, AccessControlEntry>((Func<QueriedAccessControlEntry, AccessControlEntry>) (s => new AccessControlEntry(s.IdentityDescriptor, s.Allow, s.Deny))) : accessControlList.AccessControlEntries.Select<QueriedAccessControlEntry, AccessControlEntry>((Func<QueriedAccessControlEntry, AccessControlEntry>) (s => new AccessControlEntry(s.IdentityDescriptor, s.Allow, s.Deny, s.InheritedAllow, s.InheritedDeny, s.EffectiveAllow, s.EffectiveDeny)));
        accessControlListList.Add((IAccessControlList) new AccessControlList(accessControlList.Token, accessControlList.Inherit, aces));
      }
      return (IEnumerable<IAccessControlList>) accessControlListList;
    }

    public static int QueryEffectivePermissions(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal)
    {
      requestContext.TraceEnter(56701, "Security", nameof (SecurityNamespaceExtensions), nameof (QueryEffectivePermissions));
      try
      {
        int effectiveAllow;
        securityNamespace.QueryableSecurityNamespaceInternal().QueryEffectivePermissions(requestContext, token, evaluationPrincipal, out effectiveAllow, out int _);
        effectiveAllow = securityNamespace.NamespaceExtension.QueryEffectivePermissions(requestContext, securityNamespace, token, evaluationPrincipal, effectiveAllow);
        return effectiveAllow;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56703, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56702, "Security", nameof (SecurityNamespaceExtensions), nameof (QueryEffectivePermissions));
      }
    }

    public static int QueryEffectivePermissions(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals = null)
    {
      requestContext.TraceEnter(56731, "Security", nameof (SecurityNamespaceExtensions), nameof (QueryEffectivePermissions));
      try
      {
        int num = -1;
        if (evaluationPrincipals == null)
        {
          IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
          if (actors == null || !actors.Any<IRequestActor>((Func<IRequestActor, bool>) (s => s.Principals.Count > 0)))
            return 0;
          foreach (IRequestActor requestActor in (IEnumerable<IRequestActor>) actors)
          {
            if (requestActor.Principals.Count != 0)
            {
              foreach (EvaluationPrincipal evaluationPrincipal in requestActor.Principals.Values)
              {
                num &= securityNamespace.QueryEffectivePermissions(requestContext, token, evaluationPrincipal);
                if (num == 0)
                  return 0;
              }
            }
          }
        }
        else
        {
          if (!evaluationPrincipals.Any<EvaluationPrincipal>())
            return 0;
          foreach (EvaluationPrincipal evaluationPrincipal in evaluationPrincipals)
          {
            num &= securityNamespace.QueryEffectivePermissions(requestContext, token, evaluationPrincipal);
            if (num == 0)
              return 0;
          }
        }
        return num;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56733, "Security", nameof (SecurityNamespaceExtensions), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56732, "Security", nameof (SecurityNamespaceExtensions), nameof (QueryEffectivePermissions));
      }
    }

    public static void CheckReadPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token)
    {
      securityNamespace = securityNamespace.Unsecured();
      EvaluationPrincipal failingPrincipal;
      if (securityNamespace.NamespaceExtension.HasReadPermissionExpect(requestContext, securityNamespace, token, true, out failingPrincipal))
        return;
      securityNamespace.ThrowAccessDeniedException(requestContext, token, securityNamespace.Description.ReadPermission, failingPrincipal);
    }

    internal static bool HasReadPermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token)
    {
      securityNamespace = securityNamespace.Unsecured();
      return securityNamespace.NamespaceExtension.HasReadPermission(requestContext, securityNamespace, token);
    }

    public static void CheckWritePermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      bool recurse)
    {
      securityNamespace = securityNamespace.Unsecured();
      EvaluationPrincipal failingPrincipal;
      if (securityNamespace.NamespaceExtension.HasWritePermissionExpect(requestContext, securityNamespace, token, recurse, true, out failingPrincipal))
        return;
      securityNamespace.ThrowAccessDeniedException(requestContext, token, securityNamespace.Description.WritePermission, failingPrincipal);
    }

    public static bool HasWritePermission(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      bool recurse)
    {
      securityNamespace = securityNamespace.Unsecured();
      return securityNamespace.NamespaceExtension.HasWritePermission(requestContext, securityNamespace, token, recurse);
    }

    internal static IQueryableSecurityNamespaceInternal QueryableSecurityNamespaceInternal(
      this IQueryableSecurityNamespace securityNamespace)
    {
      ArgumentUtility.CheckForNull<IQueryableSecurityNamespace>(securityNamespace, nameof (securityNamespace));
      return securityNamespace is IQueryableSecurityNamespaceInternal namespaceInternal ? namespaceInternal : throw new InvalidCastException("Unable to cast " + securityNamespace.GetType().FullName + " to IQueryableNamespaceInternal. IQueryableSecurityNamespace implementations must also implement IQueryableSecurityNamespaceInternal.");
    }

    internal static string CheckAndCanonicalizeToken(
      this IVssSecurityNamespace securityNamespace,
      string token,
      bool allowNull = false)
    {
      SecurityNamespaceExtensions.CheckValidToken(securityNamespace.Description, token, allowNull);
      return token == null ? (string) null : SecurityNamespaceExtensions.CanonicalizeToken(securityNamespace, token);
    }

    private static void CheckValidToken(
      SecurityNamespaceDescription description,
      string token,
      bool allowNull = false)
    {
      if (token == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(nameof (token));
      }
      else if (description.ElementLength != -1 && token.Length > 0 && token.Length % description.ElementLength != 0)
        throw new InvalidSecurityTokenException(FrameworkResources.InvalidSecurityTokenElementLength((object) token, (object) description.ElementLength));
    }

    private static string CanonicalizeToken(IVssSecurityNamespace securityNamespace, string token)
    {
      if (token != null && securityNamespace.IsHierarchical() && securityNamespace.Description.SeparatorValue != char.MinValue)
      {
        while (token.Length > 0 && (int) token[token.Length - 1] == (int) securityNamespace.Description.SeparatorValue)
          token = token.Substring(0, token.Length - 1);
      }
      return token;
    }

    private static string GetMethodName([CallerMemberName] string methodName = "") => methodName;
  }
}
