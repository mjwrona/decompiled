// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataReaderCoreAsync
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataReaderCoreAsync : ODataReaderCore
  {
    protected ODataReaderCoreAsync(
      ODataInputContext inputContext,
      bool readingResourceSet,
      bool readingDelta,
      IODataReaderWriterListener listener)
      : base(inputContext, readingResourceSet, readingDelta, listener)
    {
    }

    protected abstract Task<bool> ReadAtStartImplementationAsync();

    protected abstract Task<bool> ReadAtResourceSetStartImplementationAsync();

    protected abstract Task<bool> ReadAtResourceSetEndImplementationAsync();

    protected abstract Task<bool> ReadAtResourceStartImplementationAsync();

    protected abstract Task<bool> ReadAtResourceEndImplementationAsync();

    protected abstract Task<bool> ReadAtDeletedResourceEndImplementationAsync();

    protected virtual Task<bool> ReadAtPrimitiveImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtPrimitiveImplementation));

    protected virtual Task<bool> ReadAtNestedPropertyInfoImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtNestedPropertyInfoImplementation));

    protected virtual Task<bool> ReadAtStreamImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtStreamImplementation));

    protected abstract Task<bool> ReadAtNestedResourceInfoStartImplementationAsync();

    protected abstract Task<bool> ReadAtNestedResourceInfoEndImplementationAsync();

    protected abstract Task<bool> ReadAtEntityReferenceLinkAsync();

    protected virtual Task<bool> ReadAtDeltaResourceSetStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeltaResourceSetStartImplementation));

    protected virtual Task<bool> ReadAtDeltaResourceSetEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeltaResourceSetEndImplementation));

    protected virtual Task<bool> ReadAtDeletedResourceStartImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeletedResourceStartImplementation));

    protected virtual Task<bool> ReadDeletedResourceEndImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeletedResourceEndImplementation));

    protected virtual Task<bool> ReadAtDeltaLinkImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeltaLinkImplementation));

    protected virtual Task<bool> ReadAtDeltaDeletedLinkImplementationAsync() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(((ODataReaderCore) this).ReadAtDeltaDeletedLinkImplementation));

    protected override Task<bool> ReadAsynchronously()
    {
      Task<bool> antecedentTask;
      switch (this.State)
      {
        case ODataReaderState.Start:
          antecedentTask = this.ReadAtStartImplementationAsync();
          break;
        case ODataReaderState.ResourceSetStart:
          antecedentTask = this.ReadAtResourceSetStartImplementationAsync();
          break;
        case ODataReaderState.ResourceSetEnd:
          antecedentTask = this.ReadAtResourceSetEndImplementationAsync();
          break;
        case ODataReaderState.ResourceStart:
          antecedentTask = TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.IncreaseResourceDepth())).FollowOnSuccessWithTask<bool>((Func<Task, Task<bool>>) (t => this.ReadAtResourceStartImplementationAsync()));
          break;
        case ODataReaderState.ResourceEnd:
          antecedentTask = TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.DecreaseResourceDepth())).FollowOnSuccessWithTask<bool>((Func<Task, Task<bool>>) (t => this.ReadAtResourceEndImplementationAsync()));
          break;
        case ODataReaderState.NestedResourceInfoStart:
          antecedentTask = this.ReadAtNestedResourceInfoStartImplementationAsync();
          break;
        case ODataReaderState.NestedResourceInfoEnd:
          antecedentTask = this.ReadAtNestedResourceInfoEndImplementationAsync();
          break;
        case ODataReaderState.EntityReferenceLink:
          antecedentTask = this.ReadAtEntityReferenceLinkAsync();
          break;
        case ODataReaderState.Exception:
        case ODataReaderState.Completed:
          antecedentTask = TaskUtils.GetFaultedTask<bool>((Exception) new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed((object) this.State)));
          break;
        case ODataReaderState.Primitive:
          antecedentTask = this.ReadAtPrimitiveImplementationAsync();
          break;
        case ODataReaderState.DeltaResourceSetStart:
          antecedentTask = this.ReadAtDeltaResourceSetStartImplementationAsync();
          break;
        case ODataReaderState.DeltaResourceSetEnd:
          antecedentTask = this.ReadAtDeltaResourceSetEndImplementationAsync();
          break;
        case ODataReaderState.DeletedResourceStart:
          antecedentTask = TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.IncreaseResourceDepth())).FollowOnSuccessWithTask<bool>((Func<Task, Task<bool>>) (t => this.ReadAtDeletedResourceStartImplementationAsync()));
          break;
        case ODataReaderState.DeletedResourceEnd:
          antecedentTask = TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.DecreaseResourceDepth())).FollowOnSuccessWithTask<bool>((Func<Task, Task<bool>>) (t => this.ReadAtDeletedResourceEndImplementationAsync()));
          break;
        case ODataReaderState.DeltaLink:
          antecedentTask = this.ReadAtDeltaLinkImplementationAsync();
          break;
        case ODataReaderState.DeltaDeletedLink:
          antecedentTask = this.ReadAtDeltaDeletedLinkImplementationAsync();
          break;
        case ODataReaderState.NestedProperty:
          antecedentTask = this.ReadAtNestedPropertyInfoImplementationAsync();
          break;
        case ODataReaderState.Stream:
          antecedentTask = this.ReadAtStreamImplementationAsync();
          break;
        default:
          antecedentTask = TaskUtils.GetFaultedTask<bool>((Exception) new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataReaderCoreAsync_ReadAsynchronously)));
          break;
      }
      return antecedentTask.FollowOnSuccessWith<bool, bool>((Func<Task<bool>, bool>) (t => t.Result));
    }
  }
}
