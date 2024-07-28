// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeIndexingDelayAnalyzerJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class CodeIndexingDelayAnalyzerJob : AbstractIndexingDelayAnalyzerJob
  {
    private ICodeIndexingStalenessAnalyzerFactory m_codeIndexingStalenessAnalyzerFactory;

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    protected override int TracePoint => 1083139;

    public CodeIndexingDelayAnalyzerJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (ICodeIndexingStalenessAnalyzerFactory) CodeIndexingStalenessAnalyzerFactory.GetInstance())
    {
    }

    public CodeIndexingDelayAnalyzerJob(
      IDataAccessFactory dataAccessFactory,
      ICodeIndexingStalenessAnalyzerFactory codeIndexingStalenessAnalyzerFactory)
      : base(dataAccessFactory)
    {
      this.m_codeIndexingStalenessAnalyzerFactory = codeIndexingStalenessAnalyzerFactory;
    }

    internal override string AnalyzeIndexingDelay(ExecutionContext executionContext) => this.m_codeIndexingStalenessAnalyzerFactory.GetCodeIndexingStalenessAnalyzer(executionContext, new TraceMetaData(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer), this.DataAccessFactory, "Git_Repository").AnalyzeCodeIndexingStaleness(executionContext) + this.m_codeIndexingStalenessAnalyzerFactory.GetCodeIndexingStalenessAnalyzer(executionContext, new TraceMetaData(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer), this.DataAccessFactory, "TFVC_Repository").AnalyzeCodeIndexingStaleness(executionContext);

    internal override bool IsIndexingEnabled(IVssRequestContext requestContext) => requestContext.IsSearchConfigured() && requestContext.IsCodeIndexingEnabled();
  }
}
