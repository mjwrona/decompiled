// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.RegistrationComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class RegistrationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<RegistrationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<RegistrationComponent2>(2)
    }, "Registration");

    public void AddArtifacts(string toolType, IEnumerable<RegistrationArtifactType> artifacts)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      XmlTextWriter xmlArtifactLinkWriter = new XmlTextWriter((TextWriter) w);
      xmlArtifactLinkWriter.WriteStartDocument();
      xmlArtifactLinkWriter.WriteStartElement("links");
      foreach (RegistrationArtifactType artifact in artifacts)
      {
        if (artifact.OutboundLinkTypes.Length == 0)
          RegistrationComponent.WriteArtifactLinkXML(xmlArtifactLinkWriter, toolType, artifact.Name, ArtifactLinkIds.NoOutboundLink, string.Empty, string.Empty);
        foreach (OutboundLinkType outboundLinkType in artifact.OutboundLinkTypes)
          RegistrationComponent.WriteArtifactLinkXML(xmlArtifactLinkWriter, toolType, artifact.Name, outboundLinkType.Name, outboundLinkType.TargetArtifactTypeTool, outboundLinkType.TargetArtifactTypeName);
      }
      xmlArtifactLinkWriter.WriteEndElement();
      xmlArtifactLinkWriter.WriteEndDocument();
      xmlArtifactLinkWriter.Flush();
      this.PrepareStoredProcedure("prc_AddLink");
      this.BindXml("@linkXML", w.ToString());
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection ReadLinks()
    {
      this.PrepareStoredProcedure("prc_GetLinkTable");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetLinkTable", this.RequestContext);
      resultCollection.AddBinder<OutboundLinkType>((ObjectBinder<OutboundLinkType>) new OutboundLinkTypeColumns());
      return resultCollection;
    }

    private static void WriteArtifactLinkXML(
      XmlTextWriter xmlArtifactLinkWriter,
      string toolType,
      string artifactName,
      string linkName,
      string sinkType,
      string sinkArtifactTypeName)
    {
      xmlArtifactLinkWriter.WriteStartElement("l");
      xmlArtifactLinkWriter.WriteAttributeString("s", toolType);
      xmlArtifactLinkWriter.WriteAttributeString("a", artifactName);
      xmlArtifactLinkWriter.WriteAttributeString("l", linkName);
      xmlArtifactLinkWriter.WriteAttributeString("t", sinkType);
      xmlArtifactLinkWriter.WriteAttributeString("n", sinkArtifactTypeName);
      xmlArtifactLinkWriter.WriteEndElement();
    }
  }
}
