// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Events.GitFetchEvent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Events
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.git-fetch-event")]
  public class GitFetchEvent : IGitRepositoryEvent
  {
    [DataMember]
    public Guid RepositoryId { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Guid UserId { get; set; }

    [DataMember]
    public DateTime FetchDate { get; set; }

    [DataMember]
    public HashSet<Sha1Id> Wants { get; set; }

    [DebuggerStepThrough]
    public GitFetchEvent()
    {
    }
  }
}
