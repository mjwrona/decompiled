// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetGroupsWithIdsRequest
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
  public class MsGraphGetGroupsWithIdsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetGroupsWithIdsResponse>
  {
    private static readonly IReadOnlyList<string> Types = (IReadOnlyList<string>) new List<string>()
    {
      "group"
    };
    private const string PropertiesToFetch = "id,description,displayName,mailNickname,mail,onPremisesSecurityIdentifier";
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetGroupsWithIdsRequest";

    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override MsGraphGetGroupsWithIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750110, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetGroupsWithIdsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get 'GetGroupsWithIds' with parameters ='" + (this.ObjectIds.Serialize<IEnumerable<Guid>>() ?? "null") + "'"));
        IDirectoryObjectGetByIdsRequest directoryObjectsGroupsRequest = graphServiceClient.DirectoryObjects.GetByIds(this.ObjectIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())), (IEnumerable<string>) MsGraphGetGroupsWithIdsRequest.Types).Request((IEnumerable<Option>) null).Select("id,description,displayName,mailNickname,mail,onPremisesSecurityIdentifier");
        Dictionary<Guid, Group> dictionary = ((IEnumerable) (context.RunSynchronously<IDirectoryObjectGetByIdsCollectionPage>((Func<Task<IDirectoryObjectGetByIdsCollectionPage>>) (() => directoryObjectsGroupsRequest.PostAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API 'GetGroupsWithIds' call returned null"))).OfType<Group>().ToDictionary<Group, Guid, Group>((Func<Group, Guid>) (o => MicrosoftGraphConverters.CreateGuid(((Entity) o).Id)), (Func<Group, Group>) (o => o));
        return new MsGraphGetGroupsWithIdsResponse()
        {
          Groups = (IDictionary<Guid, AadGroup>) this.ObjectIds.ToDictionary<Guid, Guid, AadGroup>((Func<Guid, Guid>) (id => id), (Func<Guid, AadGroup>) (id => MicrosoftGraphConverters.ConvertGroup(AadServiceUtils.GetValueOrDefault<Guid, Group>((IDictionary<Guid, Group>) dictionary, id))))
        };
      }
      finally
      {
        context.Trace(44750111, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetGroupsWithIdsRequest), "Leaving Microsoft Graph API for 'GetGroupsWithIds'");
      }
    }

    public override string ToString()
    {
      IEnumerable<Guid> objectIds = this.ObjectIds;
      return "MsGraphGetGroupsWithIdsRequest{ObjectIds=" + (objectIds != null ? objectIds.Serialize<IEnumerable<Guid>>() : (string) null) + "}";
    }
  }
}
