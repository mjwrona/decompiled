// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.CommitId
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class CommitId : IComparable
  {
    private DateTime value;

    private CommitId(DateTime dateTime) => this.value = dateTime;

    public string Spec => this.ToString();

    public static bool operator >(CommitId c1, CommitId c2) => c1.CompareTo((object) c2) > 0;

    public static bool operator <(CommitId c1, CommitId c2) => c1.CompareTo((object) c2) < 0;

    public static CommitId Create(string spec) => new CommitId(DateTime.Parse(spec, (IFormatProvider) null, DateTimeStyles.RoundtripKind));

    public static CommitId Create(DateTime spec) => new CommitId(spec);

    public override string ToString() => this.value.ToString("O");

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      return obj is CommitId commitId ? this.value.CompareTo(commitId.value) : throw new ArgumentException(string.Format("Cannot compare {0}s to {1}s", (object) this.GetType(), (object) obj.GetType()));
    }

    public override bool Equals(object obj) => obj is CommitId commitId && this.value.Equals(commitId.value);

    public override int GetHashCode() => this.value.GetHashCode();
  }
}
