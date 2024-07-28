// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.JobManagement.NpmMetadataDiffCommitPduNotificationDetector
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.JobManagement
{
  public class NpmMetadataDiffCommitPduNotificationDetector : IPduNotificationDetector
  {
    public IEnumerable<PendingPduNotification> GetNotificationsForCommits(
      IEnumerable<ICommitLogEntry> flattenedCommits)
    {
      foreach (CastedOpCommit<NpmMetadataDiffOperationData> commit in flattenedCommits.FilterToOpType<NpmMetadataDiffOperationData>())
      {
        IEnumerable<\u003C\u003Ef__AnonymousType0<string, string, string>> datas = commit.CommitOperationData.NewVersionMetadata.Join((IEnumerable<KeyValuePair<string, VersionMetadata>>) commit.CommitOperationData.OldVersionMetadata, (Func<KeyValuePair<string, VersionMetadata>, string>) (newVer => newVer.Key), (Func<KeyValuePair<string, VersionMetadata>, string>) (oldVer => oldVer.Key), (newVer, oldVer) => new
        {
          Version = newVer.Key,
          OldDeprecateMessage = oldVer.Value.Deprecated,
          NewDeprecateMessage = newVer.Value.Deprecated
        }, (IEqualityComparer<string>) StringComparer.Ordinal).Where(x => !StringComparer.Ordinal.Equals(x.NewDeprecateMessage, x.OldDeprecateMessage));
        NpmIdentityResolver identityResolver = NpmIdentityResolver.Instance;
        foreach (var data in datas)
          yield return new PendingPduNotification(commit.Commit, TriggerCommitType.ListingStateChange, identityResolver.FusePackageIdentity(commit.CommitOperationData.PackageName, (IPackageVersion) identityResolver.ResolvePackageVersion(data.Version)), (CommitAffectsViews) CommitAffectsViews.All.Instance);
        identityResolver = (NpmIdentityResolver) null;
      }
    }
  }
}
