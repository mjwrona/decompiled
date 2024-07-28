// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.DiffOptions
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class DiffOptions
  {
    private int m_codePageOverride;
    private string m_commandLineOptions;
    private int m_contextLines;
    private CultureInfo m_cultureInfo;
    private DiffOptionFlags m_flags;
    private DiffOutputType m_outputType;
    private Encoding m_sourceEncoding;
    private Encoding m_targetEncoding;
    private string m_sourceLabel;
    private string m_targetLabel;
    private bool m_recursive;
    private StreamWriter m_streamWriter;
    private bool m_useThirdPartyTool;
    private int m_vssWidth;
    private VssOutputType m_vssSubFormat;

    public DiffOptions()
    {
      this.ContextLines = 3;
      this.CultureInfo = CultureInfo.CurrentCulture;
      this.Flags = DiffOptionFlags.IgnoreEndOfLineDifference | DiffOptionFlags.EnablePreambleHandling;
      this.OutputType = DiffOutputType.Unified;
      this.VssFormatType = VssOutputType.Vss;
      this.VssOutputWidth = -1;
    }

    public int CodePageOverride
    {
      get => this.m_codePageOverride;
      set => this.m_codePageOverride = value;
    }

    public int GetCodePage(int fallbackCodePage) => this.m_codePageOverride != 0 ? this.m_codePageOverride : fallbackCodePage;

    public string CommandLineOptions
    {
      get => this.m_commandLineOptions;
      set => this.m_commandLineOptions = value;
    }

    public int ContextLines
    {
      get => this.m_contextLines;
      set => this.m_contextLines = value;
    }

    public CultureInfo CultureInfo
    {
      get => this.m_cultureInfo;
      set => this.m_cultureInfo = value;
    }

    public DiffOptionFlags Flags
    {
      get => this.m_flags;
      set => this.m_flags = value;
    }

    public DiffOutputType OutputType
    {
      get => this.m_outputType;
      set => this.m_outputType = value;
    }

    public Encoding SourceEncoding
    {
      get => this.m_sourceEncoding;
      set => this.m_sourceEncoding = value;
    }

    public Encoding TargetEncoding
    {
      get => this.m_targetEncoding;
      set => this.m_targetEncoding = value;
    }

    public string SourceLabel
    {
      get => this.m_sourceLabel;
      set => this.m_sourceLabel = value;
    }

    public string TargetLabel
    {
      get => this.m_targetLabel;
      set => this.m_targetLabel = value;
    }

    public bool Recursive
    {
      get => this.m_recursive;
      set => this.m_recursive = value;
    }

    public StreamWriter StreamWriter
    {
      get => this.m_streamWriter;
      set => this.m_streamWriter = value;
    }

    public void SetTargetEncodingToConsole() => this.m_targetEncoding = Console.OutputEncoding;

    public bool UseThirdPartyTool
    {
      get => this.m_useThirdPartyTool;
      set => this.m_useThirdPartyTool = value;
    }

    public int VssOutputWidth
    {
      get => this.m_vssWidth;
      set => this.m_vssWidth = value;
    }

    public VssOutputType VssFormatType
    {
      get => this.m_vssSubFormat;
      set => this.m_vssSubFormat = value;
    }
  }
}
