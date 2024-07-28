// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TuplePropertyType
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public sealed class TuplePropertyType : ScopePropertyType
  {
    private List<PropertyType> items;

    public TuplePropertyType()
      : base(TypeKind.Tuple)
    {
      this.items = new List<PropertyType>();
    }

    [JsonProperty(PropertyName = "items")]
    public List<PropertyType> Items
    {
      get => this.items;
      set => this.items = value ?? new List<PropertyType>();
    }
  }
}
