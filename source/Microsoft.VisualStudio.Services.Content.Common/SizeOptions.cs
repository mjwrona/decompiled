// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.SizeOptions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class SizeOptions
  {
    public readonly long MinSize;
    public readonly long MaxSize;
    public readonly long MinSizeInBytes;
    public readonly long MaxSizeInBytes;
    private const char Separator = '_';
    private const long GBMultiplier = 1073741824;
    private const long DefaultMaxBytes = 109951162777600;
    private const long DefaultMinBytes = 0;
    private const long HundredTB = 102400;

    public SizeOptions(string sizeOptionFilter)
    {
      if (string.IsNullOrWhiteSpace(sizeOptionFilter))
        throw new ArgumentException("SizeOption cannot be Null.");
      string[] strArray = sizeOptionFilter.Contains<char>('_') ? sizeOptionFilter.Split('_') : throw new ArgumentException(string.Format("SizeOptions should contain {0} , e.g. 5_10", (object) '_'));
      if (strArray.Length != 2)
        throw new ArgumentException("SizeOptions filter should follow the format e.g. 5_10");
      if (string.IsNullOrWhiteSpace(strArray[0]))
        strArray[0] = "0";
      if (string.IsNullOrWhiteSpace(strArray[1]))
        strArray[1] = 102400L.ToString();
      long result1;
      if (!long.TryParse(strArray[0], out result1))
        throw new ArgumentException("First parameter of SizeOptions is invalid, e.g. 5_10");
      long result2;
      if (!long.TryParse(strArray[1], out result2))
        throw new ArgumentException("Second parameter of SizeOptions is invalid, e.g. 5_10");
      this.MinSize = this.IsValid(result1, result2) ? result1 : throw new ArgumentException("Minimum value of SizeOption must be lesser than Maximum value. Input Bounds: [0_102400] e.g. 5_10");
      this.MaxSize = result2;
      this.MinSizeInBytes = Math.Max(this.MinSize * 1073741824L, 0L);
      this.MaxSizeInBytes = Math.Min(this.MaxSize * 1073741824L, 109951162777600L);
    }

    public override string ToString()
    {
      long num = this.MinSize;
      string str1 = num.ToString();
      num = this.MaxSize;
      string str2 = num.ToString();
      return str1 + "_" + str2;
    }

    private bool IsValid(long minSize, long maxSize) => minSize >= 0L && minSize <= 102400L && maxSize >= 0L && maxSize <= 102400L && minSize <= maxSize;
  }
}
