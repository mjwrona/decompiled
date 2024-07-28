// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentServiceBase
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public abstract class CommentServiceBase : IVssFrameworkService
  {
    protected readonly IVssDateTimeProvider dateTimeProvider;
    protected IDisposableReadOnlyList<ICommentProvider> m_providers;
    private readonly ServiceFactory<IDisposableReadOnlyList<ICommentProvider>> m_providerFactory;

    public CommentServiceBase() => this.m_providerFactory = (ServiceFactory<IDisposableReadOnlyList<ICommentProvider>>) (context => context.GetExtensions<ICommentProvider>());

    internal CommentServiceBase(IVssDateTimeProvider dateTimeProvider)
      : this()
    {
      this.dateTimeProvider = dateTimeProvider ?? VssDateTimeProvider.DefaultProvider;
    }

    internal CommentServiceBase(
      IVssDateTimeProvider dateTimeProvider,
      IDisposableReadOnlyList<ICommentProvider> providers)
      : this(dateTimeProvider)
    {
      this.m_providers = providers;
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      this.m_providers = this.m_providerFactory(systemRequestContext);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_providers?.Dispose();

    protected ICommentProvider GetProvider(
      IVssRequestContext requestContext,
      Guid artifactKind,
      Guid? projectId = null,
      CommentFormat? format = null)
    {
      return this.m_providers.OrderByDescending<ICommentProvider, CommentFormat>((Func<ICommentProvider, CommentFormat>) (provider => provider.GetCommentFormat(requestContext, projectId ?? Guid.Empty))).FirstOrDefault<ICommentProvider>((Func<ICommentProvider, bool>) (p =>
      {
        if (!(p.ArtifactKind == artifactKind))
          return false;
        if (!format.HasValue)
          return true;
        int commentFormat = (int) p.GetCommentFormat(requestContext, projectId ?? Guid.Empty);
        CommentFormat? nullable = format;
        int valueOrDefault = (int) nullable.GetValueOrDefault();
        return commentFormat == valueOrDefault & nullable.HasValue;
      })) ?? throw new CommentProviderNotRegisteredException(artifactKind).Expected(requestContext.ServiceName);
    }
  }
}
