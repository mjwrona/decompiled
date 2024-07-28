// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.JavascriptNotifyInterop
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ComVisible(true)]
  public sealed class JavascriptNotifyInterop
  {
    internal event EventHandler<ErrorData> ProcessingFailed;

    internal event EventHandler<TokenData> TokenDataReceived;

    internal event EventHandler WindowResizing;

    public bool notifyError(string value = null)
    {
      if (value == null)
        return false;
      if (this.ProcessingFailed != null)
      {
        ErrorData e;
        try
        {
          e = JavascriptNotifyInterop.DeserializeError(value);
        }
        catch (Exception ex)
        {
          EventHandler<ErrorData> processingFailed = this.ProcessingFailed;
          if (processingFailed != null)
            processingFailed((object) this, new ErrorData()
            {
              Message = ClientResources.ErrorDeserializeFailed(),
              Details = ex.ToString(),
              Content = value
            });
          return true;
        }
        this.ProcessingFailed((object) this, e);
      }
      return true;
    }

    public bool notifyScriptError(
      string url = null,
      string message = null,
      string details = null,
      string statusCodeValue = null,
      string content = null)
    {
      if (url == null)
        return false;
      if (this.ProcessingFailed != null)
      {
        ErrorData e = new ErrorData()
        {
          Message = message?.Trim(),
          Details = details?.Trim(),
          Content = content
        };
        Uri result1 = (Uri) null;
        if (!string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out result1))
          e.Uri = result1;
        int result2 = 0;
        if (!string.IsNullOrWhiteSpace(statusCodeValue) && int.TryParse(statusCodeValue, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          e.StatusCode = result2;
        this.ProcessingFailed((object) this, e);
      }
      return true;
    }

    public bool resizeWindow(string value = null)
    {
      if (value == null)
        return false;
      EventHandler windowResizing = this.WindowResizing;
      if (windowResizing != null)
        windowResizing((object) this, (EventArgs) null);
      return true;
    }

    public bool notifyToken(string value = null)
    {
      if (value == null)
        return false;
      if (this.TokenDataReceived != null)
      {
        TokenData e;
        try
        {
          e = JavascriptNotifyInterop.DeserializeToken<TokenData>(value);
        }
        catch (Exception ex)
        {
          EventHandler<ErrorData> processingFailed = this.ProcessingFailed;
          if (processingFailed != null)
            processingFailed((object) this, new ErrorData()
            {
              Message = ClientResources.TokenDeserializeFailed(),
              Details = ex.ToString(),
              Content = value
            });
          return true;
        }
        this.TokenDataReceived((object) this, e);
      }
      return true;
    }

    private static ErrorData DeserializeError(string value) => JsonConvert.DeserializeObject<ErrorData>(Encoding.UTF8.GetString(Convert.FromBase64String(value)));

    private static T DeserializeToken<T>(string value)
    {
      using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(value), XmlDictionaryReaderQuotas.Max))
        return (T) new DataContractJsonSerializer(typeof (T)).ReadObject(jsonReader);
    }
  }
}
