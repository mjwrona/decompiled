// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.NameValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class NameValidation
  {
    public static bool IsValid(string name, bool allowHyphens = false)
    {
      bool flag = true;
      for (int index = 0; index < name.Length; ++index)
      {
        if ((name[index] < 'a' || name[index] > 'z') && (name[index] < 'A' || name[index] > 'Z') && (name[index] < '0' || name[index] > '9' || index <= 0) && name[index] != '_' && (!allowHyphens || name[index] != '-' || index <= 0))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public static string Sanitize(string name, bool allowHyphens = false)
    {
      if (name == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < name.Length; ++index)
      {
        if (name[index] >= 'a' && name[index] <= 'z' || name[index] >= 'A' && name[index] <= 'Z' || name[index] >= '0' && name[index] <= '9' && stringBuilder.Length > 0 || name[index] == '_' || allowHyphens && name[index] == '-' && stringBuilder.Length > 0)
          stringBuilder.Append(name[index]);
      }
      return stringBuilder.ToString();
    }
  }
}
