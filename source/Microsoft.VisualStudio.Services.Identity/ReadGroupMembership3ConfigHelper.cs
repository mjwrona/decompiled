// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadGroupMembership3ConfigHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ReadGroupMembership3ConfigHelper
  {
    public const string ReadGroupMembership3Config = "GroupManagement.UseReadGroupMembership3";
    private static readonly IConfigPrototype<bool> configPrototype = ConfigPrototype.Create<bool>("GroupManagement.UseReadGroupMembership3", false);
    private readonly IConfigQueryable<bool> config;

    public ReadGroupMembership3ConfigHelper()
      : this(ConfigProxy.Create<bool>(ReadGroupMembership3ConfigHelper.configPrototype))
    {
    }

    public ReadGroupMembership3ConfigHelper(IConfigQueryable<bool> config) => this.config = config;

    internal bool CanUseReadGroupMembership3(IVssRequestContext context)
    {
      bool flag = this.config.QueryByCtx<bool>(context);
      return ((context.ExecutionEnvironment.IsOnPremisesDeployment ? 0 : (!context.ServiceHost.Is(TeamFoundationHostType.Deployment) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
    }
  }
}
