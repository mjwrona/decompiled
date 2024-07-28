// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.CreateAndUploadReportsResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  public class CreateAndUploadReportsResult
  {
    public int SucceededDescriptors { get; set; }

    public IEnumerable<string> FailedDescriptors { get; set; }

    public int FileId { get; set; }
  }
}
