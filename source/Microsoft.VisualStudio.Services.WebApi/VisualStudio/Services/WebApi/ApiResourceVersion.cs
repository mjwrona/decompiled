// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ApiResourceVersion
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public class ApiResourceVersion
  {
    private const string c_PreviewStageName = "preview";

    public ApiResourceVersion(double apiVersion, int resourceVersion = 0)
      : this(new Version(apiVersion.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)), resourceVersion)
    {
    }

    public ApiResourceVersion()
      : this(1.0)
    {
    }

    public ApiResourceVersion(Version apiVersion, int resourceVersion = 0)
    {
      ArgumentUtility.CheckForNull<Version>(apiVersion, nameof (apiVersion));
      this.ApiVersion = apiVersion;
      this.ResourceVersion = resourceVersion;
      if (resourceVersion <= 0)
        return;
      this.IsPreview = true;
    }

    public ApiResourceVersion(string apiResourceVersionString) => this.FromVersionString(apiResourceVersionString);

    public Version ApiVersion { get; private set; }

    [DataMember(Name = "ApiVersion")]
    public string ApiVersionString
    {
      get => this.ApiVersion.ToString(2);
      private set
      {
        if (string.IsNullOrEmpty(value))
          this.ApiVersion = new Version(1, 0);
        else
          this.ApiVersion = new Version(value);
      }
    }

    [DataMember]
    public int ResourceVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsPreview { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.ApiVersion.ToString(2));
      if (this.IsPreview)
      {
        stringBuilder.Append('-');
        stringBuilder.Append("preview");
        if (this.ResourceVersion > 0)
        {
          stringBuilder.Append('.');
          stringBuilder.Append(this.ResourceVersion);
        }
      }
      return stringBuilder.ToString();
    }

    private void FromVersionString(string apiVersionString)
    {
      int num1 = !string.IsNullOrEmpty(apiVersionString) ? apiVersionString.IndexOf('-') : throw new VssInvalidApiResourceVersionException(apiVersionString);
      if (num1 >= 0)
      {
        int num2 = apiVersionString.IndexOf('.', num1);
        string a;
        if (num2 > 0)
        {
          a = apiVersionString.Substring(num1 + 1, num2 - num1 - 1);
          int result;
          if (!int.TryParse(apiVersionString.Substring(num2 + 1), out result))
            throw new VssInvalidApiResourceVersionException(apiVersionString);
          this.ResourceVersion = result;
        }
        else
          a = apiVersionString.Substring(num1 + 1);
        if (!string.Equals(a, "preview", StringComparison.OrdinalIgnoreCase))
          throw new VssInvalidApiResourceVersionException(apiVersionString);
        this.IsPreview = true;
        apiVersionString = apiVersionString.Substring(0, num1);
      }
      apiVersionString = apiVersionString.TrimStart('v');
      double result1;
      if (!double.TryParse(apiVersionString, NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        throw new VssInvalidApiResourceVersionException(apiVersionString);
      this.ApiVersion = new Version(result1.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
