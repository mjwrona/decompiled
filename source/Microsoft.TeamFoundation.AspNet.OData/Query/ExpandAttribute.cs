// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ExpandAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class ExpandAttribute : Attribute
  {
    private readonly Dictionary<string, ExpandConfiguration> _expandConfigurations = new Dictionary<string, ExpandConfiguration>();
    private SelectExpandType _expandType;
    private SelectExpandType? _defaultExpandType;
    private int? _defaultMaxDepth;
    private int _maxDepth;

    public ExpandAttribute()
    {
      this._defaultExpandType = new SelectExpandType?(SelectExpandType.Allowed);
      this._defaultMaxDepth = new int?(2);
    }

    public ExpandAttribute(params string[] properties)
    {
      foreach (string property in properties)
      {
        if (!this._expandConfigurations.ContainsKey(property))
          this._expandConfigurations.Add(property, new ExpandConfiguration()
          {
            ExpandType = SelectExpandType.Allowed,
            MaxDepth = 2
          });
      }
    }

    public Dictionary<string, ExpandConfiguration> ExpandConfigurations => this._expandConfigurations;

    public SelectExpandType ExpandType
    {
      get => this._expandType;
      set
      {
        this._expandType = value;
        foreach (string key in this._expandConfigurations.Keys)
          this._expandConfigurations[key].ExpandType = this._expandType;
        if (this._expandConfigurations.Count != 0)
          return;
        this._defaultExpandType = new SelectExpandType?(this._expandType);
      }
    }

    public int MaxDepth
    {
      get => this._maxDepth;
      set
      {
        this._maxDepth = value;
        foreach (string key in this._expandConfigurations.Keys)
          this._expandConfigurations[key].MaxDepth = this._maxDepth;
        if (this._expandConfigurations.Count != 0)
          return;
        this._defaultMaxDepth = new int?(this._maxDepth);
      }
    }

    internal SelectExpandType? DefaultExpandType
    {
      get => this._defaultExpandType;
      set => this._defaultExpandType = value;
    }

    internal int? DefaultMaxDepth
    {
      get => this._defaultMaxDepth;
      set => this._defaultMaxDepth = value;
    }
  }
}
