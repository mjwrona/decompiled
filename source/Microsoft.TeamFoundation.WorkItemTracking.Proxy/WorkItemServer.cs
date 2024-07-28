// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.WorkItemServer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class WorkItemServer : ITfsTeamProjectCollectionObject, ITfsRequestListener
  {
    private TfsTeamProjectCollection m_tfs;
    private WorkItemServerVersion m_serverVersion;
    private string m_clientServiceUrl;
    private string m_attachmentsUrl;
    private string m_configurationUrl;
    private Dictionary<string, string> m_replicationHeaders = new Dictionary<string, string>();
    private object m_replicationHeadersLock = new object();
    private const int BufferSize = 1048576;
    internal const int TimeOut = 900000;
    internal const int FileTimeOut = 3600000;
    private static int m_requestCount;
    private static long m_maxAttachmentSize;
    private static long m_attachmentSizeLimitForBuffering = 4194304;
    private static bool m_maxAttachmentIsCached;

    private WorkItemServer()
    {
    }

    void ITfsTeamProjectCollectionObject.Initialize(TfsTeamProjectCollection tfs) => this.InitProxyInfo(tfs);

    internal void InitProxyInfo(TfsTeamProjectCollection tfs)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(tfs, nameof (tfs));
      this.m_tfs = tfs;
      ILocationService service = tfs.GetService<ILocationService>();
      KeyValuePair<string, Guid>[] keyValuePairArray = new KeyValuePair<string, Guid>[8]
      {
        new KeyValuePair<string, Guid>("WorkitemService", ServiceIdentifiers.WorkItem),
        new KeyValuePair<string, Guid>("WorkitemService2", ServiceIdentifiers.WorkItem2),
        new KeyValuePair<string, Guid>("WorkitemService3", ServiceIdentifiers.WorkItem3),
        new KeyValuePair<string, Guid>("WorkitemService4", ServiceIdentifiers.WorkItem4),
        new KeyValuePair<string, Guid>("WorkitemService5", ServiceIdentifiers.WorkItem5),
        new KeyValuePair<string, Guid>("WorkitemService6", ServiceIdentifiers.WorkItem6),
        new KeyValuePair<string, Guid>("WorkitemService7", ServiceIdentifiers.WorkItem7),
        new KeyValuePair<string, Guid>("WorkitemService8", ServiceIdentifiers.WorkItem8)
      };
      for (int index = keyValuePairArray.Length - 1; index >= 0; --index)
      {
        this.m_clientServiceUrl = service.LocationForCurrentConnection(keyValuePairArray[index].Key, keyValuePairArray[index].Value);
        if (!string.IsNullOrEmpty(this.m_clientServiceUrl))
        {
          this.m_serverVersion = (WorkItemServerVersion) (index + 1);
          break;
        }
      }
      this.m_attachmentsUrl = service.LocationForCurrentConnection("WorkitemAttachmentHandler", ServiceIdentifiers.WorkItemAttachmentHandler);
      this.m_configurationUrl = service.LocationForCurrentConnection("configurationsettingsurl", ServiceIdentifiers.ConfigurationServerUrl);
      if (string.IsNullOrEmpty(this.m_clientServiceUrl) || string.IsNullOrEmpty(this.m_attachmentsUrl) || string.IsNullOrEmpty(this.m_configurationUrl))
        throw new ArgumentException(ResourceStrings.Get("ErrorBisMiddleTierNotRegistered"));
    }

    public WorkItemServerVersion WorkItemServerVersion => this.m_serverVersion;

    private ClientService GetClientServiceProxy(string requestId)
    {
      ClientService clientServiceProxy;
      switch (this.m_serverVersion)
      {
        case WorkItemServerVersion.V1:
          clientServiceProxy = new ClientService(this.m_tfs);
          break;
        case WorkItemServerVersion.V2:
          clientServiceProxy = (ClientService) new ClientService2(this.m_tfs);
          break;
        case WorkItemServerVersion.V3:
          clientServiceProxy = (ClientService) new ClientService3(this.m_tfs);
          break;
        case WorkItemServerVersion.V4:
          clientServiceProxy = (ClientService) new ClientService4(this.m_tfs);
          break;
        case WorkItemServerVersion.V5:
          clientServiceProxy = (ClientService) new ClientService5(this.m_tfs);
          break;
        case WorkItemServerVersion.V6:
          clientServiceProxy = (ClientService) new ClientService6(this.m_tfs);
          break;
        case WorkItemServerVersion.V7:
          clientServiceProxy = (ClientService) new ClientService7(this.m_tfs);
          break;
        default:
          clientServiceProxy = (ClientService) new ClientService8(this.m_tfs);
          break;
      }
      clientServiceProxy.AddMessageHeader((TfsMessageHeader) new RequestHeader(string.IsNullOrEmpty(requestId) ? WorkItemServer.NewRequestId() : requestId));
      clientServiceProxy.InitRequestListener((ITfsRequestListener) this);
      return clientServiceProxy;
    }

    public string GetWorkitemTrackingVersion(string requestId)
    {
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      try
      {
        this.IncRequestCount();
        return clientServiceProxy.GetWorkitemTrackingVersion();
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
    }

    public void StampWorkitemCache(string requestId)
    {
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      try
      {
        this.IncRequestCount();
        clientServiceProxy.StampWorkitemCache();
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
    }

    public void GetWorkItem(
      string requestId,
      int workItemId,
      int revisionId,
      int minimumRevisionId,
      DateTime? asOfDate,
      bool useMaster,
      out IWorkItemRowSets workItem,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyGetWorkItemBegin);
      this.ValidateRequestId(requestId);
      RowSetCollection workItem1 = (RowSetCollection) null;
      RowSetCollection metadata1 = (RowSetCollection) null;
      dbStamp = string.Empty;
      DateTime? asOfDate1 = new DateTime?();
      if (asOfDate.HasValue)
        asOfDate1 = new DateTime?(asOfDate.Value.ToLocalTime());
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetWorkItem: workitemid={0}. revisionid={1}. Url={2}.", (object) workItemId, (object) revisionId, (object) clientServiceProxy.Url.AbsoluteUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          clientServiceProxy.GetWorkItem(workItemId, revisionId, minimumRevisionId, asOfDate1, useMaster, out workItem1, metadataHave, out dbStamp, out metadata1);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      workItem = (IWorkItemRowSets) new WorkItemRowSets(workItem1);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyGetWorkItemEnd);
    }

    public void QueryWorkitemRevisions(
      string requestId,
      XmlElement psQuery,
      QuerySortOrderEntry[] sort,
      bool useMaster,
      out IdRevisionPair[] pairs,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyQueryBegin);
      this.ValidateRequestId(requestId);
      if (psQuery == null)
        throw new ArgumentNullException(nameof (psQuery));
      if (psQuery.Name.Equals("LinksQuery") || string.IsNullOrEmpty(psQuery.GetAttribute("Ever")))
        throw new ArgumentException(ResourceStrings.Get("BadQuery"), nameof (psQuery));
      RowSetCollection metadata1 = (RowSetCollection) null;
      dbStamp = string.Empty;
      pairs = (IdRevisionPair[]) null;
      asOfDate = DateTime.MinValue;
      if (!(this.GetClientServiceProxy(requestId) is ClientService3 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.QueryWorkitemRevisions: psQueryXml='{0}'. Url={1}.", (object) psQuery.InnerXml, (object) clientServiceProxy.Url.AbsoluteUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          XmlNode resultIds;
          clientServiceProxy.QueryWorkitems((XmlNode) psQuery, sort, useMaster, out resultIds, out asOfDate, metadataHave, out dbStamp, out metadata1);
          XmlNodeList xmlNodeList = resultIds.SelectNodes("id");
          List<IdRevisionPair> idRevisionPairList = new List<IdRevisionPair>(xmlNodeList.Count);
          foreach (XmlNode xmlNode in xmlNodeList)
          {
            int int32_1 = XmlConvert.ToInt32(xmlNode.Attributes.GetNamedItem("i").Value);
            int int32_2 = XmlConvert.ToInt32(xmlNode.Attributes.GetNamedItem("r").Value);
            idRevisionPairList.Add(new IdRevisionPair(int32_1, int32_2));
          }
          pairs = idRevisionPairList.ToArray();
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyQueryEnd);
    }

    public void QueryWorkitems(
      string requestId,
      XmlElement psQuery,
      QuerySortOrderEntry[] sort,
      bool useMaster,
      out int[] ids,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyQueryBegin);
      this.ValidateRequestId(requestId);
      if (psQuery == null)
        throw new ArgumentNullException(nameof (psQuery));
      if (psQuery.Name.Equals("LinksQuery"))
        throw new ArgumentException(ResourceStrings.Get("BadQuery"), nameof (psQuery));
      RowSetCollection metadata1 = (RowSetCollection) null;
      dbStamp = string.Empty;
      ids = (int[]) null;
      asOfDate = DateTime.MinValue;
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.QueryWorkitems: psQueryXml='{0}'. Url={1}.", (object) psQuery.InnerXml, (object) clientServiceProxy.Url.AbsoluteUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          XmlNode resultIds;
          clientServiceProxy.QueryWorkitems((XmlNode) psQuery, sort, useMaster, out resultIds, out asOfDate, metadataHave, out dbStamp, out metadata1);
          ids = QueryHelper.ConvertRegularQueryResult((XmlElement) resultIds).ToArray();
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyQueryEnd);
    }

    public void QueryLinkedWorkitems(
      string requestId,
      XmlElement psQuery,
      QuerySortOrderEntry[] sort,
      bool useMaster,
      out WorkItemRelation[] relations,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyQueryBegin);
      if (!psQuery.Name.Equals("LinksQuery"))
        throw new ArgumentException(ResourceStrings.Get("BadQuery"), nameof (psQuery));
      this.ValidateRequestId(requestId);
      RowSetCollection metadata1 = (RowSetCollection) null;
      dbStamp = string.Empty;
      relations = (WorkItemRelation[]) null;
      asOfDate = DateTime.MinValue;
      if (!(this.GetClientServiceProxy(requestId) is ClientService2 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.QueryLinkedWorkitems: psQueryXml='{0}'. Url={1}.", (object) psQuery.InnerXml, (object) clientServiceProxy.Url.AbsoluteUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          XmlNode resultIds;
          clientServiceProxy.QueryWorkitems((XmlNode) psQuery, sort, useMaster, out resultIds, out asOfDate, metadataHave, out dbStamp, out metadata1);
          relations = QueryHelper.ConvertLinkQueryResult((XmlElement) resultIds).Select<InternalWorkItemRelation, WorkItemRelation>((Func<InternalWorkItemRelation, WorkItemRelation>) (item => new WorkItemRelation(item.SourceID, item.TargetID, item.LinkTypeID, item.IsLocked))).ToArray<WorkItemRelation>();
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyQueryEnd);
    }

    public void PageWorkitemsByIds(
      string requestId,
      int[] ids,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      bool useMaster,
      out IPagedItemsRowSets items,
      MetadataTableHaveEntry[] metadataHave,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyPageItemsBegin);
      this.ValidateRequestId(requestId);
      RowSetCollection items1 = (RowSetCollection) null;
      RowSetCollection metadata1 = (RowSetCollection) null;
      DateTime? asOfDate1 = new DateTime?();
      if (asOfDate.HasValue)
        asOfDate1 = new DateTime?(asOfDate.Value.ToLocalTime());
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.PageWorkitemsByIds: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
          this.IncRequestCount();
          clientServiceProxy.PageWorkitemsByIds(ids, columns, longTextColumns, asOfDate1, useMaster, out items1, metadataHave, out metadata1);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      items = (IPagedItemsRowSets) new PagedItemsRowSets(items1);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyPageItemsEnd);
    }

    public void PageWorkitemsByIdRevs(
      string requestId,
      IdRevisionPair[] pairs,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      out DateTime pageDate,
      bool useMaster,
      out IPagedItemsRowSets items)
    {
      Marker.Process(Mark.ProxyPageItemsByPairsBegin);
      this.ValidateRequestId(requestId);
      RowSetCollection items1 = (RowSetCollection) null;
      pageDate = DateTime.MinValue;
      DateTime? asOfDate1 = new DateTime?();
      if (asOfDate.HasValue)
        asOfDate1 = new DateTime?(asOfDate.Value.ToLocalTime());
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.PageWorkitemsByIdRevs: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
          this.IncRequestCount();
          clientServiceProxy.PageWorkitemsByIdRevs(pairs, columns, longTextColumns, asOfDate1, out pageDate, useMaster, out items1);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      items = (IPagedItemsRowSets) new PagedItemsRowSets(items1);
      Marker.Process(Mark.ProxyPageItemsByPairsEnd);
    }

    public void QueryWorkitemCount(
      string requestId,
      XmlElement psQuery,
      bool useMaster,
      out int count,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      if (psQuery == null)
        throw new ArgumentNullException(nameof (psQuery));
      Marker.Process(Mark.ProxyQueryCountBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Entering proxy QueryWorkitemCount");
      this.ValidateRequestId(requestId);
      RowSetCollection metadata1 = (RowSetCollection) null;
      count = 0;
      asOfDate = DateTime.MinValue;
      dbStamp = string.Empty;
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.QueryWorkitemCount: psQueryXml='{0}'. Url={1}.", (object) psQuery.InnerXml, (object) clientServiceProxy.Url.AbsoluteUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          clientServiceProxy.QueryWorkitemCount((XmlNode) psQuery, useMaster, out count, out asOfDate, metadataHave, out dbStamp, out metadata1);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, ex.ToString());
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Exiting proxy QueryWorkitemCount");
      Marker.Process(Mark.ProxyQueryCountEnd);
    }

    public void GetMetadata(
      string requestId,
      bool useMaster,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity)
    {
      Marker.Process(Mark.ProxyGetMetadataBegin);
      this.ValidateRequestId(requestId);
      RowSetCollection metadata1 = (RowSetCollection) null;
      dbStamp = string.Empty;
      locale = 0;
      comparisonStyle = 0;
      callerIdentity = string.Empty;
      try
      {
        ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetMetadata: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
        RetryHandler retryHandler = new RetryHandler(RetryTypes.Deadlock);
        do
        {
          try
          {
            this.IncRequestCount();
            clientServiceProxy.GetMetadata(metadataHave, useMaster, out metadata1, out dbStamp, out locale, out comparisonStyle, out callerIdentity);
            break;
          }
          catch (SoapException ex)
          {
            retryHandler.HandleSoapException(ex);
          }
          catch (Exception ex)
          {
            this.TraceException(ex);
            throw;
          }
        }
        while (retryHandler.HasRemainingRetries);
        if (metadata1 != null)
          metadata = (IMetadataRowSets) new MetadataRowSets(metadata1);
        else
          metadata = (IMetadataRowSets) null;
      }
      finally
      {
        Marker.Process(Mark.ProxyGetMetadataEnd);
      }
    }

    public void GetMetadataEx(
      string requestId,
      bool useMaster,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid)
    {
      Marker.Process(Mark.ProxyGetMetadataBegin);
      dbStamp = string.Empty;
      metadata = (IMetadataRowSets) null;
      locale = 0;
      comparisonStyle = 0;
      callerIdentity = string.Empty;
      callerIdentitySid = string.Empty;
      try
      {
        this.ValidateRequestId(requestId);
        RowSetCollection metadata1 = (RowSetCollection) null;
        ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetMetadata: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
        RetryHandler retryHandler = new RetryHandler(RetryTypes.Deadlock);
        do
        {
          try
          {
            this.IncRequestCount();
            clientServiceProxy.GetMetadataEx(metadataHave, useMaster, out metadata1, out dbStamp, out locale, out comparisonStyle, out callerIdentity, out callerIdentitySid);
            break;
          }
          catch (SoapException ex)
          {
            retryHandler.HandleSoapException(ex);
          }
          catch (Exception ex)
          {
            this.TraceException(ex);
            throw;
          }
        }
        while (retryHandler.HasRemainingRetries);
        if (metadata1 == null)
          return;
        metadata = (IMetadataRowSets) new MetadataRowSets(metadata1);
      }
      finally
      {
        Marker.Process(Mark.ProxyGetMetadataEnd);
      }
    }

    public void GetMetadataEx2(
      string requestId,
      bool useMaster,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata,
      out int locale,
      out int comparisonStyle,
      out int displayMode)
    {
      Marker.Process(Mark.ProxyGetMetadataBegin);
      dbStamp = string.Empty;
      locale = 0;
      comparisonStyle = 0;
      displayMode = 0;
      metadata = (IMetadataRowSets) null;
      try
      {
        this.ValidateRequestId(requestId);
        RowSetCollection metadata1 = (RowSetCollection) null;
        ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetMetadata: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
        RetryHandler retryHandler = new RetryHandler(RetryTypes.Deadlock);
        do
        {
          try
          {
            this.IncRequestCount();
            clientServiceProxy.GetMetadataEx2(metadataHave, useMaster, out metadata1, out dbStamp, out locale, out comparisonStyle, out displayMode);
            break;
          }
          catch (SoapException ex)
          {
            retryHandler.HandleSoapException(ex);
          }
          catch (Exception ex)
          {
            this.TraceException(ex);
            throw;
          }
        }
        while (retryHandler.HasRemainingRetries);
        if (metadata1 == null)
          return;
        metadata = (IMetadataRowSets) new MetadataRowSets(metadata1);
      }
      finally
      {
        Marker.Process(Mark.ProxyGetMetadataEnd);
      }
    }

    public bool BulkUpdate(
      string requestId,
      XmlElement package,
      out XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyBulkUpdateBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "BulkUpdate()");
      this.ValidateRequestId(requestId);
      RowSetCollection metadata1 = (RowSetCollection) null;
      result = (XmlElement) null;
      bool flag = false;
      dbStamp = string.Empty;
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.BulkUpdate: Url={0}. PackageXml='{1}'", (object) clientServiceProxy.Url.AbsoluteUri, package != null ? (object) package.OuterXml : (object) string.Empty);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          XmlNode result1;
          flag = clientServiceProxy.BulkUpdate((XmlNode) package, out result1, metadataHave, out dbStamp, out metadata1);
          result = result1 != null ? (XmlElement) result1 : throw new ArgumentNullException(nameof (result));
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "BulkUpdate response {0}", (object) result.OuterXml);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyBulkUpdateEnd);
      return flag;
    }

    public void Update(
      string requestId,
      XmlElement package,
      out XmlElement result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out IMetadataRowSets metadata)
    {
      Marker.Process(Mark.ProxyUpdateBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Update()");
      this.ValidateRequestId(requestId);
      RowSetCollection metadata1 = (RowSetCollection) null;
      result = (XmlElement) null;
      dbStamp = string.Empty;
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      clientServiceProxy.SetTimeout(900000);
      if (package != null && package.ChildNodes.Count > 0)
      {
        XmlNode childNode = package.ChildNodes[0];
        if (TFStringComparer.UpdateAction.Equals(childNode.Name, "InsertWorkItem") || TFStringComparer.UpdateAction.Equals(childNode.Name, "UpdateWorkItem") || TFStringComparer.UpdateAction.Equals(childNode.Name, "InsertWorkItemLink") || TFStringComparer.UpdateAction.Equals(childNode.Name, "UpdateWorkItemLink") || TFStringComparer.UpdateAction.Equals(childNode.Name, "DeleteWorkItemLink") || TFStringComparer.UpdateAction.Equals(childNode.Name, "InsertQueryItem") || TFStringComparer.UpdateAction.Equals(childNode.Name, "UpdateQueryItem") || TFStringComparer.UpdateAction.Equals(childNode.Name, "DeleteQueryItem"))
          clientServiceProxy.SetTimeout((int) TfsRequestSettings.Default.SendTimeout.TotalMilliseconds);
        if (TFStringComparer.UpdateAction.Equals(childNode.Name, "UpdateField"))
          clientServiceProxy.SetTimeout(3600000);
      }
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.Update: Url={0}. PackageXml='{1}'", (object) clientServiceProxy.Url.AbsoluteUri, package != null ? (object) package.OuterXml : (object) string.Empty);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.All);
      do
      {
        try
        {
          this.IncRequestCount();
          XmlNode result1;
          clientServiceProxy.Update((XmlNode) package, out result1, metadataHave, out dbStamp, out metadata1);
          result = (XmlElement) result1;
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Update response {0}", (object) result.OuterXml);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      metadata = metadata1 == null ? (IMetadataRowSets) null : (IMetadataRowSets) new MetadataRowSets(metadata1);
      Marker.Process(Mark.ProxyUpdateEnd);
    }

    public long MaxAttachmentSize
    {
      get
      {
        if (!WorkItemServer.m_maxAttachmentIsCached)
          this.UpdateMaxAttachmentSize();
        return WorkItemServer.m_maxAttachmentSize;
      }
    }

    public void UpdateMaxAttachmentSize()
    {
      ConfigurationSettingsService configurationProxy = this.GetConfigurationProxy();
      long maxAttachmentSize;
      try
      {
        maxAttachmentSize = configurationProxy.GetMaxAttachmentSize();
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      WorkItemServer.m_maxAttachmentSize = maxAttachmentSize >= 2000000000L ? 2000000000L : maxAttachmentSize;
      WorkItemServer.m_maxAttachmentIsCached = true;
    }

    public void UploadFile(FileAttachment fileAttachment)
    {
      Marker.Process(Mark.ProxyUploadFileBegin);
      if (fileAttachment.LocalFile == null)
        throw new ArgumentNullException(nameof (fileAttachment));
      if (fileAttachment.LocalFile.Length > this.MaxAttachmentSize)
      {
        if (this.MaxAttachmentSize < 102400L)
          throw new ArgumentException(InternalsResourceStrings.Format(InternalsResourceStrings.Get("FileUploadedExceededMaxBytes"), (object) this.MaxAttachmentSize));
        throw new ArgumentException(InternalsResourceStrings.Format("FileUploadedExceededMaxMb", (object) ((float) ((double) this.MaxAttachmentSize / 1024.0 / 1024.0)).ToString("0.000")), nameof (fileAttachment));
      }
      bool allowWriteStreamBuffering = true;
      if (fileAttachment.LocalFile.Length > WorkItemServer.m_attachmentSizeLimitForBuffering)
        allowWriteStreamBuffering = false;
      while (true)
      {
        try
        {
          this.UploadFileInternal(fileAttachment, allowWriteStreamBuffering);
          break;
        }
        catch (TeamFoundationServerUnauthorizedException ex)
        {
          if (!this.HandleUnauthorizedException(ex))
            throw;
        }
        catch (WebException ex)
        {
          string content;
          this.TraceException((Exception) ex, out content);
          HttpWebResponse response = ex.Response as HttpWebResponse;
          if (!allowWriteStreamBuffering && response != null && response.StatusCode == HttpStatusCode.Unauthorized)
          {
            allowWriteStreamBuffering = true;
          }
          else
          {
            if (ex.Status == WebExceptionStatus.ProtocolError && response != null)
            {
              if (response.ContentType.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(content))
                throw new Exception(ResourceStrings.Format("HTTPStatusCodeAndDescription", (object) response.StatusCode, (object) content));
              throw new Exception(ResourceStrings.Format("HTTPStatusCode", (object) response.StatusCode));
            }
            throw;
          }
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
    }

    private HttpWebRequest CreateFileUploadWebRequest()
    {
      TfsRequestSettings settings = TfsRequestSettings.Default.Clone();
      settings.SendTimeout = TimeSpan.FromMilliseconds(3600000.0);
      return TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(this.m_attachmentsUrl), this.m_tfs.SessionId, (string) null, this.m_tfs.Culture, settings, this.m_tfs.ClientCredentials, this.m_tfs.IdentityToImpersonate);
    }

    protected void UploadFileInternal(FileAttachment fileAttachment, bool allowWriteStreamBuffering)
    {
      string str = "----------------------------" + DateTime.Now.Ticks.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture);
      HttpWebRequest uploadWebRequest = this.CreateFileUploadWebRequest();
      uploadWebRequest.Method = "POST";
      uploadWebRequest.ContentType = "multipart/form-data; boundary=" + str.Substring(2);
      MemoryStream memoryStream = new MemoryStream(1024);
      StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, (Encoding) new UTF8Encoding(false));
      streamWriter.WriteLine(str);
      streamWriter.Write("Content-Disposition: form-data; name=\"");
      streamWriter.Write("AreaNodeUri");
      streamWriter.WriteLine("\"");
      streamWriter.WriteLine();
      streamWriter.WriteLine(fileAttachment.AreaNodeUri);
      streamWriter.WriteLine(str);
      streamWriter.Write("Content-Disposition: form-data; name=\"");
      streamWriter.Write("ProjectUri");
      streamWriter.WriteLine("\"");
      streamWriter.WriteLine();
      streamWriter.WriteLine(fileAttachment.ProjectUri);
      streamWriter.WriteLine(str);
      streamWriter.Write("Content-Disposition: form-data; name=\"");
      streamWriter.Write("FileNameGUID");
      streamWriter.WriteLine("\"");
      streamWriter.WriteLine();
      streamWriter.WriteLine(fileAttachment.FileNameGUID.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
      streamWriter.WriteLine(str);
      streamWriter.Write("Content-Disposition: form-data; name=\"");
      streamWriter.Write("Content");
      streamWriter.WriteLine("\"; filename=\"FileNameGUID\"");
      streamWriter.WriteLine("Content-Type: application/octet-stream");
      streamWriter.WriteLine();
      streamWriter.Flush();
      int position = (int) memoryStream.Position;
      streamWriter.WriteLine();
      streamWriter.Write(str);
      streamWriter.WriteLine("--");
      streamWriter.Close();
      byte[] array = memoryStream.ToArray();
      byte[] buffer = new byte[1048576];
      long length;
      try
      {
        length = fileAttachment.LocalFile.Length;
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Attachment file length: {0}", (object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch (NotSupportedException ex)
      {
        throw new Exception(ResourceStrings.Get("StreamDoesNotSupportLength"));
      }
      uploadWebRequest.ContentLength = (long) array.Length + length;
      uploadWebRequest.AllowWriteStreamBuffering = allowWriteStreamBuffering;
      if (!allowWriteStreamBuffering)
        uploadWebRequest.ConnectionGroupName = Guid.NewGuid().ToString();
      using (Stream requestStream = uploadWebRequest.GetRequestStream())
      {
        requestStream.Write(array, 0, position);
        requestStream.Flush();
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Upload File buffer start");
        fileAttachment.LocalFile.Seek(0L, SeekOrigin.Begin);
        int count;
        while ((count = fileAttachment.LocalFile.Read(buffer, 0, buffer.Length)) > 0)
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Upload File buffer");
          requestStream.Write(buffer, 0, count);
          requestStream.Flush();
        }
        requestStream.Write(array, position, array.Length - position);
      }
      using (HttpWebResponse response = (HttpWebResponse) uploadWebRequest.GetResponse())
      {
        this.ProcessHttpWebResponse(response);
        if (response.StatusCode != HttpStatusCode.OK)
          throw new Exception(ResourceStrings.Format("HTTPStatusCode", (object) response.StatusCode));
      }
      Marker.Process(Mark.ProxyUploadFileEnd);
    }

    public string DownloadFile(int fileAttachmentId)
    {
      Marker.Process(Mark.ProxyDownloadFileBegin);
      TeamFoundationTrace.Info(TraceKeywordSets.Database, "DownloadFile() ID:" + fileAttachmentId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      UriBuilder uriBuilder = new UriBuilder(new Uri(this.m_attachmentsUrl));
      uriBuilder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "FileID", (object) fileAttachmentId);
      TfsRequestSettings settings = TfsRequestSettings.Default.Clone();
      settings.SendTimeout = TimeSpan.FromMilliseconds(3600000.0);
label_1:
      HttpWebRequest httpWebRequest = TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(uriBuilder.Uri), this.m_tfs.SessionId, (string) null, this.m_tfs.Culture, settings, this.m_tfs.ClientCredentials, this.m_tfs.IdentityToImpersonate);
      httpWebRequest.Method = "GET";
      string tempFileName;
      try
      {
        using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
        {
          this.ProcessHttpWebResponse(response);
          Stream stream1 = response.StatusCode == HttpStatusCode.OK ? response.GetResponseStream() : throw new Exception(ResourceStrings.Format("HTTPStatusCode", (object) response.StatusCode));
          byte[] buffer = new byte[1048576];
          tempFileName = Path.GetTempFileName();
          Stream stream2 = (Stream) System.IO.File.OpenWrite(tempFileName);
          int count;
          while ((count = stream1.Read(buffer, 0, buffer.Length)) > 0)
            stream2.Write(buffer, 0, count);
          stream1.Close();
          stream2.Close();
        }
      }
      catch (TeamFoundationServerUnauthorizedException ex)
      {
        if (!this.HandleUnauthorizedException(ex))
          throw;
        else
          goto label_1;
      }
      catch (WebException ex)
      {
        string content;
        this.TraceException((Exception) ex, out content);
        if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
        {
          if (ex.Response is HttpWebResponse response && !string.IsNullOrEmpty(content))
            throw new Exception(ResourceStrings.Format("HTTPStatusCodeAndDescription", (object) response.StatusCode, (object) content));
          throw;
        }
        else
          throw;
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      TeamFoundationTrace.Info(TraceKeywordSets.Database, "DownloadFile() End");
      Marker.Process(Mark.ProxyDownloadFileEnd);
      return tempFileName;
    }

    private void ProcessHttpWebResponse(HttpWebResponse response)
    {
      if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Found)
        throw new TeamFoundationServerUnauthorizedException();
    }

    private bool HandleUnauthorizedException(TeamFoundationServerUnauthorizedException ex) => false;

    public void GetStoredQuery(
      string requestId,
      bool useMaster,
      Guid queryId,
      out RowSetCollection queryDataSet)
    {
      Marker.Process(Mark.ProxyGetQueryBegin);
      queryDataSet = (RowSetCollection) null;
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetStoredQuery: Url={0}. queryid={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) queryId);
      try
      {
        this.IncRequestCount();
        clientServiceProxy.GetStoredQuery(queryId, out queryDataSet);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.ProxyGetQueryEnd);
    }

    public void GetStoredQueries(
      string requestId,
      bool useMaster,
      long rowVersion,
      int projectId,
      out RowSetCollection queriesDataSet)
    {
      Marker.Process(Mark.ProxyGetQueriesBegin);
      this.ValidateRequestId(requestId);
      queriesDataSet = (RowSetCollection) null;
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetStoredQueries: Url={0}. projectid={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) projectId);
      try
      {
        this.IncRequestCount();
        clientServiceProxy.GetStoredQueries(rowVersion, projectId, out queriesDataSet);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.ProxyGetQueriesEnd);
    }

    public void GetStoredQueryItems(
      string requestId,
      long rowVersion,
      int projectId,
      out RowSetCollection queriesDataSet)
    {
      Marker.Process(Mark.ProxyGetQueriesBegin);
      this.ValidateRequestId(requestId);
      queriesDataSet = (RowSetCollection) null;
      if (!(this.GetClientServiceProxy(requestId) is ClientService2 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetStoredQueryItems: Url={0}. projectid={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) projectId);
      try
      {
        this.IncRequestCount();
        clientServiceProxy.GetStoredQueryItems(rowVersion, projectId, out queriesDataSet);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.ProxyGetQueriesEnd);
    }

    public IEnumerable<WorkItemId> GetDestroyedWorkItemIds(string requestId, long rowVersion)
    {
      Marker.Process(Mark.QueryBegin);
      if (!(this.GetClientServiceProxy(requestId) is ClientService3 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetDestroyedWorkItems");
      IEnumerable<WorkItemId> destroyedWorkItemIds;
      try
      {
        this.IncRequestCount();
        destroyedWorkItemIds = (IEnumerable<WorkItemId>) clientServiceProxy.GetDestroyedWorkItemIds(rowVersion);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.QueryEnd);
      return destroyedWorkItemIds;
    }

    public IEnumerable<WorkItemId> GetChangedWorkItemIds(string requestId, long rowVersion)
    {
      Marker.Process(Mark.QueryBegin);
      if (!(this.GetClientServiceProxy(requestId) is ClientService3 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetChangedWorkItemIds");
      IEnumerable<WorkItemId> changedWorkItemIds;
      try
      {
        this.IncRequestCount();
        changedWorkItemIds = (IEnumerable<WorkItemId>) clientServiceProxy.GetChangedWorkItemIds(rowVersion);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.QueryEnd);
      return changedWorkItemIds;
    }

    public IEnumerable<WorkItemLinkChange> GetWorkItemLinkChanges(string requestId, long rowVersion)
    {
      Marker.Process(Mark.QueryBegin);
      if (!(this.GetClientServiceProxy(requestId) is ClientService3 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetWorkItemLinkChanges");
      IEnumerable<WorkItemLinkChange> workItemLinkChanges;
      try
      {
        this.IncRequestCount();
        workItemLinkChanges = (IEnumerable<WorkItemLinkChange>) clientServiceProxy.GetWorkItemLinkChanges(rowVersion);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.QueryEnd);
      return workItemLinkChanges;
    }

    public ExtendedAccessControlListData GetStoredQueryItemAccessControlList(
      string requestId,
      Guid queryItemId,
      bool getMetadata)
    {
      Marker.Process(Mark.ProxyGetQueryACLBegin);
      this.ValidateRequestId(requestId);
      if (!(this.GetClientServiceProxy(requestId) is ClientService3 clientServiceProxy))
        throw new NotSupportedException();
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.GetStoredQueryItemAccessControlList: Url={0}. queryid={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) queryItemId);
      ExtendedAccessControlListData accessControlList;
      try
      {
        this.IncRequestCount();
        accessControlList = clientServiceProxy.GetStoredQueryItemAccessControlList(queryItemId, getMetadata);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.ProxyGetQueryACLEnd);
      return accessControlList;
    }

    public void RequestCancel(string requestId, string cancelRequestId)
    {
      Marker.Process(Mark.ProxyRequestCancelBegin);
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.RequestCancel: Url={0}.", (object) clientServiceProxy.Url.AbsoluteUri);
      try
      {
        this.IncRequestCount();
        clientServiceProxy.RequestCancel(cancelRequestId);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      Marker.Process(Mark.ProxyRequestCancelEnd);
    }

    public void SyncExternalStructures(string requestId, string projectURI)
    {
      Marker.Process(Mark.ProxySyncExternalStructuresBegin);
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      clientServiceProxy.SetTimeout(3600000);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.SyncExternalStructures: Url={0}. projectUri={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) projectURI);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.Deadlock);
      do
      {
        try
        {
          clientServiceProxy.SyncExternalStructures(projectURI);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      Marker.Process(Mark.ProxySyncExternalStructuresEnd);
    }

    public void SyncBisGroupsAndUsers(string requestId, string projectUri)
    {
      Marker.Process(Mark.ProxySyncBisGroupsAndUsersBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "-->Proxies.ClientService::SyncBisGroupsAndUsers");
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      clientServiceProxy.SetTimeout(3600000);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Inside Proxy.SyncBisGroupsAndUsers: Url={0}. projectUri={1}", (object) clientServiceProxy.Url.AbsoluteUri, (object) projectUri);
      RetryHandler retryHandler = new RetryHandler(RetryTypes.Deadlock);
      do
      {
        try
        {
          this.IncRequestCount();
          TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Calling SyncBisGroupsAndUsers.");
          clientServiceProxy.SyncBisGroupsAndUsers(projectUri);
          break;
        }
        catch (SoapException ex)
        {
          retryHandler.HandleSoapException(ex);
        }
        catch (Exception ex)
        {
          this.TraceException(ex);
          throw;
        }
      }
      while (retryHandler.HasRemainingRetries);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "<--Proxies.ClientService::SyncBisGroupsAndUsers");
      Marker.Process(Mark.ProxySyncBisGroupsAndUsersEnd);
    }

    public string[] GetReferencingWorkitemUris(string requestId, string artifactUri)
    {
      Marker.Process(Mark.ProxyGetReferencingWorkitemUrisBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "-->Proxies.ClientService::GetReferencingWorkitemUris");
      this.ValidateRequestId(requestId);
      ClientService clientServiceProxy = this.GetClientServiceProxy(requestId);
      try
      {
        this.IncRequestCount();
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "--> Calling Proxies.ClientService::GetReferencingWorkitemUris");
        return clientServiceProxy.GetReferencingWorkitemUris(artifactUri);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      finally
      {
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "<--Proxies.ClientService::GetReferencingWorkitemUris");
        Marker.Process(Mark.ProxyGetReferencingWorkitemUrisEnd);
      }
    }

    public IResultCollection<ArtifactWorkItemIds> GetWorkItemIdsForArtifactUris(
      string[] artifactUris,
      DateTime? asOfDate)
    {
      Marker.Process(Mark.ProxyGetQueryACLBegin);
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "-->Proxies.ClientService::GetWorkItemIdsForArtifactUris");
      ClientServiceStreamingProxy streamingProxy = this.GetStreamingProxy();
      try
      {
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "--> Calling Proxies.ClientService::GetWorkItemIdsForArtifactUris");
        IResultCollection<ArtifactWorkItemIds> workItemIds;
        streamingProxy.GetWorkItemIdsForArtifactUris((IClientContext) new ClientContext(), artifactUris, asOfDate, out workItemIds);
        return workItemIds;
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      finally
      {
        Marker.Process(Mark.ProxyGetQueryACLEnd);
      }
    }

    public void DestroyAttachments(string requestId, int[] workItemIds)
    {
      this.ValidateRequestId(requestId);
      ArgumentUtility.CheckForNull<int[]>(workItemIds, nameof (workItemIds));
      TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "-->Proxies.ClientService5::DestroyAttachments");
      try
      {
        if (workItemIds.Length == 0)
          return;
        if (!(this.GetClientServiceProxy(requestId) is ClientService5 clientServiceProxy))
          throw new NotSupportedException();
        clientServiceProxy.SetTimeout(3600000);
        this.IncRequestCount();
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "--> Calling Proxies.ClientService5::DestroyAttachments");
        clientServiceProxy.DestroyAttachments(workItemIds);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        throw;
      }
      finally
      {
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "<--Proxies.ClientService5::DestroyAttachments");
      }
    }

    public static string NewRequestId() => "uuid:" + Guid.NewGuid().ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);

    private void ValidateRequestId(string requestId)
    {
      if (requestId == null)
        throw new ArgumentNullException(nameof (requestId));
      if (requestId.Trim().Length == 0)
        throw new ArgumentException(ResourceStrings.Get("InvalidRequestId"), nameof (requestId));
    }

    private ClientServiceStreamingProxy GetStreamingProxy() => new ClientServiceStreamingProxy(this.m_tfs);

    private ConfigurationSettingsService GetConfigurationProxy() => new ConfigurationSettingsService(this.m_tfs);

    private void TraceException(Exception ex) => this.TraceException(ex, out string _);

    private void TraceException(Exception ex, out string content) => this.TraceException(ex, (ClientService) null, out content);

    private void TraceException(Exception ex, ClientService proxy, out string content)
    {
      StreamReader streamReader = (StreamReader) null;
      content = string.Empty;
      try
      {
        string str1 = string.Empty;
        string str2 = string.Empty;
        string info = string.Empty;
        if (ex is WebException webException)
        {
          str2 = " HelpLink: " + webException.HelpLink;
          if (webException.Status == WebExceptionStatus.ProtocolError)
          {
            if (webException.Response != null)
            {
              try
              {
                using (streamReader = new StreamReader(webException.Response.GetResponseStream()))
                {
                  content = streamReader.ReadToEnd();
                  info = " Response Data: " + content;
                }
              }
              catch
              {
              }
            }
          }
        }
        else if (ex is SoapException soapException)
        {
          str2 = " HelpLink: " + soapException.HelpLink;
          if (soapException.Detail != null && soapException.Detail.InnerText != null)
            info = " SoapEx Details part: " + soapException.Detail.InnerText;
        }
        if (proxy != null)
          str1 = "Target URL: " + proxy.Url.AbsoluteUri + Environment.NewLine;
        TeamFoundationTrace.Error(str1 + "Error: " + ex.Message + Environment.NewLine + str2);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Database, info);
      }
      catch
      {
      }
      finally
      {
        streamReader?.Close();
      }
    }

    private void IncRequestCount() => TeamFoundationTrace.Verbose(TraceKeywordSets.Database, "Request Count: {0}.", (object) ++WorkItemServer.m_requestCount);

    private void StoreReplicationHeaders(HttpWebResponse response)
    {
      if (response == null || response.Headers == null)
        return;
      for (int index = 0; index < response.Headers.Count; ++index)
      {
        string key = response.Headers.GetKey(index);
        if (key.StartsWith("X-TFS-Replication-", StringComparison.Ordinal))
        {
          lock (this.m_replicationHeadersLock)
          {
            string str = response.Headers.Get(index);
            this.m_replicationHeaders[key] = str;
          }
        }
      }
    }

    private void SetReplicationHeaders(HttpWebRequest request)
    {
      request.Headers.Set("X-TFS-Replication-Enabled", "1");
      if (this.m_replicationHeaders.Count == 0)
        return;
      lock (this.m_replicationHeadersLock)
      {
        foreach (KeyValuePair<string, string> replicationHeader in this.m_replicationHeaders)
          request.Headers.Set(replicationHeader.Key, replicationHeader.Value);
      }
    }

    void ITfsRequestListener.AfterReceiveReply(
      long requestId,
      string methodName,
      HttpWebResponse response)
    {
      this.StoreReplicationHeaders(response);
    }

    void ITfsRequestListener.BeforeSendRequest(
      long requestId,
      string methodName,
      HttpWebRequest request)
    {
      this.SetReplicationHeaders(request);
    }

    long ITfsRequestListener.BeginRequest() => 0;

    void ITfsRequestListener.EndRequest(long requestId, Exception exception)
    {
    }
  }
}
