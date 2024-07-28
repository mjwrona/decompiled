// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypesRulesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "rules", ResourceVersion = 1)]
  public class ProcessWorkItemTypesRulesController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (FieldRuleModel), null, null)]
    [ClientLocationId("76FE3432-D825-479D-A5F6-983BBB78B4F3")]
    [ValidateModel]
    [ClientExample("POST__wit_rule.json", "Add the rule definition", null, null)]
    public HttpResponseMessage AddWorkItemTypeRule(
      Guid processId,
      string witRefName,
      [FromBody] FieldRuleModel fieldRule)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (fieldRule == null)
        throw new VssPropertyValidationException(nameof (fieldRule), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      fieldRule.IsSystem = false;
      fieldRule.Id = new Guid?(Guid.NewGuid());
      this.FixAndValidateFieldModel(processId, witRefName, fieldRule);
      TelemetryHelper.PublishRuleChangeCIEvent(this.TfsRequestContext, processId, witRefName, false, fieldRule);
      IDictionary<string, WorkItemRule> itemRuleDictionary = fieldRule.CreateFieldToWorkItemRuleDictionary(this.TfsRequestContext, processId);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateWorkItemTypeRules(this.TfsRequestContext, processId, witRefName, (IEnumerable<string>) itemRuleDictionary.Keys, itemRuleDictionary);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Rule", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemReferenceName", (object) witRefName);
      data.Add("RuleName", (object) fieldRule.FriendlyName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<FieldRuleModel>(HttpStatusCode.Created, fieldRule);
    }

    [HttpPut]
    [ClientResponseType(typeof (FieldRuleModel), null, null)]
    [ClientLocationId("76FE3432-D825-479D-A5F6-983BBB78B4F3")]
    [ValidateModel]
    [ClientExample("PUT__wit_rule.json", "Update the rule definition", null, null)]
    public HttpResponseMessage UpdateWorkItemTypeRule(
      Guid processId,
      string witRefName,
      Guid ruleId,
      [FromBody] FieldRuleModel fieldRule)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (ruleId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (ruleId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (ruleId)));
      Guid? nullable = fieldRule != null ? fieldRule.Id : throw new VssPropertyValidationException(nameof (fieldRule), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      if (nullable.HasValue)
      {
        nullable = fieldRule.Id;
        if (nullable.Value != Guid.Empty)
        {
          nullable = fieldRule.Id;
          if (nullable.Value != ruleId)
          {
            // ISSUE: variable of a boxed type
            __Boxed<Guid> local1 = (ValueType) ruleId;
            nullable = fieldRule.Id;
            // ISSUE: variable of a boxed type
            __Boxed<Guid> local2 = (ValueType) nullable.Value;
            throw new VssServiceException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleIdMismatchInRequestBody((object) local1, (object) local2));
          }
          goto label_13;
        }
      }
      fieldRule.Id = new Guid?(ruleId);
