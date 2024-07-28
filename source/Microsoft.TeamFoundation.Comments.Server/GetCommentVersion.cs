// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.GetCommentVersion
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class GetCommentVersion
  {
    public GetCommentVersion(string artifactId, int commentId, int version)
    {
      this.ArtifactId = artifactId;
      this.CommentId = commentId;
      this.Version = version;
    }

    public string ArtifactId { get; }

    public int CommentId { get; }

    public int Version { get; }
  }
}
