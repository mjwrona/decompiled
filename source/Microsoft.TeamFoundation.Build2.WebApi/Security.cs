// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Security
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Security
  {
    public static readonly char NamespaceSeparator = '/';
    public static readonly int MaxPathNameLength = 248;
    public const string BuildNamespaceIdString = "33344D9C-FC72-4d6f-ABA5-FA317101A7E9";
    public static readonly Guid BuildNamespaceId = new Guid("33344D9C-FC72-4d6f-ABA5-FA317101A7E9");

    public static string GetSecurityTokenPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return Security.NamespaceSeparator.ToString();
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return Security.NamespaceSeparator.ToString();
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], Security.MaxPathNameLength, true, out error))
          throw new InvalidPathException(error);
        stringBuilder.AppendFormat("{0}{1}", (object) Security.NamespaceSeparator, (object) strArray[index]);
      }
      stringBuilder.Append(Security.NamespaceSeparator);
      return stringBuilder.ToString();
    }
  }
}
