﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Cnp.Sdk;
using Moq;
using System.Text.RegularExpressions;


namespace Cnp.Sdk.Test.Unit
{
    public class TestQueryTransactionRequest
    {

        private CnpOnline cnp = new CnpOnline();

        [Fact]
        public void TestSimple()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "12345";
            query.origActionType = actionTypeEnum.D;
            query.origCnpTxnId = 54321;

            string result = query.Serialize();
            Assert.Equal("\r\n<queryTransaction id=\"myId\" reportGroup=\"myReportGroup\">\r\n<origId>12345</origId>\r\n<origActionType>D</origActionType>\r\n<origCnpTxnId>54321</origCnpTxnId>\r\n</queryTransaction>", result);
            
        }

        [Fact]
        public void TestQueryTransactionResponse()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "12345";
            query.origActionType = actionTypeEnum.D;
            query.origCnpTxnId = 54321;

            var mock = new Mock<Communications>();

            mock.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<queryTransaction.*", RegexOptions.Singleline), It.IsAny<Dictionary<String, String>>()))
                .Returns("<cnpOnlineResponse version='10.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><queryTransactionResponse id='FindAuth' reportGroup='Mer5PM1' customerId='1'><response>000</response><responseTime>2015-12-03T10:30:02</responseTime><message>Original transaction found</message><results_max10><authorizationResponse id='1' reportGroup='defaultReportGroup'><cnpTxnId>756027696701750</cnpTxnId><orderId>GenericOrderId</orderId><response>000</response><responseTime>2015-04-14T12:04:59</responseTime><postDate>2015-04-14</postDate><message>Approved</message><authCode>055858</authCode></authorizationResponse><authorizationResponse id='1' reportGroup='defaultReportGroup'><cnpTxnId>756027696701751</cnpTxnId><orderId>GenericOrderId</orderId><response>000</response><responseTime>2015-04-14T12:04:59</responseTime><postDate>2015-04-14</postDate><message>Approved</message><authCode>055858</authCode></authorizationResponse><captureResponse><response>000</response><message>Deposit approved</message></captureResponse></results_max10></queryTransactionResponse></cnpOnlineResponse>");

            Communications mockedCommunication = mock.Object;
            cnp.SetCommunication(mockedCommunication);
            transactionTypeWithReportGroup response = (transactionTypeWithReportGroup)cnp.QueryTransaction(query);
            queryTransactionResponse queryTransactionResponse = (queryTransactionResponse)response;

            Assert.NotNull(queryTransactionResponse);
            Assert.Equal("000", queryTransactionResponse.response);
            Assert.Equal(3, queryTransactionResponse.results_max10.Count);
            Assert.Equal("Original transaction found", queryTransactionResponse.message);
            Assert.Equal("000", ((authorizationResponse)queryTransactionResponse.results_max10[0]).response);
            Assert.Equal("Approved", ((authorizationResponse)queryTransactionResponse.results_max10[0]).message);
            Assert.Equal(756027696701750, ((authorizationResponse)queryTransactionResponse.results_max10[0]).cnpTxnId);

            Assert.Equal("000", ((authorizationResponse)queryTransactionResponse.results_max10[1]).response);
            Assert.Equal("Approved", ((authorizationResponse)queryTransactionResponse.results_max10[1]).message);
            Assert.Equal(756027696701751, ((authorizationResponse)queryTransactionResponse.results_max10[1]).cnpTxnId);

            Assert.Equal("000", ((authorizationResponse)queryTransactionResponse.results_max10[1]).response);
            Assert.Equal("Approved", ((authorizationResponse)queryTransactionResponse.results_max10[1]).message);
            Assert.Equal(756027696701751, ((authorizationResponse)queryTransactionResponse.results_max10[1]).cnpTxnId);

            Assert.Equal("000", ((captureResponse)queryTransactionResponse.results_max10[2]).response);
            Assert.Equal("Deposit approved", ((captureResponse)queryTransactionResponse.results_max10[2]).message);

        }

        [Fact]
        public void TestQueryTransactionUnavailableResponse()
        {
            queryTransaction query = new queryTransaction();
            query.id = "myId";
            query.reportGroup = "myReportGroup";
            query.origId = "12345";
            query.origActionType = actionTypeEnum.D;
            query.origCnpTxnId = 54321;

            var mock = new Mock<Communications>();

            mock.Setup(Communications => Communications.HttpPost(It.IsRegex(".*<queryTransaction.*", RegexOptions.Singleline), It.IsAny<Dictionary<String, String>>()))
                .Returns("<cnpOnlineResponse version='10.10' response='0' message='Valid Format' xmlns='http://www.vantivcnp.com/schema'><queryTransactionUnavailableResponse id='FindAuth' reportGroup='Mer5PM1' customerId='1'><response>152</response><responseTime>2015-12-03T14:45:31</responseTime><message>Original transaction found but response not yet available</message></queryTransactionUnavailableResponse></cnpOnlineResponse>");

            Communications mockedCommunication = mock.Object;
            cnp.SetCommunication(mockedCommunication);
            transactionTypeWithReportGroup response = (transactionTypeWithReportGroup)cnp.QueryTransaction(query);
            queryTransactionUnavailableResponse queryTransactionResponse = (queryTransactionUnavailableResponse)response;

            Assert.NotNull(queryTransactionResponse);
            Assert.Equal("152", queryTransactionResponse.response);
        }
    }
}
