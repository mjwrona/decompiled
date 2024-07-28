// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.IPackagingOperations
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  [InheritedExport]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IPackagingOperations
  {
    string Protocol { get; }

    Task DeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionIdentity> packageVersionIdentities);

    Task PermanentlyDeletePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionIdentity> packageVersionIdentities);
  }
}
