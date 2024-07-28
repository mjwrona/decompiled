// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.NodeDescription
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public class NodeDescription
  {
    public string[] Elements { get; set; }

    public string AttributeName { get; set; }

    public string AttributeValue { get; set; }

    public NodeDescription Clone() => new NodeDescription()
    {
      Elements = this.Elements,
      AttributeName = this.AttributeName,
      AttributeValue = this.AttributeValue
    };

    public string GetXPathString()
    {
      if (this.Elements == null || this.Elements.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Join("/", this.Elements));
      if (!string.IsNullOrEmpty(this.AttributeName))
        stringBuilder.AppendFormat("[{0}='{1}']", (object) this.AttributeName, (object) (this.AttributeValue ?? string.Empty));
      return stringBuilder.ToString().Trim();
    }

    public NodeDescription CreateChildNode(
      string elementName,
      string attributeName,
      string attributeValue)
    {
      List<string> stringList = new List<string>((IEnumerable<string>) this.Elements);
      if (!string.IsNullOrEmpty(elementName))
        stringList.Add(elementName);
      return new NodeDescription()
      {
        Elements = stringList.ToArray(),
        AttributeName = attributeName,
        AttributeValue = attributeValue
      };
    }

    public NodeDescription CreateChildNode(string elementName) => this.CreateChildNode(elementName, (string) null, (string) null);

    public NodeDescription CreateChildNode(string attributeName, string attributeValue) => this.CreateChildNode((string) null, attributeName, attributeValue);
  }
}
