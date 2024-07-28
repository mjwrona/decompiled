// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.Resource
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Resource.resourceMan == null)
          Resource.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Ssh.Server.External.Eldos.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Resource.resourceCulture;
      set => Resource.resourceCulture = value;
    }

    internal static string MultiplexingNotSupported => Resource.ResourceManager.GetString(nameof (MultiplexingNotSupported), Resource.resourceCulture);

    internal static string SocketClosedAfterReadWriteTimeout => Resource.ResourceManager.GetString(nameof (SocketClosedAfterReadWriteTimeout), Resource.resourceCulture);

    internal static string SshFailedToAuthenticatePublicKey => Resource.ResourceManager.GetString(nameof (SshFailedToAuthenticatePublicKey), Resource.resourceCulture);

    internal static string SSHRSAIsFailed => Resource.ResourceManager.GetString(nameof (SSHRSAIsFailed), Resource.resourceCulture);

    internal static string SSHRSAIsThrottled => Resource.ResourceManager.GetString(nameof (SSHRSAIsThrottled), Resource.resourceCulture);

    internal static string SshShellAccessBlocked => Resource.ResourceManager.GetString(nameof (SshShellAccessBlocked), Resource.resourceCulture);

    internal static string SshUnauthorizedException => Resource.ResourceManager.GetString(nameof (SshUnauthorizedException), Resource.resourceCulture);
  }
}
