// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  [DataContract]
  public class WelcomeViewModel
  {
    public const string c_lastDocumentSettingPath = "/Welcome/LastDocument/";

    [DataMember(Name = "projectVersionControlInfo")]
    public VersionControlProjectInfo ProjectVersionControlInfo { get; set; }

    [DataMember(Name = "defaultDocument")]
    public DocumentFile DefaultDocument { get; set; }

    public int MaxDashboardPerGroup { get; set; }

    public WelcomeViewModel(TfsWebContext tfsWebContext)
    {
      this.ProjectVersionControlInfo = this.GetProjectVersionControlInfo(tfsWebContext);
      this.DefaultDocument = this.GetUserDefaultDocument(tfsWebContext);
    }

    private VersionControlProjectInfo GetProjectVersionControlInfo(TfsWebContext tfsWebContext) => TeamProjectVersionControlInfoUtil.GetProjectInfo(tfsWebContext.TfsRequestContext, tfsWebContext.Project.Id);

    private DocumentFile GetUserDefaultDocument(TfsWebContext tfsWebContext)
    {
      DocumentFile userDefaultDocument = (DocumentFile) null;
      string setting = this.GetUserWebSettings(tfsWebContext).GetSetting<string>("/Welcome/LastDocument/" + tfsWebContext.CurrentProjectGuid.ToString(), (string) null);
      if (!string.IsNullOrEmpty(setting))
      {
        try
        {
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(setting)))
            userDefaultDocument = new DataContractJsonSerializer(typeof (DocumentFile)).ReadObject((Stream) memoryStream) as DocumentFile;
        }
        catch (Exception ex)
        {
          tfsWebContext.TfsRequestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
        }
      }
      return userDefaultDocument;
    }

    private ISettingsProvider GetUserWebSettings(TfsWebContext tfsWebContext)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(tfsWebContext, nameof (tfsWebContext));
      return WebSettings.GetWebSettings(tfsWebContext.TfsRequestContext, tfsWebContext.CurrentProjectGuid, (WebApiTeam) null, WebSettingsScope.User);
    }
  }
}
