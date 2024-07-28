// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientServiceStreamingProxy
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientServiceStreamingProxy : TfsHttpClient
  {
    private static readonly Guid s_collectionServiceIdentifier = new Guid("CA87FA49-58C9-4089-8535-1299FA60EEBC");

    public ClientServiceStreamingProxy(TfsTeamProjectCollection tfs)
      : base((TfsConnection) tfs)
    {
    }

    protected override string ComponentName => "ClientService";

    protected override string ServiceType => "WorkitemService3";

    protected override Guid CollectionServiceIdentifier => ServiceIdentifiers.WorkItem3;

    public void GetWorkItemIdsForArtifactUris(
      IClientContext clientContext,
      string[] artifactUris,
      DateTime? asOfDate,
      out IResultCollection<ArtifactWorkItemIds> workItemIds)
    {
      workItemIds = (IResultCollection<ArtifactWorkItemIds>) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TfsMessage message = this.Channel.Request(TfsMessage.CreateMessage("http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03/GetWorkItemIdsForArtifactUris", new TfsBodyWriter(nameof (GetWorkItemIdsForArtifactUris), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", new object[2]
      {
        (object) artifactUris,
        (object) asOfDate
      }, ClientServiceStreamingProxy.\u003C\u003EO.\u003C0\u003E__WriteParameters ?? (ClientServiceStreamingProxy.\u003C\u003EO.\u003C0\u003E__WriteParameters = new Action<XmlDictionaryWriter, object[]>(ClientServiceStreamingProxy.WriteParameters)))));
      if (message.IsFault)
      {
        Exception e = message.CreateException();
        e = e is SoapException ? this.ConvertException((SoapException) e) : throw e;
      }
      else
      {
        bool flag = true;
        XmlDictionaryReader bodyReader = (XmlDictionaryReader) null;
        try
        {
          bodyReader = message.GetBodyReader();
          int num = bodyReader.IsEmptyElement ? 1 : 0;
          bodyReader.Read();
          if (num == 0)
          {
            TfsRequestContext context = new TfsRequestContext(clientContext, (TfsHttpClientBase) this, message, bodyReader, nameof (GetWorkItemIdsForArtifactUris));
            workItemIds = (IResultCollection<ArtifactWorkItemIds>) new ResultCollection<ArtifactWorkItemIds, ArtifactWorkItemIds>(context, "artifactLinks");
          }
          flag = false;
        }
        finally
        {
          if (flag)
          {
            bodyReader?.Close();
            message?.Close();
          }
        }
      }
    }

    private static void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
    {
      string[] parameter1 = (string[]) parameters[0];
      if (parameter1 != null)
        Helper.ToXml((XmlWriter) writer, "artifactUris", parameter1);
      DateTime? parameter2 = (DateTime?) parameters[1];
      if (!parameter2.HasValue)
        return;
      XmlUtility.ToXmlElement((XmlWriter) writer, "asOfDate", parameter2.Value);
    }
  }
}
