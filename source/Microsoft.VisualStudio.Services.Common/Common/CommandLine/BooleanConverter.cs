// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.BooleanConverter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public sealed class BooleanConverter : ValueConverter
  {
    protected override Type ResultType => typeof (bool);

    protected override object ConvertValue(string value)
    {
      bool? nullable = new bool?();
      int result;
      if (int.TryParse(value, out result))
      {
        switch (result)
        {
          case 0:
            nullable = new bool?(false);
            break;
          case 1:
            nullable = new bool?(true);
            break;
        }
      }
      if (!nullable.HasValue)
        nullable = new bool?(bool.Parse(value));
      return (object) nullable.Value;
    }
  }
}
