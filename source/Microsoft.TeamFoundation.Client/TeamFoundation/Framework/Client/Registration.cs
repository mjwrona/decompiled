// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Registration
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
  internal class Registration : TfsHttpClient
  {
    public Registration(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("b8f97328-80d2-412d-9810-67c5a3f4190f");

    protected override string ComponentName => "RegistrationService";

    protected override string ServiceType => "RegistrationService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public FrameworkRegistrationEntry[] GetRegistrationEntries(string toolId) => (FrameworkRegistrationEntry[]) this.Invoke((TfsClientOperation) new Registration.GetRegistrationEntriesClientOperation(), new object[1]
    {
      (object) toolId
    });

    internal Registration(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    internal sealed class GetRegistrationEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetRegistrationEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "GetRegistrationEntriesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03/GetRegistrationEntries";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfFrameworkRegistrationEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfFrameworkRegistrationEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "toolId", parameter);
      }
    }
  }
}
