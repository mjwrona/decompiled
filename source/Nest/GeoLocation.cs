// Decompiled with JetBrains decompiler
// Type: Nest.GeoLocation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (GeoLocationFormatter))]
  public class GeoLocation : IEquatable<GeoLocation>, IFormattable
  {
    public GeoLocation(double latitude, double longitude)
    {
      this.Latitude = latitude;
      this.Longitude = longitude;
    }

    [DataMember(Name = "lat")]
    public double Latitude { get; }

    [DataMember(Name = "lon")]
    public double Longitude { get; }

    [IgnoreDataMember]
    internal GeoFormat Format { get; set; }

    public bool Equals(GeoLocation other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      double num = this.Latitude;
      if (!num.Equals(other.Latitude))
        return false;
      num = this.Longitude;
      return num.Equals(other.Longitude);
    }

    public string ToString(string format, IFormatProvider formatProvider) => this.ToString();

    public static bool IsValidLatitude(double latitude) => latitude >= -90.0 && latitude <= 90.0;

    public static bool IsValidLongitude(double longitude) => longitude >= -180.0 && longitude <= 180.0;

    public static GeoLocation TryCreate(double latitude, double longitude) => new GeoLocation(latitude, longitude);

    public override string ToString()
    {
      double num = this.Latitude;
      string str1 = num.ToString("#0.0#######", (IFormatProvider) CultureInfo.InvariantCulture);
      num = this.Longitude;
      string str2 = num.ToString("#0.0#######", (IFormatProvider) CultureInfo.InvariantCulture);
      return str1 + "," + str2;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((GeoLocation) obj);
    }

    public override int GetHashCode()
    {
      double num1 = this.Latitude;
      int num2 = num1.GetHashCode() * 397;
      num1 = this.Longitude;
      int hashCode = num1.GetHashCode();
      return num2 ^ hashCode;
    }

    public static implicit operator GeoLocation(string latLon)
    {
      string[] strArray = !string.IsNullOrEmpty(latLon) ? latLon.Split(',') : throw new ArgumentNullException(nameof (latLon));
      if (strArray.Length != 2)
        throw new ArgumentException("Invalid format: string must be in the form of lat,lon");
      double result1;
      if (!double.TryParse(strArray[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        throw new ArgumentException("Invalid latitude value");
      double result2;
      if (!double.TryParse(strArray[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        throw new ArgumentException("Invalid longitude value");
      return new GeoLocation(result1, result2);
    }

    public static implicit operator GeoLocation(double[] lonLat) => lonLat.Length == 2 ? new GeoLocation(lonLat[1], lonLat[0]) : (GeoLocation) null;
  }
}
