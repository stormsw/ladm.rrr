using System.Diagnostics;
using TechTalk.SpecFlow;

namespace Specifications
{
    /// Looks like it in different threads !!! [Binding]
    public class SpecflowHooks
    {
        [BeforeFeature]
        public static void BeforeFeatureHook()
        {
            var context = SharedContext.GetInstance();

            Trace.TraceInformation("Before Feature entered.");
        }

        [AfterFeature]
        public static void AfterFeatureHook()
        {
            var context = SharedContext.GetInstance();
            //context.Dispose()
            Trace.TraceInformation("After Feature entered.");
        }
        [BeforeScenario]
        public static void BeforeScenario()
        {
            Trace.TraceInformation("Before scenario entered.");
        }
        [AfterScenario]
        public static void AfterScenario()
        {
            Trace.TraceInformation("After scenario entered.");
        }
    }
}
