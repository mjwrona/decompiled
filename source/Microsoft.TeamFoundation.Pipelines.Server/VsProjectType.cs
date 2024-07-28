// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.VsProjectType
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class VsProjectType
  {
    public static readonly string UnknownTypeName = "UnknownType";
    private static readonly Dictionary<Guid, string> s_projectTypes = new Dictionary<Guid, string>()
    {
      {
        VsProjectType.WellKnownTypes.ASPNET_5,
        "ASP.NET 5"
      },
      {
        VsProjectType.WellKnownTypes.ASPNET_CORE,
        "ASP.net (Core)"
      },
      {
        VsProjectType.WellKnownTypes.ASPNET_MVC_1,
        "ASP.NET MVC 1.0"
      },
      {
        VsProjectType.WellKnownTypes.ASPNET_MVC_2,
        "ASP.NET MVC 2.0"
      },
      {
        VsProjectType.WellKnownTypes.ASPNET_MVC_3,
        "ASP.NET MVC 3.0"
      },
      {
        VsProjectType.WellKnownTypes.ASPNET_MVC_4,
        "ASP.NET MVC 4.0"
      },
      {
        VsProjectType.WellKnownTypes.CPP,
        "C++"
      },
      {
        VsProjectType.WellKnownTypes.CSharp,
        "C#"
      },
      {
        VsProjectType.WellKnownTypes.Database,
        "Database"
      },
      {
        VsProjectType.WellKnownTypes.Database_Other,
        "Database (other project types)"
      },
      {
        VsProjectType.WellKnownTypes.DeploymentCab,
        "Deployment Cab"
      },
      {
        VsProjectType.WellKnownTypes.DeploymentMergeModule,
        "Deployment Merge Module"
      },
      {
        VsProjectType.WellKnownTypes.DeploymentSetup,
        "Deployment Setup"
      },
      {
        VsProjectType.WellKnownTypes.DeploymentSmartDeviceCab,
        "Deployment Smart Device Cab"
      },
      {
        VsProjectType.WellKnownTypes.DistributedSystem,
        "Distributed System"
      },
      {
        VsProjectType.WellKnownTypes.DOTNETCORE,
        "Dotnet core"
      },
      {
        VsProjectType.WellKnownTypes.DOTNETCORE_TEST,
        "Dotnet core test"
      },
      {
        VsProjectType.WellKnownTypes.DOTNETCORE_WEB,
        "Dotnet core web"
      },
      {
        VsProjectType.WellKnownTypes.Dynamics_CSharp,
        "C# in Dynamics 2012 AX AOT"
      },
      {
        VsProjectType.WellKnownTypes.EXE,
        "EXE"
      },
      {
        VsProjectType.WellKnownTypes.FSharp,
        "F#"
      },
      {
        VsProjectType.WellKnownTypes.FUNCTION,
        "Function App (C#)"
      },
      {
        VsProjectType.WellKnownTypes.JSharp,
        "J#"
      },
      {
        VsProjectType.WellKnownTypes.LegacySmartDevice,
        "Legacy (2003) Smart Device (C#)"
      },
      {
        VsProjectType.WellKnownTypes.LegacySmartDevice_VB,
        "Legacy (2003) Smart Device (VB.NET)"
      },
      {
        VsProjectType.WellKnownTypes.MicroFramework,
        "Micro Framework"
      },
      {
        VsProjectType.WellKnownTypes.MonoTouch,
        "MonoTouch"
      },
      {
        VsProjectType.WellKnownTypes.MonoTouchBinding,
        "MonoTouch Binding"
      },
      {
        VsProjectType.WellKnownTypes.PortableClassLibrary,
        "Portable Class Library"
      },
      {
        VsProjectType.WellKnownTypes.ProjectFolders,
        "Project Folders"
      },
      {
        VsProjectType.WellKnownTypes.SharePoint,
        "SharePoint (C#)"
      },
      {
        VsProjectType.WellKnownTypes.SharePointWorkflow,
        "SharePoint Workflow"
      },
      {
        VsProjectType.WellKnownTypes.SharePoint_VB,
        "SharePoint (VB.NET)"
      },
      {
        VsProjectType.WellKnownTypes.Silverlight,
        "Silverlight"
      },
      {
        VsProjectType.WellKnownTypes.SmartDevice_CSharp,
        "Smart Device (C#)"
      },
      {
        VsProjectType.WellKnownTypes.SmartDevice_VB,
        "Smart Device (VB.NET)"
      },
      {
        VsProjectType.WellKnownTypes.SolutionFolder,
        "Solution Folder"
      },
      {
        VsProjectType.WellKnownTypes.Test,
        "Test"
      },
      {
        VsProjectType.WellKnownTypes.UniversalWindowsClassLibrary,
        "Universal Windows Class Library"
      },
      {
        VsProjectType.WellKnownTypes.VB,
        "VB.NET"
      },
      {
        VsProjectType.WellKnownTypes.VSTA,
        "Visual Studio Tools for Applications (VSTA)"
      },
      {
        VsProjectType.WellKnownTypes.VSTO,
        "Visual Studio Tools for Office (VSTO)"
      },
      {
        VsProjectType.WellKnownTypes.VisualDatabaseTools,
        "Visual Database Tools"
      },
      {
        VsProjectType.WellKnownTypes.VisualStudio2015Installer,
        "Visual Studio 2015 Installer Project Extension"
      },
      {
        VsProjectType.WellKnownTypes.WebApplication,
        "Web Application (incl. MVC 5)"
      },
      {
        VsProjectType.WellKnownTypes.WebSite,
        "Web Site"
      },
      {
        VsProjectType.WellKnownTypes.WindowsCommunicationFoundation,
        "Windows Communication Foundation (WCF)"
      },
      {
        VsProjectType.WellKnownTypes.WindowsPhone8_CSharp,
        "Windows Phone 8/8.1 App (C#)"
      },
      {
        VsProjectType.WellKnownTypes.WindowsPhone8_VB,
        "Windows Phone 8/8.1 App (VB.NET)"
      },
      {
        VsProjectType.WellKnownTypes.WindowsPhone8_Webview,
        "Windows Phone 8/8.1 Blank/Hub/Webview App"
      },
      {
        VsProjectType.WellKnownTypes.WindowsPresentationFoundation,
        "Windows Presentation Foundation (WPF)"
      },
      {
        VsProjectType.WellKnownTypes.WindowsStoreApps,
        "Windows Store Apps (Metro Apps)"
      },
      {
        VsProjectType.WellKnownTypes.WorkflowFoundation,
        "Workflow Foundation"
      },
      {
        VsProjectType.WellKnownTypes.Workflow_CSharp,
        "Workflow (C#)"
      },
      {
        VsProjectType.WellKnownTypes.Workflow_VB,
        "Workflow (VB.NET)"
      },
      {
        VsProjectType.WellKnownTypes.Xamarin_Android,
        "Xamarin.Android / Mono for Android"
      },
      {
        VsProjectType.WellKnownTypes.Xamarin_iOS,
        "Xamarin iOS"
      },
      {
        VsProjectType.WellKnownTypes.XNA_Windows,
        "XNA (Windows)"
      },
      {
        VsProjectType.WellKnownTypes.XNA_Xbox,
        "XNA (Xbox)"
      },
      {
        VsProjectType.WellKnownTypes.XNA_Zune,
        "XNA (Zune)"
      }
    };

    public VsProjectType(Guid type, string name)
    {
      this.Type = type;
      this.Name = name;
    }

    public Guid Type { get; }

    public string Name { get; }

    public static VsProjectType FromGuid(Guid projectType) => !VsProjectType.s_projectTypes.ContainsKey(projectType) ? new VsProjectType(projectType, VsProjectType.UnknownTypeName) : new VsProjectType(projectType, VsProjectType.s_projectTypes[projectType]);

    public static class WellKnownTypes
    {
      public static readonly Guid EXE = new Guid("{00000000-0000-0000-0000-000000000001}");
      public static readonly Guid FUNCTION = new Guid("{00000000-0000-0000-0000-000000000002}");
      public static readonly Guid ASPNET_CORE = new Guid("{00000000-0000-0000-0000-000000000003}");
      public static readonly Guid DOTNETCORE = new Guid("{00000000-0000-0000-0000-000000000004}");
      public static readonly Guid DOTNETCORE_WEB = new Guid("{00000000-0000-0000-0000-000000000005}");
      public static readonly Guid DOTNETCORE_TEST = new Guid("{00000000-0000-0000-0000-000000000006}");
      public static readonly Guid CPP = new Guid("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}");
      public static readonly Guid CSharp = new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
      public static readonly Guid FSharp = new Guid("{F2A71F9B-5D33-465A-A702-920D77279786}");
      public static readonly Guid JSharp = new Guid("{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}");
      public static readonly Guid VB = new Guid("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}");
      public static readonly Guid ASPNET_5 = new Guid("{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}");
      public static readonly Guid ASPNET_MVC_1 = new Guid("{603C0E0B-DB56-11DC-BE95-000D561079B0}");
      public static readonly Guid ASPNET_MVC_2 = new Guid("{F85E285D-A4E0-4152-9332-AB1D724D3325}");
      public static readonly Guid ASPNET_MVC_3 = new Guid("{E53F8FEA-EAE0-44A6-8774-FFD645390401}");
      public static readonly Guid ASPNET_MVC_4 = new Guid("{E3E379DF-F4C6-4180-9B81-6769533ABE47}");
      public static readonly Guid Database = new Guid("{A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124}");
      public static readonly Guid Database_Other = new Guid("{4F174C21-8C12-11D0-8340-0000F80270F8}");
      public static readonly Guid DeploymentCab = new Guid("{3EA9E505-35AC-4774-B492-AD1749C4943A}");
      public static readonly Guid DeploymentMergeModule = new Guid("{06A35CCD-C46D-44D5-987B-CF40FF872267}");
      public static readonly Guid DeploymentSetup = new Guid("{978C614F-708E-4E1A-B201-565925725DBA}");
      public static readonly Guid DeploymentSmartDeviceCab = new Guid("{AB322303-2255-48EF-A496-5904EB18DA55}");
      public static readonly Guid DistributedSystem = new Guid("{F135691A-BF7E-435D-8960-F99683D2D49C}");
      public static readonly Guid Dynamics_CSharp = new Guid("{BF6F8E12-879D-49E7-ADF0-5503146B24B8}");
      public static readonly Guid LegacySmartDevice = new Guid("{20D4826A-C6FA-45DB-90F4-C717570B9F32}");
      public static readonly Guid LegacySmartDevice_VB = new Guid("{CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8}");
      public static readonly Guid MicroFramework = new Guid("{B69E3092-B931-443C-ABE7-7E7B65F2A37F}");
      public static readonly Guid MonoTouch = new Guid("{6BC8ED88-2882-458C-8E55-DFD12B67127B}");
      public static readonly Guid MonoTouchBinding = new Guid("{F5B4F3BC-B597-4E2B-B552-EF5D8A32436F}");
      public static readonly Guid PortableClassLibrary = new Guid("{786C830F-07A1-408B-BD7F-6EE04809D6DB}");
      public static readonly Guid ProjectFolders = new Guid("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}");
      public static readonly Guid SharePoint = new Guid("{593B0543-81F6-4436-BA1E-4747859CAAE2}");
      public static readonly Guid SharePointWorkflow = new Guid("{F8810EC1-6754-47FC-A15F-DFABD2E3FA90}");
      public static readonly Guid SharePoint_VB = new Guid("{EC05E597-79D4-47f3-ADA0-324C4F7C7484}");
      public static readonly Guid Silverlight = new Guid("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}");
      public static readonly Guid SmartDevice_CSharp = new Guid("{4D628B5B-2FBC-4AA6-8C16-197242AEB884}");
      public static readonly Guid SmartDevice_VB = new Guid("{68B1623D-7FB9-47D8-8664-7ECEA3297D4F}");
      public static readonly Guid SolutionFolder = new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
      public static readonly Guid Test = new Guid("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");
      public static readonly Guid UniversalWindowsClassLibrary = new Guid("{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}");
      public static readonly Guid VSTA = new Guid("{A860303F-1F3F-4691-B57E-529FC101A107}");
      public static readonly Guid VSTO = new Guid("{BAA0C2D2-18E2-41B9-852F-F413020CAA33}");
      public static readonly Guid VisualDatabaseTools = new Guid("{C252FEB5-A946-4202-B1D4-9916A0590387}");
      public static readonly Guid VisualStudio2015Installer = new Guid("{54435603-DBB4-11D2-8724-00A0C9A8B90C}");
      public static readonly Guid WebApplication = new Guid("{349C5851-65DF-11DA-9384-00065B846F21}");
      public static readonly Guid WebSite = new Guid("{E24C65DC-7377-472B-9ABA-BC803B73C61A}");
      public static readonly Guid WindowsCommunicationFoundation = new Guid("{3D9AD99F-2412-4246-B90B-4EAA41C64699}");
      public static readonly Guid WindowsPhone8_CSharp = new Guid("{C089C8C0-30E0-4E22-80C0-CE093F111A43}");
      public static readonly Guid WindowsPhone8_VB = new Guid("{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}");
      public static readonly Guid WindowsPhone8_Webview = new Guid("{76F1466A-8B6D-4E39-A767-685A06062A39}");
      public static readonly Guid WindowsPresentationFoundation = new Guid("{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}");
      public static readonly Guid WindowsStoreApps = new Guid("{BC8A1FFA-BEE3-4634-8014-F334798102B3}");
      public static readonly Guid WorkflowFoundation = new Guid("{32F31D43-81CC-4C15-9DE6-3FC5453562B6}");
      public static readonly Guid Workflow_CSharp = new Guid("{14822709-B5A1-4724-98CA-57A101D1B079}");
      public static readonly Guid Workflow_VB = new Guid("{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}");
      public static readonly Guid Xamarin_Android = new Guid("{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}");
      public static readonly Guid Xamarin_iOS = new Guid("{FEACFBD2-3405-455C-9665-78FE426C6842}");
      public static readonly Guid XNA_Windows = new Guid("{6D335F3A-9D43-41b4-9D22-F6F17C4BE596}");
      public static readonly Guid XNA_Xbox = new Guid("{2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}");
      public static readonly Guid XNA_Zune = new Guid("{D399B71A-8929-442a-A9AC-8BEC78BB2433}");
    }
  }
}
