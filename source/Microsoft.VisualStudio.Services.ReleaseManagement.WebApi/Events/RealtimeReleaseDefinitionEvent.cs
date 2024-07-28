// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.RealtimeReleaseDefinitionEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Realtime", Justification = "This is the correct spelling")]
  [KnownType(typeof (ReleaseEnvironmentStatusUpdatedEvent))]
  [DataContract]
  public abstract class RealtimeReleaseDefinitionEvent
  {
    protected RealtimeReleaseDefinitionEvent(Guid projectId, int definitionId)
    {
      this.ProjectId = projectId;
      this.DefinitionId = definitionId;
    }

    [DataMember]
    public Guid ProjectId { get; private set; }

    [DataMember]
    public int DefinitionId { get; private set; }
  }
}
