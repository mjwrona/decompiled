// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationXmlEvent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Xml;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationXmlEvent : TeamFoundationEvent
  {
    public TeamFoundationXmlEvent(string eventType, IFieldContainer fieldContainer)
    {
      this.EventType = eventType;
      this.m_fieldContainer = fieldContainer;
    }

    public TeamFoundationXmlEvent(string eventType, string eventData)
    {
      this.EventType = eventType;
      this.m_fieldContainer = (IFieldContainer) new XmlDocumentFieldContainer(eventData);
    }

    public override IFieldContainer GetFieldContainer() => this.m_fieldContainer;

    public override void UpdateFieldContainer(string eventData) => this.m_fieldContainer = (IFieldContainer) new XmlDocumentFieldContainer(eventData);

    public XmlDocument Document => this.GetFieldContainer() is XmlDocumentFieldContainer fieldContainer ? fieldContainer.GetDocument() : (XmlDocument) null;
  }
}
