// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process.ProcessWorkItemTypeFieldFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process
{
  public class ProcessWorkItemTypeFieldFactory
  {
    private readonly IVssRequestContext m_requestContext;

    public ProcessWorkItemTypeFieldFactory(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected ProcessWorkItemTypeFieldFactory()
    {
    }

    public ProcessWorkItemTypeField Create(
      Guid processId,
      string witRefName,
      string fieldRefName,
      ProcessFieldResult field,
      IEnumerable<WorkItemRule> rules,
      bool isCustomized = false,
      ProcessWorkItemTypeFieldsExpandLevel expand = ProcessWorkItemTypeFieldsExpandLevel.None)
    {
      return this.CreateInternal(processId, witRefName, fieldRefName, (object) field, rules, isCustomized ? (field.IsSystem ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Inherited : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Custom) : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.System, expand);
    }

    public ProcessWorkItemTypeField Create(
      Guid processId,
      string witRefName,
      string fieldRefName,
      FieldEntry field,
      IEnumerable<WorkItemRule> rules,
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType customization,
      ProcessWorkItemTypeFieldsExpandLevel expand = ProcessWorkItemTypeFieldsExpandLevel.None)
    {
      return this.CreateInternal(processId, witRefName, fieldRefName, (object) field, rules, customization, expand);
    }

    private ProcessWorkItemTypeField CreateInternal(
      Guid processId,
      string witRefName,
      string fieldRefName,
      object field,
      IEnumerable<WorkItemRule> rules,
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType customization,
      ProcessWorkItemTypeFieldsExpandLevel expand = ProcessWorkItemTypeFieldsExpandLevel.None)
    {
      IBasicFieldProperties basicProps = this.GetBasicProps(field);
      IFieldRuleProperties ruleProps = this.GetRuleProps(rules, basicProps);
      IFieldLockingProperties fieldLockingProps = this.GetFieldLockingProps(field);
      string str1 = this.m_requestContext.GetService<IProcessWorkItemTypeService>().GetHelpTextRule(this.m_requestContext, processId, witRefName, fieldRefName)?.Value;
      string str2 = str1 == null || !(str1 != "") ? basicProps.Description : str1;
      return new ProcessWorkItemTypeField()
      {
        ReferenceName = basicProps.ReferenceName,
        Name = basicProps.Name,
        Description = str2,
        Type = basicProps.Type,
        ReadOnly = ruleProps.ReadOnly,
        Required = ruleProps.Required,
        AllowGroups = ruleProps.AllowGroups,
        DefaultValue = ruleProps.DefaultValue,
        Customization = customization,
        Url = this.GetFieldUrl(processId, witRefName, basicProps.ReferenceName),
        AllowedValues = (object[]) this.GetAllowedValues(processId, witRefName, fieldRefName, rules, expand),
        IsLocked = fieldLockingProps.IsLocked
      };
    }

    private string[] GetAllowedValues(
      Guid processId,
      string workItemTypeRefName,
      string fieldRefName,
      IEnumerable<WorkItemRule> rules,
      ProcessWorkItemTypeFieldsExpandLevel expandLevel = ProcessWorkItemTypeFieldsExpandLevel.None)
    {
      if (expandLevel != ProcessWorkItemTypeFieldsExpandLevel.None && !CommonWITUtils.HasReadRulesPermission(this.m_requestContext))
      {
        if (expandLevel == ProcessWorkItemTypeFieldsExpandLevel.AllowedValues)
          throw new ReadAllowedValuesNotAuthorizedException(fieldRefName);
        throw new ReadRulesPermissionException(fieldRefName, workItemTypeRefName);
      }
      string[] allowedValues1 = (string[]) null;
      if (expandLevel == ProcessWorkItemTypeFieldsExpandLevel.AllowedValues || expandLevel == ProcessWorkItemTypeFieldsExpandLevel.All)
      {
        FieldEntry field;
        if (!this.m_requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(this.m_requestContext, fieldRefName, out field))
          throw new ArgumentException("Unknown field '{FormatCurrent(fieldName)}.", fieldRefName);
        if (field.IsIdentity)
        {
          allowedValues1 = Array.Empty<string>();
        }
        else
        {
          if (rules != null)
          {
            foreach (WorkItemRule rule in rules)
            {
              if (rule is AllowedValuesRule)
              {
                IEnumerable<string> workItemFieldRule = WorkItemTrackingProcessService.GetAllowedAndSuggestedValuesFromWorkItemFieldRule(rule);
                return workItemFieldRule != null ? workItemFieldRule.ToArray<string>() : new HashSet<string>().ToArray<string>();
              }
            }
          }
          IWorkItemTrackingProcessService service = this.m_requestContext.GetService<IWorkItemTrackingProcessService>();
          string[] workItemTypeNames = new string[1]
          {
            workItemTypeRefName
          };
          IReadOnlyCollection<string> allowedValues2;
          allowedValues1 = !service.TryGetAllowedValues(this.m_requestContext, processId, field.FieldId, (IEnumerable<string>) workItemTypeNames, out allowedValues2) ? new string[0] : allowedValues2.ToArray<string>();
        }
      }
      return allowedValues1;
    }

    private IFieldRuleProperties GetRuleProps(
      IEnumerable<WorkItemRule> rules,
      IBasicFieldProperties basicProps)
    {
      return new FieldRulePropertiesFactory().Create(rules, basicProps.Type, new Func<Guid, IdentityRef>(this.GetIdentityRef));
    }

    private IBasicFieldProperties GetBasicProps(object field)
    {
      BasicFieldPropertiesFactory propertiesFactory = new BasicFieldPropertiesFactory();
      switch (field)
      {
        case FieldEntry _:
          return propertiesFactory.Create(field as FieldEntry);
        case ProcessFieldResult _:
          return propertiesFactory.Create(field as ProcessFieldResult);
        default:
          throw new ArgumentException(CommonResources.UnexpectedType((object) nameof (field), (object) field.GetType().FullName));
      }
    }

    private IFieldLockingProperties GetFieldLockingProps(object field)
    {
      BasicFieldPropertiesFactory propertiesFactory = new BasicFieldPropertiesFactory();
      switch (field)
      {
        case FieldEntry _:
          return propertiesFactory.CreateLockingProps(field as FieldEntry);
        case ProcessFieldResult _:
          return propertiesFactory.CreateLockingProps(field as ProcessFieldResult);
        default:
          throw new ArgumentException(CommonResources.UnexpectedType((object) nameof (field), (object) field.GetType().FullName));
      }
    }

    protected virtual IdentityRef GetIdentityRef(Guid vsid)
    {
      WorkItemIdentity workItemIdentity;
      WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(this.m_requestContext, (IEnumerable<Guid>) new List<Guid>()
      {
        vsid
      }).TryGetValue(vsid, out workItemIdentity);
      return workItemIdentity?.IdentityRef;
    }

    protected virtual string GetFieldUrl(Guid processId, string witRefName, string fieldRefName) => this.m_requestContext.GetService<ILocationService>().GetResourceUri(this.m_requestContext, "processes", WorkItemTrackingLocationIds.ProcessBehaviors, (object) new
    {
      processId = processId,
      witRefName = witRefName,
      fieldRefName = fieldRefName
    }).AbsoluteUri;
  }
}
