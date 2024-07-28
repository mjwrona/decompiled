// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Classification4
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
  internal class Classification4 : Classification3
  {
    internal Classification4(TfsTeamProjectCollection server, string url)
      : base(server, url)
    {
    }

    public Classification4(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("edd317f7-a7c3-4c97-a039-ba933e895201");

    protected override string ServiceType => "CommonStructure4";

    public string CreateNodeWithDates(
      string nodeName,
      string parentNodeUri,
      DateTime? startDate,
      DateTime? finishDate)
    {
      return (string) this.Invoke((TfsClientOperation) new Classification4.CreateNodeWithDatesClientOperation(), new object[4]
      {
        (object) nodeName,
        (object) parentNodeUri,
        (object) startDate,
        (object) finishDate
      });
    }

    public ProjectProperty GetProjectProperty(string projectUri, string name) => (ProjectProperty) this.Invoke((TfsClientOperation) new Classification4.GetProjectPropertyClientOperation(), new object[2]
    {
      (object) projectUri,
      (object) name
    });

    public void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate) => this.Invoke((TfsClientOperation) new Classification4.SetIterationDatesClientOperation(), new object[3]
    {
      (object) nodeUri,
      (object) startDate,
      (object) finishDate
    });

    public void SetProjectProperty(string projectUri, string name, string value) => this.Invoke((TfsClientOperation) new Classification4.SetProjectPropertyClientOperation(), new object[3]
    {
      (object) projectUri,
      (object) name,
      (object) value
    });

    internal sealed class CreateNodeWithDatesClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateNodeWithDates";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateNodeWithDatesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/CreateNodeWithDates";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeName", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "parentNodeUri", parameter2);
        DateTime? parameter3 = (DateTime?) parameters[2];
        if (parameter3.HasValue)
          XmlUtility.DateToXmlElement((XmlWriter) writer, "startDate", parameter3.Value);
        DateTime? parameter4 = (DateTime?) parameters[3];
        if (!parameter4.HasValue)
          return;
        XmlUtility.DateToXmlElement((XmlWriter) writer, "finishDate", parameter4.Value);
      }
    }

    internal sealed class GetProjectPropertyClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetProjectProperty";

      public override bool HasOutputs => true;

      public override string ResultName => "GetProjectPropertyResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetProjectProperty";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ProjectProperty.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter2);
      }
    }

    internal sealed class SetIterationDatesClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetIterationDates";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/SetIterationDates";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeUri", parameter1);
        DateTime? parameter2 = (DateTime?) parameters[1];
        if (parameter2.HasValue)
          XmlUtility.DateToXmlElement((XmlWriter) writer, "startDate", parameter2.Value);
        DateTime? parameter3 = (DateTime?) parameters[2];
        if (!parameter3.HasValue)
          return;
        XmlUtility.DateToXmlElement((XmlWriter) writer, "finishDate", parameter3.Value);
      }
    }

    internal sealed class SetProjectPropertyClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetProjectProperty";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/SetProjectProperty";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "name", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "value", parameter3);
      }
    }
  }
}
