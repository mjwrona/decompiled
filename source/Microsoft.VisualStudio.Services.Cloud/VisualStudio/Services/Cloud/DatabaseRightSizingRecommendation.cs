// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabaseRightSizingRecommendation
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DatabaseRightSizingRecommendation
  {
    public string ServerName;
    public string Listener;
    public string DatabaseName;
    public string Region;
    public string Service;
    public double Max80thDTU;
    public double MaxDTU;
    public string DTUComputedBy;
    public string Utilization;
    public double Max80thCPU;
    public double Max80thIO;
    public double Max80thLogWrite;
    public double MaxMemory;
    public string Slo;
    public string ActionToTake;
    public bool DatabaseRecentlyUpdated;

    public override string ToString() => string.Format("\r\n                      ServerName =      {0}\r\n                      Listener =        {1}\r\n                      DatabaseName =    {2}\r\n                      Region =          {3}\r\n                      Service =         {4}\r\n                      Max80thDTU =      {5}\r\n                      MaxDTU =          {6}\r\n                      DTUComputedBy =   {7}\r\n                      Utilization =     {8}\r\n                      Max80thCPU =      {9}\r\n                      Max80thIO =       {10}\r\n                      Max80thLogWrite = {11}\r\n                      MaxMemory =       {12}\r\n                      ServiceObjecive = {13}\r\n                      ActionToTake =    {14}\r\n                      DatabaseRecentlyUpdated = {15}\r\n                    ", (object) this.ServerName, (object) this.Listener, (object) this.DatabaseName, (object) this.Region, (object) this.Service, (object) this.Max80thDTU, (object) this.MaxDTU, (object) this.DTUComputedBy, (object) this.Utilization, (object) this.Max80thCPU, (object) this.Max80thIO, (object) this.Max80thLogWrite, (object) this.MaxMemory, (object) this.Slo, (object) this.ActionToTake, (object) this.DatabaseRecentlyUpdated);
  }
}
