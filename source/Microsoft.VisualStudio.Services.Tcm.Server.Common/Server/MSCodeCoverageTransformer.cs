// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MSCodeCoverageTransformer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Analysis;
using Microsoft.CodeCoverage.Analysis.Exceptions;
using Microsoft.CodeCoverage.Analysis.Reports;
using Microsoft.CodeCoverage.Core;
using Microsoft.CodeCoverage.IO.Coverage;
using Microsoft.CodeCoverage.IO.Coverage.Report;
using Microsoft.CodeCoverage.IO.Exceptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class MSCodeCoverageTransformer
  {
    private const string BlocksLabel = "Blocks";
    private const string LinesLabel = "Lines";
    private const int BlocksPosition = 1;
    private const int LinesPosition = 2;

    public BuildCoverage QueryBuildCoverage(
      TestManagementRequestContext requestContext,
      BuildConfiguration build)
    {
      using (new SimpleTimer(requestContext.RequestContext, string.Format("QueryBuildCoverage {0}", (object) build.BuildId)))
      {
        List<BuildCoverage> buildCoverageList = BuildCoverage.Query(requestContext, build.TeamProjectName, build.BuildUri, Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.Modules | Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags.BlockData);
        if (buildCoverageList != null)
        {
          foreach (BuildCoverage buildCoverage in buildCoverageList)
          {
            if (buildCoverage.Configuration.BuildConfigurationId == build.BuildConfigurationId)
              return buildCoverage;
            requestContext.Logger.Info(1015406, string.Format("MSCodeCoverageTransformer:QueryBuildCoverage Buildconfiguration didnt match {0}", (object) build.BuildId));
          }
        }
        return (BuildCoverage) null;
      }
    }

    public bool MergeExisting(
      TestManagementRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.Server.Coverage oldCoverage,
      CoverageInfo coverageInfo,
      Microsoft.TeamFoundation.TestManagement.Server.Coverage newCoverage,
      bool autoIncrementModuleId = true,
      bool mergeWithOldData = true)
    {
      Dictionary<MSCodeCoverageTransformer.ModuleIdentity, ModuleCoverage> dictionary = new Dictionary<MSCodeCoverageTransformer.ModuleIdentity, ModuleCoverage>();
      int val1 = 0;
      bool flag = false;
      if (mergeWithOldData && oldCoverage != null)
      {
        foreach (ModuleCoverage module in oldCoverage.Modules)
        {
          dictionary.Add(new MSCodeCoverageTransformer.ModuleIdentity(module), module);
          val1 = Math.Max(val1, module.ModuleId);
        }
        foreach (ICoverageModule module in coverageInfo.Modules)
        {
          ModuleCoverage moduleCoverage;
          if (dictionary.TryGetValue(new MSCodeCoverageTransformer.ModuleIdentity(module), out moduleCoverage))
          {
            byte[] numArray = Utility.DecompressCoverageBuffer(moduleCoverage.BlockData, (uint) moduleCoverage.BlockCount);
            if (numArray == null)
            {
              string format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Corrupted BlockData for module {0}, Block Count {1}, Buffer Length: {2}", (object) moduleCoverage.Name, (object) moduleCoverage.BlockCount, (object) moduleCoverage.BlockData.Length);
              requestContext.Logger.Error(1015403, format);
              flag = true;
            }
            else
              module.MergeCoverageBuffer(numArray);
          }
        }
        foreach (ModuleCoverage module in oldCoverage.Modules)
          module.BlockData = (byte[]) null;
      }
      using (new SimpleTimer(requestContext.RequestContext, "ConvertCoverageInfo"))
        this.ConvertCoverageInfo(coverageInfo, newCoverage, this.IsReportingConfigured(requestContext), requestContext);
      foreach (ModuleCoverage module in newCoverage.Modules)
      {
        ModuleCoverage moduleCoverage;
        if (dictionary.TryGetValue(new MSCodeCoverageTransformer.ModuleIdentity(module), out moduleCoverage))
        {
          module.ModuleId = moduleCoverage.ModuleId;
        }
        else
        {
          if (autoIncrementModuleId)
            ++val1;
          else
            val1 = module.Name.GetStableHashCode();
          module.ModuleId = val1;
        }
      }
      return !flag;
    }

    public void ConvertCoverageInfo(
      CoverageInfo analyzedInfo,
      Microsoft.TeamFoundation.TestManagement.Server.Coverage coverage,
      bool includeFunctions,
      TestManagementRequestContext context)
    {
      HashSet<uint> uintSet = new HashSet<uint>();
      foreach (ICoverageModule module in analyzedInfo.Modules)
      {
        ModuleCoverage moduleCoverage = new ModuleCoverage();
        moduleCoverage.Name = module.Name;
        moduleCoverage.Signature = module.Signature;
        moduleCoverage.SignatureAge = (int) module.SignatureAge;
        moduleCoverage.Statistics = new CoverageStatistics();
        byte[] coverageBuffer = module.GetCoverageBuffer((IEnumerable<Guid>) new List<Guid>());
        uintSet.Clear();
        List<FunctionCoverage> functions = moduleCoverage.Functions;
        try
        {
          using (ISymbolReader reader = module.Symbols.CreateReader())
          {
            uint num;
            string str1;
            string str2;
            string str3;
            string str4;
            IList<MultiBlockLineData> lines;
            while (reader.GetNextMethod(ref num, ref str1, ref str2, ref str3, ref str4, ref lines))
            {
              BlockCoverageStatistics statistics = this.GetStatistics(coverageBuffer, lines);
              if (includeFunctions)
              {
                FunctionCoverage functionCoverage = new FunctionCoverage()
                {
                  Class = str3,
                  Name = str1,
                  Namespace = str4,
                  SourceFile = lines.Count > 0 ? lines[0].SourceFile.Path : "",
                  Statistics = new CoverageStatistics()
                };
                functionCoverage.Statistics.BlocksCovered = (int) ((BlockCoverageStatistics) ref statistics).BlocksCovered;
                functionCoverage.Statistics.BlocksNotCovered = (int) ((BlockCoverageStatistics) ref statistics).BlocksNotCovered;
                functionCoverage.Statistics.LinesCovered = (int) ((BlockCoverageStatistics) ref statistics).LinesCovered;
                functionCoverage.Statistics.LinesNotCovered = (int) ((BlockCoverageStatistics) ref statistics).LinesNotCovered;
                functionCoverage.Statistics.LinesPartiallyCovered = (int) ((BlockCoverageStatistics) ref statistics).LinesPartiallyCovered;
                functions.Add(functionCoverage);
              }
              if (lines.Count > 0 && !lines[0].BlockIndexes.IsNullOrEmpty<uint>() && uintSet.Add(lines[0].BlockIndexes.First<uint>()))
              {
                moduleCoverage.Statistics.BlocksCovered += (int) ((BlockCoverageStatistics) ref statistics).BlocksCovered;
                moduleCoverage.Statistics.BlocksNotCovered += (int) ((BlockCoverageStatistics) ref statistics).BlocksNotCovered;
                moduleCoverage.Statistics.LinesCovered += (int) ((BlockCoverageStatistics) ref statistics).LinesCovered;
                moduleCoverage.Statistics.LinesNotCovered += (int) ((BlockCoverageStatistics) ref statistics).LinesNotCovered;
                moduleCoverage.Statistics.LinesPartiallyCovered += (int) ((BlockCoverageStatistics) ref statistics).LinesPartiallyCovered;
              }
            }
          }
        }
        catch (SymbolsNotFoundException ex)
        {
          context.Logger.Error(1015404, "There are no symbols available for the module. {0}", (object) ((Exception) ex).Message);
        }
        byte[] destinationArray = new byte[(int) module.BlockCount];
        Array.Copy((Array) coverageBuffer, (Array) destinationArray, (long) module.BlockCount);
        moduleCoverage.BlockCount = (int) module.BlockCount;
        Utility.CompressCoverageBuffer(ref destinationArray);
        moduleCoverage.BlockData = destinationArray;
        coverage.Modules.Add(moduleCoverage);
      }
    }

    public BlockCoverageStatistics GetStatistics(
      byte[] coverageBuffer,
      IList<MultiBlockLineData> lines)
    {
      (uint blocksCovered, uint blocksNotCovered) = this.GetBlocksStatistics(coverageBuffer, (IEnumerable<MultiBlockLineData>) lines);
      return this.GetLinesStatistics((IEnumerable<CoverageLineData>) lines, blocksCovered, blocksNotCovered);
    }

    internal (uint blocksCovered, uint blocksNotCovered) GetBlocksStatistics(
      byte[] coverageBuffer,
      IEnumerable<MultiBlockLineData> lines)
    {
      uint num1 = 0;
      uint num2 = 0;
      HashSet<uint> uintSet = new HashSet<uint>();
      int length = coverageBuffer.Length;
      foreach (MultiBlockLineData line in lines)
      {
        if (!line.BlockIndexes.IsNullOrEmpty<uint>())
        {
          ((CoverageLineData) line).CoverageStatus = coverageBuffer[(int) line.BlockIndexes.First<uint>()] == (byte) 0 ? (CoverageStatus) 2 : (CoverageStatus) 0;
          foreach (uint blockIndex in line.BlockIndexes)
          {
            CoverageStatus coverageStatus = coverageBuffer[(int) blockIndex] == (byte) 0 ? (CoverageStatus) 2 : (CoverageStatus) 0;
            if (((CoverageLineData) line).CoverageStatus != coverageStatus)
              ((CoverageLineData) line).CoverageStatus = (CoverageStatus) 1;
            if (!uintSet.Contains(blockIndex))
            {
              uintSet.Add(blockIndex);
              if ((long) blockIndex >= (long) length)
                throw new InvalidBufferCountException("InvalidBlockIndexExceptionMessage");
              if (coverageStatus == 2)
                ++num2;
              else
                ++num1;
            }
          }
        }
      }
      return (num1, num2);
    }

    internal BlockCoverageStatistics GetLinesStatistics(
      IEnumerable<CoverageLineData> lines,
      uint blocksCovered,
      uint blocksNotCovered)
    {
      BlockCoverageStatistics linesStatistics;
      // ISSUE: explicit constructor call
      ((BlockCoverageStatistics) ref linesStatistics).\u002Ector(blocksCovered, blocksNotCovered, 0U, 0U, 0U);
      Dictionary<uint, CoverageStatus> dictionary = new Dictionary<uint, CoverageStatus>();
      foreach (CoverageLineData line in lines)
      {
        for (uint startLine = line.StartLine; startLine <= line.EndLine; ++startLine)
        {
          CoverageStatus coverageStatus;
          dictionary[startLine] = dictionary.TryGetValue(startLine, out coverageStatus) ? (coverageStatus == line.CoverageStatus ? coverageStatus : (CoverageStatus) (object) 1) : line.CoverageStatus;
        }
      }
      foreach (int num in dictionary.Values)
      {
        switch (num)
        {
          case 0:
            ref BlockCoverageStatistics local1 = ref linesStatistics;
            ((BlockCoverageStatistics) ref local1).LinesCovered = ((BlockCoverageStatistics) ref local1).LinesCovered + 1U;
            continue;
          case 1:
            ref BlockCoverageStatistics local2 = ref linesStatistics;
            ((BlockCoverageStatistics) ref local2).LinesPartiallyCovered = ((BlockCoverageStatistics) ref local2).LinesPartiallyCovered + 1U;
            continue;
          case 2:
            ref BlockCoverageStatistics local3 = ref linesStatistics;
            ((BlockCoverageStatistics) ref local3).LinesNotCovered = ((BlockCoverageStatistics) ref local3).LinesNotCovered + 1U;
            continue;
          default:
            throw new InvalidOperationException();
        }
      }
      return linesStatistics;
    }

    public IList<CodeCoverageStatistics> GetCoverageStatistics(
      TestManagementRequestContext context,
      BuildConfiguration build)
    {
      BuildCoverage buildCoverage = this.QueryBuildCoverage(context, build);
      int total1 = 0;
      int covered1 = 0;
      int total2 = 0;
      int covered2 = 0;
      IList<CodeCoverageStatistics> coverageStatistics1 = (IList<CodeCoverageStatistics>) new List<CodeCoverageStatistics>();
      if (buildCoverage != null)
      {
        foreach (ModuleCoverage module in buildCoverage.Modules)
        {
          CoverageStatistics statistics = module.Statistics;
          covered1 += statistics.LinesCovered;
          total1 += statistics.LinesCovered + statistics.LinesNotCovered + statistics.LinesPartiallyCovered;
          covered2 += statistics.BlocksCovered;
          total2 += statistics.BlocksCovered + statistics.BlocksNotCovered;
        }
        CodeCoverageStatistics coverageStatistics2 = this.GetCodeCoverageStatistics(covered2, total2, "Blocks", 1);
        CodeCoverageStatistics coverageStatistics3 = this.GetCodeCoverageStatistics(covered1, total1, "Lines", 2);
        coverageStatistics1.Add(coverageStatistics2);
        coverageStatistics1.Add(coverageStatistics3);
      }
      return coverageStatistics1;
    }

    private bool IsReportingConfigured(TestManagementRequestContext context)
    {
      if (context.RequestContext.ServiceHost.OrganizationServiceHost == null)
        return false;
      using (IVssRequestContext servicingContext = context.RequestContext.ServiceHost.OrganizationServiceHost.CreateServicingContext())
      {
        CachedRegistryService service = servicingContext.GetService<CachedRegistryService>();
        if (service != null)
          return service.GetValue<bool>(context.RequestContext, (RegistryQuery) "/Configuration/Application/Reporting/ReportingConfigured", false);
      }
      return false;
    }

    private CodeCoverageStatistics GetCodeCoverageStatistics(
      int covered,
      int total,
      string label,
      int position)
    {
      return new CodeCoverageStatistics()
      {
        Label = label,
        Position = position,
        Covered = covered,
        Total = total
      };
    }

    private struct ModuleIdentity : IEquatable<MSCodeCoverageTransformer.ModuleIdentity>
    {
      private Guid Signature;
      private int SignatureAge;
      private int BlockCount;
      private string Name;

      internal ModuleIdentity(ICoverageModule coverage)
      {
        this.Signature = coverage.Signature;
        this.SignatureAge = (int) coverage.SignatureAge;
        this.BlockCount = (int) coverage.BlockCount;
        this.Name = coverage.Name;
      }

      internal ModuleIdentity(ModuleCoverage coverage)
      {
        this.Signature = coverage.Signature;
        this.SignatureAge = coverage.SignatureAge;
        this.BlockCount = coverage.BlockCount;
        this.Name = coverage.Name;
      }

      public bool Equals(MSCodeCoverageTransformer.ModuleIdentity other)
      {
        if (!(this.Signature == other.Signature) || this.SignatureAge != other.SignatureAge || this.BlockCount != other.BlockCount)
          return false;
        return this.Signature != Guid.Empty || StringComparer.OrdinalIgnoreCase.Equals(this.Name, other.Name);
      }

      public override int GetHashCode()
      {
        int hashCode = this.Signature.GetHashCode() ^ this.SignatureAge << 7 ^ this.BlockCount << 13;
        if (this.Signature == Guid.Empty)
          hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name);
        return hashCode;
      }
    }
  }
}
