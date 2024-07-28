// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.EditorPreferences
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class EditorPreferences : VersionControlSecuredObject
  {
    [DataMember(Name = "foldingEnabled")]
    public bool FoldingEnabled { get; set; }

    [DataMember(Name = "minimapEnabled")]
    public bool MinimapEnabled { get; set; }

    [DataMember(Name = "theme")]
    public string Theme { get; set; }

    [DataMember(Name = "whiteSpaceEnabled")]
    public bool WhiteSpaceEnabled { get; set; }

    [DataMember(Name = "wordWrapEnabled")]
    public bool WordWrapEnabled { get; set; }
  }
}
