// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.GlobalMessageService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  public class GlobalMessageService : IGlobalMessageService, IVssFrameworkService
  {
    private const string c_sharedDataKey = "globalMessageData";
    private const string c_sharedDialogDataKey = "globalDialogData";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddDialog(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalDialog dialog)
    {
      this.ProcessMessageLinks(requestContext, (GlobalMessage) dialog);
      GlobalDialogData globalDialogData = this.GetGlobalDialogData(requestContext, sharedData);
      if (globalDialogData.Dialogs == null)
      {
        globalDialogData.Dialogs = (IList<GlobalDialog>) new List<GlobalDialog>()
        {
          dialog
        };
      }
      else
      {
        for (int index = 0; index < globalDialogData.Dialogs.Count; ++index)
        {
          if (globalDialogData.Dialogs[index].Priority < dialog.Priority)
          {
            globalDialogData.Dialogs.Insert(index, dialog);
            return;
          }
        }
        globalDialogData.Dialogs.Add(dialog);
      }
    }

    public void AddMessageBanner(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string message,
      WebMessageLevel messageLevel)
    {
      GlobalMessageBanner globalMessageBanner1 = new GlobalMessageBanner();
      globalMessageBanner1.Message = message;
      globalMessageBanner1.Level = messageLevel;
      GlobalMessageBanner globalMessageBanner2 = globalMessageBanner1;
      this.ProcessMessageLinks(requestContext, (GlobalMessage) globalMessageBanner2);
      this.AddMessageBanner(requestContext, sharedData, globalMessageBanner2);
    }

    public void AddMessageBanner(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalMessageBanner banner)
    {
      GlobalMessagesData globalMessageData = this.GetGlobalMessageData(requestContext, sharedData);
      if (globalMessageData.Banners == null)
      {
        globalMessageData.Banners = (IList<GlobalMessageBanner>) new List<GlobalMessageBanner>()
        {
          banner
        };
      }
      else
      {
        for (int index = 0; index < globalMessageData.Banners.Count; ++index)
        {
          if (globalMessageData.Banners[index].Level < banner.Level)
          {
            globalMessageData.Banners.Insert(index, banner);
            return;
          }
        }
        globalMessageData.Banners.Add(banner);
      }
      if (banner.Links == null)
        return;
      ReferenceLinks target = new ReferenceLinks();
      banner.Links.CopyTo(target, (ISecuredObject) globalMessageData);
      banner.Links = target;
    }

    public void AddMessageAction(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalMessageAction action)
    {
      GlobalMessagesData globalMessageData = this.GetGlobalMessageData(requestContext, sharedData);
      if (globalMessageData.Actions == null)
        globalMessageData.Actions = (IList<GlobalMessageAction>) new List<GlobalMessageAction>();
      globalMessageData.Actions.Add(action);
    }

    private GlobalMessagesData GetGlobalMessageData(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData)
    {
      object obj;
      GlobalMessagesData globalMessageData;
      if (sharedData.TryGetValue("globalMessageData", out obj) && obj is GlobalMessagesData)
      {
        globalMessageData = (GlobalMessagesData) obj;
      }
      else
      {
        globalMessageData = new GlobalMessagesData();
        sharedData["globalMessageData"] = (object) globalMessageData;
      }
      return globalMessageData;
    }

    private GlobalDialogData GetGlobalDialogData(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData)
    {
      object obj;
      GlobalDialogData globalDialogData;
      if (sharedData.TryGetValue("globalDialogData", out obj) && obj is GlobalDialogData)
      {
        globalDialogData = (GlobalDialogData) obj;
      }
      else
      {
        globalDialogData = new GlobalDialogData();
        sharedData["globalDialogData"] = (object) globalDialogData;
      }
      return globalDialogData;
    }

    public object CreateGlobalMessageFromContribution(
      IVssRequestContext requestContext,
      ContributionNode contributionNode)
    {
      Contribution contribution = contributionNode.Contribution;
      GlobalMessageBanner globalMessageBanner = new GlobalMessageBanner();
      globalMessageBanner.SettingId = contribution.Id;
      globalMessageBanner.Message = contribution.GetProperty<string>("message", string.Empty);
      globalMessageBanner.Level = GlobalMessageService.GetMessageLevel(contribution.GetProperty<string>("level"));
      globalMessageBanner.Dismissable = new bool?(contribution.GetProperty<bool>("dismissable", true));
      globalMessageBanner.CustomIcon = contribution.GetProperty<string>("icon");
      globalMessageBanner.ContentContributionId = contribution.GetProperty<string>("contentContributionId");
      globalMessageBanner.ComponentType = contribution.GetProperty<string>("componentType");
      globalMessageBanner.ContentDependencies = contribution.GetProperty<IList<string>>("contentDependencies");
      globalMessageBanner.ContentProperties = contribution.GetProperty<IDictionary<string, object>>("contentProperties");
      globalMessageBanner.Links = ContributionMethods.GetReferenceLinks(contribution, "links");
      globalMessageBanner.Position = contribution.GetProperty<GlobalMessagePosition>("position");
      GlobalMessageBanner message = globalMessageBanner;
      this.ProcessMessageLinks(requestContext, (GlobalMessage) message);
      return (object) message;
    }

    public object CreateGlobalDialogFromContribution(
      IVssRequestContext requestContext,
      ContributionNode contributionNode)
    {
      Contribution contribution = contributionNode.Contribution;
      GlobalDialog globalDialog = new GlobalDialog();
      globalDialog.ButtonProps = contribution.GetProperty<IList<IDictionary<string, object>>>("buttonProps");
      globalDialog.CloseButtonProps = contribution.GetProperty<IDictionary<string, object>>("closeButtonProps");
      globalDialog.ComponentType = contribution.GetProperty<string>("componentType");
      globalDialog.ContentDependencies = contribution.GetProperty<IList<string>>("contentDependencies");
      globalDialog.ContentProperties = contribution.GetProperty<IDictionary<string, object>>("contentProperties");
      globalDialog.Message = contribution.GetProperty<string>("message", string.Empty);
      globalDialog.Priority = contribution.GetProperty<int>("priority");
      globalDialog.SettingId = contribution.Id;
      globalDialog.Title = contribution.GetProperty<string>("title", string.Empty);
      GlobalDialog message = globalDialog;
      this.ProcessMessageLinks(requestContext, (GlobalMessage) message);
      return (object) message;
    }

    private static WebMessageLevel GetMessageLevel(string levelValue)
    {
      WebMessageLevel result = WebMessageLevel.Info;
      if (!string.IsNullOrEmpty(levelValue) && !Enum.TryParse<WebMessageLevel>(levelValue, true, out result))
        result = WebMessageLevel.Info;
      return result;
    }

    public void ProcessMessageLinks(IVssRequestContext requestContext, GlobalMessage message)
    {
      if (string.IsNullOrEmpty(message.Message))
        return;
      try
      {
        StringReader input = new StringReader(message.Message);
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          ConformanceLevel = ConformanceLevel.Fragment
        }))
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element && string.Equals(xmlReader.Name, "a", StringComparison.OrdinalIgnoreCase))
            {
              string attribute = xmlReader.GetAttribute("href");
              if (!string.IsNullOrEmpty(attribute) && xmlReader.Read())
              {
                if (message.MessageLinks == null)
                  message.MessageLinks = (IList<GlobalMessageLink>) new List<GlobalMessageLink>();
                stringBuilder.Append("{" + message.MessageLinks.Count.ToString() + "}");
                message.MessageLinks.Add(new GlobalMessageLink()
                {
                  Name = xmlReader.Value,
                  Href = attribute
                });
              }
            }
            else
              stringBuilder.Append(xmlReader.Value);
          }
          if (message.MessageLinks == null)
            return;
          message.MessageFormat = stringBuilder.ToString();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "WebPlatform", "GlobalMessageBanner", ex);
      }
    }
  }
}
