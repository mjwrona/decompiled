// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteFieldValue
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class CheckinNoteFieldValue : IValidatable, IComparable<CheckinNoteFieldValue>
  {
    internal int checkinNoteId;
    private string m_name;
    private string m_value;

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("val")]
    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    public override string ToString() => this.Name + "= " + this.Value;

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkFieldName("Name", this.m_name, false);
    }

    public int CompareTo(CheckinNoteFieldValue other)
    {
      int num = string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
      return num != 0 ? num : string.Compare(this.Value, other.Value, StringComparison.CurrentCultureIgnoreCase);
    }
  }
}