label_13:
      this.FixAndValidateFieldModel(processId, witRefName, fieldRule);
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      FieldRuleModel fieldRuleModel;
      if (!FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).TryGetValue(ruleId, out fieldRuleModel))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      if (fieldRuleModel.IsSystem)
        throw new VssServiceException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.CannotUpdateSystemRule());
      if (fieldRuleModel.StrictEquals(fieldRule))
        return this.Request.CreateResponse(HttpStatusCode.NotModified);
      TelemetryHelper.PublishRuleChangeCIEvent(this.TfsRequestContext, processId, witRefName, true, fieldRule);
      IDictionary<string, WorkItemRule> itemRuleDictionary = fieldRule.CreateFieldToWorkItemRuleDictionary(this.TfsRequestContext, processId);
      IEnumerable<string> fieldReferenceNames = fieldRule.Actions.Select<RuleActionModel, string>((Func<RuleActionModel, string>) (a => a.TargetField)).Distinct<string>();
      service.UpdateWorkItemTypeRules(this.TfsRequestContext, processId, witRefName, fieldReferenceNames, itemRuleDictionary, (ISet<Guid>) new HashSet<Guid>()
      {
        ruleId
      });
      fieldRule.IsSystem = false;
      this.ConvertFieldIdsToRefNames(fieldRule);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Rule", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("RuleName", (object) fieldRule.FriendlyName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<FieldRuleModel>(HttpStatusCode.OK, fieldRule);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("76FE3432-D825-479D-A5F6-983BBB78B4F3")]
    [ValidateModel]
    [ClientExample("DELETE__wit_rule.json", "Delete the rule", null, null)]
    public HttpResponseMessage DeleteWorkItemTypeRule(
      Guid processId,
      string witRefName,
      Guid ruleId)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (ruleId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (ruleId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (ruleId)));
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      FieldRuleModel fieldRuleModel;
      if (!FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).TryGetValue(ruleId, out fieldRuleModel))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      if (fieldRuleModel.IsSystem)
        throw new VssServiceException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.CannotDeleteSystemRule());
      service.RemoveWorkItemTypeRule(this.TfsRequestContext, processId, witRefName, ruleId);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Rule", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("RuleName", (object) fieldRuleModel.FriendlyName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<FieldRuleModel>), null, null)]
    [ClientLocationId("76FE3432-D825-479D-A5F6-983BBB78B4F3")]
    [ValidateModel]
    [ClientExample("GET__wit_rules.json", "Get the list of rules for the work item type", null, null)]
    public HttpResponseMessage GetWorkItemTypeRules(Guid processId, string witRefName)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      IEnumerable<FieldRuleModel> workItemTypeRules = this.GetAllWorkItemTypeRules(this.TfsRequestContext.GetService<IProcessWorkItemTypeService>(), processId, witRefName);
      foreach (FieldRuleModel fieldRuleModel in workItemTypeRules)
        this.ConvertFieldIdsToRefNames(fieldRuleModel);
      return this.Request.CreateResponse<IEnumerable<FieldRuleModel>>(HttpStatusCode.OK, workItemTypeRules);
    }

    [HttpGet]
    [ClientResponseType(typeof (FieldRuleModel), null, null)]
    [ClientLocationId("76FE3432-D825-479D-A5F6-983BBB78B4F3")]
    [ClientExample("GET__wit_rule.json", "Get the specified rule definition", null, null)]
    [ValidateModel]
    public HttpResponseMessage GetWorkItemTypeRule(Guid processId, string witRefName, Guid ruleId)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (ruleId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (ruleId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (ruleId)));
      FieldRuleModel fieldRuleModel;
      if (!FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).TryGetValue(ruleId, out fieldRuleModel))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      this.ConvertFieldIdsToRefNames(fieldRuleModel);
      return this.Request.CreateResponse<FieldRuleModel>(HttpStatusCode.OK, fieldRuleModel);
    }

    private void FixAndValidateFieldModel(
      Guid processId,
      string witReferenceName,
      FieldRuleModel fieldRule)
    {
      this.TfsRequestContext.GetService<IFieldRuleModelValidatorService>().FixAndValidate(this.TfsRequestContext, processId, witReferenceName, fieldRule);
    }

    private IEnumerable<FieldRuleModel> GetAllWorkItemTypeRules(
      IProcessWorkItemTypeService witService,
      Guid processId,
      string witRefName)
    {
      return (IEnumerable<FieldRuleModel>) FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(witService.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).Values;
    }

    private void ConvertFieldIdsToRefNames(FieldRuleModel fieldRuleModel)
    {
      if (fieldRuleModel.Actions == null)
        return;
      WorkItemTrackingFieldService trackingFieldService = (WorkItemTrackingFieldService) null;
      foreach (RuleActionModel action in fieldRuleModel.Actions)
      {
        int result;
        if ((action.ActionType == "$copyFromField" || action.ActionType == "$setDefaultFromField") && int.TryParse(action.Value, out result))
        {
          if (trackingFieldService == null)
            trackingFieldService = this.TfsRequestContext.GetService<WorkItemTrackingFieldService>();
          FieldEntry fieldById = trackingFieldService.GetFieldById(this.TfsRequestContext, result, new bool?(false));
          if (fieldById != null)
            action.Value = fieldById.ReferenceName;
        }
      }
    }
  }
}
