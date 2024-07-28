// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Matcher`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class Matcher<T> : IMatcher
  {
    private readonly HashSet<T> _list;
    private readonly int _percentage;
    private readonly QueryAccessor<T> _accessor;
    private readonly IEqualityComparer<T> _comparer;

    public Matcher(
      IEnumerable<T> list,
      int percentage,
      QueryAccessor<T> accessor,
      IEqualityComparer<T> comparer)
    {
      if (percentage < 0 || percentage > 100)
        throw new ArgumentException("percentage must be in <0, 100>");
      this._list = list != null ? new HashSet<T>(list, comparer) : new HashSet<T>();
      this._percentage = percentage;
      this._accessor = accessor;
      this._comparer = comparer ?? throw new ArgumentNullException(nameof (comparer));
    }

    public bool Match(in Query query)
    {
      T y = this._accessor(in query);
      return !this._comparer.Equals(default (T), y) && (this._list.Contains(y) || this._percentage != 0 && (this._percentage == 100 || Math.Abs(this._comparer.GetHashCode(y)) % 100 < this._percentage));
    }

    bool IMatcher.Match(in Query query) => this.Match(in query);
  }
}
