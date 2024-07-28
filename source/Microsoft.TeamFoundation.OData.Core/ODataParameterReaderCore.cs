// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataParameterReaderCore
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal abstract class ODataParameterReaderCore : ODataParameterReader, IODataReaderWriterListener
  {
    private readonly ODataInputContext inputContext;
    private readonly IEdmOperation operation;
    private readonly Stack<ODataParameterReaderCore.Scope> scopes = new Stack<ODataParameterReaderCore.Scope>();
    private readonly HashSet<string> parametersRead = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
    private ODataParameterReaderCore.SubReaderState subReaderState;

    protected ODataParameterReaderCore(ODataInputContext inputContext, IEdmOperation operation)
    {
      this.inputContext = inputContext;
      this.operation = operation;
      this.EnterScope(ODataParameterReaderState.Start, (string) null, (object) null);
    }

    public override sealed ODataParameterReaderState State
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().State;
      }
    }

    public override string Name
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().Name;
      }
    }

    public override object Value
    {
      get
      {
        this.inputContext.VerifyNotDisposed();
        return this.scopes.Peek().Value;
      }
    }

    protected IEdmOperation Operation => this.operation;

    public override ODataReader CreateResourceReader()
    {
      this.VerifyCanCreateSubReader(ODataParameterReaderState.Resource);
      this.subReaderState = ODataParameterReaderCore.SubReaderState.Active;
      return this.CreateResourceReader((IEdmStructuredType) this.GetParameterTypeReference(this.Name).Definition);
    }

    public override ODataReader CreateResourceSetReader()
    {
      this.VerifyCanCreateSubReader(ODataParameterReaderState.ResourceSet);
      this.subReaderState = ODataParameterReaderCore.SubReaderState.Active;
      return this.CreateResourceSetReader((IEdmStructuredType) ((IEdmCollectionType) this.GetParameterTypeReference(this.Name).Definition).ElementType.Definition);
    }

    public override ODataCollectionReader CreateCollectionReader()
    {
      this.VerifyCanCreateSubReader(ODataParameterReaderState.Collection);
      this.subReaderState = ODataParameterReaderCore.SubReaderState.Active;
      return this.CreateCollectionReader(((IEdmCollectionType) this.GetParameterTypeReference(this.Name).Definition).ElementType);
    }

    public override sealed bool Read()
    {
      this.VerifyCanRead(true);
      return this.InterceptException<bool>(new Func<bool>(this.ReadSynchronously));
    }

    public override sealed Task<bool> ReadAsync()
    {
      this.VerifyCanRead(false);
      return this.ReadAsynchronously().FollowOnFaultWith<bool>((Action<Task<bool>>) (t => this.EnterScope(ODataParameterReaderState.Exception, (string) null, (object) null)));
    }

    void IODataReaderWriterListener.OnException() => this.EnterScope(ODataParameterReaderState.Exception, (string) null, (object) null);

    void IODataReaderWriterListener.OnCompleted() => this.subReaderState = ODataParameterReaderCore.SubReaderState.Completed;

    protected internal IEdmTypeReference GetParameterTypeReference(string parameterName) => this.inputContext.EdmTypeResolver.GetParameterType(this.Operation.FindParameter(parameterName) ?? throw new ODataException(Strings.ODataParameterReaderCore_ParameterNameNotInMetadata((object) parameterName, (object) this.Operation.Name)));

    protected internal void EnterScope(ODataParameterReaderState state, string name, object value)
    {
      if (state == ODataParameterReaderState.Value && value != null && !EdmLibraryExtensions.IsPrimitiveType(value.GetType()) && !(value is ODataEnumValue))
        throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterReaderCore_ValueMustBePrimitiveOrNull));
      if (this.scopes.Count != 0 && this.State == ODataParameterReaderState.Exception)
        return;
      if (state == ODataParameterReaderState.Completed)
      {
        List<string> stringList = new List<string>();
        foreach (IEdmOperationParameter operationParameter in this.Operation.Parameters.Skip<IEdmOperationParameter>(this.Operation.IsBound ? 1 : 0))
        {
          if (!(operationParameter is IEdmOptionalParameter) && !this.parametersRead.Contains(operationParameter.Name) && !this.inputContext.EdmTypeResolver.GetParameterType(operationParameter).IsNullable)
            stringList.Add(operationParameter.Name);
        }
        if (stringList.Count > 0)
        {
          this.scopes.Push(new ODataParameterReaderCore.Scope(ODataParameterReaderState.Exception, (string) null, (object) null));
          throw new ODataException(Strings.ODataParameterReaderCore_ParametersMissingInPayload((object) this.Operation.Name, (object) string.Join(",", stringList.ToArray())));
        }
      }
      else if (name != null)
      {
        if (this.parametersRead.Contains(name))
          throw new ODataException(Strings.ODataParameterReaderCore_DuplicateParametersInPayload((object) name));
        this.parametersRead.Add(name);
      }
      this.scopes.Push(new ODataParameterReaderCore.Scope(state, name, value));
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
    protected internal void PopScope(ODataParameterReaderState state) => this.scopes.Pop();

    protected void OnParameterCompleted() => this.subReaderState = ODataParameterReaderCore.SubReaderState.None;

    protected bool ReadImplementation()
    {
      switch (this.State)
      {
        case ODataParameterReaderState.Start:
          return this.ReadAtStartImplementation();
        case ODataParameterReaderState.Value:
        case ODataParameterReaderState.Collection:
        case ODataParameterReaderState.Resource:
        case ODataParameterReaderState.ResourceSet:
          this.OnParameterCompleted();
          return this.ReadNextParameterImplementation();
        case ODataParameterReaderState.Exception:
        case ODataParameterReaderState.Completed:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterReaderCore_ReadImplementation));
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataParameterReaderCore_ReadImplementation));
      }
    }

    protected abstract bool ReadAtStartImplementation();

    protected abstract bool ReadNextParameterImplementation();

    protected abstract ODataReader CreateResourceReader(IEdmStructuredType expectedResourceType);

    protected abstract ODataReader CreateResourceSetReader(IEdmStructuredType expectedResourceType);

    protected abstract ODataCollectionReader CreateCollectionReader(
      IEdmTypeReference expectedItemTypeReference);

    protected bool ReadSynchronously() => this.ReadImplementation();

    protected virtual Task<bool> ReadAsynchronously() => TaskUtils.GetTaskForSynchronousOperation<bool>(new Func<bool>(this.ReadImplementation));

    private static string GetCreateReaderMethodName(ODataParameterReaderState state) => "Create" + state.ToString() + "Reader";

    private void VerifyCanCreateSubReader(ODataParameterReaderState expectedState)
    {
      this.inputContext.VerifyNotDisposed();
      if (this.State != expectedState)
        throw new ODataException(Strings.ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState((object) ODataParameterReaderCore.GetCreateReaderMethodName(expectedState), (object) this.State));
      if (this.subReaderState != ODataParameterReaderCore.SubReaderState.None)
        throw new ODataException(Strings.ODataParameterReaderCore_CreateReaderAlreadyCalled((object) ODataParameterReaderCore.GetCreateReaderMethodName(expectedState), (object) this.Name));
    }

    private T InterceptException<T>(Func<T> action)
    {
      try
      {
        return action();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          this.EnterScope(ODataParameterReaderState.Exception, (string) null, (object) null);
        throw;
      }
    }

    private void VerifyCanRead(bool synchronousCall)
    {
      this.inputContext.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (this.State == ODataParameterReaderState.Exception || this.State == ODataParameterReaderState.Completed)
        throw new ODataException(Strings.ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState((object) this.State));
      if (this.State != ODataParameterReaderState.Resource && this.State != ODataParameterReaderState.ResourceSet && this.State != ODataParameterReaderState.Collection)
        return;
      if (this.subReaderState == ODataParameterReaderCore.SubReaderState.None)
        throw new ODataException(Strings.ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall((object) this.State, (object) ODataParameterReaderCore.GetCreateReaderMethodName(this.State)));
      if (this.subReaderState == ODataParameterReaderCore.SubReaderState.Active)
        throw new ODataException(Strings.ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall((object) this.State, (object) ODataParameterReaderCore.GetCreateReaderMethodName(this.State)));
    }

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
        this.VerifySynchronousCallAllowed();
      else
        this.VerifyAsynchronousCallAllowed();
    }

    private void VerifySynchronousCallAllowed()
    {
      if (!this.inputContext.Synchronous)
        throw new ODataException(Strings.ODataParameterReaderCore_SyncCallOnAsyncReader);
    }

    private void VerifyAsynchronousCallAllowed()
    {
      if (this.inputContext.Synchronous)
        throw new ODataException(Strings.ODataParameterReaderCore_AsyncCallOnSyncReader);
    }

    private enum SubReaderState
    {
      None,
      Active,
      Completed,
    }

    protected sealed class Scope
    {
      private readonly ODataParameterReaderState state;
      private readonly string name;
      private readonly object value;

      public Scope(ODataParameterReaderState state, string name, object value)
      {
        this.state = state;
        this.name = name;
        this.value = value;
      }

      public ODataParameterReaderState State => this.state;

      public string Name => this.name;

      public object Value => this.value;
    }
  }
}
