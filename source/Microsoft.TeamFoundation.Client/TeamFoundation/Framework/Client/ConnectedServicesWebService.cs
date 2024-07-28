// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectedServicesWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class ConnectedServicesWebService : TfsHttpClient
  {
    private TeamFoundationStrongBoxService m_strongBox;

    public override object GetService(Type serviceType)
    {
      if (!(serviceType == typeof (TeamFoundationStrongBoxService)))
        return base.GetService(serviceType);
      if (this.m_strongBox == null)
        this.m_strongBox = this.Connection.GetService<TeamFoundationStrongBoxService>();
      return (object) this.m_strongBox;
    }

    public ConnectedServicesWebService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("df24edb3-ce89-4907-b1a2-9041f646121e");

    protected override string ComponentName => "Framework";

    protected override string ServiceType => "ConnectedServicesService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public ConnectedServiceMetadata CreateConnectedService(
      ConnectedServiceCreationData connectedServiceCreationData)
    {
      return (ConnectedServiceMetadata) this.Invoke((TfsClientOperation) new ConnectedServicesWebService.CreateConnectedServiceClientOperation(), new object[1]
      {
        (object) connectedServiceCreationData
      });
    }

    public void DeleteConnectedService(string name, string teamProject) => this.Invoke((TfsClientOperation) new ConnectedServicesWebService.DeleteConnectedServiceClientOperation(), new object[2]
    {
      (object) name,
      (object) teamProject
    });

    public bool DoesConnectedServiceExist(string name, string teamProject) => (bool) this.Invoke((TfsClientOperation) new ConnectedServicesWebService.DoesConnectedServiceExistClientOperation(), new object[2]
    {
      (object) name,
      (object) teamProject
    });

    public ConnectedService GetConnectedService(string name, string teamProject) => (ConnectedService) this.Invoke((TfsClientOperation) new ConnectedServicesWebService.GetConnectedServiceClientOperation(), new object[2]
    {
      (object) name,
      (object) teamProject
    });

    public ConnectedServiceMetadata[] QueryConnectedServices(string teamProject) => (ConnectedServiceMetadata[]) this.Invoke((TfsClientOperation) new ConnectedServicesWebService.QueryConnectedServicesClientOperation(), new object[1]
    {
      (object) teamProject
    });

    internal sealed class CreateConnectedServiceClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateConnectedService";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateConnectedServiceResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2010/Framework/CreateConnectedService";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2010/Framework";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ConnectedServiceMetadata.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ConnectedServiceCreationData parameter = (ConnectedServiceCreationData) parameters[0];
        if (parameter == null)
          return;
        ConnectedServiceCreationData.ToXml((XmlWriter) writer, "connectedServiceCreationData", parameter);
      }
    }

    internal sealed class DeleteConnectedServiceClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteConnectedService";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2010/Framework/DeleteConnectedService";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2010/Framework";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "teamProject", parameter2);
      }
    }

    internal sealed class DoesConnectedServiceExistClientOperation : TfsClientOperation
    {
      public override string BodyName => "DoesConnectedServiceExist";

      public override bool HasOutputs => true;

      public override string ResultName => "DoesConnectedServiceExistResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2010/Framework/DoesConnectedServiceExist";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2010/Framework";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "teamProject", parameter2);
      }
    }

    internal sealed class GetConnectedServiceClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetConnectedService";

      public override bool HasOutputs => true;

      public override string ResultName => "GetConnectedServiceResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2010/Framework/GetConnectedService";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2010/Framework";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ConnectedService.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "teamProject", parameter2);
      }
    }

    internal sealed class QueryConnectedServicesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryConnectedServices";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryConnectedServicesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2010/Framework/QueryConnectedServices";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2010/Framework";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfConnectedServiceMetadata;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfConnectedServiceMetadataFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "teamProject", parameter);
      }
    }
  }
}
