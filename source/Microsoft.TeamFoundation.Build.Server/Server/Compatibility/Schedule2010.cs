// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.Schedule2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("Schedule")]
  public sealed class Schedule2010 : IValidatable
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string TimeZoneId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int UtcStartTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ScheduleDays2010 UtcDaysToBuild { get; set; }

    internal string DefinitionUri { get; set; }

    internal Guid ProjectId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("TimeZoneId", (object) this.TimeZoneId, false);
      try
      {
        TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
      }
      catch (InvalidTimeZoneException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      catch (TimeZoneNotFoundException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      ArgumentValidation.CheckBound("UtcStartTime", this.UtcStartTime, 0, 86399);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Schedule2010 DefinitionUri={0} TimeZoneId={1} UtcStartTime={2} UtcDaysToBuild={3}]", (object) this.DefinitionUri, (object) this.TimeZoneId, (object) this.UtcStartTime, (object) this.UtcDaysToBuild);
  }
}
