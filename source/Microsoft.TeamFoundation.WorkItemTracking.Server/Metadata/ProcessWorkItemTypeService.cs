// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessWorkItemTypeService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemType;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessWorkItemTypeService : 
    BaseTeamFoundationWorkItemTrackingService,
    IProcessWorkItemTypeService,
    IVssFrameworkService
  {
    private static HashSet<string> s_oobFieldsBlockedFromCusomization = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)
    {
      "Microsoft.VSTS.CMMI.Blocked",
      "Microsoft.VSTS.CMMI.CalledBy",
      "Microsoft.VSTS.CMMI.CalledDate",
      "Microsoft.VSTS.Common.ActivatedBy",
      "Microsoft.VSTS.Common.ActivatedDate",
      "Microsoft.VSTS.Common.ClosedBy",
      "Microsoft.VSTS.Common.ClosedDate",
      "Microsoft.VSTS.Common.ResolvedBy",
      "Microsoft.VSTS.Common.ResolvedDate",
      "Microsoft.VSTS.Common.ResolvedReason",
      "Microsoft.VSTS.Common.ReviewedBy",
      "Microsoft.VSTS.Common.StateChangeDate",
      "Microsoft.VSTS.Common.StateCode",
      "Microsoft.VSTS.Scheduling.RemainingWork",
      "Microsoft.VSTS.TCM.AutomatedTestId",
      "Microsoft.VSTS.TCM.AutomationStatus",
      "Microsoft.VSTS.TCM.QueryText",
      "Microsoft.VSTS.TCM.TestSuiteType",
      "Microsoft.VSTS.TCM.TestSuiteTypeId"
    };

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
    }

    public virtual ComposedWorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool bypassCache = false,
      bool onlyCustomizable = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      return requestContext.TraceBlock<ComposedWorkItemType>(910801, 910802, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetWorkItemType), (Func<ComposedWorkItemType>) (() =>
      {
        ComposedWorkItemType itemTypeInternal = this.GetWorkItemTypeInternal(requestContext, processId, witReferenceName, bypassCache);
        if (itemTypeInternal != null && (!onlyCustomizable || !((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(witReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName)))
          return itemTypeInternal;
        throw new ProcessWorkItemTypeDoesNotExistException(witReferenceName, processId.ToString("D"));
      }));
    }

    public virtual bool TryGetWorkItemTypeByName(
      IVssRequestContext requestContext,
      Guid processId,
      string witName,
      out BaseWorkItemType wit,
      bool bypassCache = false,
      bool includeParentProcessWits = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witName, nameof (witName));
      wit = (BaseWorkItemType) null;
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
      wit = this.GetWorkItemTypes(requestContext, processId, bypassCache, false).Where<BaseWorkItemType>((Func<BaseWorkItemType, bool>) (w => TFStringComparer.WorkItemTypeName.Equals(w.Name, witName))).FirstOrDefault<BaseWorkItemType>();
      return ((wit != null ? 0 : (processDescriptor.IsDerived ? 1 : 0)) & (includeParentProcessWits ? 1 : 0)) != 0 ? this.TryGetWorkItemTypeByName(requestContext, processDescriptor.Inherits, witName, out wit, bypassCache, false) : wit != null;
    }

    public IReadOnlyCollection<BaseWorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false,
      bool onlyCustomizable = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyCollection<BaseWorkItemType>) requestContext.TraceBlock<List<BaseWorkItemType>>(910803, 910804, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetWorkItemTypes), (Func<List<BaseWorkItemType>>) (() =>
      {
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
        IEnumerable<BaseWorkItemType> source = !processDescriptor.IsDerived ? requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processDescriptor).WorkItemTypeDefinitions.Cast<BaseWorkItemType>() : this.GetTypelets<ProcessWorkItemType>(requestContext, processId, bypassCache).Cast<BaseWorkItemType>();
        return onlyCustomizable ? source.Where<BaseWorkItemType>((Func<BaseWorkItemType, bool>) (t => !((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(t.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))).ToList<BaseWorkItemType>() : source.ToList<BaseWorkItemType>();
      }));
    }

    public virtual IReadOnlyCollection<ComposedWorkItemType> GetAllWorkItemTypes(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false,
      bool onlyCustomizable = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return requestContext.TraceBlock<IReadOnlyCollection<ComposedWorkItemType>>(910805, 910806, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetAllWorkItemTypes), (Func<IReadOnlyCollection<ComposedWorkItemType>>) (() =>
      {
        IReadOnlyCollection<ComposedWorkItemType> workItemTypes = new ProcessWorkItemTypeService.ComposedProcess(requestContext, processId, bypassCache).GetWorkItemTypes(requestContext);
        return onlyCustomizable ? (IReadOnlyCollection<ComposedWorkItemType>) workItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => !((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(t.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))).ToList<ComposedWorkItemType>() : workItemTypes;
      }));
    }

    public IReadOnlyCollection<ProcessFieldResult> GetAvailableFields(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      return (IReadOnlyCollection<ProcessFieldResult>) requestContext.TraceBlock<List<ProcessFieldResult>>(910807, 910808, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetAvailableFields), (Func<List<ProcessFieldResult>>) (() =>
      {
        IReadOnlyCollection<ProcessFieldResult> fields = requestContext.GetService<IProcessFieldService>().GetFields(requestContext, processId);
        ComposedWorkItemType itemTypeInternal = this.GetWorkItemTypeInternal(requestContext, processId, witReferenceName, true);
        List<ProcessFieldResult> availableFields = (List<ProcessFieldResult>) null;
        if (fields != null)
        {
          availableFields = fields.ToList<ProcessFieldResult>();
          if (itemTypeInternal != null)
          {
            IReadOnlyCollection<ProcessFieldResult> witFields = itemTypeInternal.GetLegacyFields(requestContext);
            availableFields.RemoveAll((Predicate<ProcessFieldResult>) (f => witFields.Contains<ProcessFieldResult>(f)));
          }
        }
        return availableFields;
      }));
    }

    public ProcessWorkItemType CreateDerivedWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string parentTypeReferenceName,
      string description,
      string color,
      string icon,
      bool? isDisabled = null,
      string proposedReferenceName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(parentTypeReferenceName, nameof (parentTypeReferenceName));
      string methodName = nameof (CreateDerivedWorkItemType);
      return requestContext.TraceBlock<ProcessWorkItemType>(910809, 910810, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (CreateDerivedWorkItemType), (Func<ProcessWorkItemType>) (() =>
      {
        if (((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(parentTypeReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTypeCustomizationBlocked((object) parentTypeReferenceName));
        description = CommonWITUtils.ValidateAndSanitizeDescription(description, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemType() + " : " + parentTypeReferenceName);
        CommonWITUtils.CheckColor(color);
        WorkItemTypeIconUtils.ValidateIcon(requestContext, icon);
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
        ComposedWorkItemType workItemType = this.GetWorkItemType(requestContext, processDescriptor.Inherits, parentTypeReferenceName, true, false);
        if (workItemType == null)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ParentWorkItemTypeDoesNotExist((object) parentTypeReferenceName, (object) processDescriptor.Inherits));
        if (this.GetTypeletsInheritingFromParent(requestContext, parentTypeReferenceName, this.GetTypelets<ProcessWorkItemType>(requestContext, processId, true)).FirstOrDefault<ProcessWorkItemType>() != null)
          throw new ProcessWorkItemTypeAlreadyExistException(parentTypeReferenceName, processDescriptor.Name);
        string str = proposedReferenceName ?? CommonWITUtils.GenerateReferenceName(processDescriptor.Name, workItemType.Name);
        if (this.IsTypeletReferenceNameInUse(requestContext, processDescriptor.TypeId, str, false))
          str = CommonWITUtils.GenerateUniqueRefName();
        if (isDisabled.HasValue && isDisabled.Value)
          this.CheckDisableNotBlocked(parentTypeReferenceName, workItemType.Name);
        ProcessWorkItemType itemTypeInternal = this.CreateProcessWorkItemTypeInternal(requestContext, processId, workItemType.Name, str, parentTypeReferenceName, description, Enumerable.Empty<string>(), (IEnumerable<WorkItemFieldRule>) null, (IEnumerable<Guid>) null, (string) null, color, icon, isDisabled: isDisabled);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, "CustomizedWorkItemType", parentTypeReferenceName);
        return itemTypeInternal;
      }));
    }

    public ProcessWorkItemType CreateWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string name,
      string color,
      string icon,
      string description,
      bool? IsDisabled = null,
      string proposedReferenceName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(color, nameof (color));
      string methodName = nameof (CreateWorkItemType);
      return requestContext.TraceBlock<ProcessWorkItemType>(910811, 910812, "WorkItemType", nameof (ProcessWorkItemTypeService), methodName, (Func<ProcessWorkItemType>) (() =>
      {
        name = CommonWITUtils.RemoveASCIIControlCharactersAndTrim(name);
        CommonWITUtils.CheckValidName(name, 128);
        description = CommonWITUtils.ValidateAndSanitizeDescription(description, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemType() + " : " + name);
        CommonWITUtils.CheckColor(color);
        WorkItemTypeIconUtils.ValidateIcon(requestContext, icon);
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
        string witReferenceName = proposedReferenceName ?? CommonWITUtils.GenerateReferenceName(processDescriptor.Name, name);
        this.CheckWorkItemTypesCountLimit(requestContext, processDescriptor.Name, this.GetTypelets<ProcessWorkItemType>(requestContext, processId, false).Count<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => !t.IsDerived)));
        this.CheckWorkItemTypeDoesNotAlreadyExists(requestContext, processDescriptor, name, ref witReferenceName);
        IEnumerable<WorkItemFieldRule> standardFieldRules = this.GetStandardFieldRules(requestContext);
        CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
        IReadOnlyCollection<WorkItemStateDeclaration> defaultStateModel = WorkItemStateDefinitionService.GetDefaultStateModel(processDescriptor, culture);
        Layout layout = new Layout();
        IReadOnlyCollection<string> fieldReferenceName = WorkItemStateDefinitionService.CombinedWorkFlowFieldReferenceName;
        ProcessWorkItemType itemTypeInternal = this.CreateProcessWorkItemTypeInternal(requestContext, processId, name, witReferenceName, SystemWorkItemType.ReferenceName, description, (IEnumerable<string>) fieldReferenceName, standardFieldRules, (IEnumerable<Guid>) null, JsonConvert.SerializeObject((object) layout), color, icon, defaultStateModel, IsDisabled);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, "NewWorkItemType", name);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.CreateWorkItemType);
        return itemTypeInternal;
      }));
    }

    public void DestroyWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witReferenceName, nameof (witReferenceName));
      string str = nameof (DestroyWorkItemType);
      requestContext.TraceEnter(910819, "WorkItemType", nameof (ProcessWorkItemTypeService), str);
      try
      {
        witReferenceName = witReferenceName.Trim();
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        this.DestroyProcessWorkItemTypeInternal(requestContext, processId, witReferenceName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), str, "DeletedWorkItemTypeReferenceName", witReferenceName);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.DestroyWorkItemType);
      }
      finally
      {
        requestContext.TraceLeave(910820, "WorkItemType", nameof (ProcessWorkItemTypeService), str);
      }
    }

    public string RemoveUnusedContent(IVssRequestContext requestContext, bool readOnlyMode = false)
    {
      string str = string.Empty;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        str = component.RemoveUnusedContent(readOnlyMode);
      if (readOnlyMode)
        return str;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
      {
        str += ";Removing Destroyed Content";
        component.RemoveDestroyedContent();
      }
      return str;
    }

    public string RemoveDeletedProcesses(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return string.Empty;
      Guid userId = requestContext.GetUserId();
      string empty = string.Empty;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.RemoveDeletedProcesses(userId);
    }

    public ProcessWorkItemType AddOrUpdateWorkItemTypeField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      WorkItemTypeletFieldRuleProperties fieldProps,
      bool bypassSystemFieldPropertiesUpdate = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      string methodName = nameof (AddOrUpdateWorkItemTypeField);
      return requestContext.TraceBlock<ProcessWorkItemType>(910813, 910814, "WorkItemType", nameof (ProcessWorkItemTypeService), methodName, (Func<ProcessWorkItemType>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        IProcessFieldService service = requestContext.GetService<IProcessFieldService>();
        FieldEntry fieldEntry = service.EnsureFieldExists(requestContext, fieldReferenceName);
        if (fieldProps != null && fieldProps.AllowedValues != null)
          fieldProps.AllowedValues = CommonWITUtils.ValidateAndGetPickListItems(requestContext, (IReadOnlyList<string>) fieldProps.AllowedValues, fieldEntry.FieldType, fieldEntry.ReferenceName).ToArray<string>();
        fieldEntry = service.ConvertFieldWithAllowedValuesToPicklist(requestContext, fieldEntry);
        IEnumerable<string> strings = this.MergeFields(typelet, fieldReferenceName, (string) null);
        IEnumerable<WorkItemTypeExtensionFieldEntry> fields = typelet.Fields;
        int num1 = fields != null ? fields.Count<WorkItemTypeExtensionFieldEntry>() : 0;
        int mergedFieldCount = strings.Count<string>();
        int num2 = mergedFieldCount;
        if (num1 < num2)
          this.CheckFieldCountLimit(requestContext, witReferenceName, mergedFieldCount);
        IEnumerable<WorkItemFieldRule> fieldRules = typelet.FieldRules;
        WorkItemFieldRule existingFieldRule = fieldRules != null ? fieldRules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals(fr.Field, fieldReferenceName))) : (WorkItemFieldRule) null;
        if (existingFieldRule == null)
          existingFieldRule = new WorkItemFieldRule()
          {
            Field = fieldReferenceName
          };
        WorkItemFieldRule workItemFieldRule = ProcessWorkItemTypeService.UpdateFieldRuleWithFieldProperties(requestContext, existingFieldRule, fieldProps);
        this.CheckAndFormatFieldRuleValueType(requestContext, fieldEntry, workItemFieldRule);
        RuleChangeDescriptor ruleChanges = (RuleChangeDescriptor) null;
        if (service.GetAllOutOfBoxFieldReferenceNameToNameMappings(requestContext).ContainsKey(fieldReferenceName))
        {
          ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
          if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && !bypassSystemFieldPropertiesUpdate)
            ruleChanges = this.ValidateFieldPropertiesAndGetRuleChanges(requestContext, processDescriptor, typelet, fieldEntry, workItemFieldRule);
          else if (typelet.IsDerived && this.GetWorkItemType(requestContext, processDescriptor.Inherits, typelet.ParentTypeRefName, false, false).GetLegacyFields(requestContext).Any<ProcessFieldResult>((Func<ProcessFieldResult, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, fieldReferenceName))))
            workItemFieldRule.ClearSubRules();
        }
        workItemFieldRule.FieldId = fieldEntry.FieldId;
        workItemFieldRule.Field = fieldEntry.ReferenceName;
        IEnumerable<WorkItemFieldRule> workItemFieldRules = (IEnumerable<WorkItemFieldRule>) this.MergeFieldRules(typelet.FieldRules, workItemFieldRule, !((IEnumerable<WorkItemRule>) workItemFieldRule.SubRules).Any<WorkItemRule>() ? fieldReferenceName : (string) null);
        ProcessWorkItemTypeService.CheckBooleanFieldsHaveRequiredAndDefaultValue(workItemFieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldEntry.FieldId)).FirstOrDefault<WorkItemFieldRule>(), fieldEntry);
        IEnumerable<Guid> disabledRules = typelet.IsDerived ? (IEnumerable<Guid>) this.GetMergedDisabledRules(typelet, ruleChanges) : (IEnumerable<Guid>) new List<Guid>();
        ProcessWorkItemType processWorkItemType = this.UpdateProcessWorkItemTypeInternal(requestContext, processId, typelet, typelet.Description, strings, workItemFieldRules, disabledRules, (string) null);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdatedWorkItemType", string.Format("[NonEmail: {0}]", (object) witReferenceName));
        properties.Add("UpdatedField", fieldReferenceName);
        properties.Add("UpdateFieldRuleProperties", (object) fieldProps);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, properties);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.UpdateWorkItemTypeField);
        return processWorkItemType;
      }));
    }

    public ProcessWorkItemType RemoveWorkItemTypeField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      bool suppressPermissionCheck = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      return requestContext.TraceBlock<ProcessWorkItemType>(910815, 910816, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (RemoveWorkItemTypeField), (Func<ProcessWorkItemType>) (() =>
      {
        if (!suppressPermissionCheck)
          WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        if (typelet.Fields == null)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTypeFieldDoesNotExist((object) witReferenceName, (object) fieldReferenceName));
        WorkItemTypeExtensionFieldEntry extensionFieldEntry = typelet.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldReferenceName, f.Field.ReferenceName)));
        if (extensionFieldEntry == null)
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTypeFieldDoesNotExist((object) witReferenceName, (object) fieldReferenceName));
        IEnumerable<string> fieldReferenceNames = this.MergeFields(typelet, (string) null, fieldReferenceName);
        ICollection<WorkItemFieldRule> fieldRules = this.MergeFieldRules(typelet.FieldRules, (WorkItemFieldRule) null, fieldReferenceName, extensionFieldEntry.Field.FieldId);
        string form = JsonConvert.SerializeObject((object) new LayoutOperations().RemoveControlFromLayout(typelet.ReferenceName, typelet.Form, typelet.Form, fieldReferenceName));
        ProcessWorkItemType processWorkItemType = this.UpdateProcessWorkItemTypeInternal(requestContext, processId, typelet, typelet.Description, fieldReferenceNames, (IEnumerable<WorkItemFieldRule>) fieldRules, (IEnumerable<Guid>) null, form);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.RemoveWorkItemTypeField);
        return processWorkItemType;
      }));
    }

    public ProcessWorkItemType UpdateWorkItemTypeRules(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      IEnumerable<string> fieldReferenceNames,
      IDictionary<string, WorkItemRule> workItemFieldRules,
      ISet<Guid> ruleIdsToUpdate = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForNull<IDictionary<string, WorkItemRule>>(workItemFieldRules, nameof (workItemFieldRules));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fieldReferenceNames, nameof (fieldReferenceNames));
      string methodName = nameof (UpdateWorkItemTypeRules);
      return requestContext.TraceBlock<ProcessWorkItemType>(910813, 910814, "WorkItemType", nameof (ProcessWorkItemTypeService), methodName, (Func<ProcessWorkItemType>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        ProcessFieldService service = requestContext.GetService<ProcessFieldService>();
        IEnumerable<WorkItemFieldRule> workItemFieldRules1 = typelet.FieldRules;
        List<WorkItemFieldRule> workItemFieldRuleList1 = new List<WorkItemFieldRule>();
        if (ruleIdsToUpdate != null && workItemFieldRules1 != null)
        {
          List<WorkItemFieldRule> workItemFieldRuleList2 = new List<WorkItemFieldRule>();
          foreach (WorkItemFieldRule workItemFieldRule in workItemFieldRules1)
          {
            workItemFieldRule.RemoveRules(ruleIdsToUpdate);
            if (workItemFieldRule.SubRules.Length != 0)
              workItemFieldRuleList2.Add(workItemFieldRule);
          }
          workItemFieldRules1 = (IEnumerable<WorkItemFieldRule>) workItemFieldRuleList2;
        }
        foreach (string fieldReferenceName in fieldReferenceNames)
        {
          FieldEntry fieldEntry = service.EnsureFieldExists(requestContext, fieldReferenceName);
          WorkItemRule ruleToAdd = (WorkItemRule) null;
          workItemFieldRules.TryGetValue(fieldReferenceName, out ruleToAdd);
          WorkItemFieldRule workItemFieldRule = ProcessWorkItemTypeService.MergeWorkItemRules(requestContext, workItemFieldRules1 != null ? workItemFieldRules1.ToList<WorkItemFieldRule>() : (List<WorkItemFieldRule>) null, fieldReferenceName, ruleToAdd);
          this.CheckAndFormatFieldRuleValueType(requestContext, fieldEntry, workItemFieldRule);
          workItemFieldRule.FieldId = fieldEntry.FieldId;
          workItemFieldRules1 = (IEnumerable<WorkItemFieldRule>) this.MergeFieldRules(workItemFieldRules1, workItemFieldRule, workItemFieldRule == null ? fieldReferenceName : (string) null);
          ProcessWorkItemTypeService.CheckBooleanFieldsHaveRequiredAndDefaultValue(workItemFieldRules1.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldEntry.FieldId)).FirstOrDefault<WorkItemFieldRule>(), fieldEntry);
          workItemFieldRuleList1.Add(workItemFieldRule);
        }
        ProcessWorkItemType processWorkItemType = this.UpdateProcessWorkItemTypeInternal(requestContext, processId, typelet, typelet.Description, (IEnumerable<string>) null, workItemFieldRules1, (IEnumerable<Guid>) null, (string) null);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdatedWorkItemType", witReferenceName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, properties);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.UpdateWorkItemTypeField);
        return processWorkItemType;
      }));
    }

    public void RemoveWorkItemTypeRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid ruleId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForEmptyGuid(ruleId, nameof (ruleId));
      string methodName = nameof (RemoveWorkItemTypeRule);
      requestContext.TraceBlock(910813, 910814, "WorkItemType", nameof (ProcessWorkItemTypeService), methodName, (Action) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        IEnumerable<WorkItemFieldRule> fieldRules = typelet.FieldRules;
        if (fieldRules != null)
        {
          List<WorkItemFieldRule> workItemFieldRuleList = new List<WorkItemFieldRule>();
          foreach (WorkItemFieldRule workItemFieldRule in fieldRules)
          {
            workItemFieldRule.RemoveRule(ruleId);
            if (workItemFieldRule.SubRules.Length != 0)
              workItemFieldRuleList.Add(workItemFieldRule);
          }
          fieldRules = (IEnumerable<WorkItemFieldRule>) workItemFieldRuleList;
        }
        this.UpdateProcessWorkItemTypeInternal(requestContext, processId, typelet, typelet.Description, (IEnumerable<string>) null, fieldRules, (IEnumerable<Guid>) null, (string) null);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdatedWorkItemType", witReferenceName);
        properties.Add("RemoveRule", (object) ruleId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, properties);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.UpdateWorkItemTypeField);
      }));
    }

    public IEnumerable<WorkItemRule> GetCombinedRulesForField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      bool includeDisabledRules = false)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      IEnumerable<WorkItemFieldRule> rulesForWorkItemType = this.GetAllRulesForWorkItemType(requestContext, processId, witReferenceName, includeDisabledRules);
      IEnumerable<WorkItemRule> workItemRules;
      if (rulesForWorkItemType == null)
      {
        workItemRules = (IEnumerable<WorkItemRule>) null;
      }
      else
      {
        IEnumerable<WorkItemFieldRule> source = rulesForWorkItemType.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.Field, fieldReferenceName)));
        workItemRules = source != null ? source.SelectMany<WorkItemFieldRule, WorkItemRule>((Func<WorkItemFieldRule, IEnumerable<WorkItemRule>>) (r => (IEnumerable<WorkItemRule>) r.SubRules)) : (IEnumerable<WorkItemRule>) null;
      }
      return workItemRules ?? Enumerable.Empty<WorkItemRule>();
    }

    public IEnumerable<WorkItemFieldRule> GetAllRulesForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool includeDisabledRules = false)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      return (IEnumerable<WorkItemFieldRule>) this.GetWorkItemType(requestContext, processId, witReferenceName, true, false).GetFieldRules(requestContext, includeDisabledRules);
    }

    public static WorkItemFieldRule MergeWorkItemRules(
      IVssRequestContext requestContext,
      List<WorkItemFieldRule> allFieldRules,
      string fieldReferenceName,
      WorkItemRule ruleToAdd)
    {
      WorkItemFieldRule workItemFieldRule = allFieldRules != null ? allFieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals(fr.Field, fieldReferenceName))).FirstOrDefault<WorkItemFieldRule>() : (WorkItemFieldRule) null;
      if (workItemFieldRule == null)
      {
        workItemFieldRule = new WorkItemFieldRule()
        {
          Field = fieldReferenceName
        };
        allFieldRules?.Add(workItemFieldRule);
      }
      if (ruleToAdd.Id != Guid.Empty)
        workItemFieldRule.RemoveRule(ruleToAdd.Id);
      ProcessTypeletRuleValidationContext validationHelper = new ProcessTypeletRuleValidationContext(requestContext);
      ruleToAdd.FixFieldReferences((IRuleValidationContext) validationHelper);
      workItemFieldRule.AddRule<WorkItemRule>(ruleToAdd);
      return workItemFieldRule;
    }

    public static WorkItemTypeletFieldRuleProperties ExtractPropertiesFromRules(
      IEnumerable<WorkItemRule> fieldRules,
      bool isIdentityField)
    {
      string str = (string) null;
      RuleValueFrom ruleValueFrom = RuleValueFrom.Value;
      bool flag = fieldRules.OfType<ReadOnlyRule>().Any<ReadOnlyRule>();
      fieldRules = (IEnumerable<WorkItemRule>) fieldRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is ReadOnlyRule))).ToList<WorkItemRule>();
      int num1 = fieldRules.OfType<RequiredRule>().Any<RequiredRule>() ? 1 : 0;
      fieldRules = (IEnumerable<WorkItemRule>) fieldRules.Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is RequiredRule))).ToList<WorkItemRule>();
      DefaultRule defaultRule = fieldRules.OfType<DefaultRule>().FirstOrDefault<DefaultRule>();
      if (defaultRule != null)
      {
        ruleValueFrom = defaultRule.ValueFrom;
        str = defaultRule is IdentityDefaultRule ? (defaultRule as IdentityDefaultRule).Vsid.ToString() : defaultRule.Value;
      }
      AllowedValuesRule allowedValuesRule = fieldRules.OfType<AllowedValuesRule>().FirstOrDefault<AllowedValuesRule>();
      bool? nullable = allowedValuesRule != null & isIdentityField ? new bool?(((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).Any<ConstantSetReference>((Func<ConstantSetReference, bool>) (s => s.Id == -1))) : new bool?();
      string[] strArray = (string[]) null;
      if (!isIdentityField && allowedValuesRule != null && allowedValuesRule.Values != null && allowedValuesRule.Values.Any<string>())
        strArray = allowedValuesRule.Values.ToArray<string>();
      int num2 = flag ? 1 : 0;
      string defaultValue = str;
      int defaultValueFrom = (int) ruleValueFrom;
      bool? allowGroups = nullable;
      string[] allowedValues = strArray;
      return new WorkItemTypeletFieldRuleProperties(num1 != 0, num2 != 0, defaultValue, (RuleValueFrom) defaultValueFrom, allowGroups, allowedValues);
    }

    public void AddOrUpdateHelpTextRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName,
      string helpText)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(helpText, nameof (helpText));
      HelpTextRule helpTextRule1 = this.GetHelpTextRule(requestContext, processId, witRefName, fieldRefName);
      if (helpTextRule1 != null)
      {
        if (helpTextRule1.Value == helpText)
          return;
        this.RemoveWorkItemTypeRule(requestContext, processId, witRefName, helpTextRule1.Id);
      }
      HelpTextRule helpTextRule2 = new HelpTextRule();
      helpTextRule2.Value = helpText;
      helpTextRule2.Id = Guid.NewGuid();
      HelpTextRule helpTextRule3 = helpTextRule2;
      this.UpdateWorkItemTypeRules(requestContext, processId, witRefName, (IEnumerable<string>) new string[1]
      {
        fieldRefName
      }, (IDictionary<string, WorkItemRule>) new Dictionary<string, WorkItemRule>()
      {
        {
          fieldRefName,
          (WorkItemRule) helpTextRule3
        }
      }, (ISet<Guid>) null);
    }

    public HelpTextRule GetHelpTextRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witRefName, nameof (witRefName));
      ProcessTypelet processTypelet;
      this.TryGetTypelet<ProcessTypelet>(requestContext, processId, witRefName, false, out processTypelet);
      if (processTypelet == null)
        return (HelpTextRule) null;
      IEnumerable<WorkItemFieldRule> fieldRules = processTypelet.FieldRules;
      WorkItemFieldRule workItemFieldRule = fieldRules != null ? fieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => fr.Field == fieldRefName)).FirstOrDefault<WorkItemFieldRule>() : (WorkItemFieldRule) null;
      return workItemFieldRule == null ? (HelpTextRule) null : workItemFieldRule.SubRules.OfType<HelpTextRule>().FirstOrDefault<HelpTextRule>();
    }

    public static IEnumerable<WorkItemRule> CreateWorkItemRulesFromFieldProperties(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      WorkItemTypeletFieldRuleProperties fieldRuleProperties)
    {
      if (fieldRuleProperties == null)
        return Enumerable.Empty<WorkItemRule>();
      List<WorkItemRule> fromFieldProperties = new List<WorkItemRule>();
      if (fieldRuleProperties.IsRequired)
        fromFieldProperties.Add((WorkItemRule) new RequiredRule());
      if (fieldRuleProperties.IsReadOnly)
        fromFieldProperties.Add((WorkItemRule) new ReadOnlyRule());
      FieldEntry field;
      bool flag1 = requestContext.WitContext().FieldDictionary.TryGetField(fieldReferenceName, out field) && field.IsIdentity;
      if (fieldRuleProperties.DefaultValueFrom == RuleValueFrom.Clock)
      {
        List<WorkItemRule> workItemRuleList = fromFieldProperties;
        DefaultRule defaultRule = new DefaultRule();
        defaultRule.ValueFrom = fieldRuleProperties.DefaultValueFrom;
        workItemRuleList.Add((WorkItemRule) defaultRule);
      }
      else if (fieldRuleProperties.DefaultValueFrom == RuleValueFrom.CurrentUser)
      {
        if (flag1)
        {
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          IdentityDefaultRule identityDefaultRule = new IdentityDefaultRule();
          identityDefaultRule.ValueFrom = fieldRuleProperties.DefaultValueFrom;
          workItemRuleList.Add((WorkItemRule) identityDefaultRule);
        }
        else
        {
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          DefaultRule defaultRule = new DefaultRule();
          defaultRule.ValueFrom = fieldRuleProperties.DefaultValueFrom;
          workItemRuleList.Add((WorkItemRule) defaultRule);
        }
      }
      else if (!string.IsNullOrEmpty(fieldRuleProperties.DefaultValue))
      {
        if (flag1)
        {
          Guid result;
          Guid.TryParse(fieldRuleProperties.DefaultValue, out result);
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          IdentityDefaultRule identityDefaultRule = new IdentityDefaultRule();
          identityDefaultRule.Vsid = result;
          identityDefaultRule.ValueFrom = RuleValueFrom.Value;
          workItemRuleList.Add((WorkItemRule) identityDefaultRule);
        }
        else
        {
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          DefaultRule defaultRule = new DefaultRule();
          defaultRule.Value = fieldRuleProperties.DefaultValue;
          workItemRuleList.Add((WorkItemRule) defaultRule);
        }
      }
      if (flag1)
      {
        bool? allowGroups = fieldRuleProperties.AllowGroups;
        int num;
        if (allowGroups.HasValue)
        {
          allowGroups = fieldRuleProperties.AllowGroups;
          num = allowGroups.Value ? 1 : 0;
        }
        else
          num = 0;
        bool flag2 = num != 0;
        fromFieldProperties.Add((WorkItemRule) new AllowExistingValueRule());
        List<WorkItemRule> workItemRuleList = fromFieldProperties;
        AllowedValuesRule allowedValuesRule1 = new AllowedValuesRule();
        AllowedValuesRule allowedValuesRule2 = allowedValuesRule1;
        ExtendedConstantSetRef[] extendedConstantSetRefArray = new ExtendedConstantSetRef[1];
        ExtendedConstantSetRef extendedConstantSetRef = new ExtendedConstantSetRef();
        extendedConstantSetRef.Id = flag2 ? -1 : -2;
        extendedConstantSetRef.ExcludeGroups = !flag2;
        extendedConstantSetRefArray[0] = extendedConstantSetRef;
        ConstantSetReference[] constantSetReferenceArray = (ConstantSetReference[]) extendedConstantSetRefArray;
        allowedValuesRule2.Sets = constantSetReferenceArray;
        AllowedValuesRule allowedValuesRule3 = allowedValuesRule1;
        workItemRuleList.Add((WorkItemRule) allowedValuesRule3);
      }
      if (fieldRuleProperties?.AllowedValues != null)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        if (OOBFieldValues.HasAllowedValues(requestContext, field.ReferenceName))
        {
          properties.Add("AllowedValuesFound", true);
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          AllowedValuesRule allowedValuesRule = new AllowedValuesRule();
          allowedValuesRule.Id = Guid.NewGuid();
          allowedValuesRule.Values = new HashSet<string>(((IEnumerable<string>) fieldRuleProperties.AllowedValues).Select<string, string>((Func<string, string>) (s => s.ToString())), (IEqualityComparer<string>) TFStringComparer.AllowedValue);
          workItemRuleList.Add((WorkItemRule) allowedValuesRule);
        }
        else if (OOBFieldValues.HasSuggestedValues(requestContext, field.ReferenceName))
        {
          properties.Add("SuggestedValuesFound", true);
          List<WorkItemRule> workItemRuleList = fromFieldProperties;
          SuggestedValuesRule suggestedValuesRule = new SuggestedValuesRule();
          suggestedValuesRule.Id = Guid.NewGuid();
          suggestedValuesRule.Values = new HashSet<string>(((IEnumerable<string>) fieldRuleProperties.AllowedValues).Select<string, string>((Func<string, string>) (s => s.ToString())), (IEqualityComparer<string>) TFStringComparer.AllowedValue);
          workItemRuleList.Add((WorkItemRule) suggestedValuesRule);
        }
        else
          properties.Add("NeitherAllowedValuesNorSuggestedValuesFound", true);
        CustomerIntelligenceData intelligenceData = properties;
        IEnumerable<string> strings;
        if (fieldRuleProperties == null)
        {
          strings = (IEnumerable<string>) null;
        }
        else
        {
          string[] allowedValues = fieldRuleProperties.AllowedValues;
          strings = allowedValues != null ? ((IEnumerable<string>) allowedValues).Select<string, string>((Func<string, string>) (s => s.ToString())) : (IEnumerable<string>) null;
        }
        intelligenceData.Add(nameof (CreateWorkItemRulesFromFieldProperties), (object) strings);
        properties.Add(nameof (fieldReferenceName), fieldReferenceName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), nameof (CreateWorkItemRulesFromFieldProperties), properties);
      }
      return (IEnumerable<WorkItemRule>) fromFieldProperties;
    }

    public static IEnumerable<WorkItemRule> ExtractRulesWithoutFieldProperties(
      WorkItemFieldRule rule)
    {
      return rule == null ? (IEnumerable<WorkItemRule>) null : (IEnumerable<WorkItemRule>) ((IEnumerable<WorkItemRule>) rule.SubRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (subRule => !WorkItemTypeletFieldRuleProperties.RulesAffectedByFieldRuleProp.Contains(subRule.Name))).ToArray<WorkItemRule>();
    }

    public ProcessWorkItemType UpdateCustomForm(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Layout layout,
      bool suppressPermissionCheck = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForNull<Layout>(layout, nameof (layout));
      return requestContext.TraceBlock<ProcessWorkItemType>(910817, 910818, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (UpdateCustomForm), (Func<ProcessWorkItemType>) (() =>
      {
        if (!suppressPermissionCheck)
          WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        return this.UpdateProcessWorkItemTypeInternal(requestContext, processId, this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true), (string) null, (IEnumerable<string>) null, (IEnumerable<WorkItemFieldRule>) null, (IEnumerable<Guid>) null, JsonConvert.SerializeObject((object) layout));
      }));
    }

    public ProcessWorkItemType UpdateWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string color,
      string icon,
      string description,
      bool? isDisabled = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      return requestContext.TraceBlock<ProcessWorkItemType>(910819, 910820, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (UpdateWorkItemType), (Func<ProcessWorkItemType>) (() =>
      {
        description = CommonWITUtils.ValidateAndSanitizeDescription(description, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemType() + " : " + witReferenceName);
        if (color != null)
          CommonWITUtils.CheckColor(color);
        if (icon != null)
          WorkItemTypeIconUtils.ValidateIcon(requestContext, icon);
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        if (isDisabled.HasValue && isDisabled.Value)
          this.CheckDisableNotBlocked(typelet.ParentTypeRefName, typelet.Name);
        ProcessWorkItemType processWorkItemType = this.UpdateProcessWorkItemTypeInternal(requestContext, processId, typelet, description, (IEnumerable<string>) null, (IEnumerable<WorkItemFieldRule>) null, (IEnumerable<Guid>) null, (string) null, color, icon, isDisabled);
        this.PublishWorkItemTypeChangedEvent(requestContext, processId, WorkItemTypeChangeType.UpdateWorkItemType);
        return processWorkItemType;
      }));
    }

    public BehaviorRelation GetBehaviorForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeRefName,
      string behaviorRefName,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeRefName, nameof (workItemTypeRefName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorRefName, nameof (behaviorRefName));
      return requestContext.TraceBlock<BehaviorRelation>(910829, 910830, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetBehaviorForWorkItemType), (Func<BehaviorRelation>) (() =>
      {
        return this.GetBehaviorsForWorkItemType(requestContext, processId, workItemTypeRefName, bypassCache).FirstOrDefault<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(behaviorRefName, b.Behavior.ReferenceName))) ?? throw new BehaviorNotReferencedByWorkItemTypeException(workItemTypeRefName, behaviorRefName);
      }));
    }

    public virtual IReadOnlyCollection<BehaviorRelation> GetBehaviorsForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeRefName,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeRefName, nameof (workItemTypeRefName));
      ComposedWorkItemType workItemType = this.GetWorkItemType(requestContext, processId, workItemTypeRefName, bypassCache, false);
      IReadOnlyCollection<BehaviorRelation> source;
      return this.GetBehaviorsForWorkItemTypesInternal(requestContext, processId, (IReadOnlyCollection<ComposedWorkItemType>) new ComposedWorkItemType[1]
      {
        workItemType
      }, (bypassCache ? 1 : 0) != 0).TryGetValue(workItemTypeRefName, out source) && source != null ? (IReadOnlyCollection<BehaviorRelation>) source.ToList<BehaviorRelation>() : (IReadOnlyCollection<BehaviorRelation>) new List<BehaviorRelation>();
    }

    public virtual IReadOnlyDictionary<string, ICollection<ProcessWorkItemType>> GetDerivedWorkItemTypesInBehavior(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyDictionary<string, ICollection<ProcessWorkItemType>>) requestContext.TraceBlock<Dictionary<string, ICollection<ProcessWorkItemType>>>(910821, 910822, "WorkItemType", nameof (ProcessWorkItemTypeService), "GetWorkItemTypesInBehavior", (Func<Dictionary<string, ICollection<ProcessWorkItemType>>>) (() =>
      {
        IReadOnlyCollection<ProcessWorkItemType> typelets = this.GetTypelets<ProcessWorkItemType>(requestContext, processId, false);
        Dictionary<string, ICollection<ProcessWorkItemType>> itemTypesInBehavior = new Dictionary<string, ICollection<ProcessWorkItemType>>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
        if (typelets == null)
        {
          requestContext.Trace(910852, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), "wits is null");
          return itemTypesInBehavior;
        }
        foreach (ProcessWorkItemType processWorkItemType in (IEnumerable<ProcessWorkItemType>) typelets)
        {
          if (processWorkItemType == null || processWorkItemType.BehaviorRelations == null)
          {
            requestContext.Trace(910852, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), processWorkItemType == null ? "wit is null" : "Wit.behaviors is null");
          }
          else
          {
            foreach (BehaviorRelation behaviorRelation in (IEnumerable<BehaviorRelation>) processWorkItemType.BehaviorRelations)
            {
              ICollection<ProcessWorkItemType> processWorkItemTypes;
              if (itemTypesInBehavior.TryGetValue(behaviorRelation.Behavior.ReferenceName, out processWorkItemTypes))
                processWorkItemTypes.Add(processWorkItemType);
              else
                itemTypesInBehavior[behaviorRelation.Behavior.ReferenceName] = (ICollection<ProcessWorkItemType>) new List<ProcessWorkItemType>()
                {
                  processWorkItemType
                };
            }
          }
        }
        return itemTypesInBehavior;
      }));
    }

    public virtual IReadOnlyDictionary<string, BehaviorWorkItemTypes> GetWorkItemTypesInBehavior(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyDictionary<string, BehaviorWorkItemTypes>) requestContext.TraceBlock<Dictionary<string, BehaviorWorkItemTypes>>(910821, 910822, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetWorkItemTypesInBehavior), (Func<Dictionary<string, BehaviorWorkItemTypes>>) (() =>
      {
        Dictionary<string, BehaviorWorkItemTypes> itemTypesInBehavior = new Dictionary<string, BehaviorWorkItemTypes>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
        requestContext.GetService<ITeamFoundationProcessService>();
        requestContext.GetService<IBehaviorService>();
        IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = this.GetAllWorkItemTypes(requestContext, processId, false, false);
        if (allWorkItemTypes == null)
        {
          requestContext.Trace(910852, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), "wits is null");
          return itemTypesInBehavior;
        }
        IReadOnlyDictionary<string, IReadOnlyCollection<BehaviorRelation>> itemTypesInternal = this.GetBehaviorsForWorkItemTypesInternal(requestContext, processId, allWorkItemTypes, false);
        Dictionary<string, ComposedWorkItemType> dictionary = new Dictionary<string, ComposedWorkItemType>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
        foreach (ComposedWorkItemType composedWorkItemType1 in (IEnumerable<ComposedWorkItemType>) allWorkItemTypes)
        {
          if (composedWorkItemType1 == null)
          {
            requestContext.Trace(910852, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), "wit is null");
          }
          else
          {
            IReadOnlyCollection<BehaviorRelation> behaviorRelations;
            if (itemTypesInternal.TryGetValue(composedWorkItemType1.ReferenceName, out behaviorRelations))
            {
              foreach (BehaviorRelation behaviorRelation in (IEnumerable<BehaviorRelation>) behaviorRelations)
              {
                string referenceName = behaviorRelation.Behavior.ReferenceName;
                BehaviorWorkItemTypes behaviorWorkItemTypes = (BehaviorWorkItemTypes) null;
                if (!itemTypesInBehavior.TryGetValue(referenceName, out behaviorWorkItemTypes))
                {
                  behaviorWorkItemTypes = new BehaviorWorkItemTypes()
                  {
                    BehaviorReferenceName = referenceName,
                    WorkItemTypes = (ICollection<ComposedWorkItemType>) new List<ComposedWorkItemType>()
                  };
                  itemTypesInBehavior[referenceName] = behaviorWorkItemTypes;
                }
                if (behaviorRelation.IsDefault)
                {
                  ComposedWorkItemType composedWorkItemType2 = (ComposedWorkItemType) null;
                  if (dictionary.TryGetValue(referenceName, out composedWorkItemType2))
                  {
                    if (composedWorkItemType1.IsCustomType)
                      dictionary[referenceName] = composedWorkItemType1;
                  }
                  else
                    dictionary[referenceName] = composedWorkItemType1;
                }
                behaviorWorkItemTypes.WorkItemTypes.Add(composedWorkItemType1);
              }
            }
          }
        }
        foreach (BehaviorWorkItemTypes behaviorWorkItemTypes in itemTypesInBehavior.Values)
        {
          string behaviorReferenceName = behaviorWorkItemTypes.BehaviorReferenceName;
          ComposedWorkItemType composedWorkItemType = (ComposedWorkItemType) null;
          behaviorWorkItemTypes.DefaultWorkItemTypeName = !dictionary.TryGetValue(behaviorReferenceName, out composedWorkItemType) ? behaviorWorkItemTypes.WorkItemTypes.OrderBy<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (w => w.Name), (IComparer<string>) TFStringComparer.WorkItemTypeName).First<ComposedWorkItemType>().Name : composedWorkItemType.Name;
        }
        return itemTypesInBehavior;
      }));
    }

    public ProcessWorkItemType AddBehaviorToWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName,
      bool isDefault = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      return requestContext.TraceBlock<ProcessWorkItemType>(910823, 910824, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (AddBehaviorToWorkItemType), (Func<ProcessWorkItemType>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        witReferenceName = witReferenceName.Trim();
        behaviorReferenceName = behaviorReferenceName.Trim();
        ComposedWorkItemType workItemType = this.GetWorkItemType(requestContext, processId, witReferenceName, true, false);
        if (!workItemType.IsCustomType && !workItemType.IsDerived)
          throw new CannotReferenceBehaviorFromNonCustomWorkItemTypeException(witReferenceName);
        Behavior behavior = requestContext.GetService<IBehaviorService>().GetBehavior(requestContext, processId, behaviorReferenceName);
        if (behavior.IsAbstract)
          throw new CannotReferenceAbstractBehaviorException(behavior.ReferenceName);
        IReadOnlyDictionary<string, BehaviorWorkItemTypes> itemTypesInBehavior = this.GetWorkItemTypesInBehavior(requestContext, processId);
        BehaviorWorkItemTypes behaviorWorkItemTypes;
        if (itemTypesInBehavior.TryGetValue(behavior.ReferenceName, out behaviorWorkItemTypes) && behaviorWorkItemTypes.WorkItemTypes.Any<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wit => TFStringComparer.WorkItemTypeReferenceName.Equals(wit.ReferenceName, witReferenceName))))
          throw new BehaviorReferenceAlreadyExistsException(witReferenceName, behavior.ReferenceName);
        if (itemTypesInBehavior.Any<KeyValuePair<string, BehaviorWorkItemTypes>>((Func<KeyValuePair<string, BehaviorWorkItemTypes>, bool>) (kvp => kvp.Value.WorkItemTypes.Any<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wit => TFStringComparer.WorkItemTypeReferenceName.Equals(wit.ReferenceName, witReferenceName))))))
          throw new CannotReferenceMultipleBehaviorsException(witReferenceName);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, false);
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          component.CreateWorkItemTypeBehaviorReference(processId, typelet.Id, behavior.ReferenceName, id, isDefault);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (witReferenceName), string.Format("[NonEmail: {0}]", (object) witReferenceName));
        properties.Add(nameof (behaviorReferenceName), behaviorReferenceName);
        properties.Add(nameof (processId), (object) processId);
        properties.Add(nameof (isDefault), isDefault);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), nameof (AddBehaviorToWorkItemType), properties);
        this.PublishWorkItemTypeBehaviorChangedEvent(requestContext, processId, WorkItemTypeBehaviorChangeType.AddBehaviorToWorkItemType);
        return this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
      }));
    }

    public ProcessWorkItemType UpdateDefaultWorkItemTypeForBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName,
      bool isDefault)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      return requestContext.TraceBlock<ProcessWorkItemType>(910823, 910824, "WorkItemType", nameof (ProcessWorkItemTypeService), "AddBehaviorToWorkItemType", (Func<ProcessWorkItemType>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        witReferenceName = witReferenceName.Trim();
        behaviorReferenceName = behaviorReferenceName.Trim();
        ComposedWorkItemType workItemType = this.GetWorkItemType(requestContext, processId, witReferenceName, true, false);
        if (!workItemType.IsCustomType && !workItemType.IsDerived)
          throw new CannotReferenceBehaviorFromNonCustomWorkItemTypeException(witReferenceName);
        Behavior behavior = requestContext.GetService<IBehaviorService>().GetBehavior(requestContext, processId, behaviorReferenceName);
        if (behavior.IsAbstract)
          throw new CannotReferenceAbstractBehaviorException(behavior.ReferenceName);
        BehaviorWorkItemTypes behaviorWorkItemTypes;
        if (!this.GetWorkItemTypesInBehavior(requestContext, processId).TryGetValue(behavior.ReferenceName, out behaviorWorkItemTypes) || !behaviorWorkItemTypes.WorkItemTypes.Any<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wit => TFStringComparer.WorkItemTypeReferenceName.Equals(wit.ReferenceName, witReferenceName))))
          throw new BehaviorReferenceDoesNotExistsException(witReferenceName, behavior.ReferenceName);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, false);
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          component.UpdateDefaultWorkItemTypeForBehavior(processId, typelet.Id, behavior.ReferenceName, id, isDefault);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (witReferenceName), witReferenceName);
        properties.Add(nameof (behaviorReferenceName), behaviorReferenceName);
        properties.Add(nameof (isDefault), isDefault);
        properties.Add(nameof (processId), (object) processId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), nameof (UpdateDefaultWorkItemTypeForBehavior), properties);
        this.PublishWorkItemTypeBehaviorChangedEvent(requestContext, processId, WorkItemTypeBehaviorChangeType.UpdateBehaviorDefaultWorkItemType);
        return this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
      }));
    }

    public ProcessWorkItemType RemoveBehaviorFromWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      return requestContext.TraceBlock<ProcessWorkItemType>(910825, 910826, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (RemoveBehaviorFromWorkItemType), (Func<ProcessWorkItemType>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        IReadOnlyDictionary<string, BehaviorWorkItemTypes> itemTypesInBehavior = this.GetWorkItemTypesInBehavior(requestContext, processId);
        witReferenceName = witReferenceName.Trim();
        behaviorReferenceName = behaviorReferenceName.Trim();
        string key = behaviorReferenceName;
        BehaviorWorkItemTypes behaviorWorkItemTypes;
        ref BehaviorWorkItemTypes local = ref behaviorWorkItemTypes;
        if (!itemTypesInBehavior.TryGetValue(key, out local) || !behaviorWorkItemTypes.WorkItemTypes.Any<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (wit => TFStringComparer.WorkItemTypeReferenceName.Equals(wit.ReferenceName, witReferenceName))))
          throw new BehaviorNotReferencedByWorkItemTypeException(witReferenceName, behaviorReferenceName);
        ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, false);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          component.DeleteWorkItemTypeBehaviorReference(processId, typelet.Id, behaviorReferenceName, id);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(nameof (witReferenceName), witReferenceName);
        properties.Add(nameof (behaviorReferenceName), behaviorReferenceName);
        properties.Add(nameof (processId), (object) processId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), nameof (RemoveBehaviorFromWorkItemType), properties);
        this.PublishWorkItemTypeBehaviorChangedEvent(requestContext, processId, WorkItemTypeBehaviorChangeType.RemoveBehaviorFromWorkItemType);
        return this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
      }));
    }

    internal virtual IReadOnlyDictionary<string, IReadOnlyCollection<BehaviorRelation>> GetBehaviorsForWorkItemTypesInternal(
      IVssRequestContext requestContext,
      Guid processId,
      IReadOnlyCollection<ComposedWorkItemType> composedWits,
      bool bypassCache)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<ComposedWorkItemType>>(composedWits, nameof (composedWits));
      return (IReadOnlyDictionary<string, IReadOnlyCollection<BehaviorRelation>>) requestContext.TraceBlock<Dictionary<string, IReadOnlyCollection<BehaviorRelation>>>(910827, 910828, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetBehaviorsForWorkItemTypesInternal), (Func<Dictionary<string, IReadOnlyCollection<BehaviorRelation>>>) (() =>
      {
        Dictionary<string, IReadOnlyCollection<BehaviorRelation>> itemTypesInternal = new Dictionary<string, IReadOnlyCollection<BehaviorRelation>>();
        composedWits = this.GetAllWorkItemTypes(requestContext, processId, bypassCache, false);
        IEnumerable<ComposedWorkItemType> first = composedWits.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (cw => cw.IsCustomType));
        foreach (ComposedWorkItemType composedWorkItemType in first)
          itemTypesInternal[composedWorkItemType.ReferenceName] = composedWorkItemType.CustomWorkItemType.BehaviorRelations;
        Enumerable.Empty<ComposedWorkItemType>();
        IEnumerable<ComposedWorkItemType> second = composedWits.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (cw => cw.IsDerived));
        foreach (ComposedWorkItemType composedWorkItemType in second)
          itemTypesInternal[composedWorkItemType.ReferenceName] = composedWorkItemType.DerivedWorkItemType.BehaviorRelations;
        IEnumerable<ComposedWorkItemType> source1 = composedWits.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (cw => !cw.IsCustomType));
        if (source1.Any<ComposedWorkItemType>())
        {
          ComposedWorkItemType composedWorkItemType = source1.First<ComposedWorkItemType>();
          Guid guid = composedWorkItemType.IsDerived ? composedWorkItemType.ParentProcessId : composedWorkItemType.ProcessId;
          ILegacyWorkItemTrackingProcessService service1 = requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
          IBehaviorService service2 = requestContext.GetService<IBehaviorService>();
          IVssRequestContext requestContext1 = requestContext;
          Guid processTypeId = guid;
          ProcessBacklogs processBacklogs = service1.GetProcessWorkDefinition(requestContext1, processTypeId).ProcessBacklogs;
          List<Behavior> list1 = service2.GetBehaviors(requestContext, processId).ToList<Behavior>();
          List<Behavior> list2 = list1.Where<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ParentBehaviorReferenceName, BehaviorService.PortfolioBehaviorReferenceName))).ToList<Behavior>();
          Behavior requirementBehavior = list1.FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ReferenceName, BehaviorService.RequirementBehaviorReferenceName)));
          Behavior taskBehavior = list1.FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ReferenceName, BehaviorService.TaskBehaviorReferenceName)));
          foreach (ComposedWorkItemType composedWit in source1)
          {
            IReadOnlyCollection<BehaviorRelation> fromLegacyStorage = this.GetBehaviorsFromLegacyStorage(processBacklogs, (IReadOnlyCollection<Behavior>) list2, requirementBehavior, taskBehavior, composedWit);
            if (!itemTypesInternal.ContainsKey(composedWit.ReferenceName) || fromLegacyStorage.Any<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => b.IsDefault)))
              itemTypesInternal[composedWit.ReferenceName] = fromLegacyStorage;
          }
        }
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
        foreach (ComposedWorkItemType composedWorkItemType in first.Concat<ComposedWorkItemType>(second))
        {
          IReadOnlyCollection<BehaviorRelation> source2 = composedWorkItemType.CustomWorkItemType?.BehaviorRelations ?? composedWorkItemType.DerivedWorkItemType?.BehaviorRelations;
          BehaviorRelation behaviorRelation = source2 != null ? source2.Where<BehaviorRelation>((Func<BehaviorRelation, bool>) (customWitBehavior => customWitBehavior.IsDefault)).FirstOrDefault<BehaviorRelation>() : (BehaviorRelation) null;
          if (behaviorRelation != null)
            dictionary[behaviorRelation.Behavior.ReferenceName] = composedWorkItemType.ReferenceName;
        }
        foreach (string key in itemTypesInternal.Keys)
        {
          foreach (BehaviorRelation behaviorRelation in (IEnumerable<BehaviorRelation>) itemTypesInternal[key])
          {
            if (behaviorRelation.IsDefault && dictionary.ContainsKey(behaviorRelation.Behavior.ReferenceName) && !TFStringComparer.WorkItemTypeletReferenceName.Equals(dictionary[behaviorRelation.Behavior.ReferenceName], key))
              behaviorRelation.IsDefault = false;
          }
        }
        return itemTypesInternal;
      }));
    }

    internal RuleChangeDescriptor ValidateFieldPropertiesAndGetRuleChanges(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      ProcessWorkItemType workItemType,
      FieldEntry field,
      WorkItemFieldRule fieldRule)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckForNull<ProcessWorkItemType>(workItemType, nameof (workItemType));
      ArgumentUtility.CheckForNull<FieldEntry>(field, nameof (field));
      ArgumentUtility.CheckForNull<WorkItemFieldRule>(fieldRule, nameof (fieldRule));
      List<Guid> rulesToDisable = new List<Guid>();
      List<Guid> rulesToEnable = new List<Guid>();
      WorkItemFieldRule fieldRule1 = (WorkItemFieldRule) null;
      if (workItemType.IsDerived)
        requestContext.GetService<IWorkItemRulesService>().TryGetOutOfBoxRulesForWorkItemTypeField(requestContext, processDescriptor, workItemType.ReferenceName, field.ReferenceName, out fieldRule1);
      FieldPropertyValidator<DefaultRule> propertyValidator1 = fieldRule.GetAllUnconditionalRules().OfType<ReadOnlyRule>().FirstOrDefault<ReadOnlyRule>() == null ? new FieldPropertyValidator<DefaultRule>(fieldRule, fieldRule1) : throw new ProcessWorkItemTypeValidationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ReadOnlyUnsupportedForField((object) field.ReferenceName));
      FieldPropertyValidator<RequiredRule> requiredRuleFlattener = new FieldPropertyValidator<RequiredRule>(fieldRule, fieldRule1);
      if (field.FieldId == 2)
      {
        propertyValidator1.DeleteCustomFieldProperty();
        requiredRuleFlattener.DeleteCustomFieldProperty();
        propertyValidator1.OverrideCurrentRuleWithOobRule();
        requiredRuleFlattener.OverrideCurrentRuleWithOobRule();
      }
      ComposedWorkItemType workItemType1 = this.GetWorkItemType(requestContext, processDescriptor.TypeId, workItemType.ReferenceName, false, false);
      bool canEditFieldPropertiesWithinScopeOfWorkItemType = this.CanEditFieldPropertiesWithinScopeOfWorkItemType(requestContext, field, workItemType1, fieldRule1);
      bool canEditDefaultValue = canEditFieldPropertiesWithinScopeOfWorkItemType || field.FieldId == 1;
      propertyValidator1.Validate((Func<DefaultRule, DefaultRule, bool>) ((dRule, oobDRule) => TFStringComparer.DefaultValue.Equals(oobDRule.Value, dRule.Value)), (Func<bool>) (() => canEditDefaultValue), (Func<bool>) (() => canEditDefaultValue), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.DefaultValueNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable);
      List<Guid> previousRulesToDisable = new List<Guid>();
      this.LogDisabledRulesForCustomWit(requestContext, workItemType, rulesToDisable, ref previousRulesToDisable, processDescriptor.TypeId, field.ReferenceName, "Validate");
      requiredRuleFlattener.Validate((Func<RequiredRule, RequiredRule, bool>) ((rRule, oobRRule) => true), (Func<bool>) (() =>
      {
        if (canEditFieldPropertiesWithinScopeOfWorkItemType)
          return true;
        if (!workItemType.IsCustomType || field.FieldId != 1)
          return false;
        requiredRuleFlattener.DeleteCustomFieldProperty();
        return true;
      }), (Func<bool>) (() => false), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.RequiredNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable);
      this.LogDisabledRulesForCustomWit(requestContext, workItemType, rulesToDisable, ref previousRulesToDisable, processDescriptor.TypeId, field.ReferenceName, "Validate");
      if (field.IsIdentity)
      {
        FieldPropertyValidator<AllowedValuesRule> propertyValidator2 = new FieldPropertyValidator<AllowedValuesRule>(fieldRule, fieldRule1);
        FieldPropertyValidator<AllowExistingValueRule> propertyValidator3 = new FieldPropertyValidator<AllowExistingValueRule>(fieldRule, fieldRule1);
        bool canEditAllowedValues = canEditFieldPropertiesWithinScopeOfWorkItemType && !propertyValidator2.RuleExistsInParentWIT();
        bool canEditAllowExistingValue = canEditFieldPropertiesWithinScopeOfWorkItemType && !propertyValidator3.RuleExistsInParentWIT();
        propertyValidator2.Validate((Func<AllowedValuesRule, AllowedValuesRule, bool>) ((avRule, oobAvRule) =>
        {
          if (oobAvRule.Sets == null && avRule.Sets == null)
            return true;
          return oobAvRule.Sets != null && avRule.Sets != null && ((IEnumerable<ConstantSetReference>) oobAvRule.Sets).OrderByDescending<ConstantSetReference, int>((Func<ConstantSetReference, int>) (s => s.Id)).SequenceEqual<ConstantSetReference>((IEnumerable<ConstantSetReference>) ((IEnumerable<ConstantSetReference>) avRule.Sets).OrderByDescending<ConstantSetReference, int>((Func<ConstantSetReference, int>) (s => s.Id)));
        }), (Func<bool>) (() => canEditAllowedValues), (Func<bool>) (() => canEditAllowedValues), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.AllowedValuesNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable, true);
        this.LogDisabledRulesForCustomWit(requestContext, workItemType, rulesToDisable, ref previousRulesToDisable, processDescriptor.TypeId, field.ReferenceName, "Validate");
        propertyValidator3.Validate((Func<AllowExistingValueRule, AllowExistingValueRule, bool>) ((aevRule, oobAevRule) => true), (Func<bool>) (() => canEditAllowExistingValue), (Func<bool>) (() => canEditAllowExistingValue), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.AllowExistingValueNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable, true);
        this.LogDisabledRulesForCustomWit(requestContext, workItemType, rulesToDisable, ref previousRulesToDisable, processDescriptor.TypeId, field.ReferenceName, "Validate");
      }
      else
      {
        new FieldPropertyValidator<AllowedValuesRule>(fieldRule, fieldRule1).Validate((Func<AllowedValuesRule, AllowedValuesRule, bool>) ((avRule, oobAvRule) =>
        {
          if (oobAvRule.Values == null && avRule.Values == null)
            return true;
          return oobAvRule.Values != null && avRule.Values != null && oobAvRule.Values.Count == avRule.Values.Count && oobAvRule.Values.Equals((object) avRule.Values);
        }), (Func<bool>) (() => canEditFieldPropertiesWithinScopeOfWorkItemType), (Func<bool>) (() => canEditFieldPropertiesWithinScopeOfWorkItemType), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.AllowedValuesNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable, true);
        new FieldPropertyValidator<SuggestedValuesRule>(fieldRule, fieldRule1).Validate((Func<SuggestedValuesRule, SuggestedValuesRule, bool>) ((svRule, oobSvRule) =>
        {
          if (oobSvRule.Values == null && svRule.Values == null)
            return true;
          return oobSvRule.Values != null && svRule.Values != null && oobSvRule.Values.Count == svRule.Values.Count && oobSvRule.Values.Equals((object) svRule.Values);
        }), (Func<bool>) (() => canEditFieldPropertiesWithinScopeOfWorkItemType), (Func<bool>) (() => canEditFieldPropertiesWithinScopeOfWorkItemType), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.AllowedValuesNotEditableOnField((object) field.ReferenceName), ref rulesToDisable, ref rulesToEnable, true);
        this.LogDisabledRulesForCustomWit(requestContext, workItemType, rulesToDisable, ref previousRulesToDisable, processDescriptor.TypeId, field.ReferenceName, "Validate");
      }
      return new RuleChangeDescriptor((IEnumerable<Guid>) rulesToEnable, (IEnumerable<Guid>) rulesToDisable);
    }

    private void LogDisabledRulesForCustomWit(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit,
      List<Guid> rulesToDisable,
      ref List<Guid> previousRulesToDisable,
      Guid processDescriptorTypeId,
      string fieldEntryReferenceName,
      string methodName)
    {
      if (!wit.IsCustomType)
        return;
      foreach (Guid element in rulesToDisable)
      {
        if (!previousRulesToDisable.Contains(element))
        {
          previousRulesToDisable.Append<Guid>(element);
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("CustomWitName", wit.ReferenceName);
          properties.Add("ProcessTypeId", (object) processDescriptorTypeId);
          properties.Add("FieldReferenceName", fieldEntryReferenceName);
          properties.Add("NewDisabledRuleId:", (object) element);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), methodName, properties);
        }
      }
    }

    public virtual bool CanEditFieldPropertiesWithinScopeOfWorkItemType(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      FieldEntry field,
      ComposedWorkItemType workItemType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckForNull<FieldEntry>(field, nameof (field));
      ArgumentUtility.CheckForNull<ComposedWorkItemType>(workItemType, nameof (workItemType));
      WorkItemFieldRule fieldRule = (WorkItemFieldRule) null;
      if (ProcessWorkItemTypeService.s_oobFieldsBlockedFromCusomization.Contains(field.ReferenceName))
        requestContext.GetService<IWorkItemRulesService>().TryGetOutOfBoxRulesForWorkItemTypeField(requestContext, processDescriptor, workItemType.ReferenceName, field.ReferenceName, out fieldRule);
      return this.CanEditFieldPropertiesWithinScopeOfWorkItemType(requestContext, field, workItemType, fieldRule);
    }

    private bool CanEditFieldPropertiesWithinScopeOfWorkItemType(
      IVssRequestContext requestContext,
      FieldEntry field,
      ComposedWorkItemType workItemType,
      WorkItemFieldRule outOfBoxRule)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FieldEntry>(field, nameof (field));
      ArgumentUtility.CheckForNull<ComposedWorkItemType>(workItemType, nameof (workItemType));
      if (field.FieldId == 52 || field.FieldId == 24)
        return true;
      if (field.IsCore)
        return false;
      if (workItemType.IsCustomType)
        return true;
      if (((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(workItemType.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))
        return false;
      if (!ProcessWorkItemTypeService.s_oobFieldsBlockedFromCusomization.Contains(field.ReferenceName) || outOfBoxRule == null)
        return true;
      return TFStringComparer.WorkItemFieldReferenceName.Equals(field.ReferenceName, "Microsoft.VSTS.CMMI.Blocked") && !outOfBoxRule.SelectRules<EmptyRule>().Any<EmptyRule>();
    }

    internal void CheckAndFormatFieldRuleValueType(
      IVssRequestContext requestContext,
      FieldEntry field,
      WorkItemFieldRule fieldRule)
    {
      if (fieldRule == null)
        return;
      DefaultRule defaultRule = fieldRule.GetUnconditionalRules<DefaultRule>().FirstOrDefault<DefaultRule>();
      if (defaultRule == null)
        return;
      RuleValueValidationResult validationResult = RuleValueValidator.ValidateAndTransformRuleValue(requestContext, field, defaultRule.Value, fieldRule, defaultRule.ValueFrom, defaultRule is IdentityDefaultRule, defaultRule is IdentityDefaultRule identityDefaultRule ? identityDefaultRule.Vsid : Guid.Empty);
      defaultRule.Value = validationResult.ErrorMessage == null ? validationResult.TransformedValue : throw new ProcessWorkItemTypeRuleValidationException(validationResult.ErrorMessage);
    }

    internal void CheckDisableNotBlocked(string parentTypeRefName, string workItemName)
    {
      foreach (string x in Customization.TcmWorkItemTypesBlockedFromDisable)
      {
        if (TFStringComparer.WorkItemTypeReferenceName.Equals(x, parentTypeRefName))
          throw new ProcessWorkItemTypeCannotDisableException(workItemName, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotDisableTestWorkItemTypeReason());
      }
    }

    internal virtual IReadOnlyCollection<BehaviorRelation> GetBehaviorsFromLegacyStorage(
      ProcessBacklogs legacyProcessBacklogs,
      IReadOnlyCollection<Behavior> portfolioBehaviors,
      Behavior requirementBehavior,
      Behavior taskBehavior,
      ComposedWorkItemType composedWit)
    {
      List<BehaviorRelation> source = new List<BehaviorRelation>();
      if (legacyProcessBacklogs.RequirementBacklog.WorkItemTypesInCategory != null && requirementBehavior != null && legacyProcessBacklogs.RequirementBacklog.WorkItemTypesInCategory.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, composedWit.Name))) && !source.Any<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.Behavior.ReferenceName, requirementBehavior.ReferenceName))))
        source.Add(this.GetBehaviorRelation(requirementBehavior, composedWit, legacyProcessBacklogs.RequirementBacklog));
      else if (legacyProcessBacklogs.TaskBacklog.WorkItemTypesInCategory != null && taskBehavior != null && legacyProcessBacklogs.TaskBacklog.WorkItemTypesInCategory.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, composedWit.Name))) && !source.Any<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.Behavior.ReferenceName, taskBehavior.ReferenceName))))
        source.Add(this.GetBehaviorRelation(taskBehavior, composedWit, legacyProcessBacklogs.TaskBacklog));
      foreach (ProcessBacklogDefinition portfolioBacklog1 in (IEnumerable<ProcessBacklogDefinition>) legacyProcessBacklogs.PortfolioBacklogs)
      {
        ProcessBacklogDefinition portfolioBacklog = portfolioBacklog1;
        if (portfolioBacklog.WorkItemTypesInCategory != null && portfolioBacklog.WorkItemTypesInCategory.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, composedWit.Name))))
        {
          Behavior matchingPortfolioBehavior = portfolioBehaviors.FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => b.Rank == portfolioBacklog.Rank));
          if (matchingPortfolioBehavior != null && !source.Any<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.Behavior.ReferenceName, matchingPortfolioBehavior.ReferenceName))))
            source.Add(this.GetBehaviorRelation(matchingPortfolioBehavior, composedWit, portfolioBacklog));
        }
      }
      return (IReadOnlyCollection<BehaviorRelation>) source;
    }

    private static void CheckBooleanFieldsHaveRequiredAndDefaultValue(
      WorkItemFieldRule fieldRule,
      FieldEntry fieldEntry)
    {
      if (fieldEntry.IsCore || fieldEntry.FieldType != InternalFieldType.Boolean)
        return;
      IEnumerable<WorkItemRule> unconditionalRules = fieldRule?.GetAllUnconditionalRules();
      if (unconditionalRules == null || !unconditionalRules.OfType<RequiredRule>().Any<RequiredRule>())
        throw new ProcessWorkItemTypeRuleValidationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BooleanFieldTypeMustBeRequired());
      if (!unconditionalRules.OfType<DefaultRule>().Any<DefaultRule>())
        throw new ProcessWorkItemTypeRuleValidationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BooleanFieldTypeMustHaveDefault());
    }

    private BehaviorRelation GetBehaviorRelation(
      Behavior requirementBehavior,
      ComposedWorkItemType composedWit,
      ProcessBacklogDefinition portfolioBacklog)
    {
      bool flag = TFStringComparer.WorkItemTypeName.Equals(portfolioBacklog.DefaultWorkItemTypeName, composedWit.Name);
      return new BehaviorRelation()
      {
        Behavior = requirementBehavior,
        IsDefault = flag,
        IsLegacyDefault = flag
      };
    }

    private IEnumerable<WorkItemFieldRule> GetStandardFieldRules(IVssRequestContext requestContext)
    {
      List<WorkItemFieldRule> standardFieldRules = new List<WorkItemFieldRule>();
      WorkItemFieldRule workItemFieldRule1 = new WorkItemFieldRule()
      {
        FieldId = -7,
        Field = "System.AreaPath"
      };
      WorkItemFieldRule workItemFieldRule2 = workItemFieldRule1;
      ComputedRule rule1 = new ComputedRule();
      rule1.FieldId = -2;
      workItemFieldRule2.AddRule<ComputedRule>(rule1);
      standardFieldRules.Add(workItemFieldRule1);
      WorkItemFieldRule workItemFieldRule3 = new WorkItemFieldRule()
      {
        FieldId = -2,
        Field = "System.AreaId"
      };
      WorkItemFieldRule workItemFieldRule4 = workItemFieldRule3;
      ComputedRule rule2 = new ComputedRule();
      rule2.FieldId = -7;
      workItemFieldRule4.AddRule<ComputedRule>(rule2);
      workItemFieldRule3.AddRule<RequiredRule>(new RequiredRule());
      standardFieldRules.Add(workItemFieldRule3);
      WorkItemFieldRule workItemFieldRule5 = new WorkItemFieldRule()
      {
        FieldId = -12,
        Field = "System.NodeName"
      };
      WorkItemFieldRule workItemFieldRule6 = workItemFieldRule5;
      ComputedRule rule3 = new ComputedRule();
      rule3.FieldId = -2;
      workItemFieldRule6.AddRule<ComputedRule>(rule3);
      standardFieldRules.Add(workItemFieldRule5);
      WorkItemFieldRule workItemFieldRule7 = new WorkItemFieldRule()
      {
        FieldId = -105,
        Field = "System.IterationPath"
      };
      WorkItemFieldRule workItemFieldRule8 = workItemFieldRule7;
      ComputedRule rule4 = new ComputedRule();
      rule4.FieldId = -104;
      workItemFieldRule8.AddRule<ComputedRule>(rule4);
      standardFieldRules.Add(workItemFieldRule7);
      WorkItemFieldRule workItemFieldRule9 = new WorkItemFieldRule()
      {
        FieldId = -104,
        Field = "System.IterationId"
      };
      WorkItemFieldRule workItemFieldRule10 = workItemFieldRule9;
      ComputedRule rule5 = new ComputedRule();
      rule5.FieldId = -105;
      workItemFieldRule10.AddRule<ComputedRule>(rule5);
      workItemFieldRule9.AddRule<RequiredRule>(new RequiredRule());
      standardFieldRules.Add(workItemFieldRule9);
      if (WorkItemTrackingFeatureFlags.IsInjectExcludeGroupRuleEnabled(requestContext))
      {
        WorkItemFieldRule workItemFieldRule11 = new WorkItemFieldRule()
        {
          FieldId = 24,
          Field = "System.AssignedTo"
        };
        WorkItemFieldRule workItemFieldRule12 = workItemFieldRule11;
        AllowedValuesRule allowedValuesRule1 = new AllowedValuesRule();
        AllowedValuesRule allowedValuesRule2 = allowedValuesRule1;
        ConstantSetReference[] constantSetReferenceArray = new ConstantSetReference[1];
        ExtendedConstantSetRef extendedConstantSetRef = new ExtendedConstantSetRef();
        extendedConstantSetRef.Id = -2;
        extendedConstantSetRef.ExcludeGroups = true;
        constantSetReferenceArray[0] = (ConstantSetReference) extendedConstantSetRef;
        allowedValuesRule2.Sets = constantSetReferenceArray;
        AllowedValuesRule rule6 = allowedValuesRule1;
        workItemFieldRule12.AddRule<AllowedValuesRule>(rule6);
        workItemFieldRule11.AddRule<AllowExistingValueRule>(new AllowExistingValueRule());
        standardFieldRules.Add(workItemFieldRule11);
      }
      return (IEnumerable<WorkItemFieldRule>) standardFieldRules;
    }

    private ComposedWorkItemType GetWorkItemTypeInternal(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool bypassCache)
    {
      return new ProcessWorkItemTypeService.ComposedProcess(requestContext, processId, bypassCache).GetWorkItemType(requestContext, witReferenceName);
    }

    private IEnumerable<string> MergeFields(
      ProcessWorkItemType existingWit,
      string addedFieldReferenceName,
      string removedFieldRefName)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (addedFieldReferenceName != null)
        dictionary.Add(addedFieldReferenceName, addedFieldReferenceName);
      if (existingWit.Fields != null)
      {
        foreach (WorkItemTypeExtensionFieldEntry field in existingWit.Fields)
        {
          if (!dictionary.ContainsKey(field.Field.ReferenceName))
            dictionary.Add(field.Field.ReferenceName, field.Field.ReferenceName);
        }
      }
      if (removedFieldRefName != null)
        dictionary.Remove(removedFieldRefName);
      return (IEnumerable<string>) dictionary.Values;
    }

    private ICollection<WorkItemFieldRule> MergeFieldRules(
      IEnumerable<WorkItemFieldRule> existingWitRules,
      WorkItemFieldRule addFieldRule,
      string removeRulesOnField,
      int fieldIdToRemove = -1)
    {
      Dictionary<string, WorkItemFieldRule> dictionary = new Dictionary<string, WorkItemFieldRule>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (addFieldRule != null)
        dictionary.Add(addFieldRule.Field, addFieldRule);
      if (existingWitRules != null && existingWitRules.Any<WorkItemFieldRule>())
      {
        foreach (WorkItemFieldRule existingWitRule in existingWitRules)
        {
          if (!dictionary.ContainsKey(existingWitRule.Field))
            dictionary.Add(existingWitRule.Field, existingWitRule);
        }
      }
      if (removeRulesOnField == null)
        return (ICollection<WorkItemFieldRule>) dictionary.Values;
      dictionary.Remove(removeRulesOnField);
      return (ICollection<WorkItemFieldRule>) this.RemoveCopyRulesForField((IEnumerable<WorkItemFieldRule>) dictionary.Values, fieldIdToRemove).ToList<WorkItemFieldRule>();
    }

    public IEnumerable<WorkItemFieldRule> RemoveCopyRulesForField(
      IEnumerable<WorkItemFieldRule> witRules,
      int removeRulesOnField)
    {
      return witRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => !this.DoesRuleContainsField((WorkItemRule) r, removeRulesOnField)));
    }

    private bool DoesRuleContainsField(WorkItemRule rule, int removeRulesOnField)
    {
      if (rule is CopyRule)
      {
        CopyRule copyRule = rule as CopyRule;
        if (copyRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue && copyRule.Value == removeRulesOnField.ToString())
          return true;
      }
      return rule is RuleBlock && ((IEnumerable<WorkItemRule>) (rule as RuleBlock).SubRules).Any<WorkItemRule>((Func<WorkItemRule, bool>) (_ => this.DoesRuleContainsField(_, removeRulesOnField)));
    }

    private IReadOnlyCollection<Guid> GetMergedDisabledRules(
      ProcessWorkItemType workItemType,
      RuleChangeDescriptor ruleChanges)
    {
      if (ruleChanges == null)
      {
        List<Guid> guidList;
        if (workItemType == null)
        {
          guidList = (List<Guid>) null;
        }
        else
        {
          IEnumerable<Guid> disabledRules = workItemType.DisabledRules;
          guidList = disabledRules != null ? disabledRules.ToList<Guid>() : (List<Guid>) null;
        }
        return (IReadOnlyCollection<Guid>) guidList ?? (IReadOnlyCollection<Guid>) new List<Guid>();
      }
      List<Guid> source = new List<Guid>();
      if (workItemType?.DisabledRules != null)
        source.AddRange(workItemType.DisabledRules);
      source.AddRange((IEnumerable<Guid>) ruleChanges.RulesToDisable);
      return (IReadOnlyCollection<Guid>) source.Where<Guid>((Func<Guid, bool>) (r => r != Guid.Empty && !ruleChanges.RulesToEnable.Contains(r))).Distinct<Guid>().ToList<Guid>();
    }

    private void CheckFieldCountLimit(
      IVssRequestContext requestContext,
      string witReferenceName,
      int mergedFieldCount)
    {
      int fieldsPerWorkItemType = requestContext.WitContext().TemplateValidatorConfiguration.MaxCustomFieldsPerWorkItemType;
      if (mergedFieldCount > fieldsPerWorkItemType)
        throw new ProcessWorkItemTypeFieldLimitExceededException(fieldsPerWorkItemType);
    }

    private void CheckWorkItemTypesCountLimit(
      IVssRequestContext requestContext,
      string processName,
      int count)
    {
      int itemTypesPerProcess = requestContext.WitContext().TemplateValidatorConfiguration.MaxCustomWorkItemTypesPerProcess;
      if (count > itemTypesPerProcess)
        throw new ProcessWorkItemTypesLimitExceededException(itemTypesPerProcess);
    }

    private void CheckWorkItemTypeDoesNotAlreadyExists(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string name,
      ref string witReferenceName)
    {
      if (this.GetAllWorkItemTypes(requestContext, processDescriptor.TypeId, true, false).Any<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeName.Equals(t.Name, name))))
        throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessWorkItemTypeNameAlreadyInUse((object) name));
      if (!this.IsTypeletReferenceNameInUse(requestContext, processDescriptor.TypeId, witReferenceName, false))
        return;
      witReferenceName = CommonWITUtils.GenerateUniqueRefName();
    }

    private void PublishWorkItemTypeChangedEvent(
      IVssRequestContext requestContext,
      Guid processId,
      WorkItemTypeChangeType changeType)
    {
      try
      {
        WorkItemTypeChangedEvent notificationEvent = new WorkItemTypeChangedEvent()
        {
          ProcessId = processId,
          ChangeType = changeType
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        requestContext.Trace(910831, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), "WorkItemTypeChangedEvent fired for ProcessId : {0}", (object) processId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910832, "WorkItemType", nameof (ProcessWorkItemTypeService), ex);
      }
    }

    private void PublishWorkItemTypeBehaviorChangedEvent(
      IVssRequestContext requestContext,
      Guid processId,
      WorkItemTypeBehaviorChangeType changeType)
    {
      try
      {
        WorkItemTypeBehaviorChangedEvent notificationEvent = new WorkItemTypeBehaviorChangedEvent()
        {
          ProcessId = processId,
          ChangeType = changeType
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        requestContext.Trace(910833, TraceLevel.Info, "WorkItemType", nameof (ProcessWorkItemTypeService), "WorkItemTypeBehaviorChangedEvent fired for ProcessId : {0}", (object) processId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910834, "WorkItemType", nameof (ProcessWorkItemTypeService), ex);
      }
    }

    private static WorkItemFieldRule UpdateFieldRuleWithFieldProperties(
      IVssRequestContext requestContext,
      WorkItemFieldRule existingFieldRule,
      WorkItemTypeletFieldRuleProperties fieldRuleProperties)
    {
      IEnumerable<WorkItemRule> fromFieldProperties = ProcessWorkItemTypeService.CreateWorkItemRulesFromFieldProperties(requestContext, existingFieldRule.Field, fieldRuleProperties);
      IEnumerable<WorkItemRule> withoutFieldProperties = ProcessWorkItemTypeService.ExtractRulesWithoutFieldProperties(existingFieldRule);
      existingFieldRule.SubRules = fromFieldProperties.Concat<WorkItemRule>(withoutFieldProperties).ToArray<WorkItemRule>();
      return existingFieldRule;
    }

    public virtual T GetTypelet<T>(
      IVssRequestContext requestContext,
      Guid processId,
      string typeletRefName,
      bool bypassCache = false)
      where T : ProcessTypelet
    {
      return this.GetTypelets<T>(requestContext, processId, bypassCache).FirstOrDefault<T>((Func<T, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, typeletRefName))) ?? throw new ProcessWorkItemTypeDoesNotExistException(typeletRefName, processId.ToString());
    }

    public virtual bool TryGetTypelet<T>(
      IVssRequestContext requestContext,
      Guid processId,
      string typeletRefName,
      bool bypassCache,
      out T processTypelet)
      where T : ProcessTypelet
    {
      try
      {
        processTypelet = this.GetTypelet<T>(requestContext, processId, typeletRefName, bypassCache);
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
        processTypelet = default (T);
        return false;
      }
      return true;
    }

    public virtual IReadOnlyCollection<T> GetTypelets<T>(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false)
      where T : ProcessTypelet
    {
      return (IReadOnlyCollection<T>) requestContext.GetService<ProcessTypeletCacheService>().GetProcessTypelets(requestContext, processId, bypassCache).OfType<T>().ToList<T>();
    }

    public virtual bool IsTypeletReferenceNameInUse(
      IVssRequestContext requestContext,
      Guid processId,
      string typeletRefName,
      bool bypassCache = true)
    {
      if (requestContext.GetService<ProcessTypeletCacheService>().GetProcessTypelets(requestContext, processId, bypassCache).Any<ProcessTypelet>((Func<ProcessTypelet, bool>) (typelet => TFStringComparer.WorkItemTypeletReferenceName.Equals(typeletRefName, typelet.ReferenceName))))
        return true;
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
      if (!processDescriptor.IsDerived)
        return false;
      return this.GetWorkItemTypes(requestContext, processDescriptor.Inherits, true, false).Any<BaseWorkItemType>((Func<BaseWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeletReferenceName.Equals(typeletRefName, t.ReferenceName))) || this.IsTypeletReferenceNameInUse(requestContext, processDescriptor.Inherits, typeletRefName, bypassCache);
    }

    private IReadOnlyCollection<ProcessWorkItemType> GetTypeletsInheritingFromParent(
      IVssRequestContext requestContext,
      string parentTypeletRefName,
      IReadOnlyCollection<ProcessWorkItemType> types)
    {
      return (IReadOnlyCollection<ProcessWorkItemType>) types.OfType<ProcessWorkItemType>().Where<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ParentTypeRefName, parentTypeletRefName))).ToList<ProcessWorkItemType>();
    }

    internal ProcessWorkItemType CreateProcessWorkItemTypeInternal(
      IVssRequestContext requestContext,
      Guid processId,
      string name,
      string referenceName,
      string parentType,
      string description,
      IEnumerable<string> fieldReferenceNames,
      IEnumerable<WorkItemFieldRule> fieldRules,
      IEnumerable<Guid> disabledRules,
      string form,
      string color = null,
      string icon = null,
      IReadOnlyCollection<WorkItemStateDeclaration> states = null,
      bool? isDisabled = null)
    {
      Guid extensionId = Guid.NewGuid();
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      WorkItemFieldRule[] fieldRules1 = fieldRules != null ? fieldRules.ToArray<WorkItemFieldRule>() : Array.Empty<WorkItemFieldRule>();
      Guid[] array1 = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) ? (disabledRules != null ? disabledRules.ToArray<Guid>() : (Guid[]) null) : (Guid[]) null;
      WorkItemTrackingFieldService fieldDict = requestContext.GetService<WorkItemTrackingFieldService>();
      int[] array2 = fieldReferenceNames.Select<string, int>((Func<string, int>) (n => fieldDict.GetField(requestContext, n).FieldId)).ToArray<int>();
      WorkItemTypeletRecord workItemTypelet;
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        workItemTypelet = component.CreateWorkItemTypelet(extensionId, processId, name, referenceName, description, parentType, (IList<int>) array2, (IList<WorkItemFieldRule>) fieldRules1, (IList<Guid>) array1, form, id, color, icon, states, isDisabled);
      if (states != null && states.Any<WorkItemStateDeclaration>())
        requestContext.GetService<WorkItemStateDefinitionService>().EnsureBackcompatConstantsForStates(requestContext, referenceName, states.Select<WorkItemStateDeclaration, string>((Func<WorkItemStateDeclaration, string>) (s => s.Name)));
      return ProcessWorkItemType.Create(requestContext, workItemTypelet, fieldDict);
    }

    internal virtual void DestroyProcessWorkItemTypeInternal(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName)
    {
      Guid id1 = requestContext.WitContext().RequestIdentity.Id;
      ProcessWorkItemType typelet = this.GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
      Guid id2 = typelet.Id;
      if (typelet.IsDeleted)
        throw new ProcessWorkItemTypeDoesNotExistException(witReferenceName, processId.ToString());
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        component.DestroyWorkItemTypelets(processId, id1, id2);
    }

    internal virtual ProcessWorkItemType UpdateProcessWorkItemTypeInternal(
      IVssRequestContext requestContext,
      Guid processId,
      ProcessWorkItemType originalWit,
      string description,
      IEnumerable<string> fieldReferenceNames,
      IEnumerable<WorkItemFieldRule> fieldRules,
      IEnumerable<Guid> disabledRules,
      string form,
      string color = null,
      string icon = null,
      bool? isDisabled = null)
    {
      if (string.IsNullOrEmpty(description) && fieldReferenceNames == null && fieldRules == null && string.IsNullOrEmpty(form) && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(icon) && !isDisabled.HasValue)
        throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTrackingNothingToUpdate());
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      WorkItemTrackingFieldService fieldDict = requestContext.GetService<WorkItemTrackingFieldService>();
      WorkItemFieldRule[] array1 = fieldRules != null ? fieldRules.ToArray<WorkItemFieldRule>() : (WorkItemFieldRule[]) null;
      int[] array2 = fieldReferenceNames != null ? fieldReferenceNames.Select<string, int>((Func<string, int>) (f => fieldDict.GetField(requestContext, f).FieldId)).ToArray<int>() : (int[]) null;
      Guid[] array3 = disabledRules != null ? disabledRules.Distinct<Guid>().ToArray<Guid>() : (Guid[]) null;
      this.EnsureFieldValueConstantsForBackcompat(requestContext);
      WorkItemTypeletRecord record = (WorkItemTypeletRecord) null;
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        record = component.UpdateWorkItemTypelet(originalWit.Id, processId, description, (IList<int>) array2, (IList<WorkItemFieldRule>) array1, (IList<Guid>) array3, form, id, originalWit.LastChangedDate, color, icon, isDisabled);
      ProcessWorkItemType processWorkItemType = ProcessWorkItemType.Create(requestContext, record, fieldDict);
      this.HandleAnyDisabledRuleChanges(requestContext, originalWit.DisabledRules, processWorkItemType.DisabledRules);
      return processWorkItemType;
    }

    private void HandleAnyDisabledRuleChanges(
      IVssRequestContext requestContext,
      IEnumerable<Guid> oldDisabledRules,
      IEnumerable<Guid> newDisabledRules)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(oldDisabledRules, nameof (oldDisabledRules));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(newDisabledRules, nameof (newDisabledRules));
      if (oldDisabledRules.Count<Guid>() == newDisabledRules.Count<Guid>() && new HashSet<Guid>(oldDisabledRules).SetEquals(newDisabledRules))
        return;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.StampDb();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessWorkItemTypeService), nameof (HandleAnyDisabledRuleChanges), "StampDB", true);
    }

    private void EnsureFieldValueConstantsForBackcompat(IVssRequestContext requestContext)
    {
      HashSet<string> stringSet = new HashSet<string>(OOBFieldValues.GetAllowedValuesConstants(requestContext).Union<string>((IEnumerable<string>) OOBFieldValues.GetSuggestedValuesConstants(requestContext)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetNonIdentityConstants(requestContext, (IEnumerable<string>) stringSet).Count<ConstantRecord>() == stringSet.Count)
        return;
      using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
        component.EnsureConstantsForBackcompat((IList<string>) stringSet.ToList<string>());
    }

    private class ComposedProcess
    {
      private Guid m_processId;
      private ProcessWorkItemTypeService.ComposedProcess.ProcessWorkItemTypeReference m_thisProcess;
      private ProcessWorkItemTypeService.ComposedProcess.ProcessWorkItemTypeReference m_parentProcess;

      public ComposedProcess(IVssRequestContext requestContext, Guid processId, bool bypassCache)
      {
        this.m_processId = processId;
        this.ComposeProcess(requestContext, bypassCache);
      }

      public IReadOnlyCollection<ComposedWorkItemType> GetWorkItemTypes(
        IVssRequestContext requestContext)
      {
        return this.GetWorkItemTypesInternal(requestContext);
      }

      public ComposedWorkItemType GetWorkItemType(
        IVssRequestContext requestContext,
        string witReferenceName)
      {
        return this.GetWorkItemTypesInternal(requestContext).FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, witReferenceName) || TFStringComparer.WorkItemTypeReferenceName.Equals(t.ParentTypeRefName, witReferenceName)));
      }

      private IReadOnlyCollection<ComposedWorkItemType> GetWorkItemTypesInternal(
        IVssRequestContext requestContext)
      {
        return (IReadOnlyCollection<ComposedWorkItemType>) requestContext.TraceBlock<List<ComposedWorkItemType>>(909907, 909908, "WorkItemType", nameof (ProcessWorkItemTypeService), "ComposedProcess.GetWorkItemTypesInternal", (Func<List<ComposedWorkItemType>>) (() =>
        {
          ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
          if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermission(this.m_processId, out processReadSecuredObject))
            throw new ProcessWorkItemTypeNotFoundException(this.m_processId);
          List<ComposedWorkItemType> itemTypesInternal = new List<ComposedWorkItemType>();
          if (this.m_thisProcess.IsLegacy)
          {
            itemTypesInternal = this.m_thisProcess.LegacyProcessWorkItemTypes.Select<ProcessWorkItemTypeDefinition, ComposedWorkItemType>((Func<ProcessWorkItemTypeDefinition, ComposedWorkItemType>) (t => ComposedWorkItemType.Create(requestContext, t))).ToList<ComposedWorkItemType>();
          }
          else
          {
            List<string> derivedParentTypes = new List<string>();
            if (this.m_thisProcess.ProcessWorkItemTypes != null)
            {
              foreach (ProcessWorkItemType processWorkItemType in (IEnumerable<ProcessWorkItemType>) this.m_thisProcess.ProcessWorkItemTypes)
              {
                ProcessWorkItemType wit = processWorkItemType;
                ProcessWorkItemTypeDefinition parentworkItemType = this.m_parentProcess.LegacyProcessWorkItemTypes.FirstOrDefault<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (t => t.ReferenceName == wit.ParentTypeRefName));
                if (parentworkItemType != null)
                  derivedParentTypes.Add(parentworkItemType.ReferenceName);
                itemTypesInternal.Add(ComposedWorkItemType.Create(requestContext, wit, parentworkItemType));
              }
            }
            List<ComposedWorkItemType> list = this.m_parentProcess.LegacyProcessWorkItemTypes.Where<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (wit => !derivedParentTypes.Contains<string>(wit.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))).Select<ProcessWorkItemTypeDefinition, ComposedWorkItemType>((Func<ProcessWorkItemTypeDefinition, ComposedWorkItemType>) (t => ComposedWorkItemType.Create(requestContext, t))).ToList<ComposedWorkItemType>();
            itemTypesInternal.AddRange((IEnumerable<ComposedWorkItemType>) list);
          }
          itemTypesInternal.ForEach((Action<ComposedWorkItemType>) (t => t.SetSecuredObjectProperties(processReadSecuredObject)));
          return itemTypesInternal;
        }));
      }

      private void ComposeProcess(IVssRequestContext requestContext, bool bypassCache) => requestContext.TraceBlock(909906, 909907, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (ComposeProcess), (Action) (() =>
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, this.m_processId);
        this.m_thisProcess = this.GetProcessWorkItemTypeReference(requestContext, processDescriptor, bypassCache);
        if (!processDescriptor.IsDerived)
          return;
        this.m_parentProcess = this.GetProcessWorkItemTypeReference(requestContext, service.GetProcessDescriptor(requestContext, processDescriptor.Inherits), bypassCache);
      }));

      private ProcessWorkItemTypeService.ComposedProcess.ProcessWorkItemTypeReference GetProcessWorkItemTypeReference(
        IVssRequestContext requestContext,
        ProcessDescriptor process,
        bool bypassCache)
      {
        if (process.IsDerived)
          return new ProcessWorkItemTypeService.ComposedProcess.ProcessWorkItemTypeReference()
          {
            ProcessId = this.m_processId,
            ProcessWorkItemTypes = this.GetDataFromNewStorage(requestContext, process.TypeId, bypassCache)
          };
        return new ProcessWorkItemTypeService.ComposedProcess.ProcessWorkItemTypeReference()
        {
          ProcessId = this.m_processId,
          LegacyProcessWorkItemTypes = this.GetDataFromOldStorage(requestContext, process)
        };
      }

      private IReadOnlyCollection<ProcessWorkItemTypeDefinition> GetDataFromOldStorage(
        IVssRequestContext requestContext,
        ProcessDescriptor processDescriptor)
      {
        return requestContext.TraceBlock<IReadOnlyCollection<ProcessWorkItemTypeDefinition>>(909900, 909901, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetDataFromOldStorage), (Func<IReadOnlyCollection<ProcessWorkItemTypeDefinition>>) (() => requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processDescriptor).WorkItemTypeDefinitions));
      }

      private IReadOnlyCollection<ProcessWorkItemType> GetDataFromNewStorage(
        IVssRequestContext requestContext,
        Guid processId,
        bool bypassCache)
      {
        return requestContext.TraceBlock<IReadOnlyCollection<ProcessWorkItemType>>(909902, 909903, "WorkItemType", nameof (ProcessWorkItemTypeService), nameof (GetDataFromNewStorage), (Func<IReadOnlyCollection<ProcessWorkItemType>>) (() => requestContext.GetService<IProcessWorkItemTypeService>().GetTypelets<ProcessWorkItemType>(requestContext, processId, bypassCache)));
      }

      private class ProcessWorkItemTypeReference
      {
        public Guid ProcessId { get; set; }

        public IReadOnlyCollection<ProcessWorkItemType> ProcessWorkItemTypes { get; set; }

        public IReadOnlyCollection<ProcessWorkItemTypeDefinition> LegacyProcessWorkItemTypes { get; set; }

        public bool IsLegacy => this.LegacyProcessWorkItemTypes != null;
      }
    }
  }
}
