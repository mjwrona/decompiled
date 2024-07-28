// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphCreateServicePrincipalRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphCreateServicePrincipalRequest : 
    MicrosoftGraphClientRequest<MsGraphCreateServicePrincipalResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphCreateServicePrincipalRequest";

    public AadServicePrincipal ServicePrincipalToCreate { get; set; }

    public override string ToString()
    {
      AadServicePrincipal principalToCreate = this.ServicePrincipalToCreate;
      return "CreateServicePrincipalRequest{ServicePrincipal to create=" + (principalToCreate != null ? principalToCreate.Serialize<AadServicePrincipal>() : (string) null) + "}";
    }

    internal override void Validate() => ArgumentUtility.CheckForNull<AadServicePrincipal>(this.ServicePrincipalToCreate, "ServicePrincipalToCreate");

    internal override MsGraphCreateServicePrincipalResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750170, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphCreateServicePrincipalRequest), "Entering Microsoft Graph API for Create ServicePrincipal");
        IGraphServiceServicePrincipalsCollectionRequest ServicePrincipalRequest = graphServiceClient.ServicePrincipals.Request();
        ServicePrincipal graphServicePrincipalToCreate = MicrosoftGraphConverters.ConvertAadServicePrincipal(this.ServicePrincipalToCreate);
        return new MsGraphCreateServicePrincipalResponse()
        {
          ServicePrincipalCreated = MicrosoftGraphConverters.ConvertServicePrincipal(context.RunSynchronously<ServicePrincipal>((Func<Task<ServicePrincipal>>) (() => ServicePrincipalRequest.AddAsync(graphServicePrincipalToCreate, new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Create ServicePrincipal call returned null."))
        };
      }
      finally
      {
        context.Trace(44750172, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphCreateServicePrincipalRequest), "Leaving Microsoft Graph API for Create ServicePrincipal.");
      }
    }
  }
}
