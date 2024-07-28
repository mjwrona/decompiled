// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitApplyResolutionException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  internal class GitApplyResolutionException : TeamFoundationServiceException
  {
    internal GitApplyResolutionException(GitResolutionError resolutionError)
      : base(GitApplyResolutionException.GetApplyResolutionExceptionMessage(resolutionError))
    {
      this.ResolutionError = resolutionError;
    }

    internal GitResolutionError ResolutionError { get; }

    private static string GetApplyResolutionExceptionMessage(GitResolutionError resolutionError)
    {
      switch (resolutionError)
      {
        case GitResolutionError.MergeContentNotFound:
          return Resources.Get("GitApplyResolutionException_MergeContentNotFound");
        case GitResolutionError.PathInUse:
          return Resources.Get("GitApplyResolutionException_PathInUse");
        case GitResolutionError.InvalidPath:
          return Resources.Get("GitApplyResolutionException_InvalidPath");
        case GitResolutionError.UnknownAction:
          return Resources.Get("GitApplyResolutionException_UnknownAction");
        case GitResolutionError.UnknownMergeType:
          return Resources.Get("GitApplyResolutionException_UnknownMergeType");
        default:
          return Resources.Get("GitApplyResolutionException_UnknownError");
      }
    }
  }
}
