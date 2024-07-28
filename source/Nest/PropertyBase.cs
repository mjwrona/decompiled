// Decompiled with JetBrains decompiler
// Type: Nest.PropertyBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public abstract class PropertyBase : IProperty, IFieldMapping, IPropertyWithClrOrigin
  {
    protected PropertyBase(FieldType type) => ((IProperty) this).Type = type.GetStringValue();

    public IDictionary<string, object> LocalMetadata { get; set; }

    public IDictionary<string, string> Meta { get; set; }

    public PropertyName Name { get; set; }

    protected string DebugDisplay => "Type: " + (((IProperty) this).Type ?? "<empty>") + ", Name: " + (this.Name?.DebugDisplay ?? "<empty>") + " ";

    public override string ToString() => this.DebugDisplay;

    protected string TypeOverride { get; set; }

    PropertyInfo IPropertyWithClrOrigin.ClrOrigin { get; set; }

    string IProperty.Type
    {
      get => this.TypeOverride;
      set => this.TypeOverride = value;
    }
  }
}
