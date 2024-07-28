// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Serializer.PreserveObjectReferencesAttribute
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Serializer
{
  public class PreserveObjectReferencesAttribute : Attribute, IOperationBehavior
  {
    public void AddBindingParameters(
      OperationDescription operationDescription,
      BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(
      OperationDescription operationDescription,
      ClientOperation clientOperation)
    {
      ((IOperationBehavior) new PreserveObjectReferencesOperationBehavior(operationDescription)).ApplyClientBehavior(operationDescription, clientOperation);
    }

    public void ApplyDispatchBehavior(
      OperationDescription operationDescription,
      DispatchOperation dispatchOperation)
    {
      ((IOperationBehavior) new PreserveObjectReferencesOperationBehavior(operationDescription)).ApplyDispatchBehavior(operationDescription, dispatchOperation);
    }

    public void Validate(OperationDescription operationDescription)
    {
    }
  }
}
