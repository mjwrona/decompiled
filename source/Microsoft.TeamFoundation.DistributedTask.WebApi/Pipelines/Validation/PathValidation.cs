// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.PathValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  internal class PathValidation
  {
    public static readonly int MaxGroupPathLength = 400;
    private const string c_pathSeparator = "\\";
    private static readonly char[] s_illegalNameChars = new char[2]
    {
      '%',
      '+'
    };
    private static readonly int s_maxGroupPathPartLength = 248;

    public static bool IsValid(ref string group)
    {
      bool flag = true;
      if (group.IsNullOrEmpty<char>())
        return flag;
      try
      {
        group = PathValidation.GetFullPath(group);
        ArgumentUtility.CheckStringForInvalidCharacters(group, nameof (group), PathValidation.s_illegalNameChars);
        if (!Wildcard.IsWildcard(group))
        {
          if (group.Length <= PathValidation.MaxGroupPathLength)
            goto label_6;
        }
        flag = false;
      }
      catch
      {
        flag = false;
      }
label_6:
      return flag;
    }

    public static string GetFullPath(string path, bool allowEmptyRootPath = true)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
      {
        if (!allowEmptyRootPath)
          throw new InvalidPathException("Empty root path is not allowed!");
        stringBuilder.Append('\\');
      }
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], PathValidation.s_maxGroupPathPartLength, true, out error))
          throw new InvalidPathException(error);
        stringBuilder.AppendFormat("{0}{1}", (object) "\\", (object) strArray[index]);
      }
      return stringBuilder.ToString();
    }
  }
}
