// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.GitLocalRepository
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  internal class GitLocalRepository : IRemoteRepository
  {
    private Stream m_localStream;
    private SourceContext m_sourceContext;
    private IVssRequestContext m_requestContext;
    private const string c_Layer = "GitLocalRepository";

    public GitLocalRepository(
      IVssRequestContext requestContext,
      SourceContext sourceContext,
      Stream localStream)
    {
      this.m_localStream = localStream;
      this.m_sourceContext = sourceContext;
      this.m_requestContext = requestContext;
    }

    public async Task<Stream> GetRemoteFileStream(
      string path = null,
      string branch = null,
      bool throwErrorIfNotFound = false)
    {
      TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
      await Task.Run((Action) (() => tcs.SetResult(this.m_localStream)));
      return tcs.Task.Result;
    }
  }
}
