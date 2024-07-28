// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TriggerInstance
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class TriggerInstance
  {
    public TriggerInstance(
      BuildDefinition definition,
      FilteredBuildTrigger trigger,
      long updateId,
      RefUpdateInfo refUpdate,
      IdentityRef sourceOwner,
      string sourceVersion,
      BuildRepository triggerRepository = null,
      RunResourcesParameters resourcesParameters = null)
    {
      this.Definition = definition;
      this.Trigger = trigger;
      this.UpdateId = updateId;
      this.RefUpdate = refUpdate;
      this.SourceOwner = sourceOwner;
      this.SourceVersion = sourceVersion;
      this.ResourcesParameters = resourcesParameters == null ? new RunResourcesParameters() : resourcesParameters;
      this.TriggerRepository = triggerRepository == null ? definition.Repository : triggerRepository;
    }

    public BuildDefinition Definition { get; }

    public BuildRepository TriggerRepository { get; }

    public RefUpdateInfo RefUpdate { get; }

    public IdentityRef SourceOwner { get; }

    public string SourceVersion { get; }

    public FilteredBuildTrigger Trigger { get; }

    public long UpdateId { get; }

    public RunResourcesParameters ResourcesParameters { get; set; }
  }
}
