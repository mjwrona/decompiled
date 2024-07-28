// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DefinitionEnvironmentStep
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DefinitionEnvironmentStep : ForeignKeyModelBase
  {
    public int Id { get; set; }

    public int Rank { get; set; }

    public bool IsAutomated { get; set; }

    public bool IsNotificationOn { get; set; }

    public Guid ApproverId { get; set; }

    public EnvironmentStepType StepType { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public DefinitionEnvironmentStep Clone() => new DefinitionEnvironmentStep()
    {
      Id = this.Id,
      Rank = this.Rank,
      IsAutomated = this.IsAutomated,
      IsNotificationOn = this.IsNotificationOn,
      ApproverId = this.ApproverId,
      StepType = this.StepType,
      DefinitionEnvironmentId = this.DefinitionEnvironmentId
    };
  }
}
