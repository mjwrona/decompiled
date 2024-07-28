// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.BasePackagingOperation
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class BasePackagingOperation : IPackagingOperations
  {
    public abstract string Protocol { get; }

    public virtual Task DeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionIdentity> packageVersionIdentities)
    {
      throw this.NotSupported(nameof (DeletePackageVersions));
    }

    public virtual Task PermanentlyDeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionIdentity> packageVersionIdentities)
    {
      throw this.NotSupported(nameof (PermanentlyDeletePackageVersions));
    }

    protected Exception NotSupported([CallerMemberName] string operationName = "") => (Exception) new NotSupportedException(Resources.Error_NotSupportedProtocolOperation((object) operationName, (object) this.Protocol));

    protected int GetMaxBatchSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Packaging/MaxPackagesBatchRequestSize", true, 100);

    protected int GetMaxConcurrentWritesWithinFeedProtocol(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Feed/MaxConcurrentPackagingWritesWithinFeedProtocol", true, 3);
  }
}
