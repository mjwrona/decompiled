// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetDescendantsRequest
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
  public class MsGraphGetDescendantsRequest : 
    MicrosoftGraphClientPagedRequest<MsGraphGetDescendantsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetDescendantsRequest";
    private const string PropertiesToFetch = "id,displayName,mail,otherMails,mailNickname,userPrincipalName,jobTitle,department,officeLocation,surname,userType,externalUserState,onPremisesSecurityIdentifier,onPremisesImmutableId,businessPhones,country,usageLocation,accountEnabled,description,appId,servicePrincipalType";

    public Guid GroupId { get; set; }

    public override string ToString() => string.Format("GetDescendants{{GroupId={0}}}", (object) this.GroupId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.GroupId, "GroupId");

    internal override MsGraphGetDescendantsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      IGroupMembersCollectionWithReferencesRequest membersRequest;
      if (this.PagingToken == null)
      {
        membersRequest = graphServiceClient.Groups[this.GroupId.ToString()].Members.Request().Select("id,displayName,mail,otherMails,mailNickname,userPrincipalName,jobTitle,department,officeLocation,surname,userType,externalUserState,onPremisesSecurityIdentifier,onPremisesImmutableId,businessPhones,country,usageLocation,accountEnabled,description,appId,servicePrincipalType");
        int? pageSize = this.PageSize;
        if (pageSize.HasValue)
        {
          IGroupMembersCollectionWithReferencesRequest referencesRequest = membersRequest;
          pageSize = this.PageSize;
          int num = pageSize.Value;
          membersRequest = referencesRequest.Top(num);
        }
        context.TraceConditionally(44750090, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantsRequest), (Func<string>) (() => string.Format("Entering Microsoft Graph API for Get Direct Members with GroupId = '{0}'", (object) this.GroupId)));
      }
      else
      {
        membersRequest = (IGroupMembersCollectionWithReferencesRequest) new GroupMembersCollectionWithReferencesRequest(this.GetSecureNextRequestUrlFromPageToken(graphServiceClient), (IBaseClient) graphServiceClient, (IEnumerable<Option>) null);
        context.TraceConditionally(44750091, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Direct Members with pagingToken = '" + MicrosoftGraphUtils.GetSkipTokenFromGraphRequest((IBaseRequest) membersRequest) + "'"));
      }
      try
      {
        IGroupMembersCollectionWithReferencesPage source1 = context.RunSynchronously<IGroupMembersCollectionWithReferencesPage>((Func<Task<IGroupMembersCollectionWithReferencesPage>>) (() => membersRequest.GetAsync(new CancellationToken())));
        if (source1 == null)
          throw new MicrosoftGraphException("Microsoft Graph API Get Direct Members call returned null");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<AadObject> source2 = ((IEnumerable<DirectoryObject>) source1).Select<DirectoryObject, AadObject>(MsGraphGetDescendantsRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryObject ?? (MsGraphGetDescendantsRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryObject = new Func<DirectoryObject, AadObject>(MicrosoftGraphConverters.ConvertDirectoryObject))).Where<AadObject>((Func<AadObject, bool>) (x => x != null));
        MsGraphGetDescendantsResponse descendantsResponse = new MsGraphGetDescendantsResponse();
        descendantsResponse.Descendants = (IEnumerable<AadObject>) source2.ToList<AadObject>();
        descendantsResponse.PagingToken = MicrosoftGraphUtils.GetGraphPageNextLink(context, (IBaseRequest) membersRequest, (IBaseRequest) source1.NextPageRequest);
        return descendantsResponse;
      }
      finally
      {
        context.Trace(44750093, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDescendantsRequest), "Leaving Microsoft Graph API for Get Direct Members");
      }
    }
  }
}
