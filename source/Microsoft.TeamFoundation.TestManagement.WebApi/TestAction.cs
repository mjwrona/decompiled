// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestAction
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal abstract class TestAction : ITestAction, IXmlStorage
  {
    private const string IdAttributeName = "id";
    private int m_id = 1;
    protected ITestActionOwner m_owner;

    internal TestAction(ITestActionOwner owner)
    {
      this.m_owner = owner;
      this.m_id = this.m_owner.GetNextAvailableActionId();
    }

    internal TestAction(ITestActionOwner owner, int id)
    {
      this.m_owner = owner;
      this.m_id = id;
    }

    public int Id => this.m_id;

    internal ITestActionOwner Owner => this.m_owner;

    ITestBase ITestAction.Owner => (ITestBase) this.Owner;

    public abstract void FromXml(XmlReader reader);

    public abstract void ToXml(XmlWriter writer);

    internal static TestAction CreateFromXml(ITestActionOwner owner, XmlReader reader)
    {
      string s = reader["id"];
      int result;
      if (!int.TryParse(s, out result))
        throw new XmlException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid attribute in XML, {0}={1}", (object) "id", (object) s));
      TestAction fromXml;
      switch (reader.Name)
      {
        case "step":
          fromXml = (TestAction) new TestStep(owner, result);
          break;
        case "steps":
          fromXml = (TestAction) new TestActionGroup(owner, result);
          break;
        case "compref":
          fromXml = (TestAction) new SharedStep(owner, result);
          break;
        default:
          throw new XmlException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unexpected element name {0}.", (object) reader.Name));
      }
      fromXml.FromXml(reader);
      return fromXml;
    }

    protected void WriteBaseAttributes(XmlWriter writer)
    {
      writer.WriteStartAttribute("id");
      writer.WriteValue(this.m_id);
    }
  }
}
