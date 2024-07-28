// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.LineStringCoordinates
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.Documents.Spatial
{
  [JsonConverter(typeof (LineStringCoordinatesJsonConverter))]
  internal sealed class LineStringCoordinates : IEquatable<LineStringCoordinates>
  {
    public LineStringCoordinates(IList<Position> positions) => this.Positions = positions != null ? new ReadOnlyCollection<Position>(positions) : throw new ArgumentException("points");

    public ReadOnlyCollection<Position> Positions { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as LineStringCoordinates);

    public override int GetHashCode() => this.Positions.Aggregate<Position, int>(0, (Func<int, Position, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(LineStringCoordinates other)
    {
      if (other == null)
        return false;
      return this == other || this.Positions.SequenceEqual<Position>((IEnumerable<Position>) other.Positions);
    }
  }
}
