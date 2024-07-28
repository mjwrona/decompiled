// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ETaggedDocumentUpdater
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Data.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class ETaggedDocumentUpdater : IETaggedDocumentUpdater
  {
    public async Task<(EtagValue<TDoc> FinalDocument, bool DidUpdate)> UpdateETaggedDocumentAsync<TDoc>(
      EtagValue<TDoc>? initialDoc,
      Func<Task<EtagValue<TDoc>>> getDocument,
      Func<TDoc, Task<(TDoc NewDoc, bool NeedSave)>> applyDelta,
      Func<EtagValue<TDoc>, Task<string>> putDocument,
      Func<string> generateChangeConflictExceptionMessage)
    {
      Random r = new Random();
      for (int iterations = 0; iterations < 10; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        TDoc doc;
        string etag;
        TDoc doc1;
        string etag1;
        if (iterations == 0 && initialDoc.HasValue)
        {
          initialDoc.Value.Deconstruct<TDoc>(out doc1, out etag1);
          doc = doc1;
          etag = etag1;
        }
        else
        {
          (await getDocument()).Deconstruct<TDoc>(out doc1, out etag1);
          doc = doc1;
          etag = etag1;
        }
        (TDoc, bool) valueTuple = await applyDelta(doc);
        TDoc newDoc = valueTuple.Item1;
        if (!valueTuple.Item2)
          return (new EtagValue<TDoc>(doc, etag), false);
        string etag2 = await putDocument(new EtagValue<TDoc>(newDoc, etag));
        if (etag2 != null)
          return (new EtagValue<TDoc>(newDoc, etag2), true);
        doc = default (TDoc);
        etag = (string) null;
        newDoc = default (TDoc);
      }
      throw new ChangeConflictException(generateChangeConflictExceptionMessage());
    }
  }
}
