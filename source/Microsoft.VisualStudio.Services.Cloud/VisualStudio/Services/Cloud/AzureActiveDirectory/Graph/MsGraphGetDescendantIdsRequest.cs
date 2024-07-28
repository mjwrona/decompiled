// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetDescendantIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetDescendantIdsRequest : 
    MicrosoftGraphClientPagedRequest<MsGraphGetDescendantIdsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetDescendantIdsRequest";

    public Guid GroupId { get; set; }

    public override string ToString() => string.Format("GetDescendantIds{{GroupId={0}}}", (object) this.GroupId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.GroupId, "GroupId");

    internal override MsGraphGetDescendantIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      IGroupTransitiveMembersCollectionWithReferencesRequest membersRequest;
      if (this.PagingToken == null)
      {
        membersRequest = graphServiceClient.Groups[this.GroupId.ToString()].TransitiveMembers.Request();
        if (this.PageSize.HasValue)
          membersRequest = membersRequest.Top(this.PageSize.Value);
        context.TraceConditionally(44750080, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantIdsRequest), (Func<string>) (() => string.Format("Entering Microsoft Graph API for Get Transitive Members with GroupId = '{0}'", (object) this.GroupId)));
      }
      else
      {
        membersRequest = (IGroupTransitiveMembersCollectionWithReferencesRequest) new GroupTransitiveMembersCollectionWithReferencesRequest(this.GetSecureNextRequestUrlFromPageToken(graphServiceClient), (IBaseClient) graphServiceClient, (IEnumerable<Option>) null);
        context.TraceConditionally(44750081, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantIdsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Transitive Members with pagingToken = '" + MicrosoftGraphUtils.GetSkipTokenFromGraphRequest((IBaseRequest) membersRequest) + "'"));
      }
      try
      {
        IGroupTransitiveMembersCollectionWithReferencesPage source = context.RunSynchronously<IGroupTransitiveMembersCollectionWithReferencesPage>((Func<Task<IGroupTransitiveMembersCollectionWithReferencesPage>>) (() => membersRequest.GetAsync(new CancellationToken())));
        if (source == null)
          throw new MicrosoftGraphException("Microsoft Graph API Get Transitive Members call returned null");
        IEnumerable<Tuple<Guid, AadObjectType>> collection = ((IEnumerable<DirectoryObject>) source).Select<DirectoryObject, Tuple<Guid, AadObjectType>>((Func<DirectoryObject, Tuple<Guid, AadObjectType>>) (m => new Tuple<Guid, AadObjectType>(Guid.Parse(((Entity) m).Id), MicrosoftGraphConverters.GetDirectoryObjectAadObjectType(m)))).Where<Tuple<Guid, AadObjectType>>((Func<Tuple<Guid, AadObjectType>, bool>) (n => n.Item2.Equals((object) AadObjectType.User) || n.Item2.Equals((object) AadObjectType.Group) || n.Item2.Equals((object) AadObjectType.ServicePrincipal)));
        MsGraphGetDescendantIdsResponse descendantIdsResponse = new MsGraphGetDescendantIdsResponse();
        descendantIdsResponse.DescendantIdAndTypes = (ISet<Tuple<Guid, AadObjectType>>) new HashSet<Tuple<Guid, AadObjectType>>(collection);
        descendantIdsResponse.PagingToken = MicrosoftGraphUtils.GetGraphPageNextLink(context, (IBaseRequest) membersRequest, (IBaseRequest) source.NextPageRequest);
        return descendantIdsResponse;
      }
      finally
      {
        context.Trace(44750083, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantIdsRequest), "Leaving Microsoft Graph API for Get Transitive Members");
      }
    }
  }
}
