// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.CargoVersionLabel`1
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  public abstract class CargoVersionLabel<TDerived> : IComparable<TDerived> where TDerived : CargoVersionLabel<TDerived>
  {
    protected CargoVersionLabel(IEnumerable<IVersionLabelSegment> segments)
    {
      this.Segments = (IImmutableList<IVersionLabelSegment>) segments.ToImmutableList<IVersionLabelSegment>();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.Segments, nameof (segments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) this.Segments, nameof (segments));
    }

    public IImmutableList<IVersionLabelSegment> Segments { get; }

    public int CompareTo(TDerived other)
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

    public override string ToString() => this.GetType().Name + "(" + CargoPackageVersionFormatter.FormatLabelNormalized(this.Segments) + ")";
  }
}
