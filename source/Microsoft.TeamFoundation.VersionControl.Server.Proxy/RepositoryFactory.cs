// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.RepositoryFactory
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal class RepositoryFactory
  {
    internal static Repository CreateRepository(
      TfsTeamProjectCollection teamProjectCollection,
      string cacheRoot)
    {
      return teamProjectCollection.GetService<ILocationService>().FindServiceDefinition("FileDownloadService", ProxyConstants.GenericDownloadServiceIdentifier) != null ? new Repository((IRepository) new GenericProxyRepository((TfsConnection) teamProjectCollection), cacheRoot) : new Repository((IRepository) new Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client.Repository(teamProjectCollection), cacheRoot);
    }
  }
}
