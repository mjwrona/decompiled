// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfsFileAccessorBase`1
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public abstract class TfsFileAccessorBase<U> where U : IRepositoryDescriptor
  {
    protected U m_repositoryDescriptor;
    private const string c_layer = "TfsFileAccessorBase";

    public TfsFileAccessorBase(IVssRequestContext requestContext, U repositoryDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckGenericForNull((object) repositoryDescriptor, nameof (repositoryDescriptor));
      ArgumentUtility.CheckForEmptyGuid(repositoryDescriptor.ProjectId, "ProjectId");
      ArgumentUtility.CheckForEmptyGuid(repositoryDescriptor.Id, "Id");
      this.TfsRequestContext = requestContext;
      this.RepositoryDescriptor = (IRepositoryDescriptor) repositoryDescriptor;
      this.Project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, this.RepositoryDescriptor.ProjectId);
      this.InitializeDescriptor();
    }

    protected IVssRequestContext TfsRequestContext { get; private set; }

    protected ProjectInfo Project { get; private set; }

    public IRepositoryDescriptor RepositoryDescriptor
    {
      get => (IRepositoryDescriptor) this.m_repositoryDescriptor;
      private set => this.m_repositoryDescriptor = (U) value;
    }

    protected abstract void InitializeDescriptor();
  }
}
