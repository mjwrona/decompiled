// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.CsvCollectionConverter`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [CLSCompliant(false)]
  public sealed class CsvCollectionConverter<ConversionType> : ValueConverter where ConversionType : IConvertible
  {
    protected override Type ResultType => typeof (Collection<ConversionType>);

    protected override object ConvertValue(string value)
    {
      Collection<ConversionType> collection = new Collection<ConversionType>();
      if (value != null)
      {
        string[] strArray = value.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray != null)
        {
          Type conversionType = typeof (ConversionType);
          for (int index = 0; index < strArray.Length; ++index)
            collection.Add((ConversionType) System.Convert.ChangeType((object) strArray[index].Trim(), conversionType, (IFormatProvider) CultureInfo.CurrentCulture));
        }
      }
      return (object) collection;
    }
  }
}
