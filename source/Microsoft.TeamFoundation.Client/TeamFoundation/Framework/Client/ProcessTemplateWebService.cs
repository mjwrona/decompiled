// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ProcessTemplateWebService
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
  internal class ProcessTemplateWebService : TfsHttpClient
  {
    public ProcessTemplateWebService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("75ab998e-7f09-479e-9559-b86b5b06f688");

    protected override string ComponentName => "Framework";

    protected override string ServiceType => "ProcessTemplate";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public FrameworkTemplateHeader[] DeleteTemplate(int templateId) => (FrameworkTemplateHeader[]) this.Invoke((TfsClientOperation) new ProcessTemplateWebService.DeleteTemplateClientOperation(), new object[1]
    {
      (object) templateId
    });

    public FrameworkTemplateHeader[] MakeDefaultTemplate(int templateId) => (FrameworkTemplateHeader[]) this.Invoke((TfsClientOperation) new ProcessTemplateWebService.MakeDefaultTemplateClientOperation(), new object[1]
    {
      (object) templateId
    });

    public FrameworkTemplateHeader[] TemplateHeaders() => (FrameworkTemplateHeader[]) this.Invoke((TfsClientOperation) new ProcessTemplateWebService.TemplateHeadersClientOperation(), Array.Empty<object>());

    internal sealed class DeleteTemplateClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteTemplate";

      public override bool HasOutputs => true;

      public override string ResultName => "DeleteTemplateResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03/DeleteTemplate";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfFrameworkTemplateHeader;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.ArrayOfObjectFromXml<FrameworkTemplateHeader>(serviceProvider, reader, "TemplateHeader", false, ProcessTemplateWebService.DeleteTemplateClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ProcessTemplateWebService.DeleteTemplateClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, FrameworkTemplateHeader>(FrameworkTemplateHeader.FromXml)));

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "templateId", parameter);
      }
    }

    internal sealed class MakeDefaultTemplateClientOperation : TfsClientOperation
    {
      public override string BodyName => "MakeDefaultTemplate";

      public override bool HasOutputs => true;

      public override string ResultName => "MakeDefaultTemplateResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03/MakeDefaultTemplate";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfFrameworkTemplateHeader;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.ArrayOfObjectFromXml<FrameworkTemplateHeader>(serviceProvider, reader, "TemplateHeader", false, ProcessTemplateWebService.MakeDefaultTemplateClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ProcessTemplateWebService.MakeDefaultTemplateClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, FrameworkTemplateHeader>(FrameworkTemplateHeader.FromXml)));

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "templateId", parameter);
      }
    }

    internal sealed class TemplateHeadersClientOperation : TfsClientOperation
    {
      public override string BodyName => "TemplateHeaders";

      public override bool HasOutputs => true;

      public override string ResultName => "TemplateHeadersResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03/TemplateHeaders";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfFrameworkTemplateHeader;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.ArrayOfObjectFromXml<FrameworkTemplateHeader>(serviceProvider, reader, "TemplateHeader", false, ProcessTemplateWebService.TemplateHeadersClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ProcessTemplateWebService.TemplateHeadersClientOperation.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, FrameworkTemplateHeader>(FrameworkTemplateHeader.FromXml)));
    }
  }
}
