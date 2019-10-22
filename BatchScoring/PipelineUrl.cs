using System;
using System.Net.Http;

/// <summary>
/// The URL provided by an AML Batch Scoring pipeline (that you recieve directly from the portal) is in the form:
/// 
/// https://[URL]/api/v1.0/subscriptions/[SUBID]/resourceGroups/[RG]/providers/Microsoft.MachineLearningServices/workspaces/[WS]/PipelineRuns/PipelineSubmit/[PIPELINE_ID]
/// 
/// This is the format used with a POST and a body of :
///     {"ExperimentName" : "name of experiment"}
///    
/// The different forms of URI needed to get job statuses use the base of the URI:
///     https://[URL]/api/v1.0/subscriptions/[SUBID]/resourceGroups/[RG]/providers/Microsoft.MachineLearningServices/workspaces/[WS]/PipelineRuns
///     
///     i.e. stripping off the /PipelineSubmit/[PIPELINE_ID] ending.
///     
/// To get job status you take the base then :
/// 
///     For all run statuses append : "/Pipeline/[PIPELINE_ID]"
///     For a single run append : "/[RUN_ID]"
/// </summary>
namespace BatchScoring
{
    /// <summary>
    /// Class used externally to capture the HTTP Method for the call the user wants to make
    /// along with the appropriate URL
    /// </summary>
    public class PiplelineRequest
    {
        public HttpMethod Method { get; set; }
        public String Url { get; set; }
    }

    /// <summary>
    /// Internal class to help strip down the original URI
    /// </summary>
    class ParseResult
    {
        public String OriginalInput { get; set; }
        public String DesiredPart { get; set; }
        public String RemainingInput { get; set; }
    }

    /// <summary>
    /// Class used to take in the default URL from the pipeline, which is the trigger URL, and parses 
    /// it down to the base URL. 
    /// 
    /// Then, depending on the call the user wants to make, will return an appropriate PipelineRequest object
    /// to pass along to the HTTP Client. 
    /// </summary>
    class BatchPipelineUrl
    {
        /// <summary>
        /// The URL captured in the portal.
        /// </summary>
        private String ProvidedUrl { get; set; }
        /// <summary>
        /// The base URI for other calls (JobStatus) by stripping /PipelineSubmit/[PIPELINE_ID]
        /// from the providedUrl
        /// </summary>
        private String BaseUrl { get; set; }
        /// <summary>
        /// The pipeline ID stripped off of the ProvidedUrl
        /// </summary>
        private String PipelineId { get; set; }

        public BatchPipelineUrl(string providedUrl)
        {
            this.ProvidedUrl = providedUrl;

            ParseResult pr = this.GetLastPathElement(this.ProvidedUrl);
            this.PipelineId = pr.DesiredPart;

            pr = this.GetLastPathElement(pr.RemainingInput);
            this.BaseUrl = pr.RemainingInput;
        }

        /// <summary>
        /// Get the appropriate HTTP Method and URL to trigger a job.
        /// </summary>
        public PiplelineRequest GetJobTrigger()
        {
            PiplelineRequest returnRequest = new PiplelineRequest()
            {
                Method = System.Net.Http.HttpMethod.Post,
                Url = this.ProvidedUrl
            };

            return returnRequest;
        }

        /// <summary>
        /// Get the appropriate HTTP Method and URL to get all job status'.
        /// </summary>
        public PiplelineRequest GetRunStatus()
        {
            PiplelineRequest returnRequest = new PiplelineRequest()
            {
                Method = System.Net.Http.HttpMethod.Get,
                Url = this.BaseUrl + "/Pipeline/" + this.PipelineId
            };

            return returnRequest;
        }

        /// <summary>
        /// Get the appropriate HTTP Method and URL to get a single job status.
        /// </summary>
        public PiplelineRequest GetRunStatus(string jobRun)
        {
            PiplelineRequest returnRequest = new PiplelineRequest()
            {
                Method = System.Net.Http.HttpMethod.Get,
                Url = this.BaseUrl + "/" + jobRun
            };

            return returnRequest;
        }

        #region Private Methods
        /// <summary>
        /// Basic helper method to strip down a URL 
        /// </summary>
        /// <param name="path">URL path to strip off the last element</param>
        /// <returns>ParseResult which has the original, stripped component and remainder. </returns>
        private ParseResult GetLastPathElement(string path)
        {
            ParseResult pr = new ParseResult();
            pr.OriginalInput = path;

            int pathIndex = pr.OriginalInput.LastIndexOf('/');
            pr.DesiredPart = pr.OriginalInput.Substring(pathIndex + 1);
            pr.RemainingInput = pr.OriginalInput.Substring(0, pathIndex);

            return pr;
        }
        #endregion
    }
}
