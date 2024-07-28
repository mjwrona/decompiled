// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphRemoveApplicationPasswordRequest
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
  public class MsGraphRemoveApplicationPasswordRequest : 
    MicrosoftGraphClientRequest<MsGraphRemoveApplicationPasswordResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphRemoveApplicationPasswordRequest";

    public Guid AppObjectId { get; set; }

    public Guid PasswordKeyId { get; set; }

    public override string ToString() => string.Format("RemoveApplicationPasswordRequest{{Application ObjectId={0},PasswordKeyId={1}}}", (object) this.AppObjectId, (object) this.PasswordKeyId);

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.PasswordKeyId, "PasswordKeyId");
      ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");
    }

    internal override MsGraphRemoveApplicationPasswordResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750190, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationPasswordRequest), "Entering Microsoft Graph API for Remove Application Password.");
        IApplicationRemovePasswordRequest applicationRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].RemovePassword(this.PasswordKeyId).Request((IEnumerable<Option>) null);
        context.RunSynchronously((Func<Task>) (() => applicationRequest.PostAsync(new CancellationToken())));
        return new MsGraphRemoveApplicationPasswordResponse()
        {
          Success = true
        };
      }
      catch (ServiceException ex) when (ex.IsBadRequest() || ex.IsResourceNotFoundError())
      {
        context.Trace(44750191, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationPasswordRequest), string.Format("Failed to remove application passwordf for Application '{0}' PasswordKey '{1}' bacause application or key does not exist, consider success. Exception: '{2}'.", (object) this.AppObjectId, (object) this.PasswordKeyId, (object) ex));
        return new MsGraphRemoveApplicationPasswordResponse()
        {
          Success = true
        };
      }
      finally
      {
        context.Trace(44750192, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationPasswordRequest), "Leaving Microsoft Graph API for Remove Application Password.");
      }
    }
  }
}
