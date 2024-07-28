// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectsLockedEvent
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.Types
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-tfs.projects-locked-event")]
  public class ProjectsLockedEvent
  {
    [DataMember]
    public string CollectionUrl { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public string ProjectUrl { get; set; }
  }
}
