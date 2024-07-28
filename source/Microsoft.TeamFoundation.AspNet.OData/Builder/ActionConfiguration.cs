// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ActionConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Builder
{
  public class ActionConfiguration : OperationConfiguration
  {
    internal ActionConfiguration(ODataModelBuilder builder, string name)
      : base(builder, name)
    {
    }

    public override OperationKind Kind => OperationKind.Action;

    public override bool IsSideEffecting => true;

    public ActionConfiguration HasActionLink(
      Func<ResourceContext, Uri> actionLinkFactory,
      bool followsConventions)
    {
      if (actionLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionLinkFactory));
      if (!this.IsBindable || this.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Entity)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.HasActionLinkRequiresBindToEntity, (object) this.Name);
      this.OperationLinkBuilder = new OperationLinkBuilder(actionLinkFactory, followsConventions);
      this.FollowsConventions = followsConventions;
      return this;
    }

    public Func<ResourceContext, Uri> GetActionLink() => this.OperationLinkBuilder == null ? (Func<ResourceContext, Uri>) null : this.OperationLinkBuilder.LinkFactory;

    public ActionConfiguration HasFeedActionLink(
      Func<ResourceSetContext, Uri> actionLinkFactory,
      bool followsConventions)
    {
      if (actionLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionLinkFactory));
      if (!this.IsBindable || this.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Collection || ((CollectionTypeConfiguration) this.BindingParameter.TypeConfiguration).ElementType.Kind != EdmTypeKind.Entity)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.HasActionLinkRequiresBindToCollectionOfEntity, (object) this.Name);
      this.OperationLinkBuilder = new OperationLinkBuilder(actionLinkFactory, followsConventions);
      this.FollowsConventions = followsConventions;
      return this;
    }

    public Func<ResourceSetContext, Uri> GetFeedActionLink() => this.OperationLinkBuilder == null ? (Func<ResourceSetContext, Uri>) null : this.OperationLinkBuilder.FeedLinkFactory;

    public ActionConfiguration ReturnsFromEntitySet<TEntityType>(string entitySetName) where TEntityType : class
    {
      this.ReturnsFromEntitySetImplementation<TEntityType>(entitySetName);
      return this;
    }

    public ActionConfiguration ReturnsFromEntitySet<TEntityType>(
      EntitySetConfiguration<TEntityType> entitySetConfiguration)
      where TEntityType : class
    {
      this.NavigationSource = entitySetConfiguration != null ? (NavigationSourceConfiguration) entitySetConfiguration.EntitySet : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetConfiguration));
      this.ReturnType = this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TEntityType));
      return this;
    }

    public ActionConfiguration ReturnsCollectionFromEntitySet<TElementEntityType>(
      string entitySetName)
      where TElementEntityType : class
    {
      this.ReturnsCollectionFromEntitySetImplementation<TElementEntityType>(entitySetName);
      return this;
    }

    public ActionConfiguration ReturnsCollectionFromEntitySet<TElementEntityType>(
      EntitySetConfiguration<TElementEntityType> entitySetConfiguration)
      where TElementEntityType : class
    {
      if (entitySetConfiguration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetConfiguration));
      Type clrType = typeof (IEnumerable<TElementEntityType>);
      this.NavigationSource = (NavigationSourceConfiguration) entitySetConfiguration.EntitySet;
      this.ReturnType = (IEdmTypeConfiguration) new CollectionTypeConfiguration(this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TElementEntityType)), clrType);
      return this;
    }

    public ActionConfiguration Returns(Type clrReturnType)
    {
      IEdmTypeConfiguration typeConfiguration = !(clrReturnType == (Type) null) ? this.ModelBuilder.GetTypeConfigurationOrNull(clrReturnType) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrReturnType));
      if (typeConfiguration is EntityTypeConfiguration)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReturnEntityWithoutEntitySet, (object) typeConfiguration.FullName);
      this.ReturnsImplementation(clrReturnType);
      return this;
    }

    public ActionConfiguration Returns<TReturnType>() => this.Returns(typeof (TReturnType));

    public ActionConfiguration ReturnsCollection<TReturnElementType>()
    {
      IEdmTypeConfiguration configurationOrNull = this.ModelBuilder.GetTypeConfigurationOrNull(typeof (TReturnElementType));
      if (configurationOrNull is EntityTypeConfiguration)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReturnEntityCollectionWithoutEntitySet, (object) configurationOrNull.FullName);
      this.ReturnsCollectionImplementation<TReturnElementType>();
      return this;
    }

    public ActionConfiguration SetBindingParameter(
      string name,
      IEdmTypeConfiguration bindingParameterType)
    {
      this.SetBindingParameterImplementation(name, bindingParameterType);
      return this;
    }

    public ActionConfiguration ReturnsEntityViaEntitySetPath<TEntityType>(string entitySetPath) where TEntityType : class
    {
      if (string.IsNullOrEmpty(entitySetPath))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetPath));
      this.ReturnsEntityViaEntitySetPathImplementation<TEntityType>((IEnumerable<string>) entitySetPath.Split('/'));
      return this;
    }

    public ActionConfiguration ReturnsEntityViaEntitySetPath<TEntityType>(
      params string[] entitySetPath)
      where TEntityType : class
    {
      this.ReturnsEntityViaEntitySetPathImplementation<TEntityType>((IEnumerable<string>) entitySetPath);
      return this;
    }

    public ActionConfiguration ReturnsCollectionViaEntitySetPath<TElementEntityType>(
      string entitySetPath)
      where TElementEntityType : class
    {
      if (string.IsNullOrEmpty(entitySetPath))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetPath));
      this.ReturnsCollectionViaEntitySetPathImplementation<TElementEntityType>((IEnumerable<string>) entitySetPath.Split('/'));
      return this;
    }

    public ActionConfiguration ReturnsCollectionViaEntitySetPath<TElementEntityType>(
      params string[] entitySetPath)
      where TElementEntityType : class
    {
      this.ReturnsCollectionViaEntitySetPathImplementation<TElementEntityType>((IEnumerable<string>) entitySetPath);
      return this;
    }
  }
}
