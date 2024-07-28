// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiffParameters
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class FileDiffParameters
  {
    [DataMember(Name = "originalContent", EmitDefaultValue = false)]
    public string OriginalContent { get; set; }

    [DataMember(Name = "originalPath", EmitDefaultValue = false)]
    public string OriginalPath { get; set; }

    [DataMember(Name = "originalVersion", EmitDefaultValue = false)]
    public string OriginalVersion { get; set; }

    [DataMember(Name = "modifiedContent", EmitDefaultValue = false)]
    public string ModifiedContent { get; set; }

    [DataMember(Name = "modifiedPath", EmitDefaultValue = false)]
    public string ModifiedPath { get; set; }

    [DataMember(Name = "modifiedVersion", EmitDefaultValue = false)]
    public string ModifiedVersion { get; set; }

    [DataMember(Name = "partialDiff", EmitDefaultValue = false)]
    public bool PartialDiff { get; set; }

    [DataMember(Name = "ignoreTrimmedWhitespace", EmitDefaultValue = false)]
    public bool? IgnoreTrimmedWhitespace { get; set; }

    [DataMember(Name = "lineNumbersOnly", EmitDefaultValue = false)]
    public bool LineNumbersOnly { get; set; }

    [DataMember(Name = "includeCharDiffs", EmitDefaultValue = false)]
    public bool IncludeCharDiffs { get; set; }

    [DataMember(Name = "forceLoad", EmitDefaultValue = false)]
    public bool ForceLoad { get; set; }
  }
}
