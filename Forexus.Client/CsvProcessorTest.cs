using System.Collections.Generic;
using Forexus.BLL;
using Forexus.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forexus.Client
{
    [TestClass]
    public class CsvProcessorTest
    {
        [TestMethod]
        [Description("Lucky test for csv processing. Run me")]
        public void RunCsvProcessingTest()
        {
            //Arange
            const string filesDirectory = "Files";
            var processor = new MessageProcessor();

            //Act
            //NOTE: Processig takes less than 1 second if code in MessageProcessor.cs line 74 is commented. 
            //At this line (line 74) we process all Viewers of message and it could take a while. It is the main place to consider optimisation
            List<Message> messages = processor.Process(filesDirectory);

            //Assert
            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.Count > 800);
            CollectionAssert.AllItemsAreNotNull(messages);
        }
    }
}
