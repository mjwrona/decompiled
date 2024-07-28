// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.BoundingBox
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial.Converters;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
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

    [DataMember(Name = "min")]
    public Position Min { get; private set; }

    [DataMember(Name = "max")]
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
