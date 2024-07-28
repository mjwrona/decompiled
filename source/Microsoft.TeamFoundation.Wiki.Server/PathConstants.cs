// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PathConstants
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  [GenerateAllConstants(null)]
  public static class PathConstants
  {
    public const int MaximumPagePathLength = 235;
    public static readonly string[] ResourceNameInvalidCharacters = new string[2]
    {
      "/",
      "\\"
    };
    public static readonly string[] PageNameReservedCharacters = new string[1]
    {
      "#"
    };
    public static readonly string[] GitIllegalSpecialChars = new string[8]
    {
      SpecialChars.Colon,
      SpecialChars.Question,
      SpecialChars.Star,
      SpecialChars.Lessthan,
      SpecialChars.Greaterthan,
      SpecialChars.Hyphen,
      SpecialChars.Pipe,
      SpecialChars.Quote
    };
    public static readonly string[] GitIllegalSpecialCharEscapes = new string[8]
    {
      SpecialCharEncodings.Colon,
      SpecialCharEncodings.Question,
      SpecialCharEncodings.Star,
      SpecialCharEncodings.Lessthan,
      SpecialCharEncodings.Greaterthan,
      SpecialCharEncodings.Hyphen,
      SpecialCharEncodings.Pipe,
      SpecialCharEncodings.Quote
    };
    public static readonly string[] AttachmentNameReservedCharacters = new string[1]
    {
      "#"
    };
    private static readonly string[] s_allowedAttachmentFileTypesWithoutSvg = new string[33]
    {
      ".CS",
      ".CSV",
      ".DOC",
      ".DOCX",
      ".GIF",
      ".GZ",
      ".HTM",
      ".HTML",
      ".ICO",
      ".JPEG",
      ".JPG",
      ".JSON",
      ".LYR",
      ".MD",
      ".MOV",
      ".MP4",
      ".MPP",
      ".MSG",
      ".PDF",
      ".PNG",
      ".PPT",
      ".PPTX",
      ".PS1",
      ".RAR",
      ".RDP",
      ".SQL",
      ".TXT",
      ".VSD",
      ".VSDX",
      ".XLS",
      ".XLSX",
      ".XML",
      ".ZIP"
    };
    private static readonly string[] s_allowedAttachmentFileTypesWithSvg;

    public static string[] AllowedAttachmentFileTypes(bool allowSvg) => !allowSvg ? PathConstants.s_allowedAttachmentFileTypesWithoutSvg : PathConstants.s_allowedAttachmentFileTypesWithSvg;

    static PathConstants()
    {
      string[] fileTypesWithoutSvg = PathConstants.s_allowedAttachmentFileTypesWithoutSvg;
      int index = 0;
      string[] strArray = new string[1 + fileTypesWithoutSvg.Length];
      foreach (string str in fileTypesWithoutSvg)
      {
        strArray[index] = str;
        ++index;
      }
      strArray[index] = ".SVG";
      int num = index + 1;
      PathConstants.s_allowedAttachmentFileTypesWithSvg = strArray;
    }
  }
}
