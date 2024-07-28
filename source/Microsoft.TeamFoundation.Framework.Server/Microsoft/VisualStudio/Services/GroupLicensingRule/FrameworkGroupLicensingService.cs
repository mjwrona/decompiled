// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.FrameworkGroupLicensingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkGroupLicensingService : IGroupLicensingService, IVssFrameworkService
  {
    private readonly CommandPropertiesSetter cbWith2SecondTimeout = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 2));
    private readonly CommandPropertiesSetter cbWith7SecondTimeout = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 7));
    private const string s_area = "VisualStudio.Services.FrameworkGroupLicensingService";
    private const string s_layer = "BusinessLogic";
    internal const string s_getGroupLicensingRulesCommandKey = "GroupLicensingRules";
    internal const string s_getGroupLicensingRulesBatchCommandKey = "GetGroupLicensingRulesBatch";
    internal const string s_getGroupLicensingRuleCommandKey = "GetGroupLicensingRule";
    internal const string s_addGroupLicensingRuleCommandKey = "AddGroupLicensingRule";
    internal const string s_updateGroupLicensingRuleCommandKey = "UpdateGroupLicensingRule";
    internal const string s_deleteGroupLicenseRuleCommandKey = "DeleteGroupLicensingRule";
    internal const string s_applyGroupLicensingRulesToUserCommandKey = "ApplyGroupLicensingRulesToUser";
    internal const string s_applyGroupLicensingRulesToAllUsersCommandKey = "ApplyGroupLicensingRulesToAllUsers";
    internal const string s_RemoveDirectAssignmentsCommandKey = "RemoveDirectAssignments";
    internal const string s_getApplicationStatusCommandKey = "GetApplicationStatus";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckHostedDeployment();
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule> GetGroupLicensingRules(
      IVssRequestContext requestContext,
      int top,
      int skip = 0)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102001, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRules));
      try
      {
        return (IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>) this.WrapCircuitBreaker<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>(requestContext, (Func<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>) (() => this.GetHttpClient(requestContext).GetGroupLicensingRulesAsync(top, new int?(skip)).SyncResult<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>()), (Func<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>) null, "GroupLicensingRules", this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102008, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102009, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRules));
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule> GetGroupLicensingRules(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> subjectDescriptors)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102011, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRules));
      try
      {
        return (IEnumerable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>) this.WrapCircuitBreaker<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>(requestContext, (Func<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>) (() => this.GetHttpClient(requestContext).LookupGroupLicensingRulesAsync(new GraphSubjectLookup(subjectDescriptors.Select<SubjectDescriptor, GraphSubjectLookupKey>((Func<SubjectDescriptor, GraphSubjectLookupKey>) (x => new GraphSubjectLookupKey(x))))).SyncResult<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>()), (Func<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>) null, "GetGroupLicensingRulesBatch", this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102018, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102019, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRules));
      }
    }

    public Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule GetGroupLicensingRule(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102021, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRule));
      try
      {
        return this.WrapCircuitBreaker<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>(requestContext, (Func<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>) (() => this.GetHttpClient(requestContext).GetGroupLicensingRuleAsync((string) subjectDescriptor).SyncResult<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>()), (Func<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>) null, nameof (GetGroupLicensingRule), this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102028, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102029, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetGroupLicensingRule));
      }
    }

    public OperationReference AddGroupLicensingRule(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule licensingRule,
      RuleOption? ruleOption = null)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102031, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (AddGroupLicensingRule));
      try
      {
        return this.WrapCircuitBreaker<OperationReference>(requestContext, (Func<OperationReference>) (() => this.GetHttpClient(requestContext).AddGroupLicensingRuleAsync(licensingRule, ruleOption).SyncResult<OperationReference>()), (Func<OperationReference>) null, nameof (AddGroupLicensingRule), this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102038, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102039, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (AddGroupLicensingRule));
      }
    }

    public OperationReference UpdateGroupLicensingRule(
      IVssRequestContext requestContext,
      GroupLicensingRuleUpdate licensingRuleUpdate,
      RuleOption? ruleOption = null)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102041, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (UpdateGroupLicensingRule));
      try
      {
        return this.WrapCircuitBreaker<OperationReference>(requestContext, (Func<OperationReference>) (() => this.GetHttpClient(requestContext).UpdateGroupLicensingRuleAsync(licensingRuleUpdate, ruleOption).SyncResult<OperationReference>()), (Func<OperationReference>) null, nameof (UpdateGroupLicensingRule), this.cbWith7SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102048, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102049, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (UpdateGroupLicensingRule));
      }
    }

    public OperationReference DeleteGroupLicensingRule(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      RuleOption? ruleOption = null)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102051, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (DeleteGroupLicensingRule));
      try
      {
        return this.WrapCircuitBreaker<OperationReference>(requestContext, (Func<OperationReference>) (() => this.GetHttpClient(requestContext).DeleteGroupLicenseRuleAsync((string) subjectDescriptor, ruleOption).SyncResult<OperationReference>()), (Func<OperationReference>) null, nameof (DeleteGroupLicensingRule), this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102058, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102059, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (DeleteGroupLicensingRule));
      }
    }

    public AccountEntitlement ApplyGroupLicensingRulesToUser(IVssRequestContext requestContext)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102061, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToUser));
      try
      {
        return this.ApplyGroupLicensingRulesToUserWithCB(requestContext, requestContext.GetUserId());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102068, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102069, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToUser));
      }
    }

    public AccountEntitlement ApplyGroupLicensingRulesToUser(
      IVssRequestContext requestContext,
      SubjectDescriptor identityDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return this.ApplyGroupLicensingRulesToUser(requestContext, identity.Id);
    }

    public AccountEntitlement ApplyGroupLicensingRulesToUser(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102071, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToUser));
      try
      {
        return this.ApplyGroupLicensingRulesToUserWithCB(requestContext, identityId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102078, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102079, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToUser));
      }
    }

    private AccountEntitlement ApplyGroupLicensingRulesToUserWithCB(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      return this.WrapCircuitBreaker<AccountEntitlement>(requestContext, (Func<AccountEntitlement>) (() =>
      {
        this.GetHttpClient(requestContext).ApplyGroupLicensingRulesToUserAsync(identityId).SyncResult();
        return requestContext.GetAccountLicensingHttpClient().GetAccountEntitlementAsync(identityId, false, false).SyncResult<AccountEntitlement>();
      }), (Func<AccountEntitlement>) (() => requestContext.GetAccountLicensingHttpClient().AssignAvailableEntitlementAsync(identityId).SyncResult<AccountEntitlement>()), "ApplyGroupLicensingRulesToUser", this.cbWith2SecondTimeout);
    }

    public OperationReference ApplyGroupLicensingRulesToAllUsers(
      IVssRequestContext requestContext,
      RuleOption? ruleOption = null)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102081, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToAllUsers));
      try
      {
        return this.WrapCircuitBreaker<OperationReference>(requestContext, (Func<OperationReference>) (() => this.GetHttpClient(requestContext).ApplyGroupLicensingRulesToAllUsersAsync(ruleOption).SyncResult<OperationReference>()), (Func<OperationReference>) null, nameof (ApplyGroupLicensingRulesToAllUsers), this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102088, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102089, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (ApplyGroupLicensingRulesToAllUsers));
      }
    }

    public ApplicationStatus GetApplicationStatus(
      IVssRequestContext requestContext,
      Guid? operationId = null)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102091, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetApplicationStatus));
      try
      {
        return this.WrapCircuitBreaker<ApplicationStatus>(requestContext, (Func<ApplicationStatus>) (() => this.GetHttpClient(requestContext).GetApplicationStatusAsync(operationId).SyncResult<ApplicationStatus>()), (Func<ApplicationStatus>) null, nameof (GetApplicationStatus), this.cbWith2SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102098, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102099, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (GetApplicationStatus));
      }
    }

    public void RemoveDirectAssignment(
      IVssRequestContext requestContext,
      SubjectDescriptor identityDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.RemoveDirectAssignment(requestContext, identity.Id);
    }

    public void RemoveDirectAssignment(IVssRequestContext requestContext, Guid identityId)
    {
      FrameworkGroupLicensingService.ValidateCollectionRequestContext(requestContext);
      requestContext.TraceEnter(1102101, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (RemoveDirectAssignment));
      try
      {
        this.WrapCircuitBreaker(requestContext, (Action) (() => this.GetHttpClient(requestContext).RemoveDirectAssignmentAsync(identityId).SyncResult()), (Action) null, "RemoveDirectAssignments", this.cbWith7SecondTimeout);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1102108, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1102109, "VisualStudio.Services.FrameworkGroupLicensingService", "BusinessLogic", nameof (RemoveDirectAssignment));
      }
    }

    private static void ValidateCollectionRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        throw new ArgumentException("Collection level requestContext required.", nameof (requestContext));
    }

    protected virtual LicensingRuleHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetLicensingRuleHttpClient();

    private void WrapCircuitBreaker(
      IVssRequestContext requestContext,
      Action action,
      Action fallback,
      string commandKey,
      CommandPropertiesSetter settings)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(settings);
      CommandService commandService = new CommandService(requestContext, setter, action, fallback);
      try
      {
        commandService.Execute();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new LicenseServiceUnavailableException(ex.Message);
      }
    }

    private T WrapCircuitBreaker<T>(
      IVssRequestContext requestContext,
      Func<T> action,
      Func<T> fallback,
      string commandKey,
      CommandPropertiesSetter settings)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(settings);
      T result = default (T);
      CommandService commandService = new CommandService(requestContext, setter, (Action) (() => result = action()), (Action) (() => result = fallback()));
      try
      {
        commandService.Execute();
        return result;
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new LicenseServiceUnavailableException(ex.Message);
      }
    }
  }
}
