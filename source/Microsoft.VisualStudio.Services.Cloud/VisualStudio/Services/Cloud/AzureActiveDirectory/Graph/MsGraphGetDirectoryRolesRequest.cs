// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetDirectoryRolesRequest
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
  public class MsGraphGetDirectoryRolesRequest : 
    MicrosoftGraphClientRequest<MsGraphGetDirectoryRolesResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetDirectoryRolesRequest";

    internal override MsGraphGetDirectoryRolesResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750100, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRolesRequest), "Entering Microsoft Graph API for Get DirectoryRoles");
        IGraphServiceDirectoryRolesCollectionRequest directoryRolesRequest = graphServiceClient.DirectoryRoles.Request();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<AadDirectoryRole> aadDirectoryRoles = ((IEnumerable<DirectoryRole>) (context.RunSynchronously<IGraphServiceDirectoryRolesCollectionPage>((Func<Task<IGraphServiceDirectoryRolesCollectionPage>>) (() => directoryRolesRequest.GetAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Get DirectoryRoles call returned null"))).Select<DirectoryRole, AadDirectoryRole>(MsGraphGetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryRole ?? (MsGraphGetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryRole = new Func<DirectoryRole, AadDirectoryRole>(MicrosoftGraphConverters.ConvertDirectoryRole)));
        return new MsGraphGetDirectoryRolesResponse()
        {
          DirectoryRoles = aadDirectoryRoles
        };
      }
      catch (ServiceException ex)
      {
        context.TraceException(44750101, TraceLevel.Error, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRolesRequest), (Exception) ex);
        return new MsGraphGetDirectoryRolesResponse()
        {
          Exception = (Exception) ex
        };
      }
      finally
      {
        context.Trace(44750102, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRolesRequest), "Leaving Microsoft Graph API for Get DirectoryRoles");
      }
    }

    public override string ToString() => "GetDirectoryRolesRequest{}";
  }
}
