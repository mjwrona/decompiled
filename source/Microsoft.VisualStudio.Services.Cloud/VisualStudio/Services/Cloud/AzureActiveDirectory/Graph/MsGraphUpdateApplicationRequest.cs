// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphUpdateApplicationRequest
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
  public class MsGraphUpdateApplicationRequest : 
    MicrosoftGraphClientRequest<MsGraphUpdateApplicationResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphUpdateApplicationRequest";

    public AadApplication ApplicationToUpdate { get; set; }

    public override string ToString()
    {
      AadApplication applicationToUpdate = this.ApplicationToUpdate;
      return "UpdateApplicationRequest{Application to update=" + (applicationToUpdate != null ? applicationToUpdate.Serialize<AadApplication>() : (string) null) + "}";
    }

    internal override void Validate() => ArgumentUtility.CheckForNull<AadApplication>(this.ApplicationToUpdate, "ApplicationToUpdate");

    internal override MsGraphUpdateApplicationResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750150, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphUpdateApplicationRequest), "Entering Microsoft Graph API for Update Application");
        Application applicationToUpdate = MicrosoftGraphConverters.ConvertAadApplication(this.ApplicationToUpdate);
        IApplicationRequest applicationRequest = graphServiceClient.Applications[this.ApplicationToUpdate.ObjectId.ToString()].Request();
        Application application = context.RunSynchronously<Application>((Func<Task<Application>>) (() => applicationRequest.UpdateAsync(applicationToUpdate, new CancellationToken())));
        return new MsGraphUpdateApplicationResponse()
        {
          ApplicationUpdated = MicrosoftGraphConverters.ConvertApplication(application)
        };
      }
      finally
      {
        context.Trace(44750152, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphUpdateApplicationRequest), "Leaving Microsoft Graph API for Update Application");
      }
    }
  }
}
