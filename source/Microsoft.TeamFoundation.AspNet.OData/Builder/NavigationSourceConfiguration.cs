// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationSourceConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class NavigationSourceConfiguration
  {
    private readonly ODataModelBuilder _modelBuilder;
    private string _url;
    private SelfLinkBuilder<Uri> _editLinkBuilder;
    private SelfLinkBuilder<Uri> _readLinkBuilder;
    private SelfLinkBuilder<Uri> _idLinkBuilder;
    private readonly Dictionary<NavigationPropertyConfiguration, Dictionary<string, NavigationPropertyBindingConfiguration>> _navigationPropertyBindings = new Dictionary<NavigationPropertyConfiguration, Dictionary<string, NavigationPropertyBindingConfiguration>>();
    private readonly Dictionary<NavigationPropertyConfiguration, NavigationLinkBuilder> _navigationPropertyLinkBuilders;

    protected NavigationSourceConfiguration()
    {
    }

    protected NavigationSourceConfiguration(
      ODataModelBuilder modelBuilder,
      Type entityClrType,
      string name)
      : this(modelBuilder, new EntityTypeConfiguration(modelBuilder, entityClrType), name)
    {
    }

    protected NavigationSourceConfiguration(
      ODataModelBuilder modelBuilder,
      EntityTypeConfiguration entityType,
      string name)
    {
      if (modelBuilder == null)
        throw Error.ArgumentNull(nameof (modelBuilder));
      if (entityType == null)
        throw Error.ArgumentNull(nameof (entityType));
      if (string.IsNullOrEmpty(name))
        throw Error.ArgumentNullOrEmpty(nameof (name));
      this._modelBuilder = modelBuilder;
      this.Name = name;
      this.EntityType = entityType;
      this.ClrType = entityType.ClrType;
      this._url = this.Name;
      this._editLinkBuilder = (SelfLinkBuilder<Uri>) null;
      this._readLinkBuilder = (SelfLinkBuilder<Uri>) null;
      this._navigationPropertyLinkBuilders = new Dictionary<NavigationPropertyConfiguration, NavigationLinkBuilder>();
    }

    public IEnumerable<NavigationPropertyBindingConfiguration> Bindings => this._navigationPropertyBindings.Values.SelectMany<Dictionary<string, NavigationPropertyBindingConfiguration>, NavigationPropertyBindingConfiguration>((Func<Dictionary<string, NavigationPropertyBindingConfiguration>, IEnumerable<NavigationPropertyBindingConfiguration>>) (e => (IEnumerable<NavigationPropertyBindingConfiguration>) e.Values));

    public virtual EntityTypeConfiguration EntityType { get; private set; }

    public Type ClrType { get; private set; }

    public string Name { get; private set; }

    public virtual NavigationSourceConfiguration HasUrl(string url)
    {
      this._url = url;
      return this;
    }

    public virtual NavigationSourceConfiguration HasEditLink(SelfLinkBuilder<Uri> editLinkBuilder)
    {
      this._editLinkBuilder = editLinkBuilder != null ? editLinkBuilder : throw Error.ArgumentNull(nameof (editLinkBuilder));
      return this;
    }

    public virtual NavigationSourceConfiguration HasReadLink(SelfLinkBuilder<Uri> readLinkBuilder)
    {
      this._readLinkBuilder = readLinkBuilder != null ? readLinkBuilder : throw Error.ArgumentNull(nameof (readLinkBuilder));
      return this;
    }

    public virtual NavigationSourceConfiguration HasIdLink(SelfLinkBuilder<Uri> idLinkBuilder)
    {
      this._idLinkBuilder = idLinkBuilder != null ? idLinkBuilder : throw Error.ArgumentNull(nameof (idLinkBuilder));
      return this;
    }

    public virtual NavigationSourceConfiguration HasNavigationPropertyLink(
      NavigationPropertyConfiguration navigationProperty,
      NavigationLinkBuilder navigationLinkBuilder)
    {
      if (navigationProperty == null)
        throw Error.ArgumentNull(nameof (navigationProperty));
      if (navigationLinkBuilder == null)
        throw Error.ArgumentNull(nameof (navigationLinkBuilder));
      StructuralTypeConfiguration declaringType = navigationProperty.DeclaringType;
      if (!declaringType.IsAssignableFrom((StructuralTypeConfiguration) this.EntityType) && !this.EntityType.IsAssignableFrom(declaringType))
        throw Error.Argument(nameof (navigationProperty), SRResources.NavigationPropertyNotInHierarchy, (object) declaringType.FullName, (object) this.EntityType.FullName, (object) this.Name);
      this._navigationPropertyLinkBuilders[navigationProperty] = navigationLinkBuilder;
      return this;
    }

    public virtual NavigationSourceConfiguration HasNavigationPropertiesLink(
      IEnumerable<NavigationPropertyConfiguration> navigationProperties,
      NavigationLinkBuilder navigationLinkBuilder)
    {
      if (navigationProperties == null)
        throw Error.ArgumentNull(nameof (navigationProperties));
      if (navigationLinkBuilder == null)
        throw Error.ArgumentNull(nameof (navigationLinkBuilder));
      foreach (NavigationPropertyConfiguration navigationProperty in navigationProperties)
        this.HasNavigationPropertyLink(navigationProperty, navigationLinkBuilder);
      return this;
    }

    public virtual NavigationPropertyBindingConfiguration AddBinding(
      NavigationPropertyConfiguration navigationConfiguration,
      NavigationSourceConfiguration targetNavigationSource)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      if (targetNavigationSource == null)
        throw Error.ArgumentNull(nameof (targetNavigationSource));
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      if (navigationConfiguration.DeclaringType != this.EntityType)
        bindingPath.Insert(0, TypeHelper.AsMemberInfo(navigationConfiguration.DeclaringType.ClrType));
      return this.AddBinding(navigationConfiguration, targetNavigationSource, bindingPath);
    }

    public virtual NavigationPropertyBindingConfiguration AddBinding(
      NavigationPropertyConfiguration navigationConfiguration,
      NavigationSourceConfiguration targetNavigationSource,
      IList<MemberInfo> bindingPath)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      if (targetNavigationSource == null)
        throw Error.ArgumentNull(nameof (targetNavigationSource));
      if (bindingPath == null || !bindingPath.Any<MemberInfo>())
        throw Error.ArgumentNull(nameof (bindingPath));
      this.VerifyBindingPath(navigationConfiguration, bindingPath);
      string key = bindingPath.ConvertBindingPath();
      Dictionary<string, NavigationPropertyBindingConfiguration> dictionary;
      NavigationPropertyBindingConfiguration bindingConfiguration;
      if (this._navigationPropertyBindings.TryGetValue(navigationConfiguration, out dictionary))
      {
        if (dictionary.TryGetValue(key, out bindingConfiguration))
        {
          if (bindingConfiguration.TargetNavigationSource != targetNavigationSource)
            throw Error.NotSupported(SRResources.RebindingNotSupported);
        }
        else
        {
          bindingConfiguration = new NavigationPropertyBindingConfiguration(navigationConfiguration, targetNavigationSource, bindingPath);
          this._navigationPropertyBindings[navigationConfiguration][key] = bindingConfiguration;
        }
      }
      else
      {
        this._navigationPropertyBindings[navigationConfiguration] = new Dictionary<string, NavigationPropertyBindingConfiguration>();
        bindingConfiguration = new NavigationPropertyBindingConfiguration(navigationConfiguration, targetNavigationSource, bindingPath);
        this._navigationPropertyBindings[navigationConfiguration][key] = bindingConfiguration;
      }
      return bindingConfiguration;
    }

    public virtual void RemoveBinding(
      NavigationPropertyConfiguration navigationConfiguration)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      this._navigationPropertyBindings.Remove(navigationConfiguration);
    }

    public virtual void RemoveBinding(
      NavigationPropertyConfiguration navigationConfiguration,
      string bindingPath)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      Dictionary<string, NavigationPropertyBindingConfiguration> source;
      if (!this._navigationPropertyBindings.TryGetValue(navigationConfiguration, out source))
        return;
      source.Remove(bindingPath);
      if (source.Any<KeyValuePair<string, NavigationPropertyBindingConfiguration>>())
        return;
      this._navigationPropertyBindings.Remove(navigationConfiguration);
    }

    public virtual IEnumerable<NavigationPropertyBindingConfiguration> FindBinding(
      NavigationPropertyConfiguration navigationConfiguration)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      Dictionary<string, NavigationPropertyBindingConfiguration> dictionary;
      return this._navigationPropertyBindings.TryGetValue(navigationConfiguration, out dictionary) ? (IEnumerable<NavigationPropertyBindingConfiguration>) dictionary.Values : (IEnumerable<NavigationPropertyBindingConfiguration>) null;
    }

    public virtual NavigationPropertyBindingConfiguration FindBinding(
      NavigationPropertyConfiguration navigationConfiguration,
      IList<MemberInfo> bindingPath)
    {
      if (navigationConfiguration == null)
        throw Error.ArgumentNull(nameof (navigationConfiguration));
      string key = bindingPath != null ? bindingPath.ConvertBindingPath() : throw Error.ArgumentNullOrEmpty(nameof (bindingPath));
      Dictionary<string, NavigationPropertyBindingConfiguration> dictionary;
      NavigationPropertyBindingConfiguration binding;
      if (this._navigationPropertyBindings.TryGetValue(navigationConfiguration, out dictionary) && dictionary.TryGetValue(key, out binding))
        return binding;
      if (this._modelBuilder.BindingOptions == NavigationPropertyBindingOption.None)
        return (NavigationPropertyBindingConfiguration) null;
      int num = navigationConfiguration.PropertyInfo.GetCustomAttributes<SingletonAttribute>().Any<SingletonAttribute>() ? 1 : 0;
      Type entityType = navigationConfiguration.RelatedClrType;
      NavigationSourceConfiguration[] source = num == 0 ? (NavigationSourceConfiguration[]) this._modelBuilder.EntitySets.Where<EntitySetConfiguration>((Func<EntitySetConfiguration, bool>) (es => es.EntityType.ClrType == entityType)).ToArray<EntitySetConfiguration>() : (NavigationSourceConfiguration[]) this._modelBuilder.Singletons.Where<SingletonConfiguration>((Func<SingletonConfiguration, bool>) (es => es.EntityType.ClrType == entityType)).ToArray<SingletonConfiguration>();
      if (source.Length < 1)
        return (NavigationPropertyBindingConfiguration) null;
      if (source.Length == 1 || this._modelBuilder.BindingOptions == NavigationPropertyBindingOption.Auto)
        return this.AddBinding(navigationConfiguration, source[0], bindingPath);
      throw Error.NotSupported(SRResources.CannotAutoCreateMultipleCandidates, (object) key, (object) navigationConfiguration.DeclaringType.FullName, (object) this.Name, (object) string.Join(", ", ((IEnumerable<NavigationSourceConfiguration>) source).Select<NavigationSourceConfiguration, string>((Func<NavigationSourceConfiguration, string>) (s => s.Name))));
    }

    public virtual IEnumerable<NavigationPropertyBindingConfiguration> FindBindings(
      string propertyName)
    {
      foreach (KeyValuePair<NavigationPropertyConfiguration, Dictionary<string, NavigationPropertyBindingConfiguration>> navigationPropertyBinding in this._navigationPropertyBindings)
      {
        if (navigationPropertyBinding.Key.Name == propertyName)
          return (IEnumerable<NavigationPropertyBindingConfiguration>) navigationPropertyBinding.Value.Values;
      }
      return Enumerable.Empty<NavigationPropertyBindingConfiguration>();
    }

    public virtual string GetUrl() => this._url;

    public virtual SelfLinkBuilder<Uri> GetEditLink() => this._editLinkBuilder;

    public virtual SelfLinkBuilder<Uri> GetReadLink() => this._readLinkBuilder;

    public virtual SelfLinkBuilder<Uri> GetIdLink() => this._idLinkBuilder;

    public virtual NavigationLinkBuilder GetNavigationPropertyLink(
      NavigationPropertyConfiguration navigationProperty)
    {
      if (navigationProperty == null)
        throw Error.ArgumentNull(nameof (navigationProperty));
      NavigationLinkBuilder navigationPropertyLink;
      this._navigationPropertyLinkBuilders.TryGetValue(navigationProperty, out navigationPropertyLink);
      return navigationPropertyLink;
    }

    private void VerifyBindingPath(
      NavigationPropertyConfiguration navigationConfiguration,
      IList<MemberInfo> bindingPath)
    {
      PropertyInfo propertyInfo = bindingPath.Last<MemberInfo>() as PropertyInfo;
      if (propertyInfo == (PropertyInfo) null || propertyInfo != navigationConfiguration.PropertyInfo)
        throw Error.Argument(nameof (navigationConfiguration), SRResources.NavigationPropertyBindingPathIsNotValid, (object) bindingPath.ConvertBindingPath(), (object) navigationConfiguration.Name);
      bindingPath.Aggregate<MemberInfo, Type>(this.EntityType.ClrType, new Func<Type, MemberInfo, Type>(NavigationSourceConfiguration.VerifyBindingSegment));
    }

    private static Type VerifyBindingSegment(Type current, MemberInfo info)
    {
      TypeInfo typeInfo = info as TypeInfo;
      if ((Type) typeInfo != (Type) null)
      {
        if (!typeInfo.IsAssignableFrom(current) && !current.IsAssignableFrom(typeInfo.BaseType))
          throw Error.InvalidOperation(SRResources.NavigationPropertyBindingPathNotInHierarchy, (object) typeInfo.FullName, (object) info.Name, (object) current.FullName);
        return typeInfo.BaseType;
      }
      PropertyInfo propertyInfo = info as PropertyInfo;
      Type c = !(propertyInfo == (PropertyInfo) null) ? propertyInfo.DeclaringType : throw Error.NotSupported(SRResources.NavigationPropertyBindingPathNotSupported, (object) info.Name, (object) info.MemberType);
      if (c == (Type) null || !c.IsAssignableFrom(current) && !current.IsAssignableFrom(c))
        throw Error.InvalidOperation(SRResources.NavigationPropertyBindingPathNotInHierarchy, c == (Type) null ? (object) "Unknown Type" : (object) c.FullName, (object) info.Name, (object) current.FullName);
      Type elementType;
      return TypeHelper.IsCollection(propertyInfo.PropertyType, out elementType) ? elementType : propertyInfo.PropertyType;
    }
  }
}
