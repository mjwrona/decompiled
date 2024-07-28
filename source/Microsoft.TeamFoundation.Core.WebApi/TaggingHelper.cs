// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TaggingHelper
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TaggingHelper
  {
    private const string c_tagsFormatSeparator = "; ";
    public static readonly char[] TagSeparators = new char[2]
    {
      ',',
      ';'
    };
    public const int MaxTagNameLength = 400;
    public const int DefaultTagDefinitionLimit = 150000;
    public const string TagLimitRegistryPath = "/Service/Tagging/Settings/TagLimit";
    public const string EnforceTagLimitsFeatureFlag = "TeamFoundationTaggingService.EnforceTagLimits";
    private const string c_tagRegistryPathRoot = "/Service/Tagging/Settings/";

    public static IEnumerable<string> SplitTagText(string text) => string.IsNullOrWhiteSpace(text) ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) ((IEnumerable<string>) text.Split(TaggingHelper.TagSeparators, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (t => t.Trim())).Where<string>((Func<string, bool>) (t => !string.IsNullOrEmpty(t))).Distinct<string>((IEqualityComparer<string>) VssStringComparer.TagName).ToArray<string>();

    public static string FormatTagsValue(IEnumerable<string> tags) => string.Join("; ", tags);
  }
}
