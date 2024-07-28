// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.WorkItemTypeFieldModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class WorkItemTypeFieldModelFactory
  {
    private const string c_currentDateTime = "$currentDateTime";
    private const string c_currentUser = "$currentUser";

    public static WorkItemTypeFieldModel Create(
      IVssRequestContext requestContext,
      ProcessTypelet wit,
      string fieldRefName)
    {
      FieldEntry field = wit.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.Field.ReferenceName, fieldRefName))).FirstOrDefault<WorkItemTypeExtensionFieldEntry>().Field;
      WorkItemTypeletFieldRuleProperties propertiesFromRules = ProcessWorkItemTypeService.ExtractPropertiesFromRules((IEnumerable<WorkItemRule>) requestContext.GetService<IProcessWorkItemTypeService>().GetCombinedRulesForField(requestContext, wit.ProcessId, wit.ReferenceName, fieldRefName).ToList<WorkItemRule>(), field.IsIdentity);
      string str = propertiesFromRules.DefaultValue;
      if (propertiesFromRules.DefaultValueFrom == RuleValueFrom.Clock)
        str = "$currentDateTime";
      else if (propertiesFromRules.DefaultValueFrom == RuleValueFrom.CurrentUser)
        str = "$currentUser";
      return new WorkItemTypeFieldModel()
      {
        ReferenceName = field.ReferenceName,
        Name = field.Name,
        Type = (FieldType) Enum.Parse(typeof (FieldType), field.FieldType.ToString()),
        ReadOnly = propertiesFromRules.IsReadOnly,
        Required = propertiesFromRules.IsRequired,
        DefaultValue = str,
        PickList = FieldModelFactory.GetPickListMetadataModelOrNull(requestContext, field.PickListId),
        Url = WorkItemTypeFieldModelFactory.GetLocationUrlForField(requestContext, wit.ProcessId, wit.ReferenceName, fieldRefName),
        AllowGroups = propertiesFromRules.AllowGroups
      };
    }

    public static WorkItemTypeletFieldRuleProperties GetFieldProperties(WorkItemTypeFieldModel field)
    {
      RuleValueFrom defaultValueFrom = WorkItemTypeFieldModelFactory.GetDefaultValueFrom((object) field.DefaultValue);
      return new WorkItemTypeletFieldRuleProperties(field.Required, field.ReadOnly, field.DefaultValue, defaultValueFrom, field.AllowGroups, (string[]) null);
    }

    public static WorkItemTypeletFieldRuleProperties GetFieldProperties(
      IVssRequestContext requestContext,
      WorkItemTypeFieldModel2 field)
    {
      string defaultValue = (string) null;
      if (field.DefaultValue != null)
      {
        if (field.DefaultValue is IdentityRef)
        {
          WorkItemIdentity workItemIdentity = new WorkItemIdentity()
          {
            IdentityRef = field.DefaultValue as IdentityRef
          };
          ResolvedIdentityNamesInfo identityNamesInfo = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityNames(requestContext.WitContext(), Enumerable.Empty<string>(), (IEnumerable<WorkItemIdentity>) new List<WorkItemIdentity>()
          {
            workItemIdentity
          }, false);
          if (identityNamesInfo.AllRecords.Count<ConstantsSearchRecord>() != 1)
            throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotResolveIdentityByDescriptor((object) workItemIdentity.IdentityRef.Descriptor), "DefaultValue");
          defaultValue = identityNamesInfo.AllRecords.First<ConstantsSearchRecord>().TeamFoundationId.ToString();
        }
        else
          defaultValue = field.DefaultValue is string ? field.DefaultValue as string : throw new ArgumentException("Default value should be either IdentityRef or String", "DefaultValue");
      }
      RuleValueFrom defaultValueFrom = WorkItemTypeFieldModelFactory.GetDefaultValueFrom(field.DefaultValue);
      return new WorkItemTypeletFieldRuleProperties(field.Required, field.ReadOnly, defaultValue, defaultValueFrom, field.AllowGroups, (string[]) null);
    }

    public static IEnumerable<WorkItemTypeFieldModel2> CreateFieldModelsWithIdentityRef(
      IVssRequestContext requestContext,
      ProcessTypelet processTypelet,
      IEnumerable<WorkItemTypeExtensionFieldEntry> fields)
    {
      IEnumerable<WorkItemTypeFieldModel> source = fields.Select<WorkItemTypeExtensionFieldEntry, WorkItemTypeFieldModel>((Func<WorkItemTypeExtensionFieldEntry, WorkItemTypeFieldModel>) (f => WorkItemTypeFieldModelFactory.Create(requestContext, processTypelet, f.Field.ReferenceName)));
      HashSet<string> identityFieldRefNames = new HashSet<string>(processTypelet.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.IsIdentity)).Select<WorkItemTypeExtensionFieldEntry, string>((Func<WorkItemTypeExtensionFieldEntry, string>) (f => f.Field.ReferenceName)));
      IEnumerable<string> strings = source.Where<WorkItemTypeFieldModel>((Func<WorkItemTypeFieldModel, bool>) (f => identityFieldRefNames.Contains(f.ReferenceName))).Select<WorkItemTypeFieldModel, string>((Func<WorkItemTypeFieldModel, string>) (f => f.DefaultValue));
      HashSet<Guid> vsids = new HashSet<Guid>();
      foreach (string input in strings)
      {
        Guid result;
        if (Guid.TryParse(input, out result))
          vsids.Add(result);
      }
      IDictionary<Guid, WorkItemIdentity> workItemIdentityMap = WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(requestContext, (IEnumerable<Guid>) vsids);
      List<WorkItemTypeFieldModel2> modelsWithIdentityRef = new List<WorkItemTypeFieldModel2>();
      foreach (WorkItemTypeFieldModel itemTypeFieldModel in source)
      {
        WorkItemTypeFieldModel field = itemTypeFieldModel;
        string str1 = requestContext.GetService<IProcessWorkItemTypeService>().GetHelpTextRule(requestContext, processTypelet.ProcessId, processTypelet.ReferenceName, field.ReferenceName)?.Value;
        string str2 = str1 == null || !(str1 != "") ? processTypelet.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ReferenceName == field.ReferenceName)).FirstOrDefault<WorkItemTypeExtensionFieldEntry>()?.Field?.Description : str1;
        WorkItemTypeFieldModel2 itemTypeFieldModel2 = new WorkItemTypeFieldModel2()
        {
          ReferenceName = field.ReferenceName,
          Name = field.Name,
          Description = str2,
          Type = field.Type,
          PickList = field.PickList,
          ReadOnly = field.ReadOnly,
          DefaultValue = (object) field.DefaultValue,
          Required = field.Required,
          Url = field.Url,
          AllowGroups = field.AllowGroups
        };
        if (identityFieldRefNames.Contains(field.ReferenceName))
        {
          Guid result;
          Guid.TryParse(field.DefaultValue, out result);
          if (workItemIdentityMap.ContainsKey(result))
            itemTypeFieldModel2.DefaultValue = (object) workItemIdentityMap[result].IdentityRef;
        }
        modelsWithIdentityRef.Add(itemTypeFieldModel2);
      }
      return (IEnumerable<WorkItemTypeFieldModel2>) modelsWithIdentityRef;
    }

    public static WorkItemTypeFieldModel2 CreateFieldModelWithIdentityRef(
      IVssRequestContext requestContext,
      ProcessTypelet processTypelet,
      WorkItemTypeExtensionFieldEntry field)
    {
      return WorkItemTypeFieldModelFactory.CreateFieldModelsWithIdentityRef(requestContext, processTypelet, (IEnumerable<WorkItemTypeExtensionFieldEntry>) new List<WorkItemTypeExtensionFieldEntry>()
      {
        field
      }).First<WorkItemTypeFieldModel2>();
    }

    private static RuleValueFrom GetDefaultValueFrom(object defaultValue)
    {
      RuleValueFrom defaultValueFrom = RuleValueFrom.Value;
      if (defaultValue is string)
      {
        switch (defaultValue as string)
        {
          case "$currentDateTime":
            defaultValueFrom = RuleValueFrom.Clock;
            break;
          case "$currentUser":
            defaultValueFrom = RuleValueFrom.CurrentUser;
            break;
        }
      }
      return defaultValueFrom;
    }

    private static string GetLocationUrlForField(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefNameForFields,
      string fieldRefName)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypeFields, (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields,
        fieldRefName = fieldRefName
      }).ToString();
    }
  }
}
