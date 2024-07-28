// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Board
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class Board : BoardReference
  {
    [DataMember(Order = 40)]
    public int Revision { get; set; }

    [DataMember(Order = 50)]
    public IList<BoardColumn> Columns { get; set; }

    [DataMember(Order = 60)]
    public IList<BoardRow> Rows { get; set; }

    [DataMember(Order = 70)]
    public bool IsValid { get; set; }

    [DataMember(Order = 80)]
    public IDictionary<string, IDictionary<string, string[]>> AllowedMappings { get; set; }

    [DataMember(Order = 90)]
    public bool CanEdit { get; set; }

    [DataMember(Order = 100)]
    public BoardFields Fields { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links", Order = 100)]
    public ReferenceLinks Links { get; set; }
  }
}
