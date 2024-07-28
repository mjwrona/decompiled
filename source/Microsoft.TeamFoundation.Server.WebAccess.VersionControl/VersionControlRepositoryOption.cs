// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlRepositoryOption
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class VersionControlRepositoryOption
  {
    [DataMember(Name = "key")]
    public string Key { get; set; }

    [DataMember(Name = "displayHtml")]
    public string Html { get; set; }

    [DataMember(Name = "value")]
    public bool Value { get; set; }

    [DataMember(Name = "textValue")]
    public string TextValue { get; set; }

    [DataMember(Name = "defaultTextValue")]
    public string DefaultTextValue { get; set; }

    [DataMember(Name = "category")]
    public string Category { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    [DataMember(Name = "parentOptionKey")]
    public string ParentOptionKey { get; set; }
  }
}
