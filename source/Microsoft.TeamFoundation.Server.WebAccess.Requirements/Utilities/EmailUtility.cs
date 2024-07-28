// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities.EmailUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities
{
  public static class EmailUtility
  {
    private static string NoteTextString = "Note: {0}";
    private static string LineFeedCharacter = "&#10;";
    private static string BrTag = "<br/>";
    private static int maxNoteSize = 1024;

    public static string ConstructNotesText(string notes)
    {
      int num = notes == null ? 0 : (notes.Trim().Length > 0 ? 1 : 0);
      string empty = string.Empty;
      string str = string.Empty;
      if (num != 0)
      {
        if (notes.Length > EmailUtility.maxNoteSize)
          notes = notes.Substring(0, EmailUtility.maxNoteSize);
        empty += string.Format(EmailUtility.NoteTextString, (object) notes);
      }
      if (!string.IsNullOrEmpty(empty))
        str = SafeHtmlWrapper.MakeSafeWithHtmlEncode(empty, true).Replace(EmailUtility.LineFeedCharacter, EmailUtility.BrTag);
      return str;
    }
  }
}
