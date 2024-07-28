// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemRulesService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  public class WorkItemRulesService : IWorkItemRulesService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual bool TryGetOutOfBoxRulesAndHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<WorkItemFieldRule> rules,
      out IReadOnlyCollection<HelpTextDescriptor> helpTexts)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      rules = (IReadOnlyCollection<WorkItemFieldRule>) null;
      helpTexts = (IReadOnlyCollection<HelpTextDescriptor>) null;
      ProcessDescriptor processDescriptor1 = processDescriptor;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Original WorkItemType ReferenceName", string.Format("[NonEmail: {0}]", (object) workItemTypeReferenceName));
      properties.Add("Original Process Type Id", (object) processDescriptor1.TypeId);
      properties.Add("Original Process Id", (object) processDescriptor1.RowId);
      properties.Add("Original Process Name", processDescriptor1.Name);
      properties.Add("Original Process ReferenceName", processDescriptor1.ReferenceName);
      properties.Add("Original Process IsSystem", processDescriptor1.IsSystem);
      properties.Add("Original Process IsDerived", processDescriptor1.IsDerived);
      properties.Add("Original Process Inherits", (object) processDescriptor1.Inherits);
      try
      {
        if (!processDescriptor.IsSystem)
        {
          if (processDescriptor.IsDerived)
          {
            Guid inherits = processDescriptor.Inherits;
            if (!this.IsOOBProcessWorkItemType(requestContext, inherits, workItemTypeReferenceName))
            {
              ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processDescriptor1.TypeId, workItemTypeReferenceName);
              properties.Add("WorkItemType IsDerived", workItemType.IsDerived);
              properties.Add("WorkItemType IsCustomType", workItemType.IsCustomType);
              properties.Add("WorkItemType ParentTypeRefName", workItemType.ParentTypeRefName);
              if (workItemType.IsDerived)
                workItemTypeReferenceName = workItemType.ParentTypeRefName;
              else if (workItemType.IsCustomType)
              {
                service.Publish(requestContext, nameof (WorkItemRulesService), "TryGetOutOfBoxRulesAndHelpTexts_CustomType", properties);
                return false;
              }
            }
            processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, inherits);
          }
          else
          {
            service.Publish(requestContext, nameof (WorkItemRulesService), "TryGetOutOfBoxRulesAndHelpTexts_CustomProcess", properties);
            return false;
          }
        }
        properties.Add("Final WorkItemType ReferenceName", workItemTypeReferenceName);
        properties.Add("Final Process Type Id", (object) processDescriptor.TypeId);
        properties.Add("Final Process Id", (object) processDescriptor.RowId);
        properties.Add("Final Process Name", processDescriptor.Name);
        properties.Add("Final Process ReferenceName", processDescriptor.ReferenceName);
        properties.Add("Final Process IsSystem", processDescriptor.IsSystem);
        properties.Add("Final Process IsDerived", processDescriptor.IsDerived);
        properties.Add("Final Process Inherits", (object) processDescriptor.Inherits);
        if (this.IsOOBProcessWorkItemType(requestContext, processDescriptor.TypeId, workItemTypeReferenceName))
        {
          bool outOfBoxRules = requestContext.GetService<WorkItemTrackingOutOfBoxRulesCache>().TryGetOutOfBoxRules(requestContext, processDescriptor, workItemTypeReferenceName, out rules);
          bool outOfBoxHelpTexts = requestContext.GetService<WorkItemTrackingOutOfBoxHelpTextCache>().TryGetOutOfBoxHelpTexts(requestContext, processDescriptor, workItemTypeReferenceName, out helpTexts);
          int num = outOfBoxHelpTexts & outOfBoxRules ? 1 : 0;
          if (num == 0)
          {
            properties.Add("wereOobRulesObtained", outOfBoxRules);
            properties.Add("wereOobHelpTextsObtained", outOfBoxHelpTexts);
            service.Publish(requestContext, nameof (WorkItemRulesService), "TryGetOutOfBoxRulesAndHelpTexts_Unsuccessful", properties);
          }
          return num != 0;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(911322, "Services", nameof (WorkItemRulesService), ex);
        service.Publish(requestContext, nameof (WorkItemRulesService), "TryGetOutOfBoxRulesAndHelpTexts_Exception", properties);
        return false;
      }
      service.Publish(requestContext, nameof (WorkItemRulesService), "TryGetOutOfBoxRulesAndHelpTexts_IsNotOOB", properties);
      return false;
    }

    public bool TryGetOutOfBoxRules(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<WorkItemFieldRule> rules)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      IReadOnlyCollection<HelpTextDescriptor> helpTexts = (IReadOnlyCollection<HelpTextDescriptor>) null;
      return this.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, workItemTypeReferenceName, out rules, out helpTexts);
    }

    public WorkItemRulesAndHelpTextsDescriptor GetOutOfBoxRulesAndHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      IReadOnlyCollection<WorkItemFieldRule> rules;
      IReadOnlyCollection<HelpTextDescriptor> helpTexts;
      if (!this.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, workItemTypeReferenceName, out rules, out helpTexts))
        throw new RulesNotFoundForWorkItemTypeException(workItemTypeReferenceName, processDescriptor.Name);
      return new WorkItemRulesAndHelpTextsDescriptor(rules, helpTexts);
    }

    public IReadOnlyCollection<WorkItemFieldRule> GetOutOfBoxRules(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName)
    {
      return this.GetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, workItemTypeReferenceName).Rules;
    }

    public bool TryGetOutOfBoxRulesForWorkItemTypeField(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      string workItemTypeReferenceName,
      string fieldReferenceName,
      out WorkItemFieldRule fieldRule)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(processDescriptor, nameof (processDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fieldReferenceName, nameof (fieldReferenceName));
      fieldRule = (WorkItemFieldRule) null;
      IReadOnlyCollection<WorkItemFieldRule> rules;
      if (this.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, workItemTypeReferenceName, out rules, out IReadOnlyCollection<HelpTextDescriptor> _))
        fieldRule = rules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => TFStringComparer.WorkItemFieldReferenceName.Equals(r.Field, fieldReferenceName)));
      return fieldRule != null;
    }

    public static IEnumerable<WorkItemFieldRule> PrepareRules(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      fieldRules = fieldRules.Select<WorkItemFieldRule, WorkItemFieldRule>((Func<WorkItemFieldRule, WorkItemFieldRule>) (val => val.Clone() as WorkItemFieldRule));
      IEnumerable<WorkItemFieldRule> workItemFieldRules = WorkItemRulesService.ResolveFields(requestContext, fieldRules);
      foreach (WorkItemRule workItemRule in workItemFieldRules)
        workItemRule.SetAccessModeToReadOnly();
      return workItemFieldRules;
    }

    internal static IEnumerable<WorkItemFieldRule> ResolveFields(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      IList<WorkItemFieldRule> list = (IList<WorkItemFieldRule>) fieldRules.ToList<WorkItemFieldRule>();
      foreach (WorkItemFieldRule workItemFieldRule in (IEnumerable<WorkItemFieldRule>) list)
      {
        FieldEntry field;
        if (fieldDictionary.TryGetField(workItemFieldRule.Field, out field))
          workItemFieldRule.FieldId = field.FieldId;
        foreach (WorkItemRule subRule in workItemFieldRule.SubRules)
          WorkItemRulesService.PopulateFieldId(requestContext, subRule);
      }
      return (IEnumerable<WorkItemFieldRule>) list;
    }

    internal static WorkItemRule[] UpdateRules(
      IVssRequestContext requestContext,
      int fieldId,
      WorkItemRule[] subRules)
    {
      List<WorkItemRule> workItemRuleList = new List<WorkItemRule>(subRules.Length);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(nameof (fieldId), (double) fieldId);
      foreach (WorkItemRule subRule in subRules)
      {
        FieldEntry field;
        if (subRule is AllowedValuesRule allowedValuesRule && requestContext.WitContext().FieldDictionary.TryGetField(fieldId, out field))
        {
          if (allowedValuesRule.Sets != null && ((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).Any<ConstantSetReference>((Func<ConstantSetReference, bool>) (s => s.Id == -2)) && !field.IsIdentity)
          {
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (UpdateRules), typeof (WorkItemRulesService).Name, properties);
            continue;
          }
          if (allowedValuesRule.Values != null)
            allowedValuesRule.Values = new HashSet<string>((IEnumerable<string>) OOBFieldValues.AddCustomValues(requestContext, field.ReferenceName, (IReadOnlyCollection<string>) allowedValuesRule.Values));
        }
        if (subRule is RuleBlock ruleBlock)
          ruleBlock.SubRules = WorkItemRulesService.UpdateRules(requestContext, fieldId, ruleBlock.SubRules);
        workItemRuleList.Add(subRule);
      }
      return workItemRuleList.ToArray();
    }

    public static IEnumerable<HelpTextDescriptor> ResolveHelpTexts(
      IVssRequestContext requestContext,
      IEnumerable<HelpTextDescriptor> helpTextDescriptors)
    {
      helpTextDescriptors = helpTextDescriptors.Select<HelpTextDescriptor, HelpTextDescriptor>((Func<HelpTextDescriptor, HelpTextDescriptor>) (val => val.Clone()));
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      IList<HelpTextDescriptor> list = (IList<HelpTextDescriptor>) helpTextDescriptors.ToList<HelpTextDescriptor>();
      foreach (HelpTextDescriptor helpTextDescriptor in (IEnumerable<HelpTextDescriptor>) list)
      {
        FieldEntry field;
        if (fieldDictionary.TryGetField(helpTextDescriptor.FieldReferenceName, out field))
          helpTextDescriptor.FieldId = field.FieldId;
      }
      return (IEnumerable<HelpTextDescriptor>) list;
    }

    private bool IsOOBProcessWorkItemType(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string workItemTypeReferenceName)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(nameof (processTypeId), (object) processTypeId);
      properties.Add(nameof (workItemTypeReferenceName), string.Format("[NonEmail: {0}]", (object) workItemTypeReferenceName));
      List<string> stringList = (List<string>) null;
      if (ProcessConstants.OobProcessWorkItemTypeMap.TryGetValue(processTypeId, out stringList))
      {
        if (stringList != null)
        {
          if (stringList.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeReferenceName.Equals(workItemTypeReferenceName, n))))
            return true;
          properties.Add("processWorkItemTypes", string.Join(", ", (IEnumerable<string>) stringList));
        }
        properties.Add("processWorkItemTypes", "null");
      }
      else
        properties.Add("processWorkItemTypes", "not found");
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemRulesService), nameof (IsOOBProcessWorkItemType), properties);
      return false;
    }

    private static void PopulateFieldId(IVssRequestContext requestContext, WorkItemRule rule)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      if (rule is WorkItemFieldRule)
      {
        WorkItemFieldRule workItemFieldRule = rule as WorkItemFieldRule;
        FieldEntry field;
        if (!string.IsNullOrWhiteSpace(workItemFieldRule.Field) && fieldDictionary.TryGetField(workItemFieldRule.Field, out field))
          workItemFieldRule.FieldId = field.FieldId;
      }
      else if (rule is DependentRule)
      {
        DependentRule dependentRule = rule as DependentRule;
        FieldEntry field;
        if (!string.IsNullOrWhiteSpace(dependentRule.Field) && fieldDictionary.TryGetField(dependentRule.Field, out field))
          dependentRule.FieldId = field.FieldId;
      }
      else if (rule is ConditionalBlockRule)
      {
        ConditionalBlockRule conditionalBlockRule = rule as ConditionalBlockRule;
        FieldEntry field;
        if (!string.IsNullOrWhiteSpace(conditionalBlockRule.Field) && fieldDictionary.TryGetField(conditionalBlockRule.Field, out field))
          conditionalBlockRule.FieldId = field.FieldId;
      }
      else if (rule is CopyRule)
      {
        CopyRule copyRule = rule as CopyRule;
        FieldEntry field;
        if ((copyRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue || copyRule.ValueFrom == RuleValueFrom.OtherFieldOriginalValue) && !string.IsNullOrWhiteSpace(copyRule.Value) && !int.TryParse(copyRule.Value, out int _) && fieldDictionary.TryGetField(copyRule.Value, out field))
          copyRule.Value = field.FieldId.ToString();
      }
      if (!(rule is RuleBlock ruleBlock))
        return;
      foreach (WorkItemRule subRule in ruleBlock.SubRules)
        WorkItemRulesService.PopulateFieldId(requestContext, subRule);
    }
  }
}
