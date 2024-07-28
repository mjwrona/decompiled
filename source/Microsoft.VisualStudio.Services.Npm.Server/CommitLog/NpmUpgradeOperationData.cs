// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmUpgradeOperationData
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmUpgradeOperationData : ICommitOperationData
  {
    public NpmUpgradeOperationData(IList<NpmPackageName> packageNames, IEnumerable<Guid> viewIds)
    {
      this.PackageNames = packageNames;
      this.ViewIds = viewIds ?? Enumerable.Empty<Guid>();
    }

    public IList<NpmPackageName> PackageNames { get; private set; }

    public IEnumerable<Guid> ViewIds { get; private set; }

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand { get; set; } = FeedPermissionConstants.AddPackage;

    public Dictionary<string, string> ToDictionary() => new Dictionary<string, string>()
    {
      {
        "protocolOperation",
        NpmUpgradeOperation.Instance.ToString()
      },
      {
        "PackageNames",
        this.PackageNames.Select<NpmPackageName, string>((Func<NpmPackageName, string>) (p => p.FullName)).Serialize<IEnumerable<string>>()
      },
      {
        "ViewIds",
        this.ViewIds.Serialize<IEnumerable<Guid>>()
      }
    };
  }
}
