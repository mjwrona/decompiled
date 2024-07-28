// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.IdentityPicker.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations
{
  [JsonObject(MemberSerialization.OptIn)]
  public class SearchOptions : IdentityPickerServiceExtensionData
  {
    internal int minResults;
    internal int maxResults;
    internal int imageHeight;
    internal int imageWidth;
    internal Guid scopeId;
    internal static readonly HashSet<string> ValidSearchOptions = new HashSet<string>()
    {
      "ImageWidth",
      "ImageHeight",
      "MaxResults",
      "MinResults",
      "ExtensionId"
    };
    public const string MinResultsOptionKey = "MinResults";
    public const string MaxResultsOptionKey = "MaxResults";
    public const string ScopeIdKey = "ScopeId";
    internal const string ImageHeightOptionKey = "ImageHeight";
    internal const string ImageWidthOptionKey = "ImageWidth";
    internal const int DefaultImageHeight = 50;
    internal const int DefaultImageWidth = 50;

    [JsonExtensionData]
    public override Dictionary<string, object> Options { get; set; }

    public void ParseOptions()
    {
      this.Options = this.Options != null ? new Dictionary<string, object>((IDictionary<string, object>) this.Options, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int result1 = 20;
      if (this.Options.ContainsKey("MaxResults"))
        int.TryParse(this.Options["MaxResults"].ToString(), out result1);
      int result2 = 20;
      if (this.Options.ContainsKey("MinResults"))
        int.TryParse(this.Options["MinResults"].ToString(), out result2);
      int result3 = 50;
      if (this.Options.ContainsKey("ImageHeight"))
        int.TryParse(this.Options["ImageHeight"].ToString(), out result3);
      int result4 = 50;
      if (this.Options.ContainsKey("ImageWidth"))
        int.TryParse(this.Options["ImageWidth"].ToString(), out result4);
      if (this.Options.ContainsKey("ScopeId"))
        Guid.TryParse(this.Options["ScopeId"].ToString(), out this.scopeId);
      this.minResults = result2;
      this.maxResults = result1;
      this.imageHeight = result3;
      this.imageWidth = result4;
    }

    internal static void Validate(Dictionary<string, object> options)
    {
      try
      {
        if (options == null)
          return;
        options.Keys.ToList<string>().ForEach((Action<string>) (option =>
        {
          if (SearchOptions.ValidSearchOptions.Contains<string>(option, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            return;
          options.Remove(option);
        }));
      }
      catch (Exception ex)
      {
        throw new IdentityPickerValidateException("Invalid Search options in Search request", ex);
      }
    }
  }
}
