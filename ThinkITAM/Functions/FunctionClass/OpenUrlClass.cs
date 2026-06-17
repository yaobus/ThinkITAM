using SharedImpl = ThinkITAM.Shared.Helpers.OpenUrlClass;

namespace ThinkITAM.Functions.FunctionClass
{
    public class OpenUrlClass
    {
        public static void OpenUrlInSpecificBrowser(string url, string? browserPath = null)
            => SharedImpl.OpenUrlInSpecificBrowser(url, browserPath);
    }
}
