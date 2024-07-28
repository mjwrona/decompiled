// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Catalog.Objects.SharePointWebApplication
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("For PowerTools BackCompat only.  Use Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication")]
  public class SharePointWebApplication : Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication
  {
    private Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication m_app;

    public SharePointWebApplication(Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication app)
    {
      this.m_app = app;
      this.CatalogNode = this.m_app.CatalogNode;
      this.Context = this.m_app.Context;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new string DefaultRelativePath
    {
      get => this.m_app.DefaultRelativePath;
      set => this.m_app.DefaultRelativePath = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new ServiceDefinition RootUrlService
    {
      get => this.m_app.RootUrlService;
      set => this.m_app.RootUrlService = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new ServiceDefinition AdminUrlService
    {
      get => this.m_app.AdminUrlService;
      set => this.m_app.AdminUrlService = value;
    }
  }
}
