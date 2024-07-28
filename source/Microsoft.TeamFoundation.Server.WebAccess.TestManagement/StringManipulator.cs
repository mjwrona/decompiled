// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.StringManipulator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class StringManipulator
  {
    public static string EscapeCharacters(string attachmentName)
    {
      if (attachmentName != null)
      {
        try
        {
          attachmentName = Uri.EscapeDataString(attachmentName);
        }
        catch (UriFormatException ex)
        {
        }
      }
      return attachmentName;
    }
  }
}
