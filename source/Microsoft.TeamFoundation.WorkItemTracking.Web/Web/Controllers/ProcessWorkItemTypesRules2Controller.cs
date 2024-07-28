// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypesRules2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
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
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "rules", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  public class ProcessWorkItemTypesRules2Controller : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (ProcessRule), null, null)]
    [ValidateModel]
    [ClientExample("POST__wit_rule.json", "Add the rule definition", null, null)]
    public HttpResponseMessage AddProcessWorkItemTypeRule(
      Guid processId,
      string witRefName,
      [FromBody] CreateProcessRuleRequest processRuleCreate)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (processRuleCreate == null)
        throw new VssPropertyValidationException("CreateProcessRuleRequest", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      if (processRuleCreate.Actions != null && processRuleCreate.Actions.Any<RuleAction>((Func<RuleAction, bool>) (action => action == null)))
        throw new VssPropertyValidationException("Actions", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullRuleActionListItem());
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel = processRuleCreate.ToFieldRuleModel();
      fieldRuleModel.Id = new Guid?(Guid.NewGuid());
      fieldRuleModel.IsSystem = false;
      this.FixAndValidateFieldModel(processId, witRefName, fieldRuleModel);
      TelemetryHelper.PublishRuleChangeCIEvent(this.TfsRequestContext, processId, witRefName, false, fieldRuleModel);
      IDictionary<string, WorkItemRule> itemRuleDictionary = fieldRuleModel.CreateFieldToWorkItemRuleDictionary(this.TfsRequestContext, processId);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateWorkItemTypeRules(this.TfsRequestContext, processId, witRefName, (IEnumerable<string>) itemRuleDictionary.Keys, itemRuleDictionary);
      ProcessRule processRule = fieldRuleModel.ToProcessRule(this.TfsRequestContext, processId, witRefName);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Rule", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemReferenceName", (object) witRefName);
      data.Add("RuleName", (object) processRuleCreate.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<ProcessRule>(HttpStatusCode.Created, processRule);
    }

    [HttpPut]
    [ClientResponseType(typeof (ProcessRule), null, null)]
    [ValidateModel]
    [ClientExample("PUT__wit_rule.json", "Update the rule definition", null, null)]
    public HttpResponseMessage UpdateProcessWorkItemTypeRule(
      Guid processId,
      string witRefName,
      Guid ruleId,
      [FromBody] UpdateProcessRuleRequest processRule)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (ruleId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (ruleId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (ruleId)));
      if (processRule == null)
        throw new VssPropertyValidationException(nameof (processRule), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      if (processRule.Actions != null && processRule.Actions.Any<RuleAction>((Func<RuleAction, bool>) (action => action == null)))
        throw new VssPropertyValidationException("Actions", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullRuleActionListItem());
      if (processRule.Id != Guid.Empty)
      {
        if (processRule.Id != ruleId)
          throw new VssServiceException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RuleIdMismatchInRequestBody((object) ruleId, (object) processRule.Id));
      }
      else
        processRule.Id = ruleId;
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel1 = ProcessRuleConverter.ToFieldRuleModel(processRule);
      this.FixAndValidateFieldModel(processId, witRefName, fieldRuleModel1);
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel2;
      if (!FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).TryGetValue(ruleId, out fieldRuleModel2))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      if (fieldRuleModel2.IsSystem)
        throw new VssServiceException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.CannotUpdateSystemRule());
      if (fieldRuleModel2.StrictEquals(fieldRuleModel1))
        return this.Request.CreateResponse(HttpStatusCode.NotModified);
      fieldRuleModel1.IsSystem = false;
      TelemetryHelper.PublishRuleChangeCIEvent(this.TfsRequestContext, processId, witRefName, true, fieldRuleModel1);
      IDictionary<string, WorkItemRule> itemRuleDictionary = fieldRuleModel1.CreateFieldToWorkItemRuleDictionary(this.TfsRequestContext, processId);
      IEnumerable<string> fieldReferenceNames = fieldRuleModel1.Actions.Select<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel, string>) (a => a.TargetField)).Distinct<string>();
      service.UpdateWorkItemTypeRules(this.TfsRequestContext, processId, witRefName, fieldReferenceNames, itemRuleDictionary, (ISet<Guid>) new HashSet<Guid>()
      {
        ruleId
      });
      ProcessRule processRule1 = fieldRuleModel1.ToProcessRule(this.TfsRequestContext, processId, witRefName);
      this.ConvertFieldIdsToRefNames(processRule1);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Rule", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("RuleName", (object) fieldRuleModel1.FriendlyName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<ProcessRule>(HttpStatusCode.OK, processRule1);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    [ClientExample("DELETE__wit_rule.json", "Delete the rule", null, null)]
    public HttpResponseMessage DeleteProcessWorkItemTypeRule(
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
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel;
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
    [ClientResponseType(typeof (IEnumerable<ProcessRule>), null, null)]
    [ValidateModel]
    [ClientExample("GET__wit_rules.json", "Get the list of rules for the work item type", null, null)]
    public HttpResponseMessage GetProcessWorkItemTypeRules(Guid processId, string witRefName)
    {
      WorkItemTrackingFeatureFlags.CheckProcessCustomizationEnabled(this.TfsRequestContext);
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      IEnumerable<WorkItemFieldRule> rulesForWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true);
      return this.Request.CreateResponse<List<ProcessRule>>(HttpStatusCode.OK, ProcessRulesModelFactory.CreateIdToProcessRuleDictionary(this.TfsRequestContext, processId, witRefName, rulesForWorkItemType).Values.ToList<ProcessRule>());
    }

    [HttpGet]
    [ClientResponseType(typeof (ProcessRule), null, null)]
    [ClientExample("GET__wit_rule.json", "Get the specified rule definition", null, null)]
    [ValidateModel]
    public HttpResponseMessage GetProcessWorkItemTypeRule(
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
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRule;
      if (!FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).TryGetValue(ruleId, out fieldRule))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      ProcessRule processRule = fieldRule.ToProcessRule(this.TfsRequestContext, processId, witRefName);
      this.ConvertFieldIdsToRefNames(processRule);
      return this.Request.CreateResponse<ProcessRule>(HttpStatusCode.OK, processRule);
    }

    private void FixAndValidateFieldModel(
      Guid processId,
      string witReferenceName,
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRule)
    {
      this.TfsRequestContext.GetService<IFieldRuleModelValidatorService>().FixAndValidate(this.TfsRequestContext, processId, witReferenceName, fieldRule);
    }

    private IEnumerable<ProcessRule> GetAllWorkItemTypeRules(
      IProcessWorkItemTypeService witService,
      Guid processId,
      string witRefName)
    {
      return FieldRuleModelFactory.CreateIdToFieldRuleModelDictionary(witService.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName, true)).Values.Select<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel, ProcessRule>((Func<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel, ProcessRule>) (rule => rule.ToProcessRule(this.TfsRequestContext, processId, witRefName)));
    }

    private void ConvertFieldIdsToRefNames(ProcessRule processRule)
    {
      if (processRule == null || processRule.Actions == null)
        return;
      WorkItemTrackingFieldService trackingFieldService = (WorkItemTrackingFieldService) null;
      foreach (RuleAction action in processRule.Actions)
      {
        int result;
        if ((action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromField || action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromField) && int.TryParse(action.Value, out result))
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
