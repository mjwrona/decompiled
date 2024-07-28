// Decompiled with JetBrains decompiler
// Type: Nest.Distance
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nest
{
  [JsonFormatter(typeof (DistanceFormatter))]
  public class Distance
  {
    private static readonly Regex DistanceUnitRegex = new Regex("^(?<precision>\\d+(?:\\.\\d+)?)(?<unit>\\D+)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    public Distance(double distance)
      : this(distance, DistanceUnit.Meters)
    {
    }

    public Distance(double distance, DistanceUnit unit)
    {
      this.Precision = distance;
      this.Unit = unit;
    }

    public Distance(string distanceUnit)
    {
      distanceUnit.ThrowIfNullOrEmpty(nameof (distanceUnit));
      Match match = Distance.DistanceUnitRegex.Match(distanceUnit);
      if (!match.Success)
        throw new ArgumentException("must be a valid distance unit", nameof (distanceUnit));
      double num = double.Parse(match.Groups["precision"].Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture);
      string str = match.Groups["unit"].Value;
      this.Precision = num;
      if (string.IsNullOrEmpty(str))
        this.Unit = DistanceUnit.Meters;
      else
        this.Unit = (str.ToEnum<DistanceUnit>() ?? throw new InvalidCastException("cannot parse " + typeof (DistanceUnit).Name + " from string '" + str + "'")).Value;
    }

    public double Precision { get; private set; }

    public DistanceUnit Unit { get; private set; }

    public static Distance Inches(double inches) => new Distance(inches, DistanceUnit.Inch);

    public static Distance Yards(double yards) => new Distance(yards, DistanceUnit.Yards);

    public static Distance Miles(double miles) => new Distance(miles, DistanceUnit.Miles);

    public static Distance Kilometers(double kilometers) => new Distance(kilometers, DistanceUnit.Kilometers);

    public static Distance Meters(double meters) => new Distance(meters, DistanceUnit.Meters);

    public static Distance Centimeters(double centimeters) => new Distance(centimeters, DistanceUnit.Centimeters);

    public static Distance Millimeters(double millimeter) => new Distance(millimeter, DistanceUnit.Millimeters);

    public static Distance NauticalMiles(double nauticalMiles) => new Distance(nauticalMiles, DistanceUnit.NauticalMiles);

    public static implicit operator Distance(string distanceUnit) => new Distance(distanceUnit);

    public override string ToString() => this.Precision.ToString((IFormatProvider) CultureInfo.InvariantCulture) + this.Unit.GetStringValue();
  }
}
