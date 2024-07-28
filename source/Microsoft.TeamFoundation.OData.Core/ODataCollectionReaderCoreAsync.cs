// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataCollectionReaderCoreAsync
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataCollectionReaderCoreAsync : ODataCollectionReaderCore
  {
    protected ODataCollectionReaderCoreAsync(
      ODataInputContext inputContext,
      IEdmTypeReference expectedItemTypeReference,
      IODataReaderWriterListener listener)
      : base(inputContext, expectedItemTypeReference, listener)
    {
    }

    protected abstract Task<bool> ReadAtStartImplementationAsync();

    protected abstract Task<bool> ReadAtCollectionStartImplementationAsync();

    protected abstract Task<bool> ReadAtValueImplementationAsync();

    protected abstract Task<bool> ReadAtCollectionEndImplementationAsync();

    protected override Task<bool> ReadAsynchronously()
    {
      switch (this.State)
      {
        case ODataCollectionReaderState.Start:
          return this.ReadAtStartImplementationAsync();
        case ODataCollectionReaderState.CollectionStart:
          return this.ReadAtCollectionStartImplementationAsync();
        case ODataCollectionReaderState.Value:
          return this.ReadAtValueImplementationAsync();
        case ODataCollectionReaderState.CollectionEnd:
          return this.ReadAtCollectionEndImplementationAsync();
        default:
          return TaskUtils.GetFaultedTask<bool>((Exception) new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataCollectionReaderCoreAsync_ReadAsynchronously)));
      }
    }
  }
}
