// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TriggerLoadError
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class TriggerLoadError
  {
    public TriggerLoadError(
      BuildDefinition definition,
      long updateId,
      RefUpdateInfo refUpdate,
      IdentityRef sourceOwner,
      DefinitionTriggerType triggerType,
      Exception exception)
      : this(definition, updateId, refUpdate, sourceOwner, triggerType, PipelineValidationError.Create(exception))
    {
    }

    public TriggerLoadError(
      BuildDefinition definition,
      long updateId,
      RefUpdateInfo refUpdate,
      IdentityRef sourceOwner,
      DefinitionTriggerType triggerType,
      IEnumerable<PipelineValidationError> errors)
    {
      this.Definition = definition;
      this.Branch = new BuildDefinitionBranch()
      {
        BranchName = string.IsNullOrEmpty(refUpdate.MergeRef) ? refUpdate.RefName : refUpdate.MergeRef,
        SourceId = updateId,
        PendingSourceOwner = Guid.Parse(sourceOwner.Id),
        PendingSourceVersion = refUpdate.NewObjectId
      };
      this.TriggerType = triggerType;
      this.Errors = new List<PipelineValidationError>(errors);
    }

    public BuildDefinition Definition { get; }

    public BuildDefinitionBranch Branch { get; }

    public DefinitionTriggerType TriggerType { get; }

    public List<PipelineValidationError> Errors { get; }
  }
}
