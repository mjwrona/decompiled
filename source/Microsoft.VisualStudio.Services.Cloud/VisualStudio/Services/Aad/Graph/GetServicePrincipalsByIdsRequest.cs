// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetServicePrincipalsByIdsRequest : 
    MicrosoftGraphClientRequest<GetServicePrincipalsByIdsResponse>
  {
    private readonly IEnumerable<string> Types = (IEnumerable<string>) new List<string>()
    {
      "servicePrincipal"
    };
    private const string PropertiesToFetch = "id, appId, displayName, accountEnabled, servicePrincipalType";
    private const int MaxObjectIdsSupported = 1000;

    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override GetServicePrincipalsByIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      IDictionary<Guid, AadServicePrincipal> emptyMapWithIds = this.CreateEmptyMapWithIds<Guid, AadServicePrincipal>(this.ObjectIds);
      foreach (IList<Guid> source in emptyMapWithIds.Keys.Batch<Guid>(1000))
      {
        string[] objectIdsAsString = source.Select<Guid, string>((Func<Guid, string>) (_ => _.ToString())).ToArray<string>();
        IDirectoryObjectGetByIdsCollectionPage firstPage = context.RunSynchronously<IDirectoryObjectGetByIdsCollectionPage>((Func<Task<IDirectoryObjectGetByIdsCollectionPage>>) (() => graphServiceClient.DirectoryObjects.GetByIds((IEnumerable<string>) objectIdsAsString, this.Types).Request((IEnumerable<Option>) null).Select("id, appId, displayName, accountEnabled, servicePrincipalType").PostAsync(new CancellationToken())));
        foreach (DirectoryObject readAllPage in this.ReadAllPages<DirectoryObject, IDirectoryObjectGetByIdsCollectionPage>(context, graphServiceClient, firstPage))
        {
          Guid guid = AadGraphClient.CreateGuid(((Entity) readAllPage).Id);
          if (readAllPage is ServicePrincipal servicePrincipal && emptyMapWithIds.ContainsKey(guid))
            emptyMapWithIds[guid] = MicrosoftGraphConverters.ConvertServicePrincipal(servicePrincipal);
        }
      }
      return new GetServicePrincipalsByIdsResponse(emptyMapWithIds);
    }

    public override string ToString() => "GetServicePrincipalsByIdsRequest{ObjectIds=" + (this.ObjectIds == null ? "null" : this.ObjectIds.Serialize<IEnumerable<Guid>>()) + "}";
  }
}
