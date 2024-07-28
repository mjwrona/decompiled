// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementInfo
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlElementInfo : IXmlElementAttributes
  {
    private readonly Dictionary<string, XmlAttributeInfo> attributes;
    private List<XmlAnnotationInfo> annotations;

    internal XmlElementInfo(
      string elementName,
      CsdlLocation elementLocation,
      IList<XmlAttributeInfo> attributes,
      List<XmlAnnotationInfo> annotations)
    {
      this.Name = elementName;
      this.Location = elementLocation;
      if (attributes != null && attributes.Count > 0)
      {
        this.attributes = new Dictionary<string, XmlAttributeInfo>();
        foreach (XmlAttributeInfo attribute in (IEnumerable<XmlAttributeInfo>) attributes)
          this.attributes.Add(attribute.Name, attribute);
      }
      this.annotations = annotations;
    }

    IEnumerable<XmlAttributeInfo> IXmlElementAttributes.Unused
    {
      get
      {
        if (this.attributes != null)
        {
          foreach (XmlAttributeInfo xmlAttributeInfo in this.attributes.Values.Where<XmlAttributeInfo>((Func<XmlAttributeInfo, bool>) (attr => !attr.IsUsed)))
            yield return xmlAttributeInfo;
        }
      }
    }

    internal string Name { get; private set; }

    internal CsdlLocation Location { get; private set; }

    internal IXmlElementAttributes Attributes => (IXmlElementAttributes) this;

    internal IList<XmlAnnotationInfo> Annotations => (IList<XmlAnnotationInfo>) this.annotations ?? (IList<XmlAnnotationInfo>) new XmlAnnotationInfo[0];

    XmlAttributeInfo IXmlElementAttributes.this[string attributeName]
    {
      get
      {
        XmlAttributeInfo xmlAttributeInfo;
        if (this.attributes == null || !this.attributes.TryGetValue(attributeName, out xmlAttributeInfo))
          return XmlAttributeInfo.Missing;
        xmlAttributeInfo.IsUsed = true;
        return xmlAttributeInfo;
      }
    }

    internal void AddAnnotation(XmlAnnotationInfo annotation)
    {
      if (this.annotations == null)
        this.annotations = new List<XmlAnnotationInfo>();
      this.annotations.Add(annotation);
    }
  }
}
