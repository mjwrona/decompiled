// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLooseObjectWriter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Compression;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitLooseObjectWriter
  {
    private readonly ITraceRequest m_tracer;
    private readonly ITfsGitRepository m_repo;
    private const string c_layer = "GitLooseObjectWriter";

    public GitLooseObjectWriter(ITraceRequest tracer, ITfsGitRepository repo)
    {
      this.m_tracer = tracer;
      this.m_repo = repo;
    }

    public void Write(Stream output, Sha1Id objectId, Action onBeforeWrite = null)
    {
      using (this.m_tracer.TraceBlock(1013555, 1013556, GitServerUtils.TraceArea, nameof (GitLooseObjectWriter), nameof (Write)))
      {
        GitObjectType contentType;
        using (Stream content = this.m_repo.GetContent(objectId, out contentType))
        {
          if (onBeforeWrite != null)
            onBeforeWrite();
          using (Stream destination = (Stream) new ZlibStream(output, CompressionLevel.Fastest, true))
          {
            byte[] objectHeader = GitServerUtils.CreateObjectHeader(contentType.GetPackType(), content.Length);
            destination.Write(objectHeader, 0, objectHeader.Length);
            GitStreamUtil.SmartCopyTo(content, destination);
          }
        }
      }
    }
  }
}
