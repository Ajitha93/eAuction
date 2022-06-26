using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DeleteProductLambda
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(string ProductId,ILambdaContext context)
        {
            string msg = "Not a valid input";
            //string SQSUrl = "https://sqs.us-east-1.amazonaws.com/446384173180/ProductQueue";
            //string SQSUrl = null;

            try
            {
                //string ProductId = null;
                msg = "Delete Product Lambda Function Started";
                LambdaLogger.Log(msg);
                //await SendEmail(msg);

                // Create the Amazon SQS client
                //var sqsClient = new AmazonSQSClient();
                //var qmsg = await GetMessage(sqsClient, SQSUrl);
                //Thread.Sleep(5000);
                //var qmsgs = qmsg.Messages;
                //if (qmsgs?.Count > 0)
                //{
                //    var data = JsonConvert.DeserializeObject(qmsgs[0].Body);
                //    ProductId = data?.ToString();
                    LambdaLogger.Log("SQS ProductId:" + ProductId);
                    if (!string.IsNullOrEmpty(ProductId))
                    {
                        var dynamoClient = new AmazonDynamoDBClient();
                        msg = await DeleteProduct(ProductId, dynamoClient);
                    }
                //}
                else
                    msg = "No Message found in the queue";

                await SendEmail(msg);
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Exception" + ex.Message);
                throw ex;
            }
            return ReturnMessage(msg);
        }

        //private static async Task<ReceiveMessageResponse> GetMessage(
        //    IAmazonSQS sqsClient, string qUrl, int waitTime = 0)
        //{
        //    LambdaLogger.Log("GetMessage Called");
        //    return await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        //    {
        //        QueueUrl = qUrl,
        //        //MaxNumberOfMessages = MaxMessages,
        //        WaitTimeSeconds = waitTime
        //        // (Could also request attributes, set visibility timeout, etc.)
        //    });
        //}

        public APIGatewayProxyResponse ReturnMessage(string Message)
        {

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = Message,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }

        public async Task<string> DeleteProduct(string ProductId, AmazonDynamoDBClient DynamoClient)
        {

            LambdaLogger.Log("DeleteProductMethod Called");
            string msg = string.Empty;
            var product = await GetProduct(ProductId, DynamoClient);
            DateTime bidendDate = DateTime.MinValue;
            DateTime.TryParse(product.BidEndDate, out bidendDate);
            if (bidendDate == DateTime.MinValue)
            {
                msg = "Product not found for the ProductId " + ProductId;
            }
            else
            {
                if (bidendDate < DateTime.Now)
                    msg = "Can't delete product as the BidEndDate is a past date";
                else
                {
                    var res = await DeleteItem(ProductId,DynamoClient);
                    msg = "Product Deleted Successfully!!";
                }
            }
            LambdaLogger.Log(msg);
            return msg;
        }


        public async Task<Products> GetProduct(string Id, AmazonDynamoDBClient DynamoClient)
        {
            //AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            string tableName = "Products";

            var request = new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() { { "ProductId", new AttributeValue { S = Id } } },
            };

            LambdaLogger.Log("GetAsync called start");
            //Thread.Sleep(5000);

            var response = await DynamoClient.GetItemAsync(request);

            //Thread.Sleep(5000);
            LambdaLogger.Log("GetAsync called end");

            // Check the response.
            var result = response.Item;

            LambdaLogger.Log("Item called");

            Products product = result.ToObject<Products>();

            LambdaLogger.Log("product called BidEndDate" + product.BidEndDate);

            return product;

        }

        public async Task<bool> DeleteItem(string Id, AmazonDynamoDBClient DynamoClient)
        {
            LambdaLogger.Log("DeleteItem Called");
            //AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            string tableName = "Products";

            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() { { "ProductId", new AttributeValue { S = Id } } },
            };

            //Thread.Sleep(5000);

            var response = await DynamoClient.DeleteItemAsync(request);

            //Thread.Sleep(5000);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task SendEmail(string message)
        {
            var client = new AmazonSimpleNotificationServiceClient(region: Amazon.RegionEndpoint.USEast1);

            var request = new PublishRequest
            {
                Message = message,
                TopicArn = "arn:aws:sns:us-east-1:446384173180:DeleteProduct"
            };

            var response =await client.PublishAsync(request);
        }
    }

    public static class ObjectExtensions
    {
        public static T ToObject<T>(this Dictionary<string, AttributeValue> source)
            where T : class, new()
        {
            try
            {
                var someObject = new T();
                var someObjectType = someObject.GetType();

                foreach (var item in source)
                {
                    someObjectType
                             .GetProperty(item.Key)
                             .SetValue(someObject, item.Value.S, null);
                }

                return someObject;
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("ToObject Exception " + ex.Message);
                return null;
            }
        }       
    }

    [DynamoDBTable("Products")]
    public class Products
    {
        [DynamoDBHashKey]
        public string ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ShortDescription { get; set; } = null!;

        public string DetailedDescription { get; set; } = null!;

        public string Category { get; set; } = null!;

        public string StartingPrice { get; set; }

        public string BidEndDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pin { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
