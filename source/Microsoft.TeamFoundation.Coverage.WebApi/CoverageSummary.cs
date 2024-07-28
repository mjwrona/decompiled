// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Coverage.WebApi.CoverageSummary
// Assembly: Microsoft.TeamFoundation.Coverage.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 18FFD1B8-3EB9-46CC-8BE4-74DD890A1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Coverage.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Coverage.WebApi
{
  [DataContract]
  public class CoverageSummary
  {
    [DataMember(Name = "path", IsRequired = false, EmitDefaultValue = false)]
    public string Path;
    [DataMember(Name = "isDirectory", IsRequired = false, EmitDefaultValue = false)]
    public bool IsDirectory;
    [DataMember(Name = "covered", IsRequired = false, EmitDefaultValue = false)]
    public int Covered;
    [DataMember(Name = "partiallyCovered", IsRequired = false, EmitDefaultValue = false)]
    public int PartiallyCovered;
    [DataMember(Name = "notCovered", IsRequired = false, EmitDefaultValue = false)]
    public int NotCovered;
  }
}
