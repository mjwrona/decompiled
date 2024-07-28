// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlWebService
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
  internal class AccessControlWebService : TfsHttpClient
  {
    public AccessControlWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("8eface33-7d7d-4d89-bf7d-849b8aa2cbdb");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("8eface33-7d7d-4d89-bf7d-849b8aa2cbdb");

    protected override string ServiceType => "AccessControlService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public void DeleteServiceIdentity(Guid serviceIdentityId) => this.Invoke((TfsClientOperation) new AccessControlWebService.DeleteServiceIdentityClientOperation(), new object[1]
    {
      (object) serviceIdentityId
    });

    public ServiceIdentity ProvisionServiceIdentity(
      ServiceIdentityInfo identityInfo,
      IdentityDescriptor[] addToGroups)
    {
      return (ServiceIdentity) this.Invoke((TfsClientOperation) new AccessControlWebService.ProvisionServiceIdentityClientOperation(), new object[2]
      {
        (object) identityInfo,
        (object) addToGroups
      });
    }

    public ServiceIdentity[] QueryServiceIdentities(
      string[] serviceIdentityNames,
      bool includeMemberships)
    {
      return (ServiceIdentity[]) this.Invoke((TfsClientOperation) new AccessControlWebService.QueryServiceIdentitiesClientOperation(), new object[2]
      {
        (object) serviceIdentityNames,
        (object) includeMemberships
      });
    }

    public ServiceIdentity[] QueryServiceIdentitiesById(
      Guid[] serviceIdentityIds,
      bool includeMemberships)
    {
      return (ServiceIdentity[]) this.Invoke((TfsClientOperation) new AccessControlWebService.QueryServiceIdentitiesByIdClientOperation(), new object[2]
      {
        (object) serviceIdentityIds,
        (object) includeMemberships
      });
    }

    internal sealed class DeleteServiceIdentityClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteServiceIdentity";

      public override string SoapAction => "http://microsoft.com/webservices/DeleteServiceIdentity";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "serviceIdentityId", parameter);
      }
    }

    internal sealed class ProvisionServiceIdentityClientOperation : TfsClientOperation
    {
      public override string BodyName => "ProvisionServiceIdentity";

      public override bool HasOutputs => true;

      public override string ResultName => "ProvisionServiceIdentityResult";

      public override string SoapAction => "http://microsoft.com/webservices/ProvisionServiceIdentity";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ServiceIdentity.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ServiceIdentityInfo parameter1 = (ServiceIdentityInfo) parameters[0];
        if (parameter1 != null)
          ServiceIdentityInfo.ToXml((XmlWriter) writer, "identityInfo", parameter1);
        IdentityDescriptor[] parameter2 = (IdentityDescriptor[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "addToGroups", parameter2, false, false);
      }
    }

    internal sealed class QueryServiceIdentitiesByIdClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryServiceIdentitiesById";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryServiceIdentitiesByIdResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryServiceIdentitiesById";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfServiceIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfServiceIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid[] parameter1 = (Guid[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "serviceIdentityIds", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "includeMemberships", parameter2);
      }
    }

    internal sealed class QueryServiceIdentitiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryServiceIdentities";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryServiceIdentitiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryServiceIdentities";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfServiceIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfServiceIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "serviceIdentityNames", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "includeMemberships", parameter2);
      }
    }
  }
}
