// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryPosition
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Spatial
{
  public class GeometryPosition : IEquatable<GeometryPosition>
  {
    private readonly double? m;
    private readonly double x;
    private readonly double y;
    private readonly double? z;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public GeometryPosition(double x, double y, double? z, double? m)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.m = m;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y make sense in context")]
    public GeometryPosition(double x, double y)
      : this(x, y, new double?(), new double?())
    {
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double? M => this.m;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double X => this.x;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double Y => this.y;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double? Z => this.z;

    public static bool operator ==(GeometryPosition left, GeometryPosition right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool operator !=(GeometryPosition left, GeometryPosition right) => !(left == right);

    public override bool Equals(object obj) => obj != null && !(obj.GetType() != typeof (GeometryPosition)) && this.Equals((GeometryPosition) obj);

    public bool Equals(GeometryPosition other)
    {
      if (other != (GeometryPosition) null)
      {
        double num = other.x;
        if (num.Equals(this.x))
        {
          num = other.y;
          if (num.Equals(this.y))
          {
            double? nullable = other.z;
            if (nullable.Equals((object) this.z))
            {
              nullable = other.m;
              return nullable.Equals((object) this.m);
            }
          }
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      int num1 = (this.x.GetHashCode() * 397 ^ this.y.GetHashCode()) * 397;
      double num2;
      int num3;
      if (!this.z.HasValue)
      {
        num3 = 0;
      }
      else
      {
        num2 = this.z.Value;
        num3 = num2.GetHashCode();
      }
      int num4 = (num1 ^ num3) * 397;
      double? m = this.m;
      int num5;
      if (!m.HasValue)
      {
        num5 = 0;
      }
      else
      {
        m = this.m;
        num2 = m.Value;
        num5 = num2.GetHashCode();
      }
      return num4 ^ num5;
    }

    public override string ToString()
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      object[] objArray = new object[4]
      {
        (object) this.x,
        (object) this.y,
        null,
        null
      };
      double? nullable;
      string str1;
      if (!this.z.HasValue)
      {
        str1 = "null";
      }
      else
      {
        nullable = this.z;
        str1 = nullable.ToString();
      }
      objArray[2] = (object) str1;
      nullable = this.m;
      string str2;
      if (!nullable.HasValue)
      {
        str2 = "null";
      }
      else
      {
        nullable = this.m;
        str2 = nullable.ToString();
      }
      objArray[3] = (object) str2;
      return string.Format((IFormatProvider) invariantCulture, "GeometryPosition({0}, {1}, {2}, {3})", objArray);
    }
  }
}
