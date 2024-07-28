// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DataRetention.ShelvesetDataRetentionPage
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
using System.Linq;
using System.Text;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.TeamFoundation.VersionControl.Server.DataRetention
{
  public class ShelvesetDataRetentionPage : VersionControlPage
  {
    private Shelveset m_shelveset;
    private string m_shelvesetOwner;
    private DateTime m_cutoffDate;
    private ShelvesetDataRetentionPage.SelectedAction m_selectedAction;
    private Table m_tableServerInfo;
    private Table m_tableShelveset;
    private Table m_tablePendingChanges;
    private Button m_actionButton;
    private const string c_actionDelete = "delete";
    private const string c_actionExtendAll = "extendall";
    private const string c_queryStringAction = "action";
    private const string c_queryStringName = "name";
    private const string c_queryStringOwner = "owner";
    private const string c_queryStringDate = "date";

    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.RequestContext.GetService<CachedRegistryService>().GetValue<int>(this.RequestContext.Elevate(), (RegistryQuery) Constants.ShelvesetDaysSetting, Constants.InvalidDaysSetting) == Constants.InvalidDaysSetting)
      {
        this.Response.StatusCode = 404;
        this.Response.End();
      }
      else
      {
        this.m_tableServerInfo = (Table) this.FindControl("TableServerInfo");
        this.m_tableShelveset = (Table) this.FindControl("TableShelveset");
        this.m_tablePendingChanges = (Table) this.FindControl("TablePendingChanges");
        this.m_actionButton = (Button) this.FindControl("Action");
        if (StringComparer.OrdinalIgnoreCase.Equals(this.Request.QueryString["action"], "delete"))
        {
          this.m_actionButton.Text = DataRetentionPagesResources.Get("DeleteShelveset");
          this.m_selectedAction = ShelvesetDataRetentionPage.SelectedAction.Delete;
        }
        else if (StringComparer.OrdinalIgnoreCase.Equals(this.Request.QueryString["action"], "extendall"))
        {
          this.m_actionButton.Text = DataRetentionPagesResources.Get("ExtendAllShelvesets");
          this.m_selectedAction = ShelvesetDataRetentionPage.SelectedAction.ExtendAll;
        }
        try
        {
          IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Deployment);
          ILocationService service = vssRequestContext.GetService<ILocationService>();
          this.AddRow(this.m_tableServerInfo, DataRetentionPagesResources.Get("TableServerUri"), service.GetLocationServiceUrl(vssRequestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker));
          this.AddRow(this.m_tableServerInfo, DataRetentionPagesResources.Get("TableProjectCollection"), this.RequestContext.ServiceHost.Name);
          string shelvesetOwner = this.Request.QueryString["owner"];
          if (this.m_selectedAction != ShelvesetDataRetentionPage.SelectedAction.ExtendAll)
          {
            string shelvesetName = this.Request.QueryString["name"];
            this.m_shelveset = !string.IsNullOrEmpty(shelvesetName) && !string.IsNullOrEmpty(shelvesetOwner) ? this.GetShelveset(shelvesetName, shelvesetOwner) : throw new Exception(DataRetentionPagesResources.Get("MustProvideShelvesetNameAndOwner"));
            this.m_shelvesetOwner = this.m_shelveset.Owner;
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableName"), this.m_shelveset.Name);
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableOwner"), this.m_shelveset.Owner);
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableCreationDate"), this.m_shelveset.CreationDate.ToString());
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableComment"), this.m_shelveset.Comment);
            this.AddRow(this.m_tablePendingChanges, DataRetentionPagesResources.Get("TablePCName"), DataRetentionPagesResources.Get("TablePCChange"), DataRetentionPagesResources.Get("TablePCFolder"));
            this.WritePendingChanges(this.m_shelveset);
          }
          else
          {
            this.m_shelvesetOwner = shelvesetOwner;
            string s = this.Request.QueryString["date"];
            long result;
            if (string.IsNullOrEmpty(shelvesetOwner) || string.IsNullOrEmpty(s) || !long.TryParse(s, out result))
              throw new Exception(DataRetentionPagesResources.Get("MustProviderShelvesetOwnerAndCutoffDate"));
            this.m_cutoffDate = DateTime.FromBinary(result);
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableOwner"), shelvesetOwner);
            this.AddRow(this.m_tableShelveset, DataRetentionPagesResources.Get("TableCutoffDate"), this.m_cutoffDate.ToString());
            this.AddRow(this.m_tablePendingChanges, DataRetentionPagesResources.Get("MultipleShelvesets"));
          }
        }
        catch (Exception ex)
        {
          this.ReportErrorInTable(this.m_tablePendingChanges, ex.Message);
        }
      }
    }

    private Shelveset GetShelveset(string shelvesetName, string shelvesetOwner)
    {
      foreach (Shelveset queryShelveset in this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryShelvesets(this.RequestContext, shelvesetName, shelvesetOwner))
      {
        if (TFStringComparer.ShelvesetName.Equals(shelvesetName, queryShelveset.Name))
          return queryShelveset;
      }
      throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);
    }

    private void WritePendingChanges(Shelveset shelveset)
    {
      using (TeamFoundationDataReader foundationDataReader = this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryShelvedChanges(this.RequestContext, (string) null, (string) null, shelveset.Name, shelveset.Owner, (ItemSpec[]) null, false))
      {
        PendingSet pendingSet = foundationDataReader.Current<StreamingCollection<PendingSet>>().FirstOrDefault<PendingSet>();
        int num = 25;
        foreach (PendingChange pendingChange in pendingSet.PendingChanges)
        {
          if (num-- != 0)
          {
            string parent;
            string name;
            VersionControlPath.Parse(pendingChange.ServerItem, out parent, out name);
            this.AddRow(this.m_tablePendingChanges, name, ShelvesetDataRetentionPage.BuildLocalizedStringForChangeType((ChangeType) pendingChange.ChangeEx), parent);
          }
          else
            break;
        }
        if (num >= 0 || !pendingSet.PendingChanges.MoveNext())
          return;
        this.AddRow(this.m_tablePendingChanges, "...");
      }
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

    private static string BuildLocalizedStringForChangeType(ChangeType type)
    {
      if (type == ChangeType.None)
        return DataRetentionPagesResources.Get("ChangeTypeNone");
      StringBuilder sb = new StringBuilder();
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Merge) != 0, sb, "ChangeTypeMerge");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Add) != 0, sb, "ChangeTypeAdd");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Branch) != 0, sb, "ChangeTypeBranch");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Delete) != 0, sb, "ChangeTypeDelete");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Encoding) != ChangeType.None && (type & (ChangeType.Add | ChangeType.Branch)) == ChangeType.None, sb, "ChangeTypeFileType");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Lock) != 0, sb, "ChangeTypeLock");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Rename) != 0, sb, "ChangeTypeRename");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Undelete) != 0, sb, "ChangeTypeUndelete");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Edit) != ChangeType.None && (type & ChangeType.Add) == ChangeType.None, sb, "ChangeTypeEdit");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.Rollback) != 0, sb, "ChangeTypeRollback");
      ShelvesetDataRetentionPage.AppendIf((type & ChangeType.SourceRename) != 0, sb, "ChangeTypeSourceRename");
      return sb.ToString();
    }

    private static void AppendIf(bool condition, StringBuilder sb, string resourceName)
    {
      if (!condition)
        return;
      if (sb.Length != 0)
        sb.Append(", ");
      sb.Append(DataRetentionPagesResources.Get(resourceName));
    }

    protected void Action_Click(object sender, EventArgs e)
    {
      try
      {
        TeamFoundationVersionControlService service = this.RequestContext.GetService<TeamFoundationVersionControlService>();
        if (this.m_selectedAction == ShelvesetDataRetentionPage.SelectedAction.Delete)
        {
          service.DeleteShelveset(this.RequestContext, this.m_shelveset.Name, this.m_shelveset.Owner);
          this.m_actionButton.Text = DataRetentionPagesResources.Get("DeletionSuccessful");
        }
        else if (this.m_selectedAction == ShelvesetDataRetentionPage.SelectedAction.Extend)
        {
          service.UpdateShelvesetCreationTime(this.RequestContext, this.m_shelveset.Name, this.m_shelvesetOwner, new DateTime?());
          this.m_actionButton.Text = DataRetentionPagesResources.Get("RenewalSuccessful");
        }
        else if (this.m_selectedAction == ShelvesetDataRetentionPage.SelectedAction.ExtendAll)
        {
          service.UpdateShelvesetCreationTime(this.RequestContext, (string) null, this.m_shelvesetOwner, new DateTime?(this.m_cutoffDate));
          this.m_actionButton.Text = DataRetentionPagesResources.Get("RenewalSuccessful");
        }
      }
      catch (Exception ex)
      {
        this.ReportErrorInTable(this.m_tablePendingChanges, ex.Message);
      }
      this.m_actionButton.Enabled = false;
    }

    private enum SelectedAction
    {
      Extend,
      ExtendAll,
      Delete,
    }
  }
}
