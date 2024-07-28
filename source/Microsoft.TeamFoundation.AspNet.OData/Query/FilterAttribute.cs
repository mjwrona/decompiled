// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.FilterAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class FilterAttribute : Attribute
  {
    private bool? _defaultEnableFilter;
    private bool _disable;
    private readonly Dictionary<string, bool> _filterConfigurations = new Dictionary<string, bool>();

    public FilterAttribute() => this._defaultEnableFilter = new bool?(true);

    public FilterAttribute(params string[] properties)
    {
      foreach (string property in properties)
        this._filterConfigurations.Add(property, true);
    }

    public Dictionary<string, bool> FilterConfigurations => this._filterConfigurations;

    public bool Disabled
    {
      get => this._disable;
      set
      {
        this._disable = value;
        foreach (string key in this._filterConfigurations.Keys.ToList<string>())
          this._filterConfigurations[key] = !this._disable;
        if (this._filterConfigurations.Count != 0)
          return;
        this._defaultEnableFilter = new bool?(!this._disable);
      }
    }

    internal bool? DefaultEnableFilter
    {
      get => this._defaultEnableFilter;
      set => this._defaultEnableFilter = value;
    }
  }
}
