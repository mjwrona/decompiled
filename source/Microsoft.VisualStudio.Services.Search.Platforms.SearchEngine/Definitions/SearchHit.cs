// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public abstract class SearchHit
  {
    protected SearchHit()
      : this((IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>())
    {
    }

    protected SearchHit(IDictionary<string, IEnumerable<string>> fields) => this.Fields = fields;

    public IDictionary<string, IEnumerable<string>> Fields { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual string GetFieldValue(string fieldName)
    {
      IEnumerable<string> source;
      if (!this.Fields.TryGetValue(fieldName, out source))
        throw new SearchServiceException("Search platform response: " + fieldName + " not found");
      return source.FirstOrDefault<string>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual string GetFieldValue(string fieldName, string defaultFieldValue)
    {
      IEnumerable<string> source;
      return !this.Fields.TryGetValue(fieldName, out source) ? defaultFieldValue : source.FirstOrDefault<string>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual IEnumerable<string> GetFieldValues(string fieldName)
    {
      IEnumerable<string> fieldValues;
      if (!this.Fields.TryGetValue(fieldName, out fieldValues))
        throw new SearchServiceException("Search platform response: " + fieldName + " not found");
      return fieldValues;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual IEnumerable<string> GetFieldValues(
      string fieldName,
      IEnumerable<string> defaultFieldValue)
    {
      IEnumerable<string> strings;
      return !this.Fields.TryGetValue(fieldName, out strings) ? defaultFieldValue : strings;
    }
  }
}
