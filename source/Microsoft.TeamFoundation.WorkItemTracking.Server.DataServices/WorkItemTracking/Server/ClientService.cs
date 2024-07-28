// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.TestManagement;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService", CollectionServiceIdentifier = "179b6a0b-a5be-43fc-879f-cfa2a43cd3d8")]
  public class ClientService : WorkItemTrackingWebService
  {
    private TestCaseParameterDataHelper m_testCaseParameterDataHelper;

    public ClientService()
      : base(1)
    {
    }

    public ClientService(int clientVersion)
      : base(clientVersion)
    {
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual string GetWorkitemTrackingVersion()
    {
      string version = string.Empty;
      this.ExecuteWebMethod(nameof (GetWorkitemTrackingVersion), MethodType.Normal, EstimatedMethodCost.VeryLow, nameof (ClientService), 900347, 900619, AccessIntent.NotSpecified, (Action) (() =>
      {
        version = Assembly.GetCallingAssembly().GetName().Version.ToString();
        this.RequestContext.Trace(900370, TraceLevel.Info, "WebServices", nameof (ClientService), "Workitem Tracking DataServices Assembly Version: {0}", (object) version);
      }));
      return version;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void StampWorkitemCache()
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      this.ExecuteWebMethod(nameof (StampWorkitemCache), MethodType.ReadWrite, EstimatedMethodCost.Low, nameof (ClientService), 900348, 900620, AccessIntent.Write, (Action) (() => new DataAccessLayerImpl(this.RequestContext).StampDb((IVssIdentity) this.RequestContext.GetUserIdentity())));
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual string[] GetReferencingWorkitemUris(string artifactUri)
    {
      this.CheckAndBlockWitSoapAccess();
      string[] referencedUris = (string[]) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetReferencingWorkitemUris), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (artifactUri), (object) artifactUri);
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900349, 900621, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<string>(artifactUri, nameof (artifactUri));
        dal = new DataAccessLayerImpl(this.RequestContext);
        referencedUris = dal.GetReferencingWorkitemUris(user, artifactUri);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, user, e)));
      return referencedUris;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetWorkItem(
      int workItemId,
      int revisionId,
      int minimumRevisionId,
      DateTime? asOfDate,
      bool useMaster,
      out Payload workItem,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload workItemPayload = workItem = (Payload) null;
      Payload metadataTemp = metadata = (Payload) null;
      string dbStampTemp = dbStamp = string.Empty;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetWorkItem), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (workItemId), (object) workItemId);
      methodInformation.AddParameter(nameof (revisionId), (object) revisionId);
      methodInformation.AddParameter(nameof (asOfDate), (object) asOfDate);
      methodInformation.AddParameter(nameof (minimumRevisionId), (object) minimumRevisionId);
      methodInformation.AddParameter(nameof (dbStamp), (object) dbStamp);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      MetadataTable[] tablesRequested = (MetadataTable[]) null;
      long[] rowVersions = (long[]) null;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900350, 900622, AccessIntent.Read, (Action) (() =>
      {
        if (workItemId <= 0)
          throw new ArgumentException(nameof (workItemId));
        if (revisionId < 0)
          throw new ArgumentException(nameof (revisionId));
        this.ConvertMetadataHave(metadataHave, out tablesRequested, out rowVersions, out metadataTemp);
        workItemPayload = this.GetCompatibilityPayload();
        DateTime? asOfDate1 = new DateTime?();
        if (asOfDate.HasValue && asOfDate.Value != DateTime.MinValue)
          asOfDate1 = new DateTime?(asOfDate.Value.ToUniversalTime());
        WorkItemTrackingRequestContext witRequestContext = this.RequestContext.WitContext();
        TeamFoundationWorkItemService service = this.RequestContext.GetService<TeamFoundationWorkItemService>();
        try
        {
          WorkItem workItemById = service.GetWorkItemById(this.RequestContext, workItemId, true, true, true, WorkItemRetrievalMode.NonDeleted, true, false, new Guid?(), false, new DateTime?());
          PayloadCompatibilityUtils.FillPayloadWithWorkItem(witRequestContext, workItemById, workItemPayload, minimumRevisionId, revisionId, asOfDate1);
        }
        catch (WorkItemUnauthorizedAccessException ex)
        {
          PayloadCompatibilityUtils.FillPayloadWithWorkItemNotFoundOrNoAccess(workItemPayload);
        }
        this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Fetching Metadata to use with NewAPI results only.", (object) this.ClientVersion);
        if (tablesRequested == null || tablesRequested.Length == 0)
          return;
        tablesRequested = tablesRequested ?? Array.Empty<MetadataTable>();
        rowVersions = rowVersions ?? Array.Empty<long>();
        dal = new DataAccessLayerImpl(this.RequestContext);
        int locale = 0;
        string callerIdentity = (string) null;
        this.RequestContext.PartialResultsReady();
        dal.GetMetadata((IVssIdentity) this.RequestContext.GetUserIdentity(), tablesRequested, rowVersions, metadataTemp, out locale, out int _, out callerIdentity, out dbStampTemp, out int _);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, (IVssIdentity) this.RequestContext.GetUserIdentity(), e)));
      workItem = workItemPayload;
      metadata = metadataTemp;
      dbStamp = dbStampTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void QueryWorkitems(
      XmlElement psQuery,
      QuerySortOrderEntry[] sort,
      bool useMaster,
      out XmlElement resultIds,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess();
      XmlElement resultIdsTemp = resultIds = (XmlElement) null;
      ref DateTime local = ref asOfDate;
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2;
      DateTime dateTime3 = dateTime2 = dateTime1;
      local = dateTime2;
      DateTime asOfDateTemp = dateTime3;
      string dbStampTemp = dbStamp = string.Empty;
      Payload metadataTemp = metadata = (Payload) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkitems), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
      if (psQuery != null)
        methodInformation.AddParameter(nameof (psQuery), (object) psQuery.InnerXml);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      MetadataTable[] tablesRequested = (MetadataTable[]) null;
      long[] rowVersions = (long[]) null;
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900351, 900623, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<XmlElement>(psQuery, nameof (psQuery));
        if (psQuery == null)
          throw new SoapException(ResourceStrings.Get("NoQueryXml"), Soap12FaultCodes.SenderFaultCode, new SoapFaultSubCode(Soap12FaultCodes.RpcBadArgumentsFaultCode, WorkItemTrackingFaultCodes.NoQueryXml));
        this.ConvertMetadataHave(metadataHave, out tablesRequested, out rowVersions, out metadataTemp);
        resultIdsTemp = this.QueryWorkItemsNewAPI(psQuery, sort, false, out int _, out asOfDateTemp, user, tablesRequested, rowVersions, metadataTemp, out dbStampTemp);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, user, e)));
      resultIds = resultIdsTemp;
      asOfDate = asOfDateTemp;
      dbStamp = dbStampTemp;
      metadata = metadataTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void PageWorkitemsByIds(
      int[] ids,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      bool useMaster,
      out Payload items,
      MetadataTableHaveEntry[] metadataHave,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload itemsTemp = items = (Payload) null;
      Payload metadataTemp = metadata = (Payload) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (PageWorkitemsByIds), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
      methodInformation.AddArrayParameter<string>(nameof (columns), (IList<string>) columns);
      methodInformation.AddArrayParameter<int>(nameof (longTextColumns), (IList<int>) longTextColumns);
      methodInformation.AddParameter(nameof (asOfDate), (object) asOfDate);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      MetadataTable[] tablesRequested = (MetadataTable[]) null;
      long[] rowVersions = (long[]) null;
      DateTime asOfDateDal = DateTime.MinValue;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900352, 900624, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
        if (((columns == null ? 1 : (columns.Length == 0 ? 1 : 0)) & (longTextColumns == null ? (true ? 1 : 0) : (longTextColumns.Length == 0 ? 1 : 0))) != 0)
          throw new ArgumentNullException(nameof (columns));
        this.ConvertMetadataHave(metadataHave, out tablesRequested, out rowVersions, out metadataTemp);
        dal = new DataAccessLayerImpl(this.RequestContext);
        if (asOfDate.HasValue && asOfDate.Value != DateTime.MinValue)
          asOfDateDal = asOfDate.Value.ToUniversalTime();
        itemsTemp = this.GetCompatibilityPayload();
        WorkItemTrackingRequestContext witRequestContext = this.RequestContext.WitContext();
        int workItemPageSize = witRequestContext.ServerSettings.MaxWorkItemPageSize;
        if (ids.Length > workItemPageSize)
          throw new WorkItemPageSizeExceededException(ids.Length, workItemPageSize);
        TeamFoundationWorkItemService service = this.RequestContext.GetService<TeamFoundationWorkItemService>();
        ColumnLongTextFieldEntryCollection fieldEntryCollection = new ColumnLongTextFieldEntryCollection(witRequestContext, (IEnumerable<string>) columns, (IEnumerable<int>) longTextColumns, new int[2]
        {
          3,
          8
        });
        DateTime? asOf = !(asOfDateDal != DateTime.MinValue) ? new DateTime?() : new DateTime?(asOfDateDal);
        List<WorkItemFieldData> list = service.GetWorkItemFieldValues(this.RequestContext, (IEnumerable<int>) ids, fieldEntryCollection.ExistingFieldIds, 16, asOf, 200, WorkItemRetrievalMode.NonDeleted, false, false).ToList<WorkItemFieldData>();
        PayloadCompatibilityUtils.FillPayloadWithWorkItemPageData(witRequestContext, (IReadOnlyCollection<WorkItemFieldData>) list, itemsTemp, fieldEntryCollection.ColumnFields.ToArray<FieldEntry>(), fieldEntryCollection.LongTextFields.ToArray<FieldEntry>());
        if (tablesRequested == null)
          return;
        this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Fetching Metadata to use with NewAPI results only.", (object) this.ClientVersion);
        int locale = 0;
        string callerIdentity = (string) null;
        this.RequestContext.PartialResultsReady();
        dal.GetMetadata((IVssIdentity) this.RequestContext.GetUserIdentity(), tablesRequested, rowVersions, metadataTemp, out locale, out int _, out callerIdentity, out string _, out int _);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, (IVssIdentity) this.RequestContext.GetUserIdentity(), e)));
      items = itemsTemp;
      metadata = metadataTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void PageWorkitemsByIdRevs(
      IdRevisionPair[] pairs,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      out DateTime pageDate,
      bool useMaster,
      out Payload items)
    {
      this.CheckAndBlockWitSoapAccess();
      ref DateTime local = ref pageDate;
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2;
      DateTime dateTime3 = dateTime2 = dateTime1;
      local = dateTime2;
      DateTime dateTime4 = dateTime3;
      Payload itemsTemp = items = (Payload) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (PageWorkitemsByIdRevs), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (columns), (IList<string>) columns);
      methodInformation.AddArrayParameter<int>(nameof (longTextColumns), (IList<int>) longTextColumns);
      methodInformation.AddParameter(nameof (asOfDate), (object) asOfDate);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900353, 900625, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) pairs, nameof (pairs));
        if (((columns == null ? 1 : (columns.Length == 0 ? 1 : 0)) & (longTextColumns == null ? (true ? 1 : 0) : (longTextColumns.Length == 0 ? 1 : 0))) != 0)
          throw new ArgumentNullException(nameof (columns));
        dal = new DataAccessLayerImpl(this.RequestContext);
        itemsTemp = this.GetCompatibilityPayload();
        WorkItemTrackingRequestContext witRequestContext = this.RequestContext.WitContext();
        TeamFoundationWorkItemService service = this.RequestContext.GetService<TeamFoundationWorkItemService>();
        ColumnLongTextFieldEntryCollection fieldEntryCollection = new ColumnLongTextFieldEntryCollection(witRequestContext, (IEnumerable<string>) columns, (IEnumerable<int>) longTextColumns, new int[2]
        {
          3,
          8
        });
        List<WorkItemFieldData> list = service.GetWorkItemFieldValues(this.RequestContext, ((IEnumerable<IdRevisionPair>) pairs).Select<IdRevisionPair, WorkItemIdRevisionPair>((Func<IdRevisionPair, WorkItemIdRevisionPair>) (p => new WorkItemIdRevisionPair()
        {
          Id = p.Id,
          Revision = p.Revision
        })), fieldEntryCollection.ExistingFieldIds).ToList<WorkItemFieldData>();
        PayloadCompatibilityUtils.FillPayloadWithWorkItemPageData(witRequestContext, (IReadOnlyCollection<WorkItemFieldData>) list, itemsTemp, fieldEntryCollection.ColumnFields.ToArray<FieldEntry>(), fieldEntryCollection.LongTextFields.ToArray<FieldEntry>());
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, (IVssIdentity) this.RequestContext.GetUserIdentity(), e)));
      pageDate = dateTime4;
      items = itemsTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void PageItemsOnBehalfOf(
      string userName,
      int[] ids,
      string[] columns,
      out Payload items)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload itemsTemp = items = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (PageItemsOnBehalfOf), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (userName), (object) userName);
      methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
      methodInformation.AddArrayParameter<string>(nameof (columns), (IList<string>) columns);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900354, 900626, AccessIntent.Read, (Action) (() =>
      {
        if (userName == null)
          throw new ArgumentNullException(nameof (userName));
        if (userName.Trim().Length <= 0)
          throw new ArgumentException(nameof (userName));
        IdentityService service1 = this.RequestContext.GetService<IdentityService>();
        if (!service1.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service1.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
          throw new SoapException(ResourceStrings.Get("UserNotInServiceGroup"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.CallerNotServiceAccount);
        userIdentity = this.ReadIdentity(this.RequestContext, IdentitySearchFilter.AccountName, userName, QueryMembership.None, (IEnumerable<string>) null);
        if (userIdentity == null)
        {
          this.RequestContext.Trace(900594, TraceLevel.Verbose, "WebServices", nameof (ClientService), "Identity with name {0} could not be found.", (object) userName);
          throw new IdentityNotFoundException();
        }
        if (ids == null || ids.Length == 0)
          throw new ArgumentNullException(nameof (ids));
        if (columns == null || columns.Length == 0)
          throw new ArgumentNullException(nameof (columns));
        itemsTemp = this.GetCompatibilityPayload();
        TeamFoundationWorkItemService service2 = this.RequestContext.GetService<TeamFoundationWorkItemService>();
        using (IVssRequestContext userContext = this.RequestContext.CreateUserContext(userIdentity.Descriptor))
        {
          WorkItemTrackingRequestContext witRequestContext = userContext.WitContext();
          ColumnLongTextFieldEntryCollection fieldEntryCollection = new ColumnLongTextFieldEntryCollection(witRequestContext, (IEnumerable<string>) columns, (IEnumerable<int>) null, new int[2]
          {
            3,
            8
          });
          IEnumerable<WorkItemFieldData> workItemFieldValues = service2.GetWorkItemFieldValues(userContext, (IEnumerable<int>) ids, fieldEntryCollection.ExistingFieldIds, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false);
          PayloadCompatibilityUtils.FillPayloadWithWorkItemPageData(witRequestContext, (IReadOnlyCollection<WorkItemFieldData>) workItemFieldValues.ToList<WorkItemFieldData>(), itemsTemp, fieldEntryCollection.ColumnFields.ToArray<FieldEntry>(), (FieldEntry[]) null);
        }
      }));
      items = itemsTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void QueryWorkitemCount(
      XmlElement psQuery,
      bool useMaster,
      out int count,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess();
      int countTemp = count = 0;
      ref DateTime local = ref asOfDate;
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2;
      DateTime dateTime3 = dateTime2 = dateTime1;
      local = dateTime2;
      DateTime asOfDateTemp = dateTime3;
      Payload metadataTemp = metadata = (Payload) null;
      string dbStampTemp = dbStamp = string.Empty;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkitemCount), MethodType.Normal, EstimatedMethodCost.Low);
      if (psQuery != null)
        methodInformation.AddParameter(nameof (psQuery), (object) psQuery.InnerXml);
      methodInformation.AddParameter(nameof (asOfDate), (object) asOfDate);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      MetadataTable[] tablesRequested = (MetadataTable[]) null;
      long[] rowVersions = (long[]) null;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900355, 900627, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<XmlElement>(psQuery, nameof (psQuery));
        this.ConvertMetadataHave(metadataHave, out tablesRequested, out rowVersions, out metadataTemp);
        this.QueryWorkItemsNewAPI(psQuery, (QuerySortOrderEntry[]) null, true, out countTemp, out asOfDateTemp, user, tablesRequested, rowVersions, metadataTemp, out dbStampTemp);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(dal, this.RequestContext, (XmlElement) null, user, e)));
      count = countTemp;
      asOfDate = asOfDateTemp;
      metadata = metadataTemp;
      dbStamp = dbStampTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void QueryWorkitemCountOnBehalfOf(
      string userName,
      XmlElement query,
      out int count)
    {
      this.CheckAndBlockWitSoapAccess();
      int countTemp = count = 0;
      MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkitemCountOnBehalfOf), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (userName), (object) userName);
      if (query != null)
        methodInformation.AddParameter(nameof (query), (object) query.InnerXml);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      string dbStamp = string.Empty;
      DateTime asOfDate = new DateTime();
      IVssIdentity onBehalfOfUser = (IVssIdentity) null;
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900356, 900628, AccessIntent.Read, (Action) (() =>
      {
        if (userName == null)
          throw new ArgumentNullException(nameof (userName));
        if (userName.Trim().Length == 0)
          throw new ArgumentException(nameof (userName));
        IdentityService service = this.RequestContext.GetService<IdentityService>();
        if (!service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.RequestContext.UserContext) && !service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
          throw new SoapException(ResourceStrings.Get("UserNotInServiceGroup"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.CallerNotServiceAccount);
        userIdentity = this.ReadIdentity(this.RequestContext, IdentitySearchFilter.AccountName, userName, QueryMembership.None, (IEnumerable<string>) null);
        if (userIdentity == null)
        {
          this.RequestContext.Trace(900595, TraceLevel.Verbose, "WebServices", nameof (ClientService), "Identity with name {0} could not be found.", (object) userName);
          throw new IdentityNotFoundException();
        }
        onBehalfOfUser = (IVssIdentity) userIdentity;
        this.QueryWorkItemsNewAPI(query, (QuerySortOrderEntry[]) null, true, out countTemp, out asOfDate, onBehalfOfUser, (MetadataTable[]) null, (long[]) null, (Payload) null, out dbStamp);
      }));
      count = countTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetMetadata(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity)
    {
      this.RequestContext.PartialResultsReady();
      this.GetMetadataInternal(metadataHave, out metadata, out dbStamp, out locale, out comparisonStyle, out callerIdentity, out string _, out int _);
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetMetadataEx(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid)
    {
      this.RequestContext.PartialResultsReady();
      this.GetMetadataInternal(metadataHave, out metadata, out dbStamp, out locale, out comparisonStyle, out callerIdentity, out callerIdentitySid, out int _);
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetMetadataEx2(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out int mode)
    {
      this.RequestContext.PartialResultsReady();
      this.GetMetadataInternal(metadataHave, out metadata, out dbStamp, out locale, out comparisonStyle, out string _, out string _, out mode);
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void Update(
      XmlElement package,
      out XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess(package);
      XmlElement resultTemp = result = (XmlElement) null;
      string dbStampTemp = dbStamp = string.Empty;
      Payload metadataTemp = metadata = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (Update), MethodType.ReadWrite, EstimatedMethodCost.Low);
      if (package != null)
        methodInformation.AddParameter(nameof (package), (object) package.InnerXml);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900357, 900629, AccessIntent.Write, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<XmlElement>(package, nameof (package));
        this.UpdateNewApi(package, ref resultTemp, metadataHave, ref dbStampTemp, ref metadataTemp);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, package, user, e)));
      result = resultTemp;
      dbStamp = dbStampTemp;
      metadata = metadataTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual bool BulkUpdate(
      XmlElement package,
      out XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata)
    {
      this.CheckAndBlockWitSoapAccess(package);
      XmlElement resultTemp = result = (XmlElement) null;
      string dbStampTemp = dbStamp = string.Empty;
      Payload metadataTemp = metadata = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (BulkUpdate), MethodType.ReadWrite, EstimatedMethodCost.Low);
      if (package != null)
        methodInformation.AddParameter(nameof (package), (object) package.InnerXml);
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900358, 900630, AccessIntent.Write, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<XmlElement>(package, nameof (package));
        this.UpdateNewApi(package, ref resultTemp, metadataHave, ref dbStampTemp, ref metadataTemp, true);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, package, user, e)));
      result = resultTemp;
      dbStamp = dbStampTemp;
      metadata = metadataTemp;
      return true;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetStoredQuery(Guid queryId, out Payload queryPayload)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload queryPayloadTemp = queryPayload = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetStoredQuery), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (queryId), (object) queryId);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900359, 900631, AccessIntent.Read, (Action) (() =>
      {
        queryPayloadTemp = new Payload();
        ITeamFoundationQueryItemService service = this.RequestContext.GetService<ITeamFoundationQueryItemService>();
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryById = service.GetQueryById(this.RequestContext, queryId, new int?(0), true);
        service.StripOutCurrentIterationTeamParameter(this.RequestContext, queryById);
        PayloadCompatibilityUtils.FillPayloadWithQueryItem(this.RequestContext, queryPayloadTemp, queryById);
      }));
      queryPayload = queryPayloadTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetStoredQueries(
      long rowVersion,
      int projectId,
      out Payload queriesPayload)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload queriesPayloadTemp = queriesPayload = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetStoredQueries), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (rowVersion), (object) rowVersion);
      methodInformation.AddParameter(nameof (projectId), (object) projectId);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900360, 900632, AccessIntent.Read, (Action) (() =>
      {
        if (projectId < 0)
          throw new ArgumentException(nameof (projectId));
        if (rowVersion < 0L)
          throw new ArgumentException(nameof (rowVersion));
        queriesPayloadTemp = new Payload();
        this.GetQueries(projectId, false, queriesPayloadTemp);
      }));
      queriesPayload = queriesPayloadTemp;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void SyncExternalStructures(string projectURI)
    {
      this.CheckAndBlockWitSoapAccess();
      MethodInformation methodInformation = new MethodInformation(nameof (SyncExternalStructures), MethodType.Tool, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (projectURI), (object) projectURI);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900362, 900633, AccessIntent.Write, (Action) (() =>
      {
        if (projectURI == null)
          throw new ArgumentNullException("ProjectURI");
        if (!this.HasAdminRights(this.RequestContext.UserContext, projectURI))
          return;
        int num = (int) this.RequestContext.GetService<WorkItemTrackingTreeService>().ReclassifyWorkItems(this.RequestContext);
      }));
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void SyncAccessControlLists(string projectURI)
    {
      this.CheckAndBlockWitSoapAccess();
      MethodInformation methodInformation = new MethodInformation(nameof (SyncAccessControlLists), MethodType.Tool, EstimatedMethodCost.VeryLow);
      methodInformation.AddParameter(nameof (projectURI), (object) projectURI);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900363, 900634, AccessIntent.NotSpecified, (Action) (() =>
      {
        throw new NotImplementedException();
      }));
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void SyncBisGroupsAndUsers(string projectUri)
    {
      this.CheckAndBlockWitSoapAccess();
      MethodInformation methodInformation = new MethodInformation(nameof (SyncBisGroupsAndUsers), MethodType.Tool, EstimatedMethodCost.Low);
      methodInformation.AddParameter("projectURI", (object) projectUri);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900364, 900635, AccessIntent.Write, (Action) (() =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
        if (!this.HasAdminRights(this.RequestContext.UserContext, projectUri))
          return;
        new DataAccessLayerImpl(this.RequestContext).SyncBisGroupsAndUsers(projectUri);
      }));
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void RequestCancel(string requestIdToCancel)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (RequestCancel), MethodType.Admin, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (requestIdToCancel), (object) requestIdToCancel);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900366, 900636, AccessIntent.NotSpecified, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<string>(requestIdToCancel, nameof (requestIdToCancel));
        IVssRequestContext context = RequestCancelableScope.GetContext(requestIdToCancel);
        if (context == null || !VssStringComparer.UserName.Equals(this.RequestContext.DomainUserName, context.DomainUserName))
          return;
        if (context.Items.ContainsKey("RequestNotCancelable"))
          throw new LegacyValidationException(ResourceStrings.Get("RequestNotCancellable"), 602005);
        context.Cancel((string) null);
      }));
    }

    private void _GetMetadataInternal(
      MetadataTableHaveEntry[] metadataHave,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid,
      out int mode)
    {
      if (metadataHave == null || metadataHave.Length == 0)
        throw new SoapException(ResourceStrings.Get("NoMetadataTablesRequested"), Soap12FaultCodes.SenderFaultCode, new SoapFaultSubCode(Soap12FaultCodes.RpcBadArgumentsFaultCode, WorkItemTrackingFaultCodes.NoMetadataTablesSpecified));
      callerIdentitySid = this.RequestContext.UserContext.Identifier;
      MetadataTable[] tablesRequested1 = (MetadataTable[]) null;
      long[] rowVersions1 = (long[]) null;
      this.ConvertMetadataHave(metadataHave, out tablesRequested1, out rowVersions1, out metadata);
      DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(this.RequestContext);
      this.RequestContext.PartialResultsReady();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.RequestContext.GetUserIdentity();
      MetadataTable[] tablesRequested2 = tablesRequested1;
      long[] rowVersions2 = rowVersions1;
      Payload metadataPayload = metadata;
      ref int local1 = ref locale;
      ref int local2 = ref comparisonStyle;
      ref string local3 = ref callerIdentity;
      ref string local4 = ref dbStamp;
      ref int local5 = ref mode;
      dataAccessLayerImpl.GetMetadata((IVssIdentity) userIdentity, tablesRequested2, rowVersions2, metadataPayload, out local1, out local2, out local3, out local4, out local5);
    }

    protected void GetMetadataInternal(
      MetadataTableHaveEntry[] metadataHave,
      out Payload metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid,
      out int mode)
    {
      Payload metadataTemp = metadata = (Payload) null;
      string dbStampTemp = dbStamp = string.Empty;
      int localeTemp = locale = 0;
      int comparisonStyleTemp = comparisonStyle = 0;
      string callerIdentityTemp = callerIdentity = string.Empty;
      string callerIdentitySidTemp = callerIdentitySid = string.Empty;
      int modeTemp = mode = 0;
      MethodInformation methodInformation = new MethodInformation(this.GetMetadataUpdateRequestLevel(metadataHave), MethodType.Normal, EstimatedMethodCost.Low, this.GetMetadataTimeout());
      if (metadataHave != null)
      {
        for (int index = 0; index < metadataHave.Length; ++index)
          methodInformation.AddParameter(metadataHave[index].TableName, (object) metadataHave[index].RowVersion);
      }
      this.ExecuteWebMethod(methodInformation, nameof (ClientService), 900367, 900637, AccessIntent.Read, (Action) (() => this._GetMetadataInternal(metadataHave, out metadataTemp, out dbStampTemp, out localeTemp, out comparisonStyleTemp, out callerIdentityTemp, out callerIdentitySidTemp, out modeTemp)));
      metadata = metadataTemp;
      dbStamp = dbStampTemp;
      locale = localeTemp;
      comparisonStyle = comparisonStyleTemp;
      callerIdentity = callerIdentityTemp;
      callerIdentitySid = callerIdentitySidTemp;
      mode = modeTemp;
    }

    private string GetMetadataUpdateRequestLevel(MetadataTableHaveEntry[] metadataHave) => metadataHave == null || ((IEnumerable<MetadataTableHaveEntry>) metadataHave).All<MetadataTableHaveEntry>((Func<MetadataTableHaveEntry, bool>) (md => md.RowVersion == 0L)) ? "GetMetadata_Full" : "GetMetadata_Incremental";

    private TimeSpan GetMetadataTimeout() => TimeSpan.FromSeconds((double) this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).GetMetadataSoapTimeoutInSeconds);

    protected void ConvertMetadataHave(
      MetadataTableHaveEntry[] metadataHave,
      out MetadataTable[] tablesRequested,
      out long[] rowVersions,
      out Payload metadata)
    {
      metadata = (Payload) null;
      tablesRequested = (MetadataTable[]) null;
      rowVersions = (long[]) null;
      if (metadataHave == null || metadataHave.Length == 0)
        return;
      metadata = new Payload();
      tablesRequested = new MetadataTable[metadataHave.Length];
      rowVersions = new long[metadataHave.Length];
      for (int index = 0; index < metadataHave.Length; ++index)
      {
        try
        {
          MetadataTable metadataTable = (MetadataTable) Enum.Parse(typeof (MetadataTable), metadataHave[index].TableName, true);
          tablesRequested[index] = metadataTable;
        }
        catch (ArgumentException ex)
        {
          throw new SoapException(ResourceStrings.Get("NoMetadataTablesRequested"), Soap12FaultCodes.SenderFaultCode, new SoapFaultSubCode(Soap12FaultCodes.RpcBadArgumentsFaultCode, WorkItemTrackingFaultCodes.UnknownMetadataTable));
        }
        if (metadataHave[index].RowVersion < 0L)
          throw new SoapException(ResourceStrings.Get("InvalidRowVersion"), Soap12FaultCodes.SenderFaultCode, new SoapFaultSubCode(Soap12FaultCodes.RpcBadArgumentsFaultCode, WorkItemTrackingFaultCodes.InvalidRowVersion));
        rowVersions[index] = metadataHave[index].RowVersion;
      }
    }

    protected bool HasAdminRights(IdentityDescriptor identityDescriptor, string projectUri)
    {
      bool flag = false;
      IdentityService service = this.RequestContext.GetService<IdentityService>();
      if (service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, identityDescriptor))
        flag = true;
      if (!flag && service.IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, identityDescriptor))
        flag = true;
      if (!flag && this.RequestContext.GetExtension<IAuthorizationProviderFactory>().IsPermitted(this.RequestContext, PermissionNamespaces.Global, "CREATE_PROJECTS", identityDescriptor))
        flag = true;
      if (!flag && !string.IsNullOrEmpty(projectUri))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentity(this.RequestContext.Elevate(), IdentitySearchFilter.AdministratorsGroup, projectUri, QueryMembership.None, (IEnumerable<string>) null);
        if (identity == null)
          throw new IdentityNotFoundException();
        if (service.IsMember(this.RequestContext, identity.Descriptor, identityDescriptor))
          flag = true;
      }
      return flag;
    }

    private XmlElement QueryWorkItemsNewAPI(
      XmlElement psQuery,
      QuerySortOrderEntry[] sortOrder,
      bool countOnly,
      out int count,
      out DateTime asOfDate,
      IVssIdentity user,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      Payload metadataPayload,
      out string dbStamp)
    {
      IWorkItemQueryService service = this.RequestContext.GetService<IWorkItemQueryService>();
      string wiql;
      IDictionary context;
      bool dayPrecision;
      QueryExpression query;
      if (QueryWiqlDeserializer.TryDeserialize(psQuery, out wiql, out context, out dayPrecision))
      {
        query = service.ConvertToQueryExpression(this.RequestContext, wiql, context, dayPrecision);
        IFieldTypeDictionary fieldsSnapshot = this.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(this.RequestContext);
        query.SortFields = (IEnumerable<QuerySortField>) QueryExpressionDeserializer.ConvertSortFields(fieldsSnapshot, query.QueryType, (IEnumerable<QuerySortOrderEntry>) sortOrder ?? Enumerable.Empty<QuerySortOrderEntry>()).ToArray<QuerySortField>();
      }
      else
        query = QueryExpressionDeserializer.Deserialize(this.RequestContext, psQuery, countOnly ? (IEnumerable<QuerySortOrderEntry>) (QuerySortOrderEntry[]) null : (IEnumerable<QuerySortOrderEntry>) sortOrder);
      QueryResult queryResult = service.ExecuteQuery(this.RequestContext, query);
      asOfDate = queryResult.AsOfDateTime;
      count = queryResult.Count;
      XmlElement xmlElement = QueryResultSerializer.Serialize(queryResult, countOnly);
      if (tablesRequested != null && tablesRequested.Length != 0)
      {
        int num1 = 0;
        int num2 = 0;
        string empty = string.Empty;
        int num3 = 0;
        DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(this.RequestContext);
        this.RequestContext.PartialResultsReady();
        IVssIdentity user1 = user;
        MetadataTable[] tablesRequested1 = tablesRequested;
        long[] rowVersions1 = rowVersions;
        Payload metadataPayload1 = metadataPayload;
        ref int local1 = ref num1;
        ref int local2 = ref num2;
        ref string local3 = ref empty;
        ref string local4 = ref dbStamp;
        ref int local5 = ref num3;
        dataAccessLayerImpl.GetMetadata(user1, tablesRequested1, rowVersions1, metadataPayload1, out local1, out local2, out local3, out local4, out local5);
        return xmlElement;
      }
      dbStamp = (string) null;
      return xmlElement;
    }

    private void UpdateLegacy(
      XmlElement package,
      out XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out Payload metadata,
      bool validationOnly = false,
      bool isBulk = false)
    {
      foreach (XmlNode queryTextNode in QueryItemHelper.GetQueryTextNodes(package))
        WiqlTextHelper.ValidateWiqlTextRequirements(this.RequestContext, queryTextNode.InnerText);
      XmlElement xmlElement1 = result = (XmlElement) null;
      string dbStamp1 = dbStamp = string.Empty;
      Payload metadata1 = metadata = (Payload) null;
      MetadataTable[] tablesRequested = (MetadataTable[]) null;
      long[] rowVersions = (long[]) null;
      this.ConvertMetadataHave(metadataHave, out tablesRequested, out rowVersions, out metadata1);
      bool eventsEnabled = this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.RequestContext).EventsEnabled;
      XmlElement xmlElement2 = new DataAccessLayerImpl(this.RequestContext).UpdateImpl(package, tablesRequested, rowVersions, metadata1, eventsEnabled, out dbStamp1, isBulk, out bool _, (IVssIdentity) null, false, false, validationOnly, true);
      result = xmlElement2;
      dbStamp = dbStamp1;
      metadata = metadata1;
    }

    private void UpdateNewApi(
      XmlElement package,
      ref XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      ref string dbStamp,
      ref Payload metadata,
      bool isBulk = false)
    {
      this.RequestContext.TraceEnter(905050, "WebServices", nameof (ClientService), nameof (UpdateNewApi));
      try
      {
        this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Executing Update NewAPI for client version.", (object) this.ClientVersion);
        this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "UpdateElement: {0}", (object) package.OuterXml);
        if (this.ClientVersion < 7)
          this.TestCaseParameterDataHelper.HandleUpdate(package, isBulk);
        IUpdateHandler[] updateHandlerArray = new IUpdateHandler[3]
        {
          (IUpdateHandler) new WorkItemUpdateHandler(isBulk),
          (IUpdateHandler) new WorkItemTypeRenameHandler(),
          (IUpdateHandler) new WorkItemDestroyHandler()
        };
        XElement element = new XElement((XName) "UpdateResults");
        XElement xelement;
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(package.OuterXml)))
        {
          MemoryStream input = memoryStream;
          using (XmlReader reader = XmlReader.Create((Stream) input, new XmlReaderSettings()
          {
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          }))
          {
            int content1 = (int) reader.MoveToContent();
            xelement = XDocument.Load(reader, LoadOptions.SetLineInfo).Root;
            foreach (IUpdateHandler updateHandler in updateHandlerArray)
            {
              IEnumerable<XElement> result1;
              xelement = updateHandler.ProcessUpdate(this.RequestContext, xelement, out result1);
              if (result1 != null)
              {
                foreach (XElement content2 in result1)
                  element.Add((object) content2);
              }
              if (xelement != null)
              {
                if (!xelement.HasElements)
                  break;
              }
              else
                break;
            }
          }
        }
        if (element.HasElements)
          result = element.ToXmlElement();
        if (xelement != null && xelement.HasElements)
        {
          this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Deserialized package has Legacy Elements to send to the old DAL", (object) this.ClientVersion);
          XmlElement result2;
          this.UpdateLegacy(xelement.ToXmlElement(), out result2, metadataHave, out dbStamp, out metadata, isBulk: isBulk);
          if (!element.HasElements)
          {
            result = result2;
            this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Returning results from legacy call since the incoming package didn't send any objects to the NewAPI.", (object) this.ClientVersion);
          }
          else
          {
            if (result == null)
              throw new InvalidOperationException("The result from the new API is null, but should not be");
            this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Grafting results from NewAPI with results from Old API", (object) this.ClientVersion);
            foreach (XmlNode childNode in result2.ChildNodes)
              result.AppendChild(childNode);
          }
        }
        else
        {
          if (metadataHave == null || metadataHave.Length == 0)
            return;
          this.RequestContext.Trace(905052, TraceLevel.Info, "WebServices", nameof (ClientService), "ClientVersion: {0}. Fetching Metadata to use with NewAPI results only.", (object) this.ClientVersion);
          int locale = 0;
          string callerIdentity = (string) null;
          this._GetMetadataInternal(metadataHave, out metadata, out dbStamp, out locale, out int _, out callerIdentity, out string _, out int _);
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(905062, TraceLevel.Error, "WebServices", nameof (ClientService), ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(905150, "WebServices", nameof (ClientService), nameof (UpdateNewApi));
      }
    }

    private Payload GetCompatibilityPayload()
    {
      PayloadConverter converter = new PayloadConverter();
      if (this.ClientVersion < 7)
        converter.AddGlobalProcessColumnCallback(new ProcessColumnCallback(this.TestCaseParameterDataHelper.GetParameterDataFieldValueForOldClients));
      return new Payload(converter);
    }

    private TestCaseParameterDataHelper TestCaseParameterDataHelper
    {
      get
      {
        if (this.m_testCaseParameterDataHelper == null)
          this.m_testCaseParameterDataHelper = new TestCaseParameterDataHelper(this.RequestContext);
        return this.m_testCaseParameterDataHelper;
      }
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.RequestContext.GetService<IdentityService>().ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters);
      if (source.Count > 1)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source)
          stringBuilder.AppendLine(FrameworkResources.MultipleIdentitiesFoundRow((object) identity.DisplayName, (object) identity.GetLegacyDistinctDisplayName()));
        throw new LegacyValidationException(TFCommonResources.MultipleIdentitiesFoundMessage((object) factorValue, (object) stringBuilder.ToString()));
      }
      return source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    protected internal void GetQueries(
      int projectId,
      bool useNewQueryItemsPayload,
      Payload queriesPayload)
    {
      if (projectId < 0)
        throw new ArgumentException("ProjectID");
      if (queriesPayload == null)
        throw new ArgumentNullException(nameof (queriesPayload));
      try
      {
        if (!useNewQueryItemsPayload)
          this.RequestContext.Trace(900764, TraceLevel.Info, "Queries", "Query", "Using StoredQuery legacy payload, old calls to GetStoredQueries() generating this request for project: {0}.", (object) projectId);
        else
          this.RequestContext.Trace(900765, TraceLevel.Info, "Queries", "Query", "Using new QueryItem payload, newer calls to GetStoredQueryItems() generating this request for project {0}.", (object) projectId);
        ITeamFoundationQueryItemService service = this.RequestContext.GetService<ITeamFoundationQueryItemService>();
        TreeNode node = (TreeNode) null;
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItems1;
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItems2;
        if (this.RequestContext.WitContext().TreeService.LegacyTryGetTreeNode(projectId, out node))
        {
          Guid cssNodeId = node.CssNodeId;
          QueryFolder[] queryHierarchy = service.GetQueryHierarchy(this.RequestContext, cssNodeId);
          service.StripOutCurrentIterationTeamParameter(this.RequestContext, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) queryHierarchy);
          queryItems1 = queryHierarchy[0] != null ? this.FlattenQueryFolder((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) queryHierarchy[0], useNewQueryItemsPayload) : Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
          queryItems2 = this.FlattenQueryFolder((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) queryHierarchy[1], useNewQueryItemsPayload);
        }
        else
        {
          queryItems1 = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
          queryItems2 = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
          this.RequestContext.Trace(900761, TraceLevel.Info, "Queries", "Query", "Could not locate project using projectId: {0}, will return empty payload.", (object) projectId);
        }
        if (useNewQueryItemsPayload)
        {
          PayloadCompatibilityUtils.FillPayloadWithQueryItems(queriesPayload, queryItems1, queryItems2);
        }
        else
        {
          new List<Query>().AddRange(queryItems1.OfType<Query>().Concat<Query>(queryItems2.OfType<Query>()));
          PayloadCompatibilityUtils.FillPayloadWithStoredQueryItems(queriesPayload, queryItems1.Concat<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>(queryItems2).OfType<Query>(), projectId);
        }
      }
      catch (QueryItemNotFoundException ex)
      {
        this.RequestContext.Trace(900759, TraceLevel.Info, "Queries", "Query", "An error occurred fetch public/private queries, no query hierarchy found.");
        this.RequestContext.TraceException(900760, "Queries", "Query", (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(900760, "Queries", "Query", ex);
        throw;
      }
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> FlattenQueryFolder(
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem,
      bool returnFolders)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>(queryItem, nameof (queryItem));
      if (!(queryItem is QueryFolder queryFolder))
      {
        yield return queryItem;
      }
      else
      {
        if (returnFolders)
          yield return (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) queryFolder;
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem1 in queryFolder.Children.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>>) (qc => this.FlattenQueryFolder(qc, returnFolders))))
          yield return queryItem1;
      }
    }
  }
}
