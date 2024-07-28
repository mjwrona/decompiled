// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildOptionInputDefinition
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class BuildOptionInputDefinition
  {
    [DataMember(Name = "Options")]
    private Dictionary<string, string> m_Options;
    [DataMember(Name = "Help")]
    private Dictionary<string, string> m_HelpDocuments;

    public BuildOptionInputDefinition()
    {
      this.InputType = BuildOptionInputType.String;
      this.DefaultValue = string.Empty;
      this.Required = false;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Label { get; set; }

    public string DefaultValue { get; set; }

    public bool Required { get; set; }

    [DataMember(Name = "Type")]
    public BuildOptionInputType InputType { get; set; }

    [DataMember]
    public string VisibleRule { get; set; }

    [DataMember]
    public string GroupName { get; set; }

    public Dictionary<string, string> Options
    {
      get
      {
        if (this.m_Options == null)
          this.m_Options = new Dictionary<string, string>();
        return this.m_Options;
      }
      set => this.m_Options = value;
    }

    public Dictionary<string, string> HelpDocuments
    {
      get
      {
        if (this.m_HelpDocuments == null)
          this.m_HelpDocuments = new Dictionary<string, string>();
        return this.m_HelpDocuments;
      }
      set
      {
        if (value == null)
          return;
        this.m_HelpDocuments = new Dictionary<string, string>((IDictionary<string, string>) value);
      }
    }
  }
}
