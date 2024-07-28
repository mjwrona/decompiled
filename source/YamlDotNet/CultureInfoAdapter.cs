// Decompiled with JetBrains decompiler
// Type: YamlDotNet.CultureInfoAdapter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;

namespace YamlDotNet
{
  internal sealed class CultureInfoAdapter : CultureInfo
  {
    private readonly IFormatProvider _provider;

    public CultureInfoAdapter(CultureInfo baseCulture, IFormatProvider provider)
      : base(baseCulture.LCID)
    {
      this._provider = provider;
    }

    public override object GetFormat(Type formatType) => this._provider.GetFormat(formatType);
  }
}
