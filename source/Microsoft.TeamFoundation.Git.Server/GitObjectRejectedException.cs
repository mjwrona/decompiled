// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitObjectRejectedException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class GitObjectRejectedException : TeamFoundationServerException
  {
    public GitObjectRejectedException(string message)
      : base(message)
    {
    }

    public GitObjectRejectedException(
      GitObjectType objectType,
      Sha1Id objectId,
      string rejectionReason)
      : this(Resources.Format("ObjectRejected", (object) objectType.ToString().ToLower(), (object) objectId, (object) rejectionReason))
    {
    }
  }
}
