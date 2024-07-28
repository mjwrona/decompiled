// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DialogPerfListener
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  internal class DialogPerfListener : BaseDialog
  {
    private WebServiceStatList m_statList;
    private WebServiceCallList m_callList;
    private IContainer components;
    private DataGridView dataGridViewStats;
    private Button closeButton;
    private Button clearButton;
    private TableLayoutPanel overarchingtableLayoutPanel;
    private BindingSource WebServiceStatListBindingSource;
    private DataGridView dataGridViewHistory;
    private BindingSource WebServiceCallListBindingSource;
    private DataGridViewTextBoxColumn WebServiceCalled;
    private DataGridViewTextBoxColumn RunTime;
    private DataGridViewTextBoxColumn DateTimeStamp;
    private DataGridViewTextBoxColumn ThreadId;
    private DataGridViewTextBoxColumn WebService;
    private DataGridViewTextBoxColumn TotalTime;
    private DataGridViewTextBoxColumn Last;
    private DataGridViewTextBoxColumn Count;
    private DataGridViewTextBoxColumn Average;
    private DataGridViewTextBoxColumn MinTime;
    private DataGridViewTextBoxColumn MaxTime;
    private Button buttonSaveAll;

    internal DialogPerfListener()
    {
      this.InitializeComponent();
      this.m_statList = new WebServiceStatList();
      this.m_callList = new WebServiceCallList();
      this.WebServiceStatListBindingSource.DataSource = (object) this.m_statList;
      this.WebServiceCallListBindingSource.DataSource = (object) this.m_callList;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.WindowState = FormWindowState.Minimized;
      this.dataGridViewHistory.DoubleClick += new EventHandler(this.dataGridViewHistory_DoubleClick);
      this.dataGridViewHistory.Click += new EventHandler(this.dataGridViewHistory_Click);
    }

    internal void AddRunning(string webServiceName, string serviceAddress)
    {
      if (!this.Visible)
        return;
      Thread currentThread = Thread.CurrentThread;
      StackTrace stackTrace = new StackTrace(6, true);
      this.BeginInvoke((Delegate) (() =>
      {
        this.m_callList.AddRunning(webServiceName, currentThread.ManagedThreadId, currentThread.Priority);
        this.dataGridViewHistory.CurrentCell = this.dataGridViewHistory[0, this.dataGridViewHistory.RowCount - 1];
        this.dataGridViewHistory.CurrentCell.ToolTipText = serviceAddress + "\r\n\r\n" + stackTrace.ToString();
      }));
    }

    internal void AddTime(string webServiceName, int runTimeMilliseconds)
    {
      if (!this.Visible)
        return;
      StackTrace stackTrace = new StackTrace(6, true);
      this.BeginInvoke((Delegate) (() =>
      {
        this.m_statList.AddTime(webServiceName.Split(':')[0], runTimeMilliseconds);
        this.m_callList.AddTime(webServiceName, runTimeMilliseconds);
        this.WebServiceStatListBindingSource.ResetBindings(true);
        this.dataGridViewHistory.CurrentCell = this.dataGridViewHistory[0, this.dataGridViewHistory.RowCount - 1];
      }));
    }

    private void dataGridViewHistory_DoubleClick(object sender, EventArgs e)
    {
      Clipboard.Clear();
      DataGridViewCell currentCell = this.dataGridViewHistory.CurrentCell;
      if (currentCell == null)
        return;
      string text;
      if (currentCell.OwningRow.DataBoundItem is WebServiceCall dataBoundItem)
        text = currentCell.OwningRow.Index.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\t\t\t\t--- Row index" + Environment.NewLine + dataBoundItem.WebServiceCalled + "\t\t--- Web service called" + Environment.NewLine + dataBoundItem.RunTime.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "\t\t\t\t--- Run time" + Environment.NewLine + dataBoundItem.StartTimeString + "\t\t--- Date time stamp" + Environment.NewLine + Environment.NewLine + currentCell.OwningRow.Cells[0].ToolTipText;
      else
        text = "something wrong";
      Clipboard.SetText(text);
      currentCell.OwningRow.DefaultCellStyle.ForeColor = Color.Red;
      this.dataGridViewHistory.CurrentCell = (DataGridViewCell) null;
    }

    private void dataGridViewHistory_Click(object sender, EventArgs e)
    {
      if (this.dataGridViewHistory.CurrentRow == null)
        return;
      this.dataGridViewHistory.CurrentRow.DefaultCellStyle.ForeColor = Color.Black;
    }

    private void closeButton_Click(object sender, EventArgs e) => this.Close();

    private void clearButton_Click(object sender, EventArgs e)
    {
      this.m_statList.Clear();
      this.m_callList.Clear();
    }

    private void buttonSaveAll_Click(object sender, EventArgs e)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.DefaultExt = "txt";
      saveFileDialog.Filter = "Text files (*.txt)|*.txt";
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(saveFileDialog.FileName, FileMode.Create)))
      {
        streamWriter.Write("Summary");
        streamWriter.WriteLine();
        streamWriter.Write("Web Service\t\t\tTotal Time\t\tCount\t\tMin Time\t\tMax Time\t\tAverage\t\tLast");
        streamWriter.WriteLine();
        foreach (WebServiceStats stat in (Collection<WebServiceStats>) this.m_statList)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\t{1}\t\t{2}\t\t{3}\t\t{4}\t\t{5}\t\t{6}", (object) stat.WebService, (object) stat.TotalTime, (object) stat.Count, (object) stat.MinTime, (object) stat.MaxTime, (object) stat.Average, (object) stat.Last);
          streamWriter.WriteLine(str);
        }
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.Write("Run Information");
        streamWriter.WriteLine();
        foreach (DataGridViewRow row in (IEnumerable) this.dataGridViewHistory.Rows)
        {
          string str;
          if (row.DataBoundItem is WebServiceCall dataBoundItem)
          {
            string[] strArray = new string[23];
            int num = row.Index + 1;
            strArray[0] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            strArray[1] = "\t\t\t\t--- Row index";
            strArray[2] = Environment.NewLine;
            strArray[3] = dataBoundItem.WebServiceCalled;
            strArray[4] = "\t\t--- Web service called";
            strArray[5] = Environment.NewLine;
            num = dataBoundItem.RunTime;
            strArray[6] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            strArray[7] = "\t\t\t\t--- Run time";
            strArray[8] = Environment.NewLine;
            strArray[9] = dataBoundItem.StartTimeString;
            strArray[10] = "\t\t--- Start Date time stamp";
            strArray[11] = Environment.NewLine;
            strArray[12] = dataBoundItem.EndTimeString;
            strArray[13] = "\t\t--- End Date time stamp";
            strArray[14] = Environment.NewLine;
            num = dataBoundItem.ThreadId;
            strArray[15] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            strArray[16] = "\t\t--- Thread Id";
            strArray[17] = Environment.NewLine;
            strArray[18] = dataBoundItem.ThreadPriority;
            strArray[19] = "\t\t--- Thread Priority";
            strArray[20] = Environment.NewLine;
            strArray[21] = Environment.NewLine;
            strArray[22] = row.Cells[0].ToolTipText;
            str = string.Concat(strArray);
          }
          else
            str = "something wrong";
          streamWriter.Write(str);
          streamWriter.WriteLine();
        }
      }
      using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + " - summary.csv"), FileMode.Create)))
      {
        int num = 0;
        streamWriter.Write("Web Service,Total Time,Count,Min Time,Max Time,Average,Last");
        foreach (WebServiceStats stat in (Collection<WebServiceStats>) this.m_statList)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r{0},{1},{2},{3},{4},{5},{6}", (object) stat.WebService, (object) stat.TotalTime, (object) stat.Count, (object) stat.MinTime, (object) stat.MaxTime, (object) stat.Average, (object) stat.Last);
          num += stat.TotalTime;
          streamWriter.Write(str);
        }
        streamWriter.Write("\r\rTotalTime," + num.ToString());
      }
      using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + " - sequence.csv"), FileMode.Create)))
      {
        int num = 0;
        streamWriter.Write("Web Service,Total Time,Start Time,End Time,Thread Id,Thread Priority");
        foreach (DataGridViewRow row in (IEnumerable) this.dataGridViewHistory.Rows)
        {
          if (row.DataBoundItem is WebServiceCall dataBoundItem)
          {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            object[] objArray = new object[6]
            {
              (object) dataBoundItem.WebServiceCalled,
              (object) dataBoundItem.RunTime,
              null,
              null,
              null,
              null
            };
            DateTime dateTime = dataBoundItem.StartTime;
            objArray[2] = (object) dateTime.ToLongTimeString();
            dateTime = dataBoundItem.EndTime;
            objArray[3] = (object) dateTime.ToLongTimeString();
            objArray[4] = (object) dataBoundItem.ThreadId;
            objArray[5] = (object) dataBoundItem.ThreadPriority;
            string str = string.Format((IFormatProvider) invariantCulture, "\r{0},{1},{2},{3},{4},{5}", objArray);
            num += dataBoundItem.RunTime;
            streamWriter.Write(str);
          }
          else
            streamWriter.Write("\rRow in table with no data attached");
        }
        streamWriter.Write("\r\rTotalTime," + num.ToString());
      }
      using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + " - threads.csv"), FileMode.Create)))
      {
        Dictionary<int, List<WebServiceCall>> dictionary = new Dictionary<int, List<WebServiceCall>>();
        foreach (DataGridViewRow row in (IEnumerable) this.dataGridViewHistory.Rows)
        {
          if (row.DataBoundItem is WebServiceCall dataBoundItem)
          {
            if (dictionary.ContainsKey(dataBoundItem.ThreadId))
            {
              List<WebServiceCall> webServiceCallList = dictionary[dataBoundItem.ThreadId];
              bool flag = false;
              for (int index = 0; index < webServiceCallList.Count; ++index)
              {
                if (dataBoundItem.StartTime < webServiceCallList[index].StartTime)
                {
                  webServiceCallList.Insert(index, dataBoundItem);
                  flag = true;
                }
              }
              if (!flag)
                webServiceCallList.Add(dataBoundItem);
            }
            else
              dictionary[dataBoundItem.ThreadId] = new List<WebServiceCall>()
              {
                dataBoundItem
              };
          }
        }
        foreach (int key in dictionary.Keys)
        {
          int num = 0;
          streamWriter.Write("\r\rCalls for Thread " + key.ToString());
          streamWriter.Write("\rWeb Service,Total Time,Start Time,End Time,Thread Id,Thread Priority");
          WebServiceCall webServiceCall1 = (WebServiceCall) null;
          foreach (WebServiceCall webServiceCall2 in dictionary[key])
          {
            if (webServiceCall1 != null)
            {
              TimeSpan timeSpan = webServiceCall2.StartTime - webServiceCall1.EndTime;
              string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r{0},{1},{2},{3},{4},{5}", (object) "Client Processing", (object) (int) timeSpan.TotalMilliseconds, (object) webServiceCall1.EndTime.ToLongTimeString(), (object) webServiceCall2.StartTime.ToLongTimeString(), (object) webServiceCall2.ThreadId, (object) webServiceCall2.ThreadPriority);
              num += (int) timeSpan.TotalMilliseconds;
              streamWriter.Write(str);
            }
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            object[] objArray = new object[6]
            {
              (object) webServiceCall2.WebServiceCalled,
              (object) webServiceCall2.RunTime,
              null,
              null,
              null,
              null
            };
            DateTime dateTime = webServiceCall2.StartTime;
            objArray[2] = (object) dateTime.ToLongTimeString();
            dateTime = webServiceCall2.EndTime;
            objArray[3] = (object) dateTime.ToLongTimeString();
            objArray[4] = (object) webServiceCall2.ThreadId;
            objArray[5] = (object) webServiceCall2.ThreadPriority;
            string str1 = string.Format((IFormatProvider) invariantCulture, "\r{0},{1},{2},{3},{4},{5}", objArray);
            num += webServiceCall2.RunTime;
            streamWriter.Write(str1);
            webServiceCall1 = webServiceCall2;
          }
          streamWriter.Write("\r\rTotalTime," + num.ToString());
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (DialogPerfListener));
      this.dataGridViewStats = new DataGridView();
      this.WebServiceStatListBindingSource = new BindingSource(this.components);
      this.closeButton = new Button();
      this.clearButton = new Button();
      this.overarchingtableLayoutPanel = new TableLayoutPanel();
      this.dataGridViewHistory = new DataGridView();
      this.WebServiceCalled = new DataGridViewTextBoxColumn();
      this.RunTime = new DataGridViewTextBoxColumn();
      this.DateTimeStamp = new DataGridViewTextBoxColumn();
      this.ThreadId = new DataGridViewTextBoxColumn();
      this.WebServiceCallListBindingSource = new BindingSource(this.components);
      this.WebService = new DataGridViewTextBoxColumn();
      this.buttonSaveAll = new Button();
      this.TotalTime = new DataGridViewTextBoxColumn();
      this.Last = new DataGridViewTextBoxColumn();
      this.Count = new DataGridViewTextBoxColumn();
      this.Average = new DataGridViewTextBoxColumn();
      this.MinTime = new DataGridViewTextBoxColumn();
      this.MaxTime = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.dataGridViewStats).BeginInit();
      ((ISupportInitialize) this.WebServiceStatListBindingSource).BeginInit();
      this.overarchingtableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.dataGridViewHistory).BeginInit();
      ((ISupportInitialize) this.WebServiceCallListBindingSource).BeginInit();
      this.SuspendLayout();
      this.dataGridViewStats.AllowUserToAddRows = false;
      this.dataGridViewStats.AllowUserToDeleteRows = false;
      this.dataGridViewStats.AllowUserToOrderColumns = true;
      componentResourceManager.ApplyResources((object) this.dataGridViewStats, "dataGridViewStats");
      this.dataGridViewStats.AutoGenerateColumns = false;
      this.dataGridViewStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
      this.dataGridViewStats.Columns.AddRange((DataGridViewColumn) this.WebService, (DataGridViewColumn) this.TotalTime, (DataGridViewColumn) this.Last, (DataGridViewColumn) this.Count, (DataGridViewColumn) this.Average, (DataGridViewColumn) this.MinTime, (DataGridViewColumn) this.MaxTime);
      this.overarchingtableLayoutPanel.SetColumnSpan((Control) this.dataGridViewStats, 5);
      this.dataGridViewStats.DataSource = (object) this.WebServiceStatListBindingSource;
      this.dataGridViewStats.Name = "dataGridViewStats";
      this.dataGridViewStats.ReadOnly = true;
      this.dataGridViewStats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.WebServiceStatListBindingSource.DataSource = (object) typeof (WebServiceStatList);
      componentResourceManager.ApplyResources((object) this.closeButton, "closeButton");
      this.closeButton.DialogResult = DialogResult.Cancel;
      this.closeButton.Name = "closeButton";
      this.closeButton.Click += new EventHandler(this.closeButton_Click);
      componentResourceManager.ApplyResources((object) this.clearButton, "clearButton");
      this.clearButton.Name = "clearButton";
      this.clearButton.Click += new EventHandler(this.clearButton_Click);
      componentResourceManager.ApplyResources((object) this.overarchingtableLayoutPanel, "overarchingtableLayoutPanel");
      this.overarchingtableLayoutPanel.Controls.Add((Control) this.dataGridViewStats, 0, 0);
      this.overarchingtableLayoutPanel.Controls.Add((Control) this.dataGridViewHistory, 0, 1);
      this.overarchingtableLayoutPanel.Controls.Add((Control) this.closeButton, 4, 2);
      this.overarchingtableLayoutPanel.Controls.Add((Control) this.clearButton, 3, 2);
      this.overarchingtableLayoutPanel.Controls.Add((Control) this.buttonSaveAll, 1, 2);
      this.overarchingtableLayoutPanel.Name = "overarchingtableLayoutPanel";
      this.dataGridViewHistory.AllowUserToAddRows = false;
      this.dataGridViewHistory.AllowUserToDeleteRows = false;
      componentResourceManager.ApplyResources((object) this.dataGridViewHistory, "dataGridViewHistory");
      this.dataGridViewHistory.AutoGenerateColumns = false;
      this.dataGridViewHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
      this.dataGridViewHistory.Columns.AddRange((DataGridViewColumn) this.WebServiceCalled, (DataGridViewColumn) this.RunTime, (DataGridViewColumn) this.DateTimeStamp, (DataGridViewColumn) this.ThreadId);
      this.overarchingtableLayoutPanel.SetColumnSpan((Control) this.dataGridViewHistory, 5);
      this.dataGridViewHistory.DataSource = (object) this.WebServiceCallListBindingSource;
      this.dataGridViewHistory.Name = "dataGridViewHistory";
      this.dataGridViewHistory.ReadOnly = true;
      this.dataGridViewHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.WebServiceCalled.DataPropertyName = "WebServiceCalled";
      componentResourceManager.ApplyResources((object) this.WebServiceCalled, "WebServiceCalled");
      this.WebServiceCalled.Name = "WebServiceCalled";
      this.WebServiceCalled.ReadOnly = true;
      this.RunTime.DataPropertyName = "RunTime";
      componentResourceManager.ApplyResources((object) this.RunTime, "RunTime");
      this.RunTime.Name = "RunTime";
      this.RunTime.ReadOnly = true;
      this.DateTimeStamp.DataPropertyName = "EndTimeString";
      componentResourceManager.ApplyResources((object) this.DateTimeStamp, "DateTimeStamp");
      this.DateTimeStamp.Name = "DateTimeStamp";
      this.DateTimeStamp.ReadOnly = true;
      this.ThreadId.DataPropertyName = "ThreadId";
      componentResourceManager.ApplyResources((object) this.ThreadId, "ThreadId");
      this.ThreadId.Name = "ThreadId";
      this.ThreadId.ReadOnly = true;
      this.WebServiceCallListBindingSource.DataSource = (object) typeof (WebServiceCallList);
      this.WebService.DataPropertyName = "WebService";
      componentResourceManager.ApplyResources((object) this.WebService, "WebService");
      this.WebService.Name = "WebService";
      this.WebService.ReadOnly = true;
      this.TotalTime.DataPropertyName = "TotalTime";
      componentResourceManager.ApplyResources((object) this.TotalTime, "TotalTime");
      this.TotalTime.Name = "TotalTime";
      this.TotalTime.ReadOnly = true;
      this.Last.DataPropertyName = "Last";
      componentResourceManager.ApplyResources((object) this.Last, "Last");
      this.Last.Name = "Last";
      this.Last.ReadOnly = true;
      this.Count.DataPropertyName = "Count";
      componentResourceManager.ApplyResources((object) this.Count, "Count");
      this.Count.Name = "Count";
      this.Count.ReadOnly = true;
      this.Average.DataPropertyName = "Average";
      componentResourceManager.ApplyResources((object) this.Average, "Average");
      this.Average.Name = "Average";
      this.Average.ReadOnly = true;
      this.MinTime.DataPropertyName = "MinTime";
      componentResourceManager.ApplyResources((object) this.MinTime, "MinTime");
      this.MinTime.Name = "MinTime";
      this.MinTime.ReadOnly = true;
      this.MaxTime.DataPropertyName = "MaxTime";
      componentResourceManager.ApplyResources((object) this.MaxTime, "MaxTime");
      this.MaxTime.Name = "MaxTime";
      this.MaxTime.ReadOnly = true;
      componentResourceManager.ApplyResources((object) this.buttonSaveAll, "buttonSaveAll");
      this.buttonSaveAll.Name = "buttonSaveAll";
      this.buttonSaveAll.UseVisualStyleBackColor = true;
      this.buttonSaveAll.Click += new EventHandler(this.buttonSaveAll_Click);
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.overarchingtableLayoutPanel);
      this.Name = nameof (DialogPerfListener);
      ((ISupportInitialize) this.dataGridViewStats).EndInit();
      ((ISupportInitialize) this.WebServiceStatListBindingSource).EndInit();
      this.overarchingtableLayoutPanel.ResumeLayout(false);
      ((ISupportInitialize) this.dataGridViewHistory).EndInit();
      ((ISupportInitialize) this.WebServiceCallListBindingSource).EndInit();
      this.ResumeLayout(false);
    }
  }
}
