// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraData
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public class JiraData
  {
    public class V2
    {
      public class IssuesRequestData
      {
        [JsonProperty("jql")]
        public string Jql { get; set; }

        [JsonProperty("validateQuery")]
        public string ValidateQuery { get; set; }
      }

      public class IssuesResponseData
      {
        [JsonProperty("expand")]
        public string Expand { get; set; }

        [JsonProperty("startAt")]
        public int StartAt { get; set; }

        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("issues")]
        public JiraData.V2.Issue[] Issues { get; set; }
      }

      public class Issue
      {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("fields")]
        public JiraData.V2.Field Fields { get; set; }
      }

      public class Field
      {
        [JsonProperty("issuetype")]
        public JiraData.V2.Issuetype Issuetype { get; set; }

        [JsonProperty("status")]
        public JiraData.V2.Status Status { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("project")]
        public JiraData.V2.Project Project { get; set; }
      }

      public class Issuetype
      {
        [JsonProperty("name")]
        public string Name { get; set; }
      }

      public class Status
      {
        [JsonProperty("statusCategory")]
        public JiraData.V2.StatusCategory StatusCategory { get; set; }
      }

      public class StatusCategory
      {
        [JsonProperty("name")]
        public string Name { get; set; }
      }

      public class Project
      {
        [JsonProperty("key")]
        public string Key { get; set; }
      }

      public class ErrorResponse
      {
        [JsonProperty("errorMessages")]
        public string[] ErrorMessages { get; set; }

        [JsonProperty("warningMessages")]
        public string[] WarningMessages { get; set; }
      }
    }

    public class V0_1
    {
      public class DeploymentRequestData
      {
        [JsonProperty("properties")]
        public JiraData.V0_1.PropertyData Properties { get; set; }

        [JsonProperty("deployments")]
        public JiraData.V0_1.DeploymentData[] Deployments { get; set; }
      }

      public class PropertyData
      {
        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("pipelineId")]
        public string PipelineId { get; set; }
      }

      public class DeploymentData
      {
        [JsonProperty("deploymentSequenceNumber")]
        public int DeploymentSequenceNumber { get; set; }

        [JsonProperty("updateSequenceNumber")]
        public int UpdateSequenceNumber { get; set; }

        [JsonProperty("issueKeys")]
        public string[] IssueKeys { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("pipeline")]
        public JiraData.V0_1.Pipeline Pipeline { get; set; }

        [JsonProperty("environment")]
        public JiraData.V0_1.Environment Environment { get; set; }

        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }
      }

      public class Pipeline
      {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
      }

      public class Environment
      {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
      }

      public class DeploymentResponseData
      {
        [JsonProperty("acceptedDeployments")]
        public JiraData.V0_1.Deployment[] AcceptedDeployments { get; set; }

        [JsonProperty("rejectedDeployments")]
        public JiraData.V0_1.RejectedDeployment[] RejectedDeployments { get; set; }

        [JsonProperty("unknownIssueKeys")]
        public string[] UnknownIssueKeys { get; set; }
      }

      public class Deployment
      {
        [JsonProperty("pipelineId")]
        public string PipelineId { get; set; }

        [JsonProperty("environmentId")]
        public string EnvironmentId { get; set; }

        [JsonProperty("deploymentSequenceNumber")]
        public int DeploymentSequenceNumber { get; set; }
      }

      public class RejectedDeployment
      {
        [JsonProperty("key")]
        public JiraData.V0_1.Deployment Key { get; set; }

        [JsonProperty("errors")]
        public JiraData.V0_1.Error[] Errors { get; set; }
      }

      public class Error
      {
        [JsonProperty("message")]
        public string Message { get; set; }
      }
    }
  }
}
