// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers.PipelineReferenceComparer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestReporting.Helpers
{
  public class PipelineReferenceComparer : IEqualityComparer<PipelineReference>
  {
    private PipelineNodeHierarchyLevel compareLevel = PipelineNodeHierarchyLevel.Job;

    public PipelineReferenceComparer()
    {
    }

    public PipelineReferenceComparer(PipelineNodeHierarchyLevel level) => this.compareLevel = level;

    public bool Equals(PipelineReference obj1, PipelineReference obj2) => obj1 == obj2 || obj1 != null && obj2 != null && obj1.PipelineId == obj2.PipelineId && (this.compareLevel == PipelineNodeHierarchyLevel.PipelineInstance || (obj1.StageReference == null || obj2.StageReference != null) && (obj2.StageReference != null || obj2.StageReference == null) && (obj1.StageReference == null || obj2.StageReference == null || string.Compare(obj1.StageReference.StageName, obj2.StageReference.StageName, true) == 0) && (this.compareLevel == PipelineNodeHierarchyLevel.Stage || (obj1.PhaseReference == null || obj2.PhaseReference != null) && (obj1.PhaseReference != null || obj2.PhaseReference == null) && (obj1.PhaseReference == null || obj2.PhaseReference == null || string.Compare(obj1.PhaseReference.PhaseName, obj2.PhaseReference.PhaseName, true) == 0) && (this.compareLevel == PipelineNodeHierarchyLevel.Phase || (obj1.JobReference == null || obj2.JobReference != null) && (obj1.JobReference != null || obj2.JobReference == null) && (obj1.JobReference == null || obj2.JobReference == null || string.Compare(obj1.JobReference.JobName, obj2.JobReference.JobName, true) == 0))));

    public int GetHashCode(PipelineReference obj)
    {
      int hashCode1 = 6543 + obj.PipelineId;
      if (this.compareLevel >= PipelineNodeHierarchyLevel.Stage && obj.StageReference != null && !string.IsNullOrWhiteSpace(obj.StageReference.StageName))
      {
        int hashCode2 = obj.StageReference.StageName.GetHashCode();
        hashCode1 = hashCode1 * -35211 + hashCode2;
      }
      if (this.compareLevel >= PipelineNodeHierarchyLevel.Phase && obj.PhaseReference != null && !string.IsNullOrWhiteSpace(obj.PhaseReference.PhaseName))
      {
        int hashCode3 = obj.PhaseReference.PhaseName.GetHashCode();
        hashCode1 = hashCode1 * -35211 + hashCode3;
      }
      if (this.compareLevel >= PipelineNodeHierarchyLevel.Job && obj.JobReference != null && !string.IsNullOrWhiteSpace(obj.JobReference.JobName))
      {
        int hashCode4 = obj.JobReference.JobName.GetHashCode();
        hashCode1 = hashCode1 * -35211 + hashCode4;
      }
      return hashCode1;
    }
  }
}
