using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamMoverWPF;
using SteamMoverWPF.Entities;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MainWindow mainWindows = new MainWindow();

            Assert.IsTrue(mainWindows.isSteamRunning());
        }
    }
}
