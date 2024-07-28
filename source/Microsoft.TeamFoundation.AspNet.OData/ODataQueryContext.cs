// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataQueryContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  public class ODataQueryContext
  {
    private DefaultQuerySettings _defaultQuerySettings;

    public ODataQueryContext(IEdmModel model, Type elementClrType, Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      this.ElementType = !(elementClrType == (Type) null) ? model.GetEdmType(elementClrType) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (elementClrType));
      if (this.ElementType == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (elementClrType), SRResources.ClrTypeNotInModel, (object) elementClrType.FullName);
      this.ElementClrType = elementClrType;
      this.Model = model;
      this.Path = path;
      this.NavigationSource = ODataQueryContext.GetNavigationSource(this.Model, this.ElementType, path);
      this.GetPathContext();
    }

    public ODataQueryContext(IEdmModel model, IEdmType elementType, Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (elementType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (elementType));
      this.Model = model;
      this.ElementType = elementType;
      this.Path = path;
      this.NavigationSource = ODataQueryContext.GetNavigationSource(this.Model, this.ElementType, path);
      this.GetPathContext();
    }

    internal ODataQueryContext(IEdmModel model, Type elementClrType)
      : this(model, elementClrType, (Microsoft.AspNet.OData.Routing.ODataPath) null)
    {
    }

    internal ODataQueryContext(IEdmModel model, IEdmType elementType)
      : this(model, elementType, (Microsoft.AspNet.OData.Routing.ODataPath) null)
    {
    }

    public DefaultQuerySettings DefaultQuerySettings
    {
      get
      {
        if (this._defaultQuerySettings == null)
          this._defaultQuerySettings = this.RequestContainer == null ? new DefaultQuerySettings() : this.RequestContainer.GetRequiredService<DefaultQuerySettings>();
        return this._defaultQuerySettings;
      }
    }

    public IEdmModel Model { get; private set; }

    public IEdmType ElementType { get; private set; }

    public IEdmNavigationSource NavigationSource { get; private set; }

    public Type ElementClrType { get; internal set; }

    public Microsoft.AspNet.OData.Routing.ODataPath Path { get; private set; }

    public IServiceProvider RequestContainer { get; internal set; }

    internal IEdmProperty TargetProperty { get; private set; }

    internal IEdmStructuredType TargetStructuredType { get; private set; }

    internal string TargetName { get; private set; }

    private static IEdmNavigationSource GetNavigationSource(
      IEdmModel model,
      IEdmType elementType,
      Microsoft.AspNet.OData.Routing.ODataPath odataPath)
    {
      IEdmNavigationSource navigationSource = odataPath?.NavigationSource;
      if (navigationSource != null)
        return navigationSource;
      IEdmEntityContainer entityContainer = model.EntityContainer;
      if (entityContainer == null)
        return (IEdmNavigationSource) null;
      List<IEdmEntitySet> list = entityContainer.EntitySets().Where<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (e => e.EntityType() == elementType)).ToList<IEdmEntitySet>();
      return list.Count == 1 ? (IEdmNavigationSource) list[0] : (IEdmNavigationSource) null;
    }

    private void GetPathContext()
    {
      if (this.Path != null)
      {
        IEdmProperty property;
        IEdmStructuredType structuredType;
        string name;
        EdmLibHelpers.GetPropertyAndStructuredTypeFromPath((IEnumerable<ODataPathSegment>) this.Path.Segments, out property, out structuredType, out name);
        this.TargetProperty = property;
        this.TargetStructuredType = structuredType;
        this.TargetName = name;
      }
      else
        this.TargetStructuredType = this.ElementType as IEdmStructuredType;
    }
  }
}
