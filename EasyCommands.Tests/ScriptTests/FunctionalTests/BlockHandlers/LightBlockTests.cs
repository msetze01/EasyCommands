﻿using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class LightBlockTests
    {
        [TestMethod]
        public void LightBlockHandlerTest()
        {
            String script = @"
:lightshow
set the ""cool light"" color to ""blue""
set the ""cool light"" intensity to 10
set the ""cool light"" interval to 0.5
set the ""cool light"" offset to 0.25
set the ""cool light"" falloff to 1
set the ""cool light"" length to 2
turn on the ""cool light""
";

            using (ScriptTest test = new ScriptTest(script))
            {
                var mockLight = new Mock<IMyLightingBlock>();
                test.MockBlocksOfType("cool light", mockLight);

                test.RunUntilDone();

                mockLight.VerifySet(b => b.Color = new Color(0, 0, 255));
                mockLight.VerifySet(b => b.Intensity = 10f);
                mockLight.VerifySet(b => b.BlinkIntervalSeconds = 0.5f);
                mockLight.VerifySet(b => b.BlinkOffset = 0.25f);
                mockLight.VerifySet(b => b.Falloff = 1);
                mockLight.VerifySet(b => b.BlinkLength = 2);
                mockLight.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void SetLightBlockColorFromVector() {
            String script = @"
:lightshow
set the ""cool light"" color to ""255:128:0""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockLight = new Mock<IMyLightingBlock>();
                test.MockBlocksOfType("cool light", mockLight);

                test.RunUntilDone();

                mockLight.VerifySet(b => b.Color = new Color(255, 128, 0));
            }
        }

        [TestMethod]
        public void MultipleBlocksSameType()
        {
            String script = @"
:lightshow
set the ""intense light"" intensity to 10
set the ""intense light"" interval to 0.5
set the ""intense light"" offset to 0.25
set the ""intense light"" length to 2
set the ""cool light"" color to ""blue""
set the ""cool light"" falloff to 1
turn on the ""cool light""
turn on the ""intense light""
";

            using (ScriptTest test = new ScriptTest(script))
            {
                var mockCoolLight = new Mock<IMyLightingBlock>();
                var mockIntenseLight = new Mock<IMyLightingBlock>();
                test.MockBlocksOfType("cool light", mockCoolLight);
                test.MockBlocksOfType("intense light", mockIntenseLight);

                test.RunUntilDone();

                mockIntenseLight.VerifySet(b => b.Intensity = 10f);
                mockIntenseLight.VerifySet(b => b.BlinkIntervalSeconds = 0.5f);
                mockIntenseLight.VerifySet(b => b.BlinkOffset = 0.25f);
                mockIntenseLight.VerifySet(b => b.BlinkLength = 2);
                mockCoolLight.VerifySet(b => b.Color = new Color(0, 0, 255));
                mockCoolLight.VerifySet(b => b.Falloff = 1);
                mockCoolLight.VerifySet(b => b.Enabled = true);
                mockIntenseLight.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void BlocksAloneAndInGroups()
        {
            String script = @"
:lightshow
set the ""single light"" intensity to 10
set the ""hangar lights"" interval to 0.5
set the ""hangar lights"" offset to 0.25
turn on the ""hangar lights""
";

            using (ScriptTest test = new ScriptTest(script))
            {
                // In this test, single light and other light are both in
                // a group called "hangar lights"
                // We want to ensure that we're manipulating both lights when we
                // perform commands on the group, but that we can still interact
                // with just one of the lights when we want to
                var mockSingleLight = new Mock<IMyLightingBlock>();
                var mockOtherLight = new Mock<IMyLightingBlock>();
                test.MockBlocksInGroup("hangar lights", mockSingleLight, mockOtherLight);
                test.MockBlocksOfType("other light", mockOtherLight);
                test.MockBlocksOfType("single light", mockSingleLight);

                test.RunUntilDone();

                mockSingleLight.VerifySet(b => b.Intensity = 10f);
                mockSingleLight.VerifySet(b => b.BlinkIntervalSeconds = 0.5f);
                mockSingleLight.VerifySet(b => b.BlinkOffset = 0.25f);
                mockOtherLight.VerifySet(b => b.BlinkIntervalSeconds = 0.5f);
                mockOtherLight.VerifySet(b => b.BlinkOffset = 0.25f);
                mockSingleLight.VerifySet(b => b.Enabled = true);
                mockOtherLight.VerifySet(b => b.Enabled = true);
                // We should have never manipulated the intensity of "other light"
                mockOtherLight.VerifySet(b => b.Intensity = 10f, Times.Never);
            }
        }

        [TestMethod]
        public void UseVariableAsImplicitBlockSelector() {
            String script = @"
assign ""myLights"" to 'test lights'
turn on $myLights
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockLight = new Mock<IMyLightingBlock>();
                test.MockBlocksInGroup("test lights", mockLight);

                test.RunUntilDone();

                mockLight.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void UseVariableAsImplicitBlockSelectorInConditionAndBlockCommand() {
            String script = @"
assign ""myLights"" to 'test lights'
if $myLights are on
  Print 'You Left The Lights On'
  turn off $myLights
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockLight = new Mock<IMyLightingBlock>();
                test.MockBlocksInGroup("test lights", mockLight);
                mockLight.Setup(b => b.Enabled).Returns(true);

                test.RunUntilDone();

                mockLight.VerifySet(b => b.Enabled = false);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("You Left The Lights On", test.Logger[0]);
            }
        }

    }
}
