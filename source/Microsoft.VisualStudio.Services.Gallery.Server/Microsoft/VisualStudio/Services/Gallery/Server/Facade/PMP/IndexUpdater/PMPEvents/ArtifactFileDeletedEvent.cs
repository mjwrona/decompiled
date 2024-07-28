// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.ArtifactFileDeletedEvent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.PMPEventData;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents
{
  public sealed class ArtifactFileDeletedEvent : PMPEvent<ArtifactFileEventData>
  {
    public override string Type { get; } = "com.visualstudio.marketplace." + "artifactfile.deleted";

    public ArtifactFileDeletedEvent(string source, ArtifactFileEventData eventData)
      : base(source, eventData)
    {
    }
  }
}
