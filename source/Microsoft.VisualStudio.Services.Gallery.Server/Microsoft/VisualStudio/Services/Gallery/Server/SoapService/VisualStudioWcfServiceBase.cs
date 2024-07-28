// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VisualStudioWcfServiceBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService
{
  public abstract class VisualStudioWcfServiceBase : GalleryWcfServiceBase
  {
    private const string ProjectMetadataPartInWhereClausePrefix = "Project.Metadata['SupportedVSEditions'] LIKE '%";

    protected VisualStudioIdeVersion GetClientVsIdeVersion(
      IDictionary<string, string> requestParameters,
      string whereClause,
      VisualStudioIdeVersion defaultVersion)
    {
      if (requestParameters != null)
      {
        string productVersion = (string) null;
        if (requestParameters.ContainsKey("VSVersion"))
          productVersion = requestParameters["VSVersion"];
        else if (whereClause != null)
        {
          int num1 = whereClause.IndexOf("Project.Metadata['SupportedVSEditions'] LIKE '%", StringComparison.OrdinalIgnoreCase);
          if (num1 != -1)
          {
            int num2 = whereClause.IndexOf("%", num1 + "Project.Metadata['SupportedVSEditions'] LIKE '%".Length, StringComparison.OrdinalIgnoreCase);
            if (num2 != -1)
            {
              int startIndex = num1 + "Project.Metadata['SupportedVSEditions'] LIKE '%".Length;
              int length = num2 - startIndex;
              string[] strArray = whereClause.Substring(startIndex, length).Split(',');
              if (strArray.Length == 2)
                productVersion = strArray[0].Trim();
            }
          }
        }
        if (productVersion != null)
          return ServiceHelper.GetVisualStudioIdeVersion(productVersion, defaultVersion);
      }
      return defaultVersion;
    }

    protected string GetSearchPurpose(string searchText, string whereClause)
    {
      if (!searchText.IsNullOrEmpty<char>())
        return "Search";
      return whereClause != null && whereClause.Contains("Project.Metadata['VsixID']") ? "Update" : "CategoryPage";
    }
  }
}
