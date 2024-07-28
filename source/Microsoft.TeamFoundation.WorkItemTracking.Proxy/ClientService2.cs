// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientService2 : ClientService
  {
    public ClientService2(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("7ede8c17-7965-4aee-874d-ed9b25276deb");

    protected override string ServiceType => "WorkitemService2";

    public void GetStoredQueryItems(
      long rowVersion,
      int projectId,
      out RowSetCollection queryItemsPayload)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new ClientService2.GetStoredQueryItemsClientOperation(), new object[2]
      {
        (object) rowVersion,
        (object) projectId
      }, out outputs);
      queryItemsPayload = (RowSetCollection) outputs[0];
    }

    internal sealed class GetStoredQueryItemsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetStoredQueryItems";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetStoredQueryItems";

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
        if (reader.Name == "queryItemsPayload")
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
  }
}
