// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteOrTestCaseDeletedEvent
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DataContract]
  [ServiceEventObject]
  public class TestSuiteOrTestCaseDeletedEvent : PlanedTestMetaDataEvent
  {
    public TestSuiteOrTestCaseDeletedEvent(Guid projectId, int planId, List<int> pointIds)
      : base(projectId, planId)
    {
      this.PointIds = pointIds;
    }

    [DataMember(IsRequired = true)]
    public List<int> PointIds { get; private set; }

    public override string ToString()
    {
      string str = string.Join<int>(", ", (IEnumerable<int>) this.PointIds);
      return string.Join(", ", new string[2]
      {
        base.ToString(),
        "PointIds - " + str
      });
    }
  }
}
