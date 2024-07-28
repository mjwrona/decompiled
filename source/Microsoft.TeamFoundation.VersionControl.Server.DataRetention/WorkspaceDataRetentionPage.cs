// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DataRetention.WorkspaceDataRetentionPage
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.DataRetention, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACAD6A6E-265A-4AD6-8B83-B0DD75035C8B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.DataRetention.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.TeamFoundation.VersionControl.Server.DataRetention
{
  public class WorkspaceDataRetentionPage : VersionControlPage
  {
    private Workspace m_workspace;
    private WorkspaceDataRetentionPage.SelectedAction m_selectedAction;
    private Table m_tableServerInfo;
    private Table m_tableWorkspace;
    private Table m_tableWorkingFolders;
    private Button m_actionButton;
    private static readonly TimeSpan s_oneDay = new TimeSpan(1, 0, 0, 0);
    private static readonly string s_versionControlSeparator = '/'.ToString();
    private const string c_actionDelete = "delete";
    private const string c_queryStringAction = "action";
    private const string c_queryStringName = "name";
    private const string c_queryStringOwner = "owner";

    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.RequestContext.GetService<CachedRegistryService>().GetValue<int>(this.RequestContext.Elevate(), (RegistryQuery) Constants.WorkspaceDaysSetting, Constants.InvalidDaysSetting) == Constants.InvalidDaysSetting)
      {
        this.Response.StatusCode = 404;
        this.Response.End();
      }
      else
      {
        this.m_tableServerInfo = (Table) this.FindControl("TableServerInfo");
        this.m_tableWorkspace = (Table) this.FindControl("TableWorkspace");
        this.m_tableWorkingFolders = (Table) this.FindControl("TableWorkingFolders");
        this.m_actionButton = (Button) this.FindControl("Action");
        if (StringComparer.OrdinalIgnoreCase.Equals(this.Request.QueryString["action"], "delete"))
        {
          this.m_actionButton.Text = DataRetentionPagesResources.Get("DeleteWorkspace");
          this.m_selectedAction = WorkspaceDataRetentionPage.SelectedAction.Delete;
        }
        try
        {
          IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Deployment);
          ILocationService service = vssRequestContext.GetService<ILocationService>();
          string workspaceName = this.Request.QueryString["name"];
          string workspaceOwner = this.Request.QueryString["owner"];
          this.m_workspace = !string.IsNullOrEmpty(workspaceName) && !string.IsNullOrEmpty(workspaceOwner) ? this.GetWorkspace(workspaceName, workspaceOwner) : throw new Exception(DataRetentionPagesResources.Get("MustProvideWorkspaceNameAndOwner"));
          this.AddRow(this.m_tableServerInfo, DataRetentionPagesResources.Get("TableServerUri"), service.GetLocationServiceUrl(vssRequestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker));
          this.AddRow(this.m_tableServerInfo, DataRetentionPagesResources.Get("TableProjectCollection"), this.RequestContext.ServiceHost.Name);
          this.AddRow(this.m_tableWorkspace, DataRetentionPagesResources.Get("TableName"), this.m_workspace.Name);
          this.AddRow(this.m_tableWorkspace, DataRetentionPagesResources.Get("TableOwner"), this.m_workspace.OwnerName);
          this.AddRow(this.m_tableWorkspace, DataRetentionPagesResources.Get("TableComputer"), this.m_workspace.Computer);
          this.AddRow(this.m_tableWorkspace, DataRetentionPagesResources.Get("TableComment"), this.m_workspace.Comment);
          this.AddRow(this.m_tableWorkspace, DataRetentionPagesResources.Get("TableLastAccessDate"), this.m_workspace.LastAccessDate.ToString());
          this.AddRow(this.m_tableWorkingFolders, DataRetentionPagesResources.Get("TableWFStatus"), DataRetentionPagesResources.Get("TableWFServerPath"), DataRetentionPagesResources.Get("TableWFLocalPath"));
          this.WriteWorkingFolders(this.m_workspace);
          if (this.m_selectedAction != WorkspaceDataRetentionPage.SelectedAction.Extend || !(DateTime.UtcNow.Subtract(this.m_workspace.LastAccessDate) < WorkspaceDataRetentionPage.s_oneDay))
            return;
          this.m_actionButton.Text = DataRetentionPagesResources.Get("AlreadyUpToDate");
          this.m_actionButton.Enabled = false;
        }
        catch (Exception ex)
        {
          this.ReportErrorInTable(this.m_tableWorkingFolders, ex.Message);
        }
      }
    }

    private void WriteWorkingFolders(Workspace workspace)
    {
      foreach (WorkingFolder folder in workspace.Folders)
      {
        string str1 = DataRetentionPagesResources.Get("TableWFActive");
        if (folder.Type == WorkingFolderType.Cloak)
          str1 = DataRetentionPagesResources.Get("TableWFCloaked");
        string str2 = folder.ServerItem;
        if (folder.Depth == 1)
          str2 = !str2.EndsWith(WorkspaceDataRetentionPage.s_versionControlSeparator) ? str2 + (object) '/' + (object) '*' : str2 + (object) '*';
        string str3 = string.Empty;
        if (folder.LocalItem != null)
          str3 = folder.LocalItem;
        this.AddRow(this.m_tableWorkingFolders, str1, str2, str3);
      }
    }

    private Workspace GetWorkspace(string workspaceName, string workspaceOwner)
    {
      foreach (Workspace queryWorkspace in (IEnumerable<Workspace>) this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryWorkspaces(this.RequestContext, workspaceOwner, (string) null, 0))
      {
        if (TFStringComparer.WorkspaceName.Equals(workspaceName, queryWorkspace.Name))
          return queryWorkspace;
      }
      throw new WorkspaceNotFoundException(workspaceName, workspaceOwner);
    }

    private TableRow AddRow(Table t, params string[] cells)
    {
      TableRow row = new TableRow();
      foreach (string cell1 in cells)
      {
        TableCell cell2 = new TableCell();
        cell2.Text = AntiXssEncoder.HtmlEncode(cell1, true);
        cell2.Style[HtmlTextWriterStyle.PaddingRight] = "2em";
        row.Cells.Add(cell2);
      }
      t.Rows.Add(row);
      return row;
    }

    private void ReportErrorInTable(Table t, string message)
    {
      this.m_actionButton.Enabled = false;
      TableRow row = new TableRow();
      TableCell cell = new TableCell();
      cell.Text = UriUtility.HtmlEncode(DataRetentionPagesResources.Format("ErrorInTable", (object) message));
      cell.Style[HtmlTextWriterStyle.FontWeight] = "bold";
      cell.ColumnSpan = 3;
      row.Cells.Add(cell);
      t.Rows.Add(row);
    }

    protected void Action_Click(object sender, EventArgs e)
    {
      try
      {
        IVssSecurityNamespace securityNamespace = this.RequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, SecurityConstants.WorkspaceSecurityNamespaceGuid);
        TeamFoundationVersionControlService service = this.RequestContext.GetService<TeamFoundationVersionControlService>();
        if (this.m_selectedAction == WorkspaceDataRetentionPage.SelectedAction.Delete)
        {
          securityNamespace.CheckPermission(this.RequestContext, this.m_workspace.SecurityToken, 8);
          service.DeleteWorkspace(this.RequestContext, this.m_workspace.Name, this.m_workspace.OwnerName);
          this.m_actionButton.Text = DataRetentionPagesResources.Get("DeletionSuccessful");
        }
        else
        {
          securityNamespace.CheckPermission(this.RequestContext, this.m_workspace.SecurityToken, 2);
          using (service.QueryLocalVersions(this.RequestContext, this.m_workspace.Name, this.m_workspace.OwnerName, new ItemSpec[1]
          {
            new ItemSpec("C:\\", RecursionType.None)
          }))
            ;
          this.m_actionButton.Text = DataRetentionPagesResources.Get("RenewalSuccessful");
        }
      }
      catch (Exception ex)
      {
        this.ReportErrorInTable(this.m_tableWorkingFolders, ex.Message);
      }
      this.m_actionButton.Enabled = false;
    }

    private static class WorkspacePermissions
    {
      public const int Read = 1;
      public const int Use = 2;
      public const int CheckIn = 4;
      public const int Administer = 8;
    }

    private enum SelectedAction
    {
      Extend,
      Delete,
    }
  }
}
