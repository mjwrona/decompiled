// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.RepositoryJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class RepositoryJsonConverter : BaseMultiFormatJsonConverter<Repository>
  {
    public const string RepoTypePropertyName = "type";
    public const string UrlPropertyName = "url";

    protected override Repository ParseStringValue(string value)
    {
      Uri result;
      if (Uri.TryCreate(value, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
        return new Repository()
        {
          RepoType = (string) null,
          Url = value,
          ShortcutSyntax = (string) null
        };
      return new Repository()
      {
        RepoType = (string) null,
        Url = (string) null,
        ShortcutSyntax = value
      };
    }

    protected override Repository ParseProperties(Dictionary<string, string> properties) => new Repository()
    {
      RepoType = properties.ContainsKey("type") ? properties["type"] : (string) null,
      Url = properties.ContainsKey("url") ? properties["url"] : (string) null,
      ShortcutSyntax = (string) null
    };

    protected override Repository ParseArray(List<object> tokens) => (Repository) null;
  }
}
