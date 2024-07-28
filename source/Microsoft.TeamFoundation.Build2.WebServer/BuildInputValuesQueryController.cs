// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildInputValuesQueryController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "InputValuesQuery", ResourceVersion = 2)]
  public class BuildInputValuesQueryController : BuildApiController
  {
    public const string RepositoryTypeKey = "repositoryType";

    [HttpPost]
    [ClientIgnore]
    public InputValuesQuery PostInputValuesQuery([FromBody] InputValuesQuery inputValuesQuery)
    {
      this.CheckRequestContent((object) inputValuesQuery);
      ArgumentUtility.CheckForNull<IList<InputValues>>(inputValuesQuery.InputValues, "inputValuesQuery.InputValues");
      string repositoryType = (string) null;
      if (inputValuesQuery.CurrentValues.ContainsKey("repositoryType"))
        repositoryType = inputValuesQuery.CurrentValues["repositoryType"];
      IBuildSourceProvider buildSourceProvider = repositoryType != null ? this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType) : throw new MissingRequiredParameterException("repositoryType");
      inputValuesQuery.InputValues = buildSourceProvider.GetInputValues(this.TfsRequestContext, this.ProjectId, inputValuesQuery.InputValues.Select<InputValues, string>((Func<InputValues, string>) (i => i.InputId)), inputValuesQuery.CurrentValues);
      foreach (string secretPropertyName in buildSourceProvider.GetRepositorySecretPropertyNames())
      {
        if (inputValuesQuery.CurrentValues.ContainsKey(secretPropertyName))
          inputValuesQuery.CurrentValues.Remove(secretPropertyName);
      }
      return inputValuesQuery;
    }
  }
}
