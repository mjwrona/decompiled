// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.UpdatePointStateAndTester
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
  public class UpdatePointStateAndTester
  {
    [XmlAttribute]
    public int PointId { get; set; }

    [XmlAttribute]
    public byte State { get; set; }

    [XmlAttribute]
    public Guid AssignedTo { get; set; }

    [XmlAttribute]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    public int Revision { get; set; }

    [XmlAttribute]
    public bool ResetToActive { get; set; }

    [XmlAttribute]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    public int ConfigurationId { get; set; }

    [XmlAttribute]
    public int LastTestRunId { get; set; }

    [XmlAttribute]
    public int LastTestResultId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format("PointId={0} State={1} AssignedTo={2} LastUpdatedBy={3} Revision={4} ResetToActive={5} TestCaseId={6} ConfigurationId={7} LastTestRunId={8} LastTestResultId={9}", (object) 0, (object) 1, (object) 2, (object) 3, (object) 4, (object) 5, (object) 6, (object) 7, (object) 8, (object) 9), (object) this.PointId, (object) this.State, (object) this.AssignedTo, (object) this.LastUpdatedBy, (object) this.Revision, (object) this.ResetToActive, (object) this.TestCaseId, (object) this.ConfigurationId, (object) this.LastTestRunId, (object) this.LastTestResultId);
  }
}
