// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentVersionNotFoundException
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentVersionNotFoundException : VssServiceException
  {
    public CommentVersionNotFoundException(int commentId, int version)
      : base(string.Format(Resources.CommentVersionNotFoundMessage, (object) commentId, (object) version))
    {
    }
  }
}
