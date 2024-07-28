// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Coverage.WebApi.FileCoverageDetails
// Assembly: Microsoft.TeamFoundation.Coverage.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 18FFD1B8-3EB9-46CC-8BE4-74DD890A1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Coverage.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Coverage.WebApi
{
  [DataContract]
  public class FileCoverageDetails
  {
    [DataMember(Name = "path", IsRequired = true, EmitDefaultValue = false)]
    public string Path;
    [DataMember(Name = "covered", IsRequired = false, EmitDefaultValue = false)]
    public string Covered;
    [DataMember(Name = "partiallyCovered", IsRequired = false, EmitDefaultValue = false)]
    public string PartiallyCovered;
    [DataMember(Name = "notCovered", IsRequired = false, EmitDefaultValue = false)]
    public string NotCovered;
  }
}
