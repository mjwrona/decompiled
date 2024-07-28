// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MethodInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class MethodInformation
  {
    private NameValueCollection m_parameters = new NameValueCollection();
    private string m_webMethodName;
    private MethodType m_methodType;
    private EstimatedMethodCost m_estimatedCost;
    private bool m_isLongRunning;
    private bool m_captureAsyncResourcesUsage;
    private TimeSpan m_timeout;
    private const string c_stringNull = "null";

    public MethodInformation(
      string webMethodName,
      MethodType methodType,
      EstimatedMethodCost estimatedCost)
      : this(webMethodName, methodType, estimatedCost, true)
    {
    }

    public MethodInformation(
      string webMethodName,
      MethodType methodType,
      EstimatedMethodCost estimatedCost,
      TimeSpan timeout)
      : this(webMethodName, methodType, estimatedCost, true, timeout: timeout)
    {
    }

    public MethodInformation(
      string webMethodName,
      MethodType methodType,
      EstimatedMethodCost estimatedCost,
      bool keepsHostAwake,
      bool isLongRunning = false,
      TimeSpan timeout = default (TimeSpan),
      bool captureAsyncResourcesUsage = false)
    {
      this.m_webMethodName = webMethodName;
      this.m_methodType = methodType;
      this.m_estimatedCost = estimatedCost;
      this.m_isLongRunning = isLongRunning;
      this.m_captureAsyncResourcesUsage = captureAsyncResourcesUsage;
      this.m_timeout = timeout;
      this.KeepsHostAwake = keepsHostAwake;
    }

    public void AddParameter(string parameterName, object parameterValue)
    {
      string str = "null";
      if (parameterValue != null)
      {
        try
        {
          str = parameterValue.ToString();
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.Error("MethodInformation.AddParameter: Can't ToString parameterValue.", ex);
          str = ex.Message;
        }
      }
      this.m_parameters[parameterName] = str;
    }

    public void AddArrayParameter(string parameterName, IEnumerable parameterArray)
    {
      int parameterIndex = 0;
      if (parameterArray != null)
      {
        foreach (object parameter in parameterArray)
        {
          if (parameterIndex < 10)
          {
            if (!(parameter is IRecordable recordable))
              this.AddParameter(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]", (object) parameterName, (object) parameterIndex), parameter);
            else
              recordable.RecordInformation(this, parameterIndex);
          }
          ++parameterIndex;
        }
      }
      this.AddParameter(parameterName, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Count = {0}", (object) parameterIndex));
    }

    public void AddArrayParameter<T>(string parameterName, IList<T> parameterArray) => this.AddArrayParameter(parameterName, (IEnumerable) parameterArray);

    public EstimatedMethodCost EstimatedCost => this.m_estimatedCost;

    public bool IsLongRunning => this.m_isLongRunning;

    public TimeSpan Timeout => this.m_timeout;

    public MethodType MethodType => this.m_methodType;

    public string Name => this.m_webMethodName;

    public bool CaptureAsyncResourcesUsage => this.m_captureAsyncResourcesUsage;

    internal bool KeepsHostAwake { get; private set; }

    public NameValueCollection Parameters => this.m_parameters;

    internal bool InProgress { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "MethodInformation[Name={0}, Type={1}, Cost={2}, KeepsHostAwake={3}, LongRunning={4}, Timeout={5}, Parameters=[{6}]]", (object) this.m_webMethodName, (object) this.m_methodType, (object) this.m_estimatedCost, (object) this.KeepsHostAwake, (object) this.IsLongRunning, (object) this.Timeout.TotalSeconds, (object) this.GetParametersAsString());

    private string GetParametersAsString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Parameters.Count > 0)
        stringBuilder.AppendFormat("{0}={1}", (object) this.Parameters.Keys[0], (object) this.Parameters[0]);
      for (int index = 1; index < this.Parameters.Count; ++index)
        stringBuilder.AppendFormat(", {0}={1}", (object) this.Parameters.Keys[index], (object) this.Parameters[index]);
      return stringBuilder.ToString();
    }
  }
}
