// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XPathStringExtensionFunction
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class XPathStringExtensionFunction : IXsltContextFunction
  {
    private XPathResultType[] m_ArgTypes;
    private XPathResultType m_ReturnType;
    private string m_FunctionName;
    private int m_MinArgs;
    private int m_MaxArgs;

    internal XPathStringExtensionFunction(
      string name,
      int minArgs,
      int maxArgs,
      XPathResultType[] argTypes,
      XPathResultType returnType)
    {
      this.m_FunctionName = name;
      this.m_MinArgs = minArgs;
      this.m_MaxArgs = maxArgs;
      this.m_ArgTypes = argTypes;
      this.m_ReturnType = returnType;
    }

    public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext) => this.m_FunctionName == "string-join" ? (object) XPathStringExtensionFunction.GetStringJoin(args, docContext) : (object) null;

    private static string GetStringJoin(object[] args, XPathNavigator docContext)
    {
      try
      {
        string xpath = args.Length == 2 ? args[0].ToString() : throw new ArgumentException("string-join requires 2 arguments", nameof (args));
        char ch = args[1].ToString()[0];
        string stringJoin = (string) null;
        if (!(docContext.Evaluate(xpath, (IXmlNamespaceResolver) new TfsNamespaceResolver()) is XPathNodeIterator xpathNodeIterator))
          stringJoin = (string) null;
        foreach (XPathNavigator xpathNavigator in xpathNodeIterator)
          stringJoin = stringJoin + xpathNavigator.Value + ch.ToString();
        if (!string.IsNullOrWhiteSpace(stringJoin))
          stringJoin = stringJoin.Trim(ch);
        return stringJoin;
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    public int Minargs => this.m_MinArgs;

    public int Maxargs => this.m_MaxArgs;

    public XPathResultType[] ArgTypes => this.m_ArgTypes;

    public XPathResultType ReturnType => this.m_ReturnType;
  }
}
