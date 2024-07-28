// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitQueryCommitsCriteriaModelBinder
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitQueryCommitsCriteriaModelBinder : IModelBinder
  {
    public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
    {
      if (bindingContext.ModelType != typeof (GitQueryCommitsCriteria) || !new CompositeModelBinderProvider(actionContext.ControllerContext.Configuration.Services.GetModelBinderProviders()).GetBinder(actionContext.ControllerContext.Configuration, typeof (GitQueryCommitsCriteria)).BindModel(actionContext, bindingContext))
        return false;
      string rawValue1 = (string) bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + "$top")?.RawValue;
      string rawValue2 = (string) bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + "$skip")?.RawValue;
      object rawValue3 = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + "ids")?.RawValue;
      string rawValue4 = (string) bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + "user")?.RawValue;
      (bindingContext.Model as GitQueryCommitsCriteria).Top = this.GetIntValueFromString(rawValue1);
      (bindingContext.Model as GitQueryCommitsCriteria).Skip = this.GetIntValueFromString(rawValue2);
      if (!string.IsNullOrEmpty(rawValue4))
        (bindingContext.Model as GitQueryCommitsCriteria).Committer = rawValue4;
      if (rawValue3 is string)
        (bindingContext.Model as GitQueryCommitsCriteria).Ids = this.GetListValueFromString((string) rawValue3);
      return true;
    }

    private List<string> GetListValueFromString(string valueString)
    {
      if (string.IsNullOrEmpty(valueString))
        return (List<string>) null;
      return new List<string>((IEnumerable<string>) valueString.Split(','));
    }

    private int? GetIntValueFromString(string valueString)
    {
      int result;
      return !string.IsNullOrEmpty(valueString) && int.TryParse(valueString, out result) ? new int?(result) : new int?();
    }
  }
}
