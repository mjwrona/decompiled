// Decompiled with JetBrains decompiler
// Type: Nest.CartesianPoint
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;

namespace Nest
{
  [JsonFormatter(typeof (CartesianPointFormatter))]
  public class CartesianPoint : IEquatable<CartesianPoint>
  {
    internal ShapeFormat Format;

    public float X { get; set; }

    public float Y { get; set; }

    public CartesianPoint()
    {
    }

    public CartesianPoint(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public bool Equals(CartesianPoint other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      float num = this.X;
      if (!num.Equals(other.X))
        return false;
      num = this.Y;
      return num.Equals(other.Y);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((CartesianPoint) obj);
    }

    public override int GetHashCode()
    {
      float num1 = this.X;
      int num2 = num1.GetHashCode() * 397;
      num1 = this.Y;
      int hashCode = num1.GetHashCode();
      return num2 ^ hashCode;
    }

    public static CartesianPoint FromCoordinates(string coordinates)
    {
      string[] strArray = coordinates.Split(',');
      string s1 = strArray.Length <= 3 && strArray.Length >= 2 ? strArray[0].Trim() : throw new InvalidOperationException(string.Format("failed to parse {0}, expected 2 or 3 coordinates but found: {1}", (object) coordinates, (object) strArray.Length));
      float result1;
      if (!float.TryParse(s1, out result1))
        throw new InvalidOperationException("failed to parse float for x from " + s1);
      string s2 = strArray[1].Trim();
      float result2;
      if (!float.TryParse(s2, out result2))
        throw new InvalidOperationException("failed to parse float for y from " + s2);
      if (strArray.Length > 2)
      {
        string s3 = strArray[2].Trim();
        if (!float.TryParse(s3, out float _))
          throw new InvalidOperationException("failed to parse float for z from " + s3);
      }
      return new CartesianPoint(result1, result2)
      {
        Format = ShapeFormat.String
      };
    }

    public static CartesianPoint FromWellKnownText(string wkt)
    {
      using (WellKnownTextTokenizer tokenizer = new WellKnownTextTokenizer((TextReader) new StringReader(wkt)))
      {
        string str = tokenizer.NextToken() == TokenType.Word ? tokenizer.TokenValue.ToUpperInvariant() : throw new GeoWKTException("Expected word but found " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
        if (str != "POINT")
          throw new GeoWKTException("Expected POINT but found " + str, tokenizer.LineNumber, tokenizer.Position);
        if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
          return (CartesianPoint) null;
        float single1 = Convert.ToSingle(GeoWKTReader.NextNumber(tokenizer));
        float single2 = Convert.ToSingle(GeoWKTReader.NextNumber(tokenizer));
        if (GeoWKTReader.IsNumberNext(tokenizer))
          GeoWKTReader.NextNumber(tokenizer);
        CartesianPoint cartesianPoint = new CartesianPoint(single1, single2)
        {
          Format = ShapeFormat.WellKnownText
        };
        GeoWKTReader.NextCloser(tokenizer);
        return cartesianPoint;
      }
    }

    public static implicit operator CartesianPoint(string value)
    {
      try
      {
        return value.IndexOf(",", StringComparison.InvariantCultureIgnoreCase) > -1 ? CartesianPoint.FromCoordinates(value) : CartesianPoint.FromWellKnownText(value);
      }
      catch
      {
        return (CartesianPoint) null;
      }
    }

    public static bool operator ==(CartesianPoint left, CartesianPoint right) => object.Equals((object) left, (object) right);

    public static bool operator !=(CartesianPoint left, CartesianPoint right) => !object.Equals((object) left, (object) right);
  }
}
