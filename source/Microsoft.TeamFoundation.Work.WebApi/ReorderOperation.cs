// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.ReorderOperation
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class ReorderOperation
  {
    public const int ReorderEndOfList = 0;

    [DataMember(Name = "ParentId")]
    public int? ParentId { get; set; }

    [DataMember(Name = "PreviousId")]
    public int? PreviousId { get; set; }

    [DataMember(Name = "NextId")]
    public int? NextId { get; set; }

    [DataMember(Name = "Ids")]
    public int[] Ids { get; set; }

    [DataMember(Name = "IterationPath")]
    public string IterationPath { get; set; }
  }
}
