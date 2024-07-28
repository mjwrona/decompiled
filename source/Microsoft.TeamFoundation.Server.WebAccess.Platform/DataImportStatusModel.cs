// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DataImportStatusModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DataImportStatusModel
  {
    public string Title { get; set; }

    public string Image { get; set; }

    public string StepTitle { get; set; }

    public string StepCounter { get; set; }

    public IHtmlString StepDescription { get; set; }

    public string LoadingImage { get; set; }

    public string LastUpdate { get; set; }

    public List<DataImportFileTransferProgressMetricModel> FileTransferProgress { get; set; }
  }
}
