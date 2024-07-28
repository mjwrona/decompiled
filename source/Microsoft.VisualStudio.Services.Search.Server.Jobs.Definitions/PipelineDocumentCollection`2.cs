// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.PipelineDocumentCollection`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public sealed class PipelineDocumentCollection<TId, TDoc> : IEnumerable<TDoc>, IEnumerable
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    private readonly IDictionary<TId, TDoc> m_documents;
    private readonly bool m_enableStrictDLITValidations;
    private readonly bool m_isDLITDocumentCreationEnabled;

    public PipelineDocumentCollection(
      bool enableStrictDLITValidations,
      bool isDLITDocumentCreationEnabled)
    {
      this.m_enableStrictDLITValidations = enableStrictDLITValidations;
      this.m_isDLITDocumentCreationEnabled = isDLITDocumentCreationEnabled;
      this.m_documents = (IDictionary<TId, TDoc>) new FriendlyDictionary<TId, TDoc>();
    }

    public PipelineDocumentCollection(IVssRequestContext requestContext)
      : this(requestContext.IsDLITStrictValidationEnabled(), requestContext.IsDLITDocumentCreationEnabled())
    {
    }

    public void Add(TDoc doc)
    {
      if (!this.m_isDLITDocumentCreationEnabled)
        return;
      if ((object) doc == null)
        throw new ArgumentNullException(nameof (doc));
      TDoc doc1;
      if (this.m_documents.TryGetValue(doc.Id, out doc1))
      {
        string message = FormattableString.Invariant(FormattableStringFactory.Create("Attempted to add pipeline document [{0}] with Id [{1}] but document [{2}] with same Id already exists.", (object) doc, (object) doc.Id, (object) doc1));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceErrorWithStackTrace(1082704, "Indexing Pipeline", "Pipeline", message);
        if (this.m_enableStrictDLITValidations)
          throw new SearchServiceException(message);
      }
      else
      {
        this.m_documents[doc.Id] = doc;
        doc.CurrentState = PipelineDocumentState.DocumentRegistered;
      }
    }

    public bool TryGetStrict(TId docId, out TDoc doc, bool ignoreShouldProcess = false) => this.TryGetInternal(docId, out doc, false, ignoreShouldProcess);

    public bool TryGetLenient(TId docId, out TDoc doc) => this.TryGetInternal(docId, out doc, true, true);

    private bool TryGetInternal(TId docId, out TDoc doc, bool lenient, bool ignoreShouldProcess)
    {
      if (this.m_isDLITDocumentCreationEnabled)
      {
        bool flag = this.m_documents.TryGetValue(docId, out doc) && doc.ShouldProcess | lenient | ignoreShouldProcess;
        if (!flag && !lenient)
        {
          string message = FormattableString.Invariant(FormattableStringFactory.Create("Attempting to fetch pipeline document with Id [{0}] failed. Reason: [{1}].", (object) docId, (object) doc == null ? (object) "Document was not recorded" : (object) "Document's ShouldProcess is false"));
          if ((object) doc != null)
            message += FormattableString.Invariant(FormattableStringFactory.Create(" Audit Trail: [{0}]", (object) doc.AuditTrail));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceErrorWithStackTrace(1082706, "Indexing Pipeline", "Pipeline", message);
          if (this.m_enableStrictDLITValidations)
            throw new SearchServiceException(message);
        }
        return flag;
      }
      doc = default (TDoc);
      return false;
    }

    internal void Clear() => this.m_documents.Clear();

    public IEnumerator<TDoc> GetEnumerator() => this.m_documents.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public int Count => this.m_documents.Count;

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Count = {0}", (object) this.Count));
  }
}
