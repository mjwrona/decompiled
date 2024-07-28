// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.IFilterTranslator
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  public interface IFilterTranslator
  {
    void AddFilterNameTranslation(string filterName, string translation);

    void AddFilterNameTranslation(Regex filterNameRegex, Func<string[], string> translation);

    void AddFilterExpressionTranslation(
      string filterName,
      Func<FilterExpression, string> translation);

    void AddFilterExpressionTranslation(
      Regex filterNameRegex,
      Func<FilterExpression, string[], string> translation);

    string Translate(string filter);

    IDictionary<string, string> FilterNameTranslations { get; }

    IDictionary<Regex, Func<string[], string>> FilterNameRegexTranslations { get; }

    IDictionary<string, Func<FilterExpression, string>> FilterExpressionTranslations { get; }

    IDictionary<Regex, Func<FilterExpression, string[], string>> FilterExpressionRegexTranslations { get; }
  }
}
