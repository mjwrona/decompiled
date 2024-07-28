// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceOperationsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class ResourceOperationsControllerBase : CsmControllerBase
  {
    internal override string Layer => nameof (ResourceOperationsControllerBase);

    [HttpGet]
    [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
    [SetCsmV2ResponseHeaders]
    [CsmControllerExceptionHandler(5106111)]
    [ClientResponseType(typeof (OperationListResult), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the possible operations on the Microsoft.VisualStudio resource provider.", false)]
    public virtual HttpResponseMessage Operations_List() => this.CreateHttpResponse((object) new OperationListResult()
    {
      value = {
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, OperationAction.Write),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, OperationAction.Delete),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, OperationAction.Read),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Project, OperationAction.Write),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Project, OperationAction.Delete),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Project, OperationAction.Read),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, ResourceProvider.Project, OperationAction.Read),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, ResourceProvider.Project, OperationAction.Write),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Extension, OperationAction.Write),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Extension, OperationAction.Delete),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Extension, OperationAction.Read),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.Account, ResourceProvider.Extension, OperationAction.Read),
        Operation.GetOperationDescriptorForRPandAction(ResourceProvider.None, OperationAction.Action)
      }
    });
  }
}
