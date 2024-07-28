// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.RunCreateModelExtension
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public static class RunCreateModelExtension
  {
    public static void AddCustomTestField(
      this RunCreateModel runCreateModel,
      string fieldName,
      object fieldValue)
    {
      if (((fieldValue == null ? 1 : 0) | (fieldValue == null ? 0 : (string.IsNullOrEmpty(fieldValue as string) ? 1 : 0))) != 0 || string.IsNullOrEmpty(fieldName))
        return;
      runCreateModel.CustomTestFields.Add(new CustomTestField()
      {
        FieldName = fieldName,
        Value = fieldValue
      });
    }
  }
}
