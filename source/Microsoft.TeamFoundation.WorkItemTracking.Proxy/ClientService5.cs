// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService5
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
  internal class ClientService5 : ClientService4
  {
    public ClientService5(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("4c5eb288-4c0a-4888-bb1b-742a4b5b706e");

    protected override string ServiceType => "WorkitemService5";

    public void DestroyAttachments(int[] workItemIds) => this.Invoke((TfsClientOperation) new ClientService5.DestroyAttachmentsClientOperation(), new object[1]
    {
      (object) workItemIds
    });

    public ConstantRecord[] GetConstantRecords(
      string[] searchValues,
      ConstantRecordSearchFactor searchFactor)
    {
      return (ConstantRecord[]) this.Invoke((TfsClientOperation) new ClientService5.GetConstantRecordsClientOperation(), new object[2]
      {
        (object) searchValues,
        (object) searchFactor
      });
    }

    internal sealed class DestroyAttachmentsClientOperation : TfsClientOperation
    {
      public override string BodyName => "DestroyAttachments";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/DestroyAttachments";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int[] parameter = (int[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "workItemIds", parameter, false, false);
      }
    }

    internal sealed class GetConstantRecordsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetConstantRecords";

      public override bool HasOutputs => true;

      public override string ResultName => "GetConstantRecordsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetConstantRecords";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfConstantRecord;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfConstantRecordFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "searchValues", parameter1, false, false);
        ConstantRecordSearchFactor parameter2 = (ConstantRecordSearchFactor) parameters[1];
        XmlUtility.EnumToXmlElement<ConstantRecordSearchFactor>((XmlWriter) writer, "searchFactor", parameter2);
      }
    }
  }
}
