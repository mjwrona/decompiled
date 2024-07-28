// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ArtifactTypeInputValuesQueryService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ArtifactTypeInputValuesQueryService : ArtifactTypeServiceBase
  {
    public InputValuesQuery GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      InputValuesQuery query)
    {
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      ArtifactTypeBase type;
      if (!this.IsValidArtifactType(context, query, out type))
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactTypeNotFound, (object) (query.Resource as string)));
      List<InputValues> inputValuesList = new List<InputValues>();
      if (query.InputValues != null)
      {
        foreach (InputValues inputValue in (IEnumerable<InputValues>) query.InputValues)
        {
          InputValues inputValues = type.GetInputValues(context, projectInfo, inputValue.InputId, query.CurrentValues);
          inputValuesList.Add(inputValues);
        }
      }
      query.InputValues = (IList<InputValues>) inputValuesList;
      return query;
    }

    private bool IsValidArtifactType(
      IVssRequestContext context,
      InputValuesQuery query,
      out ArtifactTypeBase type)
    {
      string resource = query.Resource as string;
      type = (ArtifactTypeBase) null;
      if (!string.IsNullOrEmpty(resource))
        type = this.GetArtifactType(context, resource, false);
      return type != null;
    }
  }
}
