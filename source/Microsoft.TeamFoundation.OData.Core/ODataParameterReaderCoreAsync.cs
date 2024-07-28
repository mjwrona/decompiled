// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataParameterReaderCoreAsync
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataParameterReaderCoreAsync : ODataParameterReaderCore
  {
    protected ODataParameterReaderCoreAsync(ODataInputContext inputContext, IEdmOperation operation)
      : base(inputContext, operation)
    {
    }

    protected abstract Task<bool> ReadAtStartImplementationAsync();

    protected abstract Task<bool> ReadNextParameterImplementationAsync();

    protected abstract Task<ODataReader> CreateResourceReaderAsync(
      IEdmStructuredType expectedResourceType);

    protected abstract Task<ODataReader> CreateResourceSetReaderAsync(
      IEdmStructuredType expectedResourceType);

    protected abstract Task<ODataCollectionReader> CreateCollectionReaderAsync(
      IEdmTypeReference expectedItemTypeReference);

    protected override Task<bool> ReadAsynchronously()
    {
      switch (this.State)
      {
        case ODataParameterReaderState.Start:
          return this.ReadAtStartImplementationAsync();
        case ODataParameterReaderState.Value:
        case ODataParameterReaderState.Collection:
        case ODataParameterReaderState.Resource:
        case ODataParameterReaderState.ResourceSet:
          this.OnParameterCompleted();
          return this.ReadNextParameterImplementationAsync();
        case ODataParameterReaderState.Exception:
        case ODataParameterReaderState.Completed:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));
      }
    }
  }
}
