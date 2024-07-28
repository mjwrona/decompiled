// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.PropertyWebService
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
  internal class PropertyWebService : TfsHttpClient
  {
    public PropertyWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("dfaddbda-1beb-425e-9f9f-41222287a177");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("ff4e1b3c-6351-4b9f-aa54-29aa9985cb77");

    protected override string ServiceType => "PropertyService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public ArtifactPropertyValue[] GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters,
      int options)
    {
      return (ArtifactPropertyValue[]) this.Invoke((TfsClientOperation) new PropertyWebService.GetPropertiesClientOperation(), new object[3]
      {
        (object) artifactSpecs,
        (object) propertyNameFilters,
        (object) options
      });
    }

    public void SetProperties(ArtifactPropertyValue[] artifactPropertyValues) => this.Invoke((TfsClientOperation) new PropertyWebService.SetPropertiesClientOperation(), new object[1]
    {
      (object) artifactPropertyValues
    });

    internal sealed class GetPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetProperties";

      public override bool HasOutputs => true;

      public override string ResultName => "GetPropertiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetProperties";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfArtifactPropertyValue;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfArtifactPropertyValueFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ArtifactSpec[] parameter1 = (ArtifactSpec[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "artifactSpecs", parameter1, false, false);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "propertyNameFilters", parameter2, false, false);
        int parameter3 = (int) parameters[2];
        if (parameter3 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "options", parameter3);
      }
    }

    internal sealed class SetPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetProperties";

      public override string SoapAction => "http://microsoft.com/webservices/SetProperties";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        ArtifactPropertyValue[] parameter = (ArtifactPropertyValue[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "artifactPropertyValues", parameter, false, false);
      }
    }
  }
}
