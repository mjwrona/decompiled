// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters.GitCommitRefConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters
{
  public static class GitCommitRefConverter
  {
    public static IList<Change> ToReleaseChanges(IList<GitCommitRef> gitCommitRefs) => (IList<Change>) gitCommitRefs.Select<GitCommitRef, Change>(GitCommitRefConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange ?? (GitCommitRefConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange = new Func<GitCommitRef, Change>(GitCommitRefConverter.ToReleaseChange))).ToList<Change>();

    private static Change ToReleaseChange(this GitCommitRef gitCommitRef) => new Change()
    {
      Id = gitCommitRef.CommitId,
      Message = gitCommitRef.Comment,
      ChangeType = "TfsGit",
      Author = new IdentityRef()
      {
        DisplayName = gitCommitRef.Author.Name,
        UniqueName = gitCommitRef.Author.Email
      },
      Timestamp = new DateTime?(gitCommitRef.Author.Date),
      Location = new Uri(gitCommitRef.RemoteUrl),
      DisplayUri = new Uri(gitCommitRef.Url),
      Pusher = (string) null,
      PushedBy = (IdentityRef) null
    };
  }
}
