// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataParameterWriterCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataParameterWriterCore : 
    ODataParameterWriter,
    IODataReaderWriterListener,
    IODataOutputInStreamErrorListener
  {
    private readonly ODataOutputContext outputContext;
    private readonly IEdmOperation operation;
    private Stack<ODataParameterWriterCore.ParameterWriterState> scopes = new Stack<ODataParameterWriterCore.ParameterWriterState>();
    private HashSet<string> parameterNamesWritten = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
    private IDuplicatePropertyNameChecker duplicatePropertyNameChecker;

    protected ODataParameterWriterCore(ODataOutputContext outputContext, IEdmOperation operation)
    {
      this.outputContext = outputContext;
      this.operation = operation;
      this.scopes.Push(ODataParameterWriterCore.ParameterWriterState.Start);
    }

    protected IDuplicatePropertyNameChecker DuplicatePropertyNameChecker => this.duplicatePropertyNameChecker ?? (this.duplicatePropertyNameChecker = this.outputContext.MessageWriterSettings.Validator.CreateDuplicatePropertyNameChecker());

    private ODataParameterWriterCore.ParameterWriterState State => this.scopes.Peek();

    public override sealed void Flush()
    {
      this.VerifyCanFlush(true);
      this.InterceptException(new Action(this.FlushSynchronously));
    }

    public override sealed Task FlushAsync()
    {
      this.VerifyCanFlush(false);
      return this.FlushAsynchronously().FollowOnFaultWith((Action<Task>) (t => this.EnterErrorScope()));
    }

    public override sealed void WriteStart()
    {
      this.VerifyCanWriteStart(true);
      this.InterceptException((Action) (() => this.WriteStartImplementation()));
    }

    public override sealed Task WriteStartAsync()
    {
      this.VerifyCanWriteStart(false);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.InterceptException((Action) (() => this.WriteStartImplementation()))));
    }

    public override sealed void WriteValue(string parameterName, object parameterValue)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference expectedTypeReference = this.VerifyCanWriteValueParameter(true, parameterName, parameterValue);
      this.InterceptException((Action) (() => this.WriteValueImplementation(parameterName, parameterValue, expectedTypeReference)));
    }

    public override sealed Task WriteValueAsync(string parameterName, object parameterValue)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference expectedTypeReference = this.VerifyCanWriteValueParameter(false, parameterName, parameterValue);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.InterceptException((Action) (() => this.WriteValueImplementation(parameterName, parameterValue, expectedTypeReference)))));
    }

    public override sealed ODataCollectionWriter CreateCollectionWriter(string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateCollectionWriter(true, parameterName);
      return this.InterceptException<ODataCollectionWriter>((Func<ODataCollectionWriter>) (() => this.CreateCollectionWriterImplementation(parameterName, itemTypeReference)));
    }

    public override sealed Task<ODataCollectionWriter> CreateCollectionWriterAsync(
      string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateCollectionWriter(false, parameterName);
      return TaskUtils.GetTaskForSynchronousOperation<ODataCollectionWriter>((Func<ODataCollectionWriter>) (() => this.InterceptException<ODataCollectionWriter>((Func<ODataCollectionWriter>) (() => this.CreateCollectionWriterImplementation(parameterName, itemTypeReference)))));
    }

    public override sealed ODataWriter CreateResourceWriter(string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateResourceWriter(true, parameterName);
      return this.InterceptException<ODataWriter>((Func<ODataWriter>) (() => this.CreateResourceWriterImplementation(parameterName, itemTypeReference)));
    }

    public override sealed Task<ODataWriter> CreateResourceWriterAsync(string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateResourceWriter(false, parameterName);
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.InterceptException<ODataWriter>((Func<ODataWriter>) (() => this.CreateResourceWriterImplementation(parameterName, itemTypeReference)))));
    }

    public override sealed ODataWriter CreateResourceSetWriter(string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateResourceSetWriter(true, parameterName);
      return this.InterceptException<ODataWriter>((Func<ODataWriter>) (() => this.CreateResourceSetWriterImplementation(parameterName, itemTypeReference)));
    }

    public override sealed Task<ODataWriter> CreateResourceSetWriterAsync(string parameterName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(parameterName, nameof (parameterName));
      IEdmTypeReference itemTypeReference = this.VerifyCanCreateResourceSetWriter(false, parameterName);
      return TaskUtils.GetTaskForSynchronousOperation<ODataWriter>((Func<ODataWriter>) (() => this.InterceptException<ODataWriter>((Func<ODataWriter>) (() => this.CreateResourceSetWriterImplementation(parameterName, itemTypeReference)))));
    }

    public override sealed void WriteEnd()
    {
      this.VerifyCanWriteEnd(true);
      this.InterceptException((Action) (() => this.WriteEndImplementation()));
      if (this.State != ODataParameterWriterCore.ParameterWriterState.Completed)
        return;
      this.Flush();
    }

    public override sealed Task WriteEndAsync()
    {
      this.VerifyCanWriteEnd(false);
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.InterceptException((Action) (() => this.WriteEndImplementation())))).FollowOnSuccessWithTask((Func<Task, Task>) (task => this.State == ODataParameterWriterCore.ParameterWriterState.Completed ? this.FlushAsync() : TaskUtils.CompletedTask));
    }

    void IODataReaderWriterListener.OnException() => this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.Error);

    void IODataReaderWriterListener.OnCompleted() => this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.CanWriteParameter);

    void IODataOutputInStreamErrorListener.OnInStreamError() => throw new ODataException(Strings.ODataParameterWriter_InStreamErrorNotSupported);

    protected abstract void VerifyNotDisposed();

    protected abstract void FlushSynchronously();

    protected abstract Task FlushAsynchronously();

    protected abstract void StartPayload();

    protected abstract void WriteValueParameter(
      string parameterName,
      object parameterValue,
      IEdmTypeReference expectedTypeReference);

    protected abstract ODataCollectionWriter CreateFormatCollectionWriter(
      string parameterName,
      IEdmTypeReference expectedItemType);

    protected abstract ODataWriter CreateFormatResourceWriter(
      string parameterName,
      IEdmTypeReference expectedItemType);

    protected abstract ODataWriter CreateFormatResourceSetWriter(
      string parameterName,
      IEdmTypeReference expectedItemType);

    protected abstract void EndPayload();

    private void VerifyCanWriteStart(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (this.State != ODataParameterWriterCore.ParameterWriterState.Start)
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteStart);
    }

    private void WriteStartImplementation()
    {
      this.InterceptException(new Action(this.StartPayload));
      this.EnterScope(ODataParameterWriterCore.ParameterWriterState.CanWriteParameter);
    }

    private IEdmTypeReference VerifyCanWriteParameterAndGetTypeReference(
      bool synchronousCall,
      string parameterName)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      this.VerifyNotInErrorOrCompletedState();
      if (this.State != ODataParameterWriterCore.ParameterWriterState.CanWriteParameter)
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteParameter);
      if (this.parameterNamesWritten.Contains(parameterName))
        throw new ODataException(Strings.ODataParameterWriterCore_DuplicatedParameterNameNotAllowed((object) parameterName));
      this.parameterNamesWritten.Add(parameterName);
      return this.GetParameterTypeReference(parameterName);
    }

    private IEdmTypeReference VerifyCanWriteValueParameter(
      bool synchronousCall,
      string parameterName,
      object parameterValue)
    {
      IEdmTypeReference typeReference = this.VerifyCanWriteParameterAndGetTypeReference(synchronousCall, parameterName);
      if (typeReference != null && !typeReference.IsODataPrimitiveTypeKind() && !typeReference.IsODataEnumTypeKind() && !typeReference.IsODataTypeDefinitionTypeKind())
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind((object) parameterName, (object) typeReference.TypeKind()));
      if (parameterValue != null && (!EdmLibraryExtensions.IsPrimitiveType(parameterValue.GetType()) || parameterValue is Stream) && !(parameterValue is ODataEnumValue))
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType((object) parameterName, (object) parameterValue.GetType()));
      return typeReference;
    }

    private IEdmTypeReference VerifyCanCreateCollectionWriter(
      bool synchronousCall,
      string parameterName)
    {
      IEdmTypeReference typeReference = this.VerifyCanWriteParameterAndGetTypeReference(synchronousCall, parameterName);
      if (typeReference != null && !typeReference.IsNonEntityCollectionType())
        throw new ODataException(Strings.ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind((object) parameterName, (object) typeReference.TypeKind()));
      return typeReference != null ? typeReference.GetCollectionItemType() : (IEdmTypeReference) null;
    }

    private IEdmTypeReference VerifyCanCreateResourceWriter(
      bool synchronousCall,
      string parameterName)
    {
      IEdmTypeReference typeReference = this.VerifyCanWriteParameterAndGetTypeReference(synchronousCall, parameterName);
      return typeReference == null || typeReference.IsStructured() ? typeReference : throw new ODataException(Strings.ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind((object) parameterName, (object) typeReference.TypeKind()));
    }

    private IEdmTypeReference VerifyCanCreateResourceSetWriter(
      bool synchronousCall,
      string parameterName)
    {
      IEdmTypeReference typeReference = this.VerifyCanWriteParameterAndGetTypeReference(synchronousCall, parameterName);
      return typeReference == null || typeReference.IsStructuredCollectionType() ? typeReference : throw new ODataException(Strings.ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind((object) parameterName, (object) typeReference.TypeKind()));
    }

    private IEdmTypeReference GetParameterTypeReference(string parameterName)
    {
      if (this.operation == null)
        return (IEdmTypeReference) null;
      return this.outputContext.EdmTypeResolver.GetParameterType(this.operation.FindParameter(parameterName) ?? throw new ODataException(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation((object) parameterName, (object) this.operation.Name)));
    }

    private void WriteValueImplementation(
      string parameterName,
      object parameterValue,
      IEdmTypeReference expectedTypeReference)
    {
      this.InterceptException((Action) (() => this.WriteValueParameter(parameterName, parameterValue, expectedTypeReference)));
    }

    private ODataCollectionWriter CreateCollectionWriterImplementation(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      ODataCollectionWriter collectionWriter = this.CreateFormatCollectionWriter(parameterName, expectedItemType);
      this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.ActiveSubWriter);
      return collectionWriter;
    }

    private ODataWriter CreateResourceWriterImplementation(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      ODataWriter formatResourceWriter = this.CreateFormatResourceWriter(parameterName, expectedItemType);
      this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.ActiveSubWriter);
      return formatResourceWriter;
    }

    private ODataWriter CreateResourceSetWriterImplementation(
      string parameterName,
      IEdmTypeReference expectedItemType)
    {
      ODataWriter resourceSetWriter = this.CreateFormatResourceSetWriter(parameterName, expectedItemType);
      this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.ActiveSubWriter);
      return resourceSetWriter;
    }

    private void VerifyCanWriteEnd(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      this.VerifyNotInErrorOrCompletedState();
      if (this.State != ODataParameterWriterCore.ParameterWriterState.CanWriteParameter)
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteEnd);
      this.VerifyAllParametersWritten();
    }

    private void VerifyAllParametersWritten()
    {
      if (this.operation == null || this.operation.Parameters == null)
        return;
      IEnumerable<string> source = (!this.operation.IsBound ? this.operation.Parameters : this.operation.Parameters.Skip<IEdmOperationParameter>(1)).Where<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => !this.parameterNamesWritten.Contains(p.Name) && !this.outputContext.EdmTypeResolver.GetParameterType(p).IsNullable)).Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name));
      if (source.Any<string>())
        throw new ODataException(Strings.ODataParameterWriterCore_MissingParameterInParameterPayload((object) string.Join(", ", source.Select<string, string>((Func<string, string>) (name => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", new object[1]
        {
          (object) name
        }))).ToArray<string>()), (object) this.operation.Name));
    }

    private void WriteEndImplementation()
    {
      this.InterceptException((Action) (() => this.EndPayload()));
      this.LeaveScope();
    }

    private void VerifyNotInErrorOrCompletedState()
    {
      if (this.State == ODataParameterWriterCore.ParameterWriterState.Error || this.State == ODataParameterWriterCore.ParameterWriterState.Completed)
        throw new ODataException(Strings.ODataParameterWriterCore_CannotWriteInErrorOrCompletedState);
    }

    private void VerifyCanFlush(bool synchronousCall)
    {
      this.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.outputContext.Synchronous)
          throw new ODataException(Strings.ODataParameterWriterCore_SyncCallOnAsyncWriter);
      }
      else if (this.outputContext.Synchronous)
        throw new ODataException(Strings.ODataParameterWriterCore_AsyncCallOnSyncWriter);
    }

    private void InterceptException(Action action)
    {
      try
      {
        action();
      }
      catch
      {
        this.EnterErrorScope();
        throw;
      }
    }

    private T InterceptException<T>(Func<T> function)
    {
      try
      {
        return function();
      }
      catch
      {
        this.EnterErrorScope();
        throw;
      }
    }

    private void EnterErrorScope()
    {
      if (this.State == ODataParameterWriterCore.ParameterWriterState.Error)
        return;
      this.EnterScope(ODataParameterWriterCore.ParameterWriterState.Error);
    }

    private void EnterScope(
      ODataParameterWriterCore.ParameterWriterState newState)
    {
      this.ValidateTransition(newState);
      this.scopes.Push(newState);
    }

    private void LeaveScope()
    {
      this.ValidateTransition(ODataParameterWriterCore.ParameterWriterState.Completed);
      if (this.State == ODataParameterWriterCore.ParameterWriterState.CanWriteParameter)
      {
        int num = (int) this.scopes.Pop();
      }
      this.ReplaceScope(ODataParameterWriterCore.ParameterWriterState.Completed);
    }

    private void ReplaceScope(
      ODataParameterWriterCore.ParameterWriterState newState)
    {
      this.ValidateTransition(newState);
      int num = (int) this.scopes.Pop();
      this.scopes.Push(newState);
    }

    private void ValidateTransition(
      ODataParameterWriterCore.ParameterWriterState newState)
    {
      if (this.State != ODataParameterWriterCore.ParameterWriterState.Error && newState == ODataParameterWriterCore.ParameterWriterState.Error)
        return;
      switch (this.State)
      {
        case ODataParameterWriterCore.ParameterWriterState.Start:
          if (newState == ODataParameterWriterCore.ParameterWriterState.CanWriteParameter || newState == ODataParameterWriterCore.ParameterWriterState.Completed)
            break;
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromStart));
        case ODataParameterWriterCore.ParameterWriterState.CanWriteParameter:
          if (newState == ODataParameterWriterCore.ParameterWriterState.CanWriteParameter || newState == ODataParameterWriterCore.ParameterWriterState.ActiveSubWriter || newState == ODataParameterWriterCore.ParameterWriterState.Completed)
            break;
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromCanWriteParameter));
        case ODataParameterWriterCore.ParameterWriterState.ActiveSubWriter:
          if (newState == ODataParameterWriterCore.ParameterWriterState.CanWriteParameter)
            break;
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromActiveSubWriter));
        case ODataParameterWriterCore.ParameterWriterState.Completed:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromCompleted));
        case ODataParameterWriterCore.ParameterWriterState.Error:
          if (newState == ODataParameterWriterCore.ParameterWriterState.Error)
            break;
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromError));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterWriterCore_ValidateTransition_UnreachableCodePath));
      }
    }

    private enum ParameterWriterState
    {
      Start,
      CanWriteParameter,
      ActiveSubWriter,
      Completed,
      Error,
    }
  }
}
