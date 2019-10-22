using System;
using System.Collections.Generic;

// NUGET REQUIREMENTS
// Microsoft.IdentityModel.Clients.ActiveDirectory
// Newtonsoft.JSON
namespace BatchScoring
{
    class Program
    {
        private const string Tenant = "YOUR_SERVICE_PRINCIPAL_TENANT";
        private const string ClientId = "YOUR_CLIENT_ID";
        private const string ClientSecret = "YOUR_CLIENT_SECRET";

       // URL captured right from portal. 
        private const string PipelineUrl = "YOUR_AMLS_PIPELINE_URI";

        static void Main(string[] args)
        {
            // Create a Service Pricipal Authentication object using the Tenant, ClientId and ClientSecret.
            // To learn how to create an Azure Service Principal see here:
            //  https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal
            ServicePrincipalAuthentication spa = new ServicePrincipalAuthentication(Tenant, ClientId, ClientSecret);
            // Create the batch pipeline URL helper. REad the top of PipelineUrl file to understand what it's doing. 
            BatchPipelineUrl pipelineUrl = new BatchPipelineUrl(PipelineUrl);

            // Retreive the AAD Token for the Service Principal
            Console.WriteLine("Collecting AAD token......");
            String aadToken = spa.GetToken().Result;


            ///
            /// Get the status' of all the pipeline runs
            ///
            Console.WriteLine("Collecting all Pipeline Runs......");
            PiplelineRequest prAll = pipelineUrl.GetRunStatus();
            List<RunResult> rr = RestRequest.GetRunResults<List<RunResult>>(prAll, aadToken);
            Console.WriteLine("***** MULTIPLE RUNS ************");
            foreach(RunResult r in rr)
            {
                ReportOnRun(r);
            }


            ///
            /// Get the status of a single pipeline run
            ///
            if (rr.Count > 0)
            {
                Console.WriteLine("Collecting one Pipeline Run......");

                string runId = rr[rr.Count - 1].Id;
                PiplelineRequest prOne = pipelineUrl.GetRunStatus(runId);
                RunResult sr = RestRequest.GetRunResults<RunResult>(prOne, aadToken);

                Console.WriteLine("***** SINGLE RUN ************");
                ReportOnRun(sr);
            }

            Console.WriteLine("COMPLETE - ENTER TO QUIT");
            Console.ReadLine();
        }

        private static void ReportOnRun(RunResult rr)
        {
            Console.WriteLine(String.Format("Reporting on run : {0}", rr.Id));
            if( !rr.IsComplete() )
            {
                Console.WriteLine("\t RUN NOT COMPLETE");
            }
            else if(rr.IsSuccess())
            {
                Console.WriteLine(String.Format("\t RUN SUCCESFUL TIME - {0}", rr.GetElapsedTimeMinutes()));
            }
            else
            {
                Console.WriteLine("\t RUN NOT FAILED");
            }


        }
    }
}
