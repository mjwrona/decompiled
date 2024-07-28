// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoStatement
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class KustoStatement
  {
    protected abstract FormattableString Statement { get; }

    public override string ToString() => this.Statement.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public static implicit operator string(KustoStatement statement) => statement?.ToString();

    protected string CheckAndNormalize(string component)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(component, nameof (component));
      return component.Trim();
    }
  }
}
