// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PlanedTestMetaDataEvent
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DataContract]
  [ServiceEventObject]
  public abstract class PlanedTestMetaDataEvent
  {
    protected PlanedTestMetaDataEvent(Guid projectId, int planId)
    {
      this.ProjectId = projectId;
      this.TestPlanId = planId;
    }

    [DataMember(IsRequired = true)]
    public Guid ProjectId { get; private set; }

    [DataMember(IsRequired = true)]
    public int TestPlanId { get; private set; }

    public override string ToString() => string.Format("ProjectId - {0}. TestPlanId - {1}.", (object) this.ProjectId, (object) this.TestPlanId);
  }
}
