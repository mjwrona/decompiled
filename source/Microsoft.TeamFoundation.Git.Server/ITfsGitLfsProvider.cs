// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitLfsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface ITfsGitLfsProvider
  {
    Stream GetLfsObject(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId);

    GitLfsObjectRef GetLfsObjectReference(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId);

    IEnumerable<GitLfsObjectRef> GetLfsObjectReferences(
      IVssRequestContext rc,
      OdbId odbId,
      IEnumerable<Sha256Id> lfsObjectIds);

    bool LfsObjectExists(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId);

    TfsGitLfsObjectCreateResult WriteLfsObject(
      IVssRequestContext rc,
      OdbId odbId,
      Sha256Id expectedLfsObjectId,
      Stream data);
  }
}
