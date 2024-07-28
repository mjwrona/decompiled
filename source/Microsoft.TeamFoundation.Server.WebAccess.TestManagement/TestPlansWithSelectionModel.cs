// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPlansWithSelectionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestPlansWithSelectionModel
  {
    [DataMember(Name = "testPlans")]
    public List<TestPlanModel> TestPlans { get; set; }

    [DataMember(Name = "selectedTestSuite")]
    public int SelectedTestSuite { get; set; }

    [DataMember(Name = "selectedTestPlan")]
    public int SelectedTestPlan { get; set; }
  }
}
