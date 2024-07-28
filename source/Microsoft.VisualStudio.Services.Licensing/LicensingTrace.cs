// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingTrace
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal sealed class LicensingTrace
  {
    public static readonly LicensingTrace Log = new LicensingTrace();

    public UnexpectedHostTypeException EntitlementProcessor_WrongHostType(
      IVssRequestContext requestContext,
      UnexpectedHostTypeException exception)
    {
      return this.TraceThrow<UnexpectedHostTypeException>(requestContext, 1033401, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", exception);
    }

    public InvalidOperationException EntitlementProcessor_WrongContext(
      IVssRequestContext requestContext,
      InvalidOperationException exception)
    {
      return this.TraceThrow<InvalidOperationException>(requestContext, 1033402, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", exception);
    }

    public IDisposable EntitlementProcessor_ReconcileEnterLeave(IVssRequestContext requestContext) => this.TraceMember(requestContext, 1033420, 1033429, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Reconcile");

    public IDisposable EntitlementProcessor_AssignEnterLeave(IVssRequestContext requestContext) => this.TraceMember(requestContext, 1033430, 1033439, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Assign");

    public void EntitlementProcessor_AttemptAssignLicenseToUser(
      IVssRequestContext requestContext,
      License license,
      Guid userId)
    {
      this.Trace(requestContext, 1033431, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Assigning license '{0}' to user '{1}'.", (object) license, (object) userId);
    }

    public void EntitlementProcessor_ReachedAssignmentDecision(
      IVssRequestContext requestContext,
      object decision)
    {
      this.Trace(requestContext, 1033432, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Assignment decision: {0}", decision);
    }

    public IDisposable EntitlementProcessor_AssignAvailableEnterLeave(
      IVssRequestContext requestContext)
    {
      return this.TraceMember(requestContext, 1033440, 1033449, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "AssignAvailable");
    }

    public IDisposable EntitlementProcessor_RevokeEnterLeave(IVssRequestContext requestContext) => this.TraceMember(requestContext, 1033450, 1033459, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Revoke");

    public IDisposable EntitlementProcessor_ProcessEnterLeave(IVssRequestContext requestContext) => this.TraceMember(requestContext, 1033460, 1033469, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Process");

    public void EntitlementProcessor_ProcessNoLicenseForCollectionOwner(
      IVssRequestContext requestContext)
    {
      this.Trace(requestContext, 1033461, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Assigned license is unavailable, but user is collection owner. Reassigning available license to owner.");
    }

    public void EntitlementProcessor_EnablingUser(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus previousUserStatus,
      AccountUserStatus newUserStatus)
    {
      this.Trace(requestContext, 1033481, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "User {0} was {1} and can now be assigned a license. Setting status to {2}.", (object) userId, (object) previousUserStatus, (object) newUserStatus);
    }

    public void EntitlementProcessor_DisablingUser(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus previousUserStatus,
      AccountUserStatus newUserStatus)
    {
      this.Trace(requestContext, 1033491, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "User {0} was {1} but can no longer be assigned a license. Setting status to {2}.", (object) userId, (object) previousUserStatus, (object) newUserStatus);
    }

    public void EntitlementProcessor_AddingUser(IVssRequestContext requestContext, Guid userId) => this.Trace(requestContext, 1033531, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "User {0} is being added to collection.", (object) userId);

    public void EntitlementProcessor_AssignLicense(
      IVssRequestContext requestContext,
      Guid userId,
      License license)
    {
      this.Trace(requestContext, 1033501, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Assigning user '{0}' to license '{1}'.", (object) userId, (object) license);
    }

    public void EntitlementProcessor_RevokeLicense(IVssRequestContext requestContext, Guid userId) => this.Trace(requestContext, 1033511, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Revoking license for user '{0}'.", (object) userId);

    public void EntitlementProcessor_NoLicensesAvailable(
      IVssRequestContext requestContext,
      Guid userId)
    {
      this.Trace(requestContext, 1033521, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "There are no available licenses to assign to user '{0}'.", (object) userId);
    }

    public void EntitlementProcessor_ProcessCollectionInvalid(IVssRequestContext requestContext) => this.Trace(requestContext, 1033462, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Collection is not enabled and is invalid for processing.");

    public void EntitlementProcessor_AssignCollectionInvalid(IVssRequestContext requestContext) => this.Trace(requestContext, 1033433, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Collection is not enabled and is invalid for license assignments.");

    public void EntitlementProcessor_AssignAvailableCollectionInvalid(
      IVssRequestContext requestContext)
    {
      this.Trace(requestContext, 1033441, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Collection is not enabled and is invalid for license assignments.");
    }

    public void EntitlementProcessor_RevokeCollectionInvalid(IVssRequestContext requestContext) => this.Trace(requestContext, 1033451, TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", "Collection is not enabled and is invalid for license revocation.");

    public InvalidLicensingOperation EntitlementProcessor_GetValidCollectionUser_InvalidUserId(
      IVssRequestContext requestContext,
      InvalidLicensingOperation exception)
    {
      return this.TraceThrow<InvalidLicensingOperation>(requestContext, 1033471, "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor", "BusinessLogic", exception);
    }

    public IDisposable EnsureLicenseCollectionEventSubscriber_ProcessEvent_EnterLeave(
      IVssRequestContext requestContext)
    {
      return this.TraceMember(requestContext, 1033600, 1033609, "Microsoft.VisualStudio.Services.Licensing.EnsureLicenseAccountEventSubscriber", "BusinessLogic", "ProcessEvent");
    }

    public IDisposable CollectionUserRemovedEventSubscriber_ProcessEvent_EnterLeave(
      IVssRequestContext requestContext)
    {
      return this.TraceMember(requestContext, 1033610, 1033619, "Microsoft.VisualStudio.Services.Licensing.AccountUserRemovedEventSubscriber", "BusinessLogic", "ProcessEvent");
    }

    public void PlatformLicensingService_GetMsdnLicenseForUser_Catch(
      IVssRequestContext requestContext,
      Exception exception)
    {
      this.TraceCatch(requestContext, 1030178, "VisualStudio.Services.LicensingService", "BusinessLogic", exception);
    }

    public void EnsureLicenseCollectionEventSubscriber_ProcessEvent_Catch(
      IVssRequestContext requestContext,
      Exception exception)
    {
      this.TraceCatch(requestContext, 1030179, "Microsoft.VisualStudio.Services.Licensing.EnsureLicenseAccountEventSubscriber", "LicensingService", exception);
    }

    public void CollectionUserRemovedEventSubscriber_ProcessEvent_Catch(
      IVssRequestContext requestContext,
      Exception exception)
    {
      this.TraceCatch(requestContext, 1030180, "Microsoft.VisualStudio.Services.Licensing.AccountUserRemovedEventSubscriber", "LicensingService", exception);
    }

    private void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      if (requestContext == null)
        return;
      VssRequestContextExtensions.Trace(requestContext, tracepoint, level, area, layer, format, args);
    }

    private void TraceCatch(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Exception exception)
    {
      if (requestContext == null)
        return;
      requestContext.TraceCatch(tracepoint, area, layer, exception);
    }

    private TException TraceThrow<TException>(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      TException exception)
      where TException : Exception
    {
      if (requestContext != null)
        requestContext.TraceThrow<TException>(tracepoint, area, layer, exception);
      return exception;
    }

    private IDisposable TraceMember(
      IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      string area,
      string layer,
      string memberName)
    {
      return requestContext != null ? requestContext.TraceMember(enterTracepoint, leaveTracepoint, area, layer, memberName) : (IDisposable) null;
    }

    public static class Areas
    {
      public const string EntitlementProcessor = "Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor";
      public const string EnsureLicenseCollectionEventSubscriber = "Microsoft.VisualStudio.Services.Licensing.EnsureLicenseAccountEventSubscriber";
      public const string CollectionUserRemovedEventSubscriber = "Microsoft.VisualStudio.Services.Licensing.AccountUserRemovedEventSubscriber";
      public const string PlatformLicensingService = "VisualStudio.Services.LicensingService";
      public const string AssignLicensesJob = "VisualStudio.Services.Licensing.Plugins.AssignLicensesJob";
      public const string ReportLicenseUsageToCommerceJob = "VisualStudio.Services.Licensing.Plugins.ReportLicenseUsageToCommerceJob";
    }

    public static class Layers
    {
      public const string BusinessLogic = "BusinessLogic";
      public const string LicensingService = "LicensingService";
      public const string AssignLicensesJob = "AssignLicensesJob";
      public const string ReportLicenseUsageToCommerceJob = "ReportLicenseUsageToCommerceJob";
    }
  }
}
