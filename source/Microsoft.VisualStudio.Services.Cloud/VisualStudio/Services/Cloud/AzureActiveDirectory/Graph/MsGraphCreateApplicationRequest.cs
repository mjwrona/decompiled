// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphCreateApplicationRequest
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
  public class MsGraphCreateApplicationRequest : 
    MicrosoftGraphClientRequest<MsGraphCreateApplicationResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphCreateApplicationRequest";

    public AadApplication ApplicationToCreate { get; set; }

    public override string ToString()
    {
      AadApplication applicationToCreate = this.ApplicationToCreate;
      return "CreateApplicationRequest{Application to create=" + (applicationToCreate != null ? applicationToCreate.Serialize<AadApplication>() : (string) null) + "}";
    }

    internal override void Validate() => ArgumentUtility.CheckForNull<AadApplication>(this.ApplicationToCreate, "ApplicationToCreate");

    internal override MsGraphCreateApplicationResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750160, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphCreateApplicationRequest), "Entering Microsoft Graph API for Create Application.");
        IGraphServiceApplicationsCollectionRequest applicationRequest = graphServiceClient.Applications.Request();
        Application graphApplicationToCreate = MicrosoftGraphConverters.ConvertAadApplication(this.ApplicationToCreate);
        return new MsGraphCreateApplicationResponse()
        {
          ApplicationCreated = MicrosoftGraphConverters.ConvertApplication(context.RunSynchronously<Application>((Func<Task<Application>>) (() => applicationRequest.AddAsync(graphApplicationToCreate, new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Create Application call returned null."))
        };
      }
      finally
      {
        context.Trace(44750162, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphCreateApplicationRequest), "Leaving Microsoft Graph API for Create Application.");
      }
    }
  }
}
