using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DeleteProductSQS
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(ILambdaContext context)
        {
            string ProductId = string.Empty;
            string SQSUrl = "https://sqs.us-east-1.amazonaws.com/446384173180/ProductQueue";
            try
            {
                var sqsClient = new AmazonSQSClient();
                //Thread.Sleep(5000);
                var qmsg = await GetMessage(sqsClient, SQSUrl);
                //Thread.Sleep(5000);
                var qmsgs = qmsg.Messages;
                LambdaLogger.Log("Queue Msgs" + qmsgs?.Count);
                if (qmsgs?.Count > 0)
                {
                    var data = JsonConvert.DeserializeObject(qmsgs[0].Body);
                    ProductId = data?.ToString();
                    LambdaLogger.Log("SQS ProductId:" + ProductId);
                }
                if(!string.IsNullOrEmpty(ProductId))
                {
                    await StartExecutionAsync(ProductId);
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Exception" + ex.Message);
                throw ex;
            }
            
            return ProductId;
        }

        private static async Task<ReceiveMessageResponse> GetMessage(
            IAmazonSQS sqsClient, string qUrl, int waitTime = 0)
        {
            LambdaLogger.Log("GetMessage Called");
            return await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = qUrl,
                //MaxNumberOfMessages = MaxMessages,
                WaitTimeSeconds = waitTime
                // (Could also request attributes, set visibility timeout, etc.)
            });
        }

        private static async Task<StartExecutionResponse> StartExecutionAsync(string input)
        {
            using (var amazonStepFunctionsClient = new AmazonStepFunctionsClient())
            {
                var jsonData1 = JsonConvert.SerializeObject(input);
                var startExecutionRequest = new StartExecutionRequest
                {
                    Input = jsonData1,
                    //Name = "DeleteProduct",
                    StateMachineArn = "arn:aws:states:us-east-1:446384173180:stateMachine:DeleteProduct"
                };
                var taskStartExecutionResponse
                    = await amazonStepFunctionsClient.StartExecutionAsync(startExecutionRequest);
                return taskStartExecutionResponse;
            }
        }
    }
}
