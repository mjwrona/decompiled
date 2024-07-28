// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestSuiteEntry
  {
    public TestSuiteEntry() => this.EntryType = (byte) 1;

    internal int ParentSuiteId { get; set; }

    internal int Order { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int EntryId { get; set; }

    [DefaultValue(1)]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte EntryType { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Private)]
    public TestPointAssignment[] PointAssignments { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) (TestSuiteEntryType) this.EntryType, (object) this.EntryId);

    internal static TestSuiteEntry FromTestCaseId(int testCaseId) => new TestSuiteEntry()
    {
      EntryId = testCaseId,
      EntryType = 1
    };

    internal bool IsTestCaseEntry => this.EntryType == (byte) 1;

    internal void Validate()
    {
      if (this.EntryType <= (byte) 0 || this.EntryType > (byte) 3 || this.EntryId <= 0)
        throw new TestManagementValidationException(ServerResources.InvalidTestSuiteEntry);
    }
  }
}
