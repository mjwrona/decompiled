// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.OperationConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class OperationConfiguration
  {
    private List<ParameterConfiguration> _parameters = new List<ParameterConfiguration>();
    private BindingParameterConfiguration _bindingParameter;
    private string _namespace;

    internal OperationConfiguration(ODataModelBuilder builder, string name)
    {
      this.Name = name;
      this.ModelBuilder = builder;
    }

    protected OperationLinkBuilder OperationLinkBuilder { get; set; }

    public bool FollowsConventions { get; protected set; }

    public string Name { get; protected set; }

    public string Title { get; set; }

    public abstract OperationKind Kind { get; }

    public virtual bool IsComposable { get; internal set; }

    public abstract bool IsSideEffecting { get; }

    public string FullyQualifiedName => this.Namespace + "." + this.Name;

    public string Namespace
    {
      get => this._namespace ?? this.ModelBuilder.Namespace;
      set => this._namespace = value;
    }

    public IEdmTypeConfiguration ReturnType { get; set; }

    public bool ReturnNullable { get; set; }

    public NavigationSourceConfiguration NavigationSource { get; set; }

    public IEnumerable<string> EntitySetPath { get; internal set; }

    public virtual BindingParameterConfiguration BindingParameter => this._bindingParameter;

    public virtual IEnumerable<ParameterConfiguration> Parameters
    {
      get
      {
        if (this._bindingParameter != null)
          yield return (ParameterConfiguration) this._bindingParameter;
        foreach (ParameterConfiguration parameter in this._parameters)
          yield return parameter;
      }
    }

    public virtual bool IsBindable => this._bindingParameter != null;

    internal void ReturnsFromEntitySetImplementation<TEntityType>(string entitySetName) where TEntityType : class
    {
      this.ModelBuilder.EntitySet<TEntityType>(entitySetName);
      this.NavigationSource = (NavigationSourceConfiguration) this.ModelBuilder.EntitySets.Single<EntitySetConfiguration>((Func<EntitySetConfiguration, bool>) (s => s.Name == entitySetName));
      this.ReturnType = this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TEntityType));
      this.ReturnNullable = true;
    }

    internal void ReturnsCollectionFromEntitySetImplementation<TElementEntityType>(
      string entitySetName)
      where TElementEntityType : class
    {
      Type clrType = typeof (IEnumerable<TElementEntityType>);
      this.ModelBuilder.EntitySet<TElementEntityType>(entitySetName);
      this.NavigationSource = (NavigationSourceConfiguration) this.ModelBuilder.EntitySets.Single<EntitySetConfiguration>((Func<EntitySetConfiguration, bool>) (s => s.Name == entitySetName));
      this.ReturnType = (IEdmTypeConfiguration) new CollectionTypeConfiguration(this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TElementEntityType)), clrType);
      this.ReturnNullable = true;
    }

    internal void ReturnsEntityViaEntitySetPathImplementation<TEntityType>(
      IEnumerable<string> entitySetPath)
      where TEntityType : class
    {
      this.ReturnType = this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TEntityType));
      this.EntitySetPath = entitySetPath;
      this.ReturnNullable = true;
    }

    internal void ReturnsCollectionViaEntitySetPathImplementation<TElementEntityType>(
      IEnumerable<string> entitySetPath)
      where TElementEntityType : class
    {
      Type clrType = typeof (IEnumerable<TElementEntityType>);
      this.ReturnType = (IEdmTypeConfiguration) new CollectionTypeConfiguration(this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TElementEntityType)), clrType);
      this.EntitySetPath = entitySetPath;
      this.ReturnNullable = true;
    }

    internal void ReturnsImplementation(Type clrReturnType)
    {
      this.ReturnType = this.GetOperationTypeConfiguration(clrReturnType);
      this.ReturnNullable = EdmLibHelpers.IsNullable(clrReturnType);
    }

    internal void ReturnsCollectionImplementation<TReturnElementType>()
    {
      Type clrType = typeof (IEnumerable<TReturnElementType>);
      Type type = typeof (TReturnElementType);
      this.ReturnType = (IEdmTypeConfiguration) new CollectionTypeConfiguration(this.GetOperationTypeConfiguration(type), clrType);
      this.ReturnNullable = EdmLibHelpers.IsNullable(type);
    }

    internal void SetBindingParameterImplementation(
      string name,
      IEdmTypeConfiguration bindingParameterType)
    {
      this._bindingParameter = new BindingParameterConfiguration(name, bindingParameterType);
    }

    public ParameterConfiguration AddParameter(string name, IEdmTypeConfiguration parameterType)
    {
      ParameterConfiguration parameterConfiguration = (ParameterConfiguration) new NonbindingParameterConfiguration(name, parameterType);
      this._parameters.Add(parameterConfiguration);
      return parameterConfiguration;
    }

    public ParameterConfiguration Parameter(Type clrParameterType, string name)
    {
      IEdmTypeConfiguration parameterType = !(clrParameterType == (Type) null) ? this.GetOperationTypeConfiguration(clrParameterType) : throw Error.ArgumentNull(nameof (clrParameterType));
      return this.AddParameter(name, parameterType);
    }

    public ParameterConfiguration Parameter<TParameter>(string name) => this.Parameter(typeof (TParameter), name);

    public ParameterConfiguration CollectionParameter<TElementType>(string name)
    {
      Type type = typeof (TElementType);
      CollectionTypeConfiguration parameterType = new CollectionTypeConfiguration(this.GetOperationTypeConfiguration(typeof (TElementType)), typeof (IEnumerable<>).MakeGenericType(type));
      return this.AddParameter(name, (IEdmTypeConfiguration) parameterType);
    }

    public ParameterConfiguration EntityParameter<TEntityType>(string name) where TEntityType : class
    {
      Type entityType = typeof (TEntityType);
      IEdmTypeConfiguration parameterType = (IEdmTypeConfiguration) (this.ModelBuilder.StructuralTypes.FirstOrDefault<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (t => t.ClrType == entityType)) ?? (StructuralTypeConfiguration) this.ModelBuilder.AddEntityType(entityType));
      return this.AddParameter(name, parameterType);
    }

    public ParameterConfiguration CollectionEntityParameter<TElementEntityType>(string name) where TElementEntityType : class
    {
      Type elementType = typeof (TElementEntityType);
      CollectionTypeConfiguration parameterType = new CollectionTypeConfiguration((IEdmTypeConfiguration) (this.ModelBuilder.StructuralTypes.FirstOrDefault<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (t => t.ClrType == elementType)) ?? (StructuralTypeConfiguration) this.ModelBuilder.AddEntityType(elementType)), typeof (IEnumerable<>).MakeGenericType(elementType));
      return this.AddParameter(name, (IEdmTypeConfiguration) parameterType);
    }

    protected ODataModelBuilder ModelBuilder { get; set; }

    private IEdmTypeConfiguration GetOperationTypeConfiguration(Type clrType)
    {
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(clrType);
      IEdmTypeConfiguration typeConfiguration1;
      if (TypeHelper.IsEnum(underlyingTypeOrSelf))
      {
        typeConfiguration1 = this.ModelBuilder.GetTypeConfigurationOrNull(underlyingTypeOrSelf);
        if (typeConfiguration1 != null && EdmLibHelpers.IsNullable(clrType))
          typeConfiguration1 = (IEdmTypeConfiguration) ((EnumTypeConfiguration) typeConfiguration1).GetNullableEnumTypeConfiguration();
      }
      else
        typeConfiguration1 = this.ModelBuilder.GetTypeConfigurationOrNull(clrType);
      if (typeConfiguration1 == null)
      {
        if (TypeHelper.IsEnum(underlyingTypeOrSelf))
        {
          EnumTypeConfiguration typeConfiguration2 = this.ModelBuilder.AddEnumType(underlyingTypeOrSelf);
          typeConfiguration1 = !EdmLibHelpers.IsNullable(clrType) ? (IEdmTypeConfiguration) typeConfiguration2 : (IEdmTypeConfiguration) typeConfiguration2.GetNullableEnumTypeConfiguration();
        }
        else
          typeConfiguration1 = (IEdmTypeConfiguration) this.ModelBuilder.AddComplexType(clrType);
      }
      return typeConfiguration1;
    }
  }
}
