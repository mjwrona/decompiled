// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRefUpdateResult
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitRefUpdateResult
  {
    [JsonConstructor]
    internal TfsGitRefUpdateResult(
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId,
      GitRefUpdateStatus status,
      string conflictingName = null,
      Guid? isLockedById = null)
    {
      this.Name = name;
      this.OldObjectId = oldObjectId;
      this.NewObjectId = newObjectId;
      this.Status = status;
      this.ConflictingName = conflictingName;
      this.IsLockedById = isLockedById;
    }

    internal static TfsGitRefUpdateResult FromRefUpdate(
      TfsGitRefUpdateRequest refUpdate,
      GitRefUpdateStatus status,
      string rejectedBy = null,
      string customMessage = null)
    {
      return new TfsGitRefUpdateResult(refUpdate.Name, refUpdate.OldObjectId, refUpdate.NewObjectId, status)
      {
        RejectedBy = rejectedBy,
        CustomMessage = customMessage
      };
    }

    internal static TfsGitRefUpdateResult UpdateResult(
      TfsGitRefUpdateResult result,
      GitRefUpdateStatus status,
      string rejectedBy = null,
      string customMessage = null)
    {
      return new TfsGitRefUpdateResult(result.Name, result.OldObjectId, result.NewObjectId, status)
      {
        RejectedBy = rejectedBy,
        CustomMessage = customMessage
      };
    }

    public string Name { get; }

    public Sha1Id OldObjectId { get; }

    public Sha1Id NewObjectId { get; }

    public GitRefUpdateStatus Status { get; }

    public string ConflictingName { get; }

    public bool Succeeded => this.Status.IsSuccessful();

    public string RejectedBy { get; internal set; }

    public string CustomMessage { get; internal set; }

    public Guid? IsLockedById { get; }
  }
}
