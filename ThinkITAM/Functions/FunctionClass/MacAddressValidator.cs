using SharedImpl = ThinkITAM.Shared.Validators.MacAddressValidator;

namespace ThinkITAM.Functions.FunctionClass
{
    public class MacAddressValidator
    {
        public static string ValidateAndFormatMacAddress(string input)
            => SharedImpl.ValidateAndFormatMacAddress(input);
    }
}
