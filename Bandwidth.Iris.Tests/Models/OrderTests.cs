﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Bandwidth.Iris.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bandwidth.Iris.Tests.Models
{
    [TestClass]
    public class OrderTests
    {
        [TestInitialize]
        public void Setup()
        {
            Helper.SetEnvironmetVariables();
        }

        [TestMethod]
        public void CreateTest()
        {
            var order = new Order
            {
                Name = "Test",
                SiteId = "10",
                CustomerOrderId = "11",
                LataSearchAndOrderType = new LataSearchAndOrderType
                {
                    Lata = "224",
                    Quantity = 1
                }
            };
            var orderResult = new OrderResult
            {
                CompletedQuantity = 1,
                CreatedByUser = "test",
                Order = new Order
                {
                    Name = "Test",
                    SiteId = "10",
                    CustomerOrderId = "11",
                    Id = "101",
                    OrderCreateDate = DateTime.Now
                }
            };
            using (var server = new HttpServer(new RequestHandler
            {
                EstimatedMethod = "POST",
                EstimatedPathAndQuery = string.Format("/v1.0/accounts/{0}/orders", Helper.AccountId),
                EstimatedContent = Helper.ToXmlString(order),
                ContentToSend = Helper.CreateXmlContent(orderResult)
            }))
            {
                var client = Helper.CreateClient();
                var result = Order.Create(client, order).Result;
                if (server.Error != null) throw server.Error;
                Helper.AssertObjects(orderResult, result);
            }
        }

        [TestMethod]
        public void CreateWithXmlTest()
        {
            var order = new Order
            {
                Name = "Test",
                SiteId = "10",
                CustomerOrderId = "11",
                LataSearchAndOrderType = new LataSearchAndOrderType
                {
                    Lata = "224",
                    Quantity = 1
                }
            };
            using (var server = new HttpServer(new RequestHandler
            {
                EstimatedMethod = "POST",
                EstimatedPathAndQuery = string.Format("/v1.0/accounts/{0}/orders", Helper.AccountId),
                EstimatedContent = Helper.ToXmlString(order),
                ContentToSend = new StringContent(TestXmlStrings.ValidOrderResponseXml, Encoding.UTF8, "application/xml")
            }))
            {
                var client = Helper.CreateClient();
                var result = Order.Create(client, order).Result;
                if (server.Error != null) throw server.Error;
                var o = result.Order;
                Assert.AreEqual("1", o.Id);
                Assert.AreEqual("2858", o.SiteId);
                Assert.AreEqual("A New Order", o.Name);
                Assert.AreEqual(DateTime.Parse("2014-10-14T17:58:15.299Z").ToUniversalTime(), o.OrderCreateDate);
                Assert.IsFalse(o.BackOrderRequested);
                Assert.AreEqual("2052865046", o.ExistingTelephoneNumberOrderType.TelephoneNumberList[0]);
                Assert.IsFalse(o.PartialAllowed);
            }
        }

        [TestMethod]
        public void CreateWithDefaultClientTest()
        {
            var order = new Order
            {
                Name = "Test",
                SiteId = "10",
                CustomerOrderId = "11",
                LataSearchAndOrderType = new LataSearchAndOrderType
                {
                    Lata = "224",
                    Quantity = 1
                }
            };
            var orderResult = new OrderResult
            {
                CompletedQuantity = 1,
                CreatedByUser = "test",
                Order = new Order
                {
                    Name = "Test",
                    SiteId = "10",
                    CustomerOrderId = "11",
                    Id = "101",
                    OrderCreateDate = DateTime.Now
                }
            };
            using (var server = new HttpServer(new RequestHandler
            {
                EstimatedMethod = "POST",
                EstimatedPathAndQuery = string.Format("/v1.0/accounts/{0}/orders", Helper.AccountId),
                EstimatedContent = Helper.ToXmlString(order),
                ContentToSend = Helper.CreateXmlContent(orderResult)
            }))
            {
                var result = Order.Create(order).Result;
                if (server.Error != null) throw server.Error;
                Helper.AssertObjects(orderResult, result);
            }
        }
        [TestMethod]
        public void UpdateTest()
        {
            var item = new Order {Id = "101"};
            var data = new Order
            {
                Name = "Test",
                CloseOrder = true
            };
            using (var server = new HttpServer(new RequestHandler
            {
                EstimatedMethod = "PUT",
                EstimatedPathAndQuery = string.Format("/v1.0/accounts/{0}/orders/101", Helper.AccountId),
                EstimatedContent = Helper.ToXmlString(data),
            }))
            {
                item.Update(data).Wait();
                if (server.Error != null) throw server.Error;
            }
        }
    }
}
