// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MimeMapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class MimeMapper
  {
    private static readonly Dictionary<string, XExtension> s_extensions = new Dictionary<string, XExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly List<XDisplayable> s_displayableEntries = new List<XDisplayable>();
    private static readonly List<XDisplayable> s_nonDisplayableEntries = new List<XDisplayable>();
    private static readonly List<KeyValuePair<Regex, string>> s_hexHeaders;
    private static readonly ConcurrentDictionary<string, XDisplayable> s_displayableContentTypesCache;
    private static readonly ConcurrentDictionary<string, XDisplayable> s_displayableExtensionsCache;
    private static readonly ConcurrentDictionary<string, XDisplayable> s_nonDisplayableContentTypesCache;
    private static readonly ConcurrentDictionary<string, XDisplayable> s_nonDisplayableExtensionsCache;

    static MimeMapper()
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null,
        IgnoreProcessingInstructions = true
      };
      Assembly assembly = Assembly.GetAssembly(typeof (MimeMapper));
      XMimeMap xmimeMap;
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".ContentValidation.MimeMapper.MimeMap.xml"))
      {
        using (XmlReader xmlReader = XmlReader.Create(manifestResourceStream, settings))
          xmimeMap = new XmlSerializer(typeof (XMimeMap)).Deserialize(xmlReader) as XMimeMap;
      }
      if (xmimeMap.Extensions != null)
      {
        foreach (XExtension extension in xmimeMap.Extensions)
          MimeMapper.s_extensions[extension.name] = extension;
      }
      if (xmimeMap.DisplayFilters != null)
      {
        if (xmimeMap.DisplayFilters.Add != null)
        {
          foreach (XDisplayable displayable in xmimeMap.DisplayFilters.Add)
          {
            if (string.IsNullOrEmpty(displayable.filter))
              displayable.filter = "Text";
            if (MimeMapper.ValidateXDisplayableEntry(displayable))
              MimeMapper.s_displayableEntries.Add(displayable);
          }
        }
        if (xmimeMap.DisplayFilters.Remove != null)
        {
          foreach (XDisplayable displayable in xmimeMap.DisplayFilters.Remove)
          {
            if (MimeMapper.ValidateXDisplayableEntry(displayable))
              MimeMapper.s_nonDisplayableEntries.Add(displayable);
          }
        }
      }
      MimeMapper.s_hexHeaders = new List<KeyValuePair<Regex, string>>();
      List<XMimeHexHeader> list = ((IEnumerable<XMimeHexHeader>) xmimeMap.HexHeaders).OrderByDescending<XMimeHexHeader, int>((Func<XMimeHexHeader, int>) (xh => xh.headerbytes.Length)).ToList<XMimeHexHeader>();
      MimeMapper.SuggestedMimeDetectHeaderBytes = list[0].headerbytes.Length / 2;
      MimeMapper.MinMimeDetectHeaderBytes = list.Last<XMimeHexHeader>().headerbytes.Length / 2;
      foreach (XMimeHexHeader xmimeHexHeader in list)
        MimeMapper.s_hexHeaders.Add(new KeyValuePair<Regex, string>(new Regex("^" + xmimeHexHeader.headerbytes, RegexOptions.Compiled), xmimeHexHeader.contenttype));
      MimeMapper.s_displayableContentTypesCache = new ConcurrentDictionary<string, XDisplayable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      MimeMapper.s_displayableExtensionsCache = new ConcurrentDictionary<string, XDisplayable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      MimeMapper.s_nonDisplayableContentTypesCache = new ConcurrentDictionary<string, XDisplayable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      MimeMapper.s_nonDisplayableExtensionsCache = new ConcurrentDictionary<string, XDisplayable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static int SuggestedMimeDetectHeaderBytes { get; }

    public static int MinMimeDetectHeaderBytes { get; }

    public static string GetContentType(string extension)
    {
      if (string.IsNullOrEmpty(extension))
        return string.Empty;
      if (!extension.StartsWith(".", StringComparison.Ordinal))
        extension = "." + extension;
      XExtension extension1 = MimeMapper.GetExtension(extension);
      return extension1 != null ? extension1.contenttype ?? "" : string.Empty;
    }

    public static ContentViewerType GetContentViewerType(string extension, string contentType)
    {
      XDisplayable xdisplayable = (XDisplayable) null;
      if (!string.IsNullOrEmpty(extension))
        xdisplayable = MimeMapper.GetDisplayableByExtension(true, extension);
      if (xdisplayable == null && !string.IsNullOrEmpty(contentType))
        xdisplayable = MimeMapper.GetDisplayableByContentType(true, contentType);
      ContentViewerType result;
      return xdisplayable == null || string.IsNullOrEmpty(xdisplayable.filter) || !Enum.TryParse<ContentViewerType>(xdisplayable.filter, true, out result) ? ContentViewerType.None : result;
    }

    public static bool TryDetectMimeType(
      IReadOnlyCollection<byte> headerBytes,
      out string maybeMimeType)
    {
      string hexString = headerBytes.ToHexString();
      foreach (KeyValuePair<Regex, string> hexHeader in MimeMapper.s_hexHeaders)
      {
        if (hexHeader.Key.IsMatch(hexString))
        {
          maybeMimeType = hexHeader.Value;
          return true;
        }
      }
      maybeMimeType = (string) null;
      return false;
    }

    private static string ToHexString(this IReadOnlyCollection<byte> data)
    {
      StringBuilder stringBuilder = data != null ? new StringBuilder(data.Count * 2) : throw new ArgumentNullException(nameof (data));
      foreach (byte num in (IEnumerable<byte>) data)
      {
        stringBuilder.Append("0123456789ABCDEF"[(int) num >> 4]);
        stringBuilder.Append("0123456789ABCDEF"[(int) num & 15]);
      }
      return stringBuilder.ToString();
    }

    private static bool ValidateXDisplayableEntry(XDisplayable displayable)
    {
      if (string.IsNullOrEmpty(displayable.value))
        return false;
      displayable.value = displayable.value.Replace("*", "[a-zA-Z_\\-0-9]*").Replace("?", "[a-zA-Z_\\-0-9]");
      try
      {
        Regex regex = new Regex(displayable.value);
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Info, nameof (MimeMapper), nameof (ValidateXDisplayableEntry), ex);
        return false;
      }
    }

    private static XExtension GetExtension(string fileExtension)
    {
      XExtension xextension;
      return MimeMapper.s_extensions.TryGetValue(fileExtension, out xextension) ? xextension : (XExtension) null;
    }

    private static XDisplayable GetDisplayableByContentType(bool displayable, string contentType)
    {
      ConcurrentDictionary<string, XDisplayable> cache = displayable ? MimeMapper.s_displayableContentTypesCache : MimeMapper.s_nonDisplayableContentTypesCache;
      IEnumerable<XDisplayable> xdisplayables = displayable ? (IEnumerable<XDisplayable>) MimeMapper.s_displayableEntries : (IEnumerable<XDisplayable>) MimeMapper.s_nonDisplayableEntries;
      string key = contentType;
      IEnumerable<XDisplayable> displayables = xdisplayables;
      return MimeMapper.GetDisplayableByKey(cache, key, displayables, XDisplayableType.contenttype);
    }

    private static XDisplayable GetDisplayableByExtension(bool displayable, string extension)
    {
      ConcurrentDictionary<string, XDisplayable> cache = displayable ? MimeMapper.s_displayableExtensionsCache : MimeMapper.s_nonDisplayableExtensionsCache;
      IEnumerable<XDisplayable> xdisplayables = displayable ? (IEnumerable<XDisplayable>) MimeMapper.s_displayableEntries : (IEnumerable<XDisplayable>) MimeMapper.s_nonDisplayableEntries;
      string key = extension;
      IEnumerable<XDisplayable> displayables = xdisplayables;
      return MimeMapper.GetDisplayableByKey(cache, key, displayables, XDisplayableType.extension);
    }

    private static XDisplayable GetDisplayableByKey(
      ConcurrentDictionary<string, XDisplayable> cache,
      string key,
      IEnumerable<XDisplayable> displayables,
      XDisplayableType type)
    {
      if (string.IsNullOrEmpty(key))
        return (XDisplayable) null;
      XDisplayable displayableByKey = (XDisplayable) null;
      if (cache.TryGetValue(key, out displayableByKey))
        return displayableByKey;
      foreach (XDisplayable displayable in displayables)
      {
        if (displayable.type == type && Regex.IsMatch(key, displayable.value, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
        {
          displayableByKey = displayable;
          break;
        }
      }
      cache.TryAdd(key, displayableByKey);
      return displayableByKey;
    }
  }
}
