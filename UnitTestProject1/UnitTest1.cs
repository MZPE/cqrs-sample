using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PensioenSysteem.Domain.Deelnemer;
using PensioenSysteem.Domain.Deelnemer.Commands;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RegistreerDeelenemerMaaktEenNieuweDeelnemerAan()
        {
            Deelnemer sut = new Deelnemer();

            RegistreerDeelnemerCommand cmd = new RegistreerDeelnemerCommand
            {
                CorrelationId = Guid.NewGuid(),
                EmailAdres = "a@b.com"
            };

            sut.Registreer(cmd);

            Assert.AreEqual(cmd.EmailAdres, sut.emailAdres);
        }
    }
}
