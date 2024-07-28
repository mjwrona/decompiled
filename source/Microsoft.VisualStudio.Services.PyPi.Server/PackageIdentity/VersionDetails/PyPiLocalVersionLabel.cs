// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails.PyPiLocalVersionLabel
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails
{
  public sealed class PyPiLocalVersionLabel : IComparable<PyPiLocalVersionLabel>
  {
    public PyPiLocalVersionLabel(IEnumerable<IVersionLabelSegment> segments)
    {
      this.Segments = (IImmutableList<IVersionLabelSegment>) segments.ToImmutableList<IVersionLabelSegment>();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) segments.ToImmutableList<IVersionLabelSegment>(), nameof (segments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) segments.ToImmutableList<IVersionLabelSegment>(), nameof (segments));
    }

    public IImmutableList<IVersionLabelSegment> Segments { get; }

    public int CompareTo(PyPiLocalVersionLabel other)
    {
      using (IEnumerator<IVersionLabelSegment> enumerator1 = this.Segments.GetEnumerator())
      {
        using (IEnumerator<IVersionLabelSegment> enumerator2 = other.Segments.GetEnumerator())
        {
          int num1;
          do
          {
            int num2 = enumerator1.MoveNext() ? 1 : 0;
            bool flag = enumerator2.MoveNext();
            if (num2 == 0)
              return !flag ? 0 : -1;
            if (!flag)
              return 1;
            num1 = enumerator1.Current.CompareTo(enumerator2.Current);
          }
          while (num1 == 0);
          return num1;
        }
      }
    }

    public override string ToString() => string.Join(".", this.Segments.Select<IVersionLabelSegment, string>((Func<IVersionLabelSegment, string>) (x => x.StringValue.ToLowerInvariant())));
  }
}
