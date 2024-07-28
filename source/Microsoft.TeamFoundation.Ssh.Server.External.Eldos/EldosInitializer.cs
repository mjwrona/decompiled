// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.EldosInitializer
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using SBBaseClasses;
using SBDumper;
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal static class EldosInitializer
  {
    private static bool s_initialized;
    private static readonly object s_lock = new object();
    private const string c_layer = "EldosInitializer";

    public static void Init()
    {
      lock (EldosInitializer.s_lock)
      {
        if (EldosInitializer.s_initialized)
          return;
        EldosInitializer.InitLicense();
        EldosInitializer.SetTracingExceptionHandler();
        EldosInitializer.SeedRandom();
        SBSSHServer.__Global.G_MinDHGroupSize = 2048;
        EldosInitializer.s_initialized = true;
      }
    }

    internal static void InitLicense()
    {
      try
      {
        SBUtils.__Global.SetLicenseKey("53424E4841444E585246323032343037303631395342323035310052414A4257574A52584442560030303030303030300000594B43574234334B57474E350000");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13001038, "Ssh", nameof (EldosInitializer), ex);
        throw;
      }
    }

    private static void SetTracingExceptionHandler()
    {
      try
      {
        SBBaseClasses.__Global.SetDefaultExceptionHandler((TElCustomExceptionHandler) new EldosInitializer.TracingExceptionHandler(SBBaseClasses.__Global.GetDefaultExceptionHandler()));
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13001037, "Ssh", nameof (EldosInitializer), ex);
        throw;
      }
    }

    private static void SeedRandom()
    {
      try
      {
        using (RandomNumberGenerator randomNumberGenerator = (RandomNumberGenerator) new RNGCryptoServiceProvider())
        {
          byte[] numArray = new byte[13];
          randomNumberGenerator.GetBytes(numArray);
          SBRandom.__Global.SBRndSeed(numArray);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13001039, "Ssh", nameof (EldosInitializer), ex);
        throw;
      }
    }

    private static void EnableSBBLogFile()
    {
      SBDumper.__Global.Dumper.DumpToFile = true;
      SBDumper.__Global.Dumper.DumpFile = "D:\\SBBLogs\\sbb.log";
      SBDumper.__Global.Dumper.FlushMode = TSBDumperFlushMode.dfmImmediateFlush;
      SBDumper.__Global.Dumper.Level = TSBDumperDetailLevel.ddlTrace;
      SBDumper.__Global.Dumper.RotationMode = TSBDumperRotationMode.drmNoRotation;
    }

    private class TracingExceptionHandler : TElCustomExceptionHandler
    {
      private readonly TElCustomExceptionHandler m_parent;
      private const string c_layer = "TracingExceptionHandler";

      public TracingExceptionHandler(TElCustomExceptionHandler parent) => this.m_parent = parent;

      public override bool HandleException(Exception ex)
      {
        bool flag = this.m_parent.HandleException(ex);
        TeamFoundationTracingService.TraceExceptionRaw(13001036, TraceLevel.Error, "Ssh", nameof (TracingExceptionHandler), ex, string.Format("{0}: {1}, ex: {2}", (object) nameof (HandleException), (object) flag, (object) ex.ToReadableStackTrace()));
        return flag;
      }
    }
  }
}
