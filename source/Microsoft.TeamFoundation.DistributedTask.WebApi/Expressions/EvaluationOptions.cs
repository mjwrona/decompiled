// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvaluationOptions
  {
    public EvaluationOptions()
    {
    }

    public EvaluationOptions(EvaluationOptions copy)
    {
      if (copy == null)
        return;
      this.Converters = copy.Converters;
      this.MaxMemory = copy.MaxMemory;
      this.TimeZone = copy.TimeZone;
      this.UseCollectionInterfaces = copy.UseCollectionInterfaces;
      this.StrictlyIndexedObjects = copy.StrictlyIndexedObjects;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IDictionary<Type, Converter<object, ConversionResult>> Converters { get; set; }

    public int MaxMemory { get; set; }

    public TimeZoneInfo TimeZone { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UseCollectionInterfaces { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ICollection<string> StrictlyIndexedObjects { get; set; }
  }
}
