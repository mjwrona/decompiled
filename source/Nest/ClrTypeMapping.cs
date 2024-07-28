// Decompiled with JetBrains decompiler
// Type: Nest.ClrTypeMapping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ClrTypeMapping : IClrTypeMapping
  {
    public ClrTypeMapping(Type type) => this.ClrType = type;

    public Type ClrType { get; }

    public string IdPropertyName { get; set; }

    public string IndexName { get; set; }

    public string RelationName { get; set; }

    public bool DisableIdInference { get; set; }
  }
}
