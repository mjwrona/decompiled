// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.GetConnectionsRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class GetConnectionsRequest : IOperationRequest
  {
    private ConnectionTypeEnum connectionType;
    private OperationScopeEnum operationScope;
    private IdentityTypeEnum identityType;
    private static IdentityProvider provider = (IdentityProvider) new DirectoryDiscoveryServiceAdapter();

    internal IList<string> ConnectionTypes { get; set; }

    internal IList<string> IdentityTypes { get; set; }

    internal IList<string> OperationScopes { get; set; }

    internal string ObjectId { get; set; }

    internal HashSet<string> RequestProperties { get; set; }

    internal string PagingToken { get; set; }

    internal int Depth { get; set; }

    internal GetConnectionsRequest()
    {
      this.identityType = IdentityTypeEnum.None;
      this.operationScope = OperationScopeEnum.None;
    }

    public void Validate(IVssRequestContext requestContext)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
      this.identityType = IdentityOperationHelper.ParseIdentityTypes(this.IdentityTypes);
      this.operationScope = IdentityOperationHelper.ParseOperationScopes(this.OperationScopes);
      this.connectionType = IdentityOperationHelper.ParseConnectionTypes(this.ConnectionTypes);
      this.RequestProperties = this.RequestProperties != null ? new HashSet<string>((IEnumerable<string>) this.RequestProperties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.PagingToken = string.IsNullOrWhiteSpace(this.PagingToken) ? string.Empty : this.PagingToken;
      this.Depth = this.Depth > 0 ? this.Depth : 1;
    }

    public IOperationResponse Process(IVssRequestContext requestContext)
    {
      Dictionary<string, IList<Identity>> dictionary1 = new Dictionary<string, IList<Identity>>();
      Dictionary<ConnectionTypeEnum, Tuple<string, int>> dictionary2 = new Dictionary<ConnectionTypeEnum, Tuple<string, int>>();
      if (this.connectionType.HasFlag((Enum) ConnectionTypeEnum.Successors))
        dictionary2[ConnectionTypeEnum.Successors] = new Tuple<string, int>("Member", 1);
      if (this.connectionType.HasFlag((Enum) ConnectionTypeEnum.Managers))
        dictionary2[ConnectionTypeEnum.Managers] = new Tuple<string, int>("Manager", this.Depth);
      if (this.connectionType.HasFlag((Enum) ConnectionTypeEnum.DirectReports))
        dictionary2[ConnectionTypeEnum.DirectReports] = new Tuple<string, int>("DirectReports", 1);
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      foreach (KeyValuePair<ConnectionTypeEnum, Tuple<string, int>> keyValuePair in dictionary2)
      {
        DirectoryDiscoveryService discoveryService = service;
        IVssRequestContext context = requestContext;
        DirectoryGetRelatedEntitiesRequest request = new DirectoryGetRelatedEntitiesRequest();
        request.Directories = (IEnumerable<string>) DirectoryDiscoveryServiceAdapter.GetDirectories(this.operationScope, requestContext);
        request.Depth = keyValuePair.Value.Item2;
        request.PropertiesToReturn = (IEnumerable<string>) this.RequestProperties;
        request.Relation = keyValuePair.Value.Item1;
        request.EntityIds = (IEnumerable<string>) new List<string>()
        {
          this.ObjectId
        };
        DirectoryGetRelatedEntitiesResponse relatedEntities = discoveryService.GetRelatedEntities(context, request);
        if (relatedEntities == null || relatedEntities.Results == null || relatedEntities.Results.Count == 0 || !relatedEntities.Results.ContainsKey(this.ObjectId))
          throw new IdentityPickerProcessException("Could not retrive connected identities from DDS");
        if (relatedEntities.Results[this.ObjectId].Exception != null)
          throw new IdentityPickerProcessException("Could not retrive connected identities from DDS", relatedEntities.Results[this.ObjectId].Exception);
        List<string> objectTypes = DirectoryDiscoveryServiceAdapter.GetDirectoryEntityTypes(this.identityType);
        dictionary1[keyValuePair.Key.ToString()] = (IList<Identity>) relatedEntities.Results[this.ObjectId].Entities.Where<IDirectoryEntity>((Func<IDirectoryEntity, bool>) (x => x is DirectoryEntity && objectTypes.Contains(x.EntityType))).Select<IDirectoryEntity, Identity>((Func<IDirectoryEntity, Identity>) (x => IdentityFactory.Create(x as DirectoryEntity, (IEnumerable<string>) this.RequestProperties, requestContext, (IDictionary<string, object>) null))).Where<Identity>((Func<Identity, bool>) (x => x != null)).ToList<Identity>();
      }
      return (IOperationResponse) new GetConnectionsResponse()
      {
        Connections = (IDictionary<string, IList<Identity>>) dictionary1
      };
    }
  }
}
