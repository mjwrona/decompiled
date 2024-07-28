// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CultureStringComparer
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class CultureStringComparer : StringComparer
  {
    private CultureInfo cultureInfo;
    private CompareOptions compareOptions;

    public CultureStringComparer(CultureInfo culture, bool ignoreCase)
    {
      this.cultureInfo = culture;
      this.compareOptions = ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
    }

    public override int Compare(string x, string y) => this.cultureInfo.CompareInfo.Compare(x, y, this.compareOptions);

    public override bool Equals(string x, string y) => this.Compare(x, y) == 0;

    public override int GetHashCode(string obj) => obj.GetHashCode();
  }
}
