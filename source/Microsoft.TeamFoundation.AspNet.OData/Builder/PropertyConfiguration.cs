// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PropertyConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class PropertyConfiguration
  {
    private string _name;

    protected PropertyConfiguration(
      PropertyInfo property,
      StructuralTypeConfiguration declaringType)
    {
      if (property == (PropertyInfo) null)
        throw Error.ArgumentNull(nameof (property));
      if (declaringType == null)
        throw Error.ArgumentNull(nameof (declaringType));
      this.PropertyInfo = property;
      this.DeclaringType = declaringType;
      this.AddedExplicitly = true;
      this._name = property.Name;
      this.QueryConfiguration = new QueryConfiguration();
    }

    public string Name
    {
      get => this._name;
      set => this._name = value != null ? value : throw Error.PropertyNull();
    }

    public StructuralTypeConfiguration DeclaringType { get; private set; }

    public PropertyInfo PropertyInfo { get; private set; }

    public abstract Type RelatedClrType { get; }

    public abstract PropertyKind Kind { get; }

    public bool AddedExplicitly { get; set; }

    public bool IsRestricted => this.NotFilterable || this.NotSortable || this.NotNavigable || this.NotExpandable || this.NotCountable || this.AutoExpand;

    public bool NotFilterable { get; set; }

    public bool AutoExpand { get; set; }

    public bool DisableAutoExpandWhenSelectIsPresent { get; set; }

    public bool NonFilterable
    {
      get => this.NotFilterable;
      set => this.NotFilterable = value;
    }

    public bool NotSortable { get; set; }

    public bool Unsortable
    {
      get => this.NotSortable;
      set => this.NotSortable = value;
    }

    public bool NotNavigable { get; set; }

    public bool NotExpandable { get; set; }

    public bool NotCountable { get; set; }

    public int Order { get; set; }

    public QueryConfiguration QueryConfiguration { get; set; }

    public PropertyConfiguration IsNotFilterable()
    {
      this.NotFilterable = true;
      return this;
    }

    public PropertyConfiguration IsNonFilterable() => this.IsNotFilterable();

    public PropertyConfiguration IsFilterable()
    {
      this.NotFilterable = false;
      return this;
    }

    public PropertyConfiguration IsNotSortable()
    {
      this.NotSortable = true;
      return this;
    }

    public PropertyConfiguration IsUnsortable() => this.IsNotSortable();

    public PropertyConfiguration IsSortable()
    {
      this.NotSortable = false;
      return this;
    }

    public PropertyConfiguration IsNotNavigable()
    {
      this.IsNotSortable();
      this.IsNotFilterable();
      this.NotNavigable = true;
      return this;
    }

    public PropertyConfiguration IsNavigable()
    {
      this.NotNavigable = false;
      return this;
    }

    public PropertyConfiguration IsNotExpandable()
    {
      this.NotExpandable = true;
      return this;
    }

    public PropertyConfiguration IsExpandable()
    {
      this.NotExpandable = false;
      return this;
    }

    public PropertyConfiguration IsNotCountable()
    {
      this.NotCountable = true;
      return this;
    }

    public PropertyConfiguration IsCountable()
    {
      this.NotCountable = false;
      return this;
    }

    public PropertyConfiguration Count()
    {
      this.QueryConfiguration.SetCount(true);
      return this;
    }

    public PropertyConfiguration Count(QueryOptionSetting queryOptionSetting)
    {
      this.QueryConfiguration.SetCount(queryOptionSetting == QueryOptionSetting.Allowed);
      return this;
    }

    public PropertyConfiguration OrderBy(QueryOptionSetting setting, params string[] properties)
    {
      this.QueryConfiguration.SetOrderBy((IEnumerable<string>) properties, setting == QueryOptionSetting.Allowed);
      return this;
    }

    public PropertyConfiguration OrderBy(params string[] properties)
    {
      this.QueryConfiguration.SetOrderBy((IEnumerable<string>) properties, true);
      return this;
    }

    public PropertyConfiguration OrderBy(QueryOptionSetting setting)
    {
      this.QueryConfiguration.SetOrderBy((IEnumerable<string>) null, setting == QueryOptionSetting.Allowed);
      return this;
    }

    public PropertyConfiguration OrderBy()
    {
      this.QueryConfiguration.SetOrderBy((IEnumerable<string>) null, true);
      return this;
    }

    public PropertyConfiguration Filter(QueryOptionSetting setting, params string[] properties)
    {
      this.QueryConfiguration.SetFilter((IEnumerable<string>) properties, setting == QueryOptionSetting.Allowed);
      return this;
    }

    public PropertyConfiguration Filter(params string[] properties)
    {
      this.QueryConfiguration.SetFilter((IEnumerable<string>) properties, true);
      return this;
    }

    public PropertyConfiguration Filter(QueryOptionSetting setting)
    {
      this.QueryConfiguration.SetFilter((IEnumerable<string>) null, setting == QueryOptionSetting.Allowed);
      return this;
    }

    public PropertyConfiguration Filter()
    {
      this.QueryConfiguration.SetFilter((IEnumerable<string>) null, true);
      return this;
    }

    public PropertyConfiguration Select(SelectExpandType selectType, params string[] properties)
    {
      this.QueryConfiguration.SetSelect((IEnumerable<string>) properties, selectType);
      return this;
    }

    public PropertyConfiguration Select(params string[] properties)
    {
      this.QueryConfiguration.SetSelect((IEnumerable<string>) properties, SelectExpandType.Allowed);
      return this;
    }

    public PropertyConfiguration Select(SelectExpandType selectType)
    {
      this.QueryConfiguration.SetSelect((IEnumerable<string>) null, selectType);
      return this;
    }

    public PropertyConfiguration Select()
    {
      this.QueryConfiguration.SetSelect((IEnumerable<string>) null, SelectExpandType.Allowed);
      return this;
    }

    public PropertyConfiguration Page(int? maxTopValue, int? pageSizeValue)
    {
      this.QueryConfiguration.SetMaxTop(maxTopValue);
      this.QueryConfiguration.SetPageSize(pageSizeValue);
      return this;
    }

    public PropertyConfiguration Page()
    {
      this.QueryConfiguration.SetMaxTop(new int?());
      this.QueryConfiguration.SetPageSize(new int?());
      return this;
    }

    public PropertyConfiguration Expand(
      int maxDepth,
      SelectExpandType expandType,
      params string[] properties)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(maxDepth), expandType);
      return this;
    }

    public PropertyConfiguration Expand(params string[] properties)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(), SelectExpandType.Allowed);
      return this;
    }

    public PropertyConfiguration Expand(int maxDepth, params string[] properties)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(maxDepth), SelectExpandType.Allowed);
      return this;
    }

    public PropertyConfiguration Expand(SelectExpandType expandType, params string[] properties)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(), expandType);
      return this;
    }

    public PropertyConfiguration Expand(SelectExpandType expandType, int maxDepth)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(maxDepth), expandType);
      return this;
    }

    public PropertyConfiguration Expand(int maxDepth)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(maxDepth), SelectExpandType.Allowed);
      return this;
    }

    public PropertyConfiguration Expand(SelectExpandType expandType)
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(), expandType);
      return this;
    }

    public PropertyConfiguration Expand()
    {
      this.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(), SelectExpandType.Allowed);
      return this;
    }
  }
}
