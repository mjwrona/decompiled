// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.SelectAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class SelectAttribute : Attribute
  {
    private readonly Dictionary<string, SelectExpandType> _selectConfigurations = new Dictionary<string, SelectExpandType>();
    private SelectExpandType _selectType;
    private SelectExpandType? _defaultSelectType;

    public SelectAttribute() => this._defaultSelectType = new SelectExpandType?(SelectExpandType.Allowed);

    public SelectAttribute(params string[] properties)
    {
      foreach (string property in properties)
      {
        if (!this._selectConfigurations.ContainsKey(property))
          this._selectConfigurations.Add(property, SelectExpandType.Allowed);
      }
    }

    public Dictionary<string, SelectExpandType> SelectConfigurations => this._selectConfigurations;

    public SelectExpandType SelectType
    {
      get => this._selectType;
      set
      {
        this._selectType = value;
        foreach (string key in this._selectConfigurations.Keys.ToList<string>())
          this._selectConfigurations[key] = this._selectType;
        if (this._selectConfigurations.Count != 0)
          return;
        this._defaultSelectType = new SelectExpandType?(this._selectType);
      }
    }

    internal SelectExpandType? DefaultSelectType
    {
      get => this._defaultSelectType;
      set => this._defaultSelectType = value;
    }
  }
}
