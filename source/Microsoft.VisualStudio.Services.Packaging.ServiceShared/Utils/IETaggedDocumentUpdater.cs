// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.IETaggedDocumentUpdater
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public interface IETaggedDocumentUpdater
  {
    Task<(EtagValue<TDoc> FinalDocument, bool DidUpdate)> UpdateETaggedDocumentAsync<TDoc>(
      EtagValue<TDoc>? initialDoc,
      Func<Task<EtagValue<TDoc>>> getDocument,
      Func<TDoc, Task<(TDoc NewDoc, bool NeedSave)>> applyDelta,
      Func<EtagValue<TDoc>, Task<string>> putDocument,
      Func<string> generateChangeConflictExceptionMessage);
  }
}
