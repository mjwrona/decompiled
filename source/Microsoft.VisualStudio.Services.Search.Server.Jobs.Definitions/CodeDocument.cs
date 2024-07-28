// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CodeDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class CodeDocument : CorePipelineDocument<CodePipelineDocumentId>
  {
    private bool m_isWikiDoc;
    [StaticSafe]
    private static Dictionary<VersionControlType, Dictionary<PipelineDocumentState, HashSet<PipelineDocumentState>>> s_validStateTransition;
    private static readonly object s_stateTransitionlock = new object();

    public VersionControlType VcType { get; }

    public MetaDataStoreUpdateType UpdateType { get; set; }

    public bool HasMultipleDeleteContentHashes { get; set; }

    public bool? HasFailedFilesAsSource { get; set; }

    public bool IsWikiDoc
    {
      get => this.m_isWikiDoc;
      set => this.m_isWikiDoc = !this.m_isWikiDoc || value ? value : throw new InvalidOperationException("Once a document is marked as Wiki document, it cannot be undone.");
    }

    public CodeDocument(
      VersionControlType vcType,
      CodePipelineDocumentId codeDocumentId,
      MetaDataStoreUpdateType updateType,
      PipelineDocumentState state)
      : base(codeDocumentId, state)
    {
      this.VcType = vcType;
      this.UpdateType = updateType;
      if (CodeDocument.s_validStateTransition != null)
        return;
      lock (CodeDocument.s_stateTransitionlock)
      {
        if (CodeDocument.s_validStateTransition != null)
          return;
        CodeDocument.s_validStateTransition = new Dictionary<VersionControlType, Dictionary<PipelineDocumentState, HashSet<PipelineDocumentState>>>()
        {
          [VersionControlType.Custom] = new Dictionary<PipelineDocumentState, HashSet<PipelineDocumentState>>()
          {
            [PipelineDocumentState.DocumentCreated] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.DocumentRegistered
            },
            [PipelineDocumentState.DocumentRegistered] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.CustomCrawled,
              PipelineDocumentState.MetaDataCrawled
            },
            [PipelineDocumentState.MetaDataCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Crawled
            },
            [PipelineDocumentState.CustomCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.FedToSearchService
            },
            [PipelineDocumentState.Crawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Parsed
            },
            [PipelineDocumentState.Parsed] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.IndexedInElasticsearch
            },
            [PipelineDocumentState.IndexedInElasticsearch] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.FedToCustomStore
            }
          },
          [VersionControlType.Git] = new Dictionary<PipelineDocumentState, HashSet<PipelineDocumentState>>()
          {
            [PipelineDocumentState.DocumentCreated] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.DocumentRegistered
            },
            [PipelineDocumentState.DocumentRegistered] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.MetaDataCrawled
            },
            [PipelineDocumentState.MetaDataCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.ContentCrawled,
              PipelineDocumentState.Crawled
            },
            [PipelineDocumentState.ContentCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Crawled
            },
            [PipelineDocumentState.Crawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Parsed
            },
            [PipelineDocumentState.Parsed] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.IndexedInElasticsearch
            }
          },
          [VersionControlType.TFVC] = new Dictionary<PipelineDocumentState, HashSet<PipelineDocumentState>>()
          {
            [PipelineDocumentState.DocumentCreated] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.DocumentRegistered
            },
            [PipelineDocumentState.DocumentRegistered] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.ContentCrawled,
              PipelineDocumentState.MetaDataCrawled
            },
            [PipelineDocumentState.MetaDataCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.ContentCrawled,
              PipelineDocumentState.Crawled
            },
            [PipelineDocumentState.ContentCrawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Crawled
            },
            [PipelineDocumentState.Crawled] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.Parsed
            },
            [PipelineDocumentState.Parsed] = new HashSet<PipelineDocumentState>()
            {
              PipelineDocumentState.IndexedInElasticsearch
            }
          }
        };
      }
    }

    public CodeDocument(
      VersionControlType vcType,
      CodePipelineDocumentId codeDocumentId,
      MetaDataStoreUpdateType updateType)
      : this(vcType, codeDocumentId, updateType, PipelineDocumentState.DocumentCreated)
    {
    }

    public CodeDocument(VersionControlType vcType)
      : this(vcType, (CodePipelineDocumentId) Guid.NewGuid().ToString(), MetaDataStoreUpdateType.None)
    {
    }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("VcType: [{0}], Id: [{1}], UpdateType: [{2}] State: [{3}]", (object) this.VcType, (object) this.Id, (object) this.UpdateType, (object) this.CurrentState));

    protected override bool CanTransitionTo(PipelineDocumentState nextState)
    {
      switch (this.VcType)
      {
        case VersionControlType.Git:
        case VersionControlType.TFVC:
        case VersionControlType.Custom:
          HashSet<PipelineDocumentState> pipelineDocumentStateSet;
          return CodeDocument.s_validStateTransition[this.VcType].TryGetValue(this.CurrentState, out pipelineDocumentStateSet) && pipelineDocumentStateSet.Contains(nextState);
        default:
          return true;
      }
    }
  }
}
