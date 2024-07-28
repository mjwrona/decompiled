// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LogspaceDetails
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LogspaceDetails
  {
    public int SessionId { get; set; }

    public DateTime TransactionBeginTime { get; set; }

    public int TransactionSeconds { get; set; }

    public long LogBytesUsed { get; set; }

    public long LogBytesReserved { get; set; }

    public string HostName { get; set; }

    public string ClientNetAddress { get; set; }

    public string ProgramName { get; set; }

    public int HostProcessId { get; set; }

    public string LoginName { get; set; }

    public DateTime LoginTime { get; set; }

    public int LoginSeconds { get; set; }

    public string Status { get; set; }

    public int CpuTime { get; set; }

    public long Reads { get; set; }

    public long Writes { get; set; }

    public string Text { get; set; }

    public override string ToString() => string.Format("{0}: {1}, {2}: {3}, {4}: {5}, \r\n                      {6}: {7}, {8}: {9}, \r\n                      {10}: {11}, {12}: {13}, {14}: {15}, \r\n                      {16}: {17}, {18}: {19}, {20}: {21}, \r\n                      {22}: {23}, {24}: {25}, {26}: {27}, {28}: {29}, {30}: {31}", (object) "SessionId", (object) this.SessionId, (object) "Text", (object) this.Text, (object) "TransactionBeginTime", (object) this.TransactionBeginTime, (object) "LogBytesUsed", (object) this.LogBytesUsed, (object) "LogBytesReserved", (object) this.LogBytesReserved, (object) "HostName", (object) this.HostName, (object) "ClientNetAddress", (object) this.ClientNetAddress, (object) "ProgramName", (object) this.ProgramName, (object) "HostProcessId", (object) this.HostProcessId, (object) "LoginName", (object) this.LoginName, (object) "LoginTime", (object) this.LoginTime, (object) "LoginSeconds", (object) this.LoginSeconds, (object) "Status", (object) this.Status, (object) "CpuTime", (object) this.CpuTime, (object) "Reads", (object) this.Reads, (object) "Writes", (object) this.Writes);
  }
}
