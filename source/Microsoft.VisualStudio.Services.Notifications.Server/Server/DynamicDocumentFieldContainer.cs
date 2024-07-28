// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DynamicDocumentFieldContainer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class DynamicDocumentFieldContainer : IFieldContainer
  {
    private IFieldContainer m_xmlContainer;
    private IFieldContainer m_jsonContainer;
    private Func<IFieldContainer> m_getXmlContainer;
    private Func<IFieldContainer> m_getJsonContainer;

    public DynamicDocumentFieldContainer(
      Func<IFieldContainer> getXmlContainer,
      Func<IFieldContainer> getJsonContainer)
    {
      this.m_getXmlContainer = getXmlContainer;
      this.m_getJsonContainer = getJsonContainer;
    }

    public bool DualExecution { get; set; }

    public object GetFieldValue(string fieldName) => this.GetDefaultContainer().GetFieldValue(fieldName);

    public string GetDocumentString() => this.GetDefaultContainer().GetDocumentString();

    public void AddOrUpdateNode(string name, string value) => this.GetDefaultContainer().AddOrUpdateNode(name, value);

    public IFieldContainer GetDynamicFieldContainer(DynamicFieldContainerType type)
    {
      if (type == DynamicFieldContainerType.Xml)
        return this.GetXmlContainer();
      if (type == DynamicFieldContainerType.Json)
        return this.GetJsonContainer();
      throw new NotSupportedException(string.Format("{0} is an unsupported value", (object) type));
    }

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      if (!flag)
        flag = this.GetDefaultContainer().Equals(obj);
      return flag;
    }

    public override int GetHashCode() => this.GetDefaultContainer().GetHashCode();

    protected virtual IFieldContainer GetDefaultContainer() => this.GetXmlContainer();

    private IFieldContainer GetXmlContainer()
    {
      if (this.m_xmlContainer == null)
      {
        this.m_xmlContainer = this.m_getXmlContainer();
        ArgumentUtility.CheckType<XmlDocumentFieldContainer>((object) this.m_xmlContainer, "m_xmlContainer", "XmlDocumentFieldContainer");
      }
      return this.m_xmlContainer;
    }

    private IFieldContainer GetJsonContainer()
    {
      if (this.m_jsonContainer == null)
      {
        this.m_jsonContainer = this.m_getJsonContainer();
        ArgumentUtility.CheckType<JsonDocumentFieldContainer>((object) this.m_jsonContainer, "m_jsonContainer", "JsonDocumentFieldContainer");
      }
      return this.m_jsonContainer;
    }
  }
}
