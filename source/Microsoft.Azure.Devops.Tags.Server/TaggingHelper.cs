// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TaggingHelper
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class TaggingHelper
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
    private const string c_tagRegistryPathRoot = "/Service/Tagging/Settings/";
    private const string NormalizeUnicodeDisabledFeatureFlag = "TeamFoundationTaggingService.NormalizeUnicodeDisabled";

    public static string[] SplitTagText(string text) => string.IsNullOrWhiteSpace(text) ? Array.Empty<string>() : ((IEnumerable<string>) text.Split(TaggingHelper.TagSeparators, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (t => t.Trim())).Where<string>((Func<string, bool>) (t => !string.IsNullOrEmpty(t))).Distinct<string>((IEqualityComparer<string>) VssStringComparer.TagName).ToArray<string>();

    public static string FormatTagsValue(IEnumerable<string> tags) => string.Join("; ", tags);

    public static bool IsUnicodeNormalizationEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("TeamFoundationTaggingService.NormalizeUnicodeDisabled");

    public static string NormalizeUnicode(string tagName) => tagName.Normalize(NormalizationForm.FormKC);
  }
}
