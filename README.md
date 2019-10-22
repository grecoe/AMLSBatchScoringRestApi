# AMLSBatchScoringRestApi
<sup>Daniel Grecoe a Microsoft Employee</sup>


Example on how to use the Azure Machine Learning Service REST API for Batch Scoring Pipelines.

Requirements:
1. An Azure Machine Learning Service with a workspace containing one pipeline running as a batch job. 
2. An Azure Service Principal with Contributor rights to the subscription containing the Azure Machine Learning Service. 


Open the project and read the Program.cs for futher instruction. 

## Purpose
An example on how to creat an AAD token and call the AMLS Batch Service REST API to:
1. Start a run of the pipleine. 
2. Retrieve run information for one or many runs. 