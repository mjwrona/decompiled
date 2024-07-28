// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ServerStatus
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  internal class ServerStatus : TfsHttpClient
  {
    public ServerStatus(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("d395630a-d784-45b9-b8d1-f4b82042a8d0");

    protected override string ServiceType => nameof (ServerStatus);

    public string CheckAuthentication() => (string) this.Invoke((TfsClientOperation) new ServerStatus.CheckAuthenticationClientOperation(), Array.Empty<object>());

    public DataChanged[] GetServerStatus() => (DataChanged[]) this.Invoke((TfsClientOperation) new ServerStatus.GetServerStatusClientOperation(), Array.Empty<object>());

    public string GetSupportedContractVersion() => (string) this.Invoke((TfsClientOperation) new ServerStatus.GetSupportedContractVersionClientOperation(), Array.Empty<object>());

    internal ServerStatus(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    protected override string ComponentName => nameof (ServerStatus);

    internal sealed class CheckAuthenticationClientOperation : TfsClientOperation
    {
      public override string BodyName => "CheckAuthentication";

      public override bool HasOutputs => true;

      public override string ResultName => "CheckAuthenticationResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03/CheckAuthentication";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);
    }

    internal sealed class GetServerStatusClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetServerStatus";

      public override bool HasOutputs => true;

      public override string ResultName => "GetServerStatusResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03/GetServerStatus";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfDataChanged;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfDataChangedFromXml(serviceProvider, reader, false);
    }

    internal sealed class GetSupportedContractVersionClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetSupportedContractVersion";

      public override bool HasOutputs => true;

      public override string ResultName => "GetSupportedContractVersionResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03/GetSupportedContractVersion";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);
    }
  }
}
