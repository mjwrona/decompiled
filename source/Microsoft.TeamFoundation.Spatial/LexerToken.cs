// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.LexerToken
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Globalization;

namespace Microsoft.Spatial
{
  internal class LexerToken
  {
    public string Text { get; set; }

    public int Type { get; set; }

    public bool MatchToken(int targetType, string targetText, StringComparison comparison)
    {
      if (this.Type != targetType)
        return false;
      return string.IsNullOrEmpty(targetText) || this.Text.Equals(targetText, comparison);
    }

    public override string ToString() => "Type:[" + this.Type.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "] Text:[" + this.Text + "]";
  }
}
