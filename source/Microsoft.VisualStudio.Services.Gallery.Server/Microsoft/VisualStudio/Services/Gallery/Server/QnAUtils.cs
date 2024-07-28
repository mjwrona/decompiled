// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.QnAUtils
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class QnAUtils : IQnAUtils
  {
    private readonly string area;
    private readonly string layer;

    public QnAUtils(string area, string layer)
    {
      this.area = area;
      this.layer = layer;
    }

    public QnAItem SanitizeItem(QnAItem qnAItem)
    {
      ArgumentUtility.CheckForNull<QnAItem>(qnAItem, nameof (qnAItem));
      qnAItem.Text = qnAItem.Text.Trim();
      qnAItem.Text = GalleryServerUtil.TruncateString(qnAItem.Text, 2048);
      return qnAItem;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetAuthenticatedIdentity(
      IVssRequestContext requestContext)
    {
      return requestContext.GetUserIdentity();
    }

    public PublishedExtension ValidateAndGetPublishedExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      bool flag = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      int num = flag ? 1 : 0;
      PublishedExtension var = service.QueryExtension(requestContext1, publisherName1, extensionName1, (string) null, ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly, (string) null, num != 0);
      ArgumentUtility.CheckForNull<PublishedExtension>(var, "extension");
      return var;
    }

    public bool IsUserPublisherWithRequiredPermissions(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<PublisherFacts>(extension.Publisher, "extension.Publisher");
      ArgumentUtility.CheckForNull<string>(extension.Publisher.PublisherName, "extension.Publisher.PublisherName");
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      Publisher publisher = vssRequestContext.GetService<IPublisherService>().QueryPublisher(vssRequestContext, extension.Publisher.PublisherName, PublisherQueryFlags.None);
      return GallerySecurity.HasPublisherPermission(requestContext, publisher, PublisherPermissions.Read | PublisherPermissions.UpdateExtension | PublisherPermissions.PublishExtension | PublisherPermissions.PrivateRead | PublisherPermissions.DeleteExtension | PublisherPermissions.ViewPermissions) || GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
    }

    public QnAItem ConvertToQnAItem(
      IVssRequestContext requestContext,
      ExtensionQnAItem extensionQnAItem)
    {
      QnAItem qnAitem = (QnAItem) null;
      if (extensionQnAItem != null)
      {
        QnAItem qnAItem;
        if (extensionQnAItem.IsQuestion)
        {
          Question question = new Question();
          question.Id = extensionQnAItem.Id;
          question.Text = extensionQnAItem.Text;
          question.CreatedDate = extensionQnAItem.CreatedDate;
          question.UpdatedDate = extensionQnAItem.UpdatedDate;
          question.Status = this.GetQnAItemStatus(extensionQnAItem);
          question.User = new UserIdentityRef()
          {
            Id = extensionQnAItem.UserId
          };
          question.Responses = new List<Response>();
          qnAItem = (QnAItem) question;
        }
        else
        {
          Response response = new Response();
          response.Id = extensionQnAItem.Id;
          response.Text = extensionQnAItem.Text;
          response.CreatedDate = extensionQnAItem.CreatedDate;
          response.UpdatedDate = extensionQnAItem.UpdatedDate;
          response.Status = this.GetQnAItemStatus(extensionQnAItem);
          response.User = new UserIdentityRef()
          {
            Id = extensionQnAItem.UserId
          };
          qnAItem = (QnAItem) response;
        }
        qnAitem = this.PopulateUserInfo(requestContext, qnAItem);
      }
      return qnAitem;
    }

    public QnAMode GetQnAMode(IDictionary<string, string> extensionProperties)
    {
      QnAMode qnAmode = QnAMode.MarketplaceQnA;
      if (extensionProperties != null)
      {
        if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.CustomerQnALink"))
          return QnAMode.CustomQnA;
        if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.EnableMarketplaceQnA"))
        {
          string str = "";
          if (extensionProperties.TryGetValue("Microsoft.VisualStudio.Services.EnableMarketplaceQnA", out str) && str.Equals("false", StringComparison.OrdinalIgnoreCase))
            qnAmode = QnAMode.None;
          return qnAmode;
        }
        if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.Links.GitHub"))
          qnAmode = QnAMode.GitHubIssues;
      }
      return qnAmode;
    }

    public void PublishReCaptchaTokenCIForQnA(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(ciData, nameof (ciData));
      Guid userId = requestContext.GetUserId();
      ciData.Add("Vsid", (object) userId);
      CustomerIntelligenceData properties = new CustomerIntelligenceData(ciData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ReCaptchaValidation", properties);
    }

    private QnAItem PopulateUserInfo(IVssRequestContext requestContext, QnAItem qnAItem)
    {
      if (qnAItem == null)
        return qnAItem;
      if (qnAItem.Status.HasFlag((Enum) QnAItemStatus.PublisherCreated))
      {
        qnAItem.User = (UserIdentityRef) null;
        return qnAItem;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      qnAItem.User.DisplayName = string.Empty;
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (!Guid.Empty.Equals(qnAItem.User.Id))
          identity = service.ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
          {
            qnAItem.User.Id
          }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
        qnAItem.User.DisplayName = identity?.DisplayName;
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(12062021, this.area, this.layer, ex);
      }
      if (string.IsNullOrEmpty(qnAItem.User.DisplayName))
      {
        qnAItem.User.DisplayName = GalleryResources.DeletedUser();
        qnAItem.User.Id = Guid.Empty;
      }
      return qnAItem;
    }

    private QnAItemStatus GetQnAItemStatus(ExtensionQnAItem extensionQnAItem)
    {
      QnAItemStatus qnAitemStatus = QnAItemStatus.None;
      if (extensionQnAItem.IsPublisherCreated)
        qnAitemStatus |= QnAItemStatus.PublisherCreated;
      return qnAitemStatus;
    }
  }
}
