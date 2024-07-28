// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.LinkingProxy
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  internal class LinkingProxy : TfsHttpClient
  {
    private const string c_namespaceUrl = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03";

    internal LinkingProxy(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    protected override string ComponentName => "LinkingService";

    protected override string ServiceType => "LinkingService";

    public Artifact[] GetArtifacts(string[] artifactUris) => (Artifact[]) this.Invoke((TfsClientOperation) new LinkingProxy.GetArtifactsClientOperation(), new object[1]
    {
      (object) artifactUris
    });

    public Artifact[] GetReferencingArtifacts(string[] uriList) => this.GetReferencingArtifacts(uriList, (LinkFilter[]) null);

    public Artifact[] GetReferencingArtifacts(string[] uriList, LinkFilter[] filters) => (Artifact[]) this.Invoke((TfsClientOperation) new LinkingProxy.GetReferencingArtifactsClientOperation(), new object[2]
    {
      (object) uriList,
      (object) filters
    });

    private sealed class GetArtifactsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetArtifacts";

      public override bool HasOutputs => true;

      public override string ResultName => "GetArtifactsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03/GetArtifacts";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03";

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Artifact.ArtifactArrayFromXml(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter = (string[]) parameters[0];
        if (parameter == null)
          return;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        XmlUtility.ArrayOfObjectToXml<string>((XmlWriter) writer, parameter, "artifactUris", "string", false, false, LinkingProxy.GetArtifactsClientOperation.\u003C\u003EO.\u003C0\u003E__StringToXmlElement ?? (LinkingProxy.GetArtifactsClientOperation.\u003C\u003EO.\u003C0\u003E__StringToXmlElement = new Action<XmlWriter, string, string>(XmlUtility.StringToXmlElement)));
      }
    }

    private sealed class GetReferencingArtifactsClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetReferencingArtifacts";

      public override bool HasOutputs => true;

      public override string ResultName => "GetReferencingArtifactsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03/GetReferencingArtifacts";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03";

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Artifact.ArtifactArrayFromXml(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        if (parameter1 != null)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          XmlUtility.ArrayOfObjectToXml<string>((XmlWriter) writer, parameter1, "uriList", "string", false, false, LinkingProxy.GetReferencingArtifactsClientOperation.\u003C\u003EO.\u003C0\u003E__StringToXmlElement ?? (LinkingProxy.GetReferencingArtifactsClientOperation.\u003C\u003EO.\u003C0\u003E__StringToXmlElement = new Action<XmlWriter, string, string>(XmlUtility.StringToXmlElement)));
        }
        LinkFilter[] parameter2 = (LinkFilter[]) parameters[1];
        if (parameter2 == null)
          return;
        LinkFilter.LinkFilterArrayToXml((XmlWriter) writer, "filters", parameter2);
      }
    }
  }
}
