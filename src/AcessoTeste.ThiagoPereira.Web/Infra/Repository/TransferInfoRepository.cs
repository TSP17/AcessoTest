using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Infra.Repository
{
    public class TransferInfoRepository : ITransferInfoRepository
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly IConfiguration _configuration;

        public TransferInfoRepository(
            IAmazonDynamoDB amazonDynamoDb,
            IConfiguration configuration)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _configuration = configuration;
        }

        public async Task<TransferInfo> GetById(string Id)
        {
            var request = new GetItemRequest
            {
                TableName = _configuration.GetValue<string>("DynamoDb:TableNameTransactions"),
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = Id} } }
            };

            var result = await _amazonDynamoDb.GetItemAsync(request);

            var transferInfoResult = new TransferInfo
            {
                Id = result.Item["Id"].S,
                AccountDestination = result.Item["AccountDestination"].S,
                AccountOrigin = result.Item["AccountOrigin"].S,
                Value = result.Item["Value"].S,
                DateCreated = result.Item["DateCreated"].S,
                DateUpdatedStatus = result.Item["DateUpdatedStatus"].S,
                Status = result.Item["Status"].S
            };

            return transferInfoResult;
        }

        public async Task Put(TransferInfo transferInfo)
        {
            var request = new PutItemRequest
            {
                TableName = _configuration.GetValue<string>("DynamoDb:TableNameTransactions"),
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = transferInfo.Id.ToString() }},
                    { "AccountOrigin", new AttributeValue { S = transferInfo.AccountOrigin }},
                    { "AccountDestination", new AttributeValue { S = transferInfo.AccountDestination }},
                    { "Value", new AttributeValue { S = transferInfo.Value }},
                    { "Status", new AttributeValue { S = transferInfo.Status }},
                    { "DateCreated", new AttributeValue { S = transferInfo.DateCreated }},
                    { "DateUpdatedStatus", new AttributeValue { S = transferInfo.DateUpdatedStatus }}
                }
            };

            await _amazonDynamoDb.PutItemAsync(request);
        }

        public async Task CreateTable()
        {
            var TableName = _configuration.GetValue<string>("DynamoDb:TableNameTransactions");

            var request = new ListTablesRequest
            {
                Limit = 10
            };

            var response = await _amazonDynamoDb.ListTablesAsync(request);

            var results = response.TableNames;

            if (!results.Contains(TableName))
            {
                var createRequest = new CreateTableRequest
                {
                    TableName = TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"  //Partition key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                };

                await _amazonDynamoDb.CreateTableAsync(createRequest);
            }
        }
    }
}
