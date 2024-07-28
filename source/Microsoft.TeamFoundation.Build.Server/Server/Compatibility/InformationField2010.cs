// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.InformationField2010
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
  [XmlType("InformationField")]
  public sealed class InformationField2010 : IValidatable
  {
    public InformationField2010()
    {
    }

    internal InformationField2010(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Value { get; set; }

    internal int NodeId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("Name", this.Name, false, (string) null);
      ArgumentValidation.Check("Value", this.Value, true, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[InformationField2010 Name={0} Value={1}]", (object) this.Name, (object) this.Value);
  }
}
