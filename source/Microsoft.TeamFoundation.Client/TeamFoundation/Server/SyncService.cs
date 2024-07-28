// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncService
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

namespace Microsoft.TeamFoundation.Server
{
  internal class SyncService : TfsHttpClient, ISyncService
  {
    public SyncService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("79aad85b-4747-48bc-a77b-d69eeda41ac9");

    protected override string ComponentName => "vstfs";

    protected override string ServiceType => nameof (SyncService);

    public SyncMapping GetSyncMapping(Guid serverId) => (SyncMapping) this.Invoke((TfsClientOperation) new SyncService.GetSyncMappingClientOperation(), new object[1]
    {
      (object) serverId
    });

    public bool GetSyncMappingChange(Guid serverId, int baselineRev, out SyncMapping mapping)
    {
      object[] outputs;
      int num = (bool) this.Invoke((TfsClientOperation) new SyncService.GetSyncMappingChangeClientOperation(), new object[2]
      {
        (object) serverId,
        (object) baselineRev
      }, out outputs) ? 1 : 0;
      mapping = (SyncMapping) outputs[0];
      return num != 0;
    }

    public SyncProperty[] GetSyncProperties(Guid serverId) => (SyncProperty[]) this.Invoke((TfsClientOperation) new SyncService.GetSyncPropertiesClientOperation(), new object[1]
    {
      (object) serverId
    });

    public SyncProperty GetSyncProperty(Guid serverId, string name) => (SyncProperty) this.Invoke((TfsClientOperation) new SyncService.GetSyncPropertyClientOperation(), new object[2]
    {
      (object) serverId,
      (object) name
    });

    public void SaveMapping(Guid serverId, int baselineRev, string mapping) => this.Invoke((TfsClientOperation) new SyncService.SaveMappingClientOperation(), new object[3]
    {
      (object) serverId,
      (object) baselineRev,
      (object) mapping
    });

    public void SaveSyncProperty(Guid serverId, int baselineRev, string name, string value) => this.Invoke((TfsClientOperation) new SyncService.SaveSyncPropertyClientOperation(), new object[4]
    {
      (object) serverId,
      (object) baselineRev,
      (object) name,
      (object) value
    });

    internal SyncService(TfsTeamProjectCollection tfsObject, string url)
      : base((TfsConnection) tfsObject, new Uri(url))
    {
    }

    protected override Exception ConvertException(SoapException e) => SoapExceptionUtilities.ConvertToStronglyTypedException(e);

    internal sealed class GetSyncMappingChangeClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetSyncMappingChange";

      public override bool HasOutputs => true;

      public override string ResultName => "GetSyncMappingChangeResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/GetSyncMappingChange";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[1];
        outputs[0] = (object) null;
        return (object) false;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        if (reader.Name == "mapping")
          outputs[0] = (object) SyncMapping.FromXml(serviceProvider, reader);
        else
          base.ReadOutput(serviceProvider, reader, outputs);
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "baselineRev", parameter2);
      }
    }

    internal sealed class GetSyncMappingClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetSyncMapping";

      public override bool HasOutputs => true;

      public override string ResultName => "GetSyncMappingResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/GetSyncMapping";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) SyncMapping.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter);
      }
    }

    internal sealed class GetSyncPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetSyncProperties";

      public override bool HasOutputs => true;

      public override string ResultName => "GetSyncPropertiesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/GetSyncProperties";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfSyncProperty;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfSyncPropertyFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter);
      }
    }

    internal sealed class GetSyncPropertyClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetSyncProperty";

      public override bool HasOutputs => true;

      public override string ResultName => "GetSyncPropertyResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/GetSyncProperty";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) SyncProperty.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter2);
      }
    }

    internal sealed class SaveMappingClientOperation : TfsClientOperation
    {
      public override string BodyName => "SaveMapping";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/SaveMapping";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "baselineRev", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "mapping", parameter3);
      }
    }

    internal sealed class SaveSyncPropertyClientOperation : TfsClientOperation
    {
      public override string BodyName => "SaveSyncProperty";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03/SaveSyncProperty";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "serverId", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "baselineRev", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter3);
        string parameter4 = (string) parameters[3];
        if (parameter4 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "value", parameter4);
      }
    }
  }
}
