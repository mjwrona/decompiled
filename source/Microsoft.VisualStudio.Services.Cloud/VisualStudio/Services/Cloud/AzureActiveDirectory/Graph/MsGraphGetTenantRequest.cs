// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetTenantRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetTenantRequest : MicrosoftGraphClientRequest<MsGraphGetTenantReponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetTenantRequest";

    public override string ToString() => "GetTenantRequest";

    internal override MsGraphGetTenantReponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750050, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetTenantRequest), "Entering Microsoft Graph API for Get Tenant");
        IGraphServiceOrganizationCollectionRequest orgRequest = graphServiceClient.Organization.Request();
        IGraphServiceOrganizationCollectionPage source = context.RunSynchronously<IGraphServiceOrganizationCollectionPage>((Func<Task<IGraphServiceOrganizationCollectionPage>>) (() => orgRequest.GetAsync(new CancellationToken())));
        AadTenant aadTenant = source != null && ((IEnumerable<Organization>) source).Any<Organization>() ? MicrosoftGraphConverters.ConvertTenant(((IEnumerable<Organization>) source).First<Organization>()) : throw new MicrosoftGraphException("Microsoft Graph API Get Tenant call returned null or empty");
        return new MsGraphGetTenantReponse()
        {
          Tenant = aadTenant
        };
      }
      finally
      {
        context.Trace(44750052, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetTenantRequest), "Leaving Microsoft Graph API for Get Tenant");
      }
    }
  }
}
