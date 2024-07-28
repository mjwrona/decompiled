// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServices.UxServicesProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.UxServices
{
  public class UxServicesProvider : IContentProvider
  {
    private readonly IContentService uxService;
    private readonly IVssRequestContext tfRequestContext;

    public UxServicesProvider(IContentService uxServiceWrapper, IVssRequestContext requestContext)
    {
      this.uxService = uxServiceWrapper;
      this.tfRequestContext = requestContext;
    }

    public MvcHtmlString GetData(UxServicesRequestData uxRequestData)
    {
      if (uxRequestData == null)
        throw new ArgumentNullException(nameof (uxRequestData));
      IUxServicesCacheService service = this.tfRequestContext.GetService<IUxServicesCacheService>();
      string contextKey = this.GetContextKey(uxRequestData);
      object obj;
      string templateData;
      if (service.TryGetValue(this.tfRequestContext.To(TeamFoundationHostType.Deployment), contextKey, out obj))
      {
        service.RefreshUxServicesCacheConfiguration(this.tfRequestContext);
        templateData = obj as string;
      }
      else
      {
        templateData = this.uxService.GetContent(uxRequestData);
        service.Set(this.tfRequestContext.To(TeamFoundationHostType.Deployment), contextKey, (object) templateData);
      }
      return new MvcHtmlString(this.GetTemplateContent(uxRequestData, templateData));
    }

    private string GetContextKey(UxServicesRequestData uxRequestData)
    {
      List<string> values = new List<string>();
      if (!string.IsNullOrEmpty(uxRequestData.Brand))
        values.Add(uxRequestData.Brand);
      if (!string.IsNullOrEmpty(uxRequestData.Control))
        values.Add(uxRequestData.Control);
      if (!string.IsNullOrEmpty(uxRequestData.Locale))
        values.Add(uxRequestData.Locale);
      if (!string.IsNullOrEmpty(uxRequestData.Site))
        values.Add(uxRequestData.Site);
      return values.StringJoin<string>('_').ToLowerInvariant();
    }

    private string GetTemplateContent(UxServicesRequestData uxRequestData, string templateData)
    {
      foreach (IContentTransformer contentTransformer in ((IEnumerable<IContentTransformer>) new IContentTransformer[1]
      {
        (IContentTransformer) new SignInPlaceholderTransformer()
      }).Where<IContentTransformer>((Func<IContentTransformer, bool>) (contentTransformer => contentTransformer.CanHandleRequest(uxRequestData, templateData))))
        templateData = contentTransformer.TransformContent(uxRequestData, templateData);
      return templateData;
    }
  }
}
