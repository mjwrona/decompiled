// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetUserThumbnailRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetUserThumbnailRequest : 
    MicrosoftGraphClientRequest<MsGraphGetUserThumbnailReponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetTenantRequest";

    public Guid UserObjectId { get; set; }

    public override string ToString() => string.Format("GetUserThumbnailRequest{{UserObjectId={0}}}", (object) this.UserObjectId);

    internal override MsGraphGetUserThumbnailReponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750060, TraceLevel.Info, "MicrosoftGraphClientRequest", "MsGraphGetTenantRequest", "Entering Microsoft Graph API for Get User Thumbnail");
        IProfilePhotoContentRequest userThumbnailRequest = graphServiceClient.Users[this.UserObjectId.ToString()].Photo.Content.Request((IEnumerable<Option>) null);
        Stream stream = context.RunSynchronously<Stream>((Func<Task<Stream>>) (() => userThumbnailRequest.GetAsync(new CancellationToken(), HttpCompletionOption.ResponseContentRead)));
        if (stream == null || stream.Length < 1L)
          throw new MicrosoftGraphException("Microsoft Graph API Get User Thumbnail call returned null or empty stream");
        using (MemoryStream destination = new MemoryStream())
        {
          stream.CopyTo((Stream) destination);
          return new MsGraphGetUserThumbnailReponse()
          {
            Thumbnail = destination.ToArray()
          };
        }
      }
      catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
      {
        return new MsGraphGetUserThumbnailReponse()
        {
          Thumbnail = (byte[]) null
        };
      }
      catch (ServiceException ex) when (ex.IsUnauthorizedError() || ex.IsRequestDeniedError())
      {
        context.TraceException(44750061, "MicrosoftGraphClientRequest", "MsGraphGetTenantRequest", (Exception) ex);
        return new MsGraphGetUserThumbnailReponse()
        {
          Thumbnail = (byte[]) null
        };
      }
      finally
      {
        context.Trace(44750062, TraceLevel.Info, "MicrosoftGraphClientRequest", "MsGraphGetTenantRequest", "Leaving Microsoft Graph API for Get User Thumbnail");
      }
    }
  }
}
