// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.ImportServiceInstanceExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public static class ImportServiceInstanceExtensions
  {
    public static string ToJson(
      this IEnumerable<ImportServiceInstance> instanceData)
    {
      return instanceData.Serialize<IEnumerable<ImportServiceInstance>>();
    }

    public static ImportServiceInstance[] ToImportServiceInstances(this string instanceData) => JsonUtilities.Deserialize<ImportServiceInstance[]>(instanceData);
  }
}
