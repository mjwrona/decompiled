// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsLocator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TfsLocator
  {
    private bool m_hostIsCollection;
    private Guid m_hostGuid;
    private const string c_defaultMacro = "@default";

    public TfsLocator()
    {
      this.m_hostGuid = Guid.Empty;
      this.m_hostIsCollection = false;
      this.ProjectGuid = Guid.Empty;
      this.TeamGuid = Guid.Empty;
    }

    public Uri ServerUrl { get; set; }

    public string CollectionName { get; set; }

    public Guid CollectionGuid
    {
      get
      {
        this.ThrowIfHostIsNotCollection();
        return this.m_hostGuid;
      }
    }

    public Guid HostGuid => this.m_hostGuid;

    public string ProjectName { get; set; }

    public Guid ProjectGuid { get; set; }

    public string TeamName { get; set; }

    public Guid TeamGuid { get; set; }

    public Uri ProjectUri { get; private set; }

    public bool CanLocateServer => this.ServerUrl != (Uri) null;

    public bool CanLocateCollection
    {
      get
      {
        if (!this.CanLocateServer)
          return false;
        return this.CanLocateProject || !string.IsNullOrEmpty(this.CollectionName) || this.CollectionGuid != Guid.Empty;
      }
    }

    public bool CanLocateProject => this.ProjectGuid != Guid.Empty || !string.IsNullOrEmpty(this.ProjectName);

    public bool CanLocateTeam => this.TeamGuid != Guid.Empty || !string.IsNullOrEmpty(this.TeamName);

    public static TfsLocator CreateFromPairs(Uri serverUrl, NameValueCollection requestParameters)
    {
      TfsLocator fromPairs = new TfsLocator();
      fromPairs.ServerUrl = serverUrl;
      fromPairs.Assign(requestParameters);
      return fromPairs;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}|{1}|{2}|{3}|{4}|{5}", (object) this.ServerUrl, (object) this.CollectionGuid, (object) this.CollectionName, (object) this.ProjectGuid, (object) this.ProjectName, (object) this.TeamName);

    public static NameValueCollection FilterQueryString(string originalQueryString)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(originalQueryString);
      queryString.Remove("tfs");
      queryString.Remove("collectionId");
      queryString.Remove("pcguid");
      queryString.Remove("pcname");
      queryString.Remove("projectId");
      queryString.Remove("projectUri");
      queryString.Remove("pguid");
      queryString.Remove("puri");
      queryString.Remove("pname");
      queryString.Remove("teamId");
      queryString.Remove("team");
      return queryString;
    }

    private void Assign(NameValueCollection nameValuePairs)
    {
      try
      {
        string nameValuePair1 = nameValuePairs["tfs"];
        if (!string.IsNullOrEmpty(nameValuePair1) && !StringComparer.OrdinalIgnoreCase.Equals(nameValuePair1, "@default"))
          this.ServerUrl = CommonUtility.CreateUriFromRequest(nameValuePair1, "tfs");
        string nameValuePair2 = nameValuePairs["collectionId"];
        if (!string.IsNullOrEmpty(nameValuePair2))
        {
          this.m_hostGuid = CommonUtility.CreateGuidFromRequest(nameValuePair2, "collectionId");
          this.m_hostIsCollection = true;
        }
        else
        {
          string nameValuePair3 = nameValuePairs["pcguid"];
          if (!string.IsNullOrEmpty(nameValuePair3))
          {
            this.m_hostGuid = CommonUtility.CreateGuidFromRequest(nameValuePair3, "pcguid");
            this.m_hostIsCollection = true;
          }
        }
        this.CollectionName = nameValuePairs["pcname"];
        string nameValuePair4 = nameValuePairs["projectId"];
        if (!string.IsNullOrEmpty(nameValuePair4))
        {
          this.ProjectGuid = CommonUtility.CreateGuidFromRequest(nameValuePair4, "projectId");
        }
        else
        {
          string nameValuePair5 = nameValuePairs["pguid"];
          if (!string.IsNullOrEmpty(nameValuePair5))
          {
            this.ProjectGuid = CommonUtility.CreateGuidFromRequest(nameValuePair5, "pguid");
          }
          else
          {
            string nameValuePair6 = nameValuePairs["puri"];
            if (!string.IsNullOrEmpty(nameValuePair6))
            {
              this.ProjectUri = CommonUtility.CreateUriFromRequest(nameValuePair6, "puri");
            }
            else
            {
              string nameValuePair7 = nameValuePairs["projectUri"];
              if (!string.IsNullOrEmpty(nameValuePair7))
                this.ProjectUri = CommonUtility.CreateUriFromRequest(nameValuePair7, "projectUri");
            }
          }
        }
        if (this.ProjectUri == (Uri) null && this.ProjectGuid != Guid.Empty)
          this.ProjectUri = TfsLocator.CreateProjectUriFrom(this.ProjectGuid);
        if (this.ProjectGuid == Guid.Empty && this.ProjectUri != (Uri) null)
          this.ProjectGuid = TfsLocator.CreateProjectGuidFrom(this.ProjectUri);
        this.ProjectName = nameValuePairs["pname"];
        this.TeamName = nameValuePairs["team"];
        string nameValuePair8 = nameValuePairs["teamId"];
        if (string.IsNullOrEmpty(nameValuePair8))
          return;
        this.TeamGuid = CommonUtility.CreateGuidFromRequest(nameValuePair8, "teamId");
      }
      catch (FormatException ex)
      {
        throw new TeamFoundationServiceException(ex.Message, (Exception) ex);
      }
    }

    public void InitializeHostId(TfsWebContext tfsWebContext)
    {
      if (!(this.m_hostGuid == Guid.Empty))
        return;
      this.ParseCollectionGuidFromRequest(tfsWebContext);
      if (!(this.m_hostGuid == Guid.Empty))
        return;
      this.SetHostGuidToDefault(tfsWebContext);
    }

    public void ThrowIfHostIsNotCollection()
    {
      if (!this.m_hostIsCollection)
        throw new TeamFoundationServiceException(WACommonResources.NoCollectionSpecifiedInCompatUrl);
    }

    private Guid ParseCollectionGuidFromRequest(TfsWebContext tfsWebContext)
    {
      if (this.ProjectUri != (Uri) null)
      {
        this.m_hostGuid = TfsHelpers.GetCollectionGuid(tfsWebContext.TfsRequestContext, this.ProjectUri);
        if (this.m_hostGuid == Guid.Empty)
          throw new HostInstanceDoesNotExistException(string.Format(WACommonResources.ProjectDoesNotExistOrNoAccess, (object) this.ProjectGuid));
      }
      else if (!string.IsNullOrEmpty(this.ProjectName))
      {
        this.m_hostGuid = TfsHelpers.GetCollectionGuid(tfsWebContext.TfsRequestContext, this.ProjectName);
        if (this.m_hostGuid == Guid.Empty)
          throw new HostInstanceDoesNotExistException(string.Format(WACommonResources.ProjectNotFound, (object) this.ProjectName));
      }
      else if (!string.IsNullOrEmpty(this.CollectionName))
        this.m_hostGuid = (tfsWebContext.GetCollectionProperties().Values.Where<TfsServiceHostDescriptor>((Func<TfsServiceHostDescriptor, bool>) (tpcp => tpcp.Name.Equals(this.CollectionName, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault<TfsServiceHostDescriptor>() ?? throw new HostInstanceDoesNotExistException(string.Format(WACommonResources.UnableToAccessCollectionFromCompatUrl, (object) this.CollectionName))).Id;
      if (this.m_hostGuid != Guid.Empty)
        this.m_hostIsCollection = true;
      return this.m_hostGuid;
    }

    private void SetHostGuidToDefault(TfsWebContext tfsWebContext)
    {
      if (tfsWebContext.TfsRequestContext.ServiceHost == null)
        return;
      this.m_hostGuid = tfsWebContext.TfsRequestContext.ServiceHost.InstanceId;
      this.m_hostIsCollection = tfsWebContext.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
    }

    public static string GetCollectionUrl(IVssRequestContext collectionRequestContext) => collectionRequestContext.GetService<ILocationService>().GetLocationServiceUrl(collectionRequestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);

    public ProjectInfo GetProjectInfo(IVssRequestContext collectionRequestContext)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      if (this.ProjectUri != (Uri) null)
        projectInfo = TfsProjectHelpers.GetProject(collectionRequestContext, this.ProjectUri);
      else if (!string.IsNullOrEmpty(this.ProjectName))
      {
        projectInfo = TfsProjectHelpers.GetProjectFromName(collectionRequestContext, this.ProjectName);
        if (projectInfo != null)
        {
          this.ProjectUri = new Uri(projectInfo.Uri);
          this.ProjectGuid = TfsLocator.CreateProjectGuidFrom(this.ProjectUri);
        }
      }
      return projectInfo;
    }

    public WebApiTeam GetTeam(IVssRequestContext tfsRequestContext, Guid projectGuid)
    {
      WebApiTeam team = (WebApiTeam) null;
      if (this.CanLocateTeam)
      {
        ITeamService service = tfsRequestContext.GetService<ITeamService>();
        if (this.TeamGuid != Guid.Empty)
        {
          team = service.GetTeamInProject(tfsRequestContext, projectGuid, this.TeamGuid.ToString());
          if (team == null)
            throw new TeamFoundationServiceException(string.Format(WACommonResources.TeamDoesNotExistOrNoAccess, (object) this.TeamGuid));
        }
        else if (!string.IsNullOrEmpty(this.TeamName) && this.ProjectUri != (Uri) null)
        {
          team = service.GetTeamInProject(tfsRequestContext, projectGuid, this.TeamName);
          if (team == null)
            throw new TeamFoundationServiceException(string.Format(WACommonResources.TeamDoesNotExistOrNoAccess, (object) this.TeamName));
        }
      }
      return team;
    }

    public static Uri CreateProjectUriFrom(Guid projectId) => new Uri(LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", projectId.ToString())));

    public static Guid CreateProjectGuidFrom(Uri projectUri)
    {
      if (projectUri == (Uri) null)
        throw new ArgumentNullException(nameof (projectUri));
      return new Guid(LinkingUtilities.DecodeUri(projectUri.AbsoluteUri).ToolSpecificId);
    }
  }
}
