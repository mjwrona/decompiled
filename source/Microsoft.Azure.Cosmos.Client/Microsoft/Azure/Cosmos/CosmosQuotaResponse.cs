// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosQuotaResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosQuotaResponse
  {
    protected IDictionary<string, double> Quotas;
    private readonly string source;
    protected const string FunctionsKey = "functions";
    protected const string StoredProceduresKey = "storedProcedures";
    protected const string TriggersKey = "triggers";
    protected const string DatabasesKey = "databases";
    protected const string DocumentSizeKey = "documentSize";
    protected const string DocumentsSizeKey = "documentsSize";
    protected const string DocumentsCountKey = "documentsCount";
    protected const string ContainerSizeKey = "collectionSize";

    internal CosmosQuotaResponse(string quotaInfo)
    {
      this.source = quotaInfo;
      this.ParseQuotaString(quotaInfo);
    }

    private void ParseQuotaString(string quotaInfo)
    {
      this.Quotas = (IDictionary<string, double>) new Dictionary<string, double>();
      string str1 = quotaInfo;
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
      {
        if (!string.IsNullOrEmpty(str2))
        {
          string[] strArray = str2.Split('=');
          if (strArray.Length == 2)
          {
            string str3 = strArray[0];
            double num = double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
            switch (str3)
            {
              case "collectionSize":
                this.ContainerSize = new double?(num);
                continue;
              case "databases":
                this.Databases = new double?(num);
                continue;
              case "documentSize":
                this.DocumentSize = new double?(num);
                continue;
              case "documentsCount":
                this.DocumentsCount = new double?(num);
                continue;
              case "documentsSize":
                this.DocumentsSize = new double?(num);
                continue;
              case "functions":
                this.UserDefinedFunctions = new double?(num);
                continue;
              case "storedProcedures":
                this.StoredProcedures = new double?(num);
                continue;
              case "triggers":
                this.Triggers = new double?(num);
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    internal double? Databases { get; private set; }

    internal double? UserDefinedFunctions { get; private set; }

    internal double? StoredProcedures { get; private set; }

    internal double? Triggers { get; private set; }

    internal double? DocumentSize { get; private set; }

    internal double? DocumentsSize { get; private set; }

    internal double? DocumentsCount { get; private set; }

    internal double? ContainerSize { get; private set; }

    public override string ToString() => this.source;
  }
}
