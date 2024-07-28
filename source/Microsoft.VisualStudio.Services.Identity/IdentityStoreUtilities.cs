// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityStoreUtilities
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityStoreUtilities
  {
    public static bool IdentityAuditEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    public static long IdentityAuditFirstSequenceId(IVssRequestContext requestContext)
    {
      long firstIdentityAuditSequenceId = 0;
      ServiceVersionInfo serviceVersion = requestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "IdentityManagement", "Default");
      if (IdentityStoreUtilities.IdentityAuditEnabled(requestContext) && serviceVersion.Version >= 14)
      {
        string query = string.Format("/Service/Identity/Settings/IdentityAuditFirstSequenceId/{0}", (object) requestContext.ServiceHost.ServiceHostInternal().DatabaseId);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        firstIdentityAuditSequenceId = vssRequestContext.GetService<CachedRegistryService>().GetValue<long>(vssRequestContext, (RegistryQuery) query, 0L);
        if (firstIdentityAuditSequenceId == 0L)
          IdentityStoreUtilities.RetrieveAndRegisterFirstSequenceId(requestContext, ref firstIdentityAuditSequenceId);
      }
      return firstIdentityAuditSequenceId;
    }

    private static void RetrieveAndRegisterFirstSequenceId(
      IVssRequestContext requestContext,
      ref long firstIdentityAuditSequenceId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      firstIdentityAuditSequenceId = 0L;
      if (requestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "Group", "Default").Version < 8)
        return;
      string path = string.Format("/Service/Identity/Settings/IdentityAuditFirstSequenceId/{0}", (object) requestContext.ServiceHost.ServiceHostInternal().DatabaseId);
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        groupComponent.RetrieveFirstAuditSequenceIds(out firstIdentityAuditSequenceId);
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      if (firstIdentityAuditSequenceId <= 0L)
        return;
      service.SetValue<long>(vssRequestContext, path, firstIdentityAuditSequenceId);
    }
  }
}
