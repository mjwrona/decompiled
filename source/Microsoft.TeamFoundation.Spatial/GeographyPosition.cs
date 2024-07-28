// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyPosition
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Spatial
{
  public class GeographyPosition : IEquatable<GeographyPosition>
  {
    private readonly double latitude;
    private readonly double longitude;
    private readonly double? m;
    private readonly double? z;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public GeographyPosition(double latitude, double longitude, double? z, double? m)
    {
      this.latitude = latitude;
      this.longitude = longitude;
      this.z = z;
      this.m = m;
    }

    public GeographyPosition(double latitude, double longitude)
      : this(latitude, longitude, new double?(), new double?())
    {
    }

    public double Latitude => this.latitude;

    public double Longitude => this.longitude;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double? M => this.m;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z, m make sense in context")]
    public double? Z => this.z;

    public static bool operator ==(GeographyPosition left, GeographyPosition right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool operator !=(GeographyPosition left, GeographyPosition right) => !(left == right);

    public override bool Equals(object obj) => obj != null && !(obj.GetType() != typeof (GeographyPosition)) && this.Equals((GeographyPosition) obj);

    public bool Equals(GeographyPosition other)
    {
      if (other != (GeographyPosition) null)
      {
        double num = other.latitude;
        if (num.Equals(this.latitude))
        {
          num = other.longitude;
          if (num.Equals(this.longitude))
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
      int num1 = (this.latitude.GetHashCode() * 397 ^ this.longitude.GetHashCode()) * 397;
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
        (object) this.latitude,
        (object) this.longitude,
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
      return string.Format((IFormatProvider) invariantCulture, "GeographyPosition(latitude:{0}, longitude:{1}, z:{2}, m:{3})", objArray);
    }
  }
}
