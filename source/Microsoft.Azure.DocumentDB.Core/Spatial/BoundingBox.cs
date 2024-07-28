// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.BoundingBox
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial.Converters;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents.Spatial
{
  [JsonConverter(typeof (BoundingBoxJsonConverter))]
  public sealed class BoundingBox : IEquatable<BoundingBox>
  {
    public BoundingBox(Position min, Position max)
    {
      if (max == null)
        throw new ArgumentException(nameof (Max));
      if (min == null)
        throw new ArgumentException(nameof (Min));
      if (max.Coordinates.Count != min.Coordinates.Count)
        throw new ArgumentException("Max and min must have same cardinality.");
      this.Max = max;
      this.Min = min;
    }

    public Position Min { get; private set; }

    public Position Max { get; private set; }

    public bool Equals(BoundingBox other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.Min.Equals(other.Min) && this.Max.Equals(other.Max);
    }

    public override bool Equals(object obj) => this.Equals(obj as BoundingBox);

    public override int GetHashCode() => this.Min.GetHashCode() * 397 ^ this.Max.GetHashCode();
  }
}
