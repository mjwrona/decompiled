// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactSpecDbPagingManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ArtifactSpecDbPagingManager : ArtifactPagingManager<ArtifactSpec>
  {
    private int m_sequenceNumber;
    private PropertiesOptions m_options;

    public ArtifactSpecDbPagingManager(
      IVssRequestContext requestContext,
      PropertiesOptions options,
      PropertyComponent component)
      : base(requestContext, component)
    {
      this.m_sequenceNumber = 0;
      this.m_options = options;
    }

    public bool ContainsWildcards { get; private set; }

    protected override string RootElement => "artifactSpecList";

    protected override void WriteSingleElement(ArtifactSpec artifactSpec)
    {
      ArgumentUtility.CheckForNull<ArtifactSpec>(artifactSpec, nameof (artifactSpec));
      this.UpdatePagedArtifactKind(artifactSpec);
      bool containsWildcards;
      artifactSpec = ArtifactSpec.TranslateSqlWildcards(artifactSpec, out containsWildcards);
      if (containsWildcards)
        this.ContainsWildcards = true;
      artifactSpec.Validate(this.m_options);
      this.m_xmlTextWriter.WriteStartElement("a");
      this.m_xmlTextWriter.WriteAttributeString("sid", this.m_sequenceNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ++this.m_sequenceNumber;
      if (string.IsNullOrEmpty(artifactSpec.Moniker))
      {
        this.m_xmlTextWriter.WriteStartAttribute("aid");
        if (artifactSpec.Id != null)
          this.m_xmlTextWriter.WriteValue((object) artifactSpec.Id);
        this.m_xmlTextWriter.WriteEndAttribute();
      }
      else
        this.m_xmlTextWriter.WriteAttributeString("m", artifactSpec.Moniker);
      if ((this.m_options & PropertiesOptions.AllVersions) != PropertiesOptions.AllVersions)
        this.m_xmlTextWriter.WriteAttributeString("v", artifactSpec.Version.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.m_component is PropertyComponent7)
        this.m_xmlTextWriter.WriteAttributeString("ds", this.m_component.GetDataspaceId(artifactSpec.DataspaceIdentifier).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.m_xmlTextWriter.WriteEndElement();
      this.m_counter += artifactSpec.Size;
    }
  }
}
