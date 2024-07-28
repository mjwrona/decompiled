// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AvatarUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AvatarUtils
  {
    internal const int s_smallAvatarFontSizeForTwoLetters = 10;
    internal const int s_mediumAvatarFontSizeForTwoLetters = 13;
    internal const int s_largeAvatarFontSizeForTwoLetters = 70;
    internal const int s_smallAvatarFontSizeForOneLetter = 14;
    internal const int s_mediumAvatarFontSizeForOneLetter = 20;
    internal const int s_largeAvatarFontSizeForOneLetter = 92;
    internal static readonly Font s_smallAvatarFontForTwoLetters = new Font("Segoe UI", 10f);
    internal static readonly Font s_mediumAvatarFontForTwoLetters = new Font("Segoe UI", 13f);
    internal static readonly Font s_largeAvatarFontForTwoLetters = new Font("Segoe UI", 70f);
    internal static readonly Font s_smallAvatarFontForOneLetter = new Font("Segoe UI", 14f);
    internal static readonly Font s_mediumAvatarFontForOneLetter = new Font("Segoe UI", 20f);
    internal static readonly Font s_largeAvatarFontForOneLetter = new Font("Segoe UI", 92f);
    private const string s_fontFamilyName = "Segoe UI";
    public const int LargeAvatarPixelSize = 220;
    public const int MediumAvatarPixelSize = 44;
    public const int SmallAvatarPixelSize = 34;
    public const int MaxAvatarWidthInPixels = 4000;
    public const int MinAvatarWidthInPixels = 10;
    public const int MaxAvatarHeightInPixels = 4000;
    public const int MinAvatarHeightInPixels = 10;
    private static readonly Color[] s_defaultColors = new Color[12]
    {
      Color.FromArgb(218, 58, 0),
      Color.FromArgb(170, 0, 0),
      Color.FromArgb(93, 0, 93),
      Color.FromArgb(50, 16, 92),
      Color.FromArgb(0, 30, 81),
      Color.FromArgb(0, 75, 81),
      Color.FromArgb(0, 76, 26),
      Color.FromArgb(182, 0, 160),
      Color.FromArgb(92, 40, 147),
      Color.FromArgb(0, 117, 218),
      Color.FromArgb(0, 130, 114),
      Color.FromArgb(2, 125, 0)
    };
    public static readonly VssJsonMediaTypeFormatter s_customJsonFormatter = new VssJsonMediaTypeFormatter();
    public const int MaxAvatarSize = 2621440;
    public const string TokenPngMediaType = "image/png";
    public const string TokenPng = "png";
    public const string AvatarUploadReportFormat = "{{ \"identityType\": \"{0}\", \"size\": {1}, \"accepted\": {2} }}";
    private static readonly TimeSpan s_OneSecond = new TimeSpan(0, 0, 1);

    public static int Initialize() => AvatarUtils.GenerateAvatar("Anything", ImageSize.Medium, AvatarImageFormat.Jpeg).Length;

    public static byte[] GenerateAvatar(
      string displayName,
      ImageSize avatarSize,
      AvatarImageFormat avatarImageFormat,
      bool isContainer = false)
    {
      Color color = AvatarUtils.MapToColor(AvatarUtils.GetRandomDefaultAvatarColor(displayName));
      int avatarSizeInPixels = AvatarUtils.MapToAvatarSizeInPixels(avatarSize);
      return AvatarUtils.GenerateAvatar(displayName, color, avatarSizeInPixels, avatarImageFormat, isContainer);
    }

    public static byte[] GenerateAvatar(
      string displayName,
      PaletteAlgorithm palette,
      ImageSize avatarSize,
      AvatarImageFormat avatarImageFormat,
      bool isContainer = false)
    {
      string initials = AvatarUtils.ToInitials(displayName, isContainer);
      Color respectingPalette = AvatarUtils.GetColorRespectingPalette(palette, displayName);
      int avatarSizeInPixels1 = AvatarUtils.MapToAvatarSizeInPixels(avatarSize);
      Color backgroundColor = respectingPalette;
      int avatarSizeInPixels2 = avatarSizeInPixels1;
      int num1 = (int) avatarImageFormat;
      int num2 = isContainer ? 1 : 0;
      return AvatarUtils.GenerateAvatar(initials, backgroundColor, avatarSizeInPixels2, (AvatarImageFormat) num1, num2 != 0);
    }

    private static Color GetColorRespectingPalette(PaletteAlgorithm palette, string displayName)
    {
      if (palette == PaletteAlgorithm.IdentityPalette)
        return AvatarUtils.MapToColor(AvatarUtils.GetRandomDefaultAvatarColor(displayName));
      if (palette == PaletteAlgorithm.CollectionPalette)
        return CollectionAvatarColorUtil.GetColorFromName(displayName);
      throw new ArgumentException("The argument is out of the supported enum range", nameof (palette));
    }

    public static byte[] GenerateAvatar(
      string displayName,
      Color backgroundColor,
      int avatarSizeInPixels,
      AvatarImageFormat avatarImageFormat,
      bool isContainer = false)
    {
      if (avatarImageFormat != AvatarImageFormat.Jpeg && avatarImageFormat != AvatarImageFormat.Png)
        throw new ArgumentException("Currently supported target formats are jpg and png only.", nameof (avatarImageFormat));
      string initials = AvatarUtils.ToInitials(displayName, isContainer);
      using (Bitmap bitmap = new Bitmap(avatarSizeInPixels, avatarSizeInPixels))
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        {
          using (SolidBrush solidBrush = new SolidBrush(backgroundColor))
            graphics.FillRectangle((Brush) solidBrush, 0, 0, avatarSizeInPixels, avatarSizeInPixels);
          StringFormat format = new StringFormat()
          {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
          };
          int length = StringInfo.ParseCombiningCharacters(initials).Length;
          Font avatarFont = AvatarUtils.MapToAvatarFont(avatarSizeInPixels, length);
          graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
          graphics.DrawString(initials, avatarFont, Brushes.White, new RectangleF(0.0f, 0.0f, (float) avatarSizeInPixels, (float) avatarSizeInPixels), format);
        }
        byte[] array;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          if (avatarImageFormat != AvatarImageFormat.Png)
          {
            if (avatarImageFormat != AvatarImageFormat.Jpeg)
              throw new ArgumentException("The argument is out of the supported enum range", "AvatarImageFormat");
            bitmap.Save((Stream) memoryStream, new ImageFormat(ImageFormat.Jpeg.Guid));
          }
          else
            bitmap.Save((Stream) memoryStream, new ImageFormat(ImageFormat.Png.Guid));
          array = memoryStream.ToArray();
        }
        return array;
      }
    }

    public static string ToInitials(string fullName, bool isContainer = false)
    {
      if (string.IsNullOrWhiteSpace(fullName))
        return string.Empty;
      if (isContainer)
      {
        int num = fullName.LastIndexOf('\\');
        if (num >= 0)
          fullName = fullName.Substring(num + 1);
      }
      int length = fullName.IndexOf('(');
      if (length > 0)
        fullName = fullName.Substring(0, length);
      StringBuilder stringBuilder = new StringBuilder(2);
      string[] strArray = fullName.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
      string str1 = (string) null;
      string str2 = (string) null;
      int num1 = 0;
      foreach (string str3 in strArray)
      {
        ++num1;
        if (char.IsLetter(str3[0]))
        {
          str1 = str3.Substring(0, 1);
          break;
        }
        if (str3.Length > 1 && char.IsHighSurrogate(str3[0]) && char.IsLowSurrogate(str3[1]))
        {
          str1 = str3.Substring(0, 2);
          break;
        }
      }
      for (int index = strArray.Length - 1; index >= num1; --index)
      {
        string str4 = strArray[index];
        if (char.IsLetter(str4[0]))
        {
          str2 = str4.Substring(0, 1);
          break;
        }
        if (str4.Length > 1 && char.IsHighSurrogate(str4[0]) && char.IsLowSurrogate(str4[1]))
        {
          str2 = str4.Substring(0, 2);
          break;
        }
      }
      if (str1 == null && str2 == null && strArray.Length != 0)
        return strArray[0][0].ToString().ToUpperInvariant();
      if (str1 != null)
        stringBuilder.Append(str1);
      if (str2 != null)
        stringBuilder.Append(str2);
      return stringBuilder.ToString().ToUpperInvariant();
    }

    public static Color MapToColor(DefaultAvatarColor avatarColor)
    {
      int index = (int) avatarColor;
      if (index < 0 || index > AvatarUtils.s_defaultColors.Length)
        throw new ArgumentOutOfRangeException(nameof (avatarColor), (object) avatarColor, "The enum is out of the supported range.");
      return AvatarUtils.s_defaultColors[index];
    }

    public static DefaultAvatarColor GetRandomDefaultAvatarColor(string seed) => seed == null ? DefaultAvatarColor.Grenadier : (DefaultAvatarColor) Math.Abs(seed.GetStableHashCode() % 12);

    public static HttpResponseMessage FormatResponse(
      HttpRequestMessage request,
      Microsoft.VisualStudio.Services.Users.Avatar avatar,
      string format)
    {
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
      switch (AvatarUtils.ExtractAcceptType(request, format))
      {
        case AvatarUtils.ContentType.Png:
          httpResponseMessage.Content = (HttpContent) new ByteArrayContent(avatar.Image);
          httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
          break;
        case AvatarUtils.ContentType.Json:
          httpResponseMessage.Content = (HttpContent) new ObjectContent(avatar.GetType(), (object) avatar, (MediaTypeFormatter) AvatarUtils.s_customJsonFormatter);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (format));
      }
      httpResponseMessage.Content.Headers.LastModified = new DateTimeOffset?(avatar.LastModified);
      httpResponseMessage.Headers.ETag = AvatarUtils.GetETag(avatar.LastModified);
      return httpResponseMessage;
    }

    public static HttpResponseMessage FormatResponse(
      HttpRequestMessage request,
      Microsoft.VisualStudio.Services.Profile.Avatar avatar,
      string format)
    {
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
      switch (AvatarUtils.ExtractAcceptType(request, format))
      {
        case AvatarUtils.ContentType.Png:
          httpResponseMessage.Content = (HttpContent) new ByteArrayContent(avatar.Value);
          httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
          break;
        case AvatarUtils.ContentType.Json:
          httpResponseMessage.Content = (HttpContent) new ObjectContent(avatar.GetType(), (object) avatar, (MediaTypeFormatter) AvatarUtils.s_customJsonFormatter);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (format));
      }
      httpResponseMessage.Content.Headers.LastModified = new DateTimeOffset?(avatar.TimeStamp);
      httpResponseMessage.Headers.ETag = AvatarUtils.GetETag(avatar.TimeStamp);
      return httpResponseMessage;
    }

    public static EntityTagHeaderValue GetETag(DateTimeOffset timeStamp) => EntityTagHeaderValue.Parse("\"" + timeStamp.GetHashCode().ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\"");

    public static EntityTagHeaderValue GetETag(Guid guid) => EntityTagHeaderValue.Parse("\"" + guid.GetHashCode().ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\"");

    public static bool IsResourceNotRequiredInResponse(
      HttpRequestMessage request,
      EntityTagHeaderValue resourceVersion,
      DateTimeOffset resourceLastModified)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      HttpHeaderValueCollection<EntityTagHeaderValue> ifNoneMatch = request.Headers.IfNoneMatch;
      if (ifNoneMatch != null && ifNoneMatch.Any<EntityTagHeaderValue>((Func<EntityTagHeaderValue, bool>) (etag => etag.Tag.Equals("\"*\"") || etag.Equals((object) resourceVersion))))
        return true;
      DateTimeOffset? ifModifiedSince = request.Headers.IfModifiedSince;
      DateTimeOffset dateTimeOffset = resourceLastModified;
      return (ifModifiedSince.HasValue ? (ifModifiedSince.GetValueOrDefault() >= dateTimeOffset ? 1 : 0) : 0) != 0;
    }

    public static bool IsActionRequiredForGivenRequest(
      HttpRequestMessage request,
      EntityTagHeaderValue resourceVersion,
      DateTimeOffset resourceLastModified)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      HttpHeaderValueCollection<EntityTagHeaderValue> ifMatch = request.Headers.IfMatch;
      DateTimeOffset? ifUnmodifiedSince = request.Headers.IfUnmodifiedSince;
      if (ifMatch != null && ifMatch.Any<EntityTagHeaderValue>((Func<EntityTagHeaderValue, bool>) (etag => etag.Tag.Equals("\"*\"") || etag.Equals((object) resourceVersion))))
        return true;
      if (ifUnmodifiedSince.HasValue)
      {
        DateTimeOffset dateTimeOffset = resourceLastModified.Subtract(AvatarUtils.s_OneSecond);
        DateTimeOffset? nullable = ifUnmodifiedSince;
        if ((nullable.HasValue ? (dateTimeOffset > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          return false;
      }
      return ifMatch != null && ifMatch.Count == 0;
    }

    internal static int GetUnsigned16BitIntFromGuidUsingMostSignificantBits(Guid g)
    {
      short int16 = BitConverter.ToInt16(g.ToByteArray(), 0);
      return int16 >= (short) 0 ? (int) int16 : (int) int16 * -1;
    }

    internal static Font MapToAvatarFont(int avatarSize, int numberOfInitials)
    {
      if (numberOfInitials < 0 || numberOfInitials > 2)
        throw new ArgumentOutOfRangeException(nameof (numberOfInitials), (object) numberOfInitials, "Number of initials must be one or two.");
      bool flag = numberOfInitials == 2;
      switch (avatarSize)
      {
        case 34:
          return !flag ? AvatarUtils.s_smallAvatarFontForOneLetter : AvatarUtils.s_smallAvatarFontForTwoLetters;
        case 44:
          return !flag ? AvatarUtils.s_mediumAvatarFontForOneLetter : AvatarUtils.s_mediumAvatarFontForTwoLetters;
        case 220:
          return !flag ? AvatarUtils.s_largeAvatarFontForOneLetter : AvatarUtils.s_largeAvatarFontForTwoLetters;
        default:
          throw new ArgumentException("The argument is not the supported value", nameof (avatarSize));
      }
    }

    internal static int MapToAvatarSizeInPixels(ImageSize size)
    {
      switch (size)
      {
        case ImageSize.Small:
          return 34;
        case ImageSize.Medium:
          return 44;
        case ImageSize.Large:
          return 220;
        default:
          throw new ArgumentException("The argument is out of the supported enum range", nameof (size));
      }
    }

    private static AvatarUtils.ContentType ExtractAcceptType(
      HttpRequestMessage request,
      string format)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.Headers == null)
        throw new ArgumentException("Request headers should not be null");
      return format == null ? (request.Headers.Accept.FirstOrDefault<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (x => AvatarUtils.IsPngMediaType(x.MediaType))) == null ? AvatarUtils.ContentType.Json : AvatarUtils.ContentType.Png) : (!AvatarUtils.IsPng(format) ? AvatarUtils.ContentType.Json : AvatarUtils.ContentType.Png);
    }

    private static bool IsPngMediaType(string str) => str != null && str.Equals("image/png", StringComparison.OrdinalIgnoreCase);

    private static bool IsPng(string str) => str != null && str.Equals("png", StringComparison.OrdinalIgnoreCase);

    private enum ContentType
    {
      Png,
      Json,
    }
  }
}
