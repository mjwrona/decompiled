// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeSenseService
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class CodeSenseService : ICodeSenseService, IVssFrameworkService
  {
    private const string ServerPathPrefix = "$/";
    private readonly IAggregationStore aggregator;

    public CodeSenseService()
      : this((IAggregationStore) AggregationStore.GetInstance())
    {
    }

    public CodeSenseService(IAggregationStore aggregator)
    {
      ArgumentUtility.CheckForNull<IAggregationStore>(aggregator, nameof (aggregator));
      this.aggregator = aggregator;
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.WithTrace(1024100, TraceLayer.Service, (Action) (() =>
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }), "ServiceStart");

    public VersionedContent GetDetails(
      IVssRequestContext requestContext,
      string path,
      CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3)
    {
      return requestContext.WithTrace<VersionedContent>(1024110, TraceLayer.Service, (Func<VersionedContent>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
        using (IAggregateReader reader = this.aggregator.CreateReader(requestContext))
          return reader.GetDetailsAggregate(path, targetResourceVersion);
      }), nameof (GetDetails));
    }

    public VersionedContent GetSummary(
      IVssRequestContext requestContext,
      string path,
      CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3)
    {
      return requestContext.WithTrace<VersionedContent>(1024130, TraceLayer.Service, (Func<VersionedContent>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
        using (IAggregateReader reader = this.aggregator.CreateReader(requestContext))
          return reader.GetSummaryAggregate(path, targetResourceVersion);
      }), nameof (GetSummary));
    }
  }
}
