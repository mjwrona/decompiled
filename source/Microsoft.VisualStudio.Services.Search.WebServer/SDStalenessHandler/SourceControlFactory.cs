// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.SourceControlFactory
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  public static class SourceControlFactory
  {
    public static string GetSourceControlAzureLocation(string sourceControl, string branch)
    {
      switch (sourceControl)
      {
        case "BingSourceDepot":
          return "WorkItemsGroup1";
        case "OfficeSourceDepot":
          return "WorkItemsGroup1";
        case "ExchangeSourceDepot":
          return "WorkItemsGroup1";
        case "DynamicsAX6SourceDepot":
          if (branch != null)
          {
            switch (branch.Length)
            {
              case 6:
                switch (branch[1])
                {
                  case 'A':
                    if (branch == "DAX6HF")
                      break;
                    goto label_16;
                  case 'Y':
                    if (branch == "DYNMOB")
                      break;
                    goto label_16;
                  default:
                    goto label_16;
                }
                break;
              case 7:
                switch (branch[5])
                {
                  case 'G':
                    if (branch == "DAX63GB")
                      break;
                    goto label_16;
                  case 'H':
                    if (branch == "DAX62HF" || branch == "DAX63HF")
                      break;
                    goto label_16;
                  case 'S':
                    if (branch == "DAX63SE")
                      break;
                    goto label_16;
                  default:
                    goto label_16;
                }
                break;
              case 8:
                if (branch == "DAX62OOB")
                  break;
                goto label_16;
              case 10:
                if (!(branch == "DAX6RESKIT"))
                  goto label_16;
                else
                  break;
              default:
                goto label_16;
            }
            return "WorkItemsGroup1";
          }
label_16:
          return "WorkItemsGroup2";
        case "DynamicsAXSourceDepot":
          return "WorkItemsGroup2";
        case "DynamicsAX5SourceDepot":
          return "WorkItemsGroup2";
        case "DynamicsNAVSourceDepot":
          return "WorkItemsGroup2";
        case "DynamicsESSSourceDepot":
          return "WorkItemsGroup2";
        case "IntuneSourceDepot":
          return "WorkItemsGroup2";
        case "OneDriveSourceDepot":
          return "WorkItemsGroup2";
        case "PacmanSourceDepot":
          return "WorkItemsGroup2";
        case "SCCMSourceDepot":
          return "WorkItemsGroup2";
        case "SPORelSourceDepot":
          return "WorkItemsGroup2";
        case "SkypeForBusinessSourceDepot":
          return "WorkItemsGroup2";
        case "BGIT_SDLGO_SD":
          return "WorkItemsGroup3";
        case "Blue_SD":
          return "WorkItemsGroup3";
        case "NT_3_51_SD":
          return "WorkItemsGroup3";
        case "NT_3_5_SD":
          return "WorkItemsGroup3";
        case "Redstone_SD":
          return "WorkItemsGroup3";
        case "SDWINCEARC_SD":
          return "WorkItemsGroup3";
        case "Threshold_SD":
          return "WorkItemsGroup3";
        case "Windows_2000_SD":
          return "WorkItemsGroup3";
        case "Windows_7_SD":
          return "WorkItemsGroup3";
        case "Windows_8_SD":
          return "WorkItemsGroup3";
        case "Windows_Phone_SD":
          return "WorkItemsGroup3";
        case "Windows_Server_2003_SD":
          return "WorkItemsGroup3";
        case "Windows_Vista_SD":
          return "WorkItemsGroup3";
        case "Windows_XP_SD":
          return "WorkItemsGroup3";
        case "Xbox_SD":
          return "WorkItemsGroup3";
        case "ANDSB_TeamProject":
          return "WorkItemsGroup3";
        case "OfficeSourceDepotPfTest":
          return "WorkItemsGroup4";
        default:
          return string.Empty;
      }
    }
  }
}
