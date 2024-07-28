// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemIconController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemIcons", ResourceVersion = 1)]
  public class WorkItemIconController : TfsApiController
  {
    private static readonly MediaTypeHeaderValue ImageSvg = new MediaTypeHeaderValue("image/svg+xml");
    private static readonly MediaTypeHeaderValue ImageXaml = new MediaTypeHeaderValue("image/xaml+xml");

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap) => exceptionMap.AddStatusCode<WorkItemTrackingIconNotFoundException>(HttpStatusCode.NotFound);

    [HttpGet]
    [ClientLocationId("4E1EB4A5-1970-4228-A682-EC48EB2DCA30")]
    [ClientResponseType(typeof (WorkItemIcon), "GetWorkItemIconJson", "application/json")]
    [ClientResponseType(typeof (Stream), "GetWorkItemIconSvg", "image/svg+xml")]
    [ClientResponseType(typeof (Stream), "GetWorkItemIconXaml", "image/xaml+xml")]
    [ClientExample("GET__workitem_icon.json", "Get work item icon", null, null)]
    public HttpResponseMessage GetWorkItemIcon(string icon, [FromUri(Name = "color")] string color = null, [FromUri(Name = "v")] int version = 2)
    {
      if (!WorkItemTypeIconUtils.Icons.Any<string>((Func<string, bool>) (i => i.Equals(icon, StringComparison.InvariantCultureIgnoreCase))))
        throw new WorkItemTrackingIconNotFoundException(icon);
      if (!string.IsNullOrWhiteSpace(color))
        CommonWITUtils.CheckColor(color);
      HttpRequestMessage request1 = this.Request;
      MediaTypeWithQualityHeaderValue qualityHeaderValue1;
      if (request1 == null)
      {
        qualityHeaderValue1 = (MediaTypeWithQualityHeaderValue) null;
      }
      else
      {
        HttpRequestHeaders headers = request1.Headers;
        if (headers == null)
        {
          qualityHeaderValue1 = (MediaTypeWithQualityHeaderValue) null;
        }
        else
        {
          HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept1 = headers.Accept;
          qualityHeaderValue1 = accept1 != null ? accept1.FirstOrDefault<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (accept => string.Equals(accept.MediaType, "application/json", StringComparison.InvariantCultureIgnoreCase))) : (MediaTypeWithQualityHeaderValue) null;
        }
      }
      if (qualityHeaderValue1 != null)
        return this.Request.CreateResponse<WorkItemIcon>(HttpStatusCode.OK, new WorkItemIcon()
        {
          Id = icon,
          Url = this.Request.RequestUri.AbsoluteUri
        });
      HttpRequestMessage request2 = this.Request;
      MediaTypeWithQualityHeaderValue qualityHeaderValue2;
      if (request2 == null)
      {
        qualityHeaderValue2 = (MediaTypeWithQualityHeaderValue) null;
      }
      else
      {
        HttpRequestHeaders headers = request2.Headers;
        if (headers == null)
        {
          qualityHeaderValue2 = (MediaTypeWithQualityHeaderValue) null;
        }
        else
        {
          HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept2 = headers.Accept;
          qualityHeaderValue2 = accept2 != null ? accept2.FirstOrDefault<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (accept => string.Equals(accept.MediaType, "image/xaml+xml", StringComparison.InvariantCultureIgnoreCase))) : (MediaTypeWithQualityHeaderValue) null;
        }
      }
      bool flag = qualityHeaderValue2 > null;
      string name1;
      string name2;
      string resourceName;
      if (flag)
      {
        name1 = "Path";
        name2 = "Fill";
        resourceName = "Resources.WorkItemIcon." + icon + ".xaml";
        MediaTypeHeaderValue imageXaml = WorkItemIconController.ImageXaml;
      }
      else
      {
        name1 = "path";
        name2 = "fill";
        resourceName = "Resources.WorkItemIcon." + icon + ".svg";
        MediaTypeHeaderValue imageSvg = WorkItemIconController.ImageSvg;
      }
      using (Stream embeddedResource = new EmbeddedResourcesHelper().GetEmbeddedResource(resourceName))
      {
        XmlDocument xmlDocument = new XmlDocument();
        using (XmlReader reader = XmlReader.Create(embeddedResource, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        }))
          xmlDocument.Load(reader);
        if (!string.IsNullOrWhiteSpace(color))
        {
          XmlNode xmlNode = xmlDocument.GetElementsByTagName(name1)[0];
          XmlAttribute attribute = xmlDocument.CreateAttribute(name2);
          attribute.Value = "#" + color;
          xmlNode.Attributes.Append(attribute);
        }
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) new StringContent(xmlDocument.OuterXml.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        response.Content.Headers.ContentType = flag ? new MediaTypeHeaderValue("image/xaml+xml") : new MediaTypeHeaderValue("image/svg+xml");
        response.Headers.CacheControl = new CacheControlHeaderValue()
        {
          Public = true,
          MaxAge = new TimeSpan?(new TimeSpan(30, 0, 0, 0))
        };
        return response;
      }
    }

    [HttpGet]
    [ClientLocationId("4E1EB4A5-1970-4228-A682-EC48EB2DCA30")]
    [ClientExample("GET__workitem_icons.json", "Get work item icons", null, null)]
    public IEnumerable<WorkItemIcon> GetWorkItemIcons()
    {
      IEnumerable<WorkItemIcon> source = WorkItemTypeIconUtils.Icons.Select<string, WorkItemIcon>((Func<string, WorkItemIcon>) (iconName => WorkItemIconFactory.Create(this.TfsRequestContext, iconName)));
      return source == null ? (IEnumerable<WorkItemIcon>) null : (IEnumerable<WorkItemIcon>) source.ToList<WorkItemIcon>();
    }
  }
}
