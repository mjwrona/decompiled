// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContentValidationUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ContentValidationUtil
  {
    private static readonly Regex s_dataUriRegex = new Regex("\r\n            data:                   # Source always starts with this\r\n            (?>(?<mime>[\\w/\\-\\.]+)) # Data URIs require a media/MIME type.  NOTE: ?> disables backtracking.\r\n            ;                       # Separator                             We don't need it and it can cause perf issues.\r\n            base64                  # Content must be base64 encoded for us to submit to CVS\r\n            ,                       # Separator\r\n            (?>(?<data>[^\"\\)]+))   # Match base64 content until we hit a double-quote or close paren (end of HTML attribute or markdown embed)\r\n            ", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5.0));

    public static ContentValidationScanType GetScanTypeFromFileName(string fileNameOrPath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fileNameOrPath, nameof (fileNameOrPath));
      string extension;
      try
      {
        extension = Path.GetExtension(fileNameOrPath);
      }
      catch (ArgumentException ex)
      {
        return ContentValidationScanType.None;
      }
      return ContentValidationUtil.GetScanTypeFromContentType(MimeMapper.GetContentType(extension));
    }

    public static ContentValidationScanType GetScanTypeFromContentType(string contentType)
    {
      if (contentType.StartsWith("text/", StringComparison.Ordinal) || contentType == string.Empty)
        return ContentValidationScanType.None;
      if (contentType.StartsWith("image/", StringComparison.Ordinal))
        return ContentValidationScanType.Image;
      return contentType.StartsWith("video/", StringComparison.Ordinal) ? ContentValidationScanType.Video : ContentValidationScanType.None;
    }

    public static ContentValidationScanType DetectScanTypeFromStream(Stream content)
    {
      byte[] buf = new byte[MimeMapper.SuggestedMimeDetectHeaderBytes];
      ContentValidationUtil.TryReadGreedy(content, buf, 0, buf.Length);
      return ContentValidationUtil.DetectScanTypeFromFileHeader((IReadOnlyCollection<byte>) buf);
    }

    public static ContentValidationScanType DetectScanTypeFromFileHeader(
      IReadOnlyCollection<byte> buf)
    {
      string maybeMimeType;
      return buf.Count >= MimeMapper.MinMimeDetectHeaderBytes && MimeMapper.TryDetectMimeType(buf, out maybeMimeType) ? ContentValidationUtil.GetScanTypeFromContentType(maybeMimeType) : ContentValidationScanType.None;
    }

    public static int TryReadGreedy(Stream stream, byte[] buf, int offset, int count)
    {
      int num1;
      int num2;
      for (num1 = 0; num1 < count; num1 += num2)
      {
        num2 = stream.Read(buf, offset + num1, count - num1);
        if (num2 == 0)
          break;
      }
      return num1;
    }

    public static bool MightHaveDataUri(string source) => source.IndexOf("data:", StringComparison.OrdinalIgnoreCase) >= 0;

    public static IEnumerable<DataUriEmbeddedContent> ExtractBase64DataUriEmbeddedContent(
      string document)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(document, nameof (document));
      if (ContentValidationUtil.MightHaveDataUri(document))
      {
        MatchCollection docMatches = ContentValidationUtil.s_dataUriRegex.Matches(document);
        for (int i = 0; i < docMatches.Count; ++i)
        {
          Match match = docMatches[i];
          if (match.Success)
            yield return new DataUriEmbeddedContent(match.Groups["data"].Value, match.Groups["mime"].Value);
        }
      }
    }
  }
}
