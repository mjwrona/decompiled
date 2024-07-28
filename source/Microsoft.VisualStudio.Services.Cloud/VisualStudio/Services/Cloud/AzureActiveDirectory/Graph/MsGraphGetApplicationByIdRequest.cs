// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetApplicationByIdRequest
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
  public class MsGraphGetApplicationByIdRequest : 
    MicrosoftGraphClientRequest<MsGraphGetApplicationByIdResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetApplicationByIdRequest";

    public Guid AppObjectId { get; set; }

    public override string ToString() => string.Format("GetApplicationByIdRequest{{Application ObjectId={0}}}", (object) this.AppObjectId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");

    internal override MsGraphGetApplicationByIdResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750220, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetApplicationByIdRequest), "Entering Microsoft Graph API for Get Application By Id");
        IApplicationRequest applicationRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].Request();
        AadApplication aadApplication = MicrosoftGraphConverters.ConvertApplication(context.RunSynchronously<Application>((Func<Task<Application>>) (() => applicationRequest.GetAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Get Application by Id call returned null or empty"));
        return new MsGraphGetApplicationByIdResponse()
        {
          Application = aadApplication
        };
      }
      finally
      {
        context.Trace(44750222, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetApplicationByIdRequest), "Leaving Microsoft Graph API for Get Application By Id.");
      }
    }
  }
}
