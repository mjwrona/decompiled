// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchServiceJobData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class SearchServiceJobData
  {
    private const string SearchServiceJobName = "SearchServiceJob";
    private const string IndexingUnitIdAttributeName = "IndexingUnitId";

    public int IndexingUnitId { get; set; }

    public SearchServiceJobData()
    {
    }

    public SearchServiceJobData(int indexingUnitId) => this.IndexingUnitId = indexingUnitId;

    public SearchServiceJobData(XmlNode xmlNode) => this.IndexingUnitId = int.Parse(xmlNode.GetAttributeValue(nameof (IndexingUnitId)), (IFormatProvider) CultureInfo.InvariantCulture);

    [SuppressMessage("Microsoft.Security.Xml", "CA3053", Justification = "PR build is reporting this issue even though it is fixed")]
    public XmlNode ToXml()
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.XmlResolver = (XmlResolver) null;
      XmlNode element = (XmlNode) xmlDoc.CreateElement("SearchServiceJob");
      xmlDoc.AddAttribute(element, "IndexingUnitId", this.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return element;
    }
  }
}
