// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.BinaryJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class BinaryJsonConverter : BaseMultiFormatJsonConverter<Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ParseStringValue(string value) => new Dictionary<string, string>()
    {
      {
        string.Empty,
        value
      }
    };

    protected override Dictionary<string, string> ParseProperties(
      Dictionary<string, string> properties)
    {
      return properties;
    }

    protected override Dictionary<string, string> ParseArray(List<object> tokens) => (Dictionary<string, string>) null;
  }
}
