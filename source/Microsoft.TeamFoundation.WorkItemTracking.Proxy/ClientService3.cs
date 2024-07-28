// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientService3 : ClientService2
  {
    public new void GetMetadata(
      [XmlArrayItem(IsNullable = false)] MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out RowSetCollection metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity)
    {
      throw new NotSupportedException();
    }

    public new void GetMetadataEx(
      [XmlArrayItem(IsNullable = false)] MetadataTableHaveEntry[] metadataHave,
      bool useMaster,
      out RowSetCollection metadata,
      out string dbStamp,
      out int locale,
      out int comparisonStyle,
      out string callerIdentity,
      out string callerIdentitySid)
    {
      throw new NotSupportedException();
    }

    public ClientService3(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("ca87fa49-58c9-4089-8535-1299fa60eebc");

    protected override string ServiceType => "WorkitemService3";

    public WorkItemId[] GetChangedWorkItemIds(long rowVersion) => (WorkItemId[]) this.Invoke((TfsClientOperation) new ClientService3.GetChangedWorkItemIdsClientOperation(), new object[1]
    {
      (object) rowVersion
    });

    public WorkItemId[] GetDestroyedWorkItemIds(long rowVersion) => (WorkItemId[]) this.Invoke((TfsClientOperation) new ClientService3.GetDestroyedWorkItemIdsClientOperation(), new object[1]
    {
      (object) rowVersion
    });

    public ExtendedAccessControlListData GetStoredQueryItemAccessControlList(
      Guid queryItemId,
      bool getMetadata)
    {
      return (ExtendedAccessControlListData) this.Invoke((TfsClientOperation) new ClientService3.GetStoredQueryItemAccessControlListClientOperation(), new object[2]
      {
        (object) queryItemId,
        (object) getMetadata
      });
    }

    public WorkItemLinkChange[] GetWorkItemLinkChanges(long rowVersion) => (WorkItemLinkChange[]) this.Invoke((TfsClientOperation) new ClientService3.GetWorkItemLinkChangesClientOperation(), new object[1]
    {
      (object) rowVersion
    });

    internal sealed class GetChangedWorkItemIdsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetChangedWorkItemIds";

      public override bool HasOutputs => true;

      public override string ResultName => "GetChangedWorkItemIdsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetChangedWorkItemIds";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfWorkItemId;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfWorkItemIdFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        long parameter = (long) parameters[0];
        if (parameter == 0L)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "rowVersion", parameter);
      }
    }

    internal sealed class GetDestroyedWorkItemIdsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetDestroyedWorkItemIds";

      public override bool HasOutputs => true;

      public override string ResultName => "GetDestroyedWorkItemIdsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetDestroyedWorkItemIds";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfWorkItemId;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfWorkItemIdFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        long parameter = (long) parameters[0];
        if (parameter == 0L)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "rowVersion", parameter);
      }
    }

    internal sealed class GetStoredQueryItemAccessControlListClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetStoredQueryItemAccessControlList";

      public override bool HasOutputs => true;

      public override string ResultName => "GetStoredQueryItemAccessControlListResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetStoredQueryItemAccessControlList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ExtendedAccessControlListData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryItemId", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "getMetadata", parameter2);
      }
    }

    internal sealed class GetWorkItemLinkChangesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetWorkItemLinkChanges";

      public override bool HasOutputs => true;

      public override string ResultName => "GetWorkItemLinkChangesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetWorkItemLinkChanges";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfWorkItemLinkChange;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfWorkItemLinkChangeFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        long parameter = (long) parameters[0];
        if (parameter == 0L)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "rowVersion", parameter);
      }
    }
  }
}
