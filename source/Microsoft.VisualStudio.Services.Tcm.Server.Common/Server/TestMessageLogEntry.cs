// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestMessageLogEntry
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(0)]
    public int TestMessageLogId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(0)]
    public int EntryId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid LogUser { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LogUserName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte LogLevel { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Message { get; set; }

    public override string ToString() => this.TestMessageLogId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
