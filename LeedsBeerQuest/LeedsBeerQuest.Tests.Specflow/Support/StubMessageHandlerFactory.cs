namespace LeedsBeerQuest.Tests.Specflow.Support
{
    internal static class StubMessageHandlerFactory
    {
        public static StubMessageHandler Create()
        {
            var data = File.ReadAllText("resources/leedsbeerquest.csv");
            return StubMessageHandler.WithStringContent(data);
        }
    }
}
