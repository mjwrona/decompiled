// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsDeltaListProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TfsDeltaListProvider : DeltaListProvider<TfsGitObjectLocation>
  {
    private readonly IGitPackIndex m_index;
    private readonly ITfsGitContentDB<TfsGitObjectLocation> m_contentDB;

    public TfsDeltaListProvider(
      ITfsGitContentDB<TfsGitObjectLocation> contentDb,
      IGitPackIndex packIndex)
    {
      this.m_contentDB = contentDb;
      this.m_index = packIndex;
    }

    public override TfsGitObjectLocation GetLocation(Sha1Id objectId) => this.m_index.LookupObject(objectId).Location;

    public override Stream GetRawContent(TfsGitObjectLocation location) => this.m_contentDB.GetRawContent(location);
  }
}
