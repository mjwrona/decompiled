// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteRepopulateInfo
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class SuiteRepopulateInfo
  {
    [XmlAttribute]
    public int SuiteId { get; set; }

    [XmlAttribute]
    public bool UpdateEntries { get; set; }

    [XmlAttribute]
    public string LastError { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SuiteId={0} UpdateEntries={1} LastError={2}", (object) this.SuiteId, (object) this.UpdateEntries, (object) this.LastError);
  }
}
