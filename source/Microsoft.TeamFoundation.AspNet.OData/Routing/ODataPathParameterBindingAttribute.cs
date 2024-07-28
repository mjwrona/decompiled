// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPathParameterBindingAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Microsoft.AspNet.OData.Routing
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
  public sealed class ODataPathParameterBindingAttribute : ParameterBindingAttribute
  {
    public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter) => (HttpParameterBinding) new ODataPathParameterBindingAttribute.ODataPathParameterBinding(parameter);

    internal class ODataPathParameterBinding : HttpParameterBinding
    {
      public ODataPathParameterBinding(HttpParameterDescriptor parameterDescriptor)
        : base(parameterDescriptor)
      {
      }

      public override Task ExecuteBindingAsync(
        ModelMetadataProvider metadataProvider,
        HttpActionContext actionContext,
        CancellationToken cancellationToken)
      {
        this.SetValue(actionContext, (object) ((actionContext != null ? actionContext.Request : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionContext))) ?? throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionContext), SRResources.ActionContextMustHaveRequest)).ODataProperties().Path);
        return Microsoft.AspNet.OData.Common.TaskHelpers.Completed();
      }
    }
  }
}
