// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetUsersWithIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetUsersWithIdsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetUsersWithIdsResponse>
  {
    private static readonly IReadOnlyList<string> Types = (IReadOnlyList<string>) new List<string>()
    {
      "user"
    };
    private const string PropertiesToFetch = "id,displayName,mail,otherMails,mailNickname,userPrincipalName,jobTitle,department,manager,directReports,officeLocation,surname,userType,externalUserState,onPremisesSecurityIdentifier,onPremisesImmutableId,businessPhones,country,usageLocation,accountEnabled";
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetUsersWithIdsRequest";

    public IEnumerable<Guid> ObjectIds { get; set; }

    public string ExpandProperty { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override MsGraphGetUsersWithIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750070, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersWithIdsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get 'GetUsersWithIds' with parameters ='" + (this.ObjectIds.Serialize<IEnumerable<Guid>>() ?? "null") + "'"));
        string expandProperty = this.GetExpandProperty();
        IDirectoryObjectGetByIdsCollectionPage source;
        if (string.IsNullOrEmpty(expandProperty))
        {
          IDirectoryObjectGetByIdsRequest directoryObjectsUsersRequest = graphServiceClient.DirectoryObjects.GetByIds(this.ObjectIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())), (IEnumerable<string>) MsGraphGetUsersWithIdsRequest.Types).Request((IEnumerable<Option>) null).Select("id,displayName,mail,otherMails,mailNickname,userPrincipalName,jobTitle,department,manager,directReports,officeLocation,surname,userType,externalUserState,onPremisesSecurityIdentifier,onPremisesImmutableId,businessPhones,country,usageLocation,accountEnabled");
          source = context.RunSynchronously<IDirectoryObjectGetByIdsCollectionPage>((Func<Task<IDirectoryObjectGetByIdsCollectionPage>>) (() => directoryObjectsUsersRequest.PostAsync(new CancellationToken())));
        }
        else
        {
          source = (IDirectoryObjectGetByIdsCollectionPage) new DirectoryObjectGetByIdsCollectionPage();
          foreach (Guid objectId in this.ObjectIds)
          {
            IUserRequest directoryObjectsUsersRequest = graphServiceClient.Users[objectId.ToString()].Request().Expand(expandProperty).Select("id,displayName,mail,otherMails,mailNickname,userPrincipalName,jobTitle,department,manager,directReports,officeLocation,surname,userType,externalUserState,onPremisesSecurityIdentifier,onPremisesImmutableId,businessPhones,country,usageLocation,accountEnabled");
            ((ICollection<DirectoryObject>) source).Add((DirectoryObject) context.RunSynchronously<User>((Func<Task<User>>) (() => directoryObjectsUsersRequest.GetAsync(new CancellationToken()))));
          }
        }
        if (source == null)
          throw new MicrosoftGraphException("Microsoft Graph API 'GetUsersWithIds' call returned null");
        Dictionary<Guid, User> dictionary = ((IEnumerable) source).OfType<User>().ToDictionary<User, Guid, User>((Func<User, Guid>) (o => MicrosoftGraphConverters.CreateGuid(((Entity) o).Id)), (Func<User, User>) (o => o));
        return new MsGraphGetUsersWithIdsResponse()
        {
          Users = (IDictionary<Guid, AadUser>) this.ObjectIds.ToDictionary<Guid, Guid, AadUser>((Func<Guid, Guid>) (id => id), (Func<Guid, AadUser>) (id => MicrosoftGraphConverters.ConvertUser(AadServiceUtils.GetValueOrDefault<Guid, User>((IDictionary<Guid, User>) dictionary, id))))
        };
      }
      finally
      {
        context.Trace(44750071, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersWithIdsRequest), "Leaving Microsoft Graph API for 'GetUsersWithIds'");
      }
    }

    private string GetExpandProperty()
    {
      string expandProperty = string.Empty;
      switch (this.ExpandProperty)
      {
        case "Manager":
          expandProperty = "manager";
          break;
        case "DirectReports":
          expandProperty = "directReports";
          break;
      }
      return expandProperty;
    }

    public override string ToString()
    {
      IEnumerable<Guid> objectIds = this.ObjectIds;
      return "MsGraphGetUsersWithIdsRequest{ObjectIds=" + (objectIds != null ? objectIds.Serialize<IEnumerable<Guid>>() : (string) null) + "}";
    }
  }
}
