// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.SharedStep
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class SharedStep : TestAction, ISharedStep, ITestAction
  {
    public const string ElementName = "compref";
    public const string SharedStepReferenceAttributeName = "ref";
    public const string SharedStepDescription = "Shared Step Id: {0}";
    private int m_sharedStepReference;

    public SharedStep(ITestActionOwner owner)
      : base(owner)
    {
      this.m_sharedStepReference = 0;
    }

    public SharedStep(ITestActionOwner owner, int id)
      : base(owner, id)
    {
      this.m_sharedStepReference = 0;
    }

    public int SharedStepId
    {
      get => this.m_sharedStepReference;
      set => this.m_sharedStepReference = value != 0 ? value : throw new ArgumentNullException(nameof (value));
    }

    public override void FromXml(XmlReader reader)
    {
      int result = 0;
      int.TryParse(reader["ref"], out result);
      this.SharedStepId = result;
      reader.Read();
    }

    public override void ToXml(XmlWriter writer)
    {
      writer.WriteStartElement("compref");
      this.WriteBaseAttributes(writer);
      writer.WriteStartAttribute("ref");
      writer.WriteValue(this.m_sharedStepReference);
    }
  }
}
