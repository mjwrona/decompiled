// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Pipeline.CorePipeline`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Pipeline
{
  public abstract class CorePipeline<TId, TDoc>
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    private IReadOnlyList<CorePipelineStage<TId, TDoc>> m_pipelineStages;

    public string Name { get; }

    protected TraceMetaData TraceMetaData { get; }

    protected CorePipeline(
      TraceMetaData traceMetaData,
      string name,
      CorePipelineContext<TId, TDoc> corePipelineContext)
    {
      this.Name = name;
      this.TraceMetaData = traceMetaData;
      this.IndexingUnit = corePipelineContext.IndexingUnit;
      this.PipelineContext = corePipelineContext;
    }

    internal abstract IReadOnlyList<CorePipelineStage<TId, TDoc>> RegisterStages();

    protected virtual void Prepare()
    {
    }

    protected internal virtual void PrePreRun()
    {
      this.PipelineContext.MarkStartOfPipelineExecution();
      if (!this.IsPrimaryRun())
        return;
      this.Prepare();
    }

    protected internal virtual void PreRun()
    {
    }

    protected internal virtual OperationStatus PostRun(OperationStatus opStatus) => opStatus;

    internal virtual void HandlePipelineError(Exception ex)
    {
    }

    protected internal virtual OperationStatus PostPostRun(OperationStatus opStatus) => opStatus;

    internal virtual bool ShouldRestartPipeline() => this.PipelineContext.RemainingExecutionTime > TimeSpan.Zero;

    internal virtual bool IsPrimaryRun() => true;

    public CorePipelineResult Run()
    {
      Tracer.TraceEnter(1082633, "Indexing Pipeline", "Framework", nameof (Run));
      OperationStatus opStatus = OperationStatus.Succeeded;
      try
      {
        if (this.IndexingUnit.Properties.IsDisabled)
        {
          this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Pipeline execution is disabled for Indexing Unit {0}.", (object) this.IndexingUnit)));
          return this.BuildPipelineResult(opStatus);
        }
        this.m_pipelineStages = this.RegisterStages();
        this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("TfsEntityId:{0}|IUType:{1}|ContractType:{2}", (object) this.IndexingUnit.TFSEntityId, (object) this.IndexingUnit.IndexingUnitType, (object) (this.PipelineContext.IndexingExecutionContext is IndexingExecutionContext executionContext ? executionContext.ProvisioningContext?.ContractType : this.PipelineContext.IndexingExecutionContext.CollectionIndexingUnit?.GetIndexInfo()?.DocumentContractType))));
        try
        {
          this.PrePreRun();
          do
          {
            this.PreRun();
            this.PipelineContext.PropertyBag.Clear();
            this.PipelineContext.PipelineDocs.Clear();
            int num1 = this.PipelineContext.PipelineDocs.Count;
            foreach (CorePipelineStage<TId, TDoc> pipelineStage in (IEnumerable<CorePipelineStage<TId, TDoc>>) this.m_pipelineStages)
            {
              pipelineStage.ResetState();
              this.PipelineContext.CurrentStage = pipelineStage;
              int count1 = this.PipelineContext.PipelineDocs.Count;
              Stopwatch stopwatch = Stopwatch.StartNew();
              pipelineStage.Run(this.PipelineContext);
              stopwatch.Stop();
              int count2 = this.PipelineContext.PipelineDocs.Count;
              if (count2 < count1)
                throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Pipeline document count after execution of stage [{0}] is less than it was before executing the stage: [{1} < {2}].", (object) pipelineStage.Name, (object) count2, (object) count1)));
              int num2 = 0;
              foreach (TDoc pipelineDoc in this.PipelineContext.PipelineDocs)
              {
                if (!pipelineDoc.ShouldProcess)
                  Tracer.TraceAlways(1082702, TraceLevel.Info, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Document [{0}] was ignored by stage [{1}]. Audit Trail: [{2}]", (object) pipelineDoc, (object) pipelineStage.Name, (object) pipelineDoc.AuditTrail)));
                else
                  ++num2;
              }
              this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Pipeline:{0}|Stage:{1}|Flow:{2}=>{3}|Time:{4}ms", (object) this.Name, (object) pipelineStage.Name, (object) num1, (object) num2, (object) (int) stopwatch.Elapsed.TotalMilliseconds)));
              IReadOnlyCollection<TDoc> inUnexpectedState = this.GetDocumentsInUnexpectedState();
              if (inUnexpectedState.Count > 0)
              {
                this.HandleDocumentFlowErrors(inUnexpectedState);
                Tracer.TraceError(1082703, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, string.Join(Environment.NewLine, inUnexpectedState.Take<TDoc>(100).Select<TDoc, string>((Func<TDoc, string>) (d => FormattableString.Invariant(FormattableStringFactory.Create("Document [{0}] is in state [{1}]", (object) d, (object) d.CurrentState))))));
                string str = FormattableString.Invariant(FormattableStringFactory.Create("[{0}] documents are in unexpected states after the execution of stage [{1}]. ", (object) inUnexpectedState.Count, (object) pipelineStage.Name));
                if (this.PipelineContext.IndexingExecutionContext.RequestContext.IsDLITStrictValidationEnabled())
                  throw new SearchServiceException(str + FormattableString.Invariant(FormattableStringFactory.Create("Check ProductTrace messages of TracePoint {0} for more details.", (object) 1082703)));
              }
              if (num1 > num2)
                this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("[{0}] document(s) were ignored by stage [{1}]", (object) (num1 - num2), (object) pipelineStage.Name)));
              num1 = num2;
            }
            opStatus = this.PostRun(opStatus);
            ++this.PipelineExecutionsCompleted;
          }
          while (this.ShouldRestartPipeline());
          this.PipelineContext.CurrentStage = (CorePipelineStage<TId, TDoc>) null;
          opStatus = this.PostPostRun(opStatus);
        }
        catch (Exception ex) when (this.HandleException(ex))
        {
        }
      }
      finally
      {
        Tracer.TraceLeave(1082634, "Indexing Pipeline", "Framework", nameof (Run));
      }
      return this.BuildPipelineResult(opStatus);
    }

    private IReadOnlyCollection<TDoc> GetDocumentsInUnexpectedState()
    {
      IReadOnlyCollection<PipelineDocumentState> expectedDocumentStates = this.PipelineContext.CurrentStage.ExpectedDocumentStates;
      List<TDoc> inUnexpectedState = new List<TDoc>();
      foreach (TDoc pipelineDoc in this.PipelineContext.PipelineDocs)
      {
        if (pipelineDoc.ShouldProcess && !expectedDocumentStates.Contains<PipelineDocumentState>(pipelineDoc.CurrentState))
          inUnexpectedState.Add(pipelineDoc);
      }
      return (IReadOnlyCollection<TDoc>) inUnexpectedState;
    }

    protected internal virtual void HandleDocumentFlowErrors(IReadOnlyCollection<TDoc> errorDocs) => this.PipelineContext.CurrentStage.HandleDocumentFlowErrors(this.PipelineContext, errorDocs);

    protected CorePipelineResult BuildPipelineResult(OperationStatus opStatus) => new CorePipelineResult(opStatus, this.PipelineResultData);

    public int PipelineExecutionsCompleted { get; private set; }

    protected Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    protected CorePipelineContext<TId, TDoc> PipelineContext { get; }

    protected internal virtual CorePipelineResultData PipelineResultData { get; protected set; } = new CorePipelineResultData();

    private bool HandleException(Exception ex)
    {
      this.PipelineContext.CurrentStage = (CorePipelineStage<TId, TDoc>) null;
      this.HandlePipelineError(ex);
      return false;
    }
  }
}
