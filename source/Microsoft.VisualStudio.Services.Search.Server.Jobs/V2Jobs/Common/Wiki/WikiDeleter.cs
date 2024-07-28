// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Wiki.WikiDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Wiki
{
  public class WikiDeleter
  {
    protected const int s_tracePoint = 1080720;
    protected const string s_traceArea = "Indexing Pipeline";
    protected const string s_traceLayer = "IndexingOperation";

    internal bool PerformBulkDelete(
      IndexingExecutionContext executionContext,
      IndexIdentity searchIndex,
      IExpression query)
    {
      return new IndexOperations().PerformBulkDelete(executionContext, searchIndex, query).Success;
    }

    public virtual IExpression GetQueryExpression(
      IndexingExecutionContext executionContext,
      WikiDeleteMetadata wikiDeleteMetadata)
    {
      List<IExpression> set = new List<IExpression>();
      if (!string.IsNullOrEmpty(wikiDeleteMetadata.WikiId))
        set.Add((IExpression) new TermExpression("wikiId", Operator.Equals, wikiDeleteMetadata.WikiId));
      return set.Count > 0 ? (IExpression) new AndExpression((IEnumerable<IExpression>) set) : (IExpression) new EmptyExpression();
    }

    public virtual List<WikiDeleteMetadata> DeleteWikis(
      IndexingExecutionContext executionContext,
      List<WikiDeleteMetadata> wikiDeleteMetadataList,
      IndexIdentity searchIndex)
    {
      List<WikiDeleteMetadata> wikiDeleteMetadataList1 = new List<WikiDeleteMetadata>();
      foreach (WikiDeleteMetadata wikiDeleteMetadata in wikiDeleteMetadataList)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Index : '{0}' for Repository {1} and Wiki {2}", (object) searchIndex, (object) wikiDeleteMetadata.RepositoryId, (object) wikiDeleteMetadata.WikiId);
        bool flag = false;
        try
        {
          if (this.PerformBulkDelete(executionContext, searchIndex, this.GetQueryExpression(executionContext, wikiDeleteMetadata)))
          {
            Tracer.TraceInfo(1080720, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of documents from {0} succeeded.", (object) str));
            flag = true;
          }
          else
            Tracer.TraceError(1080720, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of documents from {0} failed.", (object) str));
        }
        catch (Exception ex)
        {
          Tracer.TraceError(1080720, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of documents from {0} failed with exception: {1}", (object) str, (object) ex));
        }
        finally
        {
          if (!flag)
            wikiDeleteMetadataList1.Add(wikiDeleteMetadata);
        }
      }
      return wikiDeleteMetadataList1;
    }
  }
}
