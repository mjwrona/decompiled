// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PushNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PushNotification : PushNotificationDetails
  {
    [JsonConstructor]
    internal PushNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      IdentityDescriptor pusher,
      string authenticatedUserName,
      IEnumerable<TfsGitRefUpdateResult> refUpdateResults,
      IEnumerable<Sha1Id> includedCommits,
      DateTime pushTime,
      int pushId,
      string pusherIpAddress)
      : base(teamProjectUri, repositoryId, repositoryName, refUpdateResults, includedCommits, pushId)
    {
      this.Pusher = pusher;
      this.AuthenticatedUserName = authenticatedUserName;
      this.PushTime = pushTime;
      this.PusherIpAddress = pusherIpAddress;
    }

    public IdentityDescriptor Pusher { get; }

    public string AuthenticatedUserName { get; }

    public DateTime PushTime { get; }

    public string PusherIpAddress { get; }
  }
}
