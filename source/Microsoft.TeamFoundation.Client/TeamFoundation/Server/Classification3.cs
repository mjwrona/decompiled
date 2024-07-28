// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Classification3
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
  internal class Classification3 : Classification
  {
    internal Classification3(TfsTeamProjectCollection server, string url)
      : base(server, url)
    {
    }

    public Classification3(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("02ea5fcc-1e40-4d94-a8e5-ed62c15cb676");

    protected override string ServiceType => "CommonStructure3";

    public string GetChangedNodesAndProjects(int firstSequenceId) => (string) this.Invoke((TfsClientOperation) new Classification3.GetChangedNodesAndProjectsClientOperation(), new object[1]
    {
      (object) firstSequenceId
    });

    internal sealed class GetChangedNodesAndProjectsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetChangedNodesAndProjects";

      public override bool HasOutputs => true;

      public override string ResultName => "GetChangedNodesAndProjectsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetChangedNodesAndProjects";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "firstSequenceId", parameter);
      }
    }
  }
}
