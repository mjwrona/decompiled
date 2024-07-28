// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CorePipelineDocument`1
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public abstract class CorePipelineDocument<TId> where TId : IEquatable<TId>
  {
    private PipelineDocumentState m_currentState;

    protected CorePipelineDocument(TId id) => this.Id = id;

    protected CorePipelineDocument(TId id, PipelineDocumentState state)
      : this(id)
    {
      this.m_currentState = state;
    }

    public TId Id { get; }

    public bool ShouldProcess { get; set; } = true;

    protected virtual bool CanTransitionTo(PipelineDocumentState nextState) => true;

    public PipelineDocumentState CurrentState
    {
      get => this.m_currentState;
      set
      {
        PipelineDocumentState pipelineDocumentState = value;
        if (!this.ShouldProcess)
          return;
        if (CorePipelineDocument<TId>.IsUniversalTransition(pipelineDocumentState) || this.CanTransitionTo(pipelineDocumentState))
          this.m_currentState = pipelineDocumentState;
        else
          Tracer.TraceError(1082705, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Invalid state transition [{0} -> {1}] attempted for document [{2}] with audit trail: [{3}] at [{4}{5}]", (object) this.m_currentState, (object) pipelineDocumentState, (object) this, (object) this.AuditTrail, (object) Environment.NewLine, (object) EnvironmentWrapper.ToReadableStackTrace())));
      }
    }

    public AuditTrail AuditTrail { get; } = new AuditTrail();

    public abstract override string ToString();

    private static bool IsUniversalTransition(PipelineDocumentState state) => state == PipelineDocumentState.Failed || state == PipelineDocumentState.Skipped;
  }
}
