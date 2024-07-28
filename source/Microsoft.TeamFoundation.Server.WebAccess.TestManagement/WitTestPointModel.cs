// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.WitTestPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class WitTestPointModel
  {
    [DataMember(Name = "testPointId")]
    public int TestPointId { get; set; }

    [DataMember(Name = "testCaseId")]
    public int TestCaseId { get; set; }

    [DataMember(Name = "testCaseTitle")]
    public string TestCaseTitle { get; set; }

    [DataMember(Name = "outcome")]
    public string Outcome { get; set; }

    [DataMember(Name = "sequenceNumber")]
    public int SequenceNumber { get; set; }
  }
}
