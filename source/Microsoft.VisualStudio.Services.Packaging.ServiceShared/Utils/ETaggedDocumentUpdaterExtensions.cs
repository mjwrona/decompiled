// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ETaggedDocumentUpdaterExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ETaggedDocumentUpdaterExtensions
  {
    public static 
    #nullable disable
    Task<(EtagValue<TDoc> FinalDocument, bool DidUpdate)> UpdateETaggedDocumentAsync<TDoc>(
      this IETaggedDocumentUpdater updater,
      EtagValue<TDoc>? initialDoc,
      Func<Task<EtagValue<TDoc>>> getDocument,
      Func<TDoc, (TDoc NewDoc, bool NeedSave)> applyDelta,
      Func<EtagValue<TDoc>, Task<string>> putDocument,
      Func<string> generateChangeConflictExceptionMessage)
    {
      return updater.UpdateETaggedDocumentAsync<TDoc>(initialDoc, getDocument, (Func<TDoc, Task<(TDoc, bool)>>) (doc => Task.FromResult<(TDoc, bool)>(applyDelta(doc))), putDocument, generateChangeConflictExceptionMessage);
    }

    public static Task<(EtagValue<TDoc> FinalDocument, bool DidUpdate)> UpdateETaggedDocumentAsync<TDoc, TSpecifier>(
      this IETaggedDocumentUpdater updater,
      EtagValue<TDoc>? initialDoc,
      IAggregationDocumentProvider<TDoc, TSpecifier> docProvider,
      IFeedRequest feedRequest,
      TSpecifier specifier,
      Func<TDoc, Task<(TDoc NewDoc, bool NeedSave)>> applyDelta,
      Func<string> generateChangeConflictExceptionMessage)
    {
      return updater.UpdateETaggedDocumentAsync<TDoc>(initialDoc, (Func<Task<EtagValue<TDoc>>>) (async () => await docProvider.GetDocumentAsync(feedRequest, specifier)), applyDelta, (Func<EtagValue<TDoc>, Task<string>>) (async doc => await docProvider.PutDocumentAsync(feedRequest, specifier, doc.Value, doc.Etag)), generateChangeConflictExceptionMessage);
    }

    public static Task<(EtagValue<TDoc> FinalDocument, bool DidUpdate)> UpdateETaggedDocumentAsync<TDoc, TSpecifier>(
      this IETaggedDocumentUpdater updater,
      EtagValue<TDoc>? initialDoc,
      IAggregationDocumentProvider<TDoc, TSpecifier> docProvider,
      IFeedRequest feedRequest,
      TSpecifier specifier,
      Func<TDoc, (TDoc NewDoc, bool NeedSave)> applyDelta,
      Func<string> generateChangeConflictExceptionMessage)
    {
      return updater.UpdateETaggedDocumentAsync<TDoc>(initialDoc, (Func<Task<EtagValue<TDoc>>>) (async () => await docProvider.GetDocumentAsync(feedRequest, specifier)), applyDelta, (Func<EtagValue<TDoc>, Task<string>>) (async doc => await docProvider.PutDocumentAsync(feedRequest, specifier, doc.Value, doc.Etag)), generateChangeConflictExceptionMessage);
    }
  }
}
