// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.XmlCreationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class XmlCreationHelper
  {
    private const string c_propertyXmlElement = "property";
    public const string IdAttribute = "id";
    public const string TitleAttribute = "title";
    public const string UrlAttribute = "url";
    public const string TypeAttribute = "type";
    public const string ValueAttribute = "value";
    public const string NameAttribute = "name";
    public const string CountAttribute = "count";
    public const string ClassAttribute = "class";
    public const string TargetAttribute = "target";
    public const string HrefAttribute = "href";
    public const string IndexAttribute = "index";
    public const string WorkItemTypeAttribute = "workItemType";
    public const string SizeAttribute = "size";
    public const string DateAttribute = "date";
    public const string CommentsAttribute = "comments";
    public const string RefAttribute = "ref";

    public static XmlNode AddPopertyTag(
      XmlNode rootNode,
      XmlDocument doc,
      string name,
      string value,
      string url = "")
    {
      XmlElement element = doc.CreateElement("property");
      element.SetAttribute(nameof (name), name);
      element.SetAttribute(nameof (value), value);
      if (!string.IsNullOrEmpty(url))
        element.SetAttribute(nameof (url), url);
      rootNode.AppendChild((XmlNode) element);
      return (XmlNode) element;
    }
  }
}
