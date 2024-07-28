// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.OrderByAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class OrderByAttribute : Attribute
  {
    private bool? _defaultEnableOrderBy;
    private bool _disable;
    private readonly Dictionary<string, bool> _orderByConfigurations = new Dictionary<string, bool>();

    public OrderByAttribute() => this._defaultEnableOrderBy = new bool?(true);

    public OrderByAttribute(params string[] properties)
    {
      foreach (string property in properties)
      {
        if (!this._orderByConfigurations.ContainsKey(property))
          this._orderByConfigurations.Add(property, true);
      }
    }

    public Dictionary<string, bool> OrderByConfigurations => this._orderByConfigurations;

    public bool Disabled
    {
      get => this._disable;
      set
      {
        this._disable = value;
        foreach (string key in this._orderByConfigurations.Keys.ToList<string>())
          this._orderByConfigurations[key] = !this._disable;
        if (this._orderByConfigurations.Count != 0)
          return;
        this._defaultEnableOrderBy = new bool?(!this._disable);
      }
    }

    internal bool? DefaultEnableOrderBy
    {
      get => this._defaultEnableOrderBy;
      set => this._defaultEnableOrderBy = value;
    }
  }
}
