// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XPathExtensionVariable
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class XPathExtensionVariable : IXsltContextVariable
  {
    private string m_varName;

    internal XPathExtensionVariable(string varName) => this.m_varName = varName;

    public object Evaluate(XsltContext xsltContext) => ((NotificationXsltContext) xsltContext).ArgList.GetParam(this.m_varName, (string) null);

    public bool IsLocal => false;

    public bool IsParam => false;

    public XPathResultType VariableType => XPathResultType.Any;
  }
}
