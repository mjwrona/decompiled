// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models.VSServiceContextKeys
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models
{
  public static class VSServiceContextKeys
  {
    public const string LCID = "LCID";
    public const string ServiceSource = "SearchSource";
    public const string TemplateType = "TemplateType";
    public const string Skus = "Skus";
    public const string ProductArchitecure = "ProductArchitecture";
    public const string SubSkus = "SubSkus";
    public const string OSVersion = "OSVersion";
    public const string VSVersion = "VSVersion";
    public static readonly string[] RequiredVSServiceContextKeys = new string[3]
    {
      nameof (LCID),
      "SearchSource",
      nameof (OSVersion)
    };

    public static bool RequestContextIsValid(IDictionary<string, string> requestContext) => ((IEnumerable<string>) VSServiceContextKeys.RequiredVSServiceContextKeys).All<string>(new Func<string, bool>(requestContext.ContainsKey));
  }
}
