// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlCommand
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal abstract class VersionControlCommand : Command
  {
    protected VersionControlRequestContext m_versionControlRequestContext;

    public VersionControlCommand(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext.RequestContext)
    {
      this.m_versionControlRequestContext = versionControlRequestContext;
    }

    protected SecurityManager SecurityWrapper => this.m_versionControlRequestContext.VersionControlService.SecurityWrapper;

    protected IVssSecurityNamespace RepositorySecurity => this.m_versionControlRequestContext.GetRepositorySecurity();

    protected IVssSecurityNamespace RepositorySecurity2 => this.m_versionControlRequestContext.GetRepositorySecurity2();
  }
}
