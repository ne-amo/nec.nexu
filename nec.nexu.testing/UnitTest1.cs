using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using nec.nexu.Models;
using nec.nexu.bridge;
using Nec.Infrastructure.Messages;

namespace nec.nexu.testing
{
    [TestClass]
    public class UnitTest1
    {





        [TestMethod]
        public void TestMethod1()
        {
        }
        [TestMethod]
        public void BridgeTest()
        {
            try
            {
                Bridge b = new Bridge();
                TicketMessage t = b.GetMaterial(83005384);
                string h = "h";
            }
            catch (Exception ex)
            {
                throw ex;
            }
                 
        }
        [TestMethod]
        public void BridgeCreateTest()
        {
            try
            {
                Bridge b = new Bridge();
                int id = b.Create(83005383);
                string h = "h";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
