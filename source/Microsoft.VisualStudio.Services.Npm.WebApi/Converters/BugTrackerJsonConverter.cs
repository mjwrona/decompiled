// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.BugTrackerJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class BugTrackerJsonConverter : BaseMultiFormatJsonConverter<BugTracker>
  {
    public const string EmailPropertyName = "email";
    public const string UrlPropertyName = "url";

    protected override BugTracker ParseStringValue(string value) => new BugTracker()
    {
      Url = value,
      Email = (string) null
    };

    protected override BugTracker ParseProperties(Dictionary<string, string> properties) => new BugTracker()
    {
      Email = properties.ContainsKey("email") ? properties["email"] : (string) null,
      Url = properties.ContainsKey("url") ? properties["url"] : (string) null
    };

    protected override BugTracker ParseArray(List<object> tokens) => (BugTracker) null;
  }
}
