// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiUpdatedNotificationMessage
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [ServiceEventObject]
  [DataContract]
  public class WikiUpdatedNotificationMessage
  {
    public WikiUpdatedNotificationMessage(Guid collectionId, Guid projectId, Guid repositoryId)
    {
      this.CollectionId = collectionId;
      this.ProjectId = projectId;
      this.RepositoryId = repositoryId;
    }

    [DataMember]
    public Guid CollectionId { get; private set; }

    [DataMember]
    public Guid ProjectId { get; private set; }

    [DataMember]
    public Guid RepositoryId { get; private set; }
  }
}
