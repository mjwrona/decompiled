// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestFileUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitPullRequestFileUtility
  {
    public static PushStreamContent GetZipPushStreamContent(
      IVssRequestContext requestContext,
      Guid repoId,
      List<ContentInfo> contentsInfo)
    {
      return (PushStreamContent) GitFileUtility.CreateZipPushStreamContent((Action<ZipArchive, ByteArray>) ((archive, byteArray) =>
      {
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repoId))
        {
          foreach (ContentInfo contentInfo in contentsInfo)
          {
            Sha1Id objectId = new Sha1Id(contentInfo.Sha1Hash);
            if (((GitPullRequestContentFlags) contentInfo.Flags).HasFlag((Enum) GitPullRequestContentFlags.IsSubmodule))
              GitPullRequestFileUtility.CreateSubmoduleZipEntry(archive, objectId);
            else
              GitFileUtility.CreateZipEntry(repositoryById, archive, objectId, byteArray);
          }
        }
      }));
    }

    public static Stream GetSubmoduleFileContentStream(Sha1Id objectId)
    {
      MemoryStream fileContentStream = new MemoryStream();
      byte[] bytes = Encoding.UTF8.GetBytes(objectId.ToString());
      fileContentStream.Write(bytes, 0, bytes.Length);
      fileContentStream.Position = 0L;
      return (Stream) fileContentStream;
    }

    private static void CreateSubmoduleZipEntry(ZipArchive archive, Sha1Id objectId)
    {
      using (Stream stream = archive.CreateEntry(objectId.ToString()).Open())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(objectId.ToString());
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
