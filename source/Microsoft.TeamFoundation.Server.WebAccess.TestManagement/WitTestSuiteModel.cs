// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.WitTestSuiteModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class WitTestSuiteModel
  {
    [DataMember(Name = "testPlanId")]
    public int TestPlanId { get; set; }

    [DataMember(Name = "testSuiteId")]
    public int TestSuiteId { get; set; }

    [DataMember(Name = "requirementId")]
    public int RequirementId { get; set; }

    [DataMember(Name = "testPoints")]
    public List<WitTestPointModel> TestPoints { get; set; }
  }
}
