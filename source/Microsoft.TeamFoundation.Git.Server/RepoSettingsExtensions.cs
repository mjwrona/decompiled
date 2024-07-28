// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class RepoSettingsExtensions
  {
    public static TreeFsckOptions ToTreeFsckOptions(this GitRepoSettings settings)
    {
      TreeFsckOptions treeFsckOptions = TreeFsckOptions.None;
      if (settings.RejectDotGit)
        treeFsckOptions |= TreeFsckOptions.RejectDotGit;
      if (settings.EnforceConsistentCase)
        treeFsckOptions |= TreeFsckOptions.EnforceConsistentCase;
      return treeFsckOptions;
    }

    public static bool IsGvfsOnly(
      this GitRepoSettings settings,
      IVssRequestContext requestContext,
      GitObjectFilter filter)
    {
      return settings.GvfsOnly && (requestContext.IsFeatureEnabled("Git.GvfsOnlySettingAlsoAppliesToPartialClones") || filter.FilterIsNoop()) && !requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.Proxy.ServiceAccounts, requestContext.GetUserIdentity().Descriptor) && !settings.GvfsExemptUsers.Contains<Guid>(requestContext.GetUserId(true));
    }
  }
}
