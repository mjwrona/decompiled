// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.RealtimeReleaseEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Realtime", Justification = "This is the correct spelling")]
  [KnownType(typeof (ReleaseTaskLogUpdatedEvent))]
  [KnownType(typeof (ReleaseTasksUpdatedEvent))]
  [KnownType(typeof (ReleaseUpdatedEvent))]
  [DataContract]
  public abstract class RealtimeReleaseEvent
  {
    protected RealtimeReleaseEvent(Guid projectId, int releaseId)
    {
      this.ProjectId = projectId;
      this.ReleaseId = releaseId;
      this.EnvironmentId = 0;
    }

    protected RealtimeReleaseEvent(Guid projectId, int releaseId, int environmentId)
    {
      this.ProjectId = projectId;
      this.ReleaseId = releaseId;
      this.EnvironmentId = environmentId;
    }

    [DataMember]
    public Guid ProjectId { get; private set; }

    [DataMember]
    public int ReleaseId { get; private set; }

    [DataMember]
    public int EnvironmentId { get; private set; }
  }
}
