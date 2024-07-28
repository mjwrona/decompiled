// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetUserRolesAndGroupsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetUserRolesAndGroupsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetUserRolesAndGroupsResponse>
  {
    private const bool SecurityEnabledOnly = false;
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetUserRolesAndGroupsRequest";

    public Guid UserObjectId { get; set; }

    internal override MsGraphGetUserRolesAndGroupsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750120, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUserRolesAndGroupsRequest), (Func<string>) (() => string.Format("Calling Microsoft Graph API for Get 'GetUserRolesAndGroups' with parameter = '{0}'", (object) this.UserObjectId)));
        IDirectoryObjectGetMemberObjectsRequest userMemberObjectsRequest = ((IDirectoryObjectRequestBuilder) graphServiceClient.Users[this.UserObjectId.ToString()]).GetMemberObjects(new bool?(false)).Request((IEnumerable<Option>) null);
        HashSet<Guid> guidSet = new HashSet<Guid>(((IEnumerable<string>) (context.RunSynchronously<IDirectoryObjectGetMemberObjectsCollectionPage>((Func<Task<IDirectoryObjectGetMemberObjectsCollectionPage>>) (() => userMemberObjectsRequest.PostAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Get 'GetUserRolesAndGroups' call returned null"))).Select<string, Guid>((Func<string, Guid>) (s => MicrosoftGraphConverters.CreateGuid(s))));
        return new MsGraphGetUserRolesAndGroupsResponse()
        {
          Members = (ISet<Guid>) guidSet
        };
      }
      catch (ServiceException ex)
      {
        context.TraceException(44750121, TraceLevel.Error, "MicrosoftGraphClientRequest", nameof (MsGraphGetUserRolesAndGroupsRequest), (Exception) ex);
        return new MsGraphGetUserRolesAndGroupsResponse()
        {
          Exception = (Exception) ex
        };
      }
      finally
      {
        context.Trace(44750122, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUserRolesAndGroupsRequest), "Leaving Execute method for 'GetUserRolesAndGroups'");
      }
    }

    public override string ToString() => "MsGraphGetUserRolesAndGroupsRequest{UserObjectId=" + this.UserObjectId.ToString() + "}";
  }
}
