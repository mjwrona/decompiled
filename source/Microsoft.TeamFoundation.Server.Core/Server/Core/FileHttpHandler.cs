// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.FileHttpHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class FileHttpHandler : TeamFoundationHttpHandler
  {
    protected FileHttpHandler()
    {
      this.RequestContext.ServiceName = "Framework";
      this.FileService = this.RequestContext.GetService<TeamFoundationFileRepositoryService>();
    }

    protected FileHttpHandler(HttpContextBase httpContext)
      : base(httpContext)
    {
      this.RequestContext.ServiceName = "Framework";
      this.FileService = this.RequestContext.GetService<TeamFoundationFileRepositoryService>();
    }

    internal TeamFoundationFileRepositoryService FileService { get; set; }

    protected abstract void Execute();

    protected override sealed void ProcessRequestImpl(HttpContext context) => this.Execute();
  }
}
