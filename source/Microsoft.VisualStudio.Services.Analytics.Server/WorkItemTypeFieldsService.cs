// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemTypeFieldsService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class WorkItemTypeFieldsService : IWorkItemTypeFieldsService, IVssFrameworkService
  {
    private static readonly IReadOnlyCollection<FieldInfo> AllowedSharedAnalyticsOnlyFields = (IReadOnlyCollection<FieldInfo>) new List<FieldInfo>()
    {
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = false,
        PropertyName = "LeadTimeDays",
        Type = "Edm.Double"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = false,
        PropertyName = "CycleTimeDays",
        Type = "Edm.Double"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = false,
        PropertyName = "CompletedDate",
        Type = "Edm.DateTimeOffset"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = false,
        PropertyName = "StateCategory",
        wrapInQuotes = true,
        Type = "Edm.String"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = false,
        PropertyName = "ParentWorkItemId",
        Type = "Edm.Int32"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsDefaultField = true,
        IsExpansion = true,
        wrapInQuotes = true,
        PropertyName = "Area/AreaPath",
        Type = "Edm.String"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsDefaultField = true,
        IsExpansion = true,
        wrapInQuotes = true,
        PropertyName = "Iteration/IterationPath",
        Type = "Edm.String"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = true,
        PropertyName = "Iteration/StartDate",
        Type = "Edm.DateTimeOffset"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = true,
        PropertyName = "Iteration/EndDate",
        Type = "Edm.DateTimeOffset"
      },
      new FieldInfo()
      {
        IsCommonField = true,
        IsExpansion = true,
        wrapInQuotes = true,
        PropertyName = "Project/ProjectName",
        Type = "Edm.String"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        PropertyName = "AnalyticsUpdatedDate",
        Type = "Edm.DateTimeOffset"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        PropertyName = "InProgressDate",
        Type = "Edm.DateTimeOffset"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ProjectSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "AreaSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "AssignedToUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ChangedByUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "CreatedByUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ActivatedByUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ClosedByUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ResolvedByUserSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ActivatedDateSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ChangedDateSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "CreatedDateSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ClosedDateSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "ResolvedDateSK",
        Type = "Edm.Guid"
      }
    };
    private static readonly IReadOnlyCollection<FieldInfo> AllowedHistoricalAnalyticsOnlyFields = (IReadOnlyCollection<FieldInfo>) new List<FieldInfo>()
    {
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "DateSK",
        Type = "Edm.Guid"
      },
      new FieldInfo()
      {
        IsCommonField = false,
        IsExpansion = false,
        DisallowFiltering = true,
        PropertyName = "RevisedDateSK",
        Type = "Edm.Guid"
      }
    };
    private static readonly IReadOnlyCollection<string> OpsFieldsNotAllowedReferenceNames = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "System.AttachedFileCount",
      "System.ExternalLinkCount",
      "System.RelatedLinkCount",
      "System.HyperLinkCount",
      "System.Watermark",
      "System.BoardColumn",
      "System.BoardLane",
      "System.BoardColumnDone",
      "System.History",
      "Microsoft.VSTS.CMMI.MeetingType"
    };
    private static readonly IReadOnlyCollection<string> OpsFieldTypesNotAllowed = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "Html"
    };
    private static readonly IReadOnlyCollection<string> OpsFieldReferenceNamesNotAllowedInCommonFields = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "System.Rev"
    };
    private static readonly IReadOnlyCollection<string> AnalyticsServiceNavigationPropertyNamesNotAllowed = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "Microsoft_VSTS_CMMI_CalledBy",
      "Microsoft_VSTS_CMMI_RequiredAttendee1",
      "Microsoft_VSTS_CMMI_RequiredAttendee2",
      "Microsoft_VSTS_CMMI_RequiredAttendee3",
      "Microsoft_VSTS_CMMI_RequiredAttendee4",
      "Microsoft_VSTS_CMMI_RequiredAttendee5",
      "Microsoft_VSTS_CMMI_RequiredAttendee6",
      "Microsoft_VSTS_CMMI_RequiredAttendee7",
      "Microsoft_VSTS_CMMI_RequiredAttendee8",
      "Microsoft_VSTS_CMMI_OptionalAttendee1",
      "Microsoft_VSTS_CMMI_OptionalAttendee2",
      "Microsoft_VSTS_CMMI_OptionalAttendee3",
      "Microsoft_VSTS_CMMI_OptionalAttendee4",
      "Microsoft_VSTS_CMMI_OptionalAttendee5",
      "Microsoft_VSTS_CMMI_OptionalAttendee6",
      "Microsoft_VSTS_CMMI_OptionalAttendee7",
      "Microsoft_VSTS_CMMI_OptionalAttendee8",
      "Microsoft_VSTS_CMMI_ActualAttendee1",
      "Microsoft_VSTS_CMMI_ActualAttendee2",
      "Microsoft_VSTS_CMMI_ActualAttendee3",
      "Microsoft_VSTS_CMMI_ActualAttendee4",
      "Microsoft_VSTS_CMMI_ActualAttendee5",
      "Microsoft_VSTS_CMMI_ActualAttendee6",
      "Microsoft_VSTS_CMMI_ActualAttendee7",
      "Microsoft_VSTS_CMMI_ActualAttendee8",
      "Date"
    };
    private static readonly IReadOnlyCollection<string> DefaultCommonPropertyNames = WorkItemsDefinitions.DefaultCommonFields;

    private string GetReferenceName(IEdmProperty property, IEdmModel model) => !(property.VocabularyAnnotations(model).FirstOrDefault<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.Term.Name == "ReferenceName"))?.Value is IEdmStringConstantExpression constantExpression) ? (string) null : constantExpression.Value;

    private List<FieldInfo> GetAllowedNavigationPropertyFields(
      IEdmEntityType entityType,
      IList<WorkItemTypeField> fields,
      IEdmModel model,
      Type dataType,
      string propertyName,
      bool wrapInQuotes,
      bool disallowFiltering)
    {
      List<FieldInfo> navigationPropertyFields = new List<FieldInfo>();
      foreach (IEdmNavigationProperty property in entityType.NavigationProperties().Where<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (prop => prop.Type.FullName() == dataType.FullName)))
      {
        string refName = this.GetReferenceName((IEdmProperty) property, model);
        if (!WorkItemTypeFieldsService.AnalyticsServiceNavigationPropertyNamesNotAllowed.Contains<string>(property.Name) && (refName == null || fields.Any<WorkItemTypeField>((Func<WorkItemTypeField, bool>) (field => field.FieldReferenceName == refName))))
          navigationPropertyFields.Add(new FieldInfo()
          {
            IsCommonField = true,
            IsDefaultField = WorkItemTypeFieldsService.DefaultCommonPropertyNames.Contains<string>(property.Name + "/" + propertyName),
            IsExpansion = true,
            PropertyName = property.Name + "/" + propertyName,
            wrapInQuotes = wrapInQuotes,
            Type = property.Type.Definition.ToString(),
            DisallowFiltering = disallowFiltering
          });
      }
      return navigationPropertyFields;
    }

    private bool IsOpsFieldAllowed(WorkItemTypeField field) => !WorkItemTypeFieldsService.OpsFieldTypesNotAllowed.Contains<string>(field.FieldType) && !WorkItemTypeFieldsService.OpsFieldsNotAllowedReferenceNames.Contains<string>(field.FieldReferenceName);

    private IEdmStructuralProperty FindMatchingStructuralProperty(
      IEdmEntityType workItemsEntityType,
      WorkItemTypeField field,
      IEdmModel edmModel)
    {
      foreach (IEdmStructuralProperty structuralProperty in workItemsEntityType.StructuralProperties())
      {
        string referenceName = this.GetReferenceName((IEdmProperty) structuralProperty, edmModel);
        if (field.FieldReferenceName.Equals(referenceName))
          return structuralProperty;
      }
      return (IEdmStructuralProperty) null;
    }

    private List<FieldInfo> GetFields(
      IVssRequestContext requestContext,
      GetWorkItemTypeFieldsOptions getWorkItemTypeFieldOptions)
    {
      List<FieldInfo> fieldInfos = new List<FieldInfo>((IEnumerable<FieldInfo>) WorkItemTypeFieldsService.AllowedSharedAnalyticsOnlyFields);
      if (getWorkItemTypeFieldOptions.EntitySet == AnalyticsModelBuilder.s_clrTypeToEntitySetName[typeof (WorkItemRevision)])
        fieldInfos.AddRange((IEnumerable<FieldInfo>) WorkItemTypeFieldsService.AllowedHistoricalAnalyticsOnlyFields);
      List<WorkItemTypeField> fields = (List<WorkItemTypeField>) null;
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
      {
        List<string> list = getWorkItemTypeFieldOptions.WorkItemTypes.Distinct<string>().ToList<string>();
        fields = component.GetWorkItemTypeFields((IList<Guid>) getWorkItemTypeFieldOptions.ProjectIds, (IList<string>) list);
      }
      IEdmModel edmModel = requestContext.GetService<IAnalyticsViewsEdmModelService>().GetEdmModel(requestContext, getWorkItemTypeFieldOptions.ModelVersion);
      IEdmEntityType entityType = edmModel.EntityContainer.FindEntitySet(getWorkItemTypeFieldOptions.EntitySet).EntityType();
      this.GetAllowedNavigationPropertyFields(entityType, (IList<WorkItemTypeField>) fields, edmModel, typeof (User), "UserName", true, true).ForEach((Action<FieldInfo>) (f => fieldInfos.Add(f)));
      if (fields != null)
      {
        HashSet<string> opsStoreFieldIsAlreadyEvaluated = new HashSet<string>();
        fields.ForEach((Action<WorkItemTypeField>) (opsStoreField =>
        {
          if (!opsStoreFieldIsAlreadyEvaluated.Contains(opsStoreField.FieldReferenceName) && this.IsOpsFieldAllowed(opsStoreField))
          {
            IEdmStructuralProperty structuralProperty = this.FindMatchingStructuralProperty(entityType, opsStoreField, edmModel);
            if (structuralProperty != null)
              fieldInfos.Add(new FieldInfo()
              {
                IsCommonField = !WorkItemTypeFieldsService.OpsFieldReferenceNamesNotAllowedInCommonFields.Contains<string>(opsStoreField.FieldReferenceName),
                IsDefaultField = WorkItemTypeFieldsService.DefaultCommonPropertyNames.Contains<string>(structuralProperty.Name),
                PropertyName = structuralProperty.Name,
                wrapInQuotes = structuralProperty.Type.IsString(),
                Type = structuralProperty.Type.Definition.ToString()
              });
          }
          opsStoreFieldIsAlreadyEvaluated.Add(opsStoreField.FieldReferenceName);
        }));
      }
      return fieldInfos;
    }

    private void AddDisplayNames(
      IVssRequestContext requestContext,
      GetWorkItemTypeFieldsOptions getWorkItemTypeFieldOptions,
      IList<FieldInfo> fieldInfos)
    {
      IEnumerable<FieldInfo> source = fieldInfos.Where<FieldInfo>((Func<FieldInfo, bool>) (info => info.DisplayName == null));
      IEnumerable<string> values1 = source.Where<FieldInfo>((Func<FieldInfo, bool>) (info => !info.IsExpansion)).Select<FieldInfo, string>((Func<FieldInfo, string>) (info => info.PropertyName));
      IEnumerable<string> values2 = source.Where<FieldInfo>((Func<FieldInfo, bool>) (info => info.IsExpansion)).Select<FieldInfo, string[]>((Func<FieldInfo, string[]>) (info => info.PropertyName.Split('/'))).Select(splitName => new
      {
        NavigationProperty = splitName[0],
        SelectProperty = splitName[1]
      }).Select(info => info.NavigationProperty + "($select=" + info.SelectProperty + ")");
      string str = "$select=" + string.Join(",", values1) + "&$expand=" + string.Join(",", values2);
      ODataJsonSchema odataJsonSchema1 = ODataJsonSchemaGeneratorFactory.GetSchemaGenerator(requestContext, getWorkItemTypeFieldOptions.ModelVersion, AnalyticsViewType.WorkItems, getWorkItemTypeFieldOptions.EntitySet, str, str).Generate();
      foreach (FieldInfo fieldInfo in source)
      {
        FieldInfo info = fieldInfo;
        if (info.IsExpansion)
        {
          string[] strArray = info.PropertyName.Split('/');
          string navigationProperty = strArray[0];
          string selectProperty = strArray[1];
          ODataJsonSchema odataJsonSchema2 = odataJsonSchema1.Items.Properties.First<KeyValuePair<string, ODataJsonSchema>>((Func<KeyValuePair<string, ODataJsonSchema>, bool>) (prop => prop.Key == navigationProperty)).Value;
          ODataJsonSchema odataJsonSchema3 = odataJsonSchema2.Properties.First<KeyValuePair<string, ODataJsonSchema>>((Func<KeyValuePair<string, ODataJsonSchema>, bool>) (prop => prop.Key == selectProperty)).Value;
          info.DisplayName = string.IsNullOrWhiteSpace(odataJsonSchema2.DisplayNameAnnotation) ? odataJsonSchema3.DisplayNameAnnotation : odataJsonSchema2.DisplayNameAnnotation + " " + odataJsonSchema3.DisplayNameAnnotation;
        }
        else
        {
          ODataJsonSchema odataJsonSchema4 = odataJsonSchema1.Items.Properties.First<KeyValuePair<string, ODataJsonSchema>>((Func<KeyValuePair<string, ODataJsonSchema>, bool>) (prop => prop.Key == info.PropertyName)).Value;
          info.DisplayName = odataJsonSchema4.DisplayNameAnnotation;
        }
      }
    }

    public List<FieldInfo> GetWorkItemTypeFields(
      IVssRequestContext requestContext,
      GetWorkItemTypeFieldsOptions getWorkItemTypeFieldOptions)
    {
      List<FieldInfo> fields = this.GetFields(requestContext, getWorkItemTypeFieldOptions);
      this.AddDisplayNames(requestContext, getWorkItemTypeFieldOptions, (IList<FieldInfo>) fields);
      return fields;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
