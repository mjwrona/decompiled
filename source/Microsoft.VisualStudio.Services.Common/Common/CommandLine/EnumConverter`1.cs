// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.EnumConverter`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public sealed class EnumConverter<T> : ValueConverter
  {
    private EnumConverter internalConverter;

    public EnumConverter() => this.internalConverter = new EnumConverter(typeof (T));

    protected override Type ResultType => typeof (T);

    protected override object ConvertValue(string value) => this.internalConverter.Convert(value);
  }
}
