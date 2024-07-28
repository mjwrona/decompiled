// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Net;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientService : TfsHttpClient
  {
    private int? m_timeout;
    private ITfsRequestListener m_tfsRequestlistener;

    protected override string ComponentName => nameof (ClientService);

    internal void SetTimeout(int timeout) => this.m_timeout = new int?(timeout);

    protected override TfsRequestSettings ApplyCustomSettings(TfsRequestSettings settings)
    {
      if (!this.m_timeout.HasValue)
        return base.ApplyCustomSettings(settings);
      TfsRequestSettings tfsRequestSettings = settings.Clone();
      tfsRequestSettings.SendTimeout = TimeSpan.FromMilliseconds((double) this.m_timeout.Value);
      return tfsRequestSettings;
    }

    internal void InitRequestListener(ITfsRequestListener listener)
    {
      ArgumentUtility.CheckForNull<ITfsRequestListener>(listener, nameof (listener));
      this.m_tfsRequestlistener = listener;
    }

    protected override void OnAfterReceiveReply(
      long requestId,
      string methodName,
      HttpWebResponse response)
    {
      base.OnAfterReceiveReply(requestId, methodName, response);
      if (response == null || response.StatusCode != HttpStatusCode.OK || this.m_tfsRequestlistener == null)
        return;
      this.m_tfsRequestlistener.AfterReceiveReply(requestId, methodName, response);
    }

    protected override void OnBeforeSendRequest(
      long requestId,
      string methodName,
      HttpWebRequest request)
    {
      base.OnBeforeSendRequest(requestId, methodName, request);
      if (this.m_tfsRequestlistener == null)
        return;
      this.m_tfsRequestlistener.BeforeSendRequest(requestId, methodName, request);
    }

    public ClientService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("179b6a0b-a5be-43fc-879f-cfa2a43cd3d8");

    protected override string ServiceType => "WorkitemService";

    public bool BulkUpdate(
      XmlNode package,
      out XmlNode result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out RowSetCollection metadata)
    {
      object[] outputs;
      int num = (bool) this.Invoke((TfsClientOperation) new ClientService.BulkUpdateClientOperation(), new object[2]
      {
        (object) package,
        (object) metadataHave
      }, out outputs) ? 1 : 0;
      result = (XmlNode) outputs[0];
      dbStamp = (string) outputs[1];
      metadata = (RowSetCollection) outputs[2];
      return num != 0;
    }

    public void GetMetadata(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out RowSetCollection metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetMetadataClientOperation(), new object[2]
      {
        (object) metadataHave,
        (object) useMaster
      }, out outputs);
      metadata = (RowSetCollection) outputs[0];
      dbStamp = (string) outputs[1];
      locale = (int) outputs[2];
      comparisonStyle = (int) outputs[3];
      callerIdentity = (string) outputs[4];
    }

    public void GetMetadataEx(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out RowSetCollection metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetMetadataExClientOperation(), new object[2]
      {
        (object) metadataHave,
        (object) useMaster
      }, out outputs);
      metadata = (RowSetCollection) outputs[0];
      dbStamp = (string) outputs[1];
      locale = (int) outputs[2];
      comparisonStyle = (int) outputs[3];
      callerIdentity = (string) outputs[4];
      callerIdentitySid = (string) outputs[5];
    }

    public void GetMetadataEx2(
      MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out RowSetCollection metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out int mode)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetMetadataEx2ClientOperation(), new object[2]
      {
        (object) metadataHave,
        (object) useMaster
      }, out outputs);
      metadata = (RowSetCollection) outputs[0];
      dbStamp = (string) outputs[1];
      locale = (int) outputs[2];
      comparisonStyle = (int) outputs[3];
      mode = (int) outputs[4];
    }

    public string[] GetReferencingWorkitemUris(string artifactUri) => (string[]) this.Invoke((TfsClientOperation) new ClientService.GetReferencingWorkitemUrisClientOperation(), new object[1]
    {
      (object) artifactUri
    });

    public void GetStoredQueries(
      long rowVersion,
      int projectId,
      out RowSetCollection queriesPayload)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetStoredQueriesClientOperation(), new object[2]
      {
        (object) rowVersion,
        (object) projectId
      }, out outputs);
      queriesPayload = (RowSetCollection) outputs[0];
    }

    public void GetStoredQuery(Guid queryId, out RowSetCollection queryPayload)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetStoredQueryClientOperation(), new object[1]
      {
        (object) queryId
      }, out outputs);
      queryPayload = (RowSetCollection) outputs[0];
    }

    public void GetWorkItem(
      int workItemId,
      int revisionId,
      int minimumRevisionId,
      DateTime? asOfDate,
      bool useMaster,
      out RowSetCollection workItem,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out RowSetCollection metadata)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.GetWorkItemClientOperation(), new object[6]
      {
        (object) workItemId,
        (object) revisionId,
        (object) minimumRevisionId,
        (object) asOfDate,
        (object) useMaster,
        (object) metadataHave
      }, out outputs);
      workItem = (RowSetCollection) outputs[0];
      dbStamp = (string) outputs[1];
      metadata = (RowSetCollection) outputs[2];
    }

    public string GetWorkitemTrackingVersion() => (string) this.Invoke((TfsClientOperation) new ClientService.GetWorkitemTrackingVersionClientOperation(), Array.Empty<object>());

    public void PageItemsOnBehalfOf(
      string userName,
      int[] ids,
      string[] columns,
      out RowSetCollection items)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.PageItemsOnBehalfOfClientOperation(), new object[3]
      {
        (object) userName,
        (object) ids,
        (object) columns
      }, out outputs);
      items = (RowSetCollection) outputs[0];
    }

    public void PageWorkitemsByIdRevs(
      IdRevisionPair[] pairs,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      out DateTime pageDate,
      bool useMaster,
      out RowSetCollection items)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.PageWorkitemsByIdRevsClientOperation(), new object[5]
      {
        (object) pairs,
        (object) columns,
        (object) longTextColumns,
        (object) asOfDate,
        (object) useMaster
      }, out outputs);
      pageDate = (DateTime) outputs[0];
      items = (RowSetCollection) outputs[1];
    }

    public void PageWorkitemsByIds(
      int[] ids,
      string[] columns,
      int[] longTextColumns,
      DateTime? asOfDate,
      bool useMaster,
      out RowSetCollection items,
      MetadataTableHaveEntry[] metadataHave,
      out RowSetCollection metadata)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.PageWorkitemsByIdsClientOperation(), new object[6]
      {
        (object) ids,
        (object) columns,
        (object) longTextColumns,
        (object) asOfDate,
        (object) useMaster,
        (object) metadataHave
      }, out outputs);
      items = (RowSetCollection) outputs[0];
      metadata = (RowSetCollection) outputs[1];
    }

    public void QueryWorkitemCount(
      XmlNode psQuery,
      bool useMaster,
      out int count,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out RowSetCollection metadata)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.QueryWorkitemCountClientOperation(), new object[3]
      {
        (object) psQuery,
        (object) useMaster,
        (object) metadataHave
      }, out outputs);
      count = (int) outputs[0];
      asOfDate = (DateTime) outputs[1];
      dbStamp = (string) outputs[2];
      metadata = (RowSetCollection) outputs[3];
    }

    public void QueryWorkitemCountOnBehalfOf(string userName, XmlNode query, out int count)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.QueryWorkitemCountOnBehalfOfClientOperation(), new object[2]
      {
        (object) userName,
        (object) query
      }, out outputs);
      count = (int) outputs[0];
    }

    public void QueryWorkitems(
      XmlNode psQuery,
      QuerySortOrderEntry[] sort,
      bool useMaster,
      out XmlNode resultIds,
      out DateTime asOfDate,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out RowSetCollection metadata)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.QueryWorkitemsClientOperation(), new object[4]
      {
        (object) psQuery,
        (object) sort,
        (object) useMaster,
        (object) metadataHave
      }, out outputs);
      resultIds = (XmlNode) outputs[0];
      asOfDate = (DateTime) outputs[1];
      dbStamp = (string) outputs[2];
      metadata = (RowSetCollection) outputs[3];
    }

    public void RequestCancel(string requestIdToCancel) => this.Invoke((TfsClientOperation) new ClientService.RequestCancelClientOperation(), new object[1]
    {
      (object) requestIdToCancel
    });

    public void StampWorkitemCache() => this.Invoke((TfsClientOperation) new ClientService.StampWorkitemCacheClientOperation(), Array.Empty<object>());

    public void SyncAccessControlLists(string projectURI) => this.Invoke((TfsClientOperation) new ClientService.SyncAccessControlListsClientOperation(), new object[1]
    {
      (object) projectURI
    });

    public void SyncBisGroupsAndUsers(string projectUri) => this.Invoke((TfsClientOperation) new ClientService.SyncBisGroupsAndUsersClientOperation(), new object[1]
    {
      (object) projectUri
    });

    public void SyncExternalStructures(string projectURI) => this.Invoke((TfsClientOperation) new ClientService.SyncExternalStructuresClientOperation(), new object[1]
    {
      (object) projectURI
    });

    public void Update(
      XmlNode package,
      out XmlNode result,
      MetadataTableHaveEntry[] metadataHave,
      out string dbStamp,
      out RowSetCollection metadata)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService.UpdateClientOperation(), new object[2]
      {
        (object) package,
        (object) metadataHave
      }, out outputs);
      result = (XmlNode) outputs[0];
      dbStamp = (string) outputs[1];
      metadata = (RowSetCollection) outputs[2];
    }

    internal sealed class BulkUpdateClientOperation : TfsClientOperation
    {
      public override string BodyName => "BulkUpdate";

      public override bool HasOutputs => true;

      public override string ResultName => "BulkUpdateResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/BulkUpdate";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[3];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) null;
        return (object) false;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "result":
            outputs[0] = (object) XmlUtility.XmlNodeFromXmlElement(reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "metadata":
            outputs[2] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        XmlNode parameter1 = (XmlNode) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "package", parameter1);
        MetadataTableHaveEntry[] parameter2 = (MetadataTableHaveEntry[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter2, false, false);
      }
    }

    internal sealed class GetMetadataClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMetadata";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetMetadata";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[5];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) 0;
        outputs[3] = (object) 0;
        outputs[4] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "metadata":
            outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "locale":
            outputs[2] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "comparisonStyle":
            outputs[3] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "callerIdentity":
            outputs[4] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        MetadataTableHaveEntry[] parameter1 = (MetadataTableHaveEntry[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter2);
      }
    }

    internal sealed class GetMetadataEx2ClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMetadataEx2";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetMetadataEx2";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[5];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) 0;
        outputs[3] = (object) 0;
        outputs[4] = (object) 0;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "metadata":
            outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "locale":
            outputs[2] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "comparisonStyle":
            outputs[3] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "mode":
            outputs[4] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        MetadataTableHaveEntry[] parameter1 = (MetadataTableHaveEntry[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter2);
      }
    }

    internal sealed class GetMetadataExClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMetadataEx";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetMetadataEx";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[6];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) 0;
        outputs[3] = (object) 0;
        outputs[4] = (object) null;
        outputs[5] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "metadata":
            outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "locale":
            outputs[2] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "comparisonStyle":
            outputs[3] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "callerIdentity":
            outputs[4] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "callerIdentitySid":
            outputs[5] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        MetadataTableHaveEntry[] parameter1 = (MetadataTableHaveEntry[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter2);
      }
    }

    internal sealed class GetReferencingWorkitemUrisClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetReferencingWorkitemUris";

      public override bool HasOutputs => true;

      public override string ResultName => "GetReferencingWorkitemUrisResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetReferencingWorkitemUris";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfString;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfStringFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "artifactUri", parameter);
      }
    }

    internal sealed class GetStoredQueriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetStoredQueries";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetStoredQueries";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[1];
        outputs[0] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        if (reader.Name == "queriesPayload")
          outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        long parameter1 = (long) parameters[0];
        if (parameter1 != 0L)
          XmlUtility.ToXmlElement((XmlWriter) writer, "rowVersion", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectId", parameter2);
      }
    }

    internal sealed class GetStoredQueryClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetStoredQuery";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetStoredQuery";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[1];
        outputs[0] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        if (reader.Name == "queryPayload")
          outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryId", parameter);
      }
    }

    internal sealed class GetWorkItemClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetWorkItem";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetWorkItem";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[3];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "workItem":
            outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "metadata":
            outputs[2] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter1 = (int) parameters[0];
        if (parameter1 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "workItemId", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "revisionId", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "minimumRevisionId", parameter3);
        DateTime? parameter4 = (DateTime?) parameters[3];
        if (parameter4.HasValue)
          XmlUtility.ToXmlElement((XmlWriter) writer, "asOfDate", parameter4.Value);
        bool parameter5 = (bool) parameters[4];
        if (parameter5)
          XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter5);
        MetadataTableHaveEntry[] parameter6 = (MetadataTableHaveEntry[]) parameters[5];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter6, false, false);
      }
    }

    internal sealed class GetWorkitemTrackingVersionClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetWorkitemTrackingVersion";

      public override bool HasOutputs => true;

      public override string ResultName => "GetWorkitemTrackingVersionResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetWorkitemTrackingVersion";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);
    }

    internal sealed class PageItemsOnBehalfOfClientOperation : TfsClientOperation
    {
      public override string BodyName => "PageItemsOnBehalfOf";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/PageItemsOnBehalfOf";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[1];
        outputs[0] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        if (reader.Name == "items")
          outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userName", parameter1);
        int[] parameter2 = (int[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "ids", parameter2, false, false);
        string[] parameter3 = (string[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "columns", parameter3, false, false);
      }
    }

    internal sealed class PageWorkitemsByIdRevsClientOperation : TfsClientOperation
    {
      public override string BodyName => "PageWorkitemsByIdRevs";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/PageWorkitemsByIdRevs";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[2];
        outputs[0] = (object) DateTime.MinValue;
        outputs[1] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "pageDate":
            outputs[0] = (object) XmlUtility.DateTimeFromXmlElement(reader);
            break;
          case "items":
            outputs[1] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdRevisionPair[] parameter1 = (IdRevisionPair[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "pairs", parameter1, false, false);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "columns", parameter2, false, false);
        int[] parameter3 = (int[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "longTextColumns", parameter3, false, false);
        DateTime? parameter4 = (DateTime?) parameters[3];
        if (parameter4.HasValue)
          XmlUtility.ToXmlElement((XmlWriter) writer, "asOfDate", parameter4.Value);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter5);
      }
    }

    internal sealed class PageWorkitemsByIdsClientOperation : TfsClientOperation
    {
      public override string BodyName => "PageWorkitemsByIds";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/PageWorkitemsByIds";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[2];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "items":
            outputs[0] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          case "metadata":
            outputs[1] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int[] parameter1 = (int[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "ids", parameter1, false, false);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "columns", parameter2, false, false);
        int[] parameter3 = (int[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "longTextColumns", parameter3, false, false);
        DateTime? parameter4 = (DateTime?) parameters[3];
        if (parameter4.HasValue)
          XmlUtility.ToXmlElement((XmlWriter) writer, "asOfDate", parameter4.Value);
        bool parameter5 = (bool) parameters[4];
        if (parameter5)
          XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter5);
        MetadataTableHaveEntry[] parameter6 = (MetadataTableHaveEntry[]) parameters[5];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter6, false, false);
      }
    }

    internal sealed class QueryWorkitemCountClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryWorkitemCount";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/QueryWorkitemCount";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[4];
        outputs[0] = (object) 0;
        outputs[1] = (object) DateTime.MinValue;
        outputs[2] = (object) null;
        outputs[3] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "count":
            outputs[0] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "asOfDate":
            outputs[1] = (object) XmlUtility.DateTimeFromXmlElement(reader);
            break;
          case "dbStamp":
            outputs[2] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "metadata":
            outputs[3] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        XmlNode parameter1 = (XmlNode) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "psQuery", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (parameter2)
          XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter2);
        MetadataTableHaveEntry[] parameter3 = (MetadataTableHaveEntry[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter3, false, false);
      }
    }

    internal sealed class QueryWorkitemCountOnBehalfOfClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryWorkitemCountOnBehalfOf";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/QueryWorkitemCountOnBehalfOf";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[1];
        outputs[0] = (object) 0;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        if (reader.Name == "count")
          outputs[0] = (object) XmlUtility.Int32FromXmlElement(reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userName", parameter1);
        XmlNode parameter2 = (XmlNode) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "query", parameter2);
      }
    }

    internal sealed class QueryWorkitemsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryWorkitems";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/QueryWorkitems";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[4];
        outputs[0] = (object) null;
        outputs[1] = (object) DateTime.MinValue;
        outputs[2] = (object) null;
        outputs[3] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "resultIds":
            outputs[0] = (object) XmlUtility.XmlNodeFromXmlElement(reader);
            break;
          case "asOfDate":
            outputs[1] = (object) XmlUtility.DateTimeFromXmlElement(reader);
            break;
          case "dbStamp":
            outputs[2] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "metadata":
            outputs[3] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        XmlNode parameter1 = (XmlNode) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "psQuery", parameter1);
        QuerySortOrderEntry[] parameter2 = (QuerySortOrderEntry[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "sort", parameter2, false, false);
        bool parameter3 = (bool) parameters[2];
        if (parameter3)
          XmlUtility.ToXmlElement((XmlWriter) writer, "useMaster", parameter3);
        MetadataTableHaveEntry[] parameter4 = (MetadataTableHaveEntry[]) parameters[3];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter4, false, false);
      }
    }

    internal sealed class RequestCancelClientOperation : TfsClientOperation
    {
      public override string BodyName => "RequestCancel";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/RequestCancel";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "requestIdToCancel", parameter);
      }
    }

    internal sealed class StampWorkitemCacheClientOperation : TfsClientOperation
    {
      public override string BodyName => "StampWorkitemCache";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/StampWorkitemCache";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";
    }

    internal sealed class SyncAccessControlListsClientOperation : TfsClientOperation
    {
      public override string BodyName => "SyncAccessControlLists";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/SyncAccessControlLists";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectURI", parameter);
      }
    }

    internal sealed class SyncBisGroupsAndUsersClientOperation : TfsClientOperation
    {
      public override string BodyName => "SyncBisGroupsAndUsers";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/SyncBisGroupsAndUsers";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class SyncExternalStructuresClientOperation : TfsClientOperation
    {
      public override string BodyName => "SyncExternalStructures";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/SyncExternalStructures";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectURI", parameter);
      }
    }

    internal sealed class UpdateClientOperation : TfsClientOperation
    {
      public override string BodyName => "Update";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/Update";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[3];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) null;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "result":
            outputs[0] = (object) XmlUtility.XmlNodeFromXmlElement(reader);
            break;
          case "dbStamp":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "metadata":
            outputs[2] = (object) RowSetCollection.FromXml(serviceProvider, reader);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        XmlNode parameter1 = (XmlNode) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "package", parameter1);
        MetadataTableHaveEntry[] parameter2 = (MetadataTableHaveEntry[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "metadataHave", parameter2, false, false);
      }
    }
  }
}
