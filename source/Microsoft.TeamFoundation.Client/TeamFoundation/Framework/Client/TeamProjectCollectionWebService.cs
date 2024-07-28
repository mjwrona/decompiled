// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamProjectCollectionWebService
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
  internal class TeamProjectCollectionWebService : TfsHttpClient
  {
    public TeamProjectCollectionProperties[] GetCollectionProperties(
      IEnumerable<Guid> ids,
      ServiceHostFilterFlags filterFlags)
    {
      return this.GetCollectionProperties(ids, (int) filterFlags);
    }

    public TeamProjectCollectionWebService(TfsConfigurationServer connection)
      : base((TfsConnection) connection)
    {
    }

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("f358df00-1881-4db1-bb56-73ed3300cf38");

    protected override string ServiceType => "TeamProjectCollectionService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public TeamProjectCollectionProperties[] GetCollectionProperties(
      IEnumerable<Guid> ids,
      int filterFlags)
    {
      return (TeamProjectCollectionProperties[]) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.GetCollectionPropertiesClientOperation(), new object[2]
      {
        (object) ids,
        (object) filterFlags
      });
    }

    public Guid GetDefaultCollectionId() => (Guid) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.GetDefaultCollectionIdClientOperation(), Array.Empty<object>());

    public ServicingJobDetail QueueAttachCollection(
      TeamProjectCollectionProperties collectionProperties,
      bool cloneCollection)
    {
      return (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueAttachCollectionClientOperation(), new object[2]
      {
        (object) collectionProperties,
        (object) cloneCollection
      });
    }

    public ServicingJobDetail QueueCreateCollection(
      TeamProjectCollectionProperties collectionProperties,
      string dataTierConnectionString)
    {
      return (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueCreateCollectionClientOperation(), new object[2]
      {
        (object) collectionProperties,
        (object) dataTierConnectionString
      });
    }

    public ServicingJobDetail QueueDeleteCollection(Guid collectionId) => (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueDeleteCollectionClientOperation(), new object[1]
    {
      (object) collectionId
    });

    public ServicingJobDetail QueueDeleteProject(
      TeamProjectCollectionProperties collectionProperties,
      string projectUri)
    {
      return (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueDeleteProjectClientOperation(), new object[2]
      {
        (object) collectionProperties,
        (object) projectUri
      });
    }

    public ServicingJobDetail QueueDetachCollection(
      TeamProjectCollectionProperties collectionProperties,
      string collectionStoppedMessage,
      out string detachedConnectionString)
    {
      object[] outputs;
      ServicingJobDetail servicingJobDetail = (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueDetachCollectionClientOperation(), new object[2]
      {
        (object) collectionProperties,
        (object) collectionStoppedMessage
      }, out outputs);
      detachedConnectionString = (string) outputs[0];
      return servicingJobDetail;
    }

    public ServicingJobDetail QueueUpdateCollection(
      TeamProjectCollectionProperties collectionProperties)
    {
      return (ServicingJobDetail) this.Invoke((TfsClientOperation) new TeamProjectCollectionWebService.QueueUpdateCollectionClientOperation(), new object[1]
      {
        (object) collectionProperties
      });
    }

    internal sealed class GetCollectionPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetCollectionProperties";

      public override bool HasOutputs => true;

      public override string ResultName => "GetCollectionPropertiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetCollectionProperties";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamProjectCollectionProperties;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamProjectCollectionPropertiesFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter1 = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "ids", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "filterFlags", parameter2);
      }
    }

    internal sealed class GetDefaultCollectionIdClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetDefaultCollectionId";

      public override bool HasOutputs => true;

      public override string ResultName => "GetDefaultCollectionIdResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetDefaultCollectionId";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Guid.Empty;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.GuidFromXmlElement(reader);
    }

    internal sealed class QueueAttachCollectionClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueAttachCollection";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueAttachCollectionResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueAttachCollection";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        TeamProjectCollectionProperties parameter1 = (TeamProjectCollectionProperties) parameters[0];
        if (parameter1 != null)
          TeamProjectCollectionProperties.ToXml((XmlWriter) writer, "collectionProperties", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "cloneCollection", parameter2);
      }
    }

    internal sealed class QueueCreateCollectionClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueCreateCollection";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueCreateCollectionResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueCreateCollection";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        TeamProjectCollectionProperties parameter1 = (TeamProjectCollectionProperties) parameters[0];
        if (parameter1 != null)
          TeamProjectCollectionProperties.ToXml((XmlWriter) writer, "collectionProperties", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "dataTierConnectionString", parameter2);
      }
    }

    internal sealed class QueueDeleteCollectionClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueDeleteCollection";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueDeleteCollectionResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueDeleteCollection";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "collectionId", parameter);
      }
    }

    internal sealed class QueueDeleteProjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueDeleteProject";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueDeleteProjectResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueDeleteProject";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        TeamProjectCollectionProperties parameter1 = (TeamProjectCollectionProperties) parameters[0];
        if (parameter1 != null)
          TeamProjectCollectionProperties.ToXml((XmlWriter) writer, "collectionProperties", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter2);
      }
    }

    internal sealed class QueueDetachCollectionClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueDetachCollection";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueDetachCollectionResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueDetachCollection";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

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
        if (reader.Name == "detachedConnectionString")
          outputs[0] = (object) XmlUtility.StringFromXmlElement(reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        TeamProjectCollectionProperties parameter1 = (TeamProjectCollectionProperties) parameters[0];
        if (parameter1 != null)
          TeamProjectCollectionProperties.ToXml((XmlWriter) writer, "collectionProperties", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "collectionStoppedMessage", parameter2);
      }
    }

    internal sealed class QueueUpdateCollectionClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueUpdateCollection";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueUpdateCollectionResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueUpdateCollection";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServicingJobDetail.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        TeamProjectCollectionProperties parameter = (TeamProjectCollectionProperties) parameters[0];
        if (parameter == null)
          return;
        TeamProjectCollectionProperties.ToXml((XmlWriter) writer, "collectionProperties", parameter);
      }
    }
  }
}
