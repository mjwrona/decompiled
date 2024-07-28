// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableMigrationResult
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableMigrationResult
  {
    private const char ClauseSeparator = '|';
    private const char StatSeparator = ',';
    private const char KVPSeparator = '=';
    private long CP;
    private long CS;
    private long CF;
    private long CI;
    private long DP;
    private long DD;

    public StorageMigrationStatus Status { get; set; }

    private string PreviousMessage { get; set; }

    private StorageMigration Container { get; set; }

    private TableMigrationResult()
    {
    }

    internal void Merge(CopyStats copy, DeleteStats delete)
    {
      if (copy != null)
      {
        this.CP += copy.totalProcessed;
        this.CS += copy.totalSuccess;
        this.CF += copy.totalFailure;
        this.CI += copy.totalIgnored;
      }
      if (delete == null)
        return;
      this.DP += delete.totalProcessed;
      this.DD += delete.totalDeleted;
    }

    internal void UpdateDatabase(IVssRequestContext deploymentContext)
    {
      string statusReason = this.BuildMessage();
      using (HostMigrationComponent component = deploymentContext.CreateComponent<HostMigrationComponent>())
        component.UpdateContainerMigrationStatus(this.Container, this.Status, statusReason);
    }

    private string BuildMessage()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("CP");
      stringBuilder.Append('=');
      stringBuilder.Append(this.CP);
      stringBuilder.Append(',');
      stringBuilder.Append("CS");
      stringBuilder.Append('=');
      stringBuilder.Append(this.CS);
      stringBuilder.Append(',');
      stringBuilder.Append("CF");
      stringBuilder.Append('=');
      stringBuilder.Append(this.CF);
      stringBuilder.Append(',');
      stringBuilder.Append("CI");
      stringBuilder.Append('=');
      stringBuilder.Append(this.CI);
      stringBuilder.Append(',');
      stringBuilder.Append("DP");
      stringBuilder.Append('=');
      stringBuilder.Append(this.DP);
      stringBuilder.Append(',');
      stringBuilder.Append("DD");
      stringBuilder.Append('=');
      stringBuilder.Append(this.DD);
      if (!string.IsNullOrEmpty(this.PreviousMessage))
      {
        stringBuilder.Append('|');
        stringBuilder.Append(this.PreviousMessage);
      }
      return stringBuilder.ToString();
    }

    public static TableMigrationResult ReadFromStatus(
      StorageMigration container,
      IDictionary<string, StorageMigration> containerDict)
    {
      TableMigrationResult tableMigrationResult = (TableMigrationResult) null;
      string str1 = (string) null;
      StorageMigration storageMigration;
      if (containerDict.TryGetValue(container.VsoArea, out storageMigration))
      {
        StorageMigrationStatus status = storageMigration.Status;
        string statusReason = storageMigration.StatusReason;
        if (statusReason != null)
        {
          int num1 = -1;
          string[] strArray1 = statusReason.Split('|');
          for (int index = 0; index < strArray1.Length; ++index)
          {
            string str2 = strArray1[index];
            long num2 = 0;
            long num3 = 0;
            long num4 = 0;
            long num5 = 0;
            long num6 = 0;
            long num7 = 0;
            int num8 = 0;
            string[] strArray2 = str2.Trim().Split(',');
            if (strArray2.Length >= 2)
            {
              foreach (string str3 in strArray2)
              {
                string[] strArray3 = str2.Trim().Split('=');
                long result;
                if (strArray3.Length == 2 && long.TryParse(strArray3[1], out result))
                {
                  switch (strArray3[0])
                  {
                    case "CP":
                      num2 = result;
                      break;
                    case "CS":
                      num3 = result;
                      break;
                    case "CF":
                      num4 = result;
                      break;
                    case "CI":
                      num5 = result;
                      break;
                    case "DP":
                      num6 = result;
                      break;
                    case "DD":
                      num7 = result;
                      break;
                  }
                  ++num8;
                }
              }
            }
            if (num8 >= 2)
            {
              tableMigrationResult = new TableMigrationResult()
              {
                CP = num2,
                CS = num3,
                CF = num4,
                CI = num5,
                DP = num6,
                DD = num7,
                Status = status
              };
              num1 = index;
              break;
            }
          }
          if (num1 >= 0)
          {
            bool flag = true;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < strArray1.Length; ++index)
            {
              if (index != num1)
              {
                if (!flag)
                  stringBuilder.Append('|');
                string str4 = strArray1[index];
                stringBuilder.Append(str4);
                flag = false;
              }
            }
            str1 = stringBuilder.ToString();
          }
        }
      }
      if (tableMigrationResult == null)
        tableMigrationResult = new TableMigrationResult()
        {
          Status = StorageMigrationStatus.Queued
        };
      tableMigrationResult.PreviousMessage = str1;
      tableMigrationResult.Container = container;
      return tableMigrationResult;
    }
  }
}
