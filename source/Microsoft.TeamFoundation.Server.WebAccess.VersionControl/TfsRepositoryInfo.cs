// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.TfsRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public class TfsRepositoryInfo : VersionControlRepositoryInfo
  {
    internal TfsRepositoryInfo(IVssRequestContext requestContext)
      : base(VersionControlRepositoryType.TFS, (VersionControlProvider) new TfsVersionControlProvider(requestContext))
    {
    }

    public TfsVersionControlProvider TfsProvider => (TfsVersionControlProvider) this.Provider;
  }
}
