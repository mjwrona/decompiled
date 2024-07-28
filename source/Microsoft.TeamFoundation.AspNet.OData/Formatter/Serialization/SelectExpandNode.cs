// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.SelectExpandNode
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class SelectExpandNode
  {
    public SelectExpandNode()
    {
    }

    public SelectExpandNode(SelectExpandNode selectExpandNodeToCopy)
    {
      this.SelectedStructuralProperties = selectExpandNodeToCopy.SelectedStructuralProperties == null ? (ISet<IEdmStructuralProperty>) null : (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>((IEnumerable<IEdmStructuralProperty>) selectExpandNodeToCopy.SelectedStructuralProperties);
      this.SelectedComplexTypeProperties = selectExpandNodeToCopy.SelectedComplexTypeProperties == null ? (IDictionary<IEdmStructuralProperty, PathSelectItem>) null : (IDictionary<IEdmStructuralProperty, PathSelectItem>) new Dictionary<IEdmStructuralProperty, PathSelectItem>(selectExpandNodeToCopy.SelectedComplexTypeProperties);
      this.SelectedNavigationProperties = selectExpandNodeToCopy.SelectedNavigationProperties == null ? (ISet<IEdmNavigationProperty>) null : (ISet<IEdmNavigationProperty>) new HashSet<IEdmNavigationProperty>((IEnumerable<IEdmNavigationProperty>) selectExpandNodeToCopy.SelectedNavigationProperties);
      this.ExpandedProperties = selectExpandNodeToCopy.ExpandedProperties == null ? (IDictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem>) null : (IDictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem>) new Dictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem>(selectExpandNodeToCopy.ExpandedProperties);
      this.ReferencedProperties = selectExpandNodeToCopy.ReferencedProperties == null ? (IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>) null : (IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>) new Dictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>();
      this.SelectAllDynamicProperties = selectExpandNodeToCopy.SelectAllDynamicProperties;
      this.SelectedDynamicProperties = selectExpandNodeToCopy.SelectedDynamicProperties == null ? (ISet<string>) null : (ISet<string>) new HashSet<string>((IEnumerable<string>) selectExpandNodeToCopy.SelectedDynamicProperties);
      this.SelectedActions = selectExpandNodeToCopy.SelectedActions == null ? (ISet<IEdmAction>) null : (ISet<IEdmAction>) new HashSet<IEdmAction>((IEnumerable<IEdmAction>) selectExpandNodeToCopy.SelectedActions);
      this.SelectedFunctions = selectExpandNodeToCopy.SelectedFunctions == null ? (ISet<IEdmFunction>) null : (ISet<IEdmFunction>) new HashSet<IEdmFunction>((IEnumerable<IEdmFunction>) selectExpandNodeToCopy.SelectedFunctions);
    }

    public SelectExpandNode(IEdmStructuredType structuredType, ODataSerializerContext writeContext)
      : this()
    {
      this.Initialize(writeContext.SelectExpandClause, structuredType, writeContext.Model, writeContext.ExpandReference);
    }

    public SelectExpandNode(
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType,
      IEdmModel model)
      : this()
    {
      this.Initialize(selectExpandClause, structuredType, model, false);
    }

    [Obsolete("This property will be removed later, please use ReferencedProperties.")]
    public ISet<IEdmNavigationProperty> ReferencedNavigationProperties => this.ReferencedProperties == null ? (ISet<IEdmNavigationProperty>) null : (ISet<IEdmNavigationProperty>) new HashSet<IEdmNavigationProperty>((IEnumerable<IEdmNavigationProperty>) this.ReferencedProperties.Keys);

    [Obsolete("This property will be removed later, please use SelectedComplexTypeProperties.")]
    public ISet<IEdmStructuralProperty> SelectedComplexProperties => this.SelectedComplexTypeProperties == null ? (ISet<IEdmStructuralProperty>) null : (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>((IEnumerable<IEdmStructuralProperty>) this.SelectedComplexTypeProperties.Keys);

    public ISet<IEdmStructuralProperty> SelectedStructuralProperties { get; internal set; }

    public ISet<IEdmNavigationProperty> SelectedNavigationProperties { get; internal set; }

    public IDictionary<IEdmStructuralProperty, PathSelectItem> SelectedComplexTypeProperties { get; internal set; }

    public IDictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem> ExpandedProperties { get; internal set; }

    public IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> ReferencedProperties { get; internal set; }

    public ISet<string> SelectedDynamicProperties { get; internal set; }

    public bool SelectAllDynamicProperties { get; internal set; }

    public ISet<IEdmAction> SelectedActions { get; internal set; }

    public ISet<IEdmFunction> SelectedFunctions { get; internal set; }

    private void Initialize(
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType,
      IEdmModel model,
      bool expandedReference)
    {
      if (structuredType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuredType));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      IEdmEntityType type = structuredType as IEdmEntityType;
      if (expandedReference)
      {
        this.SelectAllDynamicProperties = false;
        if (type == null)
          return;
        this.SelectedStructuralProperties = (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>(type.Key());
      }
      else
      {
        SelectExpandNode.EdmStructuralTypeInfo structuralTypeInfo = new SelectExpandNode.EdmStructuralTypeInfo(model, structuredType);
        if (selectExpandClause == null)
        {
          this.SelectAllDynamicProperties = true;
          this.SelectedNavigationProperties = structuralTypeInfo.AllNavigationProperties;
          this.SelectedActions = structuralTypeInfo.AllActions;
          this.SelectedFunctions = structuralTypeInfo.AllFunctions;
          if (structuralTypeInfo.AllStructuralProperties != null)
          {
            foreach (IEdmStructuralProperty structuralProperty in (IEnumerable<IEdmStructuralProperty>) structuralTypeInfo.AllStructuralProperties)
              this.AddStructuralProperty(structuralProperty, (PathSelectItem) null);
          }
        }
        else
          this.BuildSelectExpand(selectExpandClause, structuralTypeInfo);
        this.AdjustSelectNavigationProperties();
      }
    }

    private void BuildSelectExpand(
      SelectExpandClause selectExpandClause,
      SelectExpandNode.EdmStructuralTypeInfo structuralTypeInfo)
    {
      Dictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude = new Dictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>();
      this.SelectAllDynamicProperties = false;
      foreach (SelectItem selectedItem in selectExpandClause.SelectedItems)
      {
        switch (selectedItem)
        {
          case ExpandedReferenceSelectItem expandReferenceItem:
            this.BuildExpandItem(expandReferenceItem, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) currentLevelPropertiesInclude, structuralTypeInfo);
            continue;
          case PathSelectItem pathSelectItem:
            this.BuildSelectItem(pathSelectItem, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) currentLevelPropertiesInclude, structuralTypeInfo);
            continue;
          case WildcardSelectItem _:
            SelectExpandNode.MergeAllStructuralProperties(structuralTypeInfo.AllStructuralProperties, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) currentLevelPropertiesInclude);
            this.MergeSelectedNavigationProperties(structuralTypeInfo.AllNavigationProperties);
            this.SelectAllDynamicProperties = true;
            continue;
          case NamespaceQualifiedWildcardSelectItem namespaceSelectItem:
            this.AddNamespaceWildcardOperation(namespaceSelectItem, structuralTypeInfo.AllActions, structuralTypeInfo.AllFunctions);
            continue;
          default:
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SelectionTypeNotSupported, (object) selectedItem.GetType().Name));
        }
      }
      if (selectExpandClause.AllSelected)
      {
        SelectExpandNode.MergeAllStructuralProperties(structuralTypeInfo.AllStructuralProperties, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) currentLevelPropertiesInclude);
        this.MergeSelectedNavigationProperties(structuralTypeInfo.AllNavigationProperties);
        this.MergeSelectedAction(structuralTypeInfo.AllActions);
        this.MergeSelectedFunction(structuralTypeInfo.AllFunctions);
        this.SelectAllDynamicProperties = true;
      }
      if (structuralTypeInfo.AllStructuralProperties == null)
        return;
      foreach (IEdmStructuralProperty structuralProperty in (IEnumerable<IEdmStructuralProperty>) structuralTypeInfo.AllStructuralProperties)
      {
        SelectExpandIncludedProperty includedProperty;
        if (currentLevelPropertiesInclude.TryGetValue(structuralProperty, out includedProperty))
        {
          PathSelectItem pathSelectItem = includedProperty == null ? (PathSelectItem) null : includedProperty.ToPathSelectItem();
          this.AddStructuralProperty(structuralProperty, pathSelectItem);
        }
      }
    }

    private void BuildExpandItem(
      ExpandedReferenceSelectItem expandReferenceItem,
      IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude,
      SelectExpandNode.EdmStructuralTypeInfo structuralTypeInfo)
    {
      IList<ODataPathSegment> remainingSegments;
      ODataPathSegment nonTypeCastSegment = expandReferenceItem.PathToNavigationProperty.GetFirstNonTypeCastSegment(out remainingSegments);
      if (nonTypeCastSegment is PropertySegment propertySegment)
      {
        if (!structuralTypeInfo.IsStructuralPropertyDefined(propertySegment.Property))
          return;
        SelectExpandIncludedProperty includedProperty;
        if (!currentLevelPropertiesInclude.TryGetValue(propertySegment.Property, out includedProperty))
        {
          includedProperty = new SelectExpandIncludedProperty(propertySegment);
          currentLevelPropertiesInclude[propertySegment.Property] = includedProperty;
        }
        includedProperty.AddSubExpandItem(remainingSegments, expandReferenceItem);
      }
      else
      {
        NavigationPropertySegment navigationPropertySegment = nonTypeCastSegment as NavigationPropertySegment;
        if (!structuralTypeInfo.IsNavigationPropertyDefined(navigationPropertySegment.NavigationProperty))
          return;
        if (expandReferenceItem is ExpandedNavigationSelectItem navigationSelectItem)
        {
          if (this.ExpandedProperties == null)
            this.ExpandedProperties = (IDictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem>) new Dictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem>();
          this.ExpandedProperties[navigationPropertySegment.NavigationProperty] = navigationSelectItem;
        }
        else
        {
          if (this.ReferencedProperties == null)
            this.ReferencedProperties = (IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>) new Dictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>();
          this.ReferencedProperties[navigationPropertySegment.NavigationProperty] = expandReferenceItem;
        }
      }
    }

    private void BuildSelectItem(
      PathSelectItem pathSelectItem,
      IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude,
      SelectExpandNode.EdmStructuralTypeInfo structuralTypeInfo)
    {
      IList<ODataPathSegment> remainingSegments;
      ODataPathSegment nonTypeCastSegment = pathSelectItem.SelectedPath.GetFirstNonTypeCastSegment(out remainingSegments);
      switch (nonTypeCastSegment)
      {
        case PropertySegment propertySegment:
          if (!structuralTypeInfo.IsStructuralPropertyDefined(propertySegment.Property))
            break;
          SelectExpandIncludedProperty includedProperty;
          if (!currentLevelPropertiesInclude.TryGetValue(propertySegment.Property, out includedProperty))
          {
            includedProperty = new SelectExpandIncludedProperty(propertySegment);
            currentLevelPropertiesInclude[propertySegment.Property] = includedProperty;
          }
          includedProperty.AddSubSelectItem(remainingSegments, pathSelectItem);
          break;
        case NavigationPropertySegment navigationPropertySegment:
          if (!structuralTypeInfo.IsNavigationPropertyDefined(navigationPropertySegment.NavigationProperty))
            break;
          if (this.SelectedNavigationProperties == null)
            this.SelectedNavigationProperties = (ISet<IEdmNavigationProperty>) new HashSet<IEdmNavigationProperty>();
          this.SelectedNavigationProperties.Add(navigationPropertySegment.NavigationProperty);
          break;
        case OperationSegment operationSegment:
          this.AddOperations(operationSegment, structuralTypeInfo.AllActions, structuralTypeInfo.AllFunctions);
          break;
        case DynamicPathSegment dynamicPathSegment:
          if (this.SelectedDynamicProperties == null)
            this.SelectedDynamicProperties = (ISet<string>) new HashSet<string>();
          this.SelectedDynamicProperties.Add(dynamicPathSegment.Identifier);
          break;
        default:
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SelectionTypeNotSupported, (object) nonTypeCastSegment.GetType().Name));
      }
    }

    private static void MergeAllStructuralProperties(
      ISet<IEdmStructuralProperty> allStructuralProperties,
      IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude)
    {
      if (allStructuralProperties == null)
        return;
      foreach (IEdmStructuralProperty structuralProperty in (IEnumerable<IEdmStructuralProperty>) allStructuralProperties)
      {
        if (!currentLevelPropertiesInclude.ContainsKey(structuralProperty))
          currentLevelPropertiesInclude[structuralProperty] = (SelectExpandIncludedProperty) null;
      }
    }

    private void MergeSelectedNavigationProperties(
      ISet<IEdmNavigationProperty> allNavigationProperties)
    {
      if (allNavigationProperties == null)
        return;
      if (this.SelectedNavigationProperties == null)
        this.SelectedNavigationProperties = allNavigationProperties;
      else
        this.SelectedNavigationProperties.UnionWith((IEnumerable<IEdmNavigationProperty>) allNavigationProperties);
    }

    private void MergeSelectedAction(ISet<IEdmAction> allActions)
    {
      if (allActions == null)
        return;
      if (this.SelectedActions == null)
        this.SelectedActions = allActions;
      else
        this.SelectedActions.UnionWith((IEnumerable<IEdmAction>) allActions);
    }

    private void MergeSelectedFunction(ISet<IEdmFunction> allFunctions)
    {
      if (allFunctions == null)
        return;
      if (this.SelectedFunctions == null)
        this.SelectedFunctions = allFunctions;
      else
        this.SelectedFunctions.UnionWith((IEnumerable<IEdmFunction>) allFunctions);
    }

    private void AddStructuralProperty(
      IEdmStructuralProperty structuralProperty,
      PathSelectItem pathSelectItem)
    {
      if (SelectExpandNode.IsComplexOrCollectionComplex(structuralProperty))
      {
        if (this.SelectedComplexTypeProperties == null)
          this.SelectedComplexTypeProperties = (IDictionary<IEdmStructuralProperty, PathSelectItem>) new Dictionary<IEdmStructuralProperty, PathSelectItem>();
        this.SelectedComplexTypeProperties[structuralProperty] = pathSelectItem;
      }
      else
      {
        if (this.SelectedStructuralProperties == null)
          this.SelectedStructuralProperties = (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>();
        this.SelectedStructuralProperties.Add(structuralProperty);
      }
    }

    private void AddNamespaceWildcardOperation(
      NamespaceQualifiedWildcardSelectItem namespaceSelectItem,
      ISet<IEdmAction> allActions,
      ISet<IEdmFunction> allFunctions)
    {
      this.SelectedActions = allActions != null ? (ISet<IEdmAction>) new HashSet<IEdmAction>(allActions.Where<IEdmAction>((Func<IEdmAction, bool>) (a => a.Namespace == namespaceSelectItem.Namespace))) : (ISet<IEdmAction>) null;
      if (allFunctions == null)
        this.SelectedFunctions = (ISet<IEdmFunction>) null;
      else
        this.SelectedFunctions = (ISet<IEdmFunction>) new HashSet<IEdmFunction>(allFunctions.Where<IEdmFunction>((Func<IEdmFunction, bool>) (a => a.Namespace == namespaceSelectItem.Namespace)));
    }

    private void AddOperations(
      OperationSegment operationSegment,
      ISet<IEdmAction> allActions,
      ISet<IEdmFunction> allFunctions)
    {
      foreach (IEdmOperation operation in operationSegment.Operations)
      {
        if (operation is IEdmAction edmAction && allActions.Contains(edmAction))
        {
          if (this.SelectedActions == null)
            this.SelectedActions = (ISet<IEdmAction>) new HashSet<IEdmAction>();
          this.SelectedActions.Add(edmAction);
        }
        if (operation is IEdmFunction edmFunction && allFunctions.Contains(edmFunction))
        {
          if (this.SelectedFunctions == null)
            this.SelectedFunctions = (ISet<IEdmFunction>) new HashSet<IEdmFunction>();
          this.SelectedFunctions.Add(edmFunction);
        }
      }
    }

    private void AdjustSelectNavigationProperties()
    {
      if (this.SelectedNavigationProperties != null)
      {
        if (this.ExpandedProperties != null)
          this.SelectedNavigationProperties.ExceptWith((IEnumerable<IEdmNavigationProperty>) this.ExpandedProperties.Keys);
        if (this.ReferencedProperties != null)
          this.SelectedNavigationProperties.ExceptWith((IEnumerable<IEdmNavigationProperty>) this.ReferencedProperties.Keys);
      }
      if (this.SelectedNavigationProperties == null || this.SelectedNavigationProperties.Any<IEdmNavigationProperty>())
        return;
      this.SelectedNavigationProperties = (ISet<IEdmNavigationProperty>) null;
    }

    internal static bool IsComplexOrCollectionComplex(IEdmStructuralProperty structuralProperty) => structuralProperty != null && (structuralProperty.Type.IsComplex() || structuralProperty.Type.IsCollection() && structuralProperty.Type.AsCollection().ElementType().IsComplex());

    [Obsolete("This public method is not used anymore. It will be removed later.")]
    public static void GetStructuralProperties(
      IEdmStructuredType structuredType,
      HashSet<IEdmStructuralProperty> structuralProperties,
      HashSet<IEdmStructuralProperty> nestedStructuralProperties)
    {
      if (structuredType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuredType));
      if (structuralProperties == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralProperties));
      if (nestedStructuralProperties == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (nestedStructuralProperties));
      foreach (IEdmStructuralProperty structuralProperty in structuredType.StructuralProperties())
      {
        if (structuralProperty.Type.IsComplex())
          nestedStructuralProperties.Add(structuralProperty);
        else if (structuralProperty.Type.IsCollection())
        {
          if (structuralProperty.Type.AsCollection().ElementType().IsComplex())
            nestedStructuralProperties.Add(structuralProperty);
          else
            structuralProperties.Add(structuralProperty);
        }
        else
          structuralProperties.Add(structuralProperty);
      }
    }

    internal class EdmStructuralTypeInfo
    {
      public ISet<IEdmStructuralProperty> AllStructuralProperties { get; }

      public ISet<IEdmNavigationProperty> AllNavigationProperties { get; }

      public ISet<IEdmAction> AllActions { get; }

      public ISet<IEdmFunction> AllFunctions { get; }

      public EdmStructuralTypeInfo(IEdmModel model, IEdmStructuredType structuredType)
      {
        foreach (IEdmProperty property in structuredType.Properties())
        {
          switch (property.PropertyKind)
          {
            case EdmPropertyKind.Structural:
              if (this.AllStructuralProperties == null)
                this.AllStructuralProperties = (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>();
              this.AllStructuralProperties.Add((IEdmStructuralProperty) property);
              continue;
            case EdmPropertyKind.Navigation:
              if (this.AllNavigationProperties == null)
                this.AllNavigationProperties = (ISet<IEdmNavigationProperty>) new HashSet<IEdmNavigationProperty>();
              this.AllNavigationProperties.Add((IEdmNavigationProperty) property);
              continue;
            default:
              continue;
          }
        }
        if (!(structuredType is IEdmEntityType entityType))
          return;
        IEnumerable<IEdmAction> availableActions = model.GetAvailableActions(entityType);
        this.AllActions = availableActions.Any<IEdmAction>() ? (ISet<IEdmAction>) new HashSet<IEdmAction>(availableActions) : (ISet<IEdmAction>) null;
        IEnumerable<IEdmFunction> availableFunctions = model.GetAvailableFunctions(entityType);
        this.AllFunctions = availableFunctions.Any<IEdmFunction>() ? (ISet<IEdmFunction>) new HashSet<IEdmFunction>(availableFunctions) : (ISet<IEdmFunction>) null;
      }

      public bool IsStructuralPropertyDefined(IEdmStructuralProperty property) => this.AllStructuralProperties != null && this.AllStructuralProperties.Contains(property);

      public bool IsNavigationPropertyDefined(IEdmNavigationProperty property) => this.AllNavigationProperties != null && this.AllNavigationProperties.Contains(property);
    }
  }
}
