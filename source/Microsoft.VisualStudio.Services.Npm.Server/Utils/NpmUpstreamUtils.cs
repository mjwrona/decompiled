// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.NpmUpstreamUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class NpmUpstreamUtils
  {
    public static Uri GetPackageUriFromUpstreamSource(
      NpmPackageName packageName,
      UpstreamSource source)
    {
      ArgumentUtility.CheckForNull<NpmPackageName>(packageName, nameof (packageName));
      return UriUtility.Combine(new UpstreamProvider(source).UpstreamRegistryUri, packageName.UriEscapedFullName, true);
    }
  }
}
