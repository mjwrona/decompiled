// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EdmModelHelperMethods
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal static class EdmModelHelperMethods
  {
    public static IEdmModel BuildEdmModel(ODataModelBuilder builder)
    {
      if (builder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builder));
      EdmModel edmModel = new EdmModel();
      EdmEntityContainer edmEntityContainer = new EdmEntityContainer(builder.Namespace, builder.ContainerName);
      EdmTypeMap typesAndProperties = EdmTypeBuilder.GetTypesAndProperties(((IEnumerable<IEdmTypeConfiguration>) builder.StructuralTypes).Concat<IEdmTypeConfiguration>((IEnumerable<IEdmTypeConfiguration>) builder.EnumTypes));
      Dictionary<Type, IEdmType> edmTypeMap = edmModel.AddTypes(typesAndProperties);
      IEnumerable<NavigationSourceAndAnnotations> sourceAndAnnotationses = ((IEnumerable<NavigationSourceAndAnnotations>) edmEntityContainer.AddEntitySetAndAnnotations(builder, edmTypeMap)).Concat<NavigationSourceAndAnnotations>((IEnumerable<NavigationSourceAndAnnotations>) edmEntityContainer.AddSingletonAndAnnotations(builder, edmTypeMap));
      IDictionary<string, EdmNavigationSource> navigationSourceMap = edmModel.GetNavigationSourceMap(typesAndProperties, sourceAndAnnotationses);
      edmModel.AddCoreVocabularyAnnotations(sourceAndAnnotationses, typesAndProperties);
      edmModel.AddCapabilitiesVocabularyAnnotations(sourceAndAnnotationses, typesAndProperties);
      edmModel.AddOperations(builder.Operations, edmEntityContainer, edmTypeMap, navigationSourceMap);
      edmModel.AddElement((IEdmSchemaElement) edmEntityContainer);
      edmModel.SetAnnotationValue<BindableOperationFinder>((IEdmElement) edmModel, new BindableOperationFinder((IEdmModel) edmModel));
      return (IEdmModel) edmModel;
    }

    private static void AddTypes(this EdmModel model, Dictionary<Type, IEdmType> types)
    {
      foreach (IEdmType type in types.Values)
        model.AddType(type);
    }

    private static NavigationSourceAndAnnotations[] AddEntitySetAndAnnotations(
      this EdmEntityContainer container,
      ODataModelBuilder builder,
      Dictionary<Type, IEdmType> edmTypeMap)
    {
      return EdmModelHelperMethods.AddEntitySets(builder.EntitySets, container, edmTypeMap).Select<Tuple<EdmEntitySet, EntitySetConfiguration>, NavigationSourceAndAnnotations>((Func<Tuple<EdmEntitySet, EntitySetConfiguration>, NavigationSourceAndAnnotations>) (e => new NavigationSourceAndAnnotations()
      {
        NavigationSource = (EdmNavigationSource) e.Item1,
        Configuration = (NavigationSourceConfiguration) e.Item2,
        LinkBuilder = new NavigationSourceLinkBuilderAnnotation((NavigationSourceConfiguration) e.Item2),
        Url = new NavigationSourceUrlAnnotation()
        {
          Url = e.Item2.GetUrl()
        }
      })).ToArray<NavigationSourceAndAnnotations>();
    }

    private static NavigationSourceAndAnnotations[] AddSingletonAndAnnotations(
      this EdmEntityContainer container,
      ODataModelBuilder builder,
      Dictionary<Type, IEdmType> edmTypeMap)
    {
      return EdmModelHelperMethods.AddSingletons(builder.Singletons, container, edmTypeMap).Select<Tuple<EdmSingleton, SingletonConfiguration>, NavigationSourceAndAnnotations>((Func<Tuple<EdmSingleton, SingletonConfiguration>, NavigationSourceAndAnnotations>) (e => new NavigationSourceAndAnnotations()
      {
        NavigationSource = (EdmNavigationSource) e.Item1,
        Configuration = (NavigationSourceConfiguration) e.Item2,
        LinkBuilder = new NavigationSourceLinkBuilderAnnotation((NavigationSourceConfiguration) e.Item2),
        Url = new NavigationSourceUrlAnnotation()
        {
          Url = e.Item2.GetUrl()
        }
      })).ToArray<NavigationSourceAndAnnotations>();
    }

    private static IDictionary<string, EdmNavigationSource> GetNavigationSourceMap(
      this EdmModel model,
      EdmTypeMap edmMap,
      IEnumerable<NavigationSourceAndAnnotations> navigationSourceAndAnnotations)
    {
      Dictionary<string, EdmNavigationSource> dictionary = navigationSourceAndAnnotations.ToDictionary<NavigationSourceAndAnnotations, string, EdmNavigationSource>((Func<NavigationSourceAndAnnotations, string>) (e => e.NavigationSource.Name), (Func<NavigationSourceAndAnnotations, EdmNavigationSource>) (e => e.NavigationSource));
      foreach (NavigationSourceAndAnnotations sourceAndAnnotation in navigationSourceAndAnnotations)
      {
        EdmNavigationSource navigationSource = sourceAndAnnotation.NavigationSource;
        model.SetAnnotationValue<NavigationSourceUrlAnnotation>((IEdmElement) navigationSource, sourceAndAnnotation.Url);
        model.SetNavigationSourceLinkBuilder((IEdmNavigationSource) navigationSource, sourceAndAnnotation.LinkBuilder);
        EdmModelHelperMethods.AddNavigationBindings(edmMap, sourceAndAnnotation.Configuration, navigationSource, sourceAndAnnotation.LinkBuilder, dictionary);
      }
      return (IDictionary<string, EdmNavigationSource>) dictionary;
    }

    private static void AddNavigationBindings(
      EdmTypeMap edmMap,
      NavigationSourceConfiguration navigationSourceConfiguration,
      EdmNavigationSource navigationSource,
      NavigationSourceLinkBuilderAnnotation linkBuilder,
      Dictionary<string, EdmNavigationSource> edmNavigationSourceMap)
    {
      foreach (NavigationPropertyBindingConfiguration binding in navigationSourceConfiguration.Bindings)
      {
        NavigationPropertyConfiguration navigationProperty = binding.NavigationProperty;
        int num = navigationProperty.ContainsTarget ? 1 : 0;
        IEdmNavigationProperty navigationProperty1 = (edmMap.EdmTypes[navigationProperty.DeclaringType.ClrType] as IEdmStructuredType).NavigationProperties().Single<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (np => np.Name == navigationProperty.Name));
        string path = EdmModelHelperMethods.ConvertBindingPath(edmMap, binding);
        if (num == 0)
          navigationSource.AddNavigationTarget(navigationProperty1, (IEdmNavigationSource) edmNavigationSourceMap[binding.TargetNavigationSource.Name], (IEdmPathExpression) new EdmPathExpression(path));
        NavigationLinkBuilder navigationPropertyLink = navigationSourceConfiguration.GetNavigationPropertyLink(navigationProperty);
        if (navigationPropertyLink != null)
          linkBuilder.AddNavigationPropertyLinkBuilder(navigationProperty1, navigationPropertyLink);
      }
    }

    private static string ConvertBindingPath(
      EdmTypeMap edmMap,
      NavigationPropertyBindingConfiguration binding)
    {
      IList<string> values = (IList<string>) new List<string>();
      foreach (MemberInfo memberInfo in (IEnumerable<MemberInfo>) binding.Path)
      {
        Type key1 = TypeHelper.AsType(memberInfo);
        PropertyInfo key2 = memberInfo as PropertyInfo;
        if (key1 != (Type) null)
        {
          IEdmType edmType = edmMap.EdmTypes[key1];
          values.Add(edmType.FullTypeName());
        }
        else if (key2 != (PropertyInfo) null)
          values.Add(edmMap.EdmProperties[key2].Name);
      }
      return string.Join("/", (IEnumerable<string>) values);
    }

    private static void AddOperationParameters(
      EdmOperation operation,
      OperationConfiguration operationConfiguration,
      Dictionary<Type, IEdmType> edmTypeMap)
    {
      foreach (ParameterConfiguration parameter1 in operationConfiguration.Parameters)
      {
        bool nullable = parameter1.Nullable;
        IEdmTypeReference edmTypeReference = EdmModelHelperMethods.GetEdmTypeReference(edmTypeMap, parameter1.TypeConfiguration, nullable);
        if (parameter1.IsOptional)
        {
          if (parameter1.DefaultValue != null)
            operation.AddOptionalParameter(parameter1.Name, edmTypeReference, parameter1.DefaultValue);
          else
            operation.AddOptionalParameter(parameter1.Name, edmTypeReference);
        }
        else
        {
          IEdmOperationParameter parameter2 = (IEdmOperationParameter) new EdmOperationParameter((IEdmOperation) operation, parameter1.Name, edmTypeReference);
          operation.AddParameter(parameter2);
        }
      }
    }

    private static void AddOperationLinkBuilder(
      IEdmModel model,
      IEdmOperation operation,
      OperationConfiguration operationConfiguration)
    {
      ActionConfiguration actionConfiguration = operationConfiguration as ActionConfiguration;
      IEdmAction operation1 = operation as IEdmAction;
      FunctionConfiguration functionConfiguration = operationConfiguration as FunctionConfiguration;
      IEdmFunction operation2 = operation as IEdmFunction;
      if (operationConfiguration.BindingParameter.TypeConfiguration.Kind == EdmTypeKind.Entity)
      {
        if (actionConfiguration != null && actionConfiguration.GetActionLink() != null && operation1 != null)
        {
          model.SetOperationLinkBuilder((IEdmOperation) operation1, new OperationLinkBuilder(actionConfiguration.GetActionLink(), actionConfiguration.FollowsConventions));
        }
        else
        {
          if (functionConfiguration == null || functionConfiguration.GetFunctionLink() == null || operation2 == null)
            return;
          model.SetOperationLinkBuilder((IEdmOperation) operation2, new OperationLinkBuilder(functionConfiguration.GetFunctionLink(), functionConfiguration.FollowsConventions));
        }
      }
      else
      {
        if (operationConfiguration.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Collection || ((CollectionTypeConfiguration) operationConfiguration.BindingParameter.TypeConfiguration).ElementType.Kind != EdmTypeKind.Entity)
          return;
        if (actionConfiguration != null && actionConfiguration.GetFeedActionLink() != null && operation1 != null)
        {
          model.SetOperationLinkBuilder((IEdmOperation) operation1, new OperationLinkBuilder(actionConfiguration.GetFeedActionLink(), actionConfiguration.FollowsConventions));
        }
        else
        {
          if (functionConfiguration == null || functionConfiguration.GetFeedFunctionLink() == null || operation2 == null)
            return;
          model.SetOperationLinkBuilder((IEdmOperation) operation2, new OperationLinkBuilder(functionConfiguration.GetFeedFunctionLink(), functionConfiguration.FollowsConventions));
        }
      }
    }

    private static void ValidateOperationEntitySetPath(
      IEdmModel model,
      IEdmOperationImport operationImport,
      OperationConfiguration operationConfiguration)
    {
      if (operationConfiguration.EntitySetPath != null && !operationImport.TryGetRelativeEntitySetPath(model, out IEdmOperationParameter _, out Dictionary<IEdmNavigationProperty, IEdmPathExpression> _, out IEnumerable<EdmError> _))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.OperationHasInvalidEntitySetPath, (object) string.Join("/", operationConfiguration.EntitySetPath), (object) operationConfiguration.FullyQualifiedName);
    }

    private static void AddOperations(
      this EdmModel model,
      IEnumerable<OperationConfiguration> configurations,
      EdmEntityContainer container,
      Dictionary<Type, IEdmType> edmTypeMap,
      IDictionary<string, EdmNavigationSource> edmNavigationSourceMap)
    {
      EdmModelHelperMethods.ValidateActionOverload(configurations.OfType<ActionConfiguration>());
      foreach (OperationConfiguration configuration in configurations)
      {
        IEdmTypeReference edmTypeReference = EdmModelHelperMethods.GetEdmTypeReference(edmTypeMap, configuration.ReturnType, configuration.ReturnType != null && configuration.ReturnNullable);
        IEdmExpression entitySetExpression = EdmModelHelperMethods.GetEdmEntitySetExpression(edmNavigationSourceMap, configuration);
        IEdmPathExpression pathExpression = configuration.EntitySetPath != null ? (IEdmPathExpression) new EdmPathExpression(configuration.EntitySetPath) : (IEdmPathExpression) null;
        EdmOperationImport edmOperationImport;
        switch (configuration.Kind)
        {
          case OperationKind.Action:
            edmOperationImport = EdmModelHelperMethods.CreateActionImport(configuration, container, edmTypeReference, entitySetExpression, pathExpression);
            break;
          case OperationKind.Function:
            edmOperationImport = EdmModelHelperMethods.CreateFunctionImport((FunctionConfiguration) configuration, container, edmTypeReference, entitySetExpression, pathExpression);
            break;
          case OperationKind.ServiceOperation:
            return;
          default:
            return;
        }
        EdmOperation operation = (EdmOperation) edmOperationImport.Operation;
        if (configuration.IsBindable && configuration.Title != null && configuration.Title != configuration.Name)
          model.SetOperationTitleAnnotation((IEdmOperation) operation, new OperationTitleAnnotation(configuration.Title));
        if (configuration.IsBindable && configuration.NavigationSource != null && edmNavigationSourceMap.ContainsKey(configuration.NavigationSource.Name))
          model.SetAnnotationValue<ReturnedEntitySetAnnotation>((IEdmElement) operation, new ReturnedEntitySetAnnotation(configuration.NavigationSource.Name));
        EdmModelHelperMethods.AddOperationParameters(operation, configuration, edmTypeMap);
        if (configuration.IsBindable)
        {
          EdmModelHelperMethods.AddOperationLinkBuilder((IEdmModel) model, (IEdmOperation) operation, configuration);
          EdmModelHelperMethods.ValidateOperationEntitySetPath((IEdmModel) model, (IEdmOperationImport) edmOperationImport, configuration);
        }
        else
          container.AddElement((IEdmEntityContainerElement) edmOperationImport);
        model.AddElement((IEdmSchemaElement) operation);
      }
    }

    private static EdmOperationImport CreateActionImport(
      OperationConfiguration operationConfiguration,
      EdmEntityContainer container,
      IEdmTypeReference returnReference,
      IEdmExpression expression,
      IEdmPathExpression pathExpression)
    {
      EdmAction action = new EdmAction(operationConfiguration.Namespace, operationConfiguration.Name, returnReference, operationConfiguration.IsBindable, pathExpression);
      return (EdmOperationImport) new EdmActionImport((IEdmEntityContainer) container, operationConfiguration.Name, (IEdmAction) action, expression);
    }

    private static EdmOperationImport CreateFunctionImport(
      FunctionConfiguration function,
      EdmEntityContainer container,
      IEdmTypeReference returnReference,
      IEdmExpression expression,
      IEdmPathExpression pathExpression)
    {
      EdmFunction function1 = new EdmFunction(function.Namespace, function.Name, returnReference, function.IsBindable, pathExpression, function.IsComposable);
      return (EdmOperationImport) new EdmFunctionImport((IEdmEntityContainer) container, function.Name, (IEdmFunction) function1, expression, function.IncludeInServiceDocument);
    }

    private static void ValidateActionOverload(IEnumerable<ActionConfiguration> configurations)
    {
      ActionConfiguration[] array1 = configurations.Where<ActionConfiguration>((Func<ActionConfiguration, bool>) (a => !a.IsBindable)).ToArray<ActionConfiguration>();
      if (array1.Length != 0)
      {
        HashSet<string> stringSet = new HashSet<string>();
        foreach (ActionConfiguration actionConfiguration in array1)
        {
          if (!stringSet.Contains(actionConfiguration.Name))
            stringSet.Add(actionConfiguration.Name);
          else
            throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.MoreThanOneUnboundActionFound, (object) actionConfiguration.Name);
        }
      }
      ActionConfiguration[] array2 = configurations.Where<ActionConfiguration>((Func<ActionConfiguration, bool>) (a => a.IsBindable)).ToArray<ActionConfiguration>();
      if (array2.Length == 0)
        return;
      Dictionary<string, IList<IEdmTypeConfiguration>> dictionary = new Dictionary<string, IList<IEdmTypeConfiguration>>();
      foreach (ActionConfiguration actionConfiguration in array2)
      {
        IEdmTypeConfiguration typeConfiguration1 = actionConfiguration.BindingParameter.TypeConfiguration;
        if (dictionary.ContainsKey(actionConfiguration.Name))
        {
          IList<IEdmTypeConfiguration> typeConfigurationList = dictionary[actionConfiguration.Name];
          foreach (IEdmTypeConfiguration typeConfiguration2 in (IEnumerable<IEdmTypeConfiguration>) typeConfigurationList)
          {
            if (typeConfiguration2 == typeConfiguration1)
              throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.MoreThanOneOverloadActionBoundToSameTypeFound, (object) actionConfiguration.Name, (object) typeConfiguration2.FullName);
          }
          typeConfigurationList.Add(typeConfiguration1);
        }
        else
        {
          IList<IEdmTypeConfiguration> typeConfigurationList = (IList<IEdmTypeConfiguration>) new List<IEdmTypeConfiguration>();
          typeConfigurationList.Add(typeConfiguration1);
          dictionary.Add(actionConfiguration.Name, typeConfigurationList);
        }
      }
    }

    private static Dictionary<Type, IEdmType> AddTypes(this EdmModel model, EdmTypeMap edmTypeMap)
    {
      Dictionary<Type, IEdmType> edmTypes = edmTypeMap.EdmTypes;
      model.AddTypes(edmTypes);
      model.AddClrTypeAnnotations(edmTypes);
      Dictionary<PropertyInfo, IEdmProperty> edmProperties = edmTypeMap.EdmProperties;
      model.AddClrPropertyInfoAnnotations(edmProperties);
      model.AddClrEnumMemberInfoAnnotations(edmTypeMap);
      model.AddPropertyRestrictionsAnnotations(edmTypeMap.EdmPropertiesRestrictions);
      model.AddPropertiesQuerySettings(edmTypeMap.EdmPropertiesQuerySettings);
      model.AddStructuredTypeQuerySettings(edmTypeMap.EdmStructuredTypeQuerySettings);
      model.AddDynamicPropertyDictionaryAnnotations(edmTypeMap.OpenTypes);
      return edmTypes;
    }

    private static void AddType(this EdmModel model, IEdmType type)
    {
      if (type.TypeKind == EdmTypeKind.Complex)
        model.AddElement((IEdmSchemaElement) (type as IEdmComplexType));
      else if (type.TypeKind == EdmTypeKind.Entity)
      {
        model.AddElement((IEdmSchemaElement) (type as IEdmEntityType));
      }
      else
      {
        if (type.TypeKind != EdmTypeKind.Enum)
          return;
        model.AddElement((IEdmSchemaElement) (type as IEdmEnumType));
      }
    }

    private static EdmEntitySet AddEntitySet(
      this EdmEntityContainer container,
      EntitySetConfiguration entitySet,
      IDictionary<Type, IEdmType> edmTypeMap)
    {
      return container.AddEntitySet(entitySet.Name, (IEdmEntityType) edmTypeMap[entitySet.EntityType.ClrType]);
    }

    private static IEnumerable<Tuple<EdmEntitySet, EntitySetConfiguration>> AddEntitySets(
      IEnumerable<EntitySetConfiguration> entitySets,
      EdmEntityContainer container,
      Dictionary<Type, IEdmType> edmTypeMap)
    {
      return entitySets.Select<EntitySetConfiguration, Tuple<EdmEntitySet, EntitySetConfiguration>>((Func<EntitySetConfiguration, Tuple<EdmEntitySet, EntitySetConfiguration>>) (es => Tuple.Create<EdmEntitySet, EntitySetConfiguration>(container.AddEntitySet(es, (IDictionary<Type, IEdmType>) edmTypeMap), es)));
    }

    private static EdmSingleton AddSingleton(
      this EdmEntityContainer container,
      SingletonConfiguration singletonType,
      IDictionary<Type, IEdmType> edmTypeMap)
    {
      return container.AddSingleton(singletonType.Name, (IEdmEntityType) edmTypeMap[singletonType.EntityType.ClrType]);
    }

    private static IEnumerable<Tuple<EdmSingleton, SingletonConfiguration>> AddSingletons(
      IEnumerable<SingletonConfiguration> singletons,
      EdmEntityContainer container,
      Dictionary<Type, IEdmType> edmTypeMap)
    {
      return singletons.Select<SingletonConfiguration, Tuple<EdmSingleton, SingletonConfiguration>>((Func<SingletonConfiguration, Tuple<EdmSingleton, SingletonConfiguration>>) (sg => Tuple.Create<EdmSingleton, SingletonConfiguration>(container.AddSingleton(sg, (IDictionary<Type, IEdmType>) edmTypeMap), sg)));
    }

    private static void AddClrTypeAnnotations(
      this EdmModel model,
      Dictionary<Type, IEdmType> edmTypes)
    {
      foreach (KeyValuePair<Type, IEdmType> edmType in edmTypes)
      {
        IEdmType element = edmType.Value;
        Type key = edmType.Key;
        model.SetAnnotationValue<ClrTypeAnnotation>((IEdmElement) element, new ClrTypeAnnotation(key));
      }
    }

    private static void AddClrPropertyInfoAnnotations(
      this EdmModel model,
      Dictionary<PropertyInfo, IEdmProperty> edmProperties)
    {
      foreach (KeyValuePair<PropertyInfo, IEdmProperty> edmProperty in edmProperties)
      {
        IEdmProperty element = edmProperty.Value;
        PropertyInfo key = edmProperty.Key;
        if (element.Name != key.Name)
          model.SetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) element, new ClrPropertyInfoAnnotation(key));
      }
    }

    private static void AddClrEnumMemberInfoAnnotations(this EdmModel model, EdmTypeMap edmTypeMap)
    {
      if (edmTypeMap.EnumMembers == null || !edmTypeMap.EnumMembers.Any<KeyValuePair<Enum, IEdmEnumMember>>())
        return;
      foreach (IGrouping<Type, KeyValuePair<Enum, IEdmEnumMember>> source in edmTypeMap.EnumMembers.GroupBy<KeyValuePair<Enum, IEdmEnumMember>, Type, KeyValuePair<Enum, IEdmEnumMember>>((Func<KeyValuePair<Enum, IEdmEnumMember>, Type>) (e => e.Key.GetType()), (Func<KeyValuePair<Enum, IEdmEnumMember>, KeyValuePair<Enum, IEdmEnumMember>>) (e => e)))
      {
        IEdmType edmType = edmTypeMap.EdmTypes[source.Key];
        model.SetAnnotationValue<ClrEnumMemberAnnotation>((IEdmElement) edmType, new ClrEnumMemberAnnotation((IDictionary<Enum, IEdmEnumMember>) source.ToDictionary<KeyValuePair<Enum, IEdmEnumMember>, Enum, IEdmEnumMember>((Func<KeyValuePair<Enum, IEdmEnumMember>, Enum>) (e => e.Key), (Func<KeyValuePair<Enum, IEdmEnumMember>, IEdmEnumMember>) (e => e.Value))));
      }
    }

    private static void AddDynamicPropertyDictionaryAnnotations(
      this EdmModel model,
      Dictionary<IEdmStructuredType, PropertyInfo> openTypes)
    {
      foreach (KeyValuePair<IEdmStructuredType, PropertyInfo> openType in openTypes)
      {
        IEdmStructuredType key = openType.Key;
        PropertyInfo propertyInfo = openType.Value;
        model.SetAnnotationValue<DynamicPropertyDictionaryAnnotation>((IEdmElement) key, new DynamicPropertyDictionaryAnnotation(propertyInfo));
      }
    }

    private static void AddPropertiesQuerySettings(
      this EdmModel model,
      Dictionary<IEdmProperty, ModelBoundQuerySettings> edmPropertiesQuerySettings)
    {
      foreach (KeyValuePair<IEdmProperty, ModelBoundQuerySettings> propertiesQuerySetting in edmPropertiesQuerySettings)
      {
        IEdmProperty key = propertiesQuerySetting.Key;
        ModelBoundQuerySettings boundQuerySettings = propertiesQuerySetting.Value;
        model.SetAnnotationValue<ModelBoundQuerySettings>((IEdmElement) key, boundQuerySettings);
      }
    }

    private static void AddStructuredTypeQuerySettings(
      this EdmModel model,
      Dictionary<IEdmStructuredType, ModelBoundQuerySettings> edmStructuredTypeQuerySettings)
    {
      foreach (KeyValuePair<IEdmStructuredType, ModelBoundQuerySettings> typeQuerySetting in edmStructuredTypeQuerySettings)
      {
        IEdmStructuredType key = typeQuerySetting.Key;
        ModelBoundQuerySettings boundQuerySettings = typeQuerySetting.Value;
        model.SetAnnotationValue<ModelBoundQuerySettings>((IEdmElement) key, boundQuerySettings);
      }
    }

    private static void AddPropertyRestrictionsAnnotations(
      this EdmModel model,
      Dictionary<IEdmProperty, QueryableRestrictions> edmPropertiesRestrictions)
    {
      foreach (KeyValuePair<IEdmProperty, QueryableRestrictions> propertiesRestriction in edmPropertiesRestrictions)
      {
        IEdmProperty key = propertiesRestriction.Key;
        QueryableRestrictions restrictions = propertiesRestriction.Value;
        model.SetAnnotationValue<QueryableRestrictionsAnnotation>((IEdmElement) key, new QueryableRestrictionsAnnotation(restrictions));
      }
    }

    private static void AddCoreVocabularyAnnotations(
      this EdmModel model,
      IEnumerable<NavigationSourceAndAnnotations> navigationSources,
      EdmTypeMap edmTypeMap)
    {
      if (navigationSources == null)
        return;
      foreach (NavigationSourceAndAnnotations navigationSource1 in navigationSources)
      {
        if (navigationSource1.NavigationSource is IEdmVocabularyAnnotatable navigationSource2)
        {
          NavigationSourceConfiguration configuration = navigationSource1.Configuration;
          if (configuration != null)
            model.AddOptimisticConcurrencyAnnotation(navigationSource2, configuration, edmTypeMap);
        }
      }
    }

    private static void AddOptimisticConcurrencyAnnotation(
      this EdmModel model,
      IEdmVocabularyAnnotatable target,
      NavigationSourceConfiguration navigationSourceConfiguration,
      EdmTypeMap edmTypeMap)
    {
      EntityTypeConfiguration entityType = navigationSourceConfiguration.EntityType;
      IEnumerable<StructuralPropertyConfiguration> first = entityType.Properties.OfType<StructuralPropertyConfiguration>().Where<StructuralPropertyConfiguration>((Func<StructuralPropertyConfiguration, bool>) (property => property.ConcurrencyToken));
      foreach (StructuralTypeConfiguration baseType in entityType.BaseTypes())
        first = first.Concat<StructuralPropertyConfiguration>(baseType.Properties.OfType<StructuralPropertyConfiguration>().Where<StructuralPropertyConfiguration>((Func<StructuralPropertyConfiguration, bool>) (property => property.ConcurrencyToken)));
      IList<IEdmStructuralProperty> source = (IList<IEdmStructuralProperty>) new List<IEdmStructuralProperty>();
      foreach (StructuralPropertyConfiguration propertyConfiguration in first)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty is IEdmStructuralProperty structuralProperty)
          source.Add(structuralProperty);
      }
      if (!source.Any<IEdmStructuralProperty>())
        return;
      IEdmCollectionExpression collectionExpression = (IEdmCollectionExpression) new EdmCollectionExpression((IEdmExpression[]) source.Select<IEdmStructuralProperty, EdmPropertyPathExpression>((Func<IEdmStructuralProperty, EdmPropertyPathExpression>) (p => new EdmPropertyPathExpression(p.Name))).ToArray<EdmPropertyPathExpression>());
      IEdmTerm concurrencyTerm = CoreVocabularyModel.ConcurrencyTerm;
      EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, concurrencyTerm, (IEdmExpression) collectionExpression);
      annotation.SetSerializationLocation((IEdmModel) model, new EdmVocabularyAnnotationSerializationLocation?(EdmVocabularyAnnotationSerializationLocation.Inline));
      model.SetVocabularyAnnotation((IEdmVocabularyAnnotation) annotation);
    }

    private static void AddCapabilitiesVocabularyAnnotations(
      this EdmModel model,
      IEnumerable<NavigationSourceAndAnnotations> navigationSources,
      EdmTypeMap edmTypeMap)
    {
      if (navigationSources == null)
        return;
      foreach (NavigationSourceAndAnnotations navigationSource1 in navigationSources)
      {
        if (navigationSource1.NavigationSource is IEdmEntitySet navigationSource2 && navigationSource1.Configuration is EntitySetConfiguration configuration)
        {
          model.AddCountRestrictionsAnnotation(navigationSource2, configuration, edmTypeMap);
          model.AddNavigationRestrictionsAnnotation(navigationSource2, configuration, edmTypeMap);
          model.AddFilterRestrictionsAnnotation(navigationSource2, configuration, edmTypeMap);
          model.AddSortRestrictionsAnnotation(navigationSource2, configuration, edmTypeMap);
          model.AddExpandRestrictionsAnnotation(navigationSource2, configuration, edmTypeMap);
        }
      }
    }

    private static void AddCountRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      EntitySetConfiguration entitySetConfiguration,
      EdmTypeMap edmTypeMap)
    {
      IEnumerable<PropertyConfiguration> propertyConfigurations = entitySetConfiguration.EntityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.NotCountable));
      IList<IEdmProperty> edmPropertyList = (IList<IEdmProperty>) new List<IEdmProperty>();
      IList<IEdmNavigationProperty> navigationPropertyList = (IList<IEdmNavigationProperty>) new List<IEdmNavigationProperty>();
      foreach (PropertyConfiguration propertyConfiguration in propertyConfigurations)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty != null && edmProperty.Type.TypeKind() == EdmTypeKind.Collection)
        {
          if (edmProperty.PropertyKind == EdmPropertyKind.Navigation)
            navigationPropertyList.Add((IEdmNavigationProperty) edmProperty);
          else
            edmPropertyList.Add(edmProperty);
        }
      }
      if (!edmPropertyList.Any<IEdmProperty>() && !navigationPropertyList.Any<IEdmNavigationProperty>())
        return;
      model.SetCountRestrictionsAnnotation(target, true, (IEnumerable<IEdmProperty>) edmPropertyList, (IEnumerable<IEdmNavigationProperty>) navigationPropertyList);
    }

    private static void AddNavigationRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      EntitySetConfiguration entitySetConfiguration,
      EdmTypeMap edmTypeMap)
    {
      IEnumerable<PropertyConfiguration> propertyConfigurations = entitySetConfiguration.EntityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.NotNavigable));
      IList<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>> tupleList = (IList<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>>) new List<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>>();
      foreach (PropertyConfiguration propertyConfiguration in propertyConfigurations)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty != null && edmProperty.PropertyKind == EdmPropertyKind.Navigation)
          tupleList.Add(new Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>((IEdmNavigationProperty) edmProperty, CapabilitiesNavigationType.Recursive));
      }
      if (!tupleList.Any<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>>())
        return;
      model.SetNavigationRestrictionsAnnotation(target, CapabilitiesNavigationType.Recursive, (IEnumerable<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>>) tupleList);
    }

    private static void AddFilterRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      EntitySetConfiguration entitySetConfiguration,
      EdmTypeMap edmTypeMap)
    {
      IEnumerable<PropertyConfiguration> propertyConfigurations = entitySetConfiguration.EntityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.NonFilterable));
      IList<IEdmProperty> edmPropertyList = (IList<IEdmProperty>) new List<IEdmProperty>();
      foreach (PropertyConfiguration propertyConfiguration in propertyConfigurations)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty != null)
          edmPropertyList.Add(edmProperty);
      }
      if (!edmPropertyList.Any<IEdmProperty>())
        return;
      model.SetFilterRestrictionsAnnotation(target, true, true, (IEnumerable<IEdmProperty>) null, (IEnumerable<IEdmProperty>) edmPropertyList);
    }

    private static void AddSortRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      EntitySetConfiguration entitySetConfiguration,
      EdmTypeMap edmTypeMap)
    {
      IEnumerable<PropertyConfiguration> propertyConfigurations = entitySetConfiguration.EntityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.Unsortable));
      IList<IEdmProperty> edmPropertyList = (IList<IEdmProperty>) new List<IEdmProperty>();
      foreach (PropertyConfiguration propertyConfiguration in propertyConfigurations)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty != null)
          edmPropertyList.Add(edmProperty);
      }
      if (!edmPropertyList.Any<IEdmProperty>())
        return;
      model.SetSortRestrictionsAnnotation(target, true, (IEnumerable<IEdmProperty>) null, (IEnumerable<IEdmProperty>) null, (IEnumerable<IEdmProperty>) edmPropertyList);
    }

    private static void AddExpandRestrictionsAnnotation(
      this EdmModel model,
      IEdmEntitySet target,
      EntitySetConfiguration entitySetConfiguration,
      EdmTypeMap edmTypeMap)
    {
      IEnumerable<PropertyConfiguration> propertyConfigurations = entitySetConfiguration.EntityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.NotExpandable));
      IList<IEdmNavigationProperty> navigationPropertyList = (IList<IEdmNavigationProperty>) new List<IEdmNavigationProperty>();
      foreach (PropertyConfiguration propertyConfiguration in propertyConfigurations)
      {
        IEdmProperty edmProperty;
        if (edmTypeMap.EdmProperties.TryGetValue(propertyConfiguration.PropertyInfo, out edmProperty) && edmProperty != null && edmProperty.PropertyKind == EdmPropertyKind.Navigation)
          navigationPropertyList.Add((IEdmNavigationProperty) edmProperty);
      }
      if (!navigationPropertyList.Any<IEdmNavigationProperty>())
        return;
      model.SetExpandRestrictionsAnnotation(target, true, (IEnumerable<IEdmNavigationProperty>) navigationPropertyList);
    }

    private static IEdmExpression GetEdmEntitySetExpression(
      IDictionary<string, EdmNavigationSource> navigationSources,
      OperationConfiguration operationConfiguration)
    {
      if (operationConfiguration.NavigationSource != null)
      {
        EdmNavigationSource navigationSource;
        if (navigationSources.TryGetValue(operationConfiguration.NavigationSource.Name, out navigationSource))
        {
          if (navigationSource is EdmEntitySet edmEntitySet)
            return (IEdmExpression) new EdmPathExpression(edmEntitySet.Name);
        }
        else
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EntitySetNotFoundForName, (object) operationConfiguration.NavigationSource.Name);
      }
      else if (operationConfiguration.EntitySetPath != null)
        return (IEdmExpression) new EdmPathExpression(operationConfiguration.EntitySetPath);
      return (IEdmExpression) null;
    }

    private static IEdmTypeReference GetEdmTypeReference(
      Dictionary<Type, IEdmType> availableTypes,
      IEdmTypeConfiguration configuration,
      bool nullable)
    {
      if (configuration == null)
        return (IEdmTypeReference) null;
      EdmTypeKind kind1 = configuration.Kind;
      if (kind1 == EdmTypeKind.Collection)
      {
        CollectionTypeConfiguration typeConfiguration = (CollectionTypeConfiguration) configuration;
        return (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(EdmModelHelperMethods.GetEdmTypeReference(availableTypes, typeConfiguration.ElementType, nullable)));
      }
      Type type = TypeHelper.GetUnderlyingTypeOrSelf(configuration.ClrType);
      if (!TypeHelper.IsEnum(type))
        type = configuration.ClrType;
      IEdmType edmType;
      if (availableTypes.TryGetValue(type, out edmType))
      {
        switch (kind1)
        {
          case EdmTypeKind.Entity:
            return (IEdmTypeReference) new EdmEntityTypeReference((IEdmEntityType) edmType, nullable);
          case EdmTypeKind.Complex:
            return (IEdmTypeReference) new EdmComplexTypeReference((IEdmComplexType) edmType, nullable);
          case EdmTypeKind.Enum:
            return (IEdmTypeReference) new EdmEnumTypeReference((IEdmEnumType) edmType, nullable);
          default:
            throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.UnsupportedEdmTypeKind, (object) kind1.ToString());
        }
      }
      else
      {
        EdmPrimitiveTypeKind kind2 = configuration.Kind == EdmTypeKind.Primitive ? EdmTypeBuilder.GetTypeKind(((PrimitiveTypeConfiguration) configuration).ClrType) : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NoMatchingIEdmTypeFound, (object) configuration.FullName);
        return (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(kind2, nullable);
      }
    }

    internal static string GetNavigationSourceUrl(
      this IEdmModel model,
      IEdmNavigationSource navigationSource)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      NavigationSourceUrlAnnotation sourceUrlAnnotation = navigationSource != null ? model.GetAnnotationValue<NavigationSourceUrlAnnotation>((IEdmElement) navigationSource) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationSource));
      return sourceUrlAnnotation == null ? navigationSource.Name : sourceUrlAnnotation.Url;
    }

    internal static IEnumerable<IEdmAction> GetAvailableActions(
      this IEdmModel model,
      IEdmEntityType entityType)
    {
      return model.GetAvailableOperations(entityType).OfType<IEdmAction>();
    }

    internal static IEnumerable<IEdmFunction> GetAvailableFunctions(
      this IEdmModel model,
      IEdmEntityType entityType)
    {
      return model.GetAvailableOperations(entityType).OfType<IEdmFunction>();
    }

    internal static IEnumerable<IEdmOperation> GetAvailableOperationsBoundToCollection(
      this IEdmModel model,
      IEdmEntityType entityType)
    {
      return model.GetAvailableOperations(entityType, true);
    }

    internal static IEnumerable<IEdmOperation> GetAvailableOperations(
      this IEdmModel model,
      IEdmEntityType entityType,
      bool boundToCollection = false)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (entityType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityType));
      BindableOperationFinder bindableOperationFinder = model.GetAnnotationValue<BindableOperationFinder>((IEdmElement) model);
      if (bindableOperationFinder == null)
      {
        bindableOperationFinder = new BindableOperationFinder(model);
        model.SetAnnotationValue<BindableOperationFinder>((IEdmElement) model, bindableOperationFinder);
      }
      return boundToCollection ? bindableOperationFinder.FindOperationsBoundToCollection(entityType) : bindableOperationFinder.FindOperations(entityType);
    }
  }
}
