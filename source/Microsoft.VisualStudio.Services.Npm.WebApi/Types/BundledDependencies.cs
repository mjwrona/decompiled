// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.BundledDependencies
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [JsonConverter(typeof (BundledDependenciesJsonConverter))]
  public class BundledDependencies
  {
    public static BundledDependencies All { get; } = new BundledDependencies((IReadOnlyList<string>) Array.Empty<string>(), true);

    public static BundledDependencies None { get; } = new BundledDependencies((IReadOnlyList<string>) Array.Empty<string>(), false);

    public IReadOnlyList<string> List { get; }

    public bool BundleAllDependencies { get; }

    public BundledDependencies(IReadOnlyList<string> list)
      : this(list, false)
    {
    }

    private BundledDependencies(IReadOnlyList<string> list, bool bundleAllDependencies)
    {
      this.List = list;
      this.BundleAllDependencies = bundleAllDependencies;
    }
  }
}
