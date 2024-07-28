// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessFieldService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessFieldService : IProcessFieldService, IVssFrameworkService
  {
    public static readonly string SystemFieldPrefix = "System.";
    public static readonly string MicrosoftVSTSFieldPrefix = "Microsoft.VSTS.";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual IReadOnlyCollection<ProcessFieldResult> GetFields(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyCollection<ProcessFieldResult>) requestContext.TraceBlock<List<ProcessFieldResult>>(909981, 909982, "Field", nameof (ProcessFieldService), nameof (GetFields), (Func<List<ProcessFieldResult>>) (() => requestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, processId).SelectMany<ComposedWorkItemType, ProcessFieldResult>((Func<ComposedWorkItemType, IEnumerable<ProcessFieldResult>>) (composedWorkItemType => (IEnumerable<ProcessFieldResult>) composedWorkItemType.GetLegacyFields(requestContext))).Distinct<ProcessFieldResult>().Select<ProcessFieldResult, ProcessFieldResult>((Func<ProcessFieldResult, ProcessFieldResult>) (f => new ProcessFieldResult()
      {
        Name = f.Name,
        ReferenceName = f.ReferenceName,
        Type = f.Type,
        Description = f.Description,
        PickListId = f.PickListId,
        IsIdentity = f.IsIdentity,
        IsLocked = f.IsLocked
      })).ToList<ProcessFieldResult>()));
    }

    public FieldEntry CreateField(
      IVssRequestContext requestContext,
      string name,
      string description,
      InternalFieldType type,
      Guid? processId = null,
      Guid? picklistId = null,
      string referenceName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      bool customizationEnabled = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      return requestContext.TraceBlock<FieldEntry>(909983, 909984, "Field", nameof (ProcessFieldService), nameof (CreateField), (Func<FieldEntry>) (() =>
      {
        name = CommonWITUtils.RemoveASCIIControlCharactersAndTrim(name);
        CommonWITUtils.CheckValidName(name, 128);
        description = CommonWITUtils.ValidateAndSanitizeDescription(description, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Field() + " : " + name);
        FieldDBType fieldDbType = FieldHelpers.StringToFieldDBType(type.ToString(), false);
        if (customizationEnabled && picklistId.HasValue)
          this.ValidatePicklist(requestContext, fieldDbType, picklistId.Value);
        this.CheckFieldCreatePermission(requestContext);
        if (this.IsFieldNameInUse(requestContext, name))
          throw new ProcessFieldAlreadyExistsInformedException(name);
        if (string.IsNullOrWhiteSpace(referenceName))
          referenceName = this.CreateUniqueReferenceName(requestContext, "Custom", name);
        else
          this.ValidateReferenceName(requestContext, referenceName);
        if (this.IsFieldNameInUse(requestContext, referenceName))
          throw new ProcessFieldAlreadyExistsException();
        if (this.IsFieldReferenceNameInUse(requestContext, name))
          throw new ProcessFieldAlreadyExistsInformedException(name);
        if (string.Compare(referenceName, name, true) == 0)
          throw new ProcessFieldAlreadyExistsInformedException(name);
        this.CheckFieldCountLimit(requestContext);
        CustomFieldEntry customFieldEntry = new CustomFieldEntry()
        {
          Name = name,
          ReferenceName = referenceName,
          Type = (int) fieldDbType,
          ReportingType = 0,
          ReportingEnabled = false,
          Usage = -100,
          Description = description,
          IsIdentityFromProcess = type == InternalFieldType.Identity,
          IsLocked = false
        };
        if (WorkItemTrackingFeatureFlags.IsProcessFieldAssociationEnabled(requestContext) && processId.HasValue && processId.HasValue)
          customFieldEntry.ProcessId = processId.Value;
        if (customizationEnabled && picklistId.HasValue)
          customFieldEntry.PicklistId = picklistId.Value;
        FieldEntry fieldInternal = this.CreateFieldInternal(requestContext, customFieldEntry);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("CreatedCustomField", string.Format("[NonEmail: {0}]", (object) referenceName));
        properties.Add("FieldType", !customizationEnabled || !picklistId.HasValue ? type.ToString() : string.Format("Picklist ({0})", (object) type));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessFieldService), nameof (CreateField), properties);
        return fieldInternal;
      }));
    }

    public virtual FieldEntry UpdateField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      string description = "",
      bool convertToPicklist = false,
      bool? isIdentityFromProcess = null,
      bool makePicklistSuggestedValue = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fieldNameOrRefName, nameof (fieldNameOrRefName));
      string methodName = nameof (UpdateField);
      return requestContext.TraceBlock<FieldEntry>(909995, 909996, "Field", nameof (ProcessFieldService), methodName, (Func<FieldEntry>) (() =>
      {
        FieldEntry field;
        if (!requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(requestContext, fieldNameOrRefName, out field, new bool?(true)))
          throw new ProcessFieldCouldNotBeFoundException(fieldNameOrRefName);
        if (this.IsSystemField(field.ReferenceName) || this.IsOOBField(requestContext, field.Name, field.ReferenceName))
          throw new ProcessFieldSystemFieldUpdateBlockedException();
        this.CheckCollectionLevelFieldEditPermission(requestContext, field.ReferenceName);
        Guid? convertToPicklistId = new Guid?();
        if (convertToPicklist)
        {
          if (field.IsPicklist)
            convertToPicklist = false;
          else
            convertToPicklistId = new Guid?(this.CreatePicklistForExistingField(requestContext, field, makePicklistSuggestedValue));
        }
        description = CommonWITUtils.ValidateAndSanitizeDescription(description != null ? description : "", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Field() + " : " + fieldNameOrRefName);
        FieldEntry fieldEntry = this.UpdateFieldInternal(requestContext, field, description, convertToPicklistId, isIdentityFromProcess);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdatedCustomField", fieldNameOrRefName);
        properties.Add("ConvertToPicklist", convertToPicklist);
        if (convertToPicklist)
        {
          properties.Add("PickListType", ProcessFieldService.CheckAndGetPickListType(fieldEntry).ToString());
          requestContext.GetService<LegacyWorkItemTrackingProcessWorkDefinitionCache>().Clear(requestContext);
          requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.LegacyProcessFieldDefinitionChanged, "");
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessFieldService), methodName, properties);
        return fieldEntry;
      }));
    }

    public FieldEntry EnsureFieldExists(
      IVssRequestContext requestContext,
      string fieldReferenceName)
    {
      return requestContext.TraceBlock<FieldEntry>(909985, 909986, "Field", nameof (ProcessFieldService), "EnusureFieldExists", (Func<FieldEntry>) (() =>
      {
        FieldEntry field1 = (FieldEntry) null;
        bool field2 = requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(requestContext, fieldReferenceName, out field1, new bool?(true));
        ProcessFieldDefinition processFieldDefinition = this.GetAllOutOfBoxFieldDefinitions(requestContext).FirstOrDefault<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, fieldReferenceName)));
        if (processFieldDefinition != null)
        {
          if (field2)
            return field1;
          return this.CreateFieldInternal(requestContext, new CustomFieldEntry()
          {
            Name = processFieldDefinition.Name,
            ReferenceName = processFieldDefinition.ReferenceName,
            Type = (int) FieldHelpers.StringToFieldDBType(processFieldDefinition.Type.ToString(), processFieldDefinition.SyncNameChanges),
            ReportingEnabled = processFieldDefinition.ReportingType != 0,
            ReportingType = processFieldDefinition.ReportingType,
            ReportingFormula = processFieldDefinition.ReportingFormula,
            Usage = -100,
            IsIdentityFromProcess = processFieldDefinition.IsIdentity,
            IsLocked = processFieldDefinition.IsLocked
          });
        }
        if (field2)
          return field1;
        throw new WorkItemTrackingFieldDefinitionNotFoundException(fieldReferenceName);
      }));
    }

    public FieldEntry ConvertFieldWithAllowedValuesToPicklist(
      IVssRequestContext requestContext,
      FieldEntry fieldEntry)
    {
      if (this.IsSystemField(fieldEntry.ReferenceName) || this.IsOOBField(requestContext, fieldEntry.Name, fieldEntry.ReferenceName) || fieldEntry.IsPicklist || fieldEntry.FieldType != InternalFieldType.String && fieldEntry.FieldType != InternalFieldType.Integer || fieldEntry.IsIdentity)
        return fieldEntry;
      if (this.FetchRulesListValuesInternal(requestContext, new int[1]
      {
        fieldEntry.FieldId
      }, true).ContainsKey(fieldEntry.FieldId))
        fieldEntry = this.UpdateField(requestContext, fieldEntry.ReferenceName, "", true, new bool?(), false);
      return fieldEntry;
    }

    public void DeleteField(IVssRequestContext requestContext, string referenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(referenceName, nameof (referenceName));
      requestContext.TraceBlock(909997, 909998, "Field", nameof (ProcessFieldService), nameof (DeleteField), (Action) (() =>
      {
        if (!requestContext.GetService<IWorkItemTrackingProcessService>().HasDeleteFieldPermission(requestContext))
          this.ThrowAccessDeniedException(requestContext);
        WorkItemTrackingFieldService service1 = requestContext.GetService<WorkItemTrackingFieldService>();
        FieldEntry fieldEntry;
        if (!service1.TryGetField(requestContext, referenceName, out fieldEntry, new bool?(true)))
          throw new ProcessFieldCouldNotBeFoundException(referenceName);
        if (this.GetAllOutOfBoxFieldReferenceNameToNameMappings(requestContext).ContainsKey(fieldEntry.ReferenceName))
          throw new OutOfBoxFieldCannotBeDeletedException(fieldEntry.ReferenceName);
        this.RemoveFieldFromAllProcesses(requestContext, referenceName);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          component.DeleteFields((IReadOnlyCollection<int>) new int[1]
          {
            fieldEntry.FieldId
          }, id);
        service1.InvalidateCache(requestContext);
        if (fieldEntry.IsPicklist)
        {
          Guid? pickListId1 = fieldEntry.PickListId;
          if (pickListId1.HasValue && WorkItemTrackingFeatureFlags.IsDeletePicklistWhenNotReferencedByAnyFieldEnabled(requestContext) && !service1.GetAllFields(requestContext, new bool?(true)).Any<FieldEntry>((Func<FieldEntry, bool>) (x =>
          {
            Guid? pickListId2 = x.PickListId;
            Guid guid = fieldEntry.PickListId.Value;
            if (!pickListId2.HasValue)
              return false;
            return !pickListId2.HasValue || pickListId2.GetValueOrDefault() == guid;
          })))
          {
            IWorkItemPickListService service2 = requestContext.GetService<IWorkItemPickListService>();
            IVssRequestContext requestContext1 = requestContext;
            pickListId1 = fieldEntry.PickListId;
            Guid listId = pickListId1.Value;
            service2.DeleteList(requestContext1, listId);
          }
        }
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("DeletedCustomField", referenceName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessFieldService), nameof (DeleteField), properties);
      }));
    }

    protected virtual void ThrowAccessDeniedException(IVssRequestContext requestContext)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      Guid templatesNamespaceId = FrameworkSecurity.ProcessTemplatesNamespaceId;
      string token = TFCommonResources.NAMESPACE_DELETE_FIELD();
      int deleteField = TeamProjectCollectionPermissions.DeleteField;
      IVssRequestContext requestContext1 = requestContext;
      Guid namespaceId = templatesNamespaceId;
      service.GetSecurityNamespace(requestContext1, namespaceId).ThrowAccessDeniedException(requestContext, token, deleteField);
    }

    public virtual IDictionary<string, string> GetAllOutOfBoxFieldReferenceNameToNameMappings(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IDictionary<string, string>) requestContext.TraceBlock<Dictionary<string, string>>(909999, 910000, "Field", nameof (ProcessFieldService), nameof (GetAllOutOfBoxFieldReferenceNameToNameMappings), (Func<Dictionary<string, string>>) (() =>
      {
        Dictionary<string, string> nameToNameMappings = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (ProcessFieldDefinition boxFieldDefinition in (IEnumerable<ProcessFieldDefinition>) this.GetAllOutOfBoxFieldDefinitions(requestContext))
          nameToNameMappings[boxFieldDefinition.ReferenceName] = boxFieldDefinition.Name;
        return nameToNameMappings;
      }));
    }

    public virtual IReadOnlyCollection<ProcessFieldDefinition> GetAllOutOfBoxFieldDefinitions(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IReadOnlyCollection<ProcessFieldDefinition>) requestContext.TraceBlock<List<ProcessFieldDefinition>>(910001, 910002, "Field", nameof (ProcessFieldService), nameof (GetAllOutOfBoxFieldDefinitions), (Func<List<ProcessFieldDefinition>>) (() =>
      {
        WorkItemTrackingOutOfBoxFieldsCache service = requestContext.GetService<WorkItemTrackingOutOfBoxFieldsCache>();
        List<Guid> guidList = new List<Guid>();
        guidList.Add(ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment);
        guidList.Add(ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement);
        guidList.Add(ProcessTemplateTypeIdentifiers.VisualStudioScrum);
        guidList.Add(ProcessTemplateTypeIdentifiers.MsfHydroProcess);
        HashSet<string> addedFields = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        HashSet<string> addedFieldNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldFriendlyName);
        List<ProcessFieldDefinition> fieldsToReturn = new List<ProcessFieldDefinition>();
        foreach (Guid processTypeId in guidList)
        {
          IReadOnlyCollection<ProcessFieldDefinition> fieldsForProcess = service.GetOutOfBoxFieldsForProcess(requestContext, processTypeId);
          ProcessFieldService.MakeOobFieldNamesUniqueForLocalization(requestContext, addedFields, addedFieldNames, fieldsToReturn, processTypeId, fieldsForProcess);
        }
        return fieldsToReturn;
      }));
    }

    private static void MakeOobFieldNamesUniqueForLocalization(
      IVssRequestContext requestContext,
      HashSet<string> addedFields,
      HashSet<string> addedFieldNames,
      List<ProcessFieldDefinition> fieldsToReturn,
      Guid processTypeId,
      IReadOnlyCollection<ProcessFieldDefinition> oobFields)
    {
      ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
      foreach (ProcessFieldDefinition oobField in (IEnumerable<ProcessFieldDefinition>) oobFields)
      {
        if (!addedFields.Contains(oobField.ReferenceName))
        {
          FieldEntry field;
          if (requestContext.WitContext().FieldDictionary.TryGetField(oobField.ReferenceName, out field))
            oobField.Name = field.Name;
          else if (addedFieldNames.Contains(oobField.Name))
          {
            ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
            if (processDescriptor == null)
              processDescriptor = service.GetProcessDescriptor(requestContext, processTypeId);
            oobField.Name = oobField.Name + " - " + processDescriptor?.Name;
          }
          fieldsToReturn.Add(oobField);
          addedFields.Add(oobField.ReferenceName);
          addedFieldNames.Add(oobField.Name);
        }
      }
    }

    internal virtual FieldEntry CreateFieldInternal(
      IVssRequestContext requestContext,
      CustomFieldEntry customFieldEntry)
    {
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.CreateFields((IReadOnlyCollection<CustomFieldEntry>) new CustomFieldEntry[1]
        {
          customFieldEntry
        }, id);
      requestContext.ResetMetadataDbStamps();
      return requestContext.GetService<WorkItemTrackingFieldService>().GetField(requestContext, customFieldEntry.ReferenceName, new bool?(true));
    }

    internal virtual FieldEntry UpdateFieldInternal(
      IVssRequestContext requestContext,
      FieldEntry fieldEntry,
      string description,
      Guid? convertToPicklistId,
      bool? isIdentityFromProcess)
    {
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.UpdateField(fieldEntry.ReferenceName, description, id, convertToPicklistId, isIdentityFromProcess);
      requestContext.ResetMetadataDbStamps();
      return requestContext.GetService<WorkItemTrackingFieldService>().GetField(requestContext, fieldEntry.ReferenceName, new bool?(true));
    }

    internal virtual IDictionary<int, IEnumerable<string>> FetchRulesListValuesInternal(
      IVssRequestContext requestContext,
      int[] fieldIds,
      bool excludeIdentities = false)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
      {
        WorkItemTrackingMetadataComponent metadataComponent = component;
        int[] fieldIds1 = fieldIds;
        bool flag = excludeIdentities;
        int? projectId = new int?();
        int num = flag ? 1 : 0;
        return metadataComponent.GetAllowedValues((IEnumerable<int>) fieldIds1, projectId, excludeIdentities: num != 0);
      }
    }

    private Guid CreatePicklistForExistingField(
      IVssRequestContext requestContext,
      FieldEntry fieldEntry,
      bool makePicklistSuggestedValue = false)
    {
      WorkItemPickListType pickListType = ProcessFieldService.CheckAndGetPickListType(fieldEntry);
      List<string> list = this.FetchRulesListValuesInternal(requestContext, new int[1]
      {
        fieldEntry.FieldId
      }, true)[fieldEntry.FieldId].ToList<string>();
      List<string> stringList = this.ProcessAndFilterListValues(requestContext, (IEnumerable<string>) list, pickListType);
      IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
      string str = "ConvertedPicklist_" + Guid.NewGuid().ToString();
      IVssRequestContext requestContext1 = requestContext;
      string listName = str;
      int type = (int) pickListType;
      List<string> items = stringList;
      int num = makePicklistSuggestedValue ? 1 : 0;
      return service.CreateList(requestContext1, listName, (WorkItemPickListType) type, (IReadOnlyList<string>) items, num != 0).Id;
    }

    private List<string> ProcessAndFilterListValues(
      IVssRequestContext requestContext,
      IEnumerable<string> pickListValues,
      WorkItemPickListType pickListType)
    {
      ISet<string> stringSet = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> stringList = new List<string>(pickListValues.Count<string>());
      foreach (string pickListValue in pickListValues)
      {
        if (!string.IsNullOrWhiteSpace(pickListValue))
        {
          string s = pickListValue.Trim();
          switch (pickListType)
          {
            case WorkItemPickListType.Integer:
              if (int.TryParse(s, out int _))
                break;
              continue;
            case WorkItemPickListType.Double:
              if (!double.TryParse(s, out double _))
                continue;
              break;
          }
          if (!stringSet.Contains(s))
          {
            stringSet.Add(s);
            stringList.Add(s);
          }
        }
      }
      return stringList;
    }

    private void RemoveFieldFromAllProcesses(
      IVssRequestContext requestContext,
      string referenceName)
    {
      FieldEntry field;
      if (!requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(requestContext, referenceName, out field, new bool?(true)))
        throw new ProcessFieldCouldNotBeFoundException(referenceName);
      foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext))
      {
        if (processDescriptor.IsDerived && !processDescriptor.IsDeleted)
          this.RemoveFieldFromAllWorkItemTypes(requestContext, processDescriptor.TypeId, field);
      }
    }

    private void RemoveFieldFromAllWorkItemTypes(
      IVssRequestContext requestContext,
      Guid processId,
      FieldEntry fieldEntry)
    {
      IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
      IEnumerable<ProcessWorkItemType> source = service.GetTypelets<ProcessWorkItemType>(requestContext, processId).Where<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => t.Fields != null));
      bool suppressPermissionCheck = true;
      Func<ProcessWorkItemType, bool> predicate = (Func<ProcessWorkItemType, bool>) (t => t.Fields.Any<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.LocalReferenceName, fieldEntry.ReferenceName))));
      foreach (ProcessWorkItemType processWorkItemType in source.Where<ProcessWorkItemType>(predicate))
      {
        requestContext.GetService<IFormLayoutService>().RemoveFieldFromLayout(requestContext, processId, processWorkItemType.ReferenceName, fieldEntry.ReferenceName, suppressPermissionCheck);
        service.RemoveWorkItemTypeField(requestContext, processId, processWorkItemType.ReferenceName, fieldEntry.ReferenceName, suppressPermissionCheck);
      }
    }

    private void CheckFieldCountLimit(IVssRequestContext requestContext)
    {
      int fieldLimitCount = this.GetFieldLimitCount(requestContext);
      if (requestContext.GetService<WorkItemTrackingFieldService>().GetAllFields(requestContext).Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.Usage != InternalFieldUsages.WorkItemTypeExtension)).Count<FieldEntry>() >= fieldLimitCount)
        throw new FieldLimitExceededException(fieldLimitCount);
    }

    protected virtual int GetFieldLimitCount(IVssRequestContext requestContext) => requestContext.WitContext().TemplateValidatorConfiguration.MaxFieldsPerCollection;

    public bool IsSystemField(string fieldRefName) => TFStringComparer.WorkItemFieldReferenceName.StartsWith(fieldRefName, ProcessFieldService.SystemFieldPrefix);

    public bool IsOOBField(IVssRequestContext requestContext, string name, string fieldRefName) => this.GetAllOutOfBoxFieldDefinitions(requestContext).Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, fieldRefName) || TFStringComparer.WorkItemFieldFriendlyName.Equals(f.Name, name)));

    private string CreateUniqueReferenceName(
      IVssRequestContext requestContext,
      string firstPart,
      string secondPart)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(firstPart, nameof (firstPart));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(secondPart, nameof (secondPart));
      string uniqueReferenceName = CommonWITUtils.GenerateReferenceName(firstPart, secondPart);
      try
      {
        CommonWITUtils.CheckValidName(uniqueReferenceName, 260, Array.Empty<char>());
        if (this.IsFieldReferenceNameInUse(requestContext, uniqueReferenceName))
          uniqueReferenceName = CommonWITUtils.GenerateUniqueRefName();
      }
      catch (ArgumentException ex)
      {
        uniqueReferenceName = CommonWITUtils.GenerateUniqueRefName();
      }
      return uniqueReferenceName;
    }

    private void ValidateReferenceName(IVssRequestContext requestContext, string referenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(referenceName, nameof (referenceName));
      CommonWITUtils.CheckValidName(referenceName, 260, Array.Empty<char>());
      if (this.IsFieldReferenceNameInUse(requestContext, referenceName))
        throw new ProcessFieldAlreadyExistsException();
    }

    private bool IsFieldNameInUse(IVssRequestContext requestContext, string name) => this.IsFieldInUse(requestContext, name, TFStringComparer.WorkItemFieldFriendlyName);

    private bool IsFieldReferenceNameInUse(IVssRequestContext requestContext, string referenceName) => this.IsFieldInUse(requestContext, referenceName, TFStringComparer.WorkItemFieldReferenceName);

    private bool IsFieldInUse(
      IVssRequestContext requestContext,
      string name,
      VssStringComparer comparer)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      ArgumentUtility.CheckForNull<VssStringComparer>(comparer, nameof (comparer));
      return requestContext.GetService<WorkItemTrackingFieldService>().TryGetField(requestContext, name, out FieldEntry _, new bool?(true)) | this.GetAllOutOfBoxFieldDefinitions(requestContext).Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => comparer.Equals(f.ReferenceName, name)));
    }

    private IReadOnlyCollection<ProcessFieldResult> GetFieldsFromStorage(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor)
    {
      Guid typeId = processDescriptor.TypeId;
      return processDescriptor.IsDerived ? this.GetDataFromNewStorage(requestContext, typeId) : (IReadOnlyCollection<ProcessFieldResult>) this.GetAllOutOfBoxFieldDefinitions(requestContext).Select<ProcessFieldDefinition, ProcessFieldResult>((Func<ProcessFieldDefinition, ProcessFieldResult>) (f => f.ConvertToFieldResult())).ToList<ProcessFieldResult>();
    }

    private IReadOnlyCollection<ProcessFieldResult> GetDataFromNewStorage(
      IVssRequestContext requestContext,
      Guid processId)
    {
      return (IReadOnlyCollection<ProcessFieldResult>) requestContext.TraceBlock<List<ProcessFieldResult>>(909989, 909990, "Field", nameof (ProcessFieldService), nameof (GetDataFromNewStorage), (Func<List<ProcessFieldResult>>) (() => requestContext.GetService<WorkItemTrackingFieldService>().GetAllFields(requestContext).Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.ProcessId == processId)).Select<FieldEntry, ProcessFieldResult>((Func<FieldEntry, ProcessFieldResult>) (f => new ProcessFieldResult()
      {
        Name = f.Name,
        ReferenceName = f.ReferenceName,
        Type = f.FieldType,
        Description = f.Description,
        PickListId = f.PickListId,
        IsIdentity = f.IsIdentity
      })).ToList<ProcessFieldResult>()));
    }

    private static WorkItemPickListType CheckAndGetPickListType(FieldEntry fieldEntry)
    {
      switch (fieldEntry.FieldType)
      {
        case InternalFieldType.String:
          return WorkItemPickListType.String;
        case InternalFieldType.Integer:
          return WorkItemPickListType.Integer;
        default:
          throw new InvalidFieldTypeForConversionToPicklistException(fieldEntry.ReferenceName);
      }
    }

    private void ValidatePicklist(
      IVssRequestContext requestContext,
      FieldDBType dbType,
      Guid picklistId)
    {
      WorkItemPickList list = requestContext.GetService<IWorkItemPickListService>().GetList(requestContext, picklistId);
      switch (dbType)
      {
        case FieldDBType.Keyword:
          if (list.Type == WorkItemPickListType.String)
            return;
          break;
        case FieldDBType.Integer:
          if (list.Type == WorkItemPickListType.Integer)
            return;
          break;
        case FieldDBType.Double:
          if (list.Type == WorkItemPickListType.Double)
            return;
          break;
      }
      throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidPicklistTypeForField((object) list.Type, (object) dbType));
    }

    private void CheckFieldCreatePermission(IVssRequestContext requestContext)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      if (!service.HasProcessPermission(requestContext, 1, checkDescriptorScope: false))
      {
        foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) service.GetProcessDescriptors(requestContext))
        {
          if (service.HasProcessPermission(requestContext, 1, processDescriptor))
            return;
        }
        throw new ProcessPermissionException();
      }
    }

    private void CheckCollectionLevelFieldEditPermission(
      IVssRequestContext requestContext,
      string fieldReferenceName)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      if (!service.HasProcessPermission(requestContext, 1, checkDescriptorScope: false))
      {
        foreach (ProcessDescriptor descriptor in service.GetProcessDescriptors(requestContext).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (d => !d.IsDeleted && d.IsDerived)))
        {
          if (service.HasProcessPermission(requestContext, 1, descriptor))
            return;
        }
        throw new ProcessPermissionException();
      }
    }

    public FieldEntry RestoreField(IVssRequestContext requestContext, string fieldNameOrRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fieldNameOrRefName, nameof (fieldNameOrRefName));
      requestContext.TraceEnter(910003, "Field", nameof (ProcessFieldService), nameof (RestoreField));
      if (!requestContext.GetService<IWorkItemTrackingProcessService>().HasDeleteFieldPermission(requestContext))
        this.ThrowAccessDeniedException(requestContext);
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      if (service.TryGetField(requestContext, fieldNameOrRefName, out FieldEntry _))
        throw new FieldHasNotBeenDeletedException();
      FieldEntry fieldEntry = service.RestoreField(requestContext, service.GetFieldEntries(requestContext, 0L, out long _, includeDeleted: true).FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, string.Format("{0}{1}{2}", (object) "*Del", (object) f.FieldId, (object) fieldNameOrRefName)) || TFStringComparer.WorkItemFieldFriendlyName.Equals(f.Name, string.Format("{0}{1}{2}", (object) ".Del", (object) f.FieldId, (object) fieldNameOrRefName)))) ?? throw new ProcessFieldCouldNotBeFoundException(fieldNameOrRefName));
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("RestoreCustomField", fieldNameOrRefName);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessFieldService), nameof (RestoreField), properties);
      requestContext.TraceLeave(910004, "Field", nameof (ProcessFieldService), nameof (RestoreField));
      return fieldEntry;
    }

    public FieldEntry SetFieldLocked(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      bool isLocked)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fieldNameOrRefName, nameof (fieldNameOrRefName));
      requestContext.TraceEnter(910005, "Field", nameof (ProcessFieldService), nameof (SetFieldLocked));
      this.VerifyLockingisAllowed(requestContext);
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      FieldEntry field = service.GetField(requestContext, fieldNameOrRefName);
      FieldEntry fieldEntry = service.SetFieldLocked(requestContext, field, isLocked);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("SetLockedCustomField", fieldNameOrRefName);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessFieldService), nameof (SetFieldLocked), properties);
      requestContext.TraceLeave(910006, "Field", nameof (ProcessFieldService), nameof (SetFieldLocked));
      return fieldEntry;
    }

    private void VerifyLockingisAllowed(IVssRequestContext requestContext)
    {
      if (requestContext.GetService<IWorkItemTrackingProcessService>().HasDeleteFieldPermission(requestContext))
        return;
      this.ThrowAccessDeniedException(requestContext);
    }
  }
}
