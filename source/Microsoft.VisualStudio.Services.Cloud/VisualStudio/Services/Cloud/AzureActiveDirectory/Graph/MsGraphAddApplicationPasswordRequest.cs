// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphAddApplicationPasswordRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphAddApplicationPasswordRequest : 
    MicrosoftGraphClientRequest<MsGraphAddApplicationPasswordResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphAddApplicationPasswordRequest";

    public Guid AppObjectId { get; set; }

    public DateTime SecretExpirationDate { get; set; }

    public override string ToString() => string.Format("AddApplicationPasswordRequest{{Application ObjectId={0}}}", (object) this.AppObjectId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");

    internal override MsGraphAddApplicationPasswordResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750180, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphAddApplicationPasswordRequest), "Entering Microsoft Graph API for Add Application Password.");
        IApplicationAddPasswordRequest applicationRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].AddPassword(new PasswordCredential()
        {
          EndDateTime = new DateTimeOffset?((DateTimeOffset) this.SecretExpirationDate)
        }).Request((IEnumerable<Option>) null);
        return new MsGraphAddApplicationPasswordResponse()
        {
          PasswordCredentialCreated = MicrosoftGraphConverters.ConvertPasswordCredential(context.RunSynchronously<PasswordCredential>((Func<Task<PasswordCredential>>) (() => applicationRequest.PostAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Create Application call returned null."))
        };
      }
      finally
      {
        context.Trace(44750182, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphAddApplicationPasswordRequest), "Leaving Microsoft Graph API for Add Application Password.");
      }
    }
  }
}
