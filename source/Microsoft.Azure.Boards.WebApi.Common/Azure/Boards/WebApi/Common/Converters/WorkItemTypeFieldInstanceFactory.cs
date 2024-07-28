// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemTypeFieldInstanceFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemTypeFieldInstanceFactory
  {
    public static WorkItemTypeFieldInstance Create(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry field,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType wit,
      AdditionalWorkItemTypeProperties additionalWorkItemTypeProperties,
      WorkItemTypeFieldsExpandLevel expandLevel = WorkItemTypeFieldsExpandLevel.None,
      ProcessDescriptor projectProcess = null,
      Guid? projectId = null,
      bool includeUrl = true)
    {
      Guid projectId1 = new Guid();
      if (projectId.HasValue)
        projectId1 = projectId.Value;
      return WorkItemTypeFieldInstanceFactory.Create(witRequestContext, field, additionalWorkItemTypeProperties, projectProcess, projectId1, wit, expandLevel, includeUrl);
    }

    public static WorkItemTypeFieldInstance Create(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry field,
      AdditionalWorkItemTypeProperties additionalWorkItemTypeProperties,
      ProcessDescriptor projectProcess,
      Guid projectId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType,
      WorkItemTypeFieldsExpandLevel expandLevel = WorkItemTypeFieldsExpandLevel.None,
      bool includeUrl = true)
    {
      if (expandLevel != WorkItemTypeFieldsExpandLevel.None && !CommonWITUtils.HasReadRulesPermission(witRequestContext.RequestContext))
      {
        if (expandLevel == WorkItemTypeFieldsExpandLevel.AllowedValues)
          throw new ReadAllowedValuesNotAuthorizedException(field.ReferenceName);
        if (expandLevel == WorkItemTypeFieldsExpandLevel.DependentFields)
          throw new ReadDependentFieldsNotAuthorizedException(field.ReferenceName);
        throw new ReadRulesPermissionException(field.ReferenceName, workItemType.ReferenceName);
      }
      WorkItemFieldReference itemFieldReference = WorkItemFieldReferenceFactory.Create(witRequestContext, field, projectId: new Guid?(projectId), includeUrl: includeUrl);
      WorkItemTypeFieldInstance typeFieldInstance = workItemType == null ? new WorkItemTypeFieldInstance() : new WorkItemTypeFieldInstance((ISecuredObject) workItemType);
      typeFieldInstance.Name = itemFieldReference.Name;
      typeFieldInstance.ReferenceName = itemFieldReference.ReferenceName;
      typeFieldInstance.Url = itemFieldReference.Url;
      typeFieldInstance.IsIdentity = field.IsIdentity;
      string str1;
      if (additionalWorkItemTypeProperties.FieldHelpTexts != null && additionalWorkItemTypeProperties.FieldHelpTexts.TryGetValue(field.FieldId, out str1))
        typeFieldInstance.HelpText = str1;
      IEnumerable<WorkItemFieldRule> fieldRules = additionalWorkItemTypeProperties?.FieldRules;
      WorkItemFieldRule workItemFieldRule = fieldRules != null ? fieldRules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (rule => rule.FieldId == field.FieldId)) : (WorkItemFieldRule) null;
      if (workItemFieldRule?.SubRules != null)
      {
        typeFieldInstance.AlwaysRequired = WorkItemTypeFieldInstanceFactory.HasRequiredRule((IEnumerable<WorkItemRule>) workItemFieldRule.SubRules) || WorkItemTypeFieldInstanceFactory.HasRequiredRule(workItemFieldRule.SubRules.OfType<WorkItemTypeScopedRules>().SelectMany<WorkItemTypeScopedRules, WorkItemRule>((Func<WorkItemTypeScopedRules, IEnumerable<WorkItemRule>>) (r => (IEnumerable<WorkItemRule>) r.SubRules)));
        string str2 = WorkItemTypeFieldInstanceFactory.GetDefaultRuleValue((IEnumerable<WorkItemRule>) workItemFieldRule.SubRules) ?? WorkItemTypeFieldInstanceFactory.GetDefaultRuleValue(workItemFieldRule.SubRules.OfType<WorkItemTypeScopedRules>().SelectMany<WorkItemTypeScopedRules, WorkItemRule>((Func<WorkItemTypeScopedRules, IEnumerable<WorkItemRule>>) (r => (IEnumerable<WorkItemRule>) r.SubRules)));
        typeFieldInstance.DefaultValue = str2;
      }
      if (expandLevel != WorkItemTypeFieldsExpandLevel.None)
      {
        if (expandLevel == WorkItemTypeFieldsExpandLevel.AllowedValues || expandLevel == WorkItemTypeFieldsExpandLevel.All)
        {
          string[] strArray = WorkItemTypeFieldInstanceFactory.GeFieldAllowedValues(witRequestContext, field, new string[1]
          {
            additionalWorkItemTypeProperties.WorkItemType.Name
          }, projectProcess, projectId);
          typeFieldInstance.AllowedValues = strArray;
        }
        if (expandLevel == WorkItemTypeFieldsExpandLevel.DependentFields || expandLevel == WorkItemTypeFieldsExpandLevel.All)
        {
          IEnumerable<WorkItemFieldReference> dependentFields = FieldDependentRuleFactory.GetDependentFields(witRequestContext, projectId, additionalWorkItemTypeProperties.WorkItemType.Name, field.Name).DependentFields;
          IEnumerable<WorkItemFieldReference> itemFieldReferences = (dependentFields != null ? dependentFields.Select<WorkItemFieldReference, WorkItemFieldReference>((Func<WorkItemFieldReference, WorkItemFieldReference>) (df => new WorkItemFieldReference((ISecuredObject) workItemType)
          {
            Name = df.Name,
            ReferenceName = df.ReferenceName,
            Url = df.Url
          })) : (IEnumerable<WorkItemFieldReference>) null) ?? Enumerable.Empty<WorkItemFieldReference>();
          typeFieldInstance.DependentFields = itemFieldReferences;
        }
      }
      return typeFieldInstance;
    }

    public static string[] GeFieldAllowedValues(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry field,
      string[] workItemTypeNames,
      ProcessDescriptor projectProcess,
      Guid projectId,
      bool bypassCustomProcessCache = true)
    {
      string[] strArray;
      if (field.IsIdentity)
        strArray = Array.Empty<string>();
      else if (projectProcess != null && !projectProcess.IsCustom)
      {
        IReadOnlyCollection<string> allowedValues;
        strArray = !witRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>().TryGetAllowedValues(witRequestContext.RequestContext, projectProcess.TypeId, field.FieldId, (IEnumerable<string>) workItemTypeNames, out allowedValues, bypassCustomProcessCache) ? new string[0] : allowedValues.ToArray<string>();
      }
      else
      {
        IEnumerable<string> allowedValues = witRequestContext.RequestContext.GetService<ITeamFoundationWorkItemService>().GetAllowedValues(witRequestContext.RequestContext, field.FieldId, witRequestContext.GetProjectName(projectId), (IEnumerable<string>) workItemTypeNames, true);
        strArray = allowedValues != null ? allowedValues.ToArray<string>() : (string[]) null;
      }
      if (strArray == null)
        strArray = Array.Empty<string>();
      return strArray;
    }

    public static WorkItemTypeFieldWithReferences CreateWorkItemTypeFieldWithReferences(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemTypeFieldInstance fieldInstance)
    {
      return WorkItemTypeFieldInstanceFactory.CreateWorkItemTypeFieldWithReferences(witRequestContext, (IEnumerable<WorkItemTypeFieldInstance>) new List<WorkItemTypeFieldInstance>()
      {
        fieldInstance
      }).FirstOrDefault<WorkItemTypeFieldWithReferences>();
    }

    public static IEnumerable<WorkItemTypeFieldWithReferences> CreateWorkItemTypeFieldWithReferences(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeFieldInstance> fieldInstanceList)
    {
      IEnumerable<string> allIdentityValues = WorkItemTypeFieldInstanceFactory.GetAllIdentityValues(fieldInstanceList);
      List<WorkItemTypeFieldWithReferences> fieldInstanceList1 = new List<WorkItemTypeFieldWithReferences>();
      foreach (WorkItemTypeFieldInstance fieldInstance in fieldInstanceList)
        fieldInstanceList1.Add(new WorkItemTypeFieldWithReferences(fieldInstance));
      if (allIdentityValues.Count<string>() == 0)
        return (IEnumerable<WorkItemTypeFieldWithReferences>) fieldInstanceList1;
      IDictionary<string, IdentityRef> witIdentityNames = IdentityReferenceBuilder.CreateIdentityRefFromWitIdentityNames(witRequestContext, allIdentityValues, secureInfoSourceObjectForConstantIdentity: (ISecuredObject) fieldInstanceList.First<WorkItemTypeFieldInstance>());
      return WorkItemTypeFieldInstanceFactory.GetWorkItemTypeFieldWithReferences(witRequestContext.RequestContext, (IEnumerable<WorkItemTypeFieldWithReferences>) fieldInstanceList1, witIdentityNames);
    }

    public static IEnumerable<string> GetAllIdentityValues(
      IEnumerable<WorkItemTypeFieldInstance> fieldInstanceList)
    {
      HashSet<string> allIdentityValues = new HashSet<string>();
      foreach (WorkItemTypeFieldInstance fieldInstance in fieldInstanceList)
      {
        if (fieldInstance.IsIdentity && fieldInstance.AllowedValues != null)
        {
          foreach (string allowedValue in fieldInstance.AllowedValues)
            allIdentityValues.Add(allowedValue);
          if (fieldInstance.DefaultValue != null)
            allIdentityValues.Add(fieldInstance.DefaultValue);
        }
      }
      return (IEnumerable<string>) allIdentityValues;
    }

    public static IEnumerable<WorkItemTypeFieldWithReferences> GetWorkItemTypeFieldWithReferences(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeFieldWithReferences> fieldInstanceList,
      IDictionary<string, IdentityRef> stringToIdentityRefDict)
    {
      foreach (WorkItemTypeFieldWithReferences fieldInstance in fieldInstanceList)
      {
        if (fieldInstance.IsIdentity)
        {
          IdentityRef identityRef;
          if (fieldInstance.AllowedValues != null)
          {
            List<IdentityRef> identityRefList = new List<IdentityRef>();
            foreach (string allowedValue in fieldInstance.AllowedValues)
            {
              if (stringToIdentityRefDict.TryGetValue(allowedValue, out identityRef))
                identityRefList.Add(identityRef);
              else
                requestContext.Trace(915050, TraceLevel.Info, "Services", "MetadataService", "Coudn't resolve the identity combo string " + allowedValue);
            }
            fieldInstance.AllowedValues = (object[]) identityRefList.ToArray();
          }
          if (fieldInstance.DefaultValue != null)
          {
            if (stringToIdentityRefDict.TryGetValue((string) fieldInstance.DefaultValue, out identityRef))
              fieldInstance.DefaultValue = (object) identityRef;
            else
              requestContext.Trace(915050, TraceLevel.Info, "Services", "MetadataService", "Coudn't resolve the identity combo string " + (string) fieldInstance.DefaultValue);
          }
        }
      }
      return fieldInstanceList;
    }

    private static bool HasRequiredRule(IEnumerable<WorkItemRule> rules) => rules != null && rules.Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is RequiredRule));

    private static string GetDefaultRuleValue(IEnumerable<WorkItemRule> rules) => (rules != null ? rules.FirstOrDefault<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is DefaultRule)) : (WorkItemRule) null) is DefaultRule defaultRule ? defaultRule.Value : (string) null;
  }
}
