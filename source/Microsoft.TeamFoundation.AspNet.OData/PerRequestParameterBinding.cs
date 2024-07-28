// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PerRequestParameterBinding
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Microsoft.AspNet.OData
{
  internal class PerRequestParameterBinding : HttpParameterBinding
  {
    private IEnumerable<MediaTypeFormatter> _formatters;

    public PerRequestParameterBinding(
      HttpParameterDescriptor descriptor,
      IEnumerable<MediaTypeFormatter> formatters)
      : base(descriptor)
    {
      this._formatters = formatters != null ? formatters : throw Error.ArgumentNull(nameof (formatters));
    }

    public override bool WillReadBody => true;

    public override Task ExecuteBindingAsync(
      ModelMetadataProvider metadataProvider,
      HttpActionContext actionContext,
      CancellationToken cancellationToken)
    {
      List<MediaTypeFormatter> perRequestFormatters = new List<MediaTypeFormatter>();
      foreach (MediaTypeFormatter formatter in this._formatters)
      {
        MediaTypeFormatter formatterInstance = formatter.GetPerRequestFormatterInstance(this.Descriptor.ParameterType, actionContext.Request, actionContext.Request.Content.Headers.ContentType);
        perRequestFormatters.Add(formatterInstance);
      }
      return this.CreateInnerBinding((IEnumerable<MediaTypeFormatter>) perRequestFormatters).ExecuteBindingAsync(metadataProvider, actionContext, cancellationToken);
    }

    protected virtual HttpParameterBinding CreateInnerBinding(
      IEnumerable<MediaTypeFormatter> perRequestFormatters)
    {
      return this.Descriptor.BindWithFormatter(perRequestFormatters);
    }
  }
}
