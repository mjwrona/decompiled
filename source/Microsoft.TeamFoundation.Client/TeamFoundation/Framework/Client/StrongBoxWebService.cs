// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.StrongBoxWebService
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
  internal class StrongBoxWebService : TfsHttpClient
  {
    public StrongBoxWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("2503de38-d600-4ccc-87cf-df7ee6d00396");

    protected override string ComponentName => "Framework";

    protected override string ServiceType => "StrongBoxService";

    public void AddString(Guid drawerId, string lookupKey, string value) => this.Invoke((TfsClientOperation) new StrongBoxWebService.AddStringClientOperation(), new object[3]
    {
      (object) drawerId,
      (object) lookupKey,
      (object) value
    });

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public Guid CreateDrawer(string name) => (Guid) this.Invoke((TfsClientOperation) new StrongBoxWebService.CreateDrawerClientOperation(), new object[1]
    {
      (object) name
    });

    public void DeleteDrawer(Guid drawerId) => this.Invoke((TfsClientOperation) new StrongBoxWebService.DeleteDrawerClientOperation(), new object[1]
    {
      (object) drawerId
    });

    public void DeleteItem(Guid drawerId, string lookupKey) => this.Invoke((TfsClientOperation) new StrongBoxWebService.DeleteItemClientOperation(), new object[2]
    {
      (object) drawerId,
      (object) lookupKey
    });

    public StrongBoxItemInfo[] GetDrawerContents(Guid drawerId) => (StrongBoxItemInfo[]) this.Invoke((TfsClientOperation) new StrongBoxWebService.GetDrawerContentsClientOperation(), new object[1]
    {
      (object) drawerId
    });

    public StrongBoxItemInfo GetItemInfo(Guid drawerId, string lookupKey) => (StrongBoxItemInfo) this.Invoke((TfsClientOperation) new StrongBoxWebService.GetItemInfoClientOperation(), new object[2]
    {
      (object) drawerId,
      (object) lookupKey
    });

    public string GetString(Guid drawerId, string lookupKey) => (string) this.Invoke((TfsClientOperation) new StrongBoxWebService.GetStringClientOperation(), new object[2]
    {
      (object) drawerId,
      (object) lookupKey
    });

    public Guid UnlockDrawer(string name) => (Guid) this.Invoke((TfsClientOperation) new StrongBoxWebService.UnlockDrawerClientOperation(), new object[1]
    {
      (object) name
    });

    internal sealed class AddStringClientOperation : TfsClientOperation
    {
      public override string BodyName => "AddString";

      public override string SoapAction => "http://microsoft.com/webservices/AddString";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "lookupKey", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "value", parameter3);
      }
    }

    internal sealed class CreateDrawerClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateDrawer";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateDrawerResult";

      public override string SoapAction => "http://microsoft.com/webservices/CreateDrawer";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Guid.Empty;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.GuidFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter);
      }
    }

    internal sealed class DeleteDrawerClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteDrawer";

      public override string SoapAction => "http://microsoft.com/webservices/DeleteDrawer";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter);
      }
    }

    internal sealed class DeleteItemClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteItem";

      public override string SoapAction => "http://microsoft.com/webservices/DeleteItem";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lookupKey", parameter2);
      }
    }

    internal sealed class GetDrawerContentsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetDrawerContents";

      public override bool HasOutputs => true;

      public override string ResultName => "GetDrawerContentsResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetDrawerContents";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfStrongBoxItemInfo;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfStrongBoxItemInfoFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter);
      }
    }

    internal sealed class GetItemInfoClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetItemInfo";

      public override bool HasOutputs => true;

      public override string ResultName => "GetItemInfoResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetItemInfo";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) StrongBoxItemInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lookupKey", parameter2);
      }
    }

    internal sealed class GetStringClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetString";

      public override bool HasOutputs => true;

      public override string ResultName => "GetStringResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetString";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "drawerId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "lookupKey", parameter2);
      }
    }

    internal sealed class UnlockDrawerClientOperation : TfsClientOperation
    {
      public override string BodyName => "UnlockDrawer";

      public override bool HasOutputs => true;

      public override string ResultName => "UnlockDrawerResult";

      public override string SoapAction => "http://microsoft.com/webservices/UnlockDrawer";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Guid.Empty;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.GuidFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter);
      }
    }
  }
}
