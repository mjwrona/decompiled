// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.ISshSession
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using System;
using System.Net.Sockets;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public interface ISshSession : IDisposable
  {
    void StartSession(
      IVssRequestContextProvider requestContextProvider,
      Socket socket,
      SshOptions options,
      IAuthenticationProvider authenticationProvider,
      ITunneledCommandExtensionProvider commandExtensionProvider);

    IAuthenticationProvider AuthenticationProvider { get; }

    Microsoft.VisualStudio.Services.Identity.Identity SessionUser { get; }

    event SessionClosedHandler SessionClosed;
  }
}
