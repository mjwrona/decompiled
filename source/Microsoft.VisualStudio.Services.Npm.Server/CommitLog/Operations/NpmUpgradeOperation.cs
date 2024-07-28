// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations.NpmUpgradeOperation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations
{
  public class NpmUpgradeOperation : ProtocolOperation
  {
    public const string PackageNamesKey = "PackageNames";
    public const string ViewIdsKey = "ViewIds";
    private static readonly Lazy<NpmUpgradeOperation> LazyInstance = new Lazy<NpmUpgradeOperation>((Func<NpmUpgradeOperation>) (() => new NpmUpgradeOperation()));

    private NpmUpgradeOperation()
      : base("Npm", "SaveCached", "1.0")
    {
    }

    public static NpmUpgradeOperation Instance { get; } = NpmUpgradeOperation.LazyInstance.Value;

    public NpmUpgradeOperationData GetData(IItemData storedItemData) => new NpmUpgradeOperationData((IList<NpmPackageName>) JsonUtilities.Deserialize<IList<string>>(storedItemData["PackageNames"]).Select<string, NpmPackageName>((Func<string, NpmPackageName>) (name => new NpmPackageName(name))).ToList<NpmPackageName>(), (IEnumerable<Guid>) JsonUtilities.Deserialize<IList<Guid>>(storedItemData["ViewIds"]));
  }
}
