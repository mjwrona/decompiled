// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.DefinitionEnvironmentTemplateValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class DefinitionEnvironmentTemplateValidations
  {
    public static ReleaseDefinitionEnvironmentTemplate Validate(
      this ReleaseDefinitionEnvironmentTemplate template,
      IVssRequestContext context,
      Guid projectId)
    {
      return DefinitionEnvironmentTemplateValidations.ValidateImplementation(template, (Action<ReleaseDefinitionEnvironment>) (e => e.Validate(context, projectId)));
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This parameter is being passed by wireup method")]
    public static ReleaseDefinitionEnvironmentTemplate ValidateImplementation(
      ReleaseDefinitionEnvironmentTemplate template,
      Action<ReleaseDefinitionEnvironment> validateEnvironment)
    {
      if (template == null)
        throw new ArgumentNullException(nameof (template));
      if (string.IsNullOrWhiteSpace(template.Name))
        throw new InvalidRequestException(Resources.TemplateNameCannotBeEmpty);
      if (template.Name.Length > 256)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseManagementObjectNameLengthExceeded, (object) template.GetType().Name, (object) template.Name, (object) 256));
      if (template.Environment == null)
        throw new InvalidRequestException(Resources.EnvironmentCannotBeEmpty);
      if (string.IsNullOrEmpty(template.Environment.Name))
        template.Environment.Name = "TemplateEnvironmentName";
      validateEnvironment(template.Environment);
      return template;
    }
  }
}
