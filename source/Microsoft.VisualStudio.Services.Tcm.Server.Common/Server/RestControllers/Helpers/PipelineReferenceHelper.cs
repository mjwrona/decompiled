// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers.PipelineReferenceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers
{
  public static class PipelineReferenceHelper
  {
    public static void ValidateAndHandleDefaultValuesForPipelineRefInQuery(
      PipelineReference pipelineReference)
    {
      ArgumentUtility.CheckForNull<PipelineReference>(pipelineReference, nameof (pipelineReference));
      ArgumentUtility.CheckGreaterThanZero((float) pipelineReference.PipelineId, "PipelineId", "Test Results");
      if (!string.IsNullOrEmpty(pipelineReference.StageReference?.StageName))
      {
        string stageName = pipelineReference.StageReference.StageName;
        Validator.CheckAndTrimString(ref stageName, "StageName", 256);
        pipelineReference.StageReference.StageName = stageName;
      }
      else
        pipelineReference.StageReference = (StageReference) null;
      if (!string.IsNullOrEmpty(pipelineReference.PhaseReference?.PhaseName))
      {
        string phaseName = pipelineReference.PhaseReference.PhaseName;
        Validator.CheckAndTrimString(ref phaseName, "PhaseName", 256);
        pipelineReference.PhaseReference.PhaseName = phaseName;
      }
      else
        pipelineReference.PhaseReference = (PhaseReference) null;
      if (!string.IsNullOrEmpty(pipelineReference.JobReference?.JobName))
      {
        string jobName = pipelineReference.JobReference.JobName;
        Validator.CheckAndTrimString(ref jobName, "JobName", 256);
        pipelineReference.JobReference.JobName = jobName;
      }
      else
        pipelineReference.JobReference = (JobReference) null;
      if (!string.IsNullOrEmpty(pipelineReference.JobReference?.JobName))
      {
        if (pipelineReference.PhaseReference == null)
          pipelineReference.PhaseReference = new PhaseReference();
        if (string.IsNullOrEmpty(pipelineReference.PhaseReference.PhaseName))
          pipelineReference.PhaseReference.PhaseName = "__default";
      }
      if (string.IsNullOrEmpty(pipelineReference.PhaseReference?.PhaseName))
        return;
      if (pipelineReference.StageReference == null)
        pipelineReference.StageReference = new StageReference();
      if (!string.IsNullOrEmpty(pipelineReference.StageReference.StageName))
        return;
      pipelineReference.StageReference.StageName = "__default";
    }

    public static void ValidateAndUpdatePipelineReferenceWhileCreatingRun(
      PipelineReference pipelineReference)
    {
      if (pipelineReference == null)
        return;
      if (pipelineReference.PipelineId < 0)
        throw new InvalidPropertyException("RunCreateModel.PipelineReference.PipelineId", string.Format(ServerResources.PipelineIdDoNotExist, (object) pipelineReference.PipelineId));
      if (pipelineReference.PipelineId == 0)
      {
        if (pipelineReference.StageReference != null && !string.IsNullOrWhiteSpace(pipelineReference.StageReference.StageName) || pipelineReference.PhaseReference != null && !string.IsNullOrWhiteSpace(pipelineReference.PhaseReference.PhaseName) || pipelineReference.JobReference != null && !string.IsNullOrWhiteSpace(pipelineReference.JobReference.JobName))
          throw new InvalidPropertyException("RunCreateModel.PipelineReference.PipelineId", string.Format(ServerResources.PipelineIdDoNotExist, (object) pipelineReference.PipelineId));
        pipelineReference = (PipelineReference) null;
      }
      else
      {
        if (pipelineReference.StageReference == null || string.IsNullOrWhiteSpace(pipelineReference.StageReference.StageName))
        {
          pipelineReference.StageReference = new StageReference()
          {
            StageName = "__default",
            Attempt = 1
          };
        }
        else
        {
          string stageName = pipelineReference.StageReference.StageName;
          Validator.CheckAndTrimString(ref stageName, "RunCreateModel.PipelineReference.StageReference.StageName", 256);
          pipelineReference.StageReference.StageName = stageName;
        }
        if (pipelineReference.StageReference.Attempt < 0)
          throw new InvalidPropertyException("RunCreateModel.PipelineReference.StageReference.Attempt", ServerResources.NegativeAttemptNumber);
        if (pipelineReference.PhaseReference == null || string.IsNullOrWhiteSpace(pipelineReference.PhaseReference.PhaseName))
        {
          pipelineReference.PhaseReference = new PhaseReference()
          {
            PhaseName = "__default",
            Attempt = 1
          };
        }
        else
        {
          string phaseName = pipelineReference.PhaseReference.PhaseName;
          Validator.CheckAndTrimString(ref phaseName, "RunCreateModel.PipelineReference.PhaseReference.PhaseName", 256);
          pipelineReference.PhaseReference.PhaseName = phaseName;
        }
        if (pipelineReference.PhaseReference.Attempt < 0)
          throw new InvalidPropertyException("RunCreateModel.PipelineReference.PhaseReference.Attempt", ServerResources.NegativeAttemptNumber);
        if (pipelineReference.JobReference == null || string.IsNullOrWhiteSpace(pipelineReference.JobReference.JobName))
        {
          pipelineReference.JobReference = new JobReference()
          {
            JobName = "__default",
            Attempt = 1
          };
        }
        else
        {
          string jobName = pipelineReference.JobReference.JobName;
          Validator.CheckAndTrimString(ref jobName, "RunCreateModel.PipelineReference.JobReference.JobName", 256);
          pipelineReference.JobReference.JobName = jobName;
        }
        if (pipelineReference.JobReference.Attempt < 0)
          throw new InvalidPropertyException("RunCreateModel.PipelineReference.JobReference.Attempt", ServerResources.NegativeAttemptNumber);
      }
    }
  }
}
