// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphDeleteApplicationRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphDeleteApplicationRequest : 
    MicrosoftGraphClientRequest<MsGraphDeleteApplicationResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphDeleteApplicationRequest";

    public Guid AppObjectId { get; set; }

    public override string ToString() => string.Format("DeleteApplicationRequest{{Application ObjectId={0}}}", (object) this.AppObjectId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");

    internal override MsGraphDeleteApplicationResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750200, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteApplicationRequest), "Entering Microsoft Graph API for Delete Application.");
        IApplicationRequest applicationRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].Request();
        context.RunSynchronously((Func<Task>) (() => applicationRequest.DeleteAsync(new CancellationToken())));
        return new MsGraphDeleteApplicationResponse()
        {
          Success = true
        };
      }
      catch (ServiceException ex) when (ex.IsResourceNotFoundError())
      {
        context.Trace(44750201, TraceLevel.Warning, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteApplicationRequest), string.Format("Failed to delete Application '{0}' bacause application does not exist, consider success. Exception: '{1}'.", (object) this.AppObjectId, (object) ex));
        return new MsGraphDeleteApplicationResponse()
        {
          Success = true
        };
      }
      finally
      {
        context.Trace(44750202, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteApplicationRequest), "Leaving Microsoft Graph API for Delete Application");
      }
    }
  }
}
