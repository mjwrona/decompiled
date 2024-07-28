// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.IFileAccessorFactory
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public interface IFileAccessorFactory
  {
    IEnumerable<IFileAccessor> GetFileAccessors(
      IVssRequestContext requestContext,
      IEnumerable<IRepositoryDescriptor> repoDescriptors);

    IFileAccessor GetFileAccessor(
      IVssRequestContext requestContext,
      IRepositoryDescriptor repoDescriptor);
  }
}
