// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ModelValidationHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ModelValidationHelper
  {
    public static void Validate(ApiController controller, object model, string modelName)
    {
      if (controller == null)
        throw new ArgumentNullException(nameof (controller));
      if (model == null)
        throw new ArgumentNullException(string.IsNullOrWhiteSpace(modelName) ? nameof (model) : modelName);
      controller.Validate<object>(model);
      if (!controller.ModelState.IsValid)
      {
        string name = model.GetType().Name;
        KeyValuePair<string, ModelState> keyValuePair = controller.ModelState.First<KeyValuePair<string, ModelState>>();
        ModelError modelError = keyValuePair.Value.Errors.FirstOrDefault<ModelError>();
        string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InputForProperty0In1IsNotValid, (object) keyValuePair.Key, (object) name);
        throw new ArgumentException(modelError == null || string.IsNullOrWhiteSpace(modelError.ErrorMessage) ? str : modelError.ErrorMessage, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) name, (object) keyValuePair.Key));
      }
    }
  }
}
