// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.FunctionConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Builder
{
  public class FunctionConfiguration : OperationConfiguration
  {
    internal FunctionConfiguration(ODataModelBuilder builder, string name)
      : base(builder, name)
    {
      this.IncludeInServiceDocument = true;
    }

    public override OperationKind Kind => OperationKind.Function;

    public new bool IsComposable
    {
      get => base.IsComposable;
      set => base.IsComposable = value;
    }

    public override bool IsSideEffecting => false;

    public bool SupportedInFilter { get; set; }

    public bool SupportedInOrderBy { get; set; }

    public bool IncludeInServiceDocument { get; set; }

    public FunctionConfiguration HasFunctionLink(
      Func<ResourceContext, Uri> functionLinkFactory,
      bool followsConventions)
    {
      if (functionLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (functionLinkFactory));
      if (!this.IsBindable || this.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Entity)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.HasFunctionLinkRequiresBindToEntity, (object) this.Name);
      this.OperationLinkBuilder = new OperationLinkBuilder(functionLinkFactory, followsConventions);
      this.FollowsConventions = followsConventions;
      return this;
    }

    public Func<ResourceContext, Uri> GetFunctionLink() => this.OperationLinkBuilder == null ? (Func<ResourceContext, Uri>) null : this.OperationLinkBuilder.LinkFactory;

    public FunctionConfiguration HasFeedFunctionLink(
      Func<ResourceSetContext, Uri> functionLinkFactory,
      bool followsConventions)
    {
      if (functionLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (functionLinkFactory));
      if (!this.IsBindable || this.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Collection || ((CollectionTypeConfiguration) this.BindingParameter.TypeConfiguration).ElementType.Kind != EdmTypeKind.Entity)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.HasFunctionLinkRequiresBindToCollectionOfEntity, (object) this.Name);
      this.OperationLinkBuilder = new OperationLinkBuilder(functionLinkFactory, followsConventions);
      this.FollowsConventions = followsConventions;
      return this;
    }

    public Func<ResourceSetContext, Uri> GetFeedFunctionLink() => this.OperationLinkBuilder == null ? (Func<ResourceSetContext, Uri>) null : this.OperationLinkBuilder.FeedLinkFactory;

    public FunctionConfiguration ReturnsFromEntitySet<TEntityType>(string entitySetName) where TEntityType : class
    {
      this.ReturnsFromEntitySetImplementation<TEntityType>(entitySetName);
      return this;
    }

    public FunctionConfiguration ReturnsCollectionFromEntitySet<TElementEntityType>(
      string entitySetName)
      where TElementEntityType : class
    {
      this.ReturnsCollectionFromEntitySetImplementation<TElementEntityType>(entitySetName);
      return this;
    }

    public FunctionConfiguration Returns(Type clrReturnType)
    {
      if (clrReturnType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrReturnType));
      this.ReturnsImplementation(clrReturnType);
      return this;
    }

    public FunctionConfiguration Returns<TReturnType>() => this.Returns(typeof (TReturnType));

    public FunctionConfiguration ReturnsCollection<TReturnElementType>()
    {
      this.ReturnsCollectionImplementation<TReturnElementType>();
      return this;
    }

    public FunctionConfiguration SetBindingParameter(
      string name,
      IEdmTypeConfiguration bindingParameterType)
    {
      this.SetBindingParameterImplementation(name, bindingParameterType);
      return this;
    }

    public FunctionConfiguration ReturnsEntityViaEntitySetPath<TEntityType>(string entitySetPath) where TEntityType : class
    {
      if (string.IsNullOrEmpty(entitySetPath))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetPath));
      this.ReturnsEntityViaEntitySetPathImplementation<TEntityType>((IEnumerable<string>) entitySetPath.Split('/'));
      return this;
    }

    public FunctionConfiguration ReturnsEntityViaEntitySetPath<TEntityType>(
      params string[] entitySetPath)
      where TEntityType : class
    {
      this.ReturnsEntityViaEntitySetPathImplementation<TEntityType>((IEnumerable<string>) entitySetPath);
      return this;
    }

    public FunctionConfiguration ReturnsCollectionViaEntitySetPath<TElementEntityType>(
      string entitySetPath)
      where TElementEntityType : class
    {
      if (string.IsNullOrEmpty(entitySetPath))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entitySetPath));
      this.ReturnsCollectionViaEntitySetPathImplementation<TElementEntityType>((IEnumerable<string>) entitySetPath.Split('/'));
      return this;
    }

    public FunctionConfiguration ReturnsCollectionViaEntitySetPath<TElementEntityType>(
      params string[] entitySetPath)
      where TElementEntityType : class
    {
      this.ReturnsCollectionViaEntitySetPathImplementation<TElementEntityType>((IEnumerable<string>) entitySetPath);
      return this;
    }
  }
}
