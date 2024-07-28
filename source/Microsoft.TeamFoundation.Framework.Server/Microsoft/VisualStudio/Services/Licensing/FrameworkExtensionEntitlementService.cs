// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.FrameworkExtensionEntitlementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class FrameworkExtensionEntitlementService : 
    IExtensionEntitlementService,
    IVssFrameworkService
  {
    private readonly CommandPropertiesSetter s_circuitBreakerSettings = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 10));
    internal const string s_assignExtensionToUsersCommandKey = "AssignExtensionToUsers";
    internal const string s_assignExtensionToAllEligibleUsersCommandKey = "AssignExtensionToAllEligibleUsers";
    internal const string s_unassignExtensionFromUsersCommandKey = "UnassignExtensionFromUsers";
    internal const string s_getExtensionsAssignedToUserCommandKey = "GetExtensionsAssignedToUser";
    internal const string s_getExtensionsAssignedToUsersCommandKey = "GetExtensionsAssignedToUsers";
    internal const string s_getExtensionLicenseUsageCommandKey = "GetExtensionLicenseUsage";
    internal const string s_transferExtensionsForIdentitiesCommandKey = "TransferExtensionsForIdentities";
    internal const string s_getEligibleUsersForExtensionCommandKey = "GetEligibleUsersForExtension";
    internal const string s_getExtensionStatusForUsersCommandKey = "GetExtensionStatusForUsers";
    internal const string s_evaluateExtensionAssignmentsOnAccessLevelChangeCommandKey = "EvaluateExtensionAssignmentsOnAccessLevelChange";
    private const string s_area = "VisualStudio.Services.FrameworkAccountEntitlementService";
    private const string s_layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext) => FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ICollection<ExtensionOperationResult> AssignExtensionToUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      bool autoAssignment = false)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130201, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignExtensionToUsers));
      try
      {
        ICollection<ExtensionOperationResult> results = (ICollection<ExtensionOperationResult>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).AssignExtensionToUsersAsync(extensionId, userIds, autoAssignment).SyncResult<ICollection<ExtensionOperationResult>>()), nameof (AssignExtensionToUsers));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130208, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130209, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignExtensionToUsers));
      }
    }

    public ICollection<ExtensionOperationResult> AssignExtensionToAllEligibleUsers(
      IVssRequestContext requestContext,
      string extensionId)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130210, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignExtensionToAllEligibleUsers));
      try
      {
        ICollection<ExtensionOperationResult> results = (ICollection<ExtensionOperationResult>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).AssignExtensionToAllEligibleUsersAsync(extensionId).SyncResult<ICollection<ExtensionOperationResult>>()), nameof (AssignExtensionToAllEligibleUsers));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130218, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130219, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignExtensionToAllEligibleUsers));
      }
    }

    public ICollection<ExtensionOperationResult> UnassignExtensionFromUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130220, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (UnassignExtensionFromUsers));
      try
      {
        ICollection<ExtensionOperationResult> results = (ICollection<ExtensionOperationResult>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).UnassignExtensionFromUsersAsync(extensionId, userIds, source).SyncResult<ICollection<ExtensionOperationResult>>()), nameof (UnassignExtensionFromUsers));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130228, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130229, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (UnassignExtensionFromUsers));
      }
    }

    public IDictionary<string, LicensingSource> GetExtensionsAssignedToUser(
      IVssRequestContext requestContext,
      Guid userId)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130230, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionsAssignedToUser));
      try
      {
        IDictionary<string, LicensingSource> results = (IDictionary<string, LicensingSource>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).GetExtensionsAssignedToUserAsync(userId).SyncResult<IDictionary<string, LicensingSource>>()), nameof (GetExtensionsAssignedToUser));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130238, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130239, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionsAssignedToUser));
      }
    }

    public IDictionary<Guid, IList<ExtensionSource>> GetExtensionsAssignedToUsers(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130240, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionsAssignedToUsers));
      try
      {
        if (userIds == null || userIds.Count < 1)
          return (IDictionary<Guid, IList<ExtensionSource>>) new Dictionary<Guid, IList<ExtensionSource>>();
        IDictionary<Guid, IList<ExtensionSource>> results = (IDictionary<Guid, IList<ExtensionSource>>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).BulkGetExtensionsAssignedToUsersAsync(userIds).SyncResult<IDictionary<Guid, IList<ExtensionSource>>>()), nameof (GetExtensionsAssignedToUsers));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130241, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130242, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionsAssignedToUsers));
      }
    }

    public virtual IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      IVssRequestContext requestContext,
      string extensionId)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130243, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionStatusForUsers));
      try
      {
        if (string.IsNullOrWhiteSpace(extensionId))
          return (IDictionary<Guid, ExtensionAssignmentDetails>) new Dictionary<Guid, ExtensionAssignmentDetails>();
        IDictionary<Guid, ExtensionAssignmentDetails> results = (IDictionary<Guid, ExtensionAssignmentDetails>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).GetExtensionStatusForUsersAsync(extensionId).SyncResult<IDictionary<Guid, ExtensionAssignmentDetails>>()), nameof (GetExtensionStatusForUsers));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130244, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130245, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionStatusForUsers));
      }
    }

    public IEnumerable<AccountLicenseExtensionUsage> GetExtensionLicenseUsage(
      IVssRequestContext requestContext)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130230, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionLicenseUsage));
      try
      {
        IEnumerable<AccountLicenseExtensionUsage> results = (IEnumerable<AccountLicenseExtensionUsage>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).GetExtensionLicenseUsageAsync().SyncResult<IEnumerable<AccountLicenseExtensionUsage>>()), nameof (GetExtensionLicenseUsage));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130238, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130239, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetExtensionLicenseUsage));
      }
    }

    public IList<Guid> GetEligibleUsersForExtension(
      IVssRequestContext requestContext,
      string extensionId,
      ExtensionFilterOptions options)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1130240, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetEligibleUsersForExtension));
      try
      {
        IList<Guid> results = (IList<Guid>) null;
        this.WrapCircuitBreaker(requestContext, (Action) (() => results = this.GetHttpClient(requestContext).GetEligibleUsersForExtensionAsync(extensionId, options).SyncResult<IList<Guid>>()), nameof (GetEligibleUsersForExtension));
        return results;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130248, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130249, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetEligibleUsersForExtension));
      }
    }

    public void TransferExtensionsForIdentities(
      IVssRequestContext requestContext,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMapping)
    {
      FrameworkExtensionEntitlementService.ValidateCollectionRequestContext(requestContext);
      List<IdentityMapping> identityMap = identityMapping.Select<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityMapping>((Func<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityMapping>) (x => new IdentityMapping()
      {
        SourceIdentity = x.Item1,
        TargetIdentity = x.Item2
      })).ToList<IdentityMapping>();
      requestContext.TraceEnter(1130250, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (TransferExtensionsForIdentities));
      try
      {
        this.WrapCircuitBreaker(requestContext, (Action) (() => this.GetHttpClient(requestContext).TransferExtensionsForIdentitiesAsync((IList<IdentityMapping>) identityMap).SyncResult()), nameof (TransferExtensionsForIdentities));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1130258, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1130259, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (TransferExtensionsForIdentities));
      }
    }

    public void EvaluateExtensionAssignmentsOnAccessLevelChange(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License license)
    {
      throw new NotImplementedException();
    }

    private static void ValidateCollectionRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        throw new ArgumentException("Collection level requestContext required.", nameof (requestContext));
    }

    private protected virtual IAccountExtensionLicensingHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetAccountExtensionLicensingHttpClient();
    }

    private void WrapCircuitBreaker(
      IVssRequestContext requestContext,
      Action action,
      string commandKey)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(this.CircuitBreakerSettings);
      CommandService commandService = new CommandService(requestContext, setter, action);
      try
      {
        commandService.Execute();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new LicenseServiceUnavailableException(ex.Message);
      }
    }

    internal virtual CommandPropertiesSetter CircuitBreakerSettings => this.s_circuitBreakerSettings;
  }
}
