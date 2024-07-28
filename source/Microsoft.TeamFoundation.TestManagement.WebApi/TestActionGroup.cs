// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestActionGroup
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class TestActionGroup : TestAction, ITestActionGroup, ITestAction
  {
    public const string ElementName = "steps";
    public const string LastUsedAttributeName = "last";
    private TestActionCollection m_actions;

    public TestActionGroup(ITestActionOwner owner)
      : base(owner)
    {
      this.m_actions = new TestActionCollection();
    }

    public TestActionGroup(ITestActionOwner owner, int id)
      : base(owner, id)
    {
      this.m_actions = new TestActionCollection();
    }

    public TestActionCollection Actions => this.m_actions;

    public override void ToXml(XmlWriter writer)
    {
      writer.WriteStartElement("steps");
      this.WriteAttributes(writer);
      foreach (IXmlStorage action in (Collection<ITestAction>) this.m_actions)
        action.ToXml(writer);
      writer.WriteEndElement();
    }

    public override void FromXml(XmlReader reader)
    {
      this.m_actions.Clear();
      this.ReadAttributes(reader);
      if (reader == null)
        return;
      if (!reader.IsEmptyElement)
      {
        reader.Read();
        while (reader.IsStartElement())
          this.m_actions.Add((ITestAction) TestAction.CreateFromXml(this.Owner, reader));
        reader.ReadEndElement();
      }
      else
        reader.Read();
    }

    protected virtual void ReadAttributes(XmlReader reader)
    {
    }

    protected virtual void WriteAttributes(XmlWriter writer) => this.WriteBaseAttributes(writer);
  }
}
