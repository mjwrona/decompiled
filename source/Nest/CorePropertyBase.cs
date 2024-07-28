// Decompiled with JetBrains decompiler
// Type: Nest.CorePropertyBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public abstract class CorePropertyBase : PropertyBase, ICoreProperty, IProperty, IFieldMapping
  {
    protected CorePropertyBase(FieldType type)
      : base(type)
    {
    }

    public Nest.Fields CopyTo { get; set; }

    public IProperties Fields { get; set; }

    public string Similarity { get; set; }

    public bool? Store { get; set; }
  }
}
