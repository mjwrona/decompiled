// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExceptionHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExceptionHelper
  {
    internal static object[] ExtractStrings(SqlError sqlError, params object[] stringNames)
    {
      List<object> objectList = new List<object>(stringNames.Length);
      if (sqlError != null && stringNames.Length != 0)
      {
        foreach (string stringName in stringNames)
          objectList.Add((object) TeamFoundationServiceException.ExtractString(sqlError, stringName));
      }
      return objectList.ToArray();
    }
  }
}
