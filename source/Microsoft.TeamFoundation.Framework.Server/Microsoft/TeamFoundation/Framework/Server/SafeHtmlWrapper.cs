// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SafeHtmlWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SafeHtmlWrapper
  {
    private static string reservedForPercent40 = Guid.NewGuid().ToString();
    private static string reservedForPercent3b = Guid.NewGuid().ToString();
    private static string reservedForPercent2f = Guid.NewGuid().ToString();
    private static string reservedForPercent24 = Guid.NewGuid().ToString();
    private static string reservedForPercent21 = Guid.NewGuid().ToString();
    private static string beginMentionReserved = Guid.NewGuid().ToString();
    private static string endMentionReserved = Guid.NewGuid().ToString();

    public static string MakeSafeWithHtmlEncode(string inputHtml, bool removeLinks = false) => HttpUtility.HtmlEncode(SafeHtmlWrapper.MakeSafe(inputHtml, removeLinks)).Replace("&#10;", "<br/>").Replace("&amp;", "&");

    public static string MakeSafe(string inputHtml, bool removeLinks = false)
    {
      bool isSafeHtml = false;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      SafeHtmlWrapper.EditOrDropUrls urlHandlingCallback = !removeLinks ? SafeHtmlWrapper.\u003C\u003EO.\u003C1\u003E__TfsUrlsFilter ?? (SafeHtmlWrapper.\u003C\u003EO.\u003C1\u003E__TfsUrlsFilter = new SafeHtmlWrapper.EditOrDropUrls(SafeHtmlWrapper.TfsUrlsFilter)) : SafeHtmlWrapper.\u003C\u003EO.\u003C0\u003E__TfsUrlsCleanup ?? (SafeHtmlWrapper.\u003C\u003EO.\u003C0\u003E__TfsUrlsCleanup = new SafeHtmlWrapper.EditOrDropUrls(SafeHtmlWrapper.TfsUrlsCleanup));
      return SafeHtmlWrapper.GetSafeHtml(inputHtml, 1200, 1200, 576, urlHandlingCallback, out isSafeHtml);
    }

    public static string MakeSafeWithMdMentions(string text)
    {
      string replacement = SafeHtmlWrapper.beginMentionReserved + "${guid}" + SafeHtmlWrapper.endMentionReserved;
      return Regex.Replace(Regex.Replace(SafeHtmlWrapper.MakeSafe(Regex.Replace(text, "@<(?<guid>[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12})>", replacement)), SafeHtmlWrapper.beginMentionReserved, "@<"), SafeHtmlWrapper.endMentionReserved, ">");
    }

    public static unsafe string GetSafeHtml(
      string inputHtml,
      int inputHtmlCodepage,
      int outputHtmlCodepage,
      int safeHtmlFlags,
      SafeHtmlWrapper.EditOrDropUrls urlHandlingCallback,
      out bool isSafeHtml)
    {
      isSafeHtml = true;
      if (inputHtml == null)
        return string.Empty;
      inputHtml = inputHtml.Replace("%40", SafeHtmlWrapper.reservedForPercent40);
      inputHtml = inputHtml.Replace("%3b", SafeHtmlWrapper.reservedForPercent3b);
      inputHtml = inputHtml.Replace("%3B", SafeHtmlWrapper.reservedForPercent3b);
      inputHtml = inputHtml.Replace("%2F", SafeHtmlWrapper.reservedForPercent2f);
      inputHtml = inputHtml.Replace("%2f", SafeHtmlWrapper.reservedForPercent2f);
      inputHtml = inputHtml.Replace("%24", SafeHtmlWrapper.reservedForPercent24);
      inputHtml = inputHtml.Replace("%21", SafeHtmlWrapper.reservedForPercent21);
      byte[] bytes1 = Encoding.Unicode.GetBytes(inputHtml);
      string safeHtml = string.Empty;
      byte* pv = (byte*) null;
      int cbDst = 0;
      int tagHandlingResultFlags = 0;
      if (!SafeHtmlWrapper.NativeMethods.OshFGetSafeHTMLAllocForManaged3(bytes1, bytes1.Length, inputHtmlCodepage, &pv, out cbDst, outputHtmlCodepage, safeHtmlFlags, (string[]) null, (int[]) null, 0, out tagHandlingResultFlags, urlHandlingCallback, ref isSafeHtml))
      {
        if ((IntPtr) pv != IntPtr.Zero)
          SafeHtmlWrapper.NativeMethods.OshFreePv((void*) pv);
        return (string) null;
      }
      if (cbDst > 0)
      {
        byte[] bytes2 = new byte[cbDst];
        for (int index = 0; index < cbDst; ++index)
          bytes2[index] = pv[index];
        SafeHtmlWrapper.NativeMethods.OshFreePv((void*) pv);
        safeHtml = new string(Encoding.Unicode.GetChars(bytes2));
        if (!string.IsNullOrEmpty(safeHtml))
          safeHtml = safeHtml.Replace("&#160;", "&nbsp;").Replace("%40", "@").Replace(SafeHtmlWrapper.reservedForPercent40, "%40").Replace("%3b", ";").Replace(SafeHtmlWrapper.reservedForPercent3b, "%3b").Replace("%2f", "/").Replace(SafeHtmlWrapper.reservedForPercent2f, "%2f").Replace("%24", "$").Replace(SafeHtmlWrapper.reservedForPercent24, "%24").Replace("%21", "!").Replace(SafeHtmlWrapper.reservedForPercent21, "%21");
      }
      return safeHtml;
    }

    public static bool TfsUrlsFilter(
      IntPtr unusedPointer,
      int tagId,
      int argId,
      ref bool shouldDrop,
      ref bool shouldFree,
      [MarshalAs(UnmanagedType.LPWStr)] string urlIn,
      int urlInLength,
      ref IntPtr urlOut,
      ref int urlOutLength)
    {
      Uri result;
      if (shouldDrop && urlIn != null && urlIn.Length >= urlInLength && Uri.TryCreate(urlIn.Substring(0, urlInLength), UriKind.Absolute, out result))
        shouldDrop = UriUtility.IsUriUnsafe(result);
      return true;
    }

    private static bool TfsUrlsCleanup(
      IntPtr unusedPointer,
      int tagId,
      int argId,
      ref bool shouldDrop,
      ref bool shouldFree,
      [MarshalAs(UnmanagedType.LPWStr)] string urlIn,
      int urlInLength,
      ref IntPtr urlOut,
      ref int urlOutLength)
    {
      if (tagId == 0 && urlIn != null && urlIn.Length >= urlInLength)
        shouldDrop = true;
      return true;
    }

    public class Flag
    {
      public const int oshfFragment = 64;
      public const int oshfNoWriteBOM = 512;
    }

    public class CodePage
    {
      public const int oshcpUnicodeLittle = 1200;
      public const int oshcpUnicode = 1200;
    }

    private static class NativeMethods
    {
      [DllImport("osafehtm.dll")]
      internal static extern unsafe bool OshFGetSafeHTMLAllocForManaged3(
        byte[] rgbSrc,
        int cbSrc,
        int cpSrc,
        byte** rgbDst,
        out int cbDst,
        int cpDst,
        int grfosh,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] tagsToHandle,
        int[] tagHandlingFlags,
        int tagHandlingCount,
        out int tagHandlingResultFlags,
        [MarshalAs(UnmanagedType.FunctionPtr)] SafeHtmlWrapper.EditOrDropUrls urlHandlingCallback,
        ref bool isSafeHtml);

      [DllImport("osafehtm.dll")]
      public static extern unsafe void* OshPvAlloc(int cb);

      [DllImport("osafehtm.dll")]
      public static extern unsafe void OshFreePv(void* pv);
    }

    public delegate bool EditOrDropUrls(
      IntPtr unusedPointer,
      int tag,
      int arg,
      ref bool drop,
      ref bool free,
      [MarshalAs(UnmanagedType.LPWStr)] string urlIn,
      int urlInLength,
      ref IntPtr urlOut,
      ref int urlOutLength);
  }
}
