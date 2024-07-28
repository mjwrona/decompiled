// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.DefaultFileAcessorFactory
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class DefaultFileAcessorFactory : IFileAccessorFactory
  {
    public IFileAccessor GetFileAccessor(
      IVssRequestContext requestContext,
      IRepositoryDescriptor repoDescriptor)
    {
      IFileAccessor fileAccessor = (IFileAccessor) null;
      switch (repoDescriptor)
      {
        case TfsGitRepositoryDescriptor repositoryDescriptor1:
          if (repositoryDescriptor1 != null)
          {
            fileAccessor = (IFileAccessor) new TfsGitFileAccessor(requestContext, repositoryDescriptor1);
            break;
          }
          break;
        case TfvcRepositoryDescriptor repositoryDescriptor2:
          if (repositoryDescriptor2 != null)
          {
            fileAccessor = (IFileAccessor) new TfvcFileAccessor(requestContext, repositoryDescriptor2);
            break;
          }
          break;
        default:
          throw new NotSupportedException("Does not support creating file accessors of " + repoDescriptor.Type.ToString() + " repositories.");
      }
      return fileAccessor;
    }

    public IEnumerable<IFileAccessor> GetFileAccessors(
      IVssRequestContext requestContext,
      IEnumerable<IRepositoryDescriptor> repoDescriptors)
    {
      foreach (IRepositoryDescriptor repoDescriptor in repoDescriptors)
      {
        IFileAccessor fileAccessor = this.GetFileAccessor(requestContext, repoDescriptor);
        if (fileAccessor != null)
          yield return fileAccessor;
      }
    }
  }
}
