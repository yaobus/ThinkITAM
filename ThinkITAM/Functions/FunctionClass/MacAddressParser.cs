using SharedImpl = ThinkITAM.Shared.Parsers.MacAddressParser;

namespace ThinkITAM.Functions.FunctionClass
{
    public class MacAddressParser
    {
        public static List<string> ParseMacAddresses(string input)
            => SharedImpl.ParseMacAddresses(input);
    }
}
