// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.NoContentGutterHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal class NoContentGutterHelper
  {
    public NoContentGutterModel GetIterationNoContentGutterModel(
      UrlHelper urlHelper,
      string currentIterationPath)
    {
      NoContentGutterModel contentGutterModel = new NoContentGutterModel();
      contentGutterModel.Heading = AgileViewResources.Iteration_NoData_NoItemsToShow_Heading;
      contentGutterModel.Message = string.Format(AgileViewResources.Iteration_NoData_NoItemsToShow_Message, (object) ("<em>" + currentIterationPath + "</em>"));
      contentGutterModel.Actions = new NoContentGutterActionModel[1]
      {
        new NoContentGutterActionModel()
        {
          Href = urlHelper.ActionWithParameters("", "backlogs", (object) new
          {
            parameters = ""
          }),
          Text = AgileViewResources.Iteration_NoData_NoItemsToShow_Action
        }
      };
      return contentGutterModel;
    }
  }
}
