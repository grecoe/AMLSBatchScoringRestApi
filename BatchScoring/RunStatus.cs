using System;

/* ACTUAL RETURN FROM REST
{
    "Description": "dangbatchtest_sched",
    "Status": {
      "StatusCode": 1,
      "StatusDetail": null,
      "CreationTime": "2019-10-22T15:29:26.2249236+00:00",
      "EndTime": null
    },
    "GraphId": "cebd3437-d350-4bd9-a981-a1dd82806f08",
    "IsSubmitted": true,
    "HasErrors": false,
    "HasWarnings": false,
    "UploadState": 1,
    "ParameterAssignments": {},
    "DataSetDefinitionValueAssignment": null,
    "RunHistoryExperimentName": "exp_191018132824",
    "PipelineId": "b4895c69-cf70-4a57-b486-c78e1a9e5436",
    "RunSource": "Unavailable",
    "RunType": 2,
    "TotalRunSteps": 15,
    "ScheduleId": "c5cf7f9a-5e67-411c-b44e-082f9472abe2",
    "tags": {},
    "Properties": {},
    "CreatedBy": null,
    "EntityStatus": 0,
    "Id": "36e23636-b719-479f-9b8c-9348b8e50316",
    "Etag": "\"ca00aa19-0000-0100-0000-5daf20580000\"",
    "CreatedDate": "1970-01-01T00:00:00",
    "LastModifiedDate": "1970-01-01T00:00:00"
  }
 */


namespace BatchScoring
{
    /// <summary>
    /// Sub module in the return status JSON
    /// </summary>
    class RunStatus
    {
        public int StatusCode { get; set; }
        public String StatusDetail { get; set; }
        public String CreationTime { get; set; }
        public String EndTime { get; set; }
    }

    /// <summary>
    /// JSON return for a run status. Getting all status is a list of these, getting 
    /// a single status is a single one of these. 
    /// </summary>
    class RunResult
    {
        public RunStatus Status { get; set; }
        public bool IsSubmitted { get; set; }
        public bool HasErrors { get; set; }
        public bool HasWarnings { get; set; }
        public int UploadState { get; set; }
        public string RunHistoryExperimentName { get; set;}
        public string PipelineId { get; set;}
        public string RunSource { get; set;}
        public string ScheduleId { get; set;}
        public int RunType { get; set; }
        public int TotalRunSteps { get; set; }
        public string Id { get; set; }

        public RunResult()
        {
            this.Status = new RunStatus();
        }

        public bool IsComplete()
        {
            return !String.IsNullOrEmpty(this.Status.CreationTime) && !String.IsNullOrEmpty(this.Status.EndTime);
        }

        public bool IsSuccess()
        {
            return this.IsComplete() && !this.HasErrors && !this.HasWarnings;
        }

        public double GetElapsedTimeMinutes()
        {
            double returnValue = 0.0;
            if( this.IsComplete())
            {
                DateTime start = DateTime.Parse(this.Status.CreationTime);
                DateTime end = DateTime.Parse(this.Status.EndTime);

                returnValue = (end - start).TotalMinutes;
            }

            return returnValue;
        }
    }
}
