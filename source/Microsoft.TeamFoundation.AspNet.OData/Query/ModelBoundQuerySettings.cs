// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ModelBoundQuerySettings
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query
{
  public class ModelBoundQuerySettings
  {
    private int? _pageSize;
    private int? _maxTop = new int?(0);
    private Dictionary<string, ExpandConfiguration> _expandConfigurations = new Dictionary<string, ExpandConfiguration>();
    private Dictionary<string, SelectExpandType> _selectConfigurations = new Dictionary<string, SelectExpandType>();
    private Dictionary<string, bool> _orderByConfigurations = new Dictionary<string, bool>();
    private Dictionary<string, bool> _filterConfigurations = new Dictionary<string, bool>();
    internal static ModelBoundQuerySettings DefaultModelBoundQuerySettings = new ModelBoundQuerySettings()
    {
      _maxTop = new int?(0)
    };

    public ModelBoundQuerySettings()
    {
    }

    public ModelBoundQuerySettings(ModelBoundQuerySettings querySettings)
    {
      this._maxTop = querySettings.MaxTop;
      this.PageSize = querySettings.PageSize;
      this.Countable = querySettings.Countable;
      this.DefaultEnableFilter = querySettings.DefaultEnableFilter;
      this.DefaultEnableOrderBy = querySettings.DefaultEnableOrderBy;
      this.DefaultExpandType = querySettings.DefaultExpandType;
      this.DefaultMaxDepth = querySettings.DefaultMaxDepth;
      this.DefaultSelectType = querySettings.DefaultSelectType;
      this.CopyOrderByConfigurations(querySettings.OrderByConfigurations);
      this.CopyFilterConfigurations(querySettings.FilterConfigurations);
      this.CopyExpandConfigurations(querySettings.ExpandConfigurations);
      this.CopySelectConfigurations(querySettings.SelectConfigurations);
    }

    public int? MaxTop
    {
      get => this._maxTop;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
        }
        this._maxTop = value;
      }
    }

    public int? PageSize
    {
      get => this._pageSize;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num = 0;
          if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
            throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value, (object) 1);
        }
        this._pageSize = value;
      }
    }

    public bool? Countable { get; set; }

    public Dictionary<string, ExpandConfiguration> ExpandConfigurations => this._expandConfigurations;

    public SelectExpandType? DefaultExpandType { get; set; }

    public int DefaultMaxDepth { get; set; }

    public bool? DefaultEnableOrderBy { get; set; }

    public bool? DefaultEnableFilter { get; set; }

    public SelectExpandType? DefaultSelectType { get; set; }

    public Dictionary<string, bool> OrderByConfigurations => this._orderByConfigurations;

    public Dictionary<string, bool> FilterConfigurations => this._filterConfigurations;

    public Dictionary<string, SelectExpandType> SelectConfigurations => this._selectConfigurations;

    internal void CopyExpandConfigurations(
      Dictionary<string, ExpandConfiguration> expandConfigurations)
    {
      this._expandConfigurations.Clear();
      foreach (KeyValuePair<string, ExpandConfiguration> expandConfiguration in expandConfigurations)
        this._expandConfigurations.Add(expandConfiguration.Key, expandConfiguration.Value);
    }

    internal void CopyOrderByConfigurations(Dictionary<string, bool> orderByConfigurations)
    {
      this._orderByConfigurations.Clear();
      foreach (KeyValuePair<string, bool> orderByConfiguration in orderByConfigurations)
        this._orderByConfigurations.Add(orderByConfiguration.Key, orderByConfiguration.Value);
    }

    internal void CopySelectConfigurations(
      Dictionary<string, SelectExpandType> selectConfigurations)
    {
      this._selectConfigurations.Clear();
      foreach (KeyValuePair<string, SelectExpandType> selectConfiguration in selectConfigurations)
        this._selectConfigurations.Add(selectConfiguration.Key, selectConfiguration.Value);
    }

    internal void CopyFilterConfigurations(Dictionary<string, bool> filterConfigurations)
    {
      this._filterConfigurations.Clear();
      foreach (KeyValuePair<string, bool> filterConfiguration in filterConfigurations)
        this._filterConfigurations.Add(filterConfiguration.Key, filterConfiguration.Value);
    }

    internal bool Expandable(string propertyName)
    {
      ExpandConfiguration expandConfiguration;
      if (this.ExpandConfigurations.TryGetValue(propertyName, out expandConfiguration))
        return expandConfiguration.ExpandType != SelectExpandType.Disabled;
      if (!this.DefaultExpandType.HasValue)
        return false;
      SelectExpandType? defaultExpandType = this.DefaultExpandType;
      SelectExpandType selectExpandType = SelectExpandType.Disabled;
      return !(defaultExpandType.GetValueOrDefault() == selectExpandType & defaultExpandType.HasValue);
    }

    internal bool Selectable(string propertyName)
    {
      SelectExpandType selectExpandType1;
      if (this.SelectConfigurations.TryGetValue(propertyName, out selectExpandType1))
        return selectExpandType1 != SelectExpandType.Disabled;
      if (!this.DefaultSelectType.HasValue)
        return false;
      SelectExpandType? defaultSelectType = this.DefaultSelectType;
      SelectExpandType selectExpandType2 = SelectExpandType.Disabled;
      return !(defaultSelectType.GetValueOrDefault() == selectExpandType2 & defaultSelectType.HasValue);
    }

    internal bool Sortable(string propertyName)
    {
      bool flag1;
      if (this.OrderByConfigurations.TryGetValue(propertyName, out flag1))
        return flag1;
      bool? defaultEnableOrderBy = this.DefaultEnableOrderBy;
      bool flag2 = true;
      return defaultEnableOrderBy.GetValueOrDefault() == flag2 & defaultEnableOrderBy.HasValue;
    }

    internal bool Filterable(string propertyName)
    {
      bool flag1;
      if (this.FilterConfigurations.TryGetValue(propertyName, out flag1))
        return flag1;
      bool? defaultEnableFilter = this.DefaultEnableFilter;
      bool flag2 = true;
      return defaultEnableFilter.GetValueOrDefault() == flag2 & defaultEnableFilter.HasValue;
    }

    internal bool IsAutomaticExpand(string propertyName)
    {
      ExpandConfiguration expandConfiguration;
      if (this.ExpandConfigurations.TryGetValue(propertyName, out expandConfiguration))
        return expandConfiguration.ExpandType == SelectExpandType.Automatic;
      if (!this.DefaultExpandType.HasValue)
        return false;
      SelectExpandType? defaultExpandType = this.DefaultExpandType;
      SelectExpandType selectExpandType = SelectExpandType.Automatic;
      return defaultExpandType.GetValueOrDefault() == selectExpandType & defaultExpandType.HasValue;
    }

    internal bool IsAutomaticSelect(string propertyName)
    {
      SelectExpandType selectExpandType1;
      if (this.SelectConfigurations.TryGetValue(propertyName, out selectExpandType1))
        return selectExpandType1 == SelectExpandType.Automatic;
      if (!this.DefaultSelectType.HasValue)
        return false;
      SelectExpandType? defaultSelectType = this.DefaultSelectType;
      SelectExpandType selectExpandType2 = SelectExpandType.Automatic;
      return defaultSelectType.GetValueOrDefault() == selectExpandType2 & defaultSelectType.HasValue;
    }
  }
}
