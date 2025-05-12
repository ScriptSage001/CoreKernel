using CoreKernel.Functional.Validation;

var tester = new ValidateMethodTester();
ValidateMethodTester.TestValidateMethod();

internal class ValidateMethodTester
{
    public static void TestValidateMethod()
    {
        var input = "examplessadnfbkdshfkhsdkfhaksdhfksdahfkhsdkahfksahdkdfhasdkfhkdshkjdshfklhsdkfhdskhfkjdshfkjhdaskfhdsakhfkdashfkhsdkfhsdkfhkdshfkjsdhfkhsdkfhkdshfksdahkfhdsakfhkasdhfkjhkdshfasdhfklexample.com";

        // Define validation rules
        var validationRules = new List<(Func<string, bool> rule, string errorMessage)>
        {
            (value => !string.IsNullOrWhiteSpace(value), "Input cannot be empty."),
            (value => value.Contains("@"), "Input must contain '@'."),
            (value => value.Length <= 50, "Input must not exceed 50 characters.")
        };

        // Validate the input
        var result = Validator.Validate(input, validationRules);

        // Handle the result
        if (result.IsSuccess)
        {
            Console.WriteLine("Validation succeeded: " + result.Value);
        }
        else
        {
            // if (result is IValidationResult validationResult)
            // {
            //     Console.WriteLine("Validation failed with errors:");
            //     foreach (var error in validationResult.Errors)
            //     {
            //         Console.WriteLine($"- {error.Message}");
            //     }
            // }
            // else
            // {
            //     Console.WriteLine($"Validation failed with error - {result.Error.Message}");
            // }
            
            Console.WriteLine("Validation failed with errors:");
            foreach (var error in ((IValidationResult)result).Errors)
            {
                Console.WriteLine($"- {error.Message}");
            }
        }
    }
}