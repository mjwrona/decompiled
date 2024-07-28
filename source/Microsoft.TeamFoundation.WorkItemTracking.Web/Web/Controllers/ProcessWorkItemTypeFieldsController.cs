// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypeFieldsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process;
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
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "fields", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  public class ProcessWorkItemTypeFieldsController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientExample("POST_field_to_work_item_type.json", "Adds a field to a work item type", null, null)]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    [ClientResponseType(typeof (ProcessWorkItemTypeField), null, null)]
    public HttpResponseMessage AddFieldToWorkItemType(
      Guid processId,
      string witRefName,
      [FromBody] AddProcessWorkItemTypeFieldRequest field)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName);
      if (field == null)
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      AddProcessWorkItemTypeFieldRequest request = field;
      WorkItemTypeletFieldRuleProperties serverProperties = request != null ? request.ToServerProperties(this.TfsRequestContext) : (WorkItemTypeletFieldRuleProperties) null;
      IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = service.GetAllWorkItemTypes(this.TfsRequestContext, processId, true);
      if (allWorkItemTypes.FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => t.ReferenceName == witRefName)) == null)
      {
        int lastDotPosition = witRefName.LastIndexOf('.') + 1;
        ComposedWorkItemType composedWorkItemType = allWorkItemTypes.FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => t.Name == witRefName.Substring(lastDotPosition)));
        witRefName = composedWorkItemType != null ? composedWorkItemType.ReferenceName : throw new ProcessWorkItemTypeDoesNotExistException(witRefName, processId.ToString());
        if (composedWorkItemType.ParentTypeRefName == null)
          witRefName = service.CreateDerivedWorkItemType(this.TfsRequestContext, processId, composedWorkItemType.ReferenceName, composedWorkItemType.Description, composedWorkItemType.Color, composedWorkItemType.Icon, new bool?(composedWorkItemType.IsDisabled)).ReferenceName;
      }
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType processWorkItemType = service.AddOrUpdateWorkItemTypeField(this.TfsRequestContext, processId, witRefName, field.ReferenceName, serverProperties);
      FieldEntry witField = processWorkItemType.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ReferenceName.Equals(field.ReferenceName, StringComparison.OrdinalIgnoreCase)))?.Field;
      if (witField == null)
        throw new ProcessWorkItemTypeFieldDoesNotExistException(field.ReferenceName, witRefName);
      ProcessWorkItemTypeFieldFactory typeFieldFactory = new ProcessWorkItemTypeFieldFactory(this.TfsRequestContext);
      IEnumerable<WorkItemFieldRule> rulesForWorkItemType = service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName);
      IEnumerable<WorkItemRule> rules = rulesForWorkItemType != null ? rulesForWorkItemType.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals(witField.ReferenceName, fr.Field))).SelectMany<WorkItemFieldRule, WorkItemRule>((Func<WorkItemFieldRule, IEnumerable<WorkItemRule>>) (fr => (IEnumerable<WorkItemRule>) fr.SubRules)) : (IEnumerable<WorkItemRule>) null;
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("FieldReferenceName", (object) field.ReferenceName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      ProcessWorkItemTypeFieldsExpandLevel expand = field == null || field.AllowedValues == null ? ProcessWorkItemTypeFieldsExpandLevel.None : ProcessWorkItemTypeFieldsExpandLevel.AllowedValues;
      return this.Request.CreateResponse<ProcessWorkItemTypeField>(HttpStatusCode.OK, typeFieldFactory.Create(processId, witRefName, field.ReferenceName, witField, rules, processWorkItemType.IsCustomType ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Custom : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Inherited, expand));
    }

    [HttpGet]
    [ClientExample("GET_work_item_type_field.json", "Returns a field in a work item type", null, null)]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    public ProcessWorkItemTypeField GetWorkItemTypeField(
      Guid processId,
      string witRefName,
      string fieldRefName,
      [FromUri(Name = "$expand")] ProcessWorkItemTypeFieldsExpandLevel expand = ProcessWorkItemTypeFieldsExpandLevel.None)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName, fieldRefName);
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      HashSet<string> source = (HashSet<string>) null;
      IReadOnlyCollection<ProcessFieldResult> legacyFields = (service.GetAllWorkItemTypes(this.TfsRequestContext, processId, true).FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => t.ReferenceName == witRefName)) ?? throw new ProcessWorkItemTypeDoesNotExistException(witRefName, processId.ToString())).GetLegacyFields(this.TfsRequestContext);
      try
      {
        ProcessTypelet typelet = service.GetTypelet<ProcessTypelet>(this.TfsRequestContext, processId, witRefName, true);
        HashSet<string> stringSet;
        if (typelet == null)
        {
          stringSet = (HashSet<string>) null;
        }
        else
        {
          IEnumerable<WorkItemTypeExtensionFieldEntry> fields = typelet.Fields;
          stringSet = fields != null ? fields.ToHashSet<WorkItemTypeExtensionFieldEntry, string>((Func<WorkItemTypeExtensionFieldEntry, string>) (f => f.Field.ReferenceName)) : (HashSet<string>) null;
        }
        source = stringSet;
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
      }
      Dictionary<string, IEnumerable<WorkItemRule>> fieldRuleMap = ProcessWorkItemTypeFieldsController.CreateFieldRuleMap(service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName));
      ProcessWorkItemTypeFieldFactory typeFieldFactory = new ProcessWorkItemTypeFieldFactory(this.TfsRequestContext);
      ProcessFieldResult processFieldResult = legacyFields.Where<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => f.ReferenceName.Equals(fieldRefName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ProcessFieldResult>();
      if (processFieldResult == null)
        throw new ProcessWorkItemTypeFieldDoesNotExistException(fieldRefName, witRefName);
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string fieldRefName1 = fieldRefName;
      ProcessFieldResult field = processFieldResult;
      IEnumerable<WorkItemRule> rules = fieldRuleMap.ContainsKey(processFieldResult.ReferenceName) ? fieldRuleMap[processFieldResult.ReferenceName] : (IEnumerable<WorkItemRule>) null;
      int num = source == null ? 0 : (source.Contains<string>(fieldRefName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? 1 : 0);
      int expand1 = (int) expand;
      return typeFieldFactory.Create(processId1, witRefName1, fieldRefName1, field, rules, num != 0, (ProcessWorkItemTypeFieldsExpandLevel) expand1);
    }

    [HttpGet]
    [ClientExample("GET_all_work_item_type_fields_async.json", "Returns a list of all fields in a work item type", null, null)]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    public IEnumerable<ProcessWorkItemTypeField> GetAllWorkItemTypeFields(
      Guid processId,
      string witRefName)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName);
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      HashSet<string> customizedFields = (HashSet<string>) null;
      IReadOnlyCollection<ProcessFieldResult> legacyFields = (service.GetAllWorkItemTypes(this.TfsRequestContext, processId, true).FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => t.ReferenceName == witRefName)) ?? throw new ProcessWorkItemTypeDoesNotExistException(witRefName, processId.ToString())).GetLegacyFields(this.TfsRequestContext);
      try
      {
        ProcessTypelet typelet = service.GetTypelet<ProcessTypelet>(this.TfsRequestContext, processId, witRefName, true);
        HashSet<string> stringSet;
        if (typelet == null)
        {
          stringSet = (HashSet<string>) null;
        }
        else
        {
          IEnumerable<WorkItemTypeExtensionFieldEntry> fields = typelet.Fields;
          stringSet = fields != null ? fields.ToHashSet<WorkItemTypeExtensionFieldEntry, string>((Func<WorkItemTypeExtensionFieldEntry, string>) (f => f.Field.ReferenceName)) : (HashSet<string>) null;
        }
        customizedFields = stringSet;
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
      }
      Dictionary<string, IEnumerable<WorkItemRule>> fieldRuleMap = ProcessWorkItemTypeFieldsController.CreateFieldRuleMap(service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName));
      ProcessWorkItemTypeFieldFactory factory = new ProcessWorkItemTypeFieldFactory(this.TfsRequestContext);
      IEnumerable<ProcessWorkItemTypeField> source = legacyFields.Select<ProcessFieldResult, ProcessWorkItemTypeField>((Func<ProcessFieldResult, ProcessWorkItemTypeField>) (f => factory.Create(processId, witRefName, f.ReferenceName, f, fieldRuleMap.ContainsKey(f.ReferenceName) ? fieldRuleMap[f.ReferenceName] : (IEnumerable<WorkItemRule>) null, customizedFields != null && customizedFields.Contains<string>(f.ReferenceName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))));
      return source == null ? (IEnumerable<ProcessWorkItemTypeField>) null : (IEnumerable<ProcessWorkItemTypeField>) source.ToList<ProcessWorkItemTypeField>();
    }

    [HttpPatch]
    [ClientExample("PATCH_work_item_type_field.json", "Updates a field in a work item type", null, null)]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    [ClientResponseType(typeof (ProcessWorkItemTypeField), null, null)]
    public HttpResponseMessage UpdateWorkItemTypeField(
      Guid processId,
      string witRefName,
      string fieldRefName,
      [FromBody] UpdateProcessWorkItemTypeFieldRequest field)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName, fieldRefName);
      if (field == null)
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      WorkItemTypeletFieldRuleProperties serverProperties = field != null ? field.ToServerProperties(this.TfsRequestContext) : (WorkItemTypeletFieldRuleProperties) null;
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType processWorkItemType = service.AddOrUpdateWorkItemTypeField(this.TfsRequestContext, processId, witRefName, fieldRefName, serverProperties);
      FieldEntry witField = processWorkItemType.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ReferenceName.Equals(fieldRefName, StringComparison.OrdinalIgnoreCase)))?.Field;
      if (witField == null)
        throw new ProcessWorkItemTypeFieldDoesNotExistException(fieldRefName, witRefName);
      ProcessWorkItemTypeFieldFactory typeFieldFactory = new ProcessWorkItemTypeFieldFactory(this.TfsRequestContext);
      IEnumerable<WorkItemFieldRule> rulesForWorkItemType = service.GetAllRulesForWorkItemType(this.TfsRequestContext, processId, witRefName);
      IEnumerable<WorkItemRule> rules = rulesForWorkItemType != null ? rulesForWorkItemType.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals(witField.ReferenceName, fr.Field))).SelectMany<WorkItemFieldRule, WorkItemRule>((Func<WorkItemFieldRule, IEnumerable<WorkItemRule>>) (fr => (IEnumerable<WorkItemRule>) fr.SubRules)) : (IEnumerable<WorkItemRule>) null;
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("FieldReferenceName", (object) fieldRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      ProcessWorkItemTypeFieldsExpandLevel expand = field == null || field.AllowedValues == null ? ProcessWorkItemTypeFieldsExpandLevel.None : ProcessWorkItemTypeFieldsExpandLevel.AllowedValues;
      return this.Request.CreateResponse<ProcessWorkItemTypeField>(HttpStatusCode.OK, typeFieldFactory.Create(processId, witRefName, fieldRefName, witField, rules, processWorkItemType.IsCustomType ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Custom : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Inherited, expand));
    }

    [HttpDelete]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RemoveWorkItemTypeField(
      Guid processId,
      string witRefName,
      string fieldRefName)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName, fieldRefName);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().RemoveWorkItemTypeField(this.TfsRequestContext, processId, witRefName, fieldRefName);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Remove");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("FieldReferenceName", (object) fieldRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    private static Dictionary<string, IEnumerable<WorkItemRule>> CreateFieldRuleMap(
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      return fieldRules.GroupBy<WorkItemFieldRule, string, List<WorkItemRule>>((Func<WorkItemFieldRule, string>) (fr => fr.Field), (Func<WorkItemFieldRule, List<WorkItemRule>>) (fr => ((IEnumerable<WorkItemRule>) fr.SubRules).ToList<WorkItemRule>())).ToDictionary<IGrouping<string, List<WorkItemRule>>, string, IEnumerable<WorkItemRule>>((Func<IGrouping<string, List<WorkItemRule>>, string>) (g => g.Key), (Func<IGrouping<string, List<WorkItemRule>>, IEnumerable<WorkItemRule>>) (g =>
      {
        IEnumerable<WorkItemRule> source = g.SelectMany<List<WorkItemRule>, WorkItemRule>((Func<List<WorkItemRule>, IEnumerable<WorkItemRule>>) (x => (IEnumerable<WorkItemRule>) x.ToList<WorkItemRule>()));
        if (source == null)
          return (IEnumerable<WorkItemRule>) null;
        List<WorkItemRule> list = source.ToList<WorkItemRule>();
        return list == null ? (IEnumerable<WorkItemRule>) null : list.Select<WorkItemRule, WorkItemRule>((Func<WorkItemRule, WorkItemRule>) (r => r));
      }));
    }

    private static void CheckArgsForNull(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
    }

    private static void CheckArgsForNull(Guid processId, string witRefName, string fieldRefName)
    {
      ProcessWorkItemTypeFieldsController.CheckArgsForNull(processId, witRefName);
      if (string.IsNullOrEmpty(fieldRefName))
        throw new VssPropertyValidationException(nameof (fieldRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (fieldRefName)));
    }
  }
}
