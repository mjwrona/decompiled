// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamProjectPicker
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Server;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public class TeamProjectPicker : IDisposable
  {
    private TfsConnectDataSource m_dataSource;
    private TfsConnectDialog m_internalDialog;
    private bool m_disposed;

    public TeamProjectPicker()
      : this(TeamProjectPickerMode.SingleProject, false)
    {
    }

    public TeamProjectPicker(TeamProjectPickerMode mode, bool disableCollectionChange)
      : this(mode, disableCollectionChange, (ICredentialsProvider) null)
    {
    }

    public TeamProjectPicker(
      TeamProjectPickerMode mode,
      bool disableCollectionChange,
      ICredentialsProvider credentialsProvider)
    {
      this.Initialize(mode, disableCollectionChange, credentialsProvider);
    }

    ~TeamProjectPicker() => this.Dispose(false);

    public void SetDefaultSelectionProvider(
      ITeamProjectPickerDefaultSelectionProvider provider)
    {
      this.m_dataSource.SetDefaultSelectionProvider(provider);
    }

    public DialogResult ShowDialog()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("ConnectToTFSDialog");
      return this.m_internalDialog.ShowDialog();
    }

    public DialogResult ShowDialog(IWin32Window parentWindow)
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("ConnectToTFSDialog");
      return this.m_internalDialog.ShowDialog(parentWindow);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.m_internalDialog != null)
        {
          this.m_internalDialog.Dispose();
          this.m_internalDialog = (TfsConnectDialog) null;
        }
        if (this.m_dataSource != null)
        {
          this.m_dataSource.Dispose();
          this.m_dataSource = (TfsConnectDataSource) null;
        }
      }
      this.m_disposed = true;
    }

    public event CancelEventHandler AcceptButtonClick;

    public string Text
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        return this.m_internalDialog.Text;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        this.m_internalDialog.Text = value;
      }
    }

    public string HelpTopic
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        return this.m_internalDialog.HelpTopic;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        this.m_internalDialog.HelpTopic = value;
      }
    }

    public string AcceptButtonText
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        return this.m_internalDialog.AcceptButtonText;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        this.m_internalDialog.AcceptButtonText = value;
      }
    }

    public bool AutoCommitConnectionChanges
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        return this.m_internalDialog.AutoCommitConnectionChanges;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        this.m_internalDialog.AutoCommitConnectionChanges = value;
      }
    }

    public TfsTeamProjectCollection HostActiveTeamProjectCollection
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        return this.m_dataSource.ActiveServer;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        this.m_dataSource.ActiveServer = value;
      }
    }

    public ProjectInfo[] SelectedProjects
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        int val2 = this.m_dataSource.SelectedProjects == null || !this.m_dataSource.CanSelectProject ? 0 : this.m_dataSource.SelectedProjects.Count;
        if (!this.m_dataSource.CanSelectMultipleProjects)
          val2 = Math.Max(1, val2);
        ProjectInfo[] selectedProjects = new ProjectInfo[val2];
        for (int index = 0; index < val2; ++index)
          selectedProjects[index] = this.m_dataSource.SelectedProjects[index].Info;
        return selectedProjects;
      }
      set
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("ConnectToTFSDialog");
        if (!this.m_dataSource.CanSelectProject)
          throw new InvalidOperationException();
        this.m_dataSource.SetDefaultSelectedProjects(value);
      }
    }

    public TfsTeamProjectCollection SelectedTeamProjectCollection
    {
      get => this.m_dataSource.SelectedCollection != null ? this.m_dataSource.SelectedCollection.Server : (TfsTeamProjectCollection) null;
      set => this.m_dataSource.SetSelectedCollection(value);
    }

    public bool HasNewCredentials => this.m_dataSource.SelectedServer != null && this.m_dataSource.SelectedServer.HasNewCredentials;

    private void Initialize(
      TeamProjectPickerMode mode,
      bool disableCollectionChange,
      ICredentialsProvider credentialsProvider)
    {
      this.m_dataSource = new TfsConnectDataSource(mode, disableCollectionChange);
      this.m_dataSource.SetDefaultSelectionProvider(TeamExplorerProjects.Instance.GetDefaultSelectionProvider(mode));
      this.m_internalDialog = new TfsConnectDialog(this.m_dataSource);
      this.m_internalDialog.AcceptButtonClick += new CancelEventHandler(this.m_internalDialog_AcceptButtonClick);
    }

    private void m_internalDialog_AcceptButtonClick(object sender, CancelEventArgs e)
    {
      if (this.AcceptButtonClick == null)
        return;
      this.AcceptButtonClick((object) this, e);
    }

    public static void CommitConnectionChange(TfsTeamProjectCollection collection) => TeamProjectPicker.CommitConnectionChange(collection, false);

    public static void CommitConnectionChange(
      TfsTeamProjectCollection collection,
      bool storeCredentials)
    {
      TfsConnectDialog.CommitConnectionChange(collection, storeCredentials);
    }
  }
}
