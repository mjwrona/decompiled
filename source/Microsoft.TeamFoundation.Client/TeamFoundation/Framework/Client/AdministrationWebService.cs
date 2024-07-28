// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AdministrationWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class AdministrationWebService : TfsHttpClient
  {
    public AdministrationWebService(TfsConfigurationServer connection)
      : base((TfsConnection) connection)
    {
    }

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("c18d6e34-68e8-40d2-a619-e7477558976e");

    protected override string ServiceType => "AdministrationService";

    public void CancelRequest(Guid hostId, long requestId, string reason) => this.Invoke((TfsClientOperation) new AdministrationWebService.CancelRequestClientOperation(), new object[3]
    {
      (object) hostId,
      (object) requestId,
      (object) reason
    });

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public TeamFoundationServiceHostActivity[] QueryActiveRequests(
      IEnumerable<Guid> hostIds,
      bool includeDetails)
    {
      return (TeamFoundationServiceHostActivity[]) this.Invoke((TfsClientOperation) new AdministrationWebService.QueryActiveRequestsClientOperation(), new object[2]
      {
        (object) hostIds,
        (object) includeDetails
      });
    }

    internal sealed class CancelRequestClientOperation : TfsClientOperation
    {
      public override string BodyName => "CancelRequest";

      public override string SoapAction => "http://microsoft.com/webservices/CancelRequest";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "hostId", parameter1);
        long parameter2 = (long) parameters[1];
        if (parameter2 != 0L)
          XmlUtility.ToXmlElement((XmlWriter) writer, "requestId", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "reason", parameter3);
      }
    }

    internal sealed class QueryActiveRequestsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryActiveRequests";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryActiveRequestsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryActiveRequests";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationServiceHostActivity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationServiceHostActivityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter1 = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "hostIds", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "includeDetails", parameter2);
      }
    }
  }
}
