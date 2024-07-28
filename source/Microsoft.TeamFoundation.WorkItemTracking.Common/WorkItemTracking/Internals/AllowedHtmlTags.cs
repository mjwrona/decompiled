// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.AllowedHtmlTags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  internal static class AllowedHtmlTags
  {
    private static Dictionary<string, AllowedHtmlTags.TagInfo> m_hashAllowed;
    private static Dictionary<string, int> m_hashSpecial;
    private static Dictionary<string, int> m_hashAttributes;

    public static bool AreTagsEqual(string t1, string t2) => string.Equals(t1, t2, StringComparison.OrdinalIgnoreCase);

    public static bool IsAllowedTag(string s)
    {
      AllowedHtmlTags.Init();
      return AllowedHtmlTags.m_hashAllowed.ContainsKey(s);
    }

    public static bool IsSpecialTag(string s)
    {
      AllowedHtmlTags.Init();
      return AllowedHtmlTags.m_hashSpecial.ContainsKey(s);
    }

    public static bool IsAllowedAttribute(string t, string a)
    {
      AllowedHtmlTags.Init();
      if (AllowedHtmlTags.m_hashAttributes.ContainsKey(a))
        return true;
      AllowedHtmlTags.TagInfo tagInfo;
      return AllowedHtmlTags.m_hashAllowed.TryGetValue(t, out tagInfo) && tagInfo.m_attributes != null && tagInfo.m_attributes.ContainsKey(a);
    }

    public static int GetEolBefore(string t)
    {
      AllowedHtmlTags.Init();
      AllowedHtmlTags.TagInfo tagInfo;
      return !AllowedHtmlTags.m_hashAllowed.TryGetValue(t, out tagInfo) ? 0 : tagInfo.m_eolBefore;
    }

    public static int GetEolAfter(string t)
    {
      AllowedHtmlTags.Init();
      AllowedHtmlTags.TagInfo tagInfo;
      return !AllowedHtmlTags.m_hashAllowed.TryGetValue(t, out tagInfo) ? 0 : tagInfo.m_eolAfter;
    }

    private static void AddTag(string tag, int eolBefore, int eolAfter, string[] attributes)
    {
      if (attributes == null)
      {
        AllowedHtmlTags.m_hashAllowed.Add(tag, new AllowedHtmlTags.TagInfo());
      }
      else
      {
        AllowedHtmlTags.TagInfo tagInfo = new AllowedHtmlTags.TagInfo();
        tagInfo.m_eolBefore = eolBefore;
        tagInfo.m_eolAfter = eolAfter;
        tagInfo.m_attributes = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (string attribute in attributes)
          tagInfo.m_attributes.Add(attribute, 0);
        AllowedHtmlTags.m_hashAllowed.Add(tag, tagInfo);
      }
    }

    private static void AddTag(string tag, string[] attributes) => AllowedHtmlTags.AddTag(tag, 0, 0, attributes);

    private static void AddTags(string[] tags)
    {
      foreach (string tag in tags)
        AllowedHtmlTags.m_hashAllowed.Add(tag, new AllowedHtmlTags.TagInfo());
    }

    private static void AddCommonAttributes(string[] attributes)
    {
      foreach (string attribute in attributes)
        AllowedHtmlTags.m_hashAttributes.Add(attribute, 0);
    }

    private static void AddSpecialTags(string[] tags)
    {
      foreach (string tag in tags)
        AllowedHtmlTags.m_hashSpecial.Add(tag, 0);
    }

    private static void Init()
    {
      if (AllowedHtmlTags.m_hashAllowed != null)
        return;
      AllowedHtmlTags.m_hashAllowed = new Dictionary<string, AllowedHtmlTags.TagInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AllowedHtmlTags.m_hashSpecial = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AllowedHtmlTags.m_hashAttributes = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AllowedHtmlTags.AddSpecialTags(new string[4]
      {
        "script",
        "style",
        "option",
        "select"
      });
      AllowedHtmlTags.AddTag("a", new string[9]
      {
        "charset",
        "href",
        "hreflang",
        "name",
        "rel",
        "rev",
        "shape",
        "tabindex",
        "type"
      });
      AllowedHtmlTags.AddTag("blockquote", 1, 1, new string[1]
      {
        "cite"
      });
      AllowedHtmlTags.AddTag("br", 0, 0, new string[1]
      {
        "clear"
      });
      AllowedHtmlTags.AddTag("caption", 1, 1, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("col", 1, 1, new string[6]
      {
        "align",
        "char",
        "charoff",
        "span",
        "valign",
        "width"
      });
      AllowedHtmlTags.AddTag("colgroup", 1, 1, new string[6]
      {
        "align",
        "char",
        "charoff",
        "span",
        "valign",
        "width"
      });
      AllowedHtmlTags.AddTag("del", 0, 0, new string[2]
      {
        "cite",
        "datetime"
      });
      AllowedHtmlTags.AddTag("dir", 2, 2, new string[1]
      {
        "compact"
      });
      AllowedHtmlTags.AddTag("div", 1, 1, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("dl", 2, 2, new string[1]
      {
        "compact"
      });
      AllowedHtmlTags.AddTag("font", 0, 0, new string[3]
      {
        "color",
        "face",
        "size"
      });
      AllowedHtmlTags.AddTag("h1", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("h2", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("h3", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("h4", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("h5", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("h6", 2, 2, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("hr", 2, 2, new string[3]
      {
        "align",
        "size",
        "width"
      });
      AllowedHtmlTags.AddTag("img", 0, 0, new string[15]
      {
        "align",
        "alt",
        "border",
        "height",
        "hspace",
        "ismap",
        "longdesc",
        "name",
        "src",
        "usemap",
        "vspace",
        "width",
        "alt2",
        "src2",
        "tcmdata"
      });
      AllowedHtmlTags.AddTag("video", 1, 1, new string[8]
      {
        "border",
        "height",
        "src",
        "width",
        "controls",
        "poster",
        "muted",
        "loop"
      });
      AllowedHtmlTags.AddTag("ins", 0, 0, new string[2]
      {
        "cite",
        "datetime"
      });
      AllowedHtmlTags.AddTag("li", 1, 1, new string[2]
      {
        "type",
        "value"
      });
      AllowedHtmlTags.AddTag("map", 0, 0, new string[1]
      {
        "name"
      });
      AllowedHtmlTags.AddTag("menu", 2, 2, new string[1]
      {
        "compact"
      });
      AllowedHtmlTags.AddTag("ol", 2, 2, new string[3]
      {
        "compact",
        "start",
        "type"
      });
      AllowedHtmlTags.AddTag("p", 1, 1, new string[1]
      {
        "align"
      });
      AllowedHtmlTags.AddTag("pre", 1, 1, new string[1]
      {
        "width"
      });
      AllowedHtmlTags.AddTag("q", 0, 0, new string[1]
      {
        "cite"
      });
      AllowedHtmlTags.AddTag("table", 2, 2, new string[9]
      {
        "align",
        "border",
        "cellpadding",
        "cellspacing",
        "frame",
        "rules",
        "summary",
        "width",
        "caption"
      });
      AllowedHtmlTags.AddTag("tbody", 0, 0, new string[4]
      {
        "align",
        "char",
        "charoff",
        "valign"
      });
      AllowedHtmlTags.AddTag("td", 2, 2, new string[13]
      {
        "abbr",
        "align",
        "axis",
        "char",
        "charoff",
        "colspan",
        "headers",
        "height",
        "nowrap",
        "rowspan",
        "scope",
        "valign",
        "width"
      });
      AllowedHtmlTags.AddTag("tfoot", 2, 2, new string[4]
      {
        "align",
        "char",
        "charoff",
        "valign"
      });
      AllowedHtmlTags.AddTag("th", 2, 2, new string[13]
      {
        "abbr",
        "align",
        "axis",
        "char",
        "charoff",
        "colspan",
        "headers",
        "height",
        "nowrap",
        "rowspan",
        "scope",
        "valign",
        "width"
      });
      AllowedHtmlTags.AddTag("thead", 2, 2, new string[4]
      {
        "align",
        "char",
        "charoff",
        "valign"
      });
      AllowedHtmlTags.AddTag("tr", 2, 2, new string[4]
      {
        "align",
        "char",
        "charoff",
        "valign"
      });
      AllowedHtmlTags.AddTag("ul", 2, 2, new string[2]
      {
        "compact",
        "type"
      });
      AllowedHtmlTags.AddTag("dd", 1, 2, new string[0]);
      AllowedHtmlTags.AddTag("dt", 2, 1, new string[0]);
      AllowedHtmlTags.AddTag("textarea", 1, 1, new string[8]
      {
        "readonly",
        "rows",
        "cols",
        "disabled",
        "maxlength",
        "placeholder",
        "wrap",
        "name"
      });
      AllowedHtmlTags.AddTags(new string[24]
      {
        "abbr",
        "acronym",
        "address",
        "b",
        "bdo",
        "big",
        "center",
        "cite",
        "code",
        "dfn",
        "em",
        "i",
        "kbd",
        "s",
        "samp",
        "small",
        "span",
        "strike",
        "strong",
        "sub",
        "sup",
        "tt",
        "u",
        "var"
      });
      AllowedHtmlTags.AddCommonAttributes(new string[7]
      {
        "dir",
        "lang",
        "title",
        "style",
        "id",
        "class",
        "contenteditable"
      });
    }

    private struct TagInfo
    {
      internal int m_eolBefore;
      internal int m_eolAfter;
      internal Dictionary<string, int> m_attributes;
    }
  }
}
