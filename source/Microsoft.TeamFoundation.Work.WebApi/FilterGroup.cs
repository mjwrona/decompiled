// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.FilterGroup
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class FilterGroup
  {
    public FilterGroup(int start, int end, int level)
    {
      this.Start = start;
      this.End = end;
      this.Level = level;
    }

    [DataMember(Order = 1, Name = "start", EmitDefaultValue = false)]
    public int Start { get; set; }

    [DataMember(Order = 2, Name = "end", EmitDefaultValue = false)]
    public int End { get; set; }

    [DataMember(Order = 3, Name = "level", EmitDefaultValue = false)]
    public int Level { get; set; }
  }
}
