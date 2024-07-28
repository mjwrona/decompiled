// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildFolderService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildFolderService))]
  public interface IBuildFolderService : IVssFrameworkService
  {
    Folder AddFolder(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      Folder folder);

    IList<Folder> GetFolders(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      FolderQueryOrder queryOrder);

    Folder UpdateFolder(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      Folder folder);

    void DeleteFolder(IVssRequestContext requestContext, Guid projectId, string path);
  }
}
