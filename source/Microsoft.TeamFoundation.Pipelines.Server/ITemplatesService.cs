// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ITemplatesService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (TemplatesService))]
  public interface ITemplatesService : IVssFrameworkService
  {
    Template GetTemplate(
      IVssRequestContext requestContext,
      string templateId,
      CultureInfo cultureInfo = null);

    IEnumerable<Template> GetTemplates(IVssRequestContext requestContext, CultureInfo cultureInfo = null);

    Template RenderTemplate(
      IVssRequestContext requestContext,
      string templateId,
      IReadOnlyDictionary<string, object> parameters,
      CultureInfo cultureInfo = null);
  }
}
