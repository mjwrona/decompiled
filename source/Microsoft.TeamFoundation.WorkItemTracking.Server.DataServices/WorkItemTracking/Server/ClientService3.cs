// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService3", CollectionServiceIdentifier = "CA87FA49-58C9-4089-8535-1299FA60EEBC")]
  [ProxyParentClass("ClientService2", IgnoreInheritedMethods = true)]
  public class ClientService3 : ClientService2
  {
    public ClientService3()
      : base(3)
    {
    }

    public ClientService3(int clientVersion)
      : base(clientVersion)
    {
    }

    public override void GetMetadata(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity)
    {
      throw new NotImplementedException();
    }

    public override void GetMetadataEx(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid)
    {
      throw new NotImplementedException();
    }

    [WebMethod]
    [ClientIgnore]
    public virtual void GetWorkItemIdsForArtifactUris(
      string[] artifactUris,
      DateTime? asOfDate,
      out IEnumerableWrapper<ArtifactWorkItemIds> artifactLinks)
    {
      this.CheckAndBlockWitSoapAccess();
      TeamFoundationWorkItemService service = this.RequestContext.GetService<TeamFoundationWorkItemService>();
      IEnumerable<KeyValuePair<string, int>> keyValuePairs = ((IEnumerable<string>) artifactUris).Select<string, KeyValuePair<string, int>>((Func<string, int, KeyValuePair<string, int>>) ((uri, i) => new KeyValuePair<string, int>(uri, i))).Where<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (kvp => kvp.Key != null));
      IVssRequestContext requestContext = this.RequestContext;
      IEnumerable<string> artifactUris1 = keyValuePairs.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (kvp => kvp.Key));
      DateTime? asOfDate1 = asOfDate;
      Guid? filterUnderProjectId = new Guid?();
      IEnumerable<ArtifactUriQueryResult> idsForArtifactUris = service.GetWorkItemIdsForArtifactUris(requestContext, artifactUris1, asOfDate1, filterUnderProjectId);
      artifactLinks = new IEnumerableWrapper<ArtifactWorkItemIds>(idsForArtifactUris.Zip<ArtifactUriQueryResult, KeyValuePair<string, int>, ArtifactWorkItemIds>(keyValuePairs, (Func<ArtifactUriQueryResult, KeyValuePair<string, int>, ArtifactWorkItemIds>) ((artifact, kvp) => new ArtifactWorkItemIds()
      {
        WorkItemIds = artifact.WorkItemIds.ToList<int>(),
        Uri = artifact.ArtifactUri,
        UriListOffset = kvp.Value
      })).Where<ArtifactWorkItemIds>((Func<ArtifactWorkItemIds, bool>) (aqr => aqr.WorkItemIds.Count > 0)));
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual IEnumerableWrapper<WorkItemId> GetDestroyedWorkItemIds(long rowVersion)
    {
      this.CheckAndBlockWitSoapAccess();
      IEnumerable<WorkItemId> workItemIds = (IEnumerable<WorkItemId>) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetDestroyedWorkItemIds), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (rowVersion), (object) rowVersion);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService3), 900375, 900640, AccessIntent.Read, (Action) (() => workItemIds = this.RequestContext.GetService<TeamFoundationWorkItemService>().GetDestroyedWorkItemIds(this.RequestContext, rowVersion).Select<DestroyedWorkItemQueryResult, WorkItemId>((Func<DestroyedWorkItemQueryResult, WorkItemId>) (qr => new WorkItemId()
      {
        Id = qr.WorkItemId,
        RowVersion = qr.RowVersion
      }))));
      return new IEnumerableWrapper<WorkItemId>(workItemIds);
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual StreamingCollection<WorkItemId> GetChangedWorkItemIds(long rowVersion)
    {
      this.CheckAndBlockWitSoapAccess();
      StreamingCollection<WorkItemId> workItemIds = (StreamingCollection<WorkItemId>) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetChangedWorkItemIds), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (rowVersion), (object) rowVersion);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService3), 900376, 900641, AccessIntent.Read, (Action) (() => workItemIds = this.GetWorkItemIds(rowVersion, false)));
      return workItemIds;
    }

    private StreamingCollection<WorkItemId> GetWorkItemIds(long rowVersion, bool fDestroyed)
    {
      this.RequestContext.GetExtension<IAuthorizationProviderFactory>().EnsurePermitted(this.RequestContext, PermissionNamespaces.Global, "SYNCHRONIZE_READ");
      DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(this.RequestContext);
      TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
      try
      {
        resource = dataAccessLayerImpl.GetWorkItemIds(this.RequestContext, rowVersion, fDestroyed);
        this.AddWebServiceResource((IDisposable) resource);
      }
      catch (Exception ex)
      {
        resource?.Dispose();
        throw;
      }
      return resource.Current<StreamingCollection<WorkItemId>>();
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual StreamingCollection<WorkItemLinkChange> GetWorkItemLinkChanges(long rowVersion)
    {
      this.CheckAndBlockWitSoapAccess();
      StreamingCollection<WorkItemLinkChange> workItemLinkChanges = (StreamingCollection<WorkItemLinkChange>) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetWorkItemLinkChanges), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (rowVersion), (object) rowVersion);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService3), 900377, 900642, AccessIntent.Read, (Action) (() =>
      {
        DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(this.RequestContext);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = dataAccessLayerImpl.GetWorkItemLinkChanges(this.RequestContext, rowVersion);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        workItemLinkChanges = resource.Current<StreamingCollection<WorkItemLinkChange>>();
      }));
      return workItemLinkChanges;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public ExtendedAccessControlListData GetStoredQueryItemAccessControlList(
      Guid queryItemId,
      bool getMetadata)
    {
      this.CheckAndBlockWitSoapAccess();
      MethodInformation methodInformation = new MethodInformation(nameof (GetStoredQueryItemAccessControlList), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (queryItemId), (object) queryItemId);
      methodInformation.AddParameter(nameof (getMetadata), (object) getMetadata);
      ExtendedAccessControlListData extendedAclData = new ExtendedAccessControlListData();
      extendedAclData.IsEditable = false;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService3), 900378, 900379, AccessIntent.Read, (Action) (() =>
      {
        if (queryItemId.Equals(Guid.Empty) & getMetadata)
        {
          extendedAclData.Metadata = QueryItemMethods.GetAclMetadata(this.RequestContext);
          extendedAclData.IsEditable = false;
        }
        else
          extendedAclData = new DataAccessLayerImpl(this.RequestContext).GetQueryAccessControlList(queryItemId, getMetadata);
      }));
      return extendedAclData;
    }
  }
}
