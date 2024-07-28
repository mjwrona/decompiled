// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class LocationWebService : TfsHttpClient
  {
    protected override Uri GetServiceLocation() => new Uri(TFCommonUtil.CombinePaths(this.Connection.Uri.ToString(), this.Connection.GetLocationServiceRelativePath()));

    public LocationWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("bf9cf1d0-24ac-4d35-aeca-6cd18c69c1fe");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("bf9cf1d0-24ac-4d35-aeca-6cd18c69c1fe");

    protected override string ServiceType => "LocationService";

    public LocationServiceData ConfigureAccessMapping(
      AccessMapping accessMapping,
      int lastChangeId,
      bool makeDefault)
    {
      return (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.ConfigureAccessMappingClientOperation(), new object[3]
      {
        (object) accessMapping,
        (object) lastChangeId,
        (object) makeDefault
      });
    }

    public ConnectionData Connect(int connectOptions, int lastChangeId, int features) => (ConnectionData) this.Invoke((TfsClientOperation) new LocationWebService.ConnectClientOperation(), new object[3]
    {
      (object) connectOptions,
      (object) lastChangeId,
      (object) features
    });

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public LocationServiceData QueryServices(
      ServiceTypeFilter[] serviceTypeFilters,
      int lastChangeId)
    {
      return (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.QueryServicesClientOperation(), new object[2]
      {
        (object) serviceTypeFilters,
        (object) lastChangeId
      });
    }

    public LocationServiceData RemoveAccessMapping(AccessMapping accessMapping, int lastChangeId) => (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.RemoveAccessMappingClientOperation(), new object[2]
    {
      (object) accessMapping,
      (object) lastChangeId
    });

    public LocationServiceData RemoveServiceDefinitions(
      ServiceDefinition[] serviceDefinitions,
      int lastChangeId)
    {
      return (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.RemoveServiceDefinitionsClientOperation(), new object[2]
      {
        (object) serviceDefinitions,
        (object) lastChangeId
      });
    }

    public LocationServiceData SaveServiceDefinitions(
      ServiceDefinition[] serviceDefinitions,
      int lastChangeId)
    {
      return (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.SaveServiceDefinitionsClientOperation(), new object[2]
      {
        (object) serviceDefinitions,
        (object) lastChangeId
      });
    }

    public LocationServiceData SetDefaultAccessMapping(
      AccessMapping accessMapping,
      int lastChangeId)
    {
      return (LocationServiceData) this.Invoke((TfsClientOperation) new LocationWebService.SetDefaultAccessMappingClientOperation(), new object[2]
      {
        (object) accessMapping,
        (object) lastChangeId
      });
    }

    internal sealed class ConfigureAccessMappingClientOperation : TfsClientOperation
    {
      public override string BodyName => "ConfigureAccessMapping";

      public override bool HasOutputs => true;

      public override string ResultName => "ConfigureAccessMappingResult";

      public override string SoapAction => "http://microsoft.com/webservices/ConfigureAccessMapping";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        AccessMapping parameter1 = (AccessMapping) parameters[0];
        if (parameter1 != null)
          AccessMapping.ToXml((XmlWriter) writer, "accessMapping", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
        bool parameter3 = (bool) parameters[2];
        if (!parameter3)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "makeDefault", parameter3);
      }
    }

    internal sealed class ConnectClientOperation : TfsClientOperation
    {
      public override string BodyName => "Connect";

      public override bool HasOutputs => true;

      public override string ResultName => "ConnectResult";

      public override string SoapAction => "http://microsoft.com/webservices/Connect";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ConnectionData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter1 = (int) parameters[0];
        if (parameter1 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "connectOptions", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter3);
      }
    }

    internal sealed class QueryServicesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryServices";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryServicesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryServices";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ServiceTypeFilter[] parameter1 = (ServiceTypeFilter[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "serviceTypeFilters", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
      }
    }

    internal sealed class RemoveAccessMappingClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveAccessMapping";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveAccessMappingResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveAccessMapping";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        AccessMapping parameter1 = (AccessMapping) parameters[0];
        if (parameter1 != null)
          AccessMapping.ToXml((XmlWriter) writer, "accessMapping", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
      }
    }

    internal sealed class RemoveServiceDefinitionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveServiceDefinitions";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveServiceDefinitionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveServiceDefinitions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ServiceDefinition[] parameter1 = (ServiceDefinition[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "serviceDefinitions", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
      }
    }

    internal sealed class SaveServiceDefinitionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "SaveServiceDefinitions";

      public override bool HasOutputs => true;

      public override string ResultName => "SaveServiceDefinitionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/SaveServiceDefinitions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ServiceDefinition[] parameter1 = (ServiceDefinition[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "serviceDefinitions", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
      }
    }

    internal sealed class SetDefaultAccessMappingClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetDefaultAccessMapping";

      public override bool HasOutputs => true;

      public override string ResultName => "SetDefaultAccessMappingResult";

      public override string SoapAction => "http://microsoft.com/webservices/SetDefaultAccessMapping";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) LocationServiceData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        AccessMapping parameter1 = (AccessMapping) parameters[0];
        if (parameter1 != null)
          AccessMapping.ToXml((XmlWriter) writer, "accessMapping", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lastChangeId", parameter2);
      }
    }
  }
}
