// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestCaseResultIdAndRev
  {
    public TestCaseResultIdAndRev()
    {
    }

    public TestCaseResultIdAndRev(TestCaseResultIdentifier id, int revision)
    {
      this.Id = id;
      this.Revision = revision;
    }

    public TestCaseResultIdentifier Id { get; set; }

    [XmlAttribute]
    public int Revision { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0} {1})", (object) this.Id, (object) this.Revision);
  }
}
