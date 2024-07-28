// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactPropertyValueDbPagingManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ArtifactPropertyValueDbPagingManager : 
    ArtifactPagingManager<ArtifactPropertyValue>
  {
    public ArtifactPropertyValueDbPagingManager(
      IVssRequestContext requestContext,
      PropertyComponent component)
      : base(requestContext, component)
    {
    }

    protected override string RootElement => "PropertyValueList";

    protected override void WriteSingleElement(ArtifactPropertyValue artifactPropertyValue)
    {
      ArgumentUtility.CheckForNull<ArtifactPropertyValue>(artifactPropertyValue, nameof (artifactPropertyValue));
      ArtifactSpec spec = artifactPropertyValue.Spec;
      ArgumentUtility.CheckForNull<ArtifactSpec>(spec, "artifactPropertyValue.Spec");
      spec.Validate(PropertiesOptions.None);
      this.UpdatePagedArtifactKind(spec);
      if (artifactPropertyValue.PropertyValues == null)
        return;
      foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
      {
        propertyValue.Validate();
        this.m_xmlTextWriter.WriteStartElement("pv");
        if (string.IsNullOrEmpty(spec.Moniker))
        {
          this.m_xmlTextWriter.WriteStartAttribute("aid");
          if (spec.Id != null)
            this.m_xmlTextWriter.WriteValue((object) spec.Id);
          this.m_xmlTextWriter.WriteEndAttribute();
        }
        else
          this.m_xmlTextWriter.WriteAttributeString("m", spec.Moniker);
        this.m_xmlTextWriter.WriteAttributeString("v", spec.Version.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.m_xmlTextWriter.WriteAttributeString("pn", propertyValue.PropertyName);
        XmlPropertyWriter.WriteValue((XmlWriter) this.m_xmlTextWriter, propertyValue.PropertyName, propertyValue.Value);
        if (this.m_component is PropertyComponent7)
          this.m_xmlTextWriter.WriteAttributeString("ds", this.m_component.GetDataspaceId(spec.DataspaceIdentifier).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.m_xmlTextWriter.WriteEndElement();
        this.m_counter += spec.Size + propertyValue.GetCachedSize();
      }
    }
  }
}
