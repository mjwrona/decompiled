// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationXsltContext
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationXsltContext : XsltContext
  {
    private XsltArgumentList m_ArgsList;

    internal NotificationXsltContext()
    {
    }

    internal NotificationXsltContext(System.Xml.NameTable nt)
      : base(nt)
    {
      this.AddNamespace("customExpression", "http://schemas.microsoft.com/TeamFoundation/2010/customExpression");
      this.AddNamespace("tb1", "http://schemas.microsoft.com/TeamFoundation/2010/Build");
    }

    internal NotificationXsltContext(System.Xml.NameTable nt, XsltArgumentList argList)
      : base(nt)
    {
      this.AddNamespace("customExpression", "http://schemas.microsoft.com/TeamFoundation/2010/customExpression");
      this.AddNamespace("tb1", "http://schemas.microsoft.com/TeamFoundation/2010/Build");
      this.m_ArgsList = argList;
    }

    public override IXsltContextFunction ResolveFunction(
      string prefix,
      string name,
      XPathResultType[] ArgTypes)
    {
      XPathStringExtensionFunction extensionFunction = (XPathStringExtensionFunction) null;
      if (name == "string-join")
        extensionFunction = new XPathStringExtensionFunction("string-join", 2, 2, new XPathResultType[2]
        {
          XPathResultType.String,
          XPathResultType.String
        }, XPathResultType.String);
      return (IXsltContextFunction) extensionFunction;
    }

    public override IXsltContextVariable ResolveVariable(string prefix, string name) => (IXsltContextVariable) new XPathExtensionVariable(name);

    public override int CompareDocument(string baseUri, string nextbaseUri) => 0;

    public override bool PreserveWhitespace(XPathNavigator node) => true;

    public XsltArgumentList ArgList => this.m_ArgsList;

    public override bool Whitespace => true;
  }
}
