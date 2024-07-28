// Decompiled with JetBrains decompiler
// Type: Nest.CompletionProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [DebuggerDisplay("{DebugDisplay}")]
  public class CompletionProperty : 
    DocValuesPropertyBase,
    ICompletionProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public CompletionProperty()
      : base(FieldType.Completion)
    {
    }

    public string Analyzer { get; set; }

    public IList<ISuggestContext> Contexts { get; set; }

    public int? MaxInputLength { get; set; }

    public bool? PreservePositionIncrements { get; set; }

    public bool? PreserveSeparators { get; set; }

    public string SearchAnalyzer { get; set; }
  }
}
